using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FIXME Handles and Flag_pole aren't ever registering as sinking and never get destroyed.
[RequireComponent(typeof(Rigidbody))]
public class Sinkable : MonoBehaviour
{
    public bool sinkEnabled = false;

    private bool sinking = false;
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

            // Disable all collision with water
            GameObject[] waters = GameObject.FindGameObjectsWithTag("Water");
            foreach (var water in waters)
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), water.GetComponent<Collider>());
            }

            // Set velocity to 0 and disable gravity since we are controlling its downward movement manually
            Rigidbody body = GetComponent<Rigidbody>();
            body.velocity = new Vector3(0, 0, 0);
            body.angularVelocity = new Vector3(0, 0, 0);
            body.useGravity = false;
            body.isKinematic = true; // Disable all forces, collision, and joints acting on the rigidbody. Movement controlled through setting transform.position
            body.detectCollisions = false; // Free up processing from collision detection
        }

        if (sinking)
        {
            //Rigidbody body = GetComponent<Rigidbody>();
            Vector3 newPosition = new Vector3(transform.position.x, transform.position.y - sinkingSpeed, transform.position.z);
            //body.MovePosition(newPosition);
            transform.position = newPosition;
        }
    }

    public void EnableSink()
    {
        sinkEnabled = true;
    }

    protected bool IsSettled(Rigidbody body)
    {
        if (body.velocity.magnitude < settledVelocityThreshold && body.angularVelocity.magnitude < settledAngularVelocityThreshold)
        {
            return true;
        }

        return false;
    }
}
