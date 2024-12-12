using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonTurn : MonoBehaviour
{
    private Transform targetTransform;
    private Vector3 firingDirection;
    private float cannonRotateSpeed = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        targetTransform = GetComponentInParent<Enemy>().targetTransform;
    }

    private void Update()
    {
        Vector3 upwards = new Vector3(0, 1, 0);
        if (firingDirection == Vector3.zero)
        {
            if (targetTransform != null)
            {
                Vector3 direction = targetTransform.position - transform.position;
                Quaternion towardsFiringDirection = Quaternion.LookRotation(direction, upwards);
                Quaternion newDirection = Quaternion.Slerp(transform.rotation, towardsFiringDirection, Time.deltaTime * cannonRotateSpeed);
                transform.rotation = newDirection;
            }
        }
        else
        {
            Quaternion towardsFiringDirection = Quaternion.LookRotation(firingDirection, upwards);
            Quaternion newDirection = Quaternion.Slerp(transform.rotation, towardsFiringDirection, Time.deltaTime * cannonRotateSpeed);
            transform.rotation = newDirection;
        }
    }

    public void setFiringDirection(Vector3 direction)
    {
        firingDirection = direction;
    }
}
