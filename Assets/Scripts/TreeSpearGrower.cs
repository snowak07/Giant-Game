//using System.Numerics;
using UnityEngine;

public class TreeSpearGrower : MonoBehaviour
{
    public GameObject treeSpearPrefab = null;
    protected GameObject spawnedTreeSpear;
    protected Vector3 initialScale = new Vector3(0.1f, 0.1f, 0.1f);
    protected float scaleRatePerSecond = 0.25f;
    protected bool fullyGrown = false;
    protected Vector3 groundOffset = new Vector3(0, 2.5f, 0);

    void Start()
    {
        GrowTreeSpear();
    }

    public void GrowTreeSpear()
    {
        InitializeTreeSpear();
        DisableInteraction();

        fullyGrown = false;
    }

    protected void InitializeTreeSpear()
    {
        Quaternion upwardsRotation = Quaternion.Euler(-90, 0, 0);
        spawnedTreeSpear = Instantiate(treeSpearPrefab, transform.position + initialScale.x * groundOffset, upwardsRotation * transform.rotation);
        spawnedTreeSpear.transform.localScale = initialScale;
    }

    protected void DisableInteraction()
    {
        spawnedTreeSpear.GetComponent<GiantGrabInteractable>().enabled = false; // Disallow pickup
        spawnedTreeSpear.GetComponent<Rigidbody>().isKinematic = true; // Disable movement from collisions
        spawnedTreeSpear.GetComponent<Rigidbody>().useGravity = false;
    }

    protected void EnableInteraction()
    {
        spawnedTreeSpear.GetComponent<Rigidbody>().useGravity = true;
        spawnedTreeSpear.GetComponent<GiantGrabInteractable>().enabled = true;
        spawnedTreeSpear.GetComponent<Rigidbody>().isKinematic = false;
    }

    void FixedUpdate()
    {
        if (!fullyGrown)
        {
            UpdateScale();

            if (spawnedTreeSpear.transform.localScale == Vector3.one)
            {
                EnableInteraction();
                fullyGrown = true;
            }
        }

        if (
            fullyGrown && 
            spawnedTreeSpear != null && 
            spawnedTreeSpear.TryGetComponent(out TreeSpearRoot root) && 
            root.broken
           )
        {
            GrowTreeSpear(); // Loop tree spear growth
        }
    }

    protected void UpdateScale()
    {
        float scaleIncrement = (scaleRatePerSecond * Time.deltaTime);
        spawnedTreeSpear.transform.localScale += new Vector3(scaleIncrement, scaleIncrement, scaleIncrement);
        spawnedTreeSpear.transform.position += scaleIncrement * groundOffset;

        // FIXME: I think using FixedUpdate is causing this issue?
        if (spawnedTreeSpear.transform.localScale.x >= 0.98f) // Not one because it stops before sometimes for some reason
        {
            spawnedTreeSpear.transform.localScale = Vector3.one; // Round off scale values to 1
            spawnedTreeSpear.GetComponent<TreeSpearRoot>().AddRoot();
        }
    }
}
