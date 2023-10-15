using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public static SpriteManager Instance { get { return instance; } }
    private static SpriteManager instance;

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

    [SerializeField] private Sprite[] sprites;
    private int[] spriteIndexes;
    public Sprite[] GetSprites
    {
        get
        {
            Sprite[] arr = new Sprite[spriteIndexes.Length];
            for (int i = 0; i < spriteIndexes.Length; i++)
            {
                arr[i] = sprites[spriteIndexes[i]];
            }
            return arr;
        }
    }

    private int preset; // 0: ³ªÃ÷Å°, 1: ÃÝÅ°, 2: ¹Ý, 3: Å×¸®
    public void Init()
    {
        preset = 0;
        spriteIndexes = new int[11];
        ChangePreset(0);

        UIController.Instance.SetIcons(sprites);
    }

    public void ChangePreset(int index)
    {
        preset = index;
        if (preset < 0) preset = 0;
        if (preset > 3) preset = 3;

        // 0: À¯Æ©ºê ÅëÀÏ
        // 1~10: ³ªÃ÷Å°
        // 11~20: ÃÝÅ°
        // 21~30: ¹Ý
        // 31~40: Å×¸®

        spriteIndexes[0] = 0;
        for (int i = 1; i < 11; i++)
        {
            spriteIndexes[i] = i + 10 * preset;
        }

        UIController.Instance.SetGrade();
    }

    private int changingIndex;
    private int[] changingIndexes;
    private bool changing;
    public void StartChangeIndex()
    {
        changingIndex = 0;
        changingIndexes = new int[11];
        changing = true;
        UIController.Instance.StartChangeGrade();
    }

    public void StopChangeIndex()
    {
        changingIndex = 0;
        changing = false;
    }

    public void SetSpriteIndex(int index)
    {
        if (changing == false) return;

        changingIndexes[changingIndex] = index;
        UIController.Instance.ChangeGrade(changingIndex, sprites[index]);
        changingIndex++;
        if (changingIndex == 11)
        {
            for (int i = 0; i < spriteIndexes.Length; i++)
                spriteIndexes[i] = changingIndexes[i];
            changing = false;
            UIController.Instance.SetGrade();
        }
    }
}
