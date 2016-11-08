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
    }

    public void AddSkillData(int skillIndex)
    {
        // 스킬 정보가 없으면 리턴
        if (skillInfos.Contains(skillIndex))
        {
            Debug.Log("이미 배운 스킬");
            return;
        }

        skillInfos.Add(skillIndex);

        // 스킬 습득시 주인공 정보 저장
        Network_PlayerInfo.Instance.RequestSavePlayerInfo();
    }

    public bool GetSkillData(int skillIndex)
    {
        // 스킬 정보가 없으면 리턴
        if (!skillInfos.Contains(skillIndex))
        {
            Debug.Log(skillIndex + " : 배우지 않은 스킬");
            return false;
        }

        return true;
    }
}