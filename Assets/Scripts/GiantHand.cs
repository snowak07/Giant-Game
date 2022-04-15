using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantHand : MonoBehaviour
{
    public Transform controllerT = null;
    private Transform giantHandT;
    private Rigidbody giantHand;
    private float speed = 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        giantHand = GetComponent<Rigidbody>();
        giantHandT = GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 giantHandToController = controllerT.position - giantHandT.position;
        Vector3 giantHandToControllerAcceleration = speed * giantHandToController;
        Debug.Log(giantHandToControllerAcceleration);
        giantHand.AddForce(giantHandToControllerAcceleration, ForceMode.Acceleration);
        //giantHand.AddTorque(controller.rotation);
    }
}
