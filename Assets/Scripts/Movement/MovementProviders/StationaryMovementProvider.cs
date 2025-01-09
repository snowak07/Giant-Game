using UnityEngine;

public class StationaryMovementProvider : MovementProvider
{
    public override (Vector3, Quaternion) GetNextTransform(float time)
    {
        return (transform.position, transform.rotation);
    }
}
