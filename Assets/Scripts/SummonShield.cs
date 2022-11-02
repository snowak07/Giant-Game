using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * TODO Create a script that will break a mesh into pieces. Make the mesh break along points randomly placed within it. Could just manually place points to start out.
 */
public class SummonShield : MonoBehaviour
{
    public InputActionReference summonActionReference = null;

    private bool shieldActivated = false;
    private float xMinRadius;
    private float xMaxRadius;
    private float zMinRadius;
    private float zMaxRadius;
    private float numSteps = 50.0f;
    private float xGlobalScaleFactor;
    private float zGlobalScaleFactor;
    private float relativeScaleFactor;

    // Start is called before the first frame update
    void Start()
    {
        shieldActivated = false;
        HideShield();

        xGlobalScaleFactor = 1 / gameObject.transform.lossyScale.x;
        zGlobalScaleFactor = 1 / gameObject.transform.lossyScale.z;

        relativeScaleFactor = gameObject.transform.localScale.x / gameObject.transform.localScale.z;

        xMinRadius = gameObject.transform.localScale.x;
        xMaxRadius = gameObject.transform.localScale.x * 2.8f * xGlobalScaleFactor;

        zMinRadius = gameObject.transform.localScale.z;
        zMaxRadius = gameObject.transform.localScale.z * 2.8f * zGlobalScaleFactor;

        summonActionReference.action.started += HandleSummonActionStart;
        summonActionReference.action.canceled += HandleSummonActionEnd;
    }

    void FixedUpdate()
    {
        Vector3 newScale = new Vector3(gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.localScale.z);

        float xStepSize = (xMaxRadius - xMinRadius) / numSteps;
        float zStepSize = (zMaxRadius - zMinRadius) / numSteps;

        if (shieldActivated)
        {
            if (gameObject.transform.localScale.x < xMaxRadius)
            {
                newScale.x += xStepSize;
            }

            if (gameObject.transform.localScale.z < zMaxRadius)
            {
                newScale.z += zStepSize;
            }
        }
        else
        {
            if (gameObject.transform.localScale.x > xMinRadius)
            {
                newScale.x -= xStepSize;
            }

            if (gameObject.transform.localScale.z > zMinRadius)
            {
                newScale.z -= zStepSize;
            }
        }

        gameObject.transform.localScale = newScale;

        if (gameObject.transform.localScale.x <= xMinRadius && gameObject.transform.localScale.z <= zMinRadius)
        {
            HideShield();
        }
    }

    private void ShowShield()
    {
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<MeshCollider>().enabled = true;
    }

    private void HideShield()
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<MeshCollider>().enabled = false;
    }

    private void HandleSummonActionEnd(InputAction.CallbackContext ctx)
    {
        shieldActivated = false;
    }

    private void HandleSummonActionStart(InputAction.CallbackContext ctx)
    {
        shieldActivated = true;
        ShowShield();
    }
}
