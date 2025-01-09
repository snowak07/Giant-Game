using UnityEngine;

public class StationaryMovementProvider : MovementProvider
{
    public override (Vector3, Quaternion) GetNextTransform(float time, bool preserveState = false)
    {
        return (transform.position, transform.rotation);
    }
}
