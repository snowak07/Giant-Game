using UnityEngine;

public class StickOnHit : MonoBehaviour
{
    protected bool stickCooldown = false;

    // TODO: (optional) Add Coroutine that causes the arrows to fall off after a certain amount of time.

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Giant" && !stickCooldown)
        {
            stickCooldown = true;
            transform.parent = collision.transform;
            Destroy(GetComponent<Rigidbody>());
        }
    }
}
