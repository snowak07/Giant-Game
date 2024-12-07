using UnityEngine;

[RequireComponent(typeof(PathFollower))]
[RequireComponent(typeof(ProjectileLauncher))]
public class AirshipEnemy : Enemy
{
    public float maxRotationalSpeed = 20; // Measured in units/s
    public float maxTranslationalSpeed = 3; // Measured in units/s

    protected AirshipEnemy(float health, bool killInstantly = false) : base(20.0f) { }

    public override (Vector3, Quaternion) GetNextTransform(float time, bool applyTargetingOffset = false)
    {
        float timeRemainingToSimulate = time;
        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;
        PathFollower pathFollower = GetComponent<PathFollower>();

        while (timeRemainingToSimulate > 0)
        {
            (Vector3, Quaternion) waypoint = pathFollower.getNextPathPoint(currentPosition);

            Vector3 towardsDesiredPosition = waypoint.Item1 - currentPosition;
            Vector3 horizontalTowardsDesiredPosition = new Vector3(towardsDesiredPosition.x, 0, towardsDesiredPosition.z);
            Quaternion desiredRotation = Quaternion.LookRotation(horizontalTowardsDesiredPosition, Vector3.up);
            currentRotation = Quaternion.RotateTowards(currentRotation, desiredRotation, maxRotationalSpeed * Time.deltaTime);

            // Add independant vertical velocity from the horizontal velocity since it always travels forward without pitching up
            Vector3 horizontalMovementComponent = new Vector3(towardsDesiredPosition.normalized.x, 0, towardsDesiredPosition.normalized.z);
            Vector3 verticalMovementComponent = new Vector3(0, towardsDesiredPosition.normalized.y, 0);
            Vector3 movementDirection = horizontalMovementComponent.magnitude * Vector3.forward + verticalMovementComponent;
            currentPosition = (maxTranslationalSpeed * Time.deltaTime) * (currentRotation * movementDirection) + currentPosition;

            timeRemainingToSimulate -= Time.fixedDeltaTime;
        }

        return (currentPosition, currentRotation);
    }

    protected override void UpdateEnemy()
    {
        base.UpdateEnemy();

        if (playerTransform != null)
        {
            GetComponent<ProjectileLauncher>().ShootProjectile(playerTransform.position);
        }
    }
}
