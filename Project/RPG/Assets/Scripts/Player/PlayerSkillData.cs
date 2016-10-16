using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSkillData
{
    private static PlayerSkillData playerSkillData = null;
    public static PlayerSkillData Instance
    {
        get
        {
            if (playerSkillData == null)
            {
                playerSkillData = new PlayerSkillData();
            }

            return playerSkillData;
        }
    }

    public struct SkillData
    {
        public int index;       // 인덱스
        public string name;     // 이름
        public int level;       // 스킬레벨
        public int learnLevel;  // 습득 필요레벨
        public float cooldown;  // 쿨다운
        public float attack;    // 공격력
        public float mp;        // 소모MP / 습득MP
        public float angle;     // 각도
        public float distance;  // 거리
    };

    public SkillData skillData;

    public Dictionary<int, SkillData> skillDatas = null;

    public PlayerSkillData()
    {
        skillDatas = new Dictionary<int, SkillData>();

        skillData.index = 0;
        skillData.level = 0;
        skillData.learnLevel = 0;
        skillData.cooldown = 0f;
        skillData.attack = 10f;
        skillData.mp = 5f;
        skillData.angle = 60f;
        skillData.distance = 3.5f;

        skillDatas.Add(skillData.index, skillData);
    }

    public void SetSkillData(int index, SkillData skilldata)
    {
        // 데이터 변경이 있을시 ( 다음단계의 스킬을 배움 )
    }
}