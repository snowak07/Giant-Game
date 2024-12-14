using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PunchThrow : MonoBehaviour
{
    public InputActionReference controllerVelocityReference;
    public GameObject punchProjectile = null;
    public float minPunchingSpeed = 0.0f;
    public float punchThrowSpeed = 0.1f;
    public float throwPunchDeltaVelocityThreshold = 0.0f;
    public float punchCooldownTime = 2.0f;

    private bool punchPossible = false;
    private bool punchCooldown = false;
    private Vector3 controllerVelocity;
    private Vector3 previousVelocity;

    private void Start()
    {
        previousVelocity = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        controllerVelocity = controllerVelocityReference.action.ReadValue<Vector3>();
        // Maybe check if speed is reached in the direction of punchThrowDirection?

        // Check if minimum speed is reached (min punching speed) and then set to punchPossible, then when min speed (or acceleration?) is reached send out projectile
        bool previousPunchPossibleValue = punchPossible;
        if (controllerVelocity.magnitude > minPunchingSpeed)
        {
            if (!punchPossible)
            {
                punchPossible = true;
                GetComponentInChildren<Renderer>().material.color = Color.red;
            }
        }
        else
        {
            //punchPossible = false;
            //GetComponentInChildren<Renderer>().material.color = Color.gray;
        }
        if (previousPunchPossibleValue != punchPossible)
        {
            Debug.Log("punchPossible" + punchPossible);
        }

        float speedDifference = previousVelocity.magnitude - controllerVelocity.magnitude;
        //Debug.Log("speedDifference: " + speedDifference);
        if (!punchCooldown && punchPossible && controllerVelocity.magnitude < punchThrowSpeed) // && speedDifference > throwPunchDeltaVelocityThreshold
        {
            // Throw projectile
            Instantiate(punchProjectile, transform.position, transform.rotation).GetComponent<Rigidbody>();

            Debug.Log("punch thrown, Cooldown STARTED");

            // Punch cooldown
            StartCoroutine(StartPunchCooldown());
        }

        previousVelocity = controllerVelocity;
    }

    IEnumerator StartPunchCooldown()
    {
        punchCooldown = true;
        yield return new WaitForSeconds(punchCooldownTime);
        Debug.Log("Punch Cooldown FINISHED");
        punchCooldown = false;
        punchPossible = false; // TODO remove
    }
}
