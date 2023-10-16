using System;
using System.IO;
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

        Application.targetFrameRate = 60;
        scorePath = Path.Combine(Application.persistentDataPath, "Score.bin");
        settingPath = Path.Combine(Application.persistentDataPath, "setting.ini");
    }

    public bool Pause { get { return pause; } }
    private bool pause = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        StartCoroutine(Pausing());
    }

    private IEnumerator Pausing()
    {
        bool state = !pause;
        // 다음에 pause가 걸린다면 먼저 바꾼 뒤 프레임 대기
        if (state)
        {
            pause = state;
            yield return null;
        }
        // 다음에 pause가 풀린다면 프레임 대기 후 스탑.
        else
        {
            yield return null;
            pause = state;
        }

        UIController.Instance.OpenSetting(pause);
    }

    public void StartGame()
    {
        StartCoroutine(GameStart());
    }

    private IEnumerator GameStart()
    {
        pause = true;
        UIController.Instance.ChangeScene(1);
        ScoreStorage.Instance.StartGame();
        Spawner.Instance.StartGame();
        yield return null;
        pause = false;
    }

    public void Title()
    {
        Spawner.Instance.Title();
        ScoreStorage.Instance.EndGame();
        UIController.Instance.ChangeScene(0);
    }

    public void GiveUp()
    {
        StartCoroutine(GiveUpGame());
    }

    private IEnumerator GiveUpGame()
    {
        yield return null;
        pause = !pause;
        UIController.Instance.OpenSetting(pause);

        Spawner.Instance.StopGame();
        ScoreStorage.Instance.EndGame();
        yield return new WaitForEndOfFrame();

        Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2);

        UIController.Instance.EndGame(sprite);
        yield return null;
    }

    public void GameOver()
    {
        StartCoroutine(EndGame());
    }

    private IEnumerator EndGame()
    {
        Spawner.Instance.StopGame();
        ScoreStorage.Instance.EndGame();
        yield return new WaitForEndOfFrame();

        Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2);

        UIController.Instance.EndGame(sprite);
        yield return null;
    }


    private string scorePath;
    private string settingPath;

    public void SaveSetting()
    {
        // 가벼운 프로젝트인 만큼 간단하게 처리
        // 0: bgmIndex
        // 1: bgmVolume
        // 2: sfxVolume
        // 3 ~ 13: iconIndex

        string setting = SoundController.Instance.BgmIndex.ToString() + ",";
        setting += SoundController.Instance.BgmVolume.ToString() + ",";
        setting += SoundController.Instance.SfxVolume.ToString() + ",";
        for (int i = 0; i < SpriteManager.Instance.SpriteIndexes.Length; i++)
        {
            setting += SpriteManager.Instance.SpriteIndexes[i].ToString() + ",";
        }

        File.WriteAllText(settingPath, setting);
    }

    public void LoadSetting()
    {
        int bgm, bgmV, sfxV;
        int[] arr = new int[11];
        if (File.Exists(settingPath) == false)
        {
            bgm = 0;
            bgmV = 1;
            sfxV = 1;
            arr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        }
        else
        {
            string setting = File.ReadAllText(settingPath);
            string[] settings = setting.Split(",");

            int.TryParse(settings[0], out bgm);
            int.TryParse(settings[1], out bgmV);
            int.TryParse(settings[2], out sfxV);
            for (int i = 0; i < arr.Length && i < setting.Length - 3; i++)
            {
                int.TryParse(settings[i + 3], out arr[i]);
            }
        }

        SoundController.Instance.SetBgm(bgm);
        UIController.Instance.SetBgmVolume(bgmV);
        UIController.Instance.SetSfxVolume(sfxV);
        SpriteManager.Instance.SetSpriteIndexes(arr);
    }

    public void SaveScore(int[] bestScore)
    {
        byte[] data = new byte[bestScore.Length * sizeof(int)];
        for (int i = 0; i < bestScore.Length; i++)
        {
            byte[] tmp = BitConverter.GetBytes(bestScore[i]);
            Buffer.BlockCopy(tmp, 0, data, i * sizeof(int), tmp.Length);
        }

        File.WriteAllBytes(scorePath, data);
    }

    public int[] LoadScore()
    {
        if (File.Exists(scorePath) == false) return new int[3];
        int[] bestScore = new int[3];
        byte[] data = File.ReadAllBytes(scorePath);

        for (int i = 0; i < data.Length / sizeof(int); i++)
        {
            byte[] tmp = new byte[sizeof(int)];
            Buffer.BlockCopy(data, i * sizeof(int), tmp, 0, sizeof(int));
            bestScore[i] = BitConverter.ToInt32(tmp);
        }

        return bestScore;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}