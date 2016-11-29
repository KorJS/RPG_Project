using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarriorEffect : MonoBehaviour
{
    private PlayerMovement playerMovement = null;
    private WarriorSkill warriorSkill = null;
    private WarriorSound warriorSound = null;
    private UIManager uiManager = null;

    [System.Serializable]
    public class EffectSettings
    {
        public string block         = "Block";
        public string blockDamage   = "BlockDamage";
        public string overpower     = "Overpower";
        public string rush          = "Rush";
        public string hit           = "Hit";

        public Transform skillHolder;
        public string effectPath;
    }

    public EffectSettings effectSettings;

    public Dictionary<string, GameObject> effects = null;



    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        warriorSkill = GetComponent<WarriorSkill>();
        warriorSound = GetComponent<WarriorSound>();
        effects = new Dictionary<string, GameObject>();
        effectSettings.skillHolder = transform.FindChild("SkillHolder");
        effectSettings.effectPath = "Effect/Player/Warrior/";
        ResourceLoad();
    }

    void Start()
    {
        uiManager = UIManager.Instance;
    }

    void Update()
    {
        if (GameManager.Instance.currentGameState == TypeData.GameState.종료)
        {
            return;
        }

        //BlockEffect();
        //BlockDamageEffect();
    }

    // 이펙트 리소스 로드
    private void ResourceLoad()
    {
        GameObject obj = Resources.Load(effectSettings.effectPath + effectSettings.block) as GameObject;
        effects.Add(effectSettings.block, CreateEffectObj(obj, effectSettings.block));

        obj = Resources.Load(effectSettings.effectPath + effectSettings.blockDamage) as GameObject;
        effects.Add(effectSettings.blockDamage, CreateEffectObj(obj, effectSettings.blockDamage));

        obj = Resources.Load(effectSettings.effectPath + effectSettings.overpower) as GameObject;
        effects.Add(effectSettings.overpower, CreateEffectObj(obj, effectSettings.overpower));

        obj = Resources.Load(effectSettings.effectPath + effectSettings.rush) as GameObject;
        effects.Add(effectSettings.rush, CreateEffectObj(obj, effectSettings.rush));

        obj = Resources.Load(effectSettings.effectPath + effectSettings.hit) as GameObject;
        effects.Add(effectSettings.hit, CreateEffectObj(obj, effectSettings.hit));

        obj = null;
    }

    // 이펙트 오브젝트 생성
    private GameObject CreateEffectObj(GameObject effectObj, string effectName)
    {
        GameObject obj = Instantiate(effectObj) as GameObject;
        obj.name = effectName;
        obj.transform.SetParent(effectSettings.skillHolder);
        obj.GetComponent<EffectSetting>().infoSettings.effectHoler = effectSettings.skillHolder;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
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

            if (uiManager.blockTxtObj.activeSelf)
            {
                uiManager.blockTxtObj.SetActive(false);
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

        if (uiManager.blockTxtObj.activeSelf)
        {
            uiManager.blockTxtObj.SetActive(false);
            uiManager.blockTween.ResetToBeginning();
        }
        uiManager.blockTxtObj.SetActive(true);
        uiManager.blockTween.Play();
        effects[effectSettings.blockDamage].SetActive(true);

        // TODO : 검색.. 계속하게 되는데. 나중에 수정하자
        // 이펙트 발생하고 정지 상태이면 비활성화.
        if (effects[effectSettings.blockDamage].GetComponent<ParticleSystem>().isStopped)
        {
            playerMovement.isDamage = false;
        }
    }

    public void BlockEndEffect()
    {
        if (uiManager.blockTxtObj.activeSelf)
        {
            uiManager.blockTxtObj.SetActive(false);
            uiManager.blockTween.ResetToBeginning();
        }
    }

    public void OverpowerEffect()
    {
        if (effects[effectSettings.overpower].activeSelf)
        {
            if (effects[effectSettings.overpower].transform.parent != effectSettings.skillHolder)
            {
                effects[effectSettings.overpower].transform.SetParent(effectSettings.skillHolder);
            }
            effects[effectSettings.overpower].SetActive(false);
        }

        effects[effectSettings.overpower].SetActive(true);
    }

    public void RushEffect()
    {
        if (effects[effectSettings.rush].activeSelf)
        {
            effects[effectSettings.rush].SetActive(false);
        }

        effects[effectSettings.rush].SetActive(true);
        warriorSound.SetRushBGM();
    }

    public void ComboHitEffect(bool isEffect)
    {
        if (isEffect && effects[effectSettings.hit].activeSelf)
        {
            return;
        }

        effects[effectSettings.hit].SetActive(isEffect);
    }
}