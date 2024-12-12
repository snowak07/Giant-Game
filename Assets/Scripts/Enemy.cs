using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.InputSystem.HID;

[RequireComponent(typeof(Destructible))]
[RequireComponent(typeof(PathFollower))]
public abstract class Enemy : MonoBehaviour
{
    public string[] ignoredDamageCollisionTags = { "Enemy", "Water" };
    public Transform targetTransform = null;

    public float health { get; private set; }

    public bool IsPickedUp { get; set; }

    protected bool killInstantly;

    ////////////////////////// MovementProvider //////////////////////////
    // Movement
    protected bool flying;
    protected bool pitch;
    public float maxRotationalSpeed; // Measured in units/s
    public float maxTranslationalSpeed; // Measured in units/s
    ////////////////////////// MovementProvider //////////////////////////

    protected void Initialize(float health, float maxTranslationalSpeed, float maxRotationalSpeed, bool flying = false, bool pitch = false, bool killInstantly = false, float explosionForce = 200.0f, float explosionRadius = 3.0f, float upwardsExplosionModifier = 3.0f)
    {
        this.health = health;
        this.maxTranslationalSpeed = maxTranslationalSpeed;
        this.maxRotationalSpeed = maxRotationalSpeed;
        this.flying = flying;
        this.pitch = pitch;
        this.killInstantly = killInstantly;
        GetComponent<Destructible>().Initialize(explosionForce, explosionRadius, upwardsExplosionModifier);
        ////////////////////////// MovementProvider //////////////////////////
        // Initialize here
        ////////////////////////// MovementProvider //////////////////////////
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
        OnKill(); // Handle child OnKill functions.

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
     * Set player transform so enemies can react to the player's position
     * 
     * @param pTransform    transform of the player
     * 
     * @return void
     */
    public void setTargetTransform(Transform pTransform)
    {
        targetTransform = pTransform;
    }

    /**
     * Where subclasses should handle updating the enemy each frame
     * 
     * @return void
     */
    protected virtual void UpdateEnemy()
    {
        ////////////////////////// MovementProvider //////////////////////////
        // Call WaypointMovementProvider.HandleMovement
        if (GetComponent<PathFollower>().hasPath())
        {
            (Vector3, Quaternion) nextPositionRotation = GetNextTransform(Time.fixedDeltaTime);
            GetComponent<Rigidbody>().MoveRotation(nextPositionRotation.Item2);
            GetComponent<Rigidbody>().MovePosition(nextPositionRotation.Item1);
        }
        ////////////////////////// MovementProvider //////////////////////////
    }

    ////////////////////////// MovementProvider //////////////////////////
    public virtual (Vector3, Quaternion) GetNextTransform(float time, bool applyTargetingOffset = false)
    {
        float timeRemainingToSimulate = time;
        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;
        PathFollower pathFollower = GetComponent<PathFollower>();

        while (timeRemainingToSimulate > 0)
        {
            (Vector3, Quaternion) waypoint = pathFollower.getNextPathPoint(currentPosition);

            Vector3 towardsDesiredPosition = waypoint.Item1 - currentPosition;
            Vector3 rotationDirection = towardsDesiredPosition;
            if (!pitch)
            {
                rotationDirection = new Vector3(towardsDesiredPosition.x, 0, towardsDesiredPosition.z); // Remove vertical component for rotation
            }
            Quaternion desiredRotation = Quaternion.LookRotation(rotationDirection, Vector3.up);
            currentRotation = Quaternion.RotateTowards(currentRotation, desiredRotation, maxRotationalSpeed * Time.deltaTime);

            Vector3 movementDirection = Vector3.forward;
            if (flying && !pitch)
            {
                Vector3 horizontalMovementComponent = new Vector3(towardsDesiredPosition.normalized.x, 0, towardsDesiredPosition.normalized.z);
                Vector3 verticalMovementComponent = new Vector3(0, towardsDesiredPosition.normalized.y, 0);
                movementDirection = horizontalMovementComponent.magnitude * Vector3.forward + verticalMovementComponent;
            }
            currentPosition = (maxTranslationalSpeed * Time.deltaTime) * (currentRotation * movementDirection) + currentPosition;

            timeRemainingToSimulate -= Time.fixedDeltaTime;
        }

        return (currentPosition, currentRotation);
    }
    ////////////////////////// MovementProvider //////////////////////////
}
