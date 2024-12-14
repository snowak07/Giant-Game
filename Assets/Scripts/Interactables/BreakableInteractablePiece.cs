using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableInteractablePiece : MonoBehaviour
{
    public void FormInteractablePiece()
    {
        //Rigidbody body = gameObject.AddComponent<Rigidbody>();
        Rigidbody body = GetComponent<Rigidbody>();
        body.mass = 5;
        body.useGravity = true;

        Collider collider = GetComponent<Collider>();
        collider.isTrigger = false;

        //gameObject.AddComponent<ForceGrabPullInteractable>();
    }
}
