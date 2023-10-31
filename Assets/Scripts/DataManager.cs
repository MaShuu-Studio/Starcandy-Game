using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class DataManager
{
    private static string scorePath = Path.Combine(Application.persistentDataPath, "Score.bin");
    private static string settingPath = Path.Combine(Application.persistentDataPath, "setting.ini");


    public static void Init()
    {
#if UNITY_ANDROID
        if (Directory.Exists(Application.persistentDataPath + "/Sprites") == false)
            Directory.CreateDirectory(Application.persistentDataPath + "/Sprites");
#endif
    }

    public static List<string> GetFiles(string path)
    {
#if UNITY_STANDALONE
        string[] files = Directory.GetFiles(Application.streamingAssetsPath + path);
#endif
#if UNITY_ANDROID
        string[] files = Directory.GetFiles(Application.persistentDataPath + path);
#endif
        List<string> names = new List<string>();
        for (int i = 0; i < files.Length; i++)
        {
            //if (files[i].Contains(".meta")) continue;
            names.Add(Path.GetFileName(files[i]));
        }

        return names;
    }

    public static void SaveSetting()
    {
        // 가벼운 프로젝트인 만큼 간단하게 처리
        // 0: bgmIndex
        // 1: bgmVolume
        // 2: sfxVolume
        // 3 ~ 13: iconIndex
        // 14: bgmPlayType

        string setting = SoundController.Instance.BgmIndex.ToString() + ",";
        setting += SoundController.Instance.BgmVolume.ToString() + ",";
        setting += SoundController.Instance.SfxVolume.ToString() + ",";
        for (int i = 0; i < SpriteManager.Instance.SpriteIndexes.Length; i++)
        {
            setting += SpriteManager.Instance.SpriteIndexes[i].ToString() + ",";
        }
        setting += (int)SoundController.Instance.PType + ",";

        File.WriteAllText(settingPath, setting);
    }

    public static void LoadSetting()
    {
        int bgm, bgmV, sfxV;
        int[] arr = new int[11];
        int ptype;
        if (File.Exists(settingPath) == false)
        {
            bgm = 0;
            bgmV = 1;
            sfxV = 1;
            arr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            ptype = 0;
        }
        else
        {
            string setting = File.ReadAllText(settingPath);
            string[] settings = setting.Split(",");

            int.TryParse(settings[0], out bgm);
            int.TryParse(settings[1], out bgmV);
            int.TryParse(settings[2], out sfxV);
            for (int i = 0; i < arr.Length && i < setting.Length - 3; i++)
            {
                int.TryParse(settings[i + 3], out arr[i]);
            }
            int.TryParse(settings[14], out ptype);
        }

        SoundController.Instance.SetBgm(bgm);
        UIController.Instance.SetBgmVolume(bgmV);
        UIController.Instance.SetSfxVolume(sfxV);
        SpriteManager.Instance.SetSpriteIndexes(arr);
        SoundController.Instance.SetPlayType(ptype);
    }

    public static void SaveScore(int[] bestScore)
    {
        byte[] data = new byte[bestScore.Length * sizeof(int)];
        for (int i = 0; i < bestScore.Length; i++)
        {
            byte[] tmp = BitConverter.GetBytes(bestScore[i]);
            Buffer.BlockCopy(tmp, 0, data, i * sizeof(int), tmp.Length);
        }

        File.WriteAllBytes(scorePath, data);
    }

    public static int[] LoadScore()
    {
        if (File.Exists(scorePath) == false) return new int[3];
        int[] bestScore = new int[3];
        byte[] data = File.ReadAllBytes(scorePath);

        for (int i = 0; i < data.Length / sizeof(int); i++)
        {
            byte[] tmp = new byte[sizeof(int)];
            Buffer.BlockCopy(data, i * sizeof(int), tmp, 0, sizeof(int));
            bestScore[i] = BitConverter.ToInt32(tmp);
        }

        return bestScore;
    }

    public static async Task<Sprite> LoadSprite(string path)
    {
        if (File.Exists(path) == false) return null;

        Sprite sprite = null;

#if UNITY_ANDROID
        Texture2D texture = await NativeGallery.LoadImageAtPathAsync(path, -1, false);
        if (texture == null) return null;
        sprite = TextureToSprite(texture, path);
#endif
#if UNITY_STANDALONE
        using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(path))
        {
            req.SendWebRequest();
            try
            {
                while (!req.isDone) await Task.Yield();
                if (string.IsNullOrEmpty(req.error) == false) return null;
                Texture2D texture = DownloadHandlerTexture.GetContent(req);

                sprite = TextureToSprite(texture, path);
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.Log($"{e}");
#endif
            }
        }
#endif

        return sprite;
    }

    public static Sprite TextureToSprite(Texture2D texture, string path)
    {
        int len = (texture.width > texture.height) ? texture.height : texture.width;

        Vector2Int pos = new Vector2Int(texture.width - len, texture.height - len) / 2;
        Vector2Int center = new Vector2Int(texture.width / 2, texture.height / 2);
        Color invisible = new Color(0, 0, 0, 0);

        for (int y = 0; y < texture.height; y++)
            for (int x = 0; x < texture.width; x++)
            {
                int px = x - center.x;
                int py = y - center.y;
                // 원의 방정식 밖에 있다면 가리기.
                if (px * px + py * py >= len * len / 4)
                    texture.SetPixel(x, y, invisible);
            }
        texture.Apply();

        Sprite sprite = Sprite.Create(texture, new Rect(pos.x, pos.y, len, len), Vector2.one / 2, len);
        sprite.name = Path.GetFileName(path);
        return sprite;
    }


#if UNITY_ANDROID
    public static void SaveImage(Texture2D texture, string name)
    {
        string path = Application.persistentDataPath + "/Sprites/" + name;
        if (File.Exists(path)) return;

        byte[] bytes;
        if (name.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
        {
            bytes = texture.EncodeToPNG();
        }
        else if (name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
           || name.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
        {
            bytes = texture.EncodeToJPG();
        }
        else
        {
            bytes = texture.EncodeToPNG();
            name = name.Split(".")[0] + ".png";
            path = Application.persistentDataPath + "/Sprites/" + name;
        }

        File.WriteAllBytes(path, bytes);
    }
#endif
}
