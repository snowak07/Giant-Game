using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RayInteractorHandler : MonoBehaviour
{
    public void DisableComponents()
    {
        //GetComponent<XRController>().enabled = false;
        GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>().enabled = false;
        GetComponent<LineRenderer>().enabled = false;
        GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals.XRInteractorLineVisual>().enabled = false;
    }

    public void EnableComponents()
    {
        //GetComponent<XRController>().enabled = true;
        GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>().enabled = true;
        GetComponent<LineRenderer>().enabled = true;
        GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals.XRInteractorLineVisual>().enabled = true;
    }
}
