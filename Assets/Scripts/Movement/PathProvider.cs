using System;
using System.Collections.Generic;
using UnityEngine;

public class PathProvider : MonoBehaviour
{
    public GameObject pathContainer;
    protected List<Transform> pathPoints = null;
    public bool loop = true;

    // Used to keep the PathProvider going to each point instead of getting stuck at one path point
    protected int lastPointIndex = -1;
    protected float arrivedThreshold = 1f;

    // TODO: Handle getting future position calls by not changing lastPointIndex or setting to a temp lastPointIndex value

    protected void Start()
    {
        if (pathContainer != null)
        {
            InitializePathPoints();
        }
    }

    protected void Update()
    {
        if (pathContainer != null && pathPoints == null)
        {
            InitializePathPoints();
        }
    }

    private void InitializePathPoints()
    {
        pathPoints = new List<Transform>();
        foreach (Transform pathPoint in pathContainer.transform) pathPoints.Add(pathPoint);
    }

    public bool hasPath()
    {
        return pathPoints != null && pathPoints.Count > 0;
    }

    public bool arrivedAtFinalPathPoint()
    {
        return !loop && lastPointIndex == pathPoints.Count - 1;
    }

    protected int getClosestPathPointIndex(Vector3 currentPosition)
    {
        int closestPointIndex = 0;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < pathPoints.Count; i++)
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
        if (pathPoints.Count == 0)
        {
            throw new InvalidOperationException("The PathProvider does not have a Path.");
        }
        // lastPointIndex needs to be not changed globally if called from TrajectoryHelper
        int closestPointIndex = getClosestPathPointIndex(currentPosition);
        if (closestPointIndex == lastPointIndex)
        {
            int nextPointIndex;
            if (loop)
            {
                // Return next point if the current closest has already been passed through
                nextPointIndex = (closestPointIndex + 1) % pathPoints.Count;
            }
            else
            {
                nextPointIndex = Math.Min(closestPointIndex + 1, pathPoints.Count - 1);
            }

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
