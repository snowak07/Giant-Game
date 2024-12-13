using UnityEngine;

public class StationaryMovementProvider : MovementProvider
{
    public override (Vector3, Quaternion) GetNextTransform(float time, bool applyTargetingOffset = false)
    {
        return (transform.position, transform.rotation);
    }
}
