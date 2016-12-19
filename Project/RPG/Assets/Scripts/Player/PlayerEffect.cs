using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerEffect : MonoBehaviour
{
    private PlayerState playerState = null; // 주인공 상태

    [System.Serializable]
    public class EffectSettings
    {
        public string aggro             = "Aggro";
        public string levelup           = "LevelUp";
        public string recoveryPotion    = "RecoveryPotion";
        public string buffPotion        = "BuffPotion";
    }

    [SerializeField]
    public EffectSettings effectSettings;

    public Dictionary<string, GameObject> effects = null; // 이펙트

    public Transform effectHolder = null;               // 이펙트 부모
    public string    effectPath   = "Effect/Player/";   // 이펙트 프리펩 경로

    void Awake()
    {
        playerState     = GetComponent<PlayerState>();

        effects         = new Dictionary<string, GameObject>();
        effectHolder    = transform.FindChild("EffectHolder");

        ResourceLoad();
    }

    void Update()
    {
        if (effects[effectSettings.aggro].activeSelf)
        {
            playerState.combatTimer = 0f;
        }
    }

    // 이펙트 리소스 로드
    private void ResourceLoad()
    {
        // 어그로
        GameObject obj = Resources.Load(effectPath + effectSettings.aggro) as GameObject;
        effects.Add(effectSettings.aggro, CreateEffectObj(obj, effectSettings.aggro));

        // 레벨업
        obj = Resources.Load(effectPath + effectSettings.levelup) as GameObject;
        effects.Add(effectSettings.levelup, CreateEffectObj(obj, effectSettings.levelup));

        obj = null;
    }

    // 이펙트 오브젝트 생성
    private GameObject CreateEffectObj(GameObject effectObj, string effectName)
    {
        GameObject obj = Instantiate(effectObj) as GameObject;
        obj.name = effectName;
        obj.transform.SetParent(effectHolder);
        obj.transform.localPosition = Vector3.zero;
        obj.SetActive(false);
        
        return obj;
    }

    // 이펙트 활성화 / 활성화 중이면 리턴
    public void CheckActiveEffect(string effect, bool isEffect)
    {
        if (isEffect && effects[effect].activeSelf)
        {
            return;
        }

        effects[effect].SetActive(isEffect);
    }
}