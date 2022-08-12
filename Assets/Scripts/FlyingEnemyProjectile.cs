using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyProjectile : MonoBehaviour
{
    // TODO add projectile destroy after collision or after 10 seconds have passed.
    // Start is called before the first frame update
    public ParticleSystem explosion = null;

    private float numTimesCollided = 0;
    private float startTime;
    private readonly float lifespan = 10;

    private void Start()
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

    private void OnCollisionEnter(Collision collision)
    {
        numTimesCollided += 1;
        if (collision.collider.tag != "Enemy")
        {
            //ParticleSystem explosion = GetComponentInChildren<ParticleSystem>();
            ParticleSystem exp = Instantiate(explosion, transform.position, transform.rotation);
            exp.Play();
            Destroy(exp, exp.main.duration);
            Destroy(gameObject);
        }
    }
}
