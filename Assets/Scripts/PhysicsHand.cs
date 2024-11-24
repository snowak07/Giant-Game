using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsHand : MonoBehaviour
{
    public Transform trackedTransform = null;
    public Rigidbody body = null;
    public Transform debugSpoke = null;

    public float minTeleportDistance = 10.0f;
    public float positionSpeed = 3;
    public float positionStrength = 20;
    public float rotationSpeed = 1;
    public float rotationStrength = 30;
    public float rotationThreshold = 10f;
    public float force = 10f;

    private float speedLimit = 60.0f;

    void FixedUpdate()
    {
        SetPosition();
        SetRotation();
    }

    void SetPosition()
    {
        Vector3 lerpTrackedTransformPosition = Vector3.Lerp(transform.position, trackedTransform.position, positionSpeed * Time.deltaTime);
        float distance = Vector3.Distance(lerpTrackedTransformPosition, body.position);

        if (distance > minTeleportDistance) // FIXME: Will want to check that the hand isn't carrying anything first before the hand gets teleported away
        {
            transform.position = trackedTransform.position;
        }
        else
        {
            // Hand gets exponentially faster the further away it is.
            var vel = (lerpTrackedTransformPosition - body.position) * positionStrength * Mathf.Pow(1 + distance, distance);
            if (vel.magnitude > speedLimit)
            {
                vel = (speedLimit / vel.magnitude) * vel;
            }
            body.linearVelocity = vel;
        }
    }

    void SetRotation()
    {
        // Calculate Torque from body position and controller position
        Quaternion diff = Quaternion.Inverse(body.rotation) * trackedTransform.rotation;
        Vector3 eulers = OrientTorque(diff.eulerAngles);
        Vector3 torque = eulers;

        // Put the torque back in body space
        torque = body.rotation * torque;

        /////// This should maybe be a separate class or at least a separate function.////////
        // Determine if there are attached objects (joints) and apply the same torque to those objects in order
        // to offset the unwieldyness of handling the additional mass.
        FixedJoint[] heldObjectJoints = GetComponents<FixedJoint>();
        foreach (FixedJoint joint in heldObjectJoints)
        {
            // Add torque to Joints
            joint.connectedBody.angularVelocity = Vector3.zero;
            joint.connectedBody.AddTorque(torque, ForceMode.VelocityChange);
        }
        /////////////////////////////////////////////////////////////////////////////////////

        body.angularVelocity = Vector3.zero; // TODO caculate new torque based on current angularVelocity
        body.AddTorque(torque, ForceMode.VelocityChange);
    }

    private Vector3 OrientTorque(Vector3 torque)
    {
        // Quaternion's Euler conversion results in (0-360)
        // For torque, we need -180 to 180.

        return new Vector3
        (
            torque.x > 180f ? torque.x - 360f : torque.x,
            torque.y > 180f ? torque.y - 360f : torque.y,
            torque.z > 180f ? torque.z - 360f : torque.z
        );

        //return new Vector3
        //(
        //    torque.x > 180f ? 180f - torque.x : torque.x,
        //    torque.y > 180f ? 180f - torque.y : torque.y,
        //    torque.z > 180f ? 180f - torque.z : torque.z
        //);
    }
}
