﻿using UnityEngine;
using System.Collections;

public class KumasSkill : MonoBehaviour
{
    private MonsterInfoData monsterInfoData = null;
    private MonsterState monsterState = null;
    private MonsterRange monsterRange = null;
    private MonsterMovement monsterMovement = null;

    public enum SkillType
    {
        없음 = -1,
        att01 = 0,
        att02, att03, att04, att05,
        combo_att01,
        MAX
    }

    public SkillType skillType = SkillType.없음;

    void Awake()
    {
        monsterInfoData = GetComponent<MonsterInfoData>();
        monsterState = GetComponent<MonsterState>();
        monsterRange = GetComponent<MonsterRange>();
        monsterMovement = GetComponent<MonsterMovement>();

        skillType = SkillType.없음;
    }

    void OnEnable()
    {
        skillType = SkillType.없음;
    } 

    void Update()
    {
        if (monsterState.currentState == TypeData.MonsterState.죽음)
        {
            return;
        }

        if (monsterState.currentMode == TypeData.MODE.평화)
        {
            ResetSkill();
            return;
        }

        if (monsterState.currentState != TypeData.MonsterState.스킬)
        {
            ResetSkill();
            return;
        }

        if (monsterMovement.isSkill)
        {
            Debug.Log("?????????????????");
            monsterMovement.isSkill = false;
            StartCoroutine(Attack());
        }
    }

    // 애니메이션 이벤트 - 스킬이 끝날때쯤 호출
    public void ResetSkill()
    {
        skillType = SkillType.없음;
    }

    IEnumerator Attack()
    {
        while(monsterState.currentState == TypeData.MonsterState.스킬)
        {
            yield return new WaitForSeconds(3f);
            skillType++;
            if (skillType == SkillType.MAX)
            {
                skillType = 0;
            }
            monsterMovement.SetAniSkill((int)skillType);
        }
    }
}