using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RayInteractorHandler : MonoBehaviour
{
    public void DisableComponents()
    {
        //GetComponent<XRController>().enabled = false;
        GetComponent<XRRayInteractor>().enabled = false;
        GetComponent<LineRenderer>().enabled = false;
        GetComponent<XRInteractorLineVisual>().enabled = false;
    }

    public void EnableComponents()
    {
        //GetComponent<XRController>().enabled = true;
        GetComponent<XRRayInteractor>().enabled = true;
        GetComponent<LineRenderer>().enabled = true;
        GetComponent<XRInteractorLineVisual>().enabled = true;
    }
}
