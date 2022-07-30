using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandToggler : MonoBehaviour
{
    private Collider controllerCollider;
    private MeshRenderer controllerMesh;

    // Start is called before the first frame update
    void Start()
    {
        controllerCollider = GetComponent<Collider>();
        controllerMesh = GetComponent<MeshRenderer>();
    }

    public void ToggleController()
    {
        controllerCollider.enabled = !controllerCollider.enabled;
        controllerMesh.enabled = !controllerMesh.enabled;
    }
}
