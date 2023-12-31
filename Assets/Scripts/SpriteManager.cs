using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

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

    private int preset; // 0: ����Ű, 1: ��Ű, 2: ��, 3: �׸�, 4: ������
    private int count;
    public async Task Init()
    {
        preset = 0;
        spriteIndexes = new int[11];

        originSpriteAmount = sprites.Count;

        List<string> files = DataManager.GetFiles("/Sprites/");
        for (int i = 0; i < files.Count; i++)
        {
#if UNITY_STANDALONE
        string path = Application.streamingAssetsPath + "/Sprites/" + files[i];
#endif
#if UNITY_ANDROID
            string path = Application.persistentDataPath + "/Sprites/" + files[i];
#endif
            AddSprite(path);
        }
        while (count < files.Count) await Task.Yield();

        UIController.Instance.SetIcons(sprites);
    }

    public async void AddSprite(string path)
    {
        Sprite sprite = await DataManager.LoadSprite(path);
        count++;
        if (sprite == null) return;
        sprites.Add(sprite);
    }
#if UNITY_ANDROID
    public void LoadImage()
    {
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (string.IsNullOrEmpty(path)) return;
            Texture2D texture = NativeGallery.LoadImageAtPath(path, -1, false);
            if (texture == null) return;
            Sprite sprite = DataManager.TextureToSprite(texture, path);
            sprites.Add(sprite);
            DataManager.SaveImage(texture, sprite.name);
            ResetCustomSprites();
        });
    }
    public void ResetCustomSprites()
    {
        List<Sprite> customSprites = sprites.GetRange(originSpriteAmount, sprites.Count - originSpriteAmount);
        customSprites.Sort((a, b) => string.Compare(a.name, b.name));
        UIController.Instance.SetIcons(customSprites, originSpriteAmount);
    }
#endif

    public void ChangePreset(int index)
    {
        preset = index;

        // 0: ��Ʃ�� ����
        // 1~10: ����Ű
        // 11~20: ��Ű
        // 21~30: ��
        // 31~40: �׸�
        // 41~46: ������, ��, ����, �ڼ�, ����, �¿�
        // ��Ÿ������: �� �� �� �� �� �ڼ� �� �� ȣ �� �¿�

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
