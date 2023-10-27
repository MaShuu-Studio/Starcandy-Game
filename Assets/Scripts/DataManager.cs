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

    public static List<string> GetFiles(string path)
    {
        string[] files = Directory.GetFiles(Application.streamingAssetsPath + path);
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

        string setting = SoundController.Instance.BgmIndex.ToString() + ",";
        setting += SoundController.Instance.BgmVolume.ToString() + ",";
        setting += SoundController.Instance.SfxVolume.ToString() + ",";
        for (int i = 0; i < SpriteManager.Instance.SpriteIndexes.Length; i++)
        {
            setting += SpriteManager.Instance.SpriteIndexes[i].ToString() + ",";
        }

        File.WriteAllText(settingPath, setting);
    }

    public static void LoadSetting()
    {
        int bgm, bgmV, sfxV;
        int[] arr = new int[11];
        if (File.Exists(settingPath) == false)
        {
            bgm = 0;
            bgmV = 1;
            sfxV = 1;
            arr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
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
        }

        SoundController.Instance.SetBgm(bgm);
        UIController.Instance.SetBgmVolume(bgmV);
        UIController.Instance.SetSfxVolume(sfxV);
        SpriteManager.Instance.SetSpriteIndexes(arr);
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

    public static async Task<Sprite> LoadSprite(string fileName)
    {
        string path = Application.streamingAssetsPath + "/Sprites/" + fileName;
        if (File.Exists(path) == false) return null;

        Sprite sprite = null;
        using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(path))
        {
            req.SendWebRequest();
            try
            {
                while (!req.isDone) await Task.Yield();
                if (string.IsNullOrEmpty(req.error) == false) return null;

                Texture2D texture = DownloadHandlerTexture.GetContent(req);
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

                sprite = Sprite.Create(texture, new Rect(pos.x, pos.y, len, len), Vector2.one / 2, len);
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.Log($"{e}");
#endif
            }
        }

        return sprite;
    }
}
