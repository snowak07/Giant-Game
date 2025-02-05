using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class ProjectileLauncher : MonoBehaviour
{
    public Transform firingOffset = null;
    public GameObject projectilePrefab = null;
    public int cooldownTime = 0;
    public float projectileSpeed = 0;
    public bool launcherEnabled { get; set; }
    public bool onCooldown { get; set; }

    private void Start()
    {
        launcherEnabled = true;
    }

    protected ProjectileLauncher()
    {
        onCooldown = false;
    }

    protected void Shoot(Vector3 firingPosition, Vector3 projectileVector)
    {
        if (!projectileVector.Equals(Vector3.zero))
        {
            GameObject projectileObject = Instantiate(projectilePrefab, firingPosition, transform.rotation);
            // Enable impact cooldown so the projecile doesn't collide with the Enemy
            if (projectileObject.TryGetComponent(out GiantGrabInteractable enemyProjectile))
            {
                enemyProjectile.impactCooldown = true;
            }

            // Get rigidbody and add force and torque
            Rigidbody projectile = projectileObject.GetComponent<Rigidbody>();
            projectile.AddForce(projectileVector - projectile.linearVelocity, ForceMode.VelocityChange);
            projectile.AddTorque(Random.insideUnitSphere * 3.0f);
        }
    }

    IEnumerator ShootProjectileWorker(Vector3 targetPosition, Vector3 projectileVector)
    {
        if (!onCooldown)
        {
            onCooldown = true;
            Shoot(firingOffset.position, projectileVector);
            yield return new WaitForSeconds(cooldownTime);
            onCooldown = false;
            yield break;
        }

        yield break;
    }

    public Vector3 ShootProjectile(Vector3 targetPosition)
    {
        if (launcherEnabled && !onCooldown)
        {
            Vector3 projectileVector = TrajectoryHelper.CalculateFiringDirection(firingOffset.position, targetPosition, projectileSpeed);

            StartCoroutine(ShootProjectileWorker(targetPosition, projectileVector));
            return projectileVector.normalized; // Return firing direction
        }

        return Vector3.zero;
    }
}
