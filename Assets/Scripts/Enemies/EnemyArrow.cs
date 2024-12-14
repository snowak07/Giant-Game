using UnityEngine;

[RequireComponent(typeof(FlightAlignment))]
public class EnemyArrow : MonoBehaviour
{
    void Start()
    {
        GetComponent<FlightAlignment>().Enable();
    }

    void OnCollisionEnter(Collision collision)
    {
        GetComponent<FlightAlignment>().Disable();
    }
}
