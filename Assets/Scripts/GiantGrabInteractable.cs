using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class GiantGrabInteractable : XRGrabInteractable
{
    private bool impactCooldown = false;

    void Start()
    {
        selectEntered.AddListener(EnableMovement);
    }

    // Disable isKinematic and enable useGravity so that the Interactable can be moved while selected.
    protected void EnableMovement(SelectEnterEventArgs args)
    {
        impactCooldown = false;

        Rigidbody body = GetComponent<Rigidbody>();
        body.isKinematic = false;
        body.useGravity = true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        selectEntered.RemoveListener(EnableMovement);
    }

    public void enableImpactCooldown()
    {
        impactCooldown = true;
    }

    public bool impactCooldownEnabled()
    {
        return impactCooldown;
    }
}
