using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class CustomAudioSource : Poolable
{
    private AudioSource source;
    private bool play;

    private void Update()
    {
        if (play) source.volume = SoundController.Instance.SfxVolume;

        if (play && source.isPlaying == false)
        {
            source.Stop();
            SoundController.Instance.StopAudio(name, this);
            play = false;
        }
    }

    public override void MakePrefab(string name)
    {
        _name = name;
        source = GetComponent<AudioSource>();
        source.volume = 1;
        source.loop = false;
        AudioClip clip = SoundController.Instance.Sfxes[name];

        source.clip = clip;
        source.Stop();
    }

    private void OnEnable()
    {
        if (source != null)
        {
            source.volume = SoundController.Instance.SfxVolume;
            source.Play();
            play = true;
        }
    }
}