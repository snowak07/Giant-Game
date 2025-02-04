using System.Collections.Generic;
using UnityEngine;

public class LayeredMonoBehaviour : MonoBehaviour
{
    private static IDictionary<int, IList<GameObject>> layersCache = new Dictionary<int, IList<GameObject>>();

    /**
     * Returns a list of all (active/inactive) GameObjects of the specified layer. Returns null if no GameObject was found.
     */
    public static IList<GameObject> FindGameObjectsWithLayer(int layer)
    {
        return layersCache[layer];
    }

    /**
     * Returns one GameObject with the specified layer. Returns null if no GameObject was found.
     */
    public static GameObject FindWithLayer(int layer)
    {
        var cache = layersCache[layer];
        if (cache != null && cache.Count > 0)
            return cache[0];
        else
            return null;
    }

    protected void Awake()
    {
        // careful: this code assumes that your GameObjects won't change layer during gameplay.
        IList<GameObject> cache;
        if (!layersCache.TryGetValue(gameObject.layer, out cache))
        {
            cache = new List<GameObject>();
            layersCache.Add(gameObject.layer, cache);
        }

        cache.Add(gameObject);
    }

    protected void OnDestroy()
    {
        layersCache[gameObject.layer].Remove(gameObject);
    }
}
