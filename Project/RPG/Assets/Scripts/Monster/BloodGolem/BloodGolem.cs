using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BloodGolem : MonoBehaviour
{
    private MonsterMovement monsterMovement = null;
    private MonsterState monsterState = null;

    [System.Serializable]
    public class BloodGolemAniSettings
    {
        public string isNormalTrigger   = "isNormalAtk";
        public string isStunTrigger     = "isStunAtk";
        public string isHeavyTrigger    = "isHeavyAtk";
        public string isRoundTrigger    = "isRoundAtk";
        public string isUtraTrigger     = "isUtraAtk";
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

    void Awake()
    {
        monsterMovement = GetComponent<MonsterMovement>();
        monsterState = GetComponent<MonsterState>();

        currentSkillType = SkillType.없음;
        nextSkillType = SkillType.없음;
    }

    void Update()
    {
        SwitchSkill();
    }

    // 블러드 골램 패턴
    // 기본 패턴 : 평타 4 > 내려찍기스턴 > 후려치기 
    // 30% 확률로 웅크리기 패턴
    // HP 50%, 20% 가 되면 필살기 패턴
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
                break;
            case SkillType.내려찍기스턴:
                break;
            case SkillType.후려치기:
                break;
            case SkillType.웅크리기:
                break;
            case SkillType.필살기:
                break;
        }
    }

    private void NormalAttack()
    {
        monsterMovement.SetAniSkill((int)currentSkillType);
    }
}