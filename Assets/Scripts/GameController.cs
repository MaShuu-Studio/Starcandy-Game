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

    public void StartGame()
    {
        UIController.Instance.ChangeScene(1);
        ScoreStorage.Instance.StartGame();
        Spawner.Instance.StartGame();
    }

    public void GameOver()
    {
        Spawner.Instance.GameOver();
        ScoreStorage.Instance.EndGame();
        UIController.Instance.ChangeScene(0);
    }
}
