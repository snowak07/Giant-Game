using UnityEngine;

[RequireComponent(typeof(ProjectileLauncher))]
public class AirshipEnemy : Enemy
{
    protected override void Start()
    {
        base.Start();
        Initialize(20, 3, 40, true, false);
    }

    protected override void UpdateEnemy()
    {
        base.UpdateEnemy();

        if (targetTransform != null)
        {
            GetComponent<ProjectileLauncher>().ShootProjectile(targetTransform.position);
        }
    }
}
