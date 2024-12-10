using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Handle hands colliding with interactables and connecting a joint to them on select activated.
public class GrabDirectInteractor : UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor
{
    public float massScaling = 0.1f;

    protected override void Start()
    {
        base.Start();

        selectEntered.AddListener(AttachBody);
        selectExited.AddListener(DetachBody);
    }

    protected void AttachBody(SelectEnterEventArgs args)
    {
        Debug.Log("[GrabDirectInteractor] AttachBody");
        List<UnityEngine.XR.Interaction.Toolkit.Interactables.IXRInteractable> targets = new List<UnityEngine.XR.Interaction.Toolkit.Interactables.IXRInteractable>();
        GetValidTargets(targets);
        foreach (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRInteractable interactable in targets)
        {
            AttachJoint(interactable.colliders[0].attachedRigidbody);
        }
    }

    protected void DetachBody(SelectExitEventArgs args)
    {
        Debug.Log("[GrabDirectInteractor] DetachBody");
        DetachJoint(args.interactableObject.colliders[0].attachedRigidbody);
    }

    public void AttachJoint(Rigidbody rigidbodyToAttach)
    {
        Debug.Log("[GrabDirectInteractor] AttachJoint");
        FixedJoint joint = gameObject.AddComponent<FixedJoint>();
        rigidbodyToAttach.mass = rigidbodyToAttach.mass * massScaling;
        joint.connectedBody = rigidbodyToAttach;
    }

    protected void DetachJoint(Rigidbody bodyToDetach)
    {
        Debug.Log("[GrabDirectInteractor] DetachJoint");
        FixedJoint[] joints = GetComponents<FixedJoint>();
        foreach(FixedJoint joint in joints)
        {
            joint.connectedBody.mass = joint.connectedBody.mass / massScaling;
            Destroy(joint);
        }
    }
}
