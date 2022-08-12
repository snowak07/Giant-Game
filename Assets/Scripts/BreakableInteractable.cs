using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class BreakableInteractable : MonoBehaviour
{
    public GameObject brokenPiece = null;
    public GrabDirectInteractor interactor = null;
    public NewActionBasedXRController controller = null;
    public InputActionReference gripHeld = null;
    public InputActionReference gripHeldHaptics = null;

    private bool hapticsCooldown = false;

    private void Start()
    {
        gripHeld.action.started += SpawnPieces2;
        gripHeldHaptics.action.performed += TriggerSqueezeHaptics;
        gripHeldHaptics.action.canceled += TriggerSqueezeHapticsCooldownReset;
    }

    private void OnDisable()
    {
        gripHeld.action.started -= SpawnPieces2;
        gripHeldHaptics.action.performed -= TriggerSqueezeHaptics;
        gripHeldHaptics.action.canceled -= TriggerSqueezeHapticsCooldownReset;
    }

    private void SpawnPieces2(InputAction.CallbackContext ctx)
    {
        Debug.Log("SpawnPieces2");
        Debug.Log("SpawnPieces2 isSelected: " + GetComponent<ForceGrabPullInteractable>().isSelected);
        if (GetComponent<ForceGrabPullInteractable>() != null && GetComponent<ForceGrabPullInteractable>().isSelected)
        {
            Debug.Log("SpawnPieces2 About to call SpawnPieces");
            SpawnPieces(interactor.transform);
            Destroy(gameObject);
        }
    }

    public void SpawnPieces(Transform spawnPoint)
    {
        Debug.Log("SpawnPieces");
        GameObject obj1 = Instantiate(brokenPiece, spawnPoint.position, Quaternion.identity);
        Rigidbody body1 = obj1.GetComponent<Rigidbody>();
        interactor.AttachJoint(body1);

        GameObject obj2 = Instantiate(brokenPiece, spawnPoint.position + new Vector3(0.025f, 0, 0), Quaternion.identity);
        Rigidbody body2 = obj2.GetComponent<Rigidbody>();
        interactor.AttachJoint(body2);

        GameObject obj3 = Instantiate(brokenPiece, spawnPoint.position + new Vector3(0, 0.025f, 0), Quaternion.identity);
        Rigidbody body3 = obj3.GetComponent<Rigidbody>();
        interactor.AttachJoint(body3);

        GameObject obj4 = Instantiate(brokenPiece, spawnPoint.position + new Vector3(0, 0, 0.025f), Quaternion.identity);
        Rigidbody body4 = obj4.GetComponent<Rigidbody>();
        interactor.AttachJoint(body4);

        GameObject obj5 = Instantiate(brokenPiece, spawnPoint.position + new Vector3(0.025f, 0.025f, 0), Quaternion.identity);
        Rigidbody body5 = obj5.GetComponent<Rigidbody>();
        interactor.AttachJoint(body5);
    }

    public void TriggerSqueezeHaptics(InputAction.CallbackContext ctx)
    {
        Debug.Log("TriggerSqueezeHaptics");
        Debug.Log("IsSelected: " + GetComponent<ForceGrabPullInteractable>().isSelected);
        if (!hapticsCooldown && ctx.performed && !ctx.canceled)
        {
            hapticsCooldown = true;
            //if (GetComponent<ForceGrabPullInteractable>().isSelected) // TODO: Controller isn't being registered as selected for reason? It does register as selected in the other function though.
            //{
            Debug.Log("About to trigger haptics");
            //interactor.gameObject.GetComponent<NewActionBasedXRController>().SendHapticImpulse(0.5f, 2);
            controller.SendHapticImpulse(0.5f, 2);
            //}
        }
    }

    public void TriggerSqueezeHapticsCooldownReset(InputAction.CallbackContext ctx)
    {
        Debug.Log("TriggerSqueezeHapticsCooldownReset");
    }
}