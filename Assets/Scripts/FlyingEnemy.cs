using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO add flame trail particle effect to projectiles

// TODO Possibly use a random range to fly around in. Could break off, fly in random direction, then continue circling again.

// TODO Make the FlyingEnemy bob up and down in the y direction when beelining for the player. (Might not do this?)

// TODO Adding health and visual damage indicators on the model would be interesting. Need to throw harder to do more damage. Enemies get bonked back but don't die. Are taken out the fight briefly though.

// TODO Add endless spawning of FlyingEnemy

// TODO Vary maxRange for each new enemy

public class FlyingEnemy : Enemy
{
    public float orbitSpeed = 1;
    public float radialSpeed = 0.1f;
    public Rigidbody projectile = null;
    public float firingSpeed = 1;
    public float verticalOscillationsPerRotation = 4;
    public float verticalOscillationMagnitude = 0.5f;

    private Rigidbody body;

    private float timeCounter;
    private float horizontalMaxDistance;
    private float xPlayerOffset;
    private float zPlayerOffset;
    private float yIntercept;
    private float xIntercept;
    private float zIntercept;

    private bool isSelected;
    private float maxRange;
    private bool clockwiseOrbit;
    private bool firingCooldown;
    private bool enteredRange;

    protected override void Start()
    {
        base.Start();

        maxRange = Random.Range(0, 5) + 10;
        clockwiseOrbit = Random.Range(0, 2) == 0;
        enteredRange = false;
        body = GetComponent<Rigidbody>();
        float initialTimeOffset = Mathf.Atan2(transform.position.z, transform.position.x);

        // Set time such that position on rotation circle is the same point that the FlyingEnemy enters range
        timeCounter = initialTimeOffset;
    }

    protected override void UpdateEnemy()
    {
        if (isSelected)
        {
            enteredRange = false;
        }

        if (!isKilled && playerTransform != null && !isSelected)
        {
            bool inRange = (playerTransform.position - transform.position).magnitude < maxRange;
            if (!inRange && !enteredRange) // Make the FlyingEnemy move towards the player before flying around them at a constant orbitSpeed.
            {
                //enteredRange = false; // TODO set this to false if the FlyingEnemy is moved outside a farther radius than inRange checks for. Enough extra distance that oscillations don't have a chance to go outside the bounds.

                Vector3 newPosition = Vector3.Lerp(transform.position, playerTransform.position, radialSpeed * Time.deltaTime); // Speed proportional to distance
                //Vector3 newPosition = Vector3.MoveTowards(transform.position, playerTransform.position, radialSpeed); // Constant speed
                newPosition.y = 0.5f; // TODO remove
                body.MovePosition(newPosition);

                Vector3 flyingEnemyToPlayer = playerTransform.position - transform.position;
                Vector3 upwards = new Vector3(0, 1, 0);
                Quaternion playerDirection = Quaternion.LookRotation(flyingEnemyToPlayer, upwards);
                body.MoveRotation(playerDirection);
            }
            else
            {
                // Handle first time entering range
                if (!enteredRange)
                {
                    xPlayerOffset = playerTransform.position.x;
                    zPlayerOffset = playerTransform.position.z;
                    horizontalMaxDistance = Mathf.Sqrt(Mathf.Pow(transform.position.x - xPlayerOffset, 2) + Mathf.Pow(transform.position.z - zPlayerOffset, 2));

                    if (clockwiseOrbit)
                    {
                        timeCounter -= (orbitSpeed * (Time.deltaTime / horizontalMaxDistance));
                    }
                    else
                    {
                        timeCounter += (orbitSpeed * (Time.deltaTime / horizontalMaxDistance));
                    }

                    yIntercept = transform.position.y - (verticalOscillationMagnitude * Mathf.Sin(verticalOscillationsPerRotation * timeCounter));
                    xIntercept = transform.position.x - xPlayerOffset - (horizontalMaxDistance * Mathf.Cos(timeCounter));
                    zIntercept = transform.position.z - zPlayerOffset - (horizontalMaxDistance * Mathf.Sin(timeCounter));

                    enteredRange = true;
                }

                Vector3 playerPositionHorizontal = playerTransform.position;
                playerPositionHorizontal.y = 0;
                Vector3 currentPosition = new Vector3(xPlayerOffset, 0, zPlayerOffset);

                Vector3 playerOffset = Vector3.MoveTowards(currentPosition, playerPositionHorizontal, 0.01f);

                xPlayerOffset = playerOffset.x;
                zPlayerOffset = playerOffset.z;

                // Shooting projectile at the player when they are circling.
                StartCoroutine(ShootProjectileAtPlayer());

                float y = (verticalOscillationMagnitude * Mathf.Sin(verticalOscillationsPerRotation * timeCounter)) + yIntercept;
                float x = (horizontalMaxDistance * Mathf.Cos(timeCounter)) + xPlayerOffset + xIntercept;
                float z = (horizontalMaxDistance * Mathf.Sin(timeCounter)) + zPlayerOffset + zIntercept;

                Vector3 newPosition = new Vector3(x, y, z);
                newPosition.y = 0.5f; // TODO remove
                body.MovePosition(newPosition);

                Vector3 flyingEnemyToPlayer = playerTransform.position - transform.position;
                Vector3 upwards = new Vector3(0, 1, 0);
                Quaternion playerDirection = Quaternion.LookRotation(flyingEnemyToPlayer, upwards);
                body.MoveRotation(playerDirection);

                if (clockwiseOrbit)
                {
                    timeCounter -= (orbitSpeed * (Time.deltaTime / horizontalMaxDistance));
                }
                else
                {
                    timeCounter += (orbitSpeed * (Time.deltaTime / horizontalMaxDistance));
                }
            }
        }
    }

    public void setIsSelected(bool selected)
    {
        isSelected = selected;
    }

    protected override void SetStartingHealth()
    {
        health = 20;
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
            yield break;
        }

        yield break;
    }
}
