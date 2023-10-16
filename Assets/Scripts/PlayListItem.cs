using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class PlayListItem : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject[] members;

    private int index;

    public void SetIcon(int index, BGMClip clip)
    {
        this.index = index;
        nameText.text = clip.name;

        toggle.onValueChanged.AddListener(b => SoundController.Instance.ChangePlaylist(this.index, b));

        members[0].SetActive(clip.n);
        members[1].SetActive(clip.c);
        members[2].SetActive(clip.v);
        members[3].SetActive(clip.t);

        GetComponent<Button>().onClick.AddListener(() => SoundController.Instance.SetBgm(this.index));
        GetComponent<Button>().onClick.AddListener(() => UIController.Instance.PlayPause());
    }

    public void AddPlaylist(bool b)
    {
        toggle.isOn = b;
    }
}
