using UnityEngine;
using System.Collections.Generic;
using System;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance = null;

    public AudioSource sfxSource;
    public AudioSource chronoSource;

    public AudioClip findCharacter;
    public AudioClip wrongCharacter;
    public AudioClip chooseMenu;
    public AudioClip newRound;
    public AudioClip deafeatSound;
    public AudioClip tikChrono;



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    public void PlayChrono(AudioClip clip, float volume)
    {
        chronoSource.volume = volume;
        chronoSource.PlayOneShot(clip);
    }
}
