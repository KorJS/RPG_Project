using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicianEffect : MonoBehaviour
{
    private MagicianSkill magicianSkill = null;
    private PlayerState playerState = null;

    [System.Serializable]
    public class MagicianEffectSettings
    {
        public Transform leftPoint;
        public Transform rightPoint;
        public Transform unequipEffectHolder;

        public List<GameObject> fireBallObjs;
        
    }

    [SerializeField]
    public MagicianEffectSettings effects;

    void Awake()
    {
        magicianSkill = GetComponent<MagicianSkill>();
        playerState = GetComponent<PlayerState>();

        effects.unequipEffectHolder = GameObject.Find("UnequipEffectPool").transform;

        // 스킬 이펙트 생성
        CreateEffect();
    }

    void Update()
    {
        if (playerState.currentState == TypeData.State.죽음)
        {
            DeathResetEffect();
            return;
        }
    }

    private void CreateEffect()
    {
        var resource = Resources.Load("Effect/Player/Magician/FireBall");

        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(resource) as GameObject;
            obj.transform.SetParent(effects.unequipEffectHolder);
            obj.transform.localPosition = Vector3.zero;
            obj.SetActive(false);
            effects.fireBallObjs.Add(obj);
        }
    }

    public void DeathResetEffect()
    {
        for (int i = 0; i < effects.fireBallObjs.Count; i++)
        {
            if (effects.fireBallObjs[i].activeSelf)
            {
                effects.fireBallObjs[i].SetActive(false);
                effects.fireBallObjs[i].transform.SetParent(effects.unequipEffectHolder);
                effects.fireBallObjs[i].transform.localPosition = Vector3.zero;
            }
        }
    }
}
