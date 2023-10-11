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

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image nextImage;

    private void Start()
    {
        scoreText.text = "0";
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void SetNext(Sprite sprite, float size)
    {
        nextImage.sprite = sprite;
        nextImage.transform.localScale = Vector2.one * size;
    }
}
