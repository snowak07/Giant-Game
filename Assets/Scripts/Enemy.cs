using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public abstract class Enemy : MonoBehaviour
{
    public string[] ignoredDamageCollisionTags = { "Enemy", "Water" };
    public Transform playerTransform = null;
    public Transform playerBodyTransform = null;
    public float health { get; set; }

    public bool IsPickedUp { get; set; }

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
        if (!IsPickedUp)
        {
            UpdateEnemy();
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

    private void HandlePhysicsPartsOnKill(GameObject killer, GameObject child)
    {
        // Remove from Enemy layer and set to Default layer to avoid further interactions of child parts as a result of being on the Enemy layer
        child.layer = LayerMask.NameToLayer("Default");

        if (killer != null && child.TryGetComponent(out Collider childCollider))
        {
            // Disable collisions between Enemy part and the collider that killed it
            Physics.IgnoreCollision(childCollider, killer.GetComponent<Collider>());
        }

        if (!child.TryGetComponent(out MeshRenderer mesh))
        {
            Debug.Log(child.gameObject.name);
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

    /**
     *  Handle killing enemy including disabling animations, breaking them apart, disabling gravity, and setting destroy timer
     *  
     *  @return void
     */
    public virtual void Kill(GameObject killer)
    {
        ScoreManager.Add();

        if (GetComponent<Animator>())
        {
            GetComponent<Animator>().enabled = false;
        }

        GetComponent<Rigidbody>().useGravity = true;

        foreach (var child in gameObject.GetComponentsInChildren<Transform>().Select(element => element.gameObject))
        {
            HandlePhysicsPartsOnKill(killer, child);
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
        if (!ignoredDamageCollisionTags.Contains(collision.transform.root.gameObject.tag))
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
                Kill(collision.gameObject);
            }
        }
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
