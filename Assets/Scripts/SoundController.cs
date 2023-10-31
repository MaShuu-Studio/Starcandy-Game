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
    [SerializeField] private BGMClip[] bgmClips;
    [SerializeField] private List<string> sfxNames;
    [SerializeField] private List<AudioClip> sfxClips;
    [SerializeField] private AudioSource bgmSource;

    public enum PlayType { CYCLE = 0, ONECYCLE, RANDOM }
    public PlayType PType { get { return playType; } }
    private PlayType playType;

    public bool isPlay { get { return bgmSource.isPlaying; } }
    private bool pause;
    public string[] BgmNames { get { return bgmNames; } }
    public BGMClip[] BgmClips { get { return bgmClips; } }
    private bool[] bgmIndexes;
    public int BgmIndex { get { return bgmIndex; } }
    private int bgmIndex;

    public Dictionary<string, AudioClip> Sfxes { get { return sfxes; } }
    private Dictionary<string, AudioClip> sfxes;

    [Space]
    [SerializeField] private CustomAudioSource sfxPrefab;
    private Dictionary<string, SoundPool> sfxPools;


    public int BgmVolume { get { return bgmVolume; } }
    private int bgmVolume = 1;

    public int SfxVolume { get { return sfxVolume; } }
    private int sfxVolume = 1;

    private bool isLoad = false;

    public void Init()
    {
        playType = PlayType.CYCLE;
        pause = false;

        for (int i = 0; i < bgmNames.Length; i++)
        {
            bgmClips[i].name = bgmNames[i];
        }

        for (int i = 0; i < bgmClips.Length; i++)
        {
            for (int j = i; j < bgmClips.Length; j++)
            {
                if (string.Compare(bgmClips[i].name, bgmClips[j].name) > 0)
                {
                    BGMClip clip = bgmClips[i];
                    bgmClips[i] = bgmClips[j];
                    bgmClips[j] = clip;
                }
            }
        }

        bgmIndexes = new bool[bgmNames.Length];
        for (int i = 0; i < bgmIndexes.Length; i++)
            bgmIndexes[i] = true;

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

        isLoad = true;
    }

    private void Update()
    {
        if (isLoad == false) return;

        if (!pause && !isPlay)
        {
            if (playType == PlayType.CYCLE) ChangeBgm(1);
            else if (playType == PlayType.RANDOM) ShuffleBGM();
        }
    }

    public void SetAllMusic(bool b)
    {
        for (int i = 0; i < bgmIndexes.Length; i++)
        {
            bgmIndexes[i] = b;
            UIController.Instance.AddPlaylist(i, b);
        }
    }

    public bool CheckRemainMusic()
    {
        for (int i = 0; i < bgmIndexes.Length; i++)
            if (bgmIndexes[i]) return true;
        return false;
    }

    public void PlaylistPreset(int index)
    {
        switch (index)
        {
            case 0:
                for (int i = 0; i < bgmIndexes.Length; i++)
                    if (bgmClips[i].n)
                    {
                        bgmIndexes[i] = true;
                        UIController.Instance.AddPlaylist(i, true);
                    }
                break;
            case 1:
                for (int i = 0; i < bgmIndexes.Length; i++)
                    if (bgmClips[i].c)
                    {
                        bgmIndexes[i] = true;
                        UIController.Instance.AddPlaylist(i, true);
                    }
                break;
            case 2:
                for (int i = 0; i < bgmIndexes.Length; i++)
                    if (bgmClips[i].v)
                    {
                        bgmIndexes[i] = true;
                        UIController.Instance.AddPlaylist(i, true);
                    }
                break;
            case 3:
                for (int i = 0; i < bgmIndexes.Length; i++)
                    if (bgmClips[i].t)
                    {
                        bgmIndexes[i] = true;
                        UIController.Instance.AddPlaylist(i, true);
                    }
                break;
        }
    }

    public void PlayPause()
    {
        if (bgmSource.isPlaying)
        {
            pause = true;
            bgmSource.Pause();
        }
        else
        {
            pause = false;
            bgmSource.Play();
        }

        UIController.Instance.PlayPause();
    }

    public void ChangePlaylist(int index, bool b)
    {
        bgmIndexes[index] = b;
        UIController.Instance.AddPlaylist(index, b);
    }

    public void ChangePlayType()
    {
        playType++;

        if (playType > PlayType.RANDOM) playType = PlayType.CYCLE;

        bgmSource.loop = playType == PlayType.ONECYCLE;
        DataManager.SaveSetting();
    }

    public void SetPlayType(int ptype)
    {
        ptype--;
        playType = (PlayType)ptype;
        UIController.Instance.ChangePlayType();
    }

    public void ShuffleBGM()
    {
        if (CheckRemainMusic() == false) return;

        List<int> list = new List<int>();
        for (int i = 0; i < bgmIndexes.Length; i++)
            if (bgmIndex != i && bgmIndexes[i]) list.Add(i);

        int rand = Random.Range(0, list.Count);
        SetBgm(list[rand]);
    }

    public void ChangeBgm(int i)
    {
        if (CheckRemainMusic() == false) return;

        do
        {
            bgmIndex += i;
            if (bgmIndex < 0) bgmIndex = bgmClips.Length - 1;
            else if (bgmIndex >= bgmClips.Length) bgmIndex = 0;
        } while (bgmIndexes[bgmIndex] == false);

        SetBgm(bgmIndex);
    }

    public void SetBgm(int index)
    {
        pause = false;
        bgmIndex = index;
        bgmSource.clip = bgmClips[bgmIndex].clip;
        bgmSource.Play();

        DataManager.SaveSetting();
        UIController.Instance.SetBgm(bgmClips[bgmIndex].name);
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