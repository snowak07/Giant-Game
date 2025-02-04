using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void EndGameEventHandler();

public class Player : MonoBehaviour
{
    //public event EndGameEventHandler OnEndGame; // Backlog: OnGameEnd Event Freeze system (performance enhancement)
    public MenuManager menu;
    public RayInteractorHandler rayInteractorHandler;

    public float MaxLife = 10;
    public float lifeTotal;
    private string[] damageSourceTags = new string[] { "Projectile" };

    private void ResetHealth()
    {
        lifeTotal = MaxLife;
    }

    private void Start()
    {
        ResetHealth();
    }

    private void Update()
    {
        if (lifeTotal <= 0)
        {
            EndGame();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (damageSourceTags.Contains(collision.transform.root.gameObject.tag))
        {
            lifeTotal -= 1;

            if (lifeTotal <= 0)
            {
                EndGame();
            }
        }
    }

    private void EndGame()
    {
        // Freeze all Enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<Enemy>().Freeze();
        }

        // Freeze all projectiles
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject projectile in projectiles)
        {
            projectile.GetComponent<EnemyProjectile>().Freeze();
        }

        //OnEndGame.Invoke(); // Backlog: OnGameEnd Event Freeze system (performance enhancement)

        // Stop all spawners
        GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");
        foreach (GameObject spawner in spawners)
        {
            spawner.GetComponent<EnemySpawner>().enabled = false;
        }

        // Determine position/rotation to place menu
        Vector3 playerPosition = transform.parent.position;
        Quaternion playerRotation = transform.parent.rotation;

        Quaternion newRotation = Quaternion.Euler(0, playerRotation.eulerAngles.y, 0);
        Vector3 newPosition = playerPosition + (playerRotation * new Vector3(0, 0, 11));

        menu.ShowEndScreen(newPosition, newRotation);

        // Enable Ray Interactor
        rayInteractorHandler.EnableComponents();
    }
}
