using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(FlightAlignment))]
public class TreeSpear : GiantGrabInteractable
{
    public GameObject targetChecker = null;
    protected bool detached = false;
    protected Collider detectedEnemyCollider = null;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (TryGetComponent(out InteractableRoot root))
        {
            selectEntered.AddListener(root.DisableReturnForce);
            selectExited.AddListener(root.EnableReturnForce);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (TryGetComponent(out InteractableRoot root))
        {
            selectEntered.RemoveListener(root.DisableReturnForce);
            selectExited.RemoveListener(root.EnableReturnForce);
        }
    }

    protected override void DisablePickup(SelectExitEventArgs args)
    {
        base.DisablePickup(args);

        if (TryGetComponent(out InteractableRoot root) && root.broken)
        {
            GetComponent<FlightAlignment>().Enable();
        }
    }

    /**
     * Wait for the XRGrabInteractable to detach so that the throw smoothing is applied to the Rigidbody.
     */
    protected override void Detach()
    {
        base.Detach();

        if (TryGetComponent(out InteractableRoot root) && root.broken)
        {
            InitializeTargetChecker();
        }
    }

    protected void TargetDetected(Collider detectedEnemyCollider)
    {
        Debug.Log("[TreeSpear] TargetDetected");
        this.detectedEnemyCollider = detectedEnemyCollider;
    }

    protected void FixedUpdate()
    {
        if (detached && detectedEnemyCollider != null && Helpers.TryGetComponentInParent(detectedEnemyCollider.gameObject, out Enemy detectedEnemy))
        {
            Debug.Log("[TreeSpear] Calculating intercept direction");
            Vector3 interceptDirection = TrajectoryHelper.CalculateInterceptionDirection(transform.position, GetComponent<Rigidbody>().linearVelocity.magnitude, detectedEnemy.transform.GetComponent<MovementProvider>().GetNextTransform);

            if (interceptDirection != Vector3.zero)
            {
                Debug.Log("[TreeSpear] Adjusting course towards Enemy");
                Vector3 interceptVelocity = GetComponent<Rigidbody>().linearVelocity.magnitude * interceptDirection;
                GetComponent<Rigidbody>().linearVelocity = interceptVelocity;
            } 
            
            detached = false;
            detectedEnemyCollider = null;
        }
    }

    protected void InitializeTargetChecker()
    {
        Debug.Log("[TreeSpear] InitializeTargetChecker");
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
            GetComponent<FlightAlignment>().Disable();
        }
    }
}
