using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR.OpenXR.Input;

public class BreakableInteractable : MonoBehaviour
{
    public GameObject brokenPiece = null;
    public GrabDirectInteractor interactor = null;
    public NewActionBasedXRController controller = null;
    public float HapticsFrequency;
    [SerializeField] InputActionReference gripHeld = null;
    [SerializeField] InputActionReference triggerHaptics = null;

    //private bool hapticsCooldown = false;

    private void Start()
    {
        gripHeld.action.performed += HandleGripForceAction;
    }

    private void OnDisable()
    {
        gripHeld.action.performed -= HandleGripForceAction;
    }

    private void HandleGripForceAction(InputAction.CallbackContext ctx)
    {
        if (GetComponent<GiantGrabInteractable>() != null && GetComponent<GiantGrabInteractable>().isSelected)
        {
            StartCoroutine(Crush());
        }
    }

    private IEnumerator Crush()
    {
        // Send Haptic Pulse
        OpenXRInput.SendHapticImpulse(triggerHaptics, 1.0f, HapticsFrequency, 1.0f, UnityEngine.InputSystem.XR.XRController.rightHand);

        yield return new WaitForSeconds(1.0f);

        SpawnPieces(interactor.transform);
        Destroy(gameObject);

        yield break;
    }

    public void SpawnPieces(Transform spawnPoint)
    {
        GameObject obj1 = Instantiate(brokenPiece, spawnPoint.position + new Vector3(0.025f, 0, 0), Quaternion.identity);
        Rigidbody body1 = obj1.GetComponent<Rigidbody>();
        interactor.AttachJoint(body1);

        GameObject obj2 = Instantiate(brokenPiece, spawnPoint.position + new Vector3(0.025f, 0.025f, 0), Quaternion.identity);
        Rigidbody body2 = obj2.GetComponent<Rigidbody>();
        interactor.AttachJoint(body2);

        GameObject obj3 = Instantiate(brokenPiece, spawnPoint.position + new Vector3(0.025f, -0.025f, 0), Quaternion.identity);
        Rigidbody body3 = obj3.GetComponent<Rigidbody>();
        interactor.AttachJoint(body3);

        GameObject obj4 = Instantiate(brokenPiece, spawnPoint.position + new Vector3(0.025f, 0, 0.025f), Quaternion.identity);
        Rigidbody body4 = obj4.GetComponent<Rigidbody>();
        interactor.AttachJoint(body4);

        GameObject obj5 = Instantiate(brokenPiece, spawnPoint.position + new Vector3(0.025f, 0, -0.025f), Quaternion.identity);
        Rigidbody body5 = obj5.GetComponent<Rigidbody>();
        interactor.AttachJoint(body5);

        GameObject obj6 = Instantiate(brokenPiece, spawnPoint.position + new Vector3(0.025f, 0.025f, 0.025f), Quaternion.identity);
        Rigidbody body6 = obj6.GetComponent<Rigidbody>();
        interactor.AttachJoint(body6);

        GameObject obj7 = Instantiate(brokenPiece, spawnPoint.position + new Vector3(0.025f, -0.025f, 0.025f), Quaternion.identity);
        Rigidbody body7 = obj7.GetComponent<Rigidbody>();
        interactor.AttachJoint(body7);

        GameObject obj8 = Instantiate(brokenPiece, spawnPoint.position + new Vector3(0.025f, 0.025f, -0.025f), Quaternion.identity);
        Rigidbody body8 = obj8.GetComponent<Rigidbody>();
        interactor.AttachJoint(body8);

        GameObject obj9 = Instantiate(brokenPiece, spawnPoint.position + new Vector3(0.025f, -0.025f, -0.025f), Quaternion.identity);
        Rigidbody body9 = obj9.GetComponent<Rigidbody>();
        interactor.AttachJoint(body9);
    }
}