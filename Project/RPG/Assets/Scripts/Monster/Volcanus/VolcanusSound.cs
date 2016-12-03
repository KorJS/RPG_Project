using UnityEngine;
using System.Collections;

public class VolcanusSound : MonoBehaviour
{
    private MonsterSound monsterSound = null;

    [System.Serializable]
    public class AudioSettings
    {
        public AudioClip skillWait01BGM;
        public AudioClip skillWait02BGM;
        public AudioClip[] walkBGM;
        public AudioClip[] turnBGM;
        public AudioClip deathBGM;
        public AudioClip attack01BGM;
    }

    [SerializeField]
    public AudioSettings audioSettings;

    void Awake()
    {
        monsterSound = GetComponent<MonsterSound>();
    }

    public void SetSkillWait01BGM()
    {
        monsterSound.PlaySingleVoice(audioSettings.skillWait01BGM);
    }

    public void SetSkillWait02BGM()
    {
        monsterSound.PlaySingleVoice(audioSettings.skillWait02BGM);
    }

    public void SetWalkBGM()
    {
        monsterSound.RandomVoice(audioSettings.walkBGM);
    }

    public void SetTurnBGM()
    {
        monsterSound.RandomVoice(audioSettings.turnBGM);
    }

    public void SetDeathBGM()
    {
        monsterSound.PlaySingleVoice(audioSettings.deathBGM);
    }

    public void SetAttBGM()
    {
        monsterSound.PlaySingleVoice(audioSettings.attack01BGM);
    }
}
