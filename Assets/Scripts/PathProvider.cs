using System;
using UnityEngine;

public class PathProvider : MonoBehaviour
{
    public Transform[] pathPoints = null;

    // Used to keep the PathProvider going to each point instead of getting stuck at one path point
    protected int lastPointIndex = -1;
    protected float arrivedThreshold = 1f;

    // TODO: Handle getting future position calls by not changing lastPointIndex or setting to a temp lastPointIndex value
    // TODO: Correct for a PathFollowers max rotational speed so that it doesn't try to loop around when it doesn't quite hit a waypoint

    public bool hasPath()
    {
        return pathPoints != null && pathPoints.Length > 0;
    }

    protected int getClosestPathPointIndex(Vector3 currentPosition)
    {
        int closestPointIndex = 0;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < pathPoints.Length; i++)
        {
            Transform point = pathPoints[i];
            float distance = (point.position - currentPosition).magnitude;
            if (distance < closestDistance)
            {
                closestPointIndex = i;
                closestDistance = distance;
            }
        }

        UpdateLastPointIndex(closestDistance, closestPointIndex);

        return closestPointIndex;
    }

    public (Vector3, Quaternion) getNextPathPoint(Vector3 currentPosition)
    {
        if (pathPoints.Length == 0)
        {
            throw new InvalidOperationException("The PathProvider does not have a Path.");
        }

        int closestPointIndex = getClosestPathPointIndex(currentPosition);
        if (closestPointIndex == lastPointIndex)
        {
            // Return next point if the current closest has already been passed through
            int nextPointIndex = (closestPointIndex + 1) % pathPoints.Length;
            return (pathPoints[nextPointIndex].position, pathPoints[nextPointIndex].rotation);
        }

        return (pathPoints[closestPointIndex].position, pathPoints[closestPointIndex].rotation);
    }

    private void UpdateLastPointIndex(float distance, int pointIndex)
    {
        // Hide point that has just been passed through
        if (distance < arrivedThreshold)
        {
            lastPointIndex = pointIndex;
        }
    }
}
