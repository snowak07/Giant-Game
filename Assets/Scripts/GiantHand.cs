using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantHand : MonoBehaviour
{
    public Transform controllerT = null;
    public float rotationSpeed = 3.0f;
    public float translationSpeed = 3.0f;

    private Transform giantHandT;
    private Rigidbody giantHand;

    void Start()
    {
        giantHand = GetComponent<Rigidbody>();
        giantHandT = GetComponent<Transform>();

        giantHand.interpolation = RigidbodyInterpolation.Interpolate;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Set Rotation
        Quaternion giantHandNewRotation = Quaternion.Slerp(giantHandT.rotation, controllerT.rotation, rotationSpeed * Time.deltaTime);
        //giantHandT.rotation = giantHandNewRotation;
        giantHand.MoveRotation(giantHandNewRotation);

        // Set Position
        Vector3 giantHandNewPosition = Vector3.LerpUnclamped(giantHandT.position, controllerT.position, translationSpeed * Time.deltaTime);
        //giantHandT.position = giantHandNewPosition;
        //giantHand.MovePosition(controllerT.position * Time.deltaTime);
        giantHand.MovePosition(giantHandNewPosition);
    }
}
