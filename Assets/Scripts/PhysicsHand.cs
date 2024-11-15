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

    private List<float> intermediateRotationAngle = new List<float>();

    private List<Quaternion> intermediateRotations = new List<Quaternion>();

    private List<Transform> spokes = new List<Transform>();

    void FixedUpdate()
    {
        SetPosition();
        SetRotation();
    }

    void SetPosition()
    {
        Vector3 lerpTrackedTransformPosition = Vector3.LerpUnclamped(transform.position, trackedTransform.position, positionSpeed * Time.deltaTime);
        float distance = Vector3.Distance(lerpTrackedTransformPosition, body.position);

        if (distance > minTeleportDistance) // FIXME: Will want to check that the hand isn't carrying anything first before the hand gets teleported away
        {
            Debug.Log("Distance: " + distance);
            Debug.Log("teleporting hand to controller");
            transform.position = trackedTransform.position;
        }
        else
        {
            //var vel = (lerpTrackedTransformPosition - body.position) * positionStrength; // Scales linearly with scale. Should have an exponential increase the farther away the hand is. (relative distance vector) * (speed per unit length)
            var vel = (lerpTrackedTransformPosition - body.position) * positionStrength * Mathf.Pow(1 + distance, distance);
            Debug.Log("Exponential factor: " + Mathf.Pow(1 + distance, distance));
            body.linearVelocity = vel;
        }
    }

    void SetRotation()
    {
        /////////////////// Create new spokes on every update //////////////////////////
        // Remove all spokes
        //while (spokes.Count > 0)
        //{
        //    Transform spoke = spokes[spokes.Count - 1];
        //    spokes.RemoveAt(spokes.Count - 1);
        //    Destroy(spoke.gameObject);
        //}
        //
        //float angleAmt = 30.0f;
        //float angle = Quaternion.Angle(body.rotation, trackedTransform.rotation);
        //
        //// Recreate spokes on each update
        //float numSpokesToAdd = Mathf.Floor(angle / angleAmt);
        //Quaternion previousSpoke = Quaternion.identity;
        //for (int i = 0; i < numSpokesToAdd; i++)
        //{
        //    Quaternion spoke = Quaternion.identity;
        //    if (i == 0) // last spoke distance
        //    {
        //        spoke = Quaternion.RotateTowards(body.rotation, trackedTransform.rotation, angleAmt);
        //    }
        //    else
        //    {
        //        spoke = Quaternion.RotateTowards(previousSpoke, trackedTransform.rotation, angleAmt);
        //    }
        //
        //    Transform spokeObject = Instantiate(debugSpoke, transform);
        //    spokeObject.rotation = spoke;
        //
        //    spokes.Add(spokeObject);
        //    previousSpoke = spoke;
        //}
        ////////////////////////////////////////////////////////////////////////////////////

        /////////////////// Keep persistant list of spokes every update ////////////////////////////
        //// TODO: Update all existing spokes to be on new plane defined by the rotation axis between the rotations of the controller and the giant hand.
        //// First get the rotation plane made by the controller and the giant hand. Make a transparent reconstruction like the debug spokes.
        //// Keep track of previous Controller angle and current controller angle. Find the quaternion that makes that angle translation and apply it to all spokes.
        //
        //float angleAmt = 30.0f;
        //float angle = Quaternion.Angle(body.rotation, trackedTransform.rotation);
        //
        //// Remove spokes
        //float difference = (angle - (spokes.Count * angleAmt)) / angleAmt;
        //if (difference < 0)
        //{
        //    // Calculate num to remove
        //    float numToRemove = Mathf.Ceil(Mathf.Abs(difference));
        //
        //    int spokeCount = spokes.Count;
        //    for (int i = spokeCount - 1; i > spokeCount - numToRemove - 1; i--)
        //    {
        //        Transform currentSpoke = spokes[i];
        //        spokes.RemoveAt(i);
        //        Destroy(currentSpoke.gameObject);
        //    }
        //}
        //
        //// Add new spokes
        //if (angle > ((spokes.Count + 1) * angleAmt))
        //{
        //    Quaternion spoke = Quaternion.identity;
        //    if (spokes.Count > 0)
        //    {
        //        // Rotate from spoke furthest from GiantHand
        //        spoke = Quaternion.RotateTowards(spokes[spokes.Count - 1].rotation, trackedTransform.rotation, angleAmt);
        //    }
        //    else
        //    {
        //        // Rotate directly from GiantHand
        //        spoke = Quaternion.RotateTowards(body.rotation, trackedTransform.rotation, angleAmt);
        //    }
        //    Transform spokeObject = Instantiate(debugSpoke, transform);
        //    spokeObject.rotation = spoke;
        //    spokes.Add(spokeObject);
        //}
        /////////////////////////////////////////////////////////////////////////////////////////////
        //
        //// Calculate newTorque from spokes
        //Vector3 newTorque = new Vector3(0, 0, 0);
        //foreach (Transform spoke in spokes)
        //{
        //    Vector3 spokeEulers = OrientTorque(spoke.rotation.eulerAngles);
        //    newTorque += spokeEulers;
        //}
        //
        //if (spokes.Count > 0)
        //{
        //    Quaternion lastRotationDiff = Quaternion.Inverse(body.rotation) * spokes[spokes.Count - 1].rotation;
        //    Vector3 lastRotationEulers = OrientTorque(lastRotationDiff.eulerAngles);
        //    newTorque += lastRotationEulers;
        //}
        //else
        //{
        //    Quaternion controllerGiantHandDiff = Quaternion.Inverse(body.rotation) * trackedTransform.rotation;
        //    Vector3 spokeEulers = OrientTorque(controllerGiantHandDiff.eulerAngles);
        //    newTorque += spokeEulers;
        //}
        //
        //// Put the torque back in body space
        //newTorque = body.rotation * newTorque;

        // Calculate Torque from body position and controller position
        Quaternion diff = Quaternion.Inverse(body.rotation) * trackedTransform.rotation;
        Vector3 eulers = OrientTorque(diff.eulerAngles);
        Vector3 torque = eulers;

        // Put the torque back in body space
        torque = body.rotation * torque;

        //Debug.Log("torque: " + torque + ", newTorque: " + newTorque);

        body.angularVelocity = Vector3.zero; // TODO caculate new torque based on current angularVelocity
        body.AddTorque(torque, ForceMode.VelocityChange);

        ///////////////////// Old Method ////////////////////////////
        //float angleDistance = Quaternion.Angle(body.rotation, trackedTransform.rotation);
        //if (angleDistance < rotationThreshold)
        //{
        //    body.MoveRotation(trackedTransform.rotation);
        //}
        //else
        //{
        //    Quaternion qDiff = trackedTransform.rotation * Quaternion.Inverse(transform.rotation);
        //    float kp = (6f * rotationStrength) * (6f * rotationStrength) * 0.25f;
        //    float kd = 4.5f * rotationStrength;
        //    Vector3 x;
        //    float xMag;
        //    Quaternion q = trackedTransform.rotation * Quaternion.Inverse(transform.rotation); // Subtract current rotation from trackedTransform rotation
        //    q.ToAngleAxis(out xMag, out x);
        //
        //    x.Normalize();
        //    x *= Mathf.Deg2Rad;
        //    Vector3 pidv = kp * x * xMag - kd * body.angularVelocity;
        //    Quaternion rotInertia2World = body.inertiaTensorRotation * transform.rotation;
        //    pidv = Quaternion.Inverse(rotInertia2World) * pidv;
        //    pidv.Scale(body.inertiaTensor);
        //    pidv = rotInertia2World * pidv;
        //    body.AddTorque(pidv);
        //}
        ////////////////////////////////////////////////////////////
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
