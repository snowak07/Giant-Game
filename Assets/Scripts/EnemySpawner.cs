using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO Make useable with multiple enemies
// TODO maybe make this a thing that can be destroyed? Could even be another enemy that spits out other enemies.
// TODO could increase spawn rate as time goes on.

public class EnemySpawner : MonoBehaviour
{
    public Transform playerTransform = null;
    public Transform playerBodyTransform = null;

    public float maxHorizontalDistance;
    public float minHorizontalDistance;
    public float maxVerticalDistance;
    public float minVerticalDistance;
    public float spawnCooldownTime = 10.0f;
    public GameObject EnemyPrefab = null;

    private bool spawnCooldown = false;

    // Update is called once per frame
    void Update()
    {
        // Determine if it is time to spawn a new enemy
        if (!spawnCooldown)
        {
            spawnCooldown = true;
            StartCoroutine(SpawnEnemy());
        }
    }

    IEnumerator SpawnEnemy()
    {
        // Choose position
        Vector2 spawnDirectionHorizontal = Random.insideUnitCircle.normalized;
        spawnDirectionHorizontal.y = Mathf.Abs(spawnDirectionHorizontal.y); // Limit spawn area to a half circle
        float distanceFromPlayer = Random.Range(minHorizontalDistance, maxHorizontalDistance);
        float x = distanceFromPlayer * spawnDirectionHorizontal.x;
        float z = distanceFromPlayer * spawnDirectionHorizontal.y;
        float y = Random.Range(minVerticalDistance, maxVerticalDistance);
        Vector3 spawnPosition = new Vector3(x, y, z);

        Vector3 towardsPlayer = playerTransform.position - spawnPosition;
        towardsPlayer.y = y;
        Vector3 upwards = new Vector3(0, 1, 0);
        Quaternion towardsPlayerRotation = Quaternion.LookRotation(towardsPlayer, upwards);

        // Instantiate Enemy prefab
        GameObject enemyObject = Instantiate(EnemyPrefab, spawnPosition, towardsPlayerRotation);
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        enemy.setPlayerTransform(playerTransform);
        enemy.setPlayerBodyTransform(playerBodyTransform);

        // Wait to end spawn cooldown
        yield return new WaitForSeconds(spawnCooldownTime);
        spawnCooldown = false;
        yield break;
    }
}
