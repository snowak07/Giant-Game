using UnityEngine;
using System;
using System.Collections.Generic;

public static class Helpers : object
{
    public static bool TryGetComponentInParent<T>(GameObject obj, out T component)
    {
        Transform currentTransform = obj.transform;

        while (true)
        {
            Debug.Log("[Helpers] currentTransform name: " + currentTransform.name);
            if (currentTransform.gameObject.TryGetComponent(out component))
            {
                return true;
            }

            if (currentTransform.parent == null)
            {
                break;
            }

            currentTransform = currentTransform.parent;
        }

        component = default;
        return false;
    }
}
