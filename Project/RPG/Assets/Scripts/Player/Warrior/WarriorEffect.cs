using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarriorEffect : MonoBehaviour
{
    private PlayerMovement playerMovement = null;
    private Warrior warrior = null;

    [System.Serializable]
    public class EffectSettings
    {
        public GameObject blockObj;
        public GameObject blockDamageObj;
        public GameObject overpowerObj;
        public GameObject rushObj;

        public string block         = "BlockEffect";
        public string blockDamage   = "BlockDamageEffect";
        public string overpower     = "OverpowerEffect";
        public string rush          = "RushEffect";
    }

    public EffectSettings effectSettings;

    private Transform skillHolder = null;

    public  string effectPath = "Effect/Warrior/";

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        warrior = GetComponent<Warrior>();
        skillHolder = transform.FindChild("SkillHolder");

        ResourceLoad();
    }

    void Update()
    {
        BlockEffect();
        BlockDamageEffect();
    }

    // 이펙트 리소스 로드
    private void ResourceLoad()
    {
        GameObject obj = Resources.Load(effectPath + effectSettings.block) as GameObject;
        effectSettings.blockObj = CreateEffectObj(obj, effectSettings.block);

        obj = Resources.Load(effectPath + effectSettings.blockDamage) as GameObject;
        effectSettings.blockDamageObj = CreateEffectObj(obj, effectSettings.blockDamage);

        obj = Resources.Load(effectPath + effectSettings.overpower) as GameObject;
        effectSettings.overpowerObj = CreateEffectObj(obj, effectSettings.overpower);

        obj = Resources.Load(effectPath + effectSettings.rush) as GameObject;
        effectSettings.rushObj = CreateEffectObj(obj, effectSettings.rush);

        obj = null;
    }

    // 이펙트 오브젝트 생성
    private GameObject CreateEffectObj(GameObject effectObj, string effectName)
    {
        GameObject obj = Instantiate(effectObj) as GameObject;
        obj.name = effectName;
        obj.transform.SetParent(skillHolder);
        obj.transform.localPosition = Vector3.zero;
        obj.SetActive(false);

        return obj;
    }

    // TODO : 이펙트 이름으로 이펙트 생성, 위치 설정 ( 각 이펙트 프리펩에 지속시간 설정 )

    // 방패막기중 효과
    private void BlockEffect()
    {
        // 방패막기가 아닐때 
        if (!warrior.isBlock)
        {
            // 이펙트가 활성화 상태이면
            if (effectSettings.blockObj.activeSelf)
            {
                effectSettings.blockObj.SetActive(false);
            }
            return;
        }

        effectSettings.blockObj.SetActive(true);
    }

    // 방패막기중에 맞았을때 효과
    private void BlockDamageEffect()
    {
        // 방패막기가 아니거나 / 데미지를 안받았을때
        if (!warrior.isBlock || !playerMovement.isDamage)
        {
            return;
        }

        effectSettings.blockDamageObj.SetActive(true);

        // TODO : 검색.. 계속하게 되는데. 나중에 수정하자
        // 이펙트 발생하고 정지 상태이면 비활성화.
        if (effectSettings.blockDamageObj.GetComponent<ParticleSystem>().isStopped)
        {
            playerMovement.isDamage = false;
        }
    }
}