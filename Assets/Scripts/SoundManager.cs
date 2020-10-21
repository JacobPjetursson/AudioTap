using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public AudioSource efxSource;
    public AudioSource musicSource;

    public void PlayEffect(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }
    
    public void PlaySong(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }
}
