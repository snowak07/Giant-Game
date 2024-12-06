using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public Transform[] pathPoints = null;

    // Used to keep the PathFollower going to each point instead of getting stuck at one path point
    protected int lastPointIndex = -1;
    protected float arrivedThreshold = 0.25f;

    protected int getClosestPathPointIndex()
    {
        int closestPointIndex = 0;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < pathPoints.Length; i++)
        {
            Transform point = pathPoints[i];
            float distance = (point.position - transform.position).magnitude;
            if (distance < closestDistance)
            {
                closestPointIndex = i;
                closestDistance = distance;
            }
        }

        UpdateLastPointIndex(closestDistance, closestPointIndex);

        return closestPointIndex;
    }

    public (Vector3, Quaternion) getNextPathPoint()
    {
        int closestPointIndex = getClosestPathPointIndex();
        if (closestPointIndex == lastPointIndex)
        {
            // Return next point if the current closest has already been passed through
            return (pathPoints[closestPointIndex + 1].position, pathPoints[closestPointIndex + 1].rotation);
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
