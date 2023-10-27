using System.Threading.Tasks;
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

    [SerializeField] private List<Sprite> sprites;
    private int originSpriteAmount;
    public int[] SpriteIndexes { get { return spriteIndexes; } }
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

    private int preset; // 0: ³ªÃ÷Å°, 1: ÃÝÅ°, 2: ¹Ý, 3: Å×¸®, 4: º°»çÅÁ
    public async Task Init()
    {
        preset = 0;
        spriteIndexes = new int[11];

        originSpriteAmount = sprites.Count;
        
        List<string> files = DataManager.GetFiles("/Sprites/");
        for (int i = 0; i < files.Count; i++)
        {
            Sprite sprite = await DataManager.LoadSprite(files[i]);
            if (sprite == null) continue;
            sprites.Add(sprite);
        }
        
        UIController.Instance.SetIcons(sprites);
    }

    public void ChangePreset(int index)
    {
        preset = index;

        // 0: À¯Æ©ºê ÅëÀÏ
        // 1~10: ³ªÃ÷Å°
        // 11~20: ÃÝÅ°
        // 21~30: ¹Ý
        // 31~40: Å×¸®
        // 41~46: º°»çÅÁ, ¹®, ¶¼±¼, ¹Ú¼ö, Á¡ÇÁ, ÁÂ¿ì
        // ½ºÅ¸µ¥ÀÌÁî: ¹® ³ª ÃÝ ¹Ý Å× ¹Ú¼ö ¿À ³ì È£ ¿Ã ÁÂ¿ì

        if (preset < 4)
        {
            spriteIndexes[0] = 0;
            for (int i = 1; i < 11; i++)
            {
                spriteIndexes[i] = i + 10 * preset;
            }
        }
        else
        {
            spriteIndexes[0] = 42;
            spriteIndexes[1] = 9;
            spriteIndexes[2] = 19;
            spriteIndexes[3] = 29;
            spriteIndexes[4] = 39;
            spriteIndexes[5] = 44;
            spriteIndexes[6] = 5;
            spriteIndexes[7] = 13;
            spriteIndexes[8] = 24;
            spriteIndexes[9] = 35;
            spriteIndexes[10] = 46;
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

    public void SetSpriteIndexes(int[] indexes)
    {
        for (int i = 0; i < spriteIndexes.Length; i++)
        {
            int index = indexes[i];
            if (index >= sprites.Count) index = 0;
            spriteIndexes[i] = index;
        }
        UIController.Instance.SetGrade();
    }
}
