using UnityEngine;

public class InteractableGrower : MonoBehaviour
{
    public GameObject interactablePrefab = null;
    protected GameObject spawnedInteractable;
    protected Vector3 initialScale = new Vector3(0.1f, 0.1f, 0.1f);
    protected float scaleRatePerSecond = 0.25f;
    protected bool fullyGrown = false;
    protected Vector3 groundOffset = new Vector3(0, 0, 0);
    protected Vector3 rootOffset = new Vector3(0, 0, 0);

    protected virtual void Start()
    {
        GrowInteractable();
    }

    public void GrowInteractable()
    {
        InitializeInteractable();
        DisableInteraction();

        fullyGrown = false;
    }

    protected void InitializeInteractable()
    {
        Quaternion upwardsRotation = Quaternion.Euler(-90, 0, 0);
        spawnedInteractable = Instantiate(interactablePrefab, transform.position + initialScale.x * groundOffset, upwardsRotation * transform.rotation);
        spawnedInteractable.transform.localScale = initialScale;
    }

    protected void DisableInteraction()
    {
        spawnedInteractable.GetComponent<GiantGrabInteractable>().enabled = false; // Disallow pickup
        spawnedInteractable.GetComponent<Rigidbody>().isKinematic = true; // Disable movement from collisions
        spawnedInteractable.GetComponent<Rigidbody>().useGravity = false;
    }

    protected void EnableInteraction()
    {
        spawnedInteractable.GetComponent<Rigidbody>().useGravity = true;
        spawnedInteractable.GetComponent<GiantGrabInteractable>().enabled = true;
        spawnedInteractable.GetComponent<Rigidbody>().isKinematic = false;
    }

    void Update()
    {
        if (!fullyGrown)
        {
            UpdateScale();

            if (spawnedInteractable.transform.localScale == Vector3.one)
            {
                EnableInteraction();
                fullyGrown = true;
            }
        }

        if (InteractableFullyGrownAndUprooted())
        {
            GrowInteractable(); // Loop interactable spear growth
        }
    }

    protected void UpdateScale()
    {
        float scaleIncrement = (scaleRatePerSecond * Time.deltaTime);
        spawnedInteractable.transform.localScale += new Vector3(scaleIncrement, scaleIncrement, scaleIncrement);
        spawnedInteractable.transform.position += scaleIncrement * groundOffset;

        if (spawnedInteractable.transform.localScale.x >= 1.0f)
        {
            spawnedInteractable.transform.localScale = Vector3.one; // Round off scale values to 1
            spawnedInteractable.GetComponent<InteractableRoot>().AddRoot(rootOffset);
        }
    }

    protected bool InteractableFullyGrownAndUprooted()
    {
        return
            fullyGrown &&
            spawnedInteractable != null &&
            spawnedInteractable.TryGetComponent(out InteractableRoot root) &&
            root.broken;
    }
}
