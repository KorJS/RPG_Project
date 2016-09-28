using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SKillData
{
    private static SKillData skillData = null;
    public static SKillData Instance
    {
        get
        {
            if (skillData == null)
            {
                skillData = new SKillData();
            }

            return skillData;
        }
    }

    public struct SkillInfo
    {
        public int index;       // 인덱스
        public string name;     // 이름
        public int level;       // 습득 필요레벨
        public float cooldown;  // 쿨다운
        public float attack;    // 공격력
        public float mp;        // 소모MP

    };

    public SkillInfo skillinfo;

    Dictionary<int, SkillInfo> skillinfos = null;
    Dictionary<TypeData.PlayerType, Dictionary<int, SkillInfo>> typeSkillInfos = null;

    public SKillData()
    {
        skillinfos = new Dictionary<int, SkillInfo>();
        typeSkillInfos = new Dictionary<TypeData.PlayerType, Dictionary<int, SkillInfo>>();

        skillinfo.index = 110;
        skillinfos.Add(skillinfo.index, skillinfo);

        typeSkillInfos.Add(TypeData.PlayerType.기사, skillinfos);
    }

    public Dictionary<int, SkillInfo> CheckPlayerType(TypeData.PlayerType playerType)
    {
        Dictionary<int, SkillInfo> temp = typeSkillInfos[playerType];

        return temp;
    }
}