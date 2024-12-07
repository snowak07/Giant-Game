using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO Allow for uneven terrain movement.Perhaps disable gravity when moving, enable gravity after collision with player, once enemy comes to rest, disable gravity again showing a "Get Up" animation and then continuing to move on.
[RequireComponent(typeof(Rigidbody))]
public class GroundEnemy : Enemy
{
    public float moveSpeedPerSecond = 1.0f;
    public float attackRange = 1.5f;
    public float projectileSpeed = 3.0f;
    public float velocityGetUpMovementThreshold = 0.05f;

    private bool knockedDown = false;

    protected override void Start()
    {
        base.Start();
        Initialize(10.0f, 2, 20);
    }

    protected override void UpdateEnemy()
    {
        // Move towards player until within striking distance
        Vector3 playerPosition = playerTransform.position;
        Vector3 targetPosition = playerPosition;
        targetPosition.y = transform.position.y; // TODO This assumes ground enemies are moving on flat ground towards enemies.

        if (!knockedDown)
        {
            // If in striking distance than stay still and attack player on cooldown. Check minimum distance or collision with player lower body.
            if ((targetPosition - transform.position).magnitude < attackRange)
            {
                // Attack
                Rigidbody body = GetComponent<Rigidbody>();

                Vector3 projectileVector = calculateFiringDirection(transform.position, playerBodyTransform.position);

                if (projectileVector != Vector3.zero)
                {
                    body.AddForce(projectileVector - body.linearVelocity, ForceMode.VelocityChange);

                    knockedDown = true;
                }
            }
            else
            {
                Rigidbody body = GetComponent<Rigidbody>();

                // Rotate and move towards target so that it is standing up facing the player
                Vector3 towardsTarget = targetPosition - transform.position;
                Quaternion newRotation = Quaternion.LookRotation(towardsTarget);
                body.MoveRotation(newRotation);

                Vector3 moveStep = transform.forward * (moveSpeedPerSecond * Time.fixedDeltaTime);
                body.MovePosition(transform.position + moveStep);
            }
        }
        else
        {
            // Check if enemy is on the ground and at a standstill
            if (EnemyStoppedMoving())
            {
                knockedDown = false;
            }
        }
    }

    public override (Vector3, Quaternion) GetNextTransform(float time, bool applyTargetOffset = false)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        // Disable movement until
        if (collision.transform.tag == "Giant" || collision.transform.root.TryGetComponent(out Shockwave shockwave) || (collision.transform.root.TryGetComponent(out GiantGrabInteractable interactable) && !interactable.ImpactCooldown))
        {
            knockedDown = true;
        }

        base.OnCollisionEnter(collision);
    }

    protected bool EnemyStoppedMoving()
    {
        Rigidbody body = GetComponent<Rigidbody>();

        if (body.linearVelocity.magnitude < velocityGetUpMovementThreshold)
        {
            return true;
        }

        return false;
    }

    public Vector3 calculateFiringDirection(Vector3 firingPosition, Vector3 firingTargetLocation)
    {
        float t = calculateTimeToImpact(firingPosition, firingTargetLocation);

        if (t != float.PositiveInfinity)
        {
            Vector3 directDistanceVector = firingTargetLocation - firingPosition;

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

    private float calculateTimeToImpact(Vector3 firingPosition, Vector3 firingTargetLocation)
    {
        float t1 = calculateTimeToImpact1(firingPosition, firingTargetLocation);
        float t2 = -t1;
        float t3 = calculateTimeToImpact2(firingPosition, firingTargetLocation);
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

    private float calculateTimeToImpact1(Vector3 firingPosition, Vector3 firingTargetLocation)
    {
        float y = firingTargetLocation.y - firingPosition.y;
        Vector3 horizontal = firingTargetLocation - firingPosition;
        horizontal.y = 0;
        float xz = horizontal.magnitude;

        return Mathf.Sqrt(2 * (Mathf.Pow(projectileSpeed, 2) + Mathf.Sqrt(Mathf.Pow(projectileSpeed, 4) - 2 * Mathf.Pow(projectileSpeed, 2) * y * Physics.gravity.y - Mathf.Pow(xz * Physics.gravity.y, 2)) - y * Physics.gravity.y)) / Physics.gravity.y;
    }

    private float calculateTimeToImpact2(Vector3 firingPosition, Vector3 firingTargetLocation)
    {
        float y = firingTargetLocation.y - firingPosition.y;
        Vector3 horizontal = firingTargetLocation - firingPosition;
        horizontal.y = 0;
        float xz = horizontal.magnitude;

        return Mathf.Sqrt(2 * (Mathf.Pow(projectileSpeed, 2) - Mathf.Sqrt(Mathf.Pow(projectileSpeed, 4) - 2 * Mathf.Pow(projectileSpeed, 2) * y * Physics.gravity.y - Mathf.Pow(xz * Physics.gravity.y, 2)) - y * Physics.gravity.y)) / Physics.gravity.y;
    }
}
