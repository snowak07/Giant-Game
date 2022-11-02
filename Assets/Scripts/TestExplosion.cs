using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestExplosion : MonoBehaviour
{
    public float explosionForce = 10.0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position - new Vector3(0, -0.05f, 0), 10.0f, 3.0f, ForceMode.Impulse);
    }
}
