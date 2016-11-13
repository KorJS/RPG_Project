using UnityEngine;
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
        combo01,
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
            if (monsterMovement.isSkillWait)
            {
                Debug.Log("스킬 Go");
                skillType++;
                if (skillType == SkillType.MAX)
                {
                    skillType = 0;
                }
                monsterMovement.SetAniSkill((int)skillType);
            }

            yield return null;
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

    // 양손공격
    public void Att01()
    {
        // 데미지
        Hit((int)SkillType.att01);

        // 이펙트
    }

    // 내려찍기
    public void Att02()
    {
        Hit((int)SkillType.att02);
    }

    // 썬더 스톰
    public void Att03()
    {
        Hit((int)SkillType.att03);
    }

    // 마력볼
    public void Att04()
    {
        Hit((int)SkillType.att04);
    }

    // 브레스
    public void Att05()
    {
        Hit((int)SkillType.att05);
    }
    
    // 연속 마력볼
    public void Combo01()
    {
        Hit((int)SkillType.combo01);

    }
}