using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushInteractable : MonoBehaviour
{
    Rigidbody r_interactable;

    // Start is called before the first frame update
    void Start()
    {
        r_interactable = GetComponent<Rigidbody>();
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            Vector3 otherColliderVelocity = collision.rigidbody.velocity;
            Debug.Log("giantHand Velocity: " + otherColliderVelocity.ToString());
            r_interactable.AddForce(otherColliderVelocity, ForceMode.VelocityChange);
        }
    }
}
