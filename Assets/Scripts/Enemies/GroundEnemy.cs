using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO Allow for uneven terrain movement.Perhaps disable gravity when moving, enable gravity after collision with player, once enemy comes to rest, disable gravity again showing a "Get Up" animation and then continuing to move on.
[RequireComponent(typeof(ProjectileLauncher))]
public class GroundEnemy : Enemy
{
    public float velocityGetUpMovementThreshold = 0.05f;

    private bool knockedDown = false;

    protected override void Start()
    {
        base.Start();
        Initialize(10.0f, 2, 40);
    }

    protected override void UpdateEnemy()
    {
        base.UpdateEnemy();

        if (!knockedDown)
        {
            if (targetTransform != null) // && !GetComponent<ProjectileLauncher>().onCooldown
            {
                GetComponent<ProjectileLauncher>().ShootProjectile(targetTransform.position);
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
        if (collision.transform.tag == "Giant" || collision.transform.root.TryGetComponent(out Shockwave shockwave) || (collision.transform.root.TryGetComponent(out GiantGrabInteractable interactable) && !interactable.impactCooldown))
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
}
