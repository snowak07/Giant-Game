using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.InputSystem.HID;

[RequireComponent(typeof(Destructible))]
[RequireComponent(typeof(MovementProvider))]
public abstract class Enemy : MonoBehaviour
{
    public string[] ignoredDamageCollisionTags = { "Enemy", "GroundEnemy", "Water" };
    public Transform targetTransform = null;
    public float health { get; private set; }
    public bool IsPickedUp { get; set; }
    protected bool killInstantly;

    protected void Initialize(float health, float maxTranslationalSpeed, float maxRotationalSpeed, bool flying = false, bool pitch = false, bool killInstantly = false, float explosionForce = 200.0f, float explosionRadius = 3.0f, float upwardsExplosionModifier = 3.0f)
    {
        this.health = health;
        this.killInstantly = killInstantly;
        GetComponent<MovementProvider>().Initialize(maxTranslationalSpeed, maxRotationalSpeed, flying, pitch);
        GetComponent<Destructible>().Initialize(explosionForce, explosionRadius, upwardsExplosionModifier);
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
     *  Handle killing enemy including disabling animations, breaking them apart, disabling gravity, and setting destroy timer
     *  
     *  @return void
     */
    public virtual void Kill(GameObject killer = null)
    {
        if (killInstantly)
        {
            DestroyAllChildren();
            Destroy(gameObject);
            return;
        }

        HandleKillActions();
    }

    private void DestroyAllChildren()
    {
        foreach (Transform childTransform in GetComponentsInChildren<Transform>())
        {
            if (childTransform.gameObject != gameObject) // Avoid destroying self
            {
                Destroy(childTransform.gameObject);
            }
        }
    }

    private void HandleKillActions()
    {
        OnKill();

        ScoreManager.Add(); // FIXME: Scoreable component?

        GetComponent<Destructible>()?.DestructComponents();
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
        if (IsIgnoredCollision(collision))
        {
            return;
        }

        if (killInstantly)
        {
            Kill(collision.gameObject);
        }

        ProcessCollisionImpact(collision);

        if (health <= 0)
        {
            Kill(collision.gameObject);
        }
    }

    private bool IsIgnoredCollision(Collision collision)
    {
        return ignoredDamageCollisionTags.Contains(collision.transform.root.gameObject.tag);
    }

    private void ProcessCollisionImpact(Collision collision)
    {
        if (collision.transform.TryGetComponent(out GiantGrabInteractable giantGrabInteractable))
        {
            if (!giantGrabInteractable.impactCooldown)
            {
                giantGrabInteractable.impactCooldown = true;
                ApplyDamage(collision);
            }
        }
        else
        {
            ApplyDamage(collision);
        }
    }

    private void ApplyDamage(Collision collision)
    {
        health -= collision.impulse.magnitude;
    }

    protected virtual void OnKill() { }

    /**
     * Where subclasses should handle updating the enemy each frame
     * 
     * @return void
     */
    protected virtual void UpdateEnemy()
    {
        GetComponent<MovementProvider>().HandleMovement();
    }
}
