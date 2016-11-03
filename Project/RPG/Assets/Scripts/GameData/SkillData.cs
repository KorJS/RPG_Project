using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillData
{
    private static SkillData skillData = null;
    public static SkillData Instance
    {
        get
        {
            if (skillData == null)
            {
                skillData = new SkillData();
            }

            return skillData;
        }
    }

    // TODO : 데이터를 받아올때 각 스킬별로 스킬렙업시 상승 효과도 같이 받아와서 PlayerSkillData에서 처리
    // 케릭 직업에 맞게 데이터를 받아온다
    public struct SkillInfo
    {
        public int index;           // 인덱스
        public string name;         // 이름
        public string iconName;     // 아이콘 이름
        public string description;  // 스킬 설정
        public int level;           // 습득 필요레벨
        public float coolTime;      // 쿨타임
        public float attack;        // 공격력
        public float mp;            // 소모MP / 습득MP
        public float angle;         // 각도
        public float distance;      // 거리
    };

    public SkillInfo skillinfo;

    public Dictionary<int, SkillInfo> skillInfos = null;

    public SkillData()
    {
        skillInfos = new Dictionary<int, SkillInfo>();

        skillinfo.index = 0;
        skillinfo.name = "연속 공격";
        skillinfo.iconName = "Combo";
        skillinfo.description = "근접한 대상에게 타격을 입히고 일정량의 MP를 회복합니다. 스킬 버튼을 연속적으로 누르면 공격이 이어지면서 더 강한 타격을 입힐 수 있습니다."; // 형식 포맷 사용하자
        skillinfo.level = 1;
        skillinfo.coolTime = 1f;
        skillinfo.attack = 1f;
        skillinfo.mp = 5f;
        skillinfo.angle = 60f;
        skillinfo.distance = 3f;

        skillInfos.Add(skillinfo.index, skillinfo);

        skillinfo.index = 1;
        skillinfo.name = "방패막기";
        skillinfo.iconName = "Block";
        skillinfo.description = "방패를 들어 전방으로부터의 공격을 방어합니다. 방어 가능한 최대 피해량은 장비의 성능에 따라 달라집니다. 스킬 버튼을 누르고 있는 동안에만 효과가 발휘.";
        skillinfo.level = 1;
        skillinfo.coolTime = 8f;
        skillinfo.attack = 0f;
        skillinfo.mp = -6f;
        skillinfo.angle = 90f;
        skillinfo.distance = 5f;

        skillInfos.Add(skillinfo.index, skillinfo);

        skillinfo.index = 2;
        skillinfo.name = "긴급 회피";
        skillinfo.iconName = "Tumbling";
        skillinfo.description = "재빠르게 몸을 날려 적의 공격을 회피합니다.";
        skillinfo.level = 10;
        skillinfo.coolTime = 5f;
        skillinfo.attack = 0f;
        skillinfo.mp = -100f;
        skillinfo.angle = 0f;
        skillinfo.distance = 0f;

        skillInfos.Add(skillinfo.index, skillinfo);

        skillinfo.index = 3;
        skillinfo.name = "난폭한 돌진";
        skillinfo.iconName = "Rush";
        skillinfo.description = "전방으로 빠르게 돌진하여 부딪히는 적들을 밀고 나갑니다.";
        skillinfo.level = 20;
        skillinfo.coolTime = 10f;
        skillinfo.attack = 0.3f;
        skillinfo.mp = -50f;
        skillinfo.angle = 90f;
        skillinfo.distance = 2f;

        skillInfos.Add(skillinfo.index, skillinfo);

        skillinfo.index = 4;
        skillinfo.name = "압도";
        skillinfo.iconName = "Overpower";
        skillinfo.description = "전방을 향해 점프하여 검를 땅으로 내려찍어 공격을 가하고 다운 시킵니다.";
        skillinfo.level = 30;
        skillinfo.coolTime = 10f;
        skillinfo.attack = 5f;
        skillinfo.mp = -50f;
        skillinfo.angle = 90f;
        skillinfo.distance = 5f;

        skillInfos.Add(skillinfo.index, skillinfo);
    }

    // 주인공의 스킬 데이터 전부
    // TODO : PlayerTpye에 맞는 데이터만 가져옴 
    // 나중에 네트워크통신할때는 공격력만 보내고 받은사람이 계산 하면되니깐. 내꺼만 가져오자
    public void SetSkillData()
    {

    }
}