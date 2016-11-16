using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TODO : 지역정보로 몬스터 정보를 받음 > 타입별로 분류 > 이름으로 스폰지역 결정( 지역이 여러개라면..)
public class MonsterData
{
    private static MonsterData monsterData = null;
    public static MonsterData Instance
    {
        get
        {
            if (monsterData == null)
            {
                monsterData = new MonsterData();
            }

            return monsterData;
        }
    }

    // TODO : 지역으로 검색. 해당 지역에 몬스터 정보를 받아온다.
    [System.Serializable]
    public class MonsterInfo
    {
        public int monsterIndex;
        public int monsterType;
        public int areaTpye;
        public int def;
        public float hp;
        public int exp;
        public int glod;
        public string itemType;
        public string itemIndex;
        public string quantity;
        public string name;
        public string fileName;
    }

    public MonsterInfo monsterInfo;

    [System.Serializable]
    public class MonsterSkillInfo
    {
        public int monsterIndex;
        public int skillIndex;
        public string skillPoint;
        public int att;
        public string range;
        public string aniName;
    }

    public MonsterSkillInfo monsterSkillInfo;

    public Dictionary<string, MonsterInfo> monsterInfos = null;
    public List<MonsterSkillInfo> monsterSkillInfos = null;

    public MonsterData()
    {
        monsterInfos = new Dictionary<string, MonsterInfo>();
    }

    public bool GetMonsterData(string spawnName, ref MonsterInfo monsterInfo)
    {
        // 해당 몬스터 정보가 없다면 리턴
        if (!monsterInfos.ContainsKey(spawnName))
        {
            return false;
        }

        monsterInfo = monsterInfos[spawnName];

        return true;
    }

    public bool GetMonsterSkillData(int monsterIndex, ref Dictionary<int, MonsterSkillInfo> _monsterSkillInfos)
    {
        if (monsterSkillInfos.Count <= 0)
        {
            Debug.Log("보스 스킬 정보가 없습니다.");
            return false;
        }

        bool isExist = false;

        for (int i = 0; i < monsterSkillInfos.Count; i++)
        {
            if (monsterIndex == monsterSkillInfos[i].monsterIndex)
            {
                _monsterSkillInfos.Add(monsterSkillInfos[i].skillIndex, monsterSkillInfos[i]);
                isExist = true;
            }
        }

        if (!isExist)
        {
            Debug.Log("해당 보스의 스킬 정보가 없습니다");
        }

        return isExist;
    }

    public void DataClear()
    {
        monsterInfos.Clear();
        monsterSkillInfos.Clear();
    }
}