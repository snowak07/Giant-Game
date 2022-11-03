using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public abstract class Enemy : MonoBehaviour
{
    public string[] ignoredDamageCollisionTags = { "Enemy", "Water" };
    public Transform playerTransform = null;
    public float health;

    public bool IsPickedUp { get; set; }

    protected bool isKilled = false;

    /**
     * Handle when enemy is first created
     * 
     * @return void
     */
    protected virtual void Start()
    {
        IsPickedUp = false;

        SetStartingHealth();
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
        if (!IsPickedUp && !isKilled)
        {
            UpdateEnemy();
        }
    }

    /**
     *  Handle breaking enemy into pieces.
     *  
     *  @return List<GameObject>    Return List of gameobjects so child functions can manipulate objects in their own ways
     */
    protected virtual List<GameObject> DismantleEnemy()
    {
        // Separate each enemy piece that is visible (rendered)
        MeshRenderer[] childMeshes = GetComponentsInChildren<MeshRenderer>();
        List<GameObject> childObjects = new List<GameObject>();
        foreach (var childMesh in childMeshes) // Get each subobject that has a mesh renderer and set its parent transform to null
        {
            GameObject childObject = childMesh.gameObject;
            childObject.transform.parent = null; // Detach enemy part

            Rigidbody childBody;
            if (!childObject.TryGetComponent(out childBody))
            {
                childBody = childObject.AddComponent<Rigidbody>();
            }

            if (!childObject.TryGetComponent(out Collider collider))
            {
                childObject.AddComponent<BoxCollider>();
            }

            // Add small explosion force to separate the objects on death
            childBody.AddExplosionForce(200.0f, transform.position, 3.0f, 3.0f);

            childObjects.Add(childObject);
        }

        return childObjects; // Return childObjects so that child functions can act on them.
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
        Debug.Log("Enemy damage dealt: " + collision.impulse.magnitude);
        if (!isKilled && !ignoredDamageCollisionTags.Contains(collision.transform.root.gameObject.tag))
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
        isKilled = true;

        // Increment score
        ScoreManager.Add();

        if (GetComponent<Animator>())
        {
            GetComponent<Animator>().enabled = false;
        }

        GetComponent<Rigidbody>().useGravity = true;
        DismantleEnemy();

        Destroy(gameObject, 60); // Destroy parent object and any remaining children
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
     * Handle setting starting health
     * 
     * @return void
     */
    protected abstract void SetStartingHealth();

    /**
     * Where subclasses should handle updating the enemy each frame
     * 
     * @return void
     */
    protected abstract void UpdateEnemy();
}
