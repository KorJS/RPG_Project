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
    public Dictionary<int, int> skillInfos;

    public PlayerSkillData()
    {
        skillInfos = new Dictionary<int, int>();

        skillInfos.Add(0, 0);
    }

    public void SetSkillData()
    {
        
    }

    public bool GetSkillData(int skillIndex)
    {
        // 스킬 정보가 없으면 리턴
        if (!skillInfos.ContainsKey(skillIndex))
        {
            return false;
        }


        return true;
    }
}