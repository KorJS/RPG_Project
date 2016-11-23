using UnityEngine;
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

    public AudioSource efxSource = null;
    public AudioSource musicSource = null;

    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;

    void Awake()
    {
        if (soundManager == null)
        {
            soundManager = this;
        }
        else if (soundManager != this)
        {
            Destroy(soundManager);
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

    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }

    public void RandomizeSfx(params AudioClip[] clips)
    {
        if (efxSource.isPlaying)
        {
            return;
        }

        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex];
        efxSource.Play();
    }
}