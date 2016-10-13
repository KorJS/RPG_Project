using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerEffect : MonoBehaviour
{
    [System.Serializable]
    public class EffectSettings
    {
        public string aggro = "Aggro";
    }

    [SerializeField]
    public EffectSettings effectSettings;

    public Dictionary<string, GameObject> effects = null;

    public Transform effectHolder = null;

    public string effectPath = "Effect/Player/";
    public GameObject oo = null;
    void Awake()
    {
        effects = new Dictionary<string, GameObject>();
        effectHolder = transform.FindChild("EffectHolder");

        ResourceLoad();
    }

    // 이펙트 리소스 로드
    private void ResourceLoad()
    {
        GameObject obj = Resources.Load(effectPath + effectSettings.aggro) as GameObject;
        effects.Add(effectSettings.aggro, CreateEffectObj(obj, effectSettings.aggro));
    }

    // 이펙트 오브젝트 생성
    private GameObject CreateEffectObj(GameObject effectObj, string effectName)
    {
        GameObject obj = Instantiate(effectObj) as GameObject;
        obj.name = effectName;
        obj.transform.SetParent(effectHolder);
        obj.transform.localPosition = Vector3.zero;
        obj.SetActive(false);
        oo = obj;
        return obj;
    }

    public void CheckActiveEffect(string effect, bool isEffect)
    {
        effects[effect].SetActive(isEffect);
    }
}