using UnityEngine;

public class ScrapeArrows : MonoBehaviour
{
    private float distanceThreshold = 0.5f;

    void OnCollisionEnter(Collision collision)
    {
        EnemyArrow[] arrows = GetComponentsInChildren<EnemyArrow>();
        Vector3 contactPoint = collision.GetContact(0).point;
        if (collision.gameObject.tag == "Giant")
        {
            foreach (EnemyArrow arrow in arrows)
            {
                Vector3 arrowPosition = arrow.transform.position;
                float arrowDistance = (arrowPosition - contactPoint).magnitude;
                if (arrowDistance < distanceThreshold)
                {
                    // Add rigidbody and explosion force
                    Rigidbody arrowBody = arrow.gameObject.AddComponent<Rigidbody>();
                    arrow.transform.parent = null;
                }
            }
        }
    }
}
