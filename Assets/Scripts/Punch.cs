using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour
{
    public float punchInitialVelocity = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * punchInitialVelocity, ForceMode.VelocityChange);
    }
}
