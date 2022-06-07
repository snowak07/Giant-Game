using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class giantHandHider : MonoBehaviour
{
    public XRController controller = null;
    public InputActionReference selectReference = null;
    public MeshRenderer giantHandMesh = null;

    // Start is called before the first frame update
    void Start()
    {
        selectReference.action.started += ToggleHideController;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        selectReference.action.started -= ToggleHideController;
    }

    void ToggleHideController(InputAction.CallbackContext context)
    {
        // If currently colliding with object then hide.
        // Or is there a way to check if a controller is attached to an interactable?
        // if ()
    }
}
