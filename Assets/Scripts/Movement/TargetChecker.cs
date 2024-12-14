//using System.Diagnostics;
using UnityEngine;

public delegate void TargetDetectedCallback(Collider collision);

public class TargetChecker : MonoBehaviour
{
    protected TargetDetectedCallback targetDetectedCallback = null;
    protected Vector3 velocity = Vector3.zero;
    protected Vector3 initialPosition = Vector3.zero;
    void Start()
    {
        initialPosition = transform.position;
    }

    protected void FixedUpdate()
    {
        while (transform.position.y > -1 && velocity != Vector3.zero && targetDetectedCallback != null)
        {
            RaycastHit raycastHit;
            float detectorSphereSize = 0.5f * (initialPosition - transform.position).magnitude; // TODO: Incorporate into Sphere collider size
            if (Physics.SphereCast(transform.position, 15.0f, velocity.normalized, out raycastHit, Mathf.Infinity, LayerMask.GetMask("Enemy")))
            {
                targetDetectedCallback(raycastHit.collider);
                targetDetectedCallback = null;
                velocity = Vector3.zero;
                Destroy(gameObject);
            }

            transform.position += velocity * Time.fixedDeltaTime;
            velocity += Physics.gravity * Time.fixedDeltaTime;
        }

        if (transform.position.y < -1 && targetDetectedCallback != null)
        {
            Destroy(gameObject);
        }

        velocity = Vector3.zero;
        targetDetectedCallback = null;
    }

    public void SetCallback(TargetDetectedCallback callback)
    {
        targetDetectedCallback = callback;
    }

    public void SetInitialVelocity(Vector3 initialVelocity)
    {
        velocity = initialVelocity;
    }
}
