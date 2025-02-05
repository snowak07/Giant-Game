using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Sinkable))]
public class GiantGrabInteractable : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
{
    public bool impactCooldown { get; set; }

    private bool gravityEnabled;

    void Start()
    {
        // Disable XRGrabInteractable tracking because we handle it ourselves in GiantDirectInteractor
        trackPosition = false;
        trackRotation = false;

        // Keep track of base gravity enabled state
        gravityEnabled = GetComponent<Rigidbody>().useGravity;

        selectEntered.AddListener(EnablePickup);
        selectExited.AddListener(DisablePickup);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        selectEntered.RemoveListener(EnablePickup);
        selectExited.RemoveListener(DisablePickup);
    }

    // Disable isKinematic and enable useGravity so that the Interactable can be moved while selected.
    protected virtual void EnablePickup(SelectEnterEventArgs args)
    {
        impactCooldown = false;

        Rigidbody body = GetComponent<Rigidbody>();
        body.isKinematic = false;
        body.useGravity = true;
    }

    protected virtual void DisablePickup(SelectExitEventArgs args)
    {
        Rigidbody body = GetComponent<Rigidbody>();
        body.useGravity = gravityEnabled;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Water" || collision.collider.gameObject.tag == "Ground")
        {
            GetComponent<Sinkable>().EnableSink();
        }
    }
}
