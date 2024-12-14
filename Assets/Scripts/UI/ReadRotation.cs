using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadRotation : MonoBehaviour
{
    public Transform giantHandT;
    public Transform controllerT;
    TextMesh rotationReadOut;

    // Start is called before the first frame update
    void Start()
    {
        rotationReadOut = GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        //rotationReadOut.text = "X:" + giantHandT.rotation.x.ToString("F2") + "Y:" + giantHandT.rotation.y.ToString("F2") + "Z:" + giantHandT.rotation.z.ToString("F2");
        rotationReadOut.text = "GiantHand:" + giantHandT.rotation.ToString() + "\n" + "Controller:" + controllerT.rotation.ToString();
    }
}
