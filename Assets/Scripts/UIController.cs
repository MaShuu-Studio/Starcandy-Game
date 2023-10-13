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

    [Header("Setting")]
    [SerializeField] private GameObject setting;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private TextMeshProUGUI bgmValueText;
    [SerializeField] private TextMeshProUGUI bgmText;

    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxValueText;

    public void Init()
    {
        scoreText.text = "0";
        ChangeScene(0);

        OpenSetting(false);
        bgmSlider.value = 10;
        sfxSlider.value = 10;
        AdjustBGM();
        AdjustSFX();
    }

    public void OpenSetting(bool b)
    {
        setting.SetActive(b);
    }

    public void ChangeScene(int index)
    {
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

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void SetBestScore(int[] bestScore)
    {
        for (int i = 0; i < bestScore.Length; i++)
            bestScoreTexts[i].text = bestScore[i].ToString(); ;
    }

    public void SetNext(Sprite sprite, float size)
    {
        nextImage.sprite = sprite;
        nextImageObject.transform.localScale = Vector2.one * size;
    }

    public void SetGrade(Sprite[] sprites)
    {
        for (int i = 0; i < sprites.Length; i++)
            gradeImages[i].sprite = sprites[i];
    }
}
