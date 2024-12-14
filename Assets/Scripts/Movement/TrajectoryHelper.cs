using UnityEngine;

public delegate (Vector3, Quaternion) GetNextTransformCallback(float time, bool applyTargetOffset = false);

public static class TrajectoryHelper
{
    /**
     * Returns the projectile velocity vector required to hit targetPosition from firingPosition with projectileSpeed
     * 
     * @param Vector3
     * @param Vector3
     * @param float
     */
    public static Vector3 CalculateFiringDirection(Vector3 firingPosition, Vector3 targetPosition, float projectileSpeed)
    {
        float t = CalculateTimeToImpact(firingPosition, targetPosition, projectileSpeed);

        if (t != float.PositiveInfinity)
        {
            Vector3 directDistanceVector = targetPosition - firingPosition;

            float y = (directDistanceVector.y + (0.5f * Mathf.Abs(Physics.gravity.y) * Mathf.Pow(t, 2))) / t;
            float x = directDistanceVector.x / t;
            float z = directDistanceVector.z / t;

            Vector3 projectileVector = new Vector3(x, y, z);
            return projectileVector;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private static float CalculateTimeToImpact(Vector3 firingPosition, Vector3 targetPosition, float projectileSpeed)
    {
        float t1 = CalculateTimeToImpact1(firingPosition, targetPosition, projectileSpeed);
        float t2 = -t1;
        float t3 = CalculateTimeToImpact2(firingPosition, targetPosition, projectileSpeed);
        float t4 = -t3;

        float t = float.PositiveInfinity;
        if (!float.IsNaN(t1) && t1 > 0 && t1 < t)
        {
            t = t1;
        }

        if (!float.IsNaN(t2) && t2 > 0 && t2 < t)
        {
            t = t2;
        }

        if (!float.IsNaN(t3) && t3 > 0 && t3 < t)
        {
            t = t3;
        }

        if (!float.IsNaN(t4) && t4 > 0 && t4 < t)
        {
            t = t4;
        }

        return t;
    }

    private static float CalculateTimeToImpact1(Vector3 firingPosition, Vector3 targetPosition, float projectileSpeed)
    {
        float y = targetPosition.y - firingPosition.y;
        Vector3 horizontal = targetPosition - firingPosition;
        horizontal.y = 0;
        float xz = horizontal.magnitude;

        return Mathf.Sqrt(2 * (Mathf.Pow(projectileSpeed, 2) + Mathf.Sqrt(Mathf.Pow(projectileSpeed, 4) - 2 * Mathf.Pow(projectileSpeed, 2) * y * Physics.gravity.y - Mathf.Pow(xz * Physics.gravity.y, 2)) - y * Physics.gravity.y)) / Physics.gravity.y;
    }

    private static float CalculateTimeToImpact2(Vector3 firingPosition, Vector3 targetPosition, float projectileSpeed)
    {
        float y = targetPosition.y - firingPosition.y;
        Vector3 horizontal = targetPosition - firingPosition;
        horizontal.y = 0;
        float xz = horizontal.magnitude;

        return Mathf.Sqrt(2 * (Mathf.Pow(projectileSpeed, 2) - Mathf.Sqrt(Mathf.Pow(projectileSpeed, 4) - 2 * Mathf.Pow(projectileSpeed, 2) * y * Physics.gravity.y - Mathf.Pow(xz * Physics.gravity.y, 2)) - y * Physics.gravity.y)) / Physics.gravity.y;
    }

    public static Vector3 CalculateInterceptionDirection(Vector3 pursuerPosition, float pursuerSpeed, GetNextTransformCallback getNextTransformCallback)
    {
        // Initial guess for the time of interception
        float time = 0;
        (Vector3, Quaternion) evaderTransform = getNextTransformCallback(time);

        // Calculate the relative position
        Vector3 relativePosition = evaderTransform.Item1 - pursuerPosition; // FIXME: No longer needed I think

        // Loop to find the interception time
        float tIncrement = 0.1f; // Time increment for the loop
        float tolerance = 0.1f; // Tolerance for stopping the loop

        while (true)
        {
            // Calculate the evader's position at the current time
            evaderTransform = getNextTransformCallback(time);

            float tPursuer = CalculateTimeToImpact(pursuerPosition, evaderTransform.Item1, pursuerSpeed);

            // Check if the difference between the current time and the calculated time is within the tolerance
            if (Mathf.Abs(tPursuer - time) < tolerance)
            {
                break;
            }

            if (pursuerPosition.y < -10)
            {
                return Vector3.zero;
            }

            // Increment the time
            time += tIncrement;
        }

        // Calculate the required direction
        Vector3 interceptPoint = getNextTransformCallback(time).Item1;
        Vector3 direction = CalculateFiringDirection(pursuerPosition, interceptPoint, pursuerSpeed).normalized;

        return direction;
    }
}
