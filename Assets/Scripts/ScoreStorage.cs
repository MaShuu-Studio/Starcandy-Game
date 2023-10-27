using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreStorage : MonoBehaviour
{
    public static  ScoreStorage Instance { get { return instance; } }
    private static ScoreStorage instance;

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
    private int[] bestScore;

    public void Init()
    {
        bestScore = DataManager.LoadScore();
        UIController.Instance.SetBestScore(bestScore);

        score = 0;
        addedScores = new int[]
            { 0, 1, 3, 6, 10, 15, 21, 28, 36, 45, 55, 66 };
    }

    public void StartGame()
    {
        score = 0;
        UIController.Instance.SetScore(score);
    }

    public void AddScore(int level)
    {
        score += addedScores[level];
        UIController.Instance.SetScore(score);
    }

    public void EndGame()
    {
        for (int i = 0; i < bestScore.Length; i++)
        {
            if (bestScore[i] < score)
            {
                int tmp = bestScore[i];
                bestScore[i] = score;
                score = tmp;
            }
        }

        DataManager.SaveScore(bestScore);
        UIController.Instance.SetBestScore(bestScore);
    }
}
