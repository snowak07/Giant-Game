using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    // TODO add projectile destroy after collision or after 10 seconds have passed.
    // Start is called before the first frame update
    public GameObject explosion = null;
    public float explosionRadius = 5.0f;
    public float explosionPower = 100.0f;
    public float killRadius = 2.0f;

    private float startTime;
    private readonly float lifespan = 15;

    void Start()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - startTime > lifespan)
        {
            Destroy(gameObject);
        }
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (
            collision.collider.tag == "Giant" && 
            Helpers.TryGetComponentInParent(collision.gameObject, out NewActionBasedXRController controller) && 
            collision.gameObject.GetComponentInParent<NewActionBasedXRController>().PickupEnabled()
        ) {
            // Change layer off Enemy layer when interacted with by the player so that it can damage enemies afterward
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
        else
        {
            // Create LayerMask that doesn't interact with itself
            int layer = gameObject.layer;
            int layerMask = ~(1 << layer);

            // Check if Enemies and in explosion radius and if so kill them
            Collider[] explosionKills = Physics.OverlapSphere(transform.position, killRadius, layerMask);
            foreach (var explosionHit in explosionKills)
            {
                if (explosionHit.transform.root.gameObject.TryGetComponent(out Enemy enemy))
                {
                    enemy.Kill(gameObject);
                }
            }

            // Check if Enemies and in explosion radius and if so kill them
            Collider[] explosionHits = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);
            foreach (var explosionHit in explosionHits)
            {
                if (explosionHit.transform.root.gameObject.TryGetComponent(out Rigidbody body) && explosionHit.transform.tag != "Giant")
                {
                    body.AddExplosionForce(explosionPower, transform.position, explosionRadius, 3.0f);
                }
            }

            // Trigger explosion effect and destroy object
            GameObject explosionPrefab = Instantiate(explosion, transform.position, Quaternion.identity);
            explosionPrefab.GetComponent<ExplosionHandler>().Explode();
            Destroy(gameObject);
        }
    }
}
