using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushInteractor : MonoBehaviour
{
    Rigidbody r_interactor;
    List<Vector3> pastVelocities;

    // Start is called before the first frame update
    void Start()
    {
        r_interactor = GetComponent<Rigidbody>();
        pastVelocities = new List<Vector3>(50);
    }

    private void FixedUpdate()
    {
        if (pastVelocities.Count >= 50)
        {
            pastVelocities.RemoveAt(49);
        }

        pastVelocities.Add(r_interactor.linearVelocity);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            // Average past velocities
            float[] xyzSums = new float[3];
            xyzSums[0] = 0.0f;
            xyzSums[1] = 0.0f;
            xyzSums[2] = 0.0f;

            foreach (Vector3 pastVelocity in pastVelocities)
            {
                xyzSums[0] = xyzSums[0] + pastVelocity.x;
                xyzSums[1] = xyzSums[1] + pastVelocity.y;
                xyzSums[2] = xyzSums[2] + pastVelocity.z;
            }

            float[] xyzAverage = new float[3];
            Vector3 averageInteractorVelocity = new Vector3();
            averageInteractorVelocity.x = xyzAverage[0] / pastVelocities.Count; // FIXME: I think something is going wrong when calculating the average velocity. Either with the assigning to the Vector3 or in the calculation (different types?)
            averageInteractorVelocity.y = xyzAverage[1] / pastVelocities.Count; // FIXME: I think something is going wrong when calculating the average velocity. Either with the assigning to the Vector3 or in the calculation (different types?)
            averageInteractorVelocity.z = xyzAverage[2] / pastVelocities.Count; // FIXME: I think something is going wrong when calculating the average velocity. Either with the assigning to the Vector3 or in the calculation (different types?)

            //Debug.Log("giantHand velocity: " + r_interactor.velocity.ToString());
            collision.rigidbody.AddForce(averageInteractorVelocity, ForceMode.VelocityChange);
            //Vector3 otherColliderVelocity = collision.rigidbody.velocity;
            //Debug.Log("giantHand Velocity: " + otherColliderVelocity.ToString());
            //r_interactor.AddForce(otherColliderVelocity, ForceMode.VelocityChange);
        }
    }
}
