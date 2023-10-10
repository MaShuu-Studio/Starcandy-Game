using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static  SoundController Instance { get { return instance; } }
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

    [SerializeField] private List<string> clipNames;
    [SerializeField] private List<AudioClip> clips;
    private Dictionary<string, AudioClip> sounds;

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private GameObject sfxSource;
    private List<AudioSource> sfxes;
    private int bgmVolume = 1;
    private int sfxVolume = 1;

    private void Start()
    {
        sounds = new Dictionary<string, AudioClip>();
        sfxes = new List<AudioSource>();
        for (int i = 0; i < clips.Count && i < clipNames.Count; i++)
        {
            sounds.Add(clipNames[i], clips[i]);
        }
    }

    private void Update()
    {
        for (int i = 0; i < sfxes.Count; i++)
        {
            if (sfxes[i].isPlaying == false)
            {
                Destroy(sfxes[i]);
                sfxes.RemoveAt(i);
                i--;
            }
        }
    }

    public void SetBgm(string name)
    {
    }

    public void AddSfx(string name)
    {
        if (sounds.ContainsKey(name) == false) return;

        AudioSource source = sfxSource.AddComponent<AudioSource>();
        source.clip = sounds[name];
        source.volume = sfxVolume;
        source.Play();
        sfxes.Add(source);
    }

    public void SetBgmVolume(int volume)
    {
        bgmSource.volume = volume;
    }

    public void SetSfxVolume(int volume)
    {
        sfxVolume = volume;
        foreach(var source in sfxes)
        {
            source.volume = volume;
        }
    }
}


