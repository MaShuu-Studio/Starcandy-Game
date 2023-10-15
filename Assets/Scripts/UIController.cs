using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get { return instance; } }
    private static UIController instance;

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

    [SerializeField] private GameObject[] scenes;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject nextImageObject;
    [SerializeField] private Image nextImage;
    [SerializeField] private Image[] gradeImages;
    [SerializeField] private TextMeshProUGUI[] bestScoreTexts;

    [Header("Set Icons")]
    [SerializeField] private Icon iconPrefab;
    [SerializeField] private RectTransform iconsParent;
    [SerializeField] private Image[] iconList;

    [Header("Game Over")]
    [SerializeField] private GameObject gameEndObject;
    [SerializeField] private Image gameEndScreenShot;
    [SerializeField] private TextMeshProUGUI gameEndScoreText;
    [SerializeField] private TextMeshProUGUI[] gameEndBestScoreTexts;

    [Header("Setting")]
    [SerializeField] private GameObject setting;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private TextMeshProUGUI bgmValueText;
    [SerializeField] private TextMeshProUGUI bgmText;

    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxValueText;

    [SerializeField] private TMP_Dropdown graphicDrop;
    [SerializeField] private Toggle[] graphicToggles;
    private int screenType;

    [SerializeField] private GameObject giveupButton;

    public void Init()
    {
        scoreText.text = "0";
        ChangeScene(0);

        OpenSetting(false);
        bgmSlider.value = 1;
        sfxSlider.value = 1;
        AdjustBGM();
        AdjustSFX();

        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.ExclusiveFullScreen: screenType = 0; break;
            case FullScreenMode.FullScreenWindow: screenType = 1; break;
            case FullScreenMode.Windowed: screenType = 2; break;
        }
        for (int i = 0; i < graphicToggles.Length; i++)
            graphicToggles[i].isOn = i == screenType;

        if (Screen.width == 1280) graphicDrop.value = 0;
        else graphicDrop.value = 1;

        iconPrefab.gameObject.SetActive(false);
    }

    public void SetIcons(Sprite[] sprites)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            Icon icon = Instantiate(iconPrefab, iconsParent);
            icon.SetIcon(sprites[i], i);
        }

        iconsParent.sizeDelta = new Vector2(475, 125 * (sprites.Length / 4 + 1));

    }

    public void OpenSetting(bool b)
    {
        setting.SetActive(b);
    }

    public void ChangeScene(int index)
    {
        gameEndObject.SetActive(false);
        giveupButton.SetActive(index == 1);
        for (int i = 0; i < scenes.Length; i++)
            scenes[i].SetActive(i == index);
    }

    public void AdjustBGM()
    {
        SoundController.Instance.SetBgmVolume(bgmSlider.value);
        bgmValueText.text = ((int)bgmSlider.value).ToString();
    }

    public void AdjustSFX()
    {
        SoundController.Instance.SetSfxVolume(sfxSlider.value);
        sfxValueText.text = ((int)sfxSlider.value).ToString();
    }

    public void SetBGM(string name)
    {
        bgmText.text = name;
    }

    public void ChangeBgm(int i)
    {
        SoundController.Instance.ChangeBgm(i);
    }

    public void ChangeRes()
    {
        if (graphicDrop.value == 0)
        {
            Screen.SetResolution(1280, 720, Screen.fullScreenMode);
        }
        else if (graphicDrop.value == 1)
        {
            Screen.SetResolution(1920, 1080, Screen.fullScreenMode);
        }
    }

    public void ChangeScreenType(int index)
    {
        if (graphicToggles[index].isOn == false) return;

        screenType = index;
        //graphicDrop.gameObject.SetActive(screenType != 1);
        FullScreenMode mode = FullScreenMode.ExclusiveFullScreen;
        if (screenType == 1) mode = FullScreenMode.FullScreenWindow;
        if (screenType == 2) mode = FullScreenMode.Windowed;
        Screen.SetResolution(Screen.width, Screen.height, mode);
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
        gameEndScoreText.text = score.ToString();
    }

    public void SetBestScore(int[] bestScore)
    {
        for (int i = 0; i < bestScore.Length; i++)
        {
            bestScoreTexts[i].text = bestScore[i].ToString();
            gameEndBestScoreTexts[i].text = bestScore[i].ToString();
        }
    }

    public void SetNext(Sprite sprite, float size)
    {
        nextImage.sprite = sprite;
        nextImageObject.transform.localScale = Vector2.one * size;
    }

    public void SetGrade()
    {
        Sprite[] sprites = SpriteManager.Instance.GetSprites;
        for (int i = 0; i < sprites.Length; i++)
        {
            gradeImages[i].sprite = sprites[i];
            iconList[i].sprite = sprites[i];
        }
    }

    public void StartChangeGrade()
    {
        for (int i = 0; i < iconList.Length; i++)
        {
            iconList[i].sprite = null;
        }
    }

    public void ChangeGrade(int index, Sprite sprite)
    {
        iconList[index].sprite = sprite;
    }

    public void EndGame(Sprite sprite)
    {
        gameEndObject.SetActive(true);
        gameEndScreenShot.sprite = sprite;
    }
}
