using UnityEngine;
using System.Linq;

public class Destructible : MonoBehaviour
{
    protected float explosionForce { get; set; }
    protected float explosionRadius { get; set; }
    protected float upwardsExplosionModifier { get; set; }

    public void Initialize(float explosionForce, float explosionRadius, float upwardsExplosionModifier)
    {
        this.explosionForce = explosionForce;
        this.explosionRadius = explosionRadius;
        this.upwardsExplosionModifier = upwardsExplosionModifier;
    }

    public void DestructComponents(GameObject killer = null)
    {
        GetComponent<Rigidbody>().useGravity = true;

        foreach (var child in gameObject.GetComponentsInChildren<Transform>().Select(element => element.gameObject))
        {
            HandlePhysicsPartOnKill(killer, child);
        }
    }

    private void DisableAnimator()
    {
        if (TryGetComponent(out Animator animator)) // FIXME: AnimationHandler component?
        {
            animator.enabled = false;
        }
    }

    /**
     *  Handle breaking off Enemy piece.
     *  
     *  @return List<GameObject>    Return List of gameobjects so child functions can manipulate objects in their own ways
     */
    protected virtual void DismantleEnemyPart(GameObject part) // FIXME: This could probably be moved to a Dismantleable class
    {
        if (part.TryGetComponent(out MeshRenderer mesh))
        {
            part.transform.parent = null; // Detach enemy part

            Rigidbody childBody;
            if (!part.TryGetComponent(out childBody))
            {
                childBody = part.AddComponent<Rigidbody>();
            }

            if (!part.TryGetComponent(out Collider collider))
            {
                collider = part.AddComponent<BoxCollider>();
            }

            // Add small explosion force to separate the objects on death
            childBody.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsExplosionModifier, ForceMode.Force);
        }
    }

    private void HandlePhysicsPartOnKill(GameObject killer, GameObject child)
    {
        // Remove from Enemy layer and set to Default layer to avoid further interactions of child parts as a result of being on the Enemy layer
        child.layer = LayerMask.NameToLayer("Default");

        if (killer != null && child.TryGetComponent(out Collider childCollider) && killer.TryGetComponent(out Collider killerCollider))
        {
            // Disable collisions between Enemy part and the collider that killed it
            Physics.IgnoreCollision(childCollider, killerCollider);
        }

        if (!child.TryGetComponent(out MeshRenderer mesh))
        {
            // Destroy all non-visible parts and the Parent gameobject
            Destroy(child);
        }
        else
        {
            // Start sink and dismantle visual to rendered parts
            Sinkable sinkable = child.AddComponent<Sinkable>();
            sinkable.EnableSink();
            DismantleEnemyPart(child);
        }
    }
}
