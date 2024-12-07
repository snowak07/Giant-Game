using UnityEngine;

public class FlightAlignment : MonoBehaviour
{
    private bool _enabled = false;

    public void Enable() { _enabled = true; }
    public void Disable() { _enabled = false; }

    protected void FixedUpdate()
    {
        if (_enabled)
        {
            if (TryGetComponent<Rigidbody>(out Rigidbody body))
            {
                Vector3 newOrientation = body.linearVelocity.normalized;
                if (newOrientation != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(newOrientation);
                }
            }
        }
    }
}
