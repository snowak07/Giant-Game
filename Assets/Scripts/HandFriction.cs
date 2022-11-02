using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandFriction : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Get rigidbody and apply drag to it proportional to contactCount
        Rigidbody collisionBody = (Rigidbody) collision.body;
        collisionBody.drag = 2;
        collisionBody.angularDrag = 0.25f;
    }

    private void OnCollisionExit(Collision collision)
    {
        // Change drag back to previous value.
        Rigidbody collisionBody = (Rigidbody)collision.body;
        collisionBody.drag = 0;
        collisionBody.angularDrag = 0.05f;
    }
}
