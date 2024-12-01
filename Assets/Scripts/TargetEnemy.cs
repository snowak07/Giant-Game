using UnityEngine;

public class TargetEnemy : Enemy
{
    protected TargetEnemy() : base(0.01f)
    {

    }
    public override (Vector3, Quaternion) GetNextTransform(float time, bool applyTargetingOffset = false)
    {
        return (transform.position, transform.rotation);
    }
}
