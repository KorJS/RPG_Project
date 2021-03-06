﻿using UnityEngine;
using System.Collections;

public class DarkReaperSkill : MonoBehaviour
{
    private MonsterInfoData monsterInfoData = null;
    private MonsterState monsterState = null;
    private MonsterRange monsterRange = null;
    private MonsterMovement monsterMovement = null;

    public enum SkillType
    {
        없음 = -1,
        att01 = 0,
        att02, att03,

        MAX
    }

    public SkillType skillType = SkillType.없음;

    private Vector3 skillPos = Vector3.zero;
    private Vector3 skillRange = Vector3.zero;
    private float skillAtt = 0f;

    void Awake()
    {
        monsterInfoData = GetComponent<MonsterInfoData>();
        monsterState = GetComponent<MonsterState>();
        monsterRange = GetComponent<MonsterRange>();
        monsterMovement = GetComponent<MonsterMovement>();

        skillType = SkillType.없음;
    }

    void Update()
    {
        if (monsterState.currentState == TypeData.MonsterState.죽음)
        {
            return;
        }

        if (monsterState.currentState == TypeData.MonsterState.데미지)
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
            return;
        }

        if (monsterMovement.isSkill)
        {
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
        while (monsterState.currentState == TypeData.MonsterState.스킬)
        {
            if (monsterMovement.isSkillWait)
            {
                Debug.Log("스킬 Go");
                skillType++;

                if (skillType == SkillType.MAX)
                {
                    skillType = SkillType.att01;
                }

                if (skillType == SkillType.att03)
                {
                    monsterMovement.animator.applyRootMotion = true;
                }

                monsterMovement.SetAniSkill((int)skillType);
            }

            yield return new WaitForSeconds(2f);
        }
    }

    private Vector3 stringToVector3(string data)
    {
        Vector3 pos = Vector3.zero;

        string[] values = data.Split(',');

        pos.x = float.Parse(values[0]);
        pos.y = float.Parse(values[1]);
        pos.z = float.Parse(values[2]);

        return pos;
    }

    private void Hit(int index)
    {
        skillPos = stringToVector3(monsterInfoData.monsterSkillInfos[index].skillPoint);
        skillRange = stringToVector3(monsterInfoData.monsterSkillInfos[index].range);
        skillAtt = monsterInfoData.monsterSkillInfos[index].att;

        monsterRange.HitRange(skillPos, skillRange, skillAtt);
    }

    public void Att01()
    {
        // 데미지
        Hit((int)SkillType.att01);
    }

    public void Att02()
    {
        Hit((int)SkillType.att02);
    }

    public void Att03()
    {
        Hit((int)SkillType.att03);
    }

    public void Att03End()
    {
        monsterMovement.animator.applyRootMotion = false;
    }
}
