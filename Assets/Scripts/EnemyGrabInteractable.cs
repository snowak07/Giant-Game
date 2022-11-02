using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EnemyGrabInteractable : XRGrabInteractable
{
    void Start()
    {
        selectEntered.AddListener(EnablePickup);
        selectExited.AddListener(DisablePickup);
    }

    protected void DisablePickup(SelectExitEventArgs args)
    {
        Rigidbody body = GetComponent<Rigidbody>();
        body.useGravity = false;

        FlyingEnemy enemy = GetComponent<FlyingEnemy>(); // TODO make generic type
        enemy.setIsSelected(false);

        //Animator animation = GetComponent<Animator>(); // Disable animation on pickup?
        //animation.enabled = true;
    }

    // Enable useGravity so that the Interactable can be moved with PhysicsHand while selected.
    protected void EnablePickup(SelectEnterEventArgs args)
    {
        Rigidbody body = GetComponent<Rigidbody>();
        body.useGravity = true;
        body.isKinematic = false;

        FlyingEnemy enemy = GetComponent<FlyingEnemy>(); // TODO make generic type
        enemy.setIsSelected(true);

        //Animator animation = GetComponent<Animator>(); // Disable animation on pickup?
        //animation.enabled = false;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        selectEntered.RemoveListener(EnablePickup);
        selectExited.RemoveListener(DisablePickup);
    }
}
