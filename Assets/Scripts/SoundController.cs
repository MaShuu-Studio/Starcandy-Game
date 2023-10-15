using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance { get { return instance; } }
    private static SoundController instance;

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
    [SerializeField] private string[] bgmNames;
    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] private List<string> sfxNames;
    [SerializeField] private List<AudioClip> sfxClips;
    [SerializeField] private AudioSource bgmSource;

    public Dictionary<string, AudioClip> Sfxes { get { return sfxes; } }
    private Dictionary<string, AudioClip> sfxes;

    [Space]
    [SerializeField] private CustomAudioSource sfxPrefab;
    private Dictionary<string, SoundPool> sfxPools;

    public int BgmIndex { get { return bgmIndex; } }
    private int bgmIndex;

    public int BgmVolume { get { return bgmVolume; } }
    private int bgmVolume = 1;

    public int SfxVolume { get { return sfxVolume; } }
    private int sfxVolume = 1;

    public void Init()
    {
        bgmSource.loop = true;

        sfxes = new Dictionary<string, AudioClip>();
        sfxPools = new Dictionary<string, SoundPool>();
        for (int i = 0; i < sfxNames.Count && i < sfxClips.Count; i++)
        {
            sfxes.Add(sfxNames[i], sfxClips[i]);

            CustomAudioSource source = Instantiate(sfxPrefab);
            source.MakePrefab(sfxNames[i]);

            GameObject go = new GameObject(sfxNames[i]);
            go.transform.SetParent(transform);
            SoundPool pool = go.AddComponent<SoundPool>();
            pool.Init(source);

            sfxPools.Add(sfxNames[i], pool);
        }
    }

    public void ChangeBgm(int i)
    {
        bgmIndex += i;

        if (bgmIndex < 0) bgmIndex = bgmClips.Length - 1;
        else if (bgmIndex >= bgmClips.Length) bgmIndex = 0;

        bgmSource.clip = bgmClips[bgmIndex];
        bgmSource.Play();
        UIController.Instance.SetBGM(bgmNames[bgmIndex]);
    }

    public void SetBgm(int index)
    {
        bgmIndex = index;
        ChangeBgm(0);
    }

    public void AddSfx(string name)
    {
        if (sfxPools.ContainsKey(name) == false) return;
        sfxPools[name].Pop();
    }

    public void SetBgmVolume(int volume)
    {
        bgmVolume = volume;
        bgmSource.volume = volume / 10f;
    }

    public void SetSfxVolume(int volume)
    {
        sfxVolume = volume;
    }

    public void StopAudio(string name, CustomAudioSource source)
    {
        if (sfxPools.ContainsKey(name) == false) return;
        sfxPools[name].Push(source);
    }
}