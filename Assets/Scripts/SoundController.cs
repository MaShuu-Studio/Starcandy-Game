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
    public List<int> Playlist
    {
        get
        {
            List<int> plist = new List<int>();
            foreach (var clip in bgmClips)
            {
                if (clip.isPlist) plist.Add(clip.index);
            }
            plist.Sort();
            return plist;
        }
    }
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
            bgmClips[i].index = i;
            bgmClips[i].name = bgmNames[i];
        }

        // 재생목록 정렬
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

        if (isPlay) UIController.Instance.ChangeProgress(bgmSource.time / bgmSource.clip.length);

        if (!pause && !isPlay)
        {
            if (playType == PlayType.CYCLE) ChangeBgm(1);
            else if (playType == PlayType.RANDOM) ShuffleBGM();
        }
    }

    public void SetAllMusic(bool b)
    {
        for (int i = 0; i < bgmClips.Length; i++)
            ChangePlaylist(i, b);
    }

    public void ChangeBGMProgress(float p)
    {
        bgmSource.time = p * bgmSource.clip.length;
    }

    public bool CheckRemainMusic()
    {
        for (int i = 0; i < bgmClips.Length; i++)
            if (bgmClips[i].isPlist) return true;
        return false;
    }

    public void SetPlaylist(int amount, List<int> plist)
    {
        if (plist.Count == 0) SetAllMusic(true);
        else
        {
            SetAllMusic(false);
            // 이미 노래의 정렬이 끝났기 때문에 직접 세팅하러 다닐 필요가 있음.
            // Init의 순서를 바꾸는 것도 방법이지만 노래의 갯수가 기하급수적으로 많지 않으므로
            // O(n^2)로 세팅해주는 대신에 다른 코드와 충돌이 나지 않도록 함.

            // 새롭게 추가된 곡이 있을 경우
            if (bgmClips.Length > amount)
            {
                for (int index = amount; index < bgmClips.Length; index++)
                    plist.Add(index);
            }

            foreach (var index in plist)
            {
                for (int i = 0; i < bgmClips.Length; i++)
                {
                    if (bgmClips[i].index == index)
                    {
                        ChangePlaylist(i, true);
                        break;
                    }
                }
            }

        }
    }

    public void PlaylistPreset(int index)
    {
        switch (index)
        {
            case 0:
                for (int i = 0; i < bgmClips.Length; i++)
                    if (bgmClips[i].n)
                        ChangePlaylist(i, true);
                break;
            case 1:
                for (int i = 0; i < bgmClips.Length; i++)
                    if (bgmClips[i].c)
                        ChangePlaylist(i, true);
                break;
            case 2:
                for (int i = 0; i < bgmClips.Length; i++)
                    if (bgmClips[i].v)
                        ChangePlaylist(i, true);
                break;
            case 3:
                for (int i = 0; i < bgmClips.Length; i++)
                    if (bgmClips[i].t)
                        ChangePlaylist(i, true);
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
        bgmClips[index].isPlist = b;
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
        for (int i = 0; i < bgmClips.Length; i++)
            if (bgmIndex != i && bgmClips[i].isPlist) list.Add(i);

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
        } while (bgmClips[bgmIndex].isPlist == false);

        SetBgm(bgmIndex);
    }

    public void SetBgm(int index)
    {
        pause = false;
        bgmIndex = index;
        bgmSource.clip = bgmClips[bgmIndex].clip;
        bgmSource.time = 0;
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