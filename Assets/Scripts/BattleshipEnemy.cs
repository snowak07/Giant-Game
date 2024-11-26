using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(ProjectileLauncher))]
public class BattleshipEnemy : Enemy
{
    public int maxRandomizedOrbitRadius = 300;
    public int minRandonizedOrbitRadius = 150;
    public float orbitRadiusStepSize = 0.1f;

    public float maxRotationalSpeed = 20; // Measured in units/s
    public float maxTranslationalSpeed = 2; // Measured in units/s

    public float desiredPositionLeadingAngleDegrees = 10;

    private float orbitRadius;

    /**
     * Assign starting health
     * 
     * @param float         health
     */
    protected BattleshipEnemy() : base(40.0f) 
    {
        // Set battleship death explosion force
        explosionForce = 1000.0f;
        explosionRadius = 10.0f;
        upwardsExplosionModifier = 2.0f;
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
            ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
            ps.Play();
        }
    }

    protected void OnCollisionExit(Collision collision)
    {
        if (collision.transform.root.gameObject.tag == "Water")
        {
            ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
            ps.Stop();
        }
    }

    protected override void Start()
    {
        base.Start();

        // Determine random orbit radius that will determine the "desired" path.
        orbitRadius = orbitRadiusStepSize * Random.Range(minRandonizedOrbitRadius, maxRandomizedOrbitRadius);
    }

    protected override Transform GetNextTransform(float time)
    {
        float timeRemainingToSimulate = time;
        Transform currentTransform = transform;

        while (timeRemainingToSimulate > 0)
        {
            Vector3 currentPathPosition = currentTransform.position.normalized * orbitRadius;
            float timeCountCurrent = Mathf.Atan2(currentPathPosition.z, currentPathPosition.x);
            float desiredPositionTimeCount = timeCountCurrent + desiredPositionLeadingAngleDegrees * Mathf.PI / 180;

            Vector3 desiredPosition = new Vector3(orbitRadius * Mathf.Cos(desiredPositionTimeCount), currentTransform.position.y, orbitRadius * Mathf.Sin(desiredPositionTimeCount));

            Vector3 towardsDesiredPosition = desiredPosition - currentTransform.position;
            Quaternion desiredRotation = Quaternion.LookRotation(towardsDesiredPosition, Vector3.up);
            Quaternion newDirection = Quaternion.RotateTowards(currentTransform.rotation, desiredRotation, maxRotationalSpeed * Time.deltaTime);

            currentTransform.rotation = newDirection;

            Vector3 newPosition = (maxTranslationalSpeed * Time.deltaTime) * currentTransform.forward + currentTransform.position;

            currentTransform.position = newPosition;

            timeRemainingToSimulate -= Time.fixedDeltaTime;
        }

        return currentTransform;
    }

    protected override void UpdateEnemy()
    {
        base.UpdateEnemy();

        // Check if the boat has tipped over
        if ((transform.rotation.eulerAngles.x > 90 && transform.rotation.eulerAngles.x < 270) || (transform.rotation.eulerAngles.z > 90 && transform.rotation.eulerAngles.z < 270))
        {
            Kill();
        }

        if (playerTransform != null)
        {
            Vector3 firingDirection = GetComponent<ProjectileLauncher>().ShootProjectile(playerTransform.position);
            UpdateCannonPosition(firingDirection);
        }
    }

    private void UpdateCannonPosition(Vector3 firingDirection)
    {
        GetComponentInChildren<CannonTurn>().setFiringDirection(firingDirection);
    }
}
