using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Sinkable : MonoBehaviour
{
    private bool sinkEnabled = false;
    private bool sinking = false;
    private float sinkingSpeed = 0.005f;
    private float settledVelocityThreshold = 0.01f;
    private float settledAngularVelocityThreshold = 0.005f;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!sinking && sinkEnabled && IsSettled(GetComponent<Rigidbody>()))
        {
            sinking = true;

            // Disable all collisions
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }

            DisableMovement();
        }

        if (sinking)
        {
            Vector3 newPosition = new Vector3(transform.position.x, transform.position.y - sinkingSpeed, transform.position.z);
            transform.position = newPosition;
        }
    }

    protected void DisableMovement()
    {
        // Set velocity to 0 and disable gravity since we are controlling its downward movement manually
        Rigidbody body = GetComponent<Rigidbody>();
        body.linearVelocity = new Vector3(0, 0, 0);
        body.angularVelocity = new Vector3(0, 0, 0);
        body.useGravity = false;
        body.isKinematic = true; // Disable all forces, collision, and joints acting on the rigidbody. Movement controlled through setting transform.position
        body.detectCollisions = false; // Free up processing from collision detection
    }

    public void EnableSink()
    {
        StartCoroutine(StartSink());
    }

    protected bool IsSettled(Rigidbody body)
    {
        if (body.linearVelocity.magnitude < settledVelocityThreshold && body.angularVelocity.magnitude < settledAngularVelocityThreshold)
        {
            return true;
        }

        return false;
    }

    protected IEnumerator StartSink()
    {
        yield return new WaitForSeconds(0.1f);

        sinkEnabled = true;
        Destroy(gameObject, 30);
    }
}
