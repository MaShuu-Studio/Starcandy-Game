using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get { return instance; } }
    private static GameController instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private bool isPlaying = false;
    private void Update()
    {
        if (isPlaying && Input.GetKeyDown(KeyCode.Escape))
        {
            GameOver();
        }
    }

    public void StartGame()
    {
        isPlaying = true;
        UIController.Instance.ChangeScene(1);
        ScoreStorage.Instance.StartGame();
        Spawner.Instance.StartGame();
    }

    public void GameOver()
    {
        isPlaying = false;
        Spawner.Instance.GameOver();
        ScoreStorage.Instance.EndGame();
        UIController.Instance.ChangeScene(0);
    }
}
