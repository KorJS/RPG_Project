using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarriorEffect : MonoBehaviour
{
    private Warrior warrior = null;

    [System.Serializable]
    public class Effectsettings
    {
        public GameObject blockObj;
        public GameObject blockDamageObj;
        public GameObject overpowerObj;
        public GameObject rushObj;

        public string block = "BlockEffect";
        public string blockDamage = "BlockDamageEffect";
        public string overpower = "OverpowerEffect";
        public string rush = "RushEffect";
    }

    public Effectsettings effectSettings;

    private Transform skillHolder = null;

    public string effectPath = "Effect/Warrior";

    void Awake()
    {
        warrior = GetComponent<Warrior>();
        skillHolder = transform.FindChild("SkillHolder");

        ResourceLoad();
    }

    // 이펙트 리소스 로드
    private void ResourceLoad()
    {
        effectSettings.blockObj = Resources.Load(effectPath + effectSettings.block) as GameObject;
        effectSettings.blockDamageObj = Resources.Load(effectPath + effectSettings.blockDamage) as GameObject;
        effectSettings.overpowerObj = Resources.Load(effectPath + effectSettings.overpower) as GameObject;
        effectSettings.rushObj = Resources.Load(effectPath + effectSettings.rush) as GameObject;
    }

    // TODO : 이펙트 이름으로 이펙트 생성, 위치 설정 ( 각 이펙트 프리펩에 지속시간 설정 )

    // 방패막기중 효과
    public void BlockEffect()
    {
        if (!warrior.isBlock)
        {
            return;
        }

    }

    // 방패막기중에 맞았을때 효과
    public void BlockDamageEffect()
    {
        if (!warrior.isBlock)
        {
            return;
        }

    }
}