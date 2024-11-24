using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TreeSpear : GiantGrabInteractable
{
    protected bool flightAlignmentEnabled = false;

    protected override void DisablePickup(SelectExitEventArgs args)
    {
        base.DisablePickup(args);
        flightAlignmentEnabled = true;
    }

    protected void FixedUpdate()
    {
        if (flightAlignmentEnabled)
        {
            if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody body))
            {
                Vector3 newOrientation = body.linearVelocity.normalized;
                if (newOrientation != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(newOrientation);
                }
            }
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (collision.collider.gameObject.tag == "Water" || collision.collider.gameObject.tag == "Ground")
        {
            flightAlignmentEnabled = false;
        }
    }
}
