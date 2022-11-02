using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveAbility : MonoBehaviour
{
    public GameObject shockwavePrefab = null;
    public float velocityToActivate = 7;

    /**
     *  Send out gravity-less object with a constant speed and a trigger collider. Any rigidbody it hits on the way has an explosion force applied to it with the explosion at the shockwave object origin.
     */
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" && collision.relativeVelocity.magnitude > velocityToActivate)
        {
            // Create vector on x,z plane pointing straight out from GiantHand
            Vector3 horizontalVector = transform.forward;
            horizontalVector.y = 0;
            horizontalVector = horizontalVector.normalized;

            // Create Quaternion to make shockwave rotation point straight horizontally
            Quaternion horizontal = Quaternion.identity;
            horizontal.SetLookRotation(horizontalVector);

            // Get giant hand position and rotation
            GameObject shockwave = Instantiate(shockwavePrefab, collision.GetContact(0).point, horizontal);
        }
    }
}
