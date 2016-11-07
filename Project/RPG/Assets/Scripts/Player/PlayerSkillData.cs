using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 주인공이 배운 스킬
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

    // 주인공이 배운 스킬
    public List<int> skillInfos;

    public PlayerSkillData()
    {
        skillInfos = new List<int>();

        //skillInfos.Add(0);
        //skillInfos.Add(1);
    }

    public bool GetSkillData(int skillIndex)
    {
        // 스킬 정보가 없으면 리턴
        if (!skillInfos.Contains(skillIndex))
        {
            return false;
        }

        return true;
    }
}