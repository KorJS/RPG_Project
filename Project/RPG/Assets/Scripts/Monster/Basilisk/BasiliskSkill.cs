using UnityEngine;
using System.Collections;

public class BasiliskSkill : MonoBehaviour
{
    private MonsterInfoData monsterInfoData = null; // 몬스터 정보
    private MonsterState    monsterState    = null; // 몬스터 상태
    private MonsterRange    monsterRange    = null; // 몬스터 범위
    private MonsterMovement monsterMovement = null; // 몬스터 동작

    public enum SkillType
    {
        없음 = -1,
        att01 = 0,
        att02,

        MAX
    }

    public SkillType skillType  = SkillType.없음; // 몬스터 스킬타입

    private Vector3 skillPos    = Vector3.zero; // 스킬 기준점
    private Vector3 skillRange  = Vector3.zero; // 스킬 반경
    private float   skillAtt    = 0f;           // 스킬 공격력

    void Awake()
    {
        monsterInfoData = GetComponent<MonsterInfoData>();
        monsterState    = GetComponent<MonsterState>();
        monsterRange    = GetComponent<MonsterRange>();
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

        // 몬스터 상태가 스킬중이면 코루틴 시작
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

                monsterMovement.SetAniSkill((int)skillType);
            }

            yield return new WaitForSeconds(2f);
        }
    }

    // 스킬 범위 (DB로부터 받은 x,y,z 문자열을 Vector3 로 변환)
    private Vector3 stringToVector3(string data)
    {
        Vector3 pos = Vector3.zero;

        string[] values = data.Split(',');

        pos.x = float.Parse(values[0]);
        pos.y = float.Parse(values[1]);
        pos.z = float.Parse(values[2]);

        return pos;
    }

    // 스킬 범위 스크립트로 스킬 정보 전달
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
}
