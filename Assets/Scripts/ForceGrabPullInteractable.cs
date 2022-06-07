using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class ForceGrabPullInteractable : XRGrabInteractable
{
    public InputActionReference activateReference = null;

    private bool interactableHovering;
    private Transform hoveringInteractor_T = null;
    private Transform interactable_T = null;
    private Rigidbody interactableRigidbody = null;

    // Start is called before the first frame update
    void Start()
    {
        interactableHovering = false;
        interactable_T = GetComponent<Transform>();
        interactableRigidbody = GetComponent<Rigidbody>();
        activateReference.action.performed += HandlePullAction;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        activateReference.action.performed -= HandlePullAction;
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);

        hoveringInteractor_T = args.interactorObject.transform;

        interactableHovering = true;
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);

        hoveringInteractor_T = null;

        interactableHovering = false;
    }

    private void HandlePullAction(InputAction.CallbackContext ctx)
    {
        if (interactableHovering && hoveringInteractor_T != null)
            Pull(hoveringInteractor_T);
    }

    private void Pull(Transform pullLocation_T)
    {
        Vector3 directDistanceVector = pullLocation_T.position - interactable_T.position;

        // Calculate Velocity y vector needed to be at the pull y position in 1 second
        float y = directDistanceVector.y + (0.5f * Mathf.Abs(Physics.gravity.y));

        Vector3 projectileVector = new Vector3(directDistanceVector.x, y, directDistanceVector.z);

        interactableRigidbody.AddForce(projectileVector - interactableRigidbody.velocity, ForceMode.VelocityChange);
    }
}
