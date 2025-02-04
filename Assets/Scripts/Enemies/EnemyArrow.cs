using UnityEngine;

[RequireComponent(typeof(FlightAlignment))]
public class EnemyArrow : EnemyProjectile
{
    protected override void Start()
    {
        base.Start();

        GetComponent<FlightAlignment>().Enable();
    }

    void OnCollisionEnter(Collision collision)
    {
        GetComponent<FlightAlignment>().Disable();
    }
}
