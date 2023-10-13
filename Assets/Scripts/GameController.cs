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
        pause = !pause;
        UIController.Instance.OpenSetting(pause);
    }

    public void StartGame()
    {
        UIController.Instance.ChangeScene(1);
        ScoreStorage.Instance.StartGame();
        Spawner.Instance.StartGame();
    }

    public void Title()
    {
        Spawner.Instance.Title();
        ScoreStorage.Instance.EndGame();
        UIController.Instance.ChangeScene(0);
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
}
