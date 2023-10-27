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

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    public bool Pause { get { return pause; } }
    private bool pause = false;

    public bool isLoad = false;

    private void Update()
    {
        if (isLoad == false) return;

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
        UIController.Instance.ChangeScene(2);
        ScoreStorage.Instance.StartGame();
        Spawner.Instance.StartGame();
        yield return null;
        pause = false;
    }

    public void Title()
    {
        Spawner.Instance.Title();
        ScoreStorage.Instance.EndGame();
        UIController.Instance.ChangeScene(1);
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

    public void ExitGame()
    {
        Application.Quit();
    }

}