using UnityEngine;

public class BoulderGrower : InteractableGrower
{
    protected BoulderGrower()
    {
        groundOffset = new Vector3(0, 0.6f, 0);
        rootOffset = new Vector3(0, 0.6f, 0);
    }
}
