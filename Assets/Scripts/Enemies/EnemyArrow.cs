using UnityEngine;

[RequireComponent(typeof(FlightAlignment))]
public class EnemyArrow : MonoBehaviour
{
    void Start()
    {
        GetComponent<FlightAlignment>().Enable();
        Destroy(gameObject, 30);
    }

    void OnCollisionEnter(Collision collision)
    {
        GetComponent<FlightAlignment>().Disable();
    }
}
