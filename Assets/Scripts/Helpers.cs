using UnityEngine;
using System;
using System.Collections.Generic;

public static class Helpers : object
{
    public static bool TryGetComponentInParent<T>(GameObject obj, out T component)
    {
        Transform currentTransform = obj.transform;
        if (obj.TryGetComponent(out component))
        {
            return true;
        }

        while (currentTransform.parent != null)
        {
            if (currentTransform.gameObject.TryGetComponent(out component))
            {
                return true;
            }

            currentTransform = currentTransform.parent;
        }

        component = default;
        return false;
    }
}
