using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    [SerializeField] private Sprite[] nSprites;
    [SerializeField] private Sprite[] cSprites;
    [SerializeField] private Sprite[] vSprites;
    [SerializeField] private Sprite[] tSprites;
    [SerializeField] private Sprite[] sSprites;
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

    public Sprite[] GetSprites
    {
        get
        {
            switch (preset)
            {
                case 0: return nSprites;
                case 1: return cSprites;
                case 2: return vSprites;
                case 3: return tSprites;
            }
            return sSprites;
        }
    }

    private int preset; // 0: ³ªÃ÷Å°, 1: ÃÝÅ°, 2: ¹Ý, 3: Å×¸®
    public void Init()
    {
        preset = 0;
    }

    public void ChangePreset(int index)
    {
        preset = index;
        if (preset < 0) preset = 0;
        if (preset > 3) preset = 3;
    }
}
