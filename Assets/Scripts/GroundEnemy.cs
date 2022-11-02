using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO Allow for uneven terrain movement.Perhaps disable gravity when moving, enable gravity after collision with player, once enemy comes to rest, disable gravity again showing a "Get Up" animation and then continuing to move on.
[RequireComponent(typeof(Rigidbody))]
public class GroundEnemy : Enemy
{
    public float moveSpeedPerSecond = 1.0f;
    public float attackRange = 0.5f;
    public float velocityGetUpMovementThreshold = 0.05f;

    private bool knockedDown = false;

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

    protected override void OnCollisionEnter(Collision collision)
    {
        // Disable movement until
        if (collision.transform.tag == "Player" || collision.transform.root.TryGetComponent(out Shockwave shockwave) || (collision.transform.root.TryGetComponent(out GiantGrabInteractable interactable) && !interactable.impactCooldownEnabled()))
        {
            knockedDown = true;
        }

        base.OnCollisionEnter(collision);
    }

    protected bool EnemyStoppedMoving()
    {
        Rigidbody body = GetComponent<Rigidbody>();

        if (body.velocity.magnitude < velocityGetUpMovementThreshold)
        {
            return true;
        }

        return false;
    }
}
