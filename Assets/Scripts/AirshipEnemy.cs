using UnityEngine;

[RequireComponent(typeof(PathFollower))]
[RequireComponent(typeof(ProjectileLauncher))]
public class AirshipEnemy : Enemy
{
    protected override void Start()
    {
        base.Start();
        Initialize(20, 3, 20, true, false);
    }

    protected override void UpdateEnemy()
    {
        base.UpdateEnemy();

        if (playerTransform != null)
        {
            GetComponent<ProjectileLauncher>().ShootProjectile(playerTransform.position);
        }
    }
}
