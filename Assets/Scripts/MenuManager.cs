using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Text score;
    public RayInteractorHandler rayInteractorHandler;

    private Canvas menu;

    private void Start()
    {
        menu = GetComponent<Canvas>();
        menu.enabled = false;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //// Stop all spawners
        //GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");
        //foreach (GameObject spawner in spawners)
        //{
        //    spawner.GetComponent<EnemySpawner>().enabled = true;
        //}

        //// Show Menu
        //menu.enabled = false;

        //// Enable Ray Interactor
        //rayInteractorHandler.DisableComponents();

        //// Reset score
        //ScoreManager.Reset();
    }

    public void ShowEndScreen(Vector3 menuPosition, Quaternion menuRotation)
    {
        // Show Menu
        menu.enabled = true;
        menu.transform.rotation = menuRotation;
        menu.transform.root.position = menuPosition;

        // Set score
        score.text = ScoreManager.Get().ToString();
    }
}
