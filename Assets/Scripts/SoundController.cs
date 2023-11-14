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

    private List<int>[] memberPlaylist;

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

        // ������ ����
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

        // ����� �÷��̸���Ʈ ����
        memberPlaylist = new List<int>[4];
        for (int m = 0; m < memberPlaylist.Length; m++)
        {
            memberPlaylist[m] = new List<int>();
            for (int i = 0; i < bgmClips.Length; i++)
            {
                if (m == 0 && bgmClips[i].n
                    || m == 1 && bgmClips[i].c
                    || m == 2 && bgmClips[i].v
                    || m == 3 && bgmClips[i].t)
                    memberPlaylist[m].Add(i);
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

    private void SetAllMusic(bool b)
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
            // �̹� �뷡�� ������ ������ ������ ���� �����Ϸ� �ٴ� �ʿ䰡 ����.
            // Init�� ������ �ٲٴ� �͵� ��������� �뷡�� ������ ���ϱ޼������� ���� �����Ƿ�
            // O(n^2)�� �������ִ� ��ſ� �ٸ� �ڵ�� �浹�� ���� �ʵ��� ��.

            // ���Ӱ� �߰��� ���� ���� ���
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

    public void PlaylistPreset(int member)
    {
        bool b = false;
        // �ϳ��� false�� true�� �ٲ�����
        // ���� true�� false��.

        // -1�� ��쿡�� ALL�� �� �ܿ��� �����.
        if (member == -1)
        {
            for (int i = 0; i < bgmClips.Length; i++)
            {
                if (bgmClips[i].isPlist == false)
                {
                    b = true;
                    break;
                }
            }

            SetAllMusic(b);
        }
        else
        {
            for (int i = 0; i < memberPlaylist[member].Count; i++)
            {
                int index = memberPlaylist[member][i];
                if (bgmClips[index].isPlist == false)
                {
                    b = true;
                    break;
                }
            }

            for (int i = 0; i < memberPlaylist[member].Count; i++)
            {
                int index = memberPlaylist[member][i];
                ChangePlaylist(index, b);
            }
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