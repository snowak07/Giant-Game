using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactExplosion : MonoBehaviour
{
    public float maxImpactPower = 50.0f;
    public float explosionRadius = 30.0f;

    protected void OnCollisionEnter(Collision collision)
    {
        // Apply explosion force to any enemies in the area
        if (collision.transform.root.gameObject.tag == "Water")
        {
            Collider[] nearbyEnemyColliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach(Collider collider in nearbyEnemyColliders)
            {
                if (collider.transform.root.gameObject.tag == "Enemy")
                {
                    Rigidbody body = collider.transform.root.GetComponentInChildren<Rigidbody>();

                    if (body != null)
                    {
                        body.AddExplosionForce(maxImpactPower, transform.position, explosionRadius, 3.0f, ForceMode.Impulse);
                    }
                }
            }
        }
    }
}
