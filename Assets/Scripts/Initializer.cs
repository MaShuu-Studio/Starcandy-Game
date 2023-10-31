using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class Initializer : MonoBehaviour
{
    public static Initializer Instance { get { return instance; } }
    private static Initializer instance;

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
    }

    private void Start()
    {
        Init();
    }

    private async void Init()
    {
        UIController.Instance.ChangeScene(0);
        DataManager.Init();
        await SpriteManager.Instance.Init();
        ScoreStorage.Instance.Init();
        SoundController.Instance.Init();
        Spawner.Instance.Init();
        UIController.Instance.Init();

        DataManager.LoadSetting();
        GameController.Instance.isLoad = true;
        UIController.Instance.ChangeScene(1);
    }
}
