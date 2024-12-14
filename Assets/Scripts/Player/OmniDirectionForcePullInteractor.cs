using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OmniDirectionForcePullInteractor : MonoBehaviour
{
    public InputActionReference pullActionReference = null;
    public Rigidbody r_forcePullObject = null;
    public Transform t_forcePullObject = null;

    private Transform t_pullLocation;

    // Start is called before the first frame update
    void Start()
    {
        t_pullLocation = GetComponent<Transform>();

        pullActionReference.action.started += forcePull;
    }

    private void OnDisable()
    {
        pullActionReference.action.started -= forcePull;
    }

    private void forcePull(InputAction.CallbackContext ctx)
    {
        Vector3 directDistanceVector = t_pullLocation.position - t_forcePullObject.position;
        
        // Calculate Velocity y vector needed to be at the pull y position in 1 second
        float y = directDistanceVector.y + (0.5f * Mathf.Abs(Physics.gravity.y));
        
        Vector3 projectileVector = new Vector3(directDistanceVector.x, y, directDistanceVector.z);
        
        r_forcePullObject.AddForce(projectileVector - r_forcePullObject.linearVelocity, ForceMode.VelocityChange);
    }
}
