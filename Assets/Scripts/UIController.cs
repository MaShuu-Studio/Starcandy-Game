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

    // 0: Loading, 1: Title, 2: Game
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
    [SerializeField] private GameObject galleryIcon;
    private List<Icon> icons = new List<Icon>();

    [Header("Game Over")]
    [SerializeField] private GameObject gameEndObject;
    [SerializeField] private Image gameEndScreenShot;
    [SerializeField] private TextMeshProUGUI gameEndScoreText;
    [SerializeField] private TextMeshProUGUI[] gameEndBestScoreTexts;

    [Header("Setting")]
    [SerializeField] private GameObject settingButton;
    [SerializeField] private RectTransform setting;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private TextMeshProUGUI bgmValueText;

    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxValueText;

    [SerializeField] private GameObject graphicOptionObject;
    [SerializeField] private TMP_Dropdown graphicDrop;
    [SerializeField] private Toggle[] graphicToggles;
    private int screenType;

    [SerializeField] private GameObject giveupButton;

    [Header("Playlist")]
    [SerializeField] private TextMeshProUGUI curBGMText;
    [SerializeField] private RectTransform plContent;
    [SerializeField] private PlayListItem plItemPrefab;
    [SerializeField] private Image playButton;
    [SerializeField] private Image cycleButton;
    [SerializeField] private Sprite[] playSprites;
    [SerializeField] private Sprite[] cycleSprites;
    [SerializeField] private Slider progress;
    private bool selectProgressHandle;
    private List<PlayListItem> plItems;

    public void Init()
    {
        scoreText.text = "0";
        settingButton.SetActive(true);
#if UNITY_STANDALONE
        graphicOptionObject.SetActive(true);
        setting.sizeDelta = new Vector2(650, 550);
        galleryIcon.SetActive(false);
#endif
#if UNITY_ANDROID
        graphicOptionObject.SetActive(false);
        setting.sizeDelta = new Vector2(650, 350);
        galleryIcon.SetActive(true);
#endif

        OpenSetting(false);

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

        plItems = new List<PlayListItem>();
        // 플레이리스트
        for (int i = 0; i < SoundController.Instance.BgmClips.Length; i++)
        {
            PlayListItem item = Instantiate(plItemPrefab, plContent);
            item.SetIcon(i, SoundController.Instance.BgmClips[i]);
            plItems.Add(item);
        }
        plItemPrefab.gameObject.SetActive(false);
        plContent.sizeDelta = new Vector2(470, SoundController.Instance.BgmClips.Length * 75);
    }

    public void SetIcons(List<Sprite> sprites, int targetCount = -1)
    {
        if (targetCount != -1)
        {
            while (icons.Count > targetCount)
            {
                Destroy(icons[icons.Count - 1].gameObject);
                icons.RemoveAt(icons.Count - 1);
            }
        }

        for (int i = 0; i < sprites.Count; i++)
        {
            Icon icon = Instantiate(iconPrefab, iconsParent);
            icon.SetIcon(sprites[i], i);
            icons.Add(icon);
        }

        iconsParent.sizeDelta = new Vector2(475, 125 * ((icons.Count - 1) / 4 + 1));
    }

#if UNITY_ANDROID
    public void LoadImage()
    {
        SpriteManager.Instance.LoadImage();
    }
#endif

    public void OpenSetting(bool b)
    {
        setting.gameObject.SetActive(b);
        selectProgressHandle = false;
    }

    public void ChangeScene(int index)
    {
        gameEndObject.SetActive(false);
        giveupButton.SetActive(index == 2);
        for (int i = 0; i < scenes.Length; i++)
            scenes[i].SetActive(i == index);
    }

    public void SetBgmVolume(int volume)
    {
        bgmSlider.value = volume;
        bgmValueText.text = volume.ToString();
        SoundController.Instance.SetBgmVolume(volume);
    }

    public void SetSfxVolume(int volume)
    {
        sfxSlider.value = volume;
        sfxValueText.text = volume.ToString();
        SoundController.Instance.SetSfxVolume(volume);
    }

    public void AddPlaylist(int index, bool b)
    {
        if (plItems == null || index >= plItems.Count) return;
        plItems[index].AddPlaylist(b);
        DataManager.SaveSetting();
    }

    public void AdjustBGM()
    {
        SoundController.Instance.SetBgmVolume((int)bgmSlider.value);
        bgmValueText.text = ((int)bgmSlider.value).ToString();
    }

    public void AdjustSFX()
    {
        SoundController.Instance.SetSfxVolume((int)sfxSlider.value);
        sfxValueText.text = ((int)sfxSlider.value).ToString();
    }

    public void PlayPause()
    {
        if (SoundController.Instance.isPlay) playButton.sprite = playSprites[0];
        else playButton.sprite = playSprites[1];
    }

    public void ChangePlayType()
    {
        SoundController.Instance.ChangePlayType();
        cycleButton.sprite = cycleSprites[(int)SoundController.Instance.PType];
    }

    public void ChangeBgm(int i)
    {
        if (SoundController.Instance.PType == SoundController.PlayType.RANDOM)
            SoundController.Instance.ShuffleBGM();
        else SoundController.Instance.ChangeBgm(i);
    }

    public void SetBgm(string name)
    {
        curBGMText.text = name;
    }

    public void ChangeProgress(float p)
    {
        if (selectProgressHandle == false) progress.value = p;
    }

    public void SelectProgressHandle(bool b)
    {
        selectProgressHandle = b;
        // 위치를 조정했을 때
        if (selectProgressHandle == false)
        {
            SoundController.Instance.ChangeBGMProgress(progress.value);
        }
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
        DataManager.SaveSetting();
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
