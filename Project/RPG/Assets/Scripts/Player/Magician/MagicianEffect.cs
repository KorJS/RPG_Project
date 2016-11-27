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

    private int fireBallCount = 0;

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

    public void FireBall(Vector3 lookPos, float skillAtt)
    {
        if (fireBallCount >= effects.fireBallObjs.Count)
        {
            fireBallCount = 0;
        }

        // 사용자, ball 처음위치, 방향, 스피드, 최대거리, 공격력, 히트이펙트이름
        effects.fireBallObjs[fireBallCount].transform.position = effects.rightPoint.position;
        effects.fireBallObjs[fireBallCount].SetActive(true);

        BallControl ballControl = effects.fireBallObjs[fireBallCount].GetComponent<BallControl>();
        ballControl.SetBall(gameObject.transform, effects.rightPoint.position, lookPos, 8, 15, skillAtt, "FireBall");

        fireBallCount++;
    }
}
