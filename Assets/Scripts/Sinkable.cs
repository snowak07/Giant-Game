using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FIXME Handles and Flag_pole aren't ever registering as sinking and never get destroyed.
[RequireComponent(typeof(Rigidbody))]
public class Sinkable : MonoBehaviour
{
    public bool sinkEnabled = false;

    public bool sinking = false;
    private float sinkingSpeed = 0.002f;
    private float settledVelocityThreshold = 0.01f;
    private float settledAngularVelocityThreshold = 0.005f;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!sinking && sinkEnabled && IsSettled(GetComponent<Rigidbody>()))
        {
            sinking = true;

            // Destroy sinking object after 15 seconds as it should have disappeared beneath the water
            Destroy(gameObject, 15);

            // Disable all collisions
            Collider collider = GetComponent<Collider>();
            collider.enabled = false;

            // Set velocity to 0 and disable gravity since we are controlling its downward movement manually
            Rigidbody body = GetComponent<Rigidbody>();
            body.linearVelocity = new Vector3(0, 0, 0);
            body.angularVelocity = new Vector3(0, 0, 0);
            body.useGravity = false;
            body.isKinematic = true; // Disable all forces, collision, and joints acting on the rigidbody. Movement controlled through setting transform.position
            body.detectCollisions = false; // Free up processing from collision detection
        }

        if (sinking)
        {
            Vector3 newPosition = new Vector3(transform.position.x, transform.position.y - sinkingSpeed, transform.position.z);
            transform.position = newPosition;
        }
    }

    public void EnableSink()
    {
        sinkEnabled = true;
        Destroy(gameObject, 30); // Failsafe incase the object never settles
    }

    protected bool IsSettled(Rigidbody body)
    {
        if (body.linearVelocity.magnitude < settledVelocityThreshold && body.angularVelocity.magnitude < settledAngularVelocityThreshold)
        {
            return true;
        }

        return false;
    }
}
