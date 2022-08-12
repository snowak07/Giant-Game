using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Handle hands colliding with interactables and connecting a joint to them on select activated.
public class GrabDirectInteractor : XRDirectInteractor
{
    public float massScaling = 0.2f;

    protected override void Start()
    {
        base.Start();

        selectEntered.AddListener(AttachBody);
        selectExited.AddListener(DetachBody);
    }

    protected void AttachBody(SelectEnterEventArgs args)
    {
        //Rigidbody interactableRigidbody = args.interactableObject.colliders[0].attachedRigidbody;
        List<IXRInteractable> targets = new List<IXRInteractable>();
        GetValidTargets(targets);
        foreach (IXRInteractable interactable in targets)
        {
            AttachJoint(interactable.colliders[0].attachedRigidbody);
        }
        //List<Collider> interactableColliders = args.interactableObject.colliders;
        //foreach (Collider interactableCollider in interactableColliders)
        //{
        //    Debug.Log("Attaching Joint");
        //    AttachJoint(interactableCollider.attachedRigidbody);
        //}
    }

    protected void DetachBody(SelectExitEventArgs args)
    {
        DetachJoint(args.interactableObject.colliders[0].attachedRigidbody);
    }

    public void AttachJoint(Rigidbody rigidbodyToAttach)
    {
        Debug.Log("Attaching Joint");
        Debug.Log("RigidbodyToAttach: " + rigidbodyToAttach.name);
        FixedJoint joint = gameObject.AddComponent<FixedJoint>();
        rigidbodyToAttach.mass = rigidbodyToAttach.mass * massScaling;
        joint.connectedBody = rigidbodyToAttach;
        Debug.Log("AttachJoint End");
    }

    protected void DetachJoint(Rigidbody bodyToDetach)
    {
        Debug.Log("DetachJoints");
        FixedJoint[] joints = GetComponents<FixedJoint>();
        Debug.Log("joints to detach length: " + joints.Length);
        foreach(FixedJoint joint in joints)
        {
            Debug.Log("Checking if correct Rigidbody");
            //if (joint.connectedBody.Equals(bodyToDetach)) // TODO change to check for all colliders in SelectExitEventArgs
            //{
                Debug.Log("Detaching Rigidbody");
                joint.connectedBody.mass = joint.connectedBody.mass / massScaling;
                Destroy(joint);
            //}
        }
    }
}
