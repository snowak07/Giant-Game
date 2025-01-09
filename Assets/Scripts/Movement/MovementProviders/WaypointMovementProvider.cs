using UnityEngine;

public class WaypointMovementProvider : MovementProvider
{
    public override (Vector3, Quaternion) GetNextTransform(float time)
    {
        float timeRemainingToSimulate = time;
        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;
        PathProvider pathProvider = GetComponent<PathProvider>();

        while (timeRemainingToSimulate > 0)
        {
            (Vector3, Quaternion) waypoint = pathProvider.getNextPathPoint(currentPosition);

            if (pathProvider.arrivedAtFinalPathPoint())
            {
                return (currentPosition, currentRotation);
            }

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
