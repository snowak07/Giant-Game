using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HoveringTarget : MonoBehaviour
{
    public float initialPositionOffset = 2;

    private Vector3 velocityToAddUpwards;
    private Vector3 velocityToAddDownwards;
    private Rigidbody body;
    private Vector3 startPosition;
    private bool oscillateEnabled;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        startPosition = transform.position;
        transform.position = transform.position + new Vector3(0, initialPositionOffset, 0); // Set target to be at max height bound
        oscillateEnabled = false;
        velocityToAddUpwards = new Vector3(0, 0.1f, 0);
        velocityToAddDownwards = new Vector3(0, -0.1f, 0);

        System.Random rd = new System.Random();
        StartCoroutine(InitializeTargetAtTime((float) (rd.NextDouble() * 3)));
    }

    IEnumerator InitializeTargetAtTime(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        oscillateEnabled = true;
    }

    void FixedUpdate()
    {
        if (oscillateEnabled)
        {
            //Vector3 targetTopBound = startPosition + new Vector3(0, initialPositionOffset, 0); // FIXME: Delete. Not used?
            //Vector3 targetBottomBound = startPosition + new Vector3(0, -initialPositionOffset, 0);

            if (transform.position.y >= startPosition.y)
            {
                body.AddForce(velocityToAddDownwards);
            }
            else if (transform.position.y < startPosition.y)
            {
                body.AddForce(velocityToAddUpwards);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        oscillateEnabled = false;
        body.useGravity = true;
    }
}
