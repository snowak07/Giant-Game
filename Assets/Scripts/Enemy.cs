using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

[RequireComponent(typeof(Sinkable))]
public abstract class Enemy : MonoBehaviour
{
    public string[] ignoredDamageCollisionTags = { "Enemy", "Water" };
    public Transform playerTransform = null;
    public Transform playerBodyTransform = null;
    public float health { get; set; }

    public bool IsPickedUp { get; set; }

    protected bool alive = true;
    protected float explosionForce = 200.0f;
    protected float explosionRadius = 3.0f;
    protected float upwardsExplosionModifier = 3.0f;

    protected Enemy(float health)
    {
        this.health = health;
    }

    /**
     * Handle when enemy is first created
     * 
     * @return void
     */
    protected virtual void Start()
    {
        IsPickedUp = false;
    }

    /**
     * Call UpdateEnemy and allow checks for all enemies that can skip updates.
     *
     * TODO: Add different Update functions for particular actions an enemy might take e.g. UpdatePosition, UpdateHealth, etc
     * 
     * @return void
     */
    protected void FixedUpdate()
    {
        if (!IsPickedUp && alive)
        {
            UpdateEnemy();
        }
    }

    /**
     *  Handle breaking enemy into pieces.
     *  
     *  @return List<GameObject>    Return List of gameobjects so child functions can manipulate objects in their own ways
     */
    protected virtual void DismantleEnemy(List<GameObject> children) // FIXME: This could probably be moved to a Dismantleable class
    {
        // Separate each enemy piece that is visible (rendered)
        foreach (var childObject in children) // Get each subobject that has a mesh renderer and set its parent transform to null
        {
            if (childObject.TryGetComponent(out MeshRenderer mesh))
            {
                childObject.transform.parent = null; // Detach enemy part

                Rigidbody childBody;
                if (!childObject.TryGetComponent(out childBody))
                {
                    childBody = childObject.AddComponent<Rigidbody>();
                }

                if (!childObject.TryGetComponent(out Collider collider))
                {
                   collider = childObject.AddComponent<BoxCollider>();
                }
                
                // Add small explosion force to separate the objects on death
                childBody.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsExplosionModifier, ForceMode.Force);
            }
        }
    }

    /**
     * Disable collisions and velocity of objects, slowly sink downward and destroy after some time.
     * 
     * @return IEnumerator
     */
    IEnumerator EnableSink(List<GameObject> children) // FIXME: Can this be moved to the Sinkable class?
    {
        yield return new WaitForSeconds(0.1f);

        foreach (var childObject in children)
        {
            Sinkable sinkable = childObject.AddComponent<Sinkable>();
            sinkable.EnableSink();
        }
    }

    /**
     *  Handle colliding with objects that could potentially kill the enemy.
     *  
     *  @param collision    collision that can do damage to enemy
     *  
     *  @return void
     */
    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (alive && !ignoredDamageCollisionTags.Contains(collision.transform.root.gameObject.tag))
        {
            if (collision.transform.TryGetComponent(out GiantGrabInteractable forceGrabPullInteractable))
            {
                // Check for impact cooldown so that an Enemy can only be hit once by an object after leaving the player's hand
                if (!forceGrabPullInteractable.ImpactCooldown)
                {
                    forceGrabPullInteractable.ImpactCooldown = true;

                    health -= collision.impulse.magnitude;
                }
            }
            else
            {
                health -= collision.impulse.magnitude;
            }

            if (health <= 0)
            {
                // Disable collision between the killing object and the enemy
                Collider[] colliders = GetComponentsInChildren<Collider>();
                foreach (var childCollider in colliders)
                {
                    Physics.IgnoreCollision(childCollider, collision.collider);
                }

                Kill();
            }
        }
    }

    /**
     *  Handle killing enemy including disabling animations, breaking them apart, disabling gravity, and setting destroy timer
     *  
     *  @return void
     */
    public virtual void Kill()
    {
        alive = false; // FIXME: is this used anywhere? Parent object gets destroyed right after.

        // Increment score
        ScoreManager.Add();

        if (GetComponent<Animator>())
        {
            GetComponent<Animator>().enabled = false;
        }

        GetComponent<Rigidbody>().useGravity = true;

        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        List<GameObject> renderedChildObjects = new List<GameObject>();
        foreach (var child in children)
        {
            // Remove from Enemy layer and set to Default layer to avoid further interactions as a result of being an Enemy
            child.gameObject.layer = LayerMask.NameToLayer("Default");

            //Destroy(child.gameObject);
            if (!child.TryGetComponent(out MeshRenderer mesh))
            {
                // Destroy all non-visible objects
                Destroy(child.gameObject, 5); // FIXME: Why is this not immediate?
            }
            else
            {
                renderedChildObjects.Add(child.gameObject);
            }
        }

        DismantleEnemy(renderedChildObjects);
        StartCoroutine(EnableSink(renderedChildObjects));
        //Destroy(gameObject); // FIXME: this might cause issues with the EnableSink coroutine
    }

    /**
     * Set player body transform so enemies can react to the player's body position
     * 
     * @param pbTransform    transform of the player's body
     * 
     * @return void
     */
    public void setPlayerBodyTransform(Transform pbTransform)
    {
        playerBodyTransform = pbTransform;
    }

    /**
     * Set player transform so enemies can react to the player's position
     * 
     * @param pTransform    transform of the player
     * 
     * @return void
     */
    public void setPlayerTransform(Transform pTransform)
    {
        playerTransform = pTransform;
    }

    /**
     * Where subclasses should handle updating the enemy each frame
     * 
     * @return void
     */
    protected virtual void UpdateEnemy()
    {
        (Vector3, Quaternion) nextPositionRotation = GetNextTransform(Time.fixedDeltaTime);
        GetComponent<Rigidbody>().MoveRotation(nextPositionRotation.Item2);
        GetComponent<Rigidbody>().MovePosition(nextPositionRotation.Item1);
    }

    public abstract (Vector3, Quaternion) GetNextTransform(float time, bool applyTargetingOffset = false);
}
