using UnityEngine;
using System.Collections;

public class BloodGolemEffect : MonoBehaviour
{
    private BloodGolemSkill bloodGolemSkill = null;

    [System.Serializable]
    public class EffectSettings
    {
        
    }

    [SerializeField]
    public EffectSettings effectSettings;

    void Awake()
    {
        bloodGolemSkill = GetComponent<BloodGolemSkill>();
    }

    void Update()
    {

    }
}