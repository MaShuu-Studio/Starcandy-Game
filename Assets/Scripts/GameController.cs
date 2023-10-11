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

    private int[] addedScores;
    private int score;

    private void Start()
    {
        score = 0;
        addedScores = new int[]
            { 0, 1, 3, 6, 10, 15, 21, 28, 36, 45, 55, 66 };
    }

    public void AddScore(int level)
    {
        score += addedScores[level];
        UIController.Instance.SetScore(score);
    }
}
