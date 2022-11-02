using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    public float shockwaveSpeed = 1.0f;
    public float explosionForce = 500.0f;
    public float explosionRadius = 5.0f;
    public float lifetime = 7.5f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        Vector3 shockwaveStep = transform.forward * (shockwaveSpeed * Time.deltaTime);
        transform.position = transform.position + shockwaveStep;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.gameObject.TryGetComponent(out Enemy enemy)) // Handle Enemy collision
        {
            List<GameObject> enemyParts = new List<GameObject>();
            MeshRenderer[] meshParts = enemy.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mesh in meshParts)
            {
                enemyParts.Add(mesh.gameObject);
            }

            enemy.Kill();

            // Apply explosion force to each piece of enemy with a rigidbody.
            foreach (GameObject enemyPart in enemyParts)
            {
                if (enemyPart.TryGetComponent(out Rigidbody enemyPartBody))
                {
                    enemyPartBody.AddExplosionForce(explosionForce, transform.position, explosionRadius, 4.0f);
                }
            }
        }
        else // Handle Non-enemy collision
        {
            if (other.attachedRigidbody != null)
            {
                other.attachedRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius, 4.0f);
            }
        }
    }
}
