using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(ProjectileLauncher))]
public class BattleshipEnemy : Enemy
{
    protected override void OnKill()
    {
        base.OnKill();

        SetHullMass(3);
    }

    private void SetHullMass(float mass)
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

                body.mass = mass;
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

        Initialize(40.0f, 4, 20, false, false, false, 1000.0f, 10.0f, 2.0f);
    }

    protected override void UpdateEnemy()
    {
        base.UpdateEnemy();

        if (BoatHasTippedOver())
        {
            Kill();
        }

        if (targetTransform != null)
        {
            Vector3 firingDirection = GetComponent<ProjectileLauncher>().ShootProjectile(targetTransform.position);
            UpdateCannonPosition(firingDirection);
        }
    }

    private bool BoatHasTippedOver()
    {
        return 
            (transform.rotation.eulerAngles.x > 90 && transform.rotation.eulerAngles.x < 270) ||
            (transform.rotation.eulerAngles.z > 90 && transform.rotation.eulerAngles.z < 270);
    }

    private void UpdateCannonPosition(Vector3 firingDirection)
    {
        if (firingDirection != Vector3.zero)
        {
            GetComponentInChildren<CannonTurn>().setFiringDirection(firingDirection);
        }
    }
}
