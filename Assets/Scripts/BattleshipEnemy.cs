using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleshipEnemy : Enemy
{
    public int maxRandomizedOrbitRadius = 300;
    public int minRandonizedOrbitRadius = 150;
    public float orbitRadiusStepSize = 0.1f;

    public float maxRotationalSpeed = 20; // Measured in units/s
    public float maxTranslationalSpeed = 2; // Measured in units/s

    public float desiredPositionLeadingAngleDegrees = 10;

    public Transform firingOffset = null;

    public GameObject projectile = null;
    public float projectileSpeed = 3.0f;
    public float firingCooldownTime = 1;

    private float orbitRadius;

    private bool firingCooldown = false;

    private bool inWater = false;
    private bool isSinking = false;
    private float sinkingSpeed = 0.002f;

    private float settledVelocityThreshold = 0.01f;
    private float settledAngularVelocityThreshold = 0.005f;

    void Start()
    {
        // Determine random orbit radius that will determine the "desired" path.
        orbitRadius = orbitRadiusStepSize * Random.Range(minRandonizedOrbitRadius, maxRandomizedOrbitRadius);

        damageThreshold = 40;
    }

    protected override void UpdateEnemy()
    {
        // Check if boat has tipped over and is in water
        if (!isKilled && inWater && ((transform.rotation.eulerAngles.x > 90 && transform.rotation.eulerAngles.x < 270) || (transform.rotation.eulerAngles.z > 90 && transform.rotation.eulerAngles.z < 270)))
        {
            Kill();
        }

        if (!isKilled && playerTransform != null)
        {
            Vector3 currentPathPosition = (transform.position - playerTransform.position).normalized * orbitRadius; // Assumes that the center is playerTransform
            float timeCountCurrent = Mathf.Atan2(currentPathPosition.z, currentPathPosition.x);
            float desiredPositionTimeCount = timeCountCurrent + desiredPositionLeadingAngleDegrees * Mathf.PI / 180;

            Vector3 desiredPosition = new Vector3(orbitRadius * Mathf.Cos(desiredPositionTimeCount) + playerTransform.position.x, transform.position.y, orbitRadius * Mathf.Sin(desiredPositionTimeCount) + playerTransform.position.x);

            Vector3 towardsDesiredPosition = desiredPosition - transform.position;
            Vector3 upwards = new Vector3(0, 1, 0);
            Quaternion desiredRotation = Quaternion.LookRotation(towardsDesiredPosition, upwards);
            Quaternion newDirection = Quaternion.RotateTowards(transform.rotation, desiredRotation, maxRotationalSpeed * Time.deltaTime);

            Quaternion modelOffset = Quaternion.FromToRotation(new Vector3(-1, 0, 0), new Vector3(0, 0, 1));

            GetComponent<Rigidbody>().MoveRotation(newDirection);

            Vector3 newPosition = (maxTranslationalSpeed * Time.deltaTime) * transform.forward + transform.position;

            GetComponent<Rigidbody>().MovePosition(newPosition);

            StartCoroutine(ShootProjectileAtPlayer());
        }

        if (isKilled && EnemySettled(GetComponent<Rigidbody>()))
        {
            isSinking = true;
        }

        if (isSinking)
        {
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.isTrigger = true;
            }

            Rigidbody body = GetComponent<Rigidbody>();
            body.velocity = new Vector3(0, 0, 0);
            body.angularVelocity = new Vector3(0, 0, 0);
            body.useGravity = false;

            Vector3 newPosition = new Vector3(transform.position.x, transform.position.y - sinkingSpeed, transform.position.z);
            body.MovePosition(newPosition);
        }
    }

    protected override List<GameObject> DismantleEnemy()
    {
        List<GameObject> childObjects = base.DismantleEnemy();
        
        StartCoroutine(EnableSink(childObjects));
        return childObjects;
    }

    IEnumerator EnableSink(List<GameObject> objects)
    {
        yield return new WaitForSeconds(0.1f);

        foreach (var childObject in objects)
        {
            Sinkable sinkable = childObject.AddComponent<Sinkable>();
            sinkable.EnableSink();
        }
    }

    protected bool EnemySettled(Rigidbody body)
    {
        if (body.velocity.magnitude < settledVelocityThreshold && body.angularVelocity.magnitude < settledAngularVelocityThreshold)
        {
            return true;
        }

        return false;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (collision.transform.root.gameObject.tag == "Water")
        {
            inWater = true;
            ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
            ps.Play();
        }
    }

    protected void OnCollisionExit(Collision collision)
    {
        if (collision.transform.root.gameObject.tag == "Water")
        {
            inWater = false;
            ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
            ps.Stop();
        }
    }

    public override void Kill()
    {
        // Find "Hull" child object
        int i = 0;
        GameObject hull = null;
        while (i < transform.childCount)
        {
            GameObject child = transform.GetChild(i++).gameObject;
            if (child.transform.tag == "Hull")
            {
                hull = child;
            }
        }

        base.Kill();

        // Add heavier weight to the shattered Hull piece
        if (hull != null && hull.TryGetComponent(out Rigidbody body))
        {
            body.mass = 10;
        }
    }

    void Shoot(Vector3 firingPosition, Vector3 projectileVector)
    {
        if (!projectileVector.Equals(Vector3.zero))
        {
            GameObject projectileObject = Instantiate(projectile, firingPosition, transform.rotation);

            // Enable impact cooldown so the projecile doesn't collide with the Enemy
            GiantGrabInteractable ep = projectileObject.GetComponent<GiantGrabInteractable>();
            ep.enableImpactCooldown();

            // Get rigidbody and add force and torque
            Rigidbody p = projectileObject.GetComponent<Rigidbody>();
            p.AddForce(projectileVector - p.velocity, ForceMode.VelocityChange);
            p.AddTorque(Random.insideUnitSphere * 3.0f);
        }
    }

    IEnumerator ShootProjectileAtPlayer()
    {
        // Calculate projectileVector early so that cannon can have correct orientation at each update
        Vector3 firingPosition = firingOffset.position;
        Vector3 projectileVector = calculateFiringDirection(firingPosition);
        updateCannonPosition(projectileVector);

        if (!firingCooldown)
        {
            firingCooldown = true;
            Shoot(firingPosition, projectileVector);
            yield return new WaitForSeconds(firingCooldownTime);
            firingCooldown = false;
            yield break;
        }

        yield break;
    }

    public Vector3 calculateFiringDirection(Vector3 firingPosition)
    {
        float t = calculateTimeToImpact(firingPosition);

        if (t != float.PositiveInfinity)
        {
            Vector3 directDistanceVector = playerTransform.position - firingPosition;

            float y = (directDistanceVector.y + (0.5f * Mathf.Abs(Physics.gravity.y) * Mathf.Pow(t, 2))) / t;
            float x = directDistanceVector.x / t;
            float z = directDistanceVector.z / t;

            Vector3 projectileVector = new Vector3(x, y, z);
            return projectileVector;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private float calculateTimeToImpact(Vector3 firingPosition)
    {
        float t1 = calculateTimeToImpact1(firingPosition);
        float t2 = -t1;
        float t3 = calculateTimeToImpact2(firingPosition);
        float t4 = -t3;

        float t = float.PositiveInfinity;
        if (!float.IsNaN(t1) && t1 > 0 && t1 < t)
        {
            t = t1;
        }

        if (!float.IsNaN(t2) && t2 > 0 && t2 < t)
        {
            t = t2;
        }

        if (!float.IsNaN(t3) && t3 > 0 && t3 < t)
        {
            t = t3;
        }

        if (!float.IsNaN(t4) && t4 > 0 && t4 < t)
        {
            t = t4;
        }

        return t;
    }

    private float calculateTimeToImpact1(Vector3 firingPosition)
    {
        float y = playerTransform.position.y - firingPosition.y;
        Vector3 horizontal = playerTransform.position - firingPosition;
        horizontal.y = 0;
        float xz = horizontal.magnitude;

        return Mathf.Sqrt(2 * (Mathf.Pow(projectileSpeed, 2) + Mathf.Sqrt(Mathf.Pow(projectileSpeed, 4) - 2 * Mathf.Pow(projectileSpeed, 2) * y * Physics.gravity.y - Mathf.Pow(xz * Physics.gravity.y, 2)) - y * Physics.gravity.y)) / Physics.gravity.y;
    }

    private float calculateTimeToImpact2(Vector3 firingPosition)
    {
        float y = playerTransform.position.y - firingPosition.y;
        Vector3 horizontal = playerTransform.position - firingPosition;
        horizontal.y = 0;
        float xz = horizontal.magnitude;

        return Mathf.Sqrt(2 * (Mathf.Pow(projectileSpeed, 2) - Mathf.Sqrt(Mathf.Pow(projectileSpeed, 4) - 2 * Mathf.Pow(projectileSpeed, 2) * y * Physics.gravity.y - Mathf.Pow(xz * Physics.gravity.y, 2)) - y * Physics.gravity.y)) / Physics.gravity.y;
    }

    private void updateCannonPosition(Vector3 firingDirection)
    {
        GetComponentInChildren<CannonTurn>().setFiringDirection(firingDirection);
    }
}
