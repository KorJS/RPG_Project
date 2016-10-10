﻿using UnityEngine;
using System.Collections;

public class BloodGolemSkill : MonoBehaviour
{
    private MonsterMovement monsterMovement = null;
    private MonsterState monsterState = null;

    [System.Serializable]
    public class BloodGolemAniSettings
    {
        public string isNormalTrigger = "isNormalAtk";
        public string isStunTrigger = "isStunAtk";
        public string isHeavyTrigger = "isHeavyAtk01";
        public string isRoundTrigger = "isRoundAtk";
        public string isUtraTrigger = "isUtraAtk";

        public string normalAtkClip = "normalAtk";
        public string stunAtkClip = "stunAtk";
        public string heavyAtk01Clip = "heavyAtk01";
        public string roundAtkClip = "roundAtk01";
    }

    [SerializeField]
    public BloodGolemAniSettings bloodGolemAniSettings;

    public enum SkillType
    {
        없음 = -1,
        일반공격 = 0,
        내려찍기스턴,
        후려치기,
        웅크리기,
        필살기
    };

    public SkillType currentSkillType = SkillType.없음;
    public SkillType nextSkillType = SkillType.없음;

    // TODO : 몬스터의 공격력은 고정. 주인공쪽에서 들어온 데미지를 방어에 따라 데미지 변화를 주자
    public float skillAngle = 0f;
    public float skillRange = 0f;

    public int patternCount = 0;
    public int test = 100;
    void Awake()
    {
        monsterMovement = GetComponent<MonsterMovement>();
        monsterState = GetComponent<MonsterState>();

        currentSkillType = SkillType.없음;
        nextSkillType = SkillType.일반공격;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            monsterState.nextState = TypeData.State.스킬;
        }

        if (monsterState.currentState != TypeData.State.스킬)
        {
            return;
        }


        Pattern();
        SwitchSkill();
    }

    private void ActiveSkill(SkillType skillType, string aniTrigger)
    {
        monsterMovement.SetAniSkill((int)skillType);
        monsterMovement.animator.SetTrigger(aniTrigger);
    }

    private void Pattern()
    {
        float a = Random.Range(0f, 3000f);
        if (a > 10f && a < 15f)
        {
            Debug.Log(a);
            nextSkillType = SkillType.내려찍기스턴;
            return;
        }

        if (patternCount < 3 && monsterMovement.isIdle)
        {
            nextSkillType = SkillType.일반공격;
        }

        if (patternCount >= 3 && monsterMovement.isIdle)
        {
            nextSkillType = SkillType.후려치기;
        }
    }

    private void SwitchSkill()
    {
        if (nextSkillType == SkillType.없음)
        {
            return;
        }

        currentSkillType = nextSkillType;
        nextSkillType = SkillType.없음;

        switch (currentSkillType)
        {
            case SkillType.일반공격:
                {
                    ActiveSkill(currentSkillType, bloodGolemAniSettings.isNormalTrigger);
                    patternCount++;
                }
                break;

            case SkillType.내려찍기스턴:
                {
                    ActiveSkill(currentSkillType, bloodGolemAniSettings.isStunTrigger);
                }
                break;

            case SkillType.후려치기:
                {
                    ActiveSkill(currentSkillType, bloodGolemAniSettings.isHeavyTrigger);
                    patternCount = 0;
                }
                break;

            case SkillType.웅크리기:
                {
                    ActiveSkill(currentSkillType, bloodGolemAniSettings.isRoundTrigger);
                }
                break;
            case SkillType.필살기:
                {
                    ActiveSkill(currentSkillType, bloodGolemAniSettings.isUtraTrigger);
                }
                break;
        }
    }
}
