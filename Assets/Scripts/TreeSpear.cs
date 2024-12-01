using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TreeSpear : GiantGrabInteractable
{
    public GameObject targetChecker = null;
    protected bool detached = false;
    protected Collider detectedEnemyCollider = null;
    protected bool flightAlignmentEnabled = false;

    //public bool debugThrowTreeSpear = false;

    protected override void DisablePickup(SelectExitEventArgs args)
    {
        base.DisablePickup(args);

        flightAlignmentEnabled = true;
    }

    /**
     * Wait for the XRGrabInteractable to detach so that the throw smoothing is applied to the Rigidbody.
     */
    protected override void Detach()
    {
        base.Detach();
        InitializeTargetChecker();
    }

    protected void TargetDetected(Collider detectedEnemyCollider)
    {
        this.detectedEnemyCollider = detectedEnemyCollider;
    }

    protected void FixedUpdate()
    {
        //if (debugThrowTreeSpear)
        //{
        //    GetComponent<Rigidbody>().useGravity = true;
        //    debugThrowTreeSpear = false;
        //    Vector3 debugVelocity = new Vector3(-60.0f, 0, 0);
        //    GetComponent<Rigidbody>().linearVelocity = debugVelocity;
        //    InitializeTargetChecker();
        //    flightAlignmentEnabled = true;
        //}

        if (detached && detectedEnemyCollider != null)
        {
            GameObject detectedEnemyObject = detectedEnemyCollider.gameObject;
            Vector3 interceptDirection = TrajectoryHelper.CalculateInterceptionDirection(transform.position, GetComponent<Rigidbody>().linearVelocity.magnitude, detectedEnemyObject.GetComponentInParent<Enemy>().GetNextTransform);

            if (interceptDirection != Vector3.zero)
            {
                Vector3 interceptVelocity = GetComponent<Rigidbody>().linearVelocity.magnitude * interceptDirection;
                GetComponent<Rigidbody>().linearVelocity = interceptVelocity;
            } 
            
            detached = false;
            detectedEnemyCollider = null;
        }

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

    protected void InitializeTargetChecker()
    {
        GameObject targetChecker = Instantiate(this.targetChecker, transform.position, transform.rotation);
        TargetChecker checker = targetChecker.GetComponent<TargetChecker>();
        checker.SetCallback(TargetDetected);
        checker.SetInitialVelocity(GetComponent<Rigidbody>().linearVelocity);
        detached = true;
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
