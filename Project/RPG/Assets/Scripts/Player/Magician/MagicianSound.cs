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
        public AudioClip startTeleportBGM;
        public AudioClip endTeleportBGM;
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

    public void FireBallBGM()
    {
        soundManager.RandomEfx(audioSettings.fireballBGM);
    }

    public void MpCondensingBGM()
    {
        soundManager.PlaySingleEfx(audioSettings.mpCondensingBGM);
    }

    public void StartTeleportBGM()
    {
        soundManager.PlaySingleEfx(audioSettings.startTeleportBGM);
    }

    public void EndTeleportBGM()
    {
        soundManager.PlaySingleEfx(audioSettings.endTeleportBGM);
    }

    public void IceStormBGM()
    {
        soundManager.PlaySingleEfx(audioSettings.iceStormBGM);
    }
}