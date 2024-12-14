using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EnemyGrabInteractable : GiantGrabInteractable
{
    protected override void DisablePickup(SelectExitEventArgs args)
    {
        base.DisablePickup(args);

        Enemy enemy = GetComponent<Enemy>();
        enemy.IsPickedUp = false;
    }

    // Enable useGravity so that the Interactable can be moved with PhysicsHand while selected.
    protected override void EnablePickup(SelectEnterEventArgs args)
    {
        base.EnablePickup(args);

        Enemy enemy = GetComponent<Enemy>();
        enemy.IsPickedUp = true;
    }
}
