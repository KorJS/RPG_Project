using UnityEngine;
using System.Collections;

public class BloodGolemEffect : MonoBehaviour
{
    private BloodGolem bloodGolem = null;
    private BloodGolemSkill bloodGolemSkill = null;

    [System.Serializable]
    public class EffectSettings
    {
        
    }

    [SerializeField]
    public EffectSettings effectSettings;

    void Awake()
    {
        bloodGolem = GetComponent<BloodGolem>();
        bloodGolemSkill = GetComponent<BloodGolemSkill>();
    }

    void Update()
    {

    }
}