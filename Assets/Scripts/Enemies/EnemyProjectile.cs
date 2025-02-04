using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    //public Player player = null;
    //public void InitializePlayerListener(Player player) // Backlog: OnGameEnd Event Freeze system (performance enhancement)
    //{
    //    Debug.Log("[EnemyProjectile] InitializePlayerListener");

    //    player.OnEndGame += Freeze;
    //}
    private bool frozen { get; set; }
    private float startTime;
    private readonly float lifespan = 15;

    protected virtual void Start()
    {
        startTime = Time.time;
    }

    protected void Update()
    {
        if (!frozen && Time.time - startTime > lifespan)
        {
            Destroy(gameObject);
        }
    }

    public void Freeze()
    {
        frozen = true;
        Rigidbody body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
    }
}
