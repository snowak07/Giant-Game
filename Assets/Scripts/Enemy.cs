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
    public Transform playerTransform = null;
    public Transform playerBodyTransform = null;
    public float health { get; set; } // TODO: Make this viewable but not editable in editor?

    public bool IsPickedUp { get; set; }

    protected bool killInstantly;

    // Movement
    protected bool flying;
    protected bool pitch;
    public float maxRotationalSpeed; // Measured in units/s
    public float maxTranslationalSpeed; // Measured in units/s

    protected void Initialize(float health, float maxTranslationalSpeed, float maxRotationalSpeed, bool flying = false, bool pitch = false, bool killInstantly = false)
    {
        this.health = health;
        this.maxTranslationalSpeed = maxTranslationalSpeed;
        this.maxRotationalSpeed = maxRotationalSpeed;
        this.flying = flying;
        this.pitch = pitch;
        this.killInstantly = killInstantly;
    }

    protected void InitializeDestructible(float explosionForce = 200.0f, float explosionRadius = 3.0f, float upwardsExplosionModifier = 3.0f)
    {
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
            foreach (GameObject child in gameObject.GetComponentsInChildren<Transform>().Select(element => element.gameObject))
            {
                Destroy(child);
            }

            return;
        }

        OnKill(); // Handle child OnKill functions. // TODO: Add below calls to OnKill?

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
        if (!ignoredDamageCollisionTags.Contains(collision.transform.root.gameObject.tag))
        {
            if (killInstantly)
            {
                Kill(collision.gameObject);
            }

            if (collision.transform.TryGetComponent(out GiantGrabInteractable forceGrabPullInteractable))
            {
                // Check for impact cooldown so that an Enemy can only be hit once by an object after leaving the player's hand
                if (!forceGrabPullInteractable.ImpactCooldown) // FIXME: Move this to its own component
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

    protected virtual void OnKill() { }

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
        if (GetComponent<PathFollower>().hasPath())
        {
            (Vector3, Quaternion) nextPositionRotation = GetNextTransform(Time.fixedDeltaTime);
            GetComponent<Rigidbody>().MoveRotation(nextPositionRotation.Item2);
            GetComponent<Rigidbody>().MovePosition(nextPositionRotation.Item1);
        }
    }

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
}
