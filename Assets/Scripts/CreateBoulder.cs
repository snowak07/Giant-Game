using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CreateBoulder : MonoBehaviour
{
    public GameObject boulderPrefab = null;
    public InputActionReference createBoulderActionReference = null;
    public Transform spawnPoint = null;

    // Start is called before the first frame update
    void Start()
    {
        createBoulderActionReference.action.started += Create;
    }

    private void OnDisable()
    {
        createBoulderActionReference.action.started -= Create;
    }

    private void Create(InputAction.CallbackContext ctx)
    {
        Rigidbody boulder = Instantiate(boulderPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<Rigidbody>();

        GetComponent<GrabDirectInteractor>().AttachJoint(boulder);
    }
}
