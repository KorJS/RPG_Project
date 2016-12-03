using UnityEngine;
using System.Collections;

public class KumasSound : MonoBehaviour
{
    private MonsterSound monsterSound = null;

    [System.Serializable]
    public class AudioSettings
    {
        public AudioClip[] walkBGM;
        public AudioClip[] turnBGM;
        public AudioClip deathBGM;
        public AudioClip[] attack01BGM;
        public AudioClip attack02BGM;
        public AudioClip attack04BGM;
        public AudioClip[] combo01BGM;
        public AudioClip breathStartBGM;
        public AudioClip breathShotBGM;
    }

    [SerializeField]
    public AudioSettings audioSettings;

    void Awake()
    {
        monsterSound = GetComponent<MonsterSound>();
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

    public void SetAtt01BGM()
    {
        monsterSound.RandomVoice(audioSettings.attack01BGM);
    }

    public void SetAtt02BGM()
    {
        monsterSound.PlaySingleVoice(audioSettings.attack02BGM);
    }

    public void SetAtt04BGM()
    {
        monsterSound.PlaySingleVoice(audioSettings.attack04BGM);
    }

    public void SetCombo01BGM()
    {
        monsterSound.RandomVoice(audioSettings.combo01BGM);
    }

    public void SetBreathStartBGM()
    {
        monsterSound.PlaySingleVoice(audioSettings.breathStartBGM);
    }

    public void SetBreathShotBGM()
    {
        monsterSound.PlaySingleVoice(audioSettings.breathShotBGM);
    }
}