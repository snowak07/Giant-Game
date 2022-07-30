using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public Transform playerTransform = null;
    public float speed = 1;
    public Rigidbody projectile = null;
    public float firingSpeed = 1;
    public float verticalOscillationsPerRotation = 4;
    public float verticalOscillationMagnitude = 0.5f;

    private Rigidbody body;
    private bool isKilled = false;

    private float timeCounter;
    private float horizontalMaxDistance;
    private float initialYHeight;
    private float xPlayerOffset;
    private float zPlayerOffset;

    private bool firingCooldown;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        float initialTimeOffset = Mathf.Atan2(transform.position.z, transform.position.x);
        timeCounter = initialTimeOffset;
        initialYHeight = transform.position.y;
        xPlayerOffset = playerTransform.position.x;
        zPlayerOffset = playerTransform.position.z;
        horizontalMaxDistance = Mathf.Sqrt(Mathf.Pow(transform.position.x, 2) + Mathf.Pow(transform.position.z, 2));
    }

    void FixedUpdate()
    {
        // TODO Make the FlyingEnemy move towards the player before flying around them at a constant speed. Possibly use a random range to fly around in. Could break off, fly in random direction, then continue circling again.

        // TODO Make the FlyingEnemy bob up and down in the y direction both when circling and beelining for the player.

        // TODO Adding health and visual damage indicators on the model would be interesting. Need to throw harder to do more damage. Enemies get bonked back but don't die. Are taken out the fight briefly though.

        if (!isKilled)
        {
            // Shooting projectile at the player when they are circling.
            StartCoroutine(ShootProjectileAtPlayer());

            timeCounter += (speed * (Time.deltaTime / horizontalMaxDistance));
            float y = initialYHeight + (verticalOscillationMagnitude * Mathf.Sin(verticalOscillationsPerRotation * timeCounter));
            float x = (horizontalMaxDistance * Mathf.Cos(timeCounter));// + xPlayerOffset; // TODO I think these are necessary to properly orbit around the player.
            float z = (horizontalMaxDistance * Mathf.Sin(timeCounter));// + zPlayerOffset;
            Vector3 newPosition = new Vector3(x, y, z);
            body.MovePosition(newPosition); // TODO movement appears jittery. Can I use forces instead? Use normal Update() function instead?
        }
    }

    void Shoot()
    {
        Rigidbody p = Instantiate(projectile, transform.position, transform.rotation);
        Vector3 directDistanceVector = playerTransform.position - transform.position;

        // Calculate Velocity y vector needed to be at the pull y position in 1 second
        float y = directDistanceVector.y + (0.5f * Mathf.Abs(Physics.gravity.y));

        Vector3 projectileVector = new Vector3(directDistanceVector.x, y, directDistanceVector.z);

        p.AddForce(projectileVector - p.velocity, ForceMode.VelocityChange);
    }

    IEnumerator ShootProjectileAtPlayer()
    {
        if (!firingCooldown)
        {
            firingCooldown = true;
            Shoot();
            yield return new WaitForSeconds(firingSpeed);
            firingCooldown = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isKilled = true;
        body.useGravity = true;
    }
}
