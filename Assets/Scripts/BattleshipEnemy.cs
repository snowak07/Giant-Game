using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    public Vector3 CalculateFiringDirection(Vector3 firingPosition)
    {
        float t = CalculateTimeToImpact(firingPosition);

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

    // TODO: Move to helper class
    private float CalculateTimeToImpact(Vector3 firingPosition)
    {
        float t1 = CalculateTimeToImpact1(firingPosition);
        float t2 = -t1;
        float t3 = CalculateTimeToImpact2(firingPosition);
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

    // TODO: Move to helper class
    private float CalculateTimeToImpact1(Vector3 firingPosition)
    {
        float y = playerTransform.position.y - firingPosition.y;
        Vector3 horizontal = playerTransform.position - firingPosition;
        horizontal.y = 0;
        float xz = horizontal.magnitude;

        return Mathf.Sqrt(2 * (Mathf.Pow(projectileSpeed, 2) + Mathf.Sqrt(Mathf.Pow(projectileSpeed, 4) - 2 * Mathf.Pow(projectileSpeed, 2) * y * Physics.gravity.y - Mathf.Pow(xz * Physics.gravity.y, 2)) - y * Physics.gravity.y)) / Physics.gravity.y;
    }

    // TODO: Move to helper class
    private float CalculateTimeToImpact2(Vector3 firingPosition)
    {
        float y = playerTransform.position.y - firingPosition.y;
        Vector3 horizontal = playerTransform.position - firingPosition;
        horizontal.y = 0;
        float xz = horizontal.magnitude;

        return Mathf.Sqrt(2 * (Mathf.Pow(projectileSpeed, 2) - Mathf.Sqrt(Mathf.Pow(projectileSpeed, 4) - 2 * Mathf.Pow(projectileSpeed, 2) * y * Physics.gravity.y - Mathf.Pow(xz * Physics.gravity.y, 2)) - y * Physics.gravity.y)) / Physics.gravity.y;
    }

    public override void Kill()
    {
        // Find "Hull" child object
        int i = 0;
        while (i < transform.childCount)
        {
            GameObject child = transform.GetChild(i++).gameObject;
            if (child.transform.tag == "Hull")
            {
                // Add heavier weight to the shattered Hull piece
                Rigidbody body;
                if (!child.TryGetComponent(out body))
                {
                    body = child.AddComponent<Rigidbody>();
                }

                body.mass = 3;
                break;
            }
        }

        base.Kill();
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
    protected override void SetStartingHealth()
    {
        health = 40;
    }

    void Shoot(Vector3 firingPosition, Vector3 projectileVector)
    {
        if (!projectileVector.Equals(Vector3.zero))
        {
            GameObject projectileObject = Instantiate(projectile, firingPosition, transform.rotation);

            // Enable impact cooldown so the projecile doesn't collide with the Enemy
            GiantGrabInteractable ep = projectileObject.GetComponent<GiantGrabInteractable>();
            ep.ImpactCooldown = true;

            // Get rigidbody and add force and torque
            Rigidbody p = projectileObject.GetComponent<Rigidbody>();
            p.AddForce(projectileVector - p.linearVelocity, ForceMode.VelocityChange);
            p.AddTorque(Random.insideUnitSphere * 3.0f);
        }
    }

    IEnumerator ShootProjectileAtPlayer()
    {
        // Calculate projectileVector early so that cannon can have correct orientation at each update
        Vector3 firingPosition = firingOffset.position;
        Vector3 projectileVector = CalculateFiringDirection(firingPosition);
        UpdateCannonPosition(projectileVector);

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
    protected override void Start()
    {
        base.Start();

        // Set battleship death explosion force
        explosionForce = 1000.0f;
        explosionRadius = 10.0f;
        upwardsExplosionModifier = 2.0f;

        // Determine random orbit radius that will determine the "desired" path.
        orbitRadius = orbitRadiusStepSize * Random.Range(minRandonizedOrbitRadius, maxRandomizedOrbitRadius);
    }

    protected override void UpdateEnemy()
    {
        // Check if boat has tipped over and is in water
        if (alive && inWater && ((transform.rotation.eulerAngles.x > 90 && transform.rotation.eulerAngles.x < 270) || (transform.rotation.eulerAngles.z > 90 && transform.rotation.eulerAngles.z < 270)))
        {
            Kill();
        }

        if (alive && playerTransform != null)
        {
            Vector3 currentPathPosition = (transform.position - playerTransform.position).normalized * orbitRadius; // Assumes that the center is playerTransform
            float timeCountCurrent = Mathf.Atan2(currentPathPosition.z, currentPathPosition.x);
            float desiredPositionTimeCount = timeCountCurrent + desiredPositionLeadingAngleDegrees * Mathf.PI / 180;

            Vector3 desiredPosition = new Vector3(orbitRadius * Mathf.Cos(desiredPositionTimeCount) + playerTransform.position.x, transform.position.y, orbitRadius * Mathf.Sin(desiredPositionTimeCount) + playerTransform.position.x);

            Vector3 towardsDesiredPosition = desiredPosition - transform.position;
            Quaternion desiredRotation = Quaternion.LookRotation(towardsDesiredPosition, Vector3.up);
            Quaternion newDirection = Quaternion.RotateTowards(transform.rotation, desiredRotation, maxRotationalSpeed * Time.deltaTime);

            GetComponent<Rigidbody>().MoveRotation(newDirection);

            Vector3 newPosition = (maxTranslationalSpeed * Time.deltaTime) * transform.forward + transform.position;

            GetComponent<Rigidbody>().MovePosition(newPosition);

            StartCoroutine(ShootProjectileAtPlayer());
        }
    }

    private void UpdateCannonPosition(Vector3 firingDirection)
    {
        GetComponentInChildren<CannonTurn>().setFiringDirection(firingDirection);
    }
}
