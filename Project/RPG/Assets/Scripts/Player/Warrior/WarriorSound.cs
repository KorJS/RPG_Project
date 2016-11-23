using UnityEngine;
using System.Collections;

public class WarriorSound : MonoBehaviour
{
    private SoundManager soundManager = null;

    [System.Serializable]
    public class AudioSettings
    {
        public AudioClip comboBGM;
        public AudioClip tumblingBGM;
        public AudioClip overpowerBGM;
        public AudioClip rushBGM;
        public AudioClip startBlockBGM;
        public AudioClip damageBlockBGM;
    }

    [SerializeField]
    public AudioSettings audioSettings;

    void Start()
    {
        soundManager = SoundManager.Instance;
    }

    public void SetComboBGM()
    {
        
    }

    public void SetTumblingBGM()
    {

    }

    public void SetOverpowerBGM()
    {

    }

    public void SetStartBlockBGM()
    {

    }

    public void SetDamageBlockBGM()
    {

    }
}
