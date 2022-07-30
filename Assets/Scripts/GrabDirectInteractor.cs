using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Handle hands colliding with interactables and connecting a joint to them on select activated.
public class GrabDirectInteractor : XRDirectInteractor
{
    protected override void Start()
    {
        base.Start();

        selectEntered.AddListener(AttachBody);
        selectExited.AddListener(DetachBody);
    }

    protected void AttachBody(SelectEnterEventArgs args)
    {
        Rigidbody interactableRigidbody = args.interactableObject.colliders[0].attachedRigidbody;
        AttachJoint(interactableRigidbody);
    }

    protected void DetachBody(SelectExitEventArgs args)
    {
        DetachJoint();
    }

    protected void AttachJoint(Rigidbody rigidbodyToAttach)
    {
        gameObject.AddComponent<FixedJoint>();
        FixedJoint joint = GetComponent<FixedJoint>();
        rigidbodyToAttach.mass = 5;
        joint.connectedBody = rigidbodyToAttach;
    }

    protected void DetachJoint()
    {
        FixedJoint[] joints = GetComponents<FixedJoint>();
        foreach(FixedJoint joint in joints)
        {
            joint.connectedBody.mass = 25;
            Destroy(joint);
        }
    }
}
