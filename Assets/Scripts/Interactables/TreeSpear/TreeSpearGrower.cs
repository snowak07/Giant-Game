//using System.Numerics;
using UnityEngine;

public class TreeSpearGrower : InteractableGrower
{
    protected TreeSpearGrower()
    {
        groundOffset = new Vector3(0, 2.5f, 0);
        rootOffset = new Vector3(0, 0, -2.5f);
    }
}
