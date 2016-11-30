using UnityEngine;
using System.Collections;

public class WarriorSound : MonoBehaviour
{
    private SoundManager soundManager = null;

    [System.Serializable]
    public class AudioSettings
    {
        public AudioClip combo01BGM;
        public AudioClip combo02BGM;
        public AudioClip comboAtt01BGM;
        public AudioClip comboAtt02BGM;
        public AudioClip tumblingBGM;
        public AudioClip overpowerBGM;
        public AudioClip rushBGM;
        public AudioClip[] shieldDefenceBGM;
    }

    [SerializeField]
    public AudioSettings audioSettings;

    void Start()
    {
        soundManager = SoundManager.Instance;
    }

    public void SetComboBGM()
    {
        soundManager.RandomVoice(audioSettings.combo01BGM, audioSettings.combo02BGM);
        soundManager.RandomEfx(audioSettings.comboAtt01BGM, audioSettings.comboAtt02BGM);
    }

    public void SetTumblingBGM()
    {
        soundManager.PlaySingleEfx(audioSettings.tumblingBGM);
    }

    public void SetOverpowerBGM()
    {
        soundManager.PlaySingleEfx(audioSettings.overpowerBGM);
    }

    public void SetRushBGM()
    {
        soundManager.PlaySingleEfx(audioSettings.rushBGM);
    }

    public void SetDamageBlockBGM()
    {
        soundManager.RandomEfx(audioSettings.shieldDefenceBGM);
    }
}
