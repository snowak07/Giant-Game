using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class GiantBody : MonoBehaviour
{
    public Transform mainCamera = null;
    public CapsuleCollider bodyCollider = null;
    public Transform attackPosition = null;

    private float minBodyHeight = 0.25f;
    private float attackPositionOffset = 0.5f;
    //private float heightChangeAmount = 0.1f;

    // Update is called once per frame
    void FixedUpdate()
    {
        // Track body below the camera (x,z)
        Vector3 newHeadTrackedPosition = new Vector3(mainCamera.localPosition.x, transform.localPosition.y, mainCamera.localPosition.z);
        transform.localPosition = newHeadTrackedPosition;

        // Always keep body upright
        transform.rotation = Quaternion.identity;

        // Lower body height depending on mainCamera height
        float colliderTopPosition = transform.localPosition.y + (1.0f / 2.0f * bodyCollider.height);
        float giantHeadTopPosition = mainCamera.localPosition.y + mainCamera.GetComponentInChildren<SphereCollider>().radius;
        float colliderHeightAdjustment = (giantHeadTopPosition - colliderTopPosition) * 2;
        bodyCollider.height = Mathf.Max(bodyCollider.height + colliderHeightAdjustment, minBodyHeight);

        // Set attackposition to constant offset from bottom of head
        Vector3 newAttackPosition = new Vector3(mainCamera.localPosition.x, giantHeadTopPosition - attackPositionOffset, mainCamera.localPosition.z);
        attackPosition.localPosition = newAttackPosition;
    }
}
