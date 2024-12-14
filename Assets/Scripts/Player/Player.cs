using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    //public Canvas menu;
    public MenuManager menu;
    public RayInteractorHandler rayInteractorHandler;

    public float maxLife = 10;

    private float lifeTotal;
    private string[] damageSourceTags = new string[] { "Projectile" };

    private void Start()
    {
        lifeTotal = maxLife;
        rayInteractorHandler.DisableComponents();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (damageSourceTags.Contains(collision.transform.root.gameObject.tag))
        {
            lifeTotal -= 1;

            if (lifeTotal <= 0)
            {
                lifeTotal = maxLife;
                EndGame();
            }
        }
    }

    private void EndGame()
    {
        // Destroy all Enemies // NOTE: This is accomplished in the Menu Manager
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

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
