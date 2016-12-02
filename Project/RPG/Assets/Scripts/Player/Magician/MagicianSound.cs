using UnityEngine;
using System.Collections;

public class MagicianSound : MonoBehaviour
{
    private SoundManager soundManager = null;

    [System.Serializable]
    public class AudioSettings
    {
        public AudioClip[] fireballBGM;
        public AudioClip mpCondensingBGM;
        public AudioClip teleportBGM;
        public AudioClip iceStormBGM;
        public AudioClip meteorAuraBGM;
        public AudioClip[] meteorShotBGM;
    }

    [SerializeField]
    public AudioSettings audioSettings;

    void Start()
    {
        soundManager = SoundManager.Instance;
    }

    public void SetFireBallBGM()
    {
        soundManager.RandomEfx(audioSettings.fireballBGM);
    }

    public void SetMpCondensingBGM()
    {
        soundManager.PlaySingleEfx(audioSettings.mpCondensingBGM);
    }

    public void SetTeleportBGM()
    {
        soundManager.PlaySingleEfx(audioSettings.teleportBGM);
    }

    public void SetIceStormBGM()
    {
        soundManager.PlaySingleEfx(audioSettings.iceStormBGM);
    }
}