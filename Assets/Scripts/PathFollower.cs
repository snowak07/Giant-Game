using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public Transform[] pathPoints = null;
    protected int currentPathIndex = -1;

    public (Vector3, Quaternion) getClosestPathPoint()
    {
        int closestPointIndex = -1;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < pathPoints.Length; i++)
        {
            Transform point = pathPoints[i];
            float distance = (point.position - transform.position).magnitude;
            if (closestPointIndex == -1 || distance < closestDistance)
            {
                closestPointIndex = i;
                closestDistance = distance;
            }
        }

        currentPathIndex = closestPointIndex;

        return (pathPoints[closestPointIndex].position, pathPoints[closestPointIndex].rotation);
    }
}
