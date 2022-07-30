using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class ForceGrabPullInteractable : XRGrabInteractable
{
    private Transform interactable_T = null;
    private Rigidbody interactableRigidbody = null;

    void Start()
    {
        interactable_T = GetComponent<Transform>();
        interactableRigidbody = GetComponent<Rigidbody>();

        selectEntered.AddListener(EnableMovement);
    }

    // Disable isKinematic and enable useGravity so that the Interactable can be moved while selected.
    protected void EnableMovement(SelectEnterEventArgs args)
    {
        Rigidbody body = GetComponent<Rigidbody>();
        body.isKinematic = false;
        body.useGravity = true;
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        // Get position of activater
        Transform pullLocation_T = args.interactorObject.transform;
        Pull(pullLocation_T);
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
