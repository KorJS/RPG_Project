using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarriorEffect : MonoBehaviour
{
    private PlayerMovement playerMovement = null;
    private WarriorSkill warriorSkill = null;

    [System.Serializable]
    public class EffectSettings
    {
        public string block         = "Block";
        public string blockDamage   = "BlockDamage";
        public string overpower     = "Overpower";
        public string rush          = "Rush";
    }

    public EffectSettings effectSettings;

    public Dictionary<string, GameObject> effects = null;

    private Transform skillHolder = null;

    public  string effectPath = "Effect/Player/Warrior/";

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        warriorSkill = GetComponent<WarriorSkill>();
        effects = new Dictionary<string, GameObject>();
        skillHolder = transform.FindChild("SkillHolder");

        ResourceLoad();
    }

    void Update()
    {
        BlockEffect();
        //BlockDamageEffect();
    }

    // 이펙트 리소스 로드
    private void ResourceLoad()
    {
        GameObject obj = Resources.Load(effectPath + effectSettings.block) as GameObject;
        effects.Add(effectSettings.block, CreateEffectObj(obj, effectSettings.block));

        obj = Resources.Load(effectPath + effectSettings.blockDamage) as GameObject;
        effects.Add(effectSettings.blockDamage, CreateEffectObj(obj, effectSettings.blockDamage));

        obj = Resources.Load(effectPath + effectSettings.overpower) as GameObject;
        effects.Add(effectSettings.overpower, CreateEffectObj(obj, effectSettings.overpower));

        obj = Resources.Load(effectPath + effectSettings.rush) as GameObject;
        effects.Add(effectSettings.rush, CreateEffectObj(obj, effectSettings.rush));

        obj = null;
    }

    // 이펙트 오브젝트 생성
    private GameObject CreateEffectObj(GameObject effectObj, string effectName)
    {
        GameObject obj = Instantiate(effectObj) as GameObject;
        obj.name = effectName;
        obj.transform.SetParent(skillHolder);
        obj.GetComponent<EffectSetting>().infoSettings.effectHoler = skillHolder;
        obj.transform.localPosition = Vector3.zero;
        obj.SetActive(false);

        return obj;
    }

    // TODO : 이펙트 이름으로 이펙트 생성, 위치 설정 ( 각 이펙트 프리펩에 지속시간 설정 )

    // 방패막기중 효과
    private void BlockEffect()
    {
        // 방패막기가 아닐때 
        if (!warriorSkill.isBlock)
        {
            // 이펙트가 활성화 상태이면
            if (effects[effectSettings.block].activeSelf)
            {
                effects[effectSettings.block].SetActive(false);
            }
            return;
        }

        effects[effectSettings.block].SetActive(true);
    }

    // 방패막기중에 맞았을때 효과
    public void BlockDamageEffect()
    {
        //// 방패막기가 아니거나 / 데미지를 안받았을때
        //if (!warriorSkill.isBlock || !playerMovement.isDamage)
        //{
        //    return;
        //}

        effects[effectSettings.blockDamage].SetActive(true);

        // TODO : 검색.. 계속하게 되는데. 나중에 수정하자
        // 이펙트 발생하고 정지 상태이면 비활성화.
        if (effects[effectSettings.blockDamage].GetComponent<ParticleSystem>().isStopped)
        {
            playerMovement.isDamage = false;
        }
    }

    public void OverpowerEffect()
    {
        if (effects[effectSettings.overpower].activeSelf)
        {
            effects[effectSettings.overpower].transform.SetParent(skillHolder);
            effects[effectSettings.overpower].SetActive(false);
        }

        effects[effectSettings.overpower].SetActive(true);
    }
}