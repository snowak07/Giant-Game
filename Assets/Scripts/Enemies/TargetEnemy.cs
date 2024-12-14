using UnityEngine;

public class TargetEnemy : Enemy
{
    protected override void Start()
    {
        base.Start();
        Initialize(0.01f, 0, 0, false, false, true);
    }
}
