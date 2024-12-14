using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShockwaveAbility : MonoBehaviour
{
    public InputActionReference shockwaveAbilityActionReference = null;
    public GameObject shockwavePrefab = null;
    public float velocityToActivate = 7;

    private bool shockwaveEnabled = false;

    /**
     * Handle setting up Action Reference callbacks
     * 
     * @return void
     */
    private void Start()
    {
        shockwaveAbilityActionReference.action.started += EnableShockwave;
        shockwaveAbilityActionReference.action.canceled += DisableShockwave;
    }

    /**
     *  Send out gravity-less object with a constant speed and a trigger collider. Any rigidbody it hits on the way has an explosion force applied to it with the explosion at the shockwave object origin.
     *  
     *  @param collision    Hand collision object
     *  
     *  @return void
     */
    private void OnCollisionEnter(Collision collision)
    {
        if (shockwaveEnabled)
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

    /**
     * Handle disallowing shockwave ability
     * 
     * @param ctx       InputAction context
     * 
     * @return void
     */
    private void DisableShockwave(InputAction.CallbackContext ctx)
    {
        shockwaveEnabled = false;
    }

    /**
     * Handle allowing shockwave ability
     * 
     * @param ctx       InputAction context
     * 
     * @return void
     */
    private void EnableShockwave(InputAction.CallbackContext ctx)
    {
        shockwaveEnabled = true;
    }
}
