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
    public struct Skillinfo
    {
        public int index;       // 인덱스
        public string name;     // 이름
        public int level;       // 습득 필요레벨
        public float coolTime;  // 쿨타임
        public float attack;    // 공격력
        public float mp;        // 소모MP / 습득MP
        public float angle;     // 각도
        public float distance;  // 거리
    };

    public Skillinfo skillinfo;

    public Dictionary<int, Skillinfo> skillInfos = null;

    public SkillData()
    {
        skillInfos = new Dictionary<int, Skillinfo>();

        skillinfo.index = 0;
        skillinfo.name = "Combo";
        skillinfo.level = 1;
        skillinfo.coolTime = 1f;
        skillinfo.attack = 100f;
        skillinfo.mp = 5f;
        skillinfo.angle = 60f;
        skillinfo.distance = 3f;

        skillInfos.Add(skillinfo.index, skillinfo);

        skillinfo.index = 1;
        skillinfo.name = "Overpower";
        skillinfo.level = 5;
        skillinfo.coolTime = 8f;
        skillinfo.attack = 500f;
        skillinfo.mp = -100f;
        skillinfo.angle = 60f;
        skillinfo.distance = 5f;

        skillInfos.Add(skillinfo.index, skillinfo);

        skillinfo.index = 2;
        skillinfo.name = "Rush";
        skillinfo.level = 8;
        skillinfo.coolTime = 10f;
        skillinfo.attack = 0f;
        skillinfo.mp = -50f;
        skillinfo.angle = 90f;
        skillinfo.distance = 2f;

        skillInfos.Add(skillinfo.index, skillinfo);
    }

    // 주인공의 스킬 데이터 전부
    // TODO : PlayerTpye에 맞는 데이터만 가져옴 
    // 나중에 네트워크통신할때는 공격력만 보내고 받은사람이 계산 하면되니깐. 내꺼만 가져오자
    public void SetSkillData()
    {

    }
}