using UnityEngine;

[RequireComponent(typeof(PathProvider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class MovementProvider : MonoBehaviour
{
    protected bool flying;
    protected bool pitch;
    protected float maxRotationalSpeed; // Measured in units/s
    protected float maxTranslationalSpeed; // Measured in units/s

    public virtual void Initialize(float maxTranslationalSpeed, float maxRotationalSpeed, bool flying = false, bool pitch = false)
    {
        this.maxTranslationalSpeed = maxTranslationalSpeed;
        this.maxRotationalSpeed = maxRotationalSpeed;
        this.flying = flying;
        this.pitch = pitch;
    }

    public virtual (Vector3, Quaternion) HandleMovement()
    {
        if (GetComponent<PathProvider>().hasPath())
        {
            (Vector3, Quaternion) nextPositionRotation = GetNextTransform(Time.fixedDeltaTime);
            GetComponent<Rigidbody>().MoveRotation(nextPositionRotation.Item2);
            GetComponent<Rigidbody>().MovePosition(nextPositionRotation.Item1);

            return nextPositionRotation;
        }

        return (Vector3.zero, Quaternion.identity);
    }

    public abstract (Vector3, Quaternion) GetNextTransform(float time, bool preserveState = false);
}
