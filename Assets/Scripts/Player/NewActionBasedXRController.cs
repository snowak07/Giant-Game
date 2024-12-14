using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class NewActionBasedXRController : ActionBasedController
{
    private bool pickupEnabled;

    private void Start()
    {
        pickupEnabled = false;

        selectAction.action.started += EnablePickup;
        selectAction.action.canceled += DisablePickup;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        selectAction.action.started -= EnablePickup;
        selectAction.action.canceled -= DisablePickup;
    }

    protected void EnablePickup(InputAction.CallbackContext ctx)
    {
        pickupEnabled = true;
    }

    protected void DisablePickup(InputAction.CallbackContext ctx)
    {
        pickupEnabled = false;
    }

    public bool PickupEnabled()
    {
        return pickupEnabled;
    }
}
