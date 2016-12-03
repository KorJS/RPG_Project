using UnityEngine;
using System.Collections;

public class MonsterSound : MonoBehaviour
{
    public AudioSource voiceSource = null;
    public AudioSource efxSource = null;

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
