﻿using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    private static SoundManager soundManager = null;
    public static SoundManager Instance
    {
        get
        {
            if (soundManager == null)
            {
                Debug.Log("SoundManager Script Null");
            }

            return soundManager;
        }
    }

    public AudioSource voiceSource = null;
    public AudioSource efxSource = null;
    public AudioSource uiSource = null;
    public AudioSource musicSource = null;
    public AudioSource systemSource = null;

    void Awake()
    {
        if (soundManager == null)
        {
            soundManager = this;
        }
        else if (soundManager != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    public void PlayBackMusic(AudioClip clip)
    {
        if (musicSource.clip != clip)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public void PlaySingleUI(AudioClip clip)
    {
        uiSource.clip = clip;
        uiSource.PlayOneShot(uiSource.clip);
    }

    public void PlaySingleSystem(AudioClip clip)
    {
        systemSource.clip = clip;
        uiSource.PlayOneShot(systemSource.clip);
    }


    public void PlaySingleEfx(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.PlayOneShot(efxSource.clip);
    }

    public void RandomEfx(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);

        efxSource.pitch = 0.8f;
        efxSource.clip = clips[randomIndex];
        efxSource.PlayOneShot(efxSource.clip);
    }

    public void PlaySingleVoice(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.PlayOneShot(efxSource.clip);
    }

    public void RandomVoice(params AudioClip[] clips)
    {
        if (efxSource.isPlaying)
        {
            return;
        }

        int randomIndex = Random.Range(0, clips.Length);

        voiceSource.pitch = 1f;
        voiceSource.clip = clips[randomIndex];
        voiceSource.Play();
    }
}