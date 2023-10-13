using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreStorage : MonoBehaviour
{
    private static string path;

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
        
        path = Path.Combine(Application.persistentDataPath, "Score.bin");
    }

    private int[] addedScores;
    private int score;
    private int[] bestScore;

    public void Init()
    {
        bestScore = new int[3];
        LoadScore();
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

        SaveScore();
        UIController.Instance.SetBestScore(bestScore);
    }

    public void LoadScore()
    {
        if (File.Exists(path) == false) return;

        byte[] data = File.ReadAllBytes(path);

        for (int i = 0; i < data.Length / sizeof(int); i++)
        {
            byte[] tmp = new byte[sizeof(int)];
            Buffer.BlockCopy(data, i * sizeof(int), tmp, 0, sizeof(int));
            bestScore[i] = BitConverter.ToInt32(tmp);
        }
    }

    public void SaveScore()
    {
        byte[] data = new byte[bestScore.Length * sizeof(int)];
        for (int i = 0; i < bestScore.Length; i++)
        {
            byte[] tmp = BitConverter.GetBytes(bestScore[i]);
            Buffer.BlockCopy(tmp, 0, data, i * sizeof(int), tmp.Length);
        }

        File.WriteAllBytes(path, data);
    }
}
