using UnityEngine;

public class TargetEnemy : Enemy
{
    protected override void Start()
    {
        base.Start();
        Initialize(0.01f, 0, 0, false, false, true);
    }

    public override (Vector3, Quaternion) GetNextTransform(float time, bool applyTargetingOffset = false)
    {
        return (transform.position, transform.rotation);
    }
}
