using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(ProjectileLauncher))]
[RequireComponent(typeof(PathFollower))]
public class BattleshipEnemy : Enemy
{
    public Transform targetingOffset = null;
    public Transform debugFutureTransform = null;

    protected override void OnKill()
    {
        base.OnKill();

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

        Initialize(40.0f, 4, 20);

        // Set battleship death explosion force
        explosionForce = 1000.0f;
        explosionRadius = 10.0f;
        upwardsExplosionModifier = 2.0f;

        //if (debugFutureTransform != null)
        //{
        //    (Vector3, Quaternion) futurePositionRotation = GetNextTransform(30.0f);
        //    Debug.Log("Future position: " + futurePositionRotation.Item1 + ", rotation: " + futurePositionRotation.Item2);
        //    debugFutureTransform.position = futurePositionRotation.Item1;
        //    debugFutureTransform.rotation = futurePositionRotation.Item2;
        //}
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
