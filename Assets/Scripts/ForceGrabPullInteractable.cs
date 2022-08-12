using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class ForceGrabPullInteractable : XRGrabInteractable
{
    void Start()
    {
        selectEntered.AddListener(EnableMovement);
    }

    // Disable isKinematic and enable useGravity so that the Interactable can be moved while selected.
    protected void EnableMovement(SelectEnterEventArgs args)
    {
        Rigidbody body = GetComponent<Rigidbody>();
        body.isKinematic = false;
        body.useGravity = true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        selectEntered.RemoveListener(EnableMovement);
    }

    //protected override void OnActivated(ActivateEventArgs args)
    //{
    //    FixedJoint[] joints = parentObject.GetComponentsInChildren<FixedJoint>();
    //    foreach (FixedJoint joint in joints)
    //    {
    //        joint.connectedBody = null;
    //    }
    //
    //    foreach (FixedJoint joint in joints)
    //    {
    //        Destroy(joint);
    //    }
    //}
}
