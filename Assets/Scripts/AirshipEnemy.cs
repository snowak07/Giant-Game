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
            Vector3 horizontalTowardsDesiredPosition = towardsDesiredPosition;
            horizontalTowardsDesiredPosition.y = 0; // Remove vertical component for rotation so airship doesn't pitch up
            Quaternion desiredRotation = Quaternion.LookRotation(horizontalTowardsDesiredPosition, Vector3.up);
            currentRotation = Quaternion.RotateTowards(currentRotation, desiredRotation, maxRotationalSpeed * Time.deltaTime);

            // Add independant vertical velocity from the horizontal velocity since it always travels forward without pitching up
            Vector3 horizontalTowardsDesiredPositionNormalized = towardsDesiredPosition.normalized;
            horizontalTowardsDesiredPositionNormalized.y = 0;
            Vector3 verticalTowardsDesiredPositionNormalized = towardsDesiredPosition.normalized;
            verticalTowardsDesiredPositionNormalized.x = 0;
            verticalTowardsDesiredPositionNormalized.z = 0;
            Vector3 movementDirection = horizontalTowardsDesiredPositionNormalized.magnitude * Vector3.forward + verticalTowardsDesiredPositionNormalized;
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
