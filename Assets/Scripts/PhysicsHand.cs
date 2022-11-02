using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsHand : MonoBehaviour
{
    public Transform trackedTransform = null;
    public Rigidbody body = null;

    public float positionSpeed = 3;
    public float positionStrength = 20;
    public float positionThreshold = 0.005f;
    public float rotationSpeed = 3;
    public float rotationStrength = 30;
    public float rotationThreshold = 10f;

    private Quaternion previousTrackedRotation;

    void FixedUpdate()
    {
        SetPosition();
        SetRotation();
    }

    void SetPosition()
    {
        Vector3 lerpTrackedTransformPosition = Vector3.LerpUnclamped(transform.position, trackedTransform.position, positionSpeed * Time.deltaTime);
        var distance = Vector3.Distance(lerpTrackedTransformPosition, body.position);
        var vel = (lerpTrackedTransformPosition - body.position).normalized * positionStrength * distance;
        body.velocity = vel;
    }

    // TODO Add rotation "history" tracking so that the controller doesn't rotate in the direction of the shortest distance but follows the path of the controller rotation instead
    void SetRotation()
    {
        float angleDistance = Quaternion.Angle(body.rotation, trackedTransform.rotation);
        if (angleDistance < rotationThreshold)
        {
            body.MoveRotation(trackedTransform.rotation);
        }
        else
        {
            float kp = (6f * rotationStrength) * (6f * rotationStrength) * 0.25f;
            float kd = 4.5f * rotationStrength;
            Vector3 x;
            float xMag;
            Quaternion q = trackedTransform.rotation * Quaternion.Inverse(transform.rotation);
            q.ToAngleAxis(out xMag, out x);
            x.Normalize();
            x *= Mathf.Deg2Rad;
            Vector3 pidv = kp * x * xMag - kd * body.angularVelocity;
            Quaternion rotInertia2World = body.inertiaTensorRotation * transform.rotation;
            pidv = Quaternion.Inverse(rotInertia2World) * pidv;
            pidv.Scale(body.inertiaTensor);
            pidv = rotInertia2World * pidv;
            body.AddTorque(pidv);
        }
    }
}
