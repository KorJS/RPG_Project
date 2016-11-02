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
    public struct MonsterInfo
    {
        public string name;
        public string fileName;
        public int monsterType;
        public int defence;
        public float hp;
        public float exp;
        public int glod;
        public int itemType;
        public int itemIndex;
    }

    public MonsterInfo monsterInfo;

    public Dictionary<string, MonsterInfo> monsterInfos = null;

    public MonsterData()
    {
        monsterInfos = new Dictionary<string, MonsterInfo>();

        monsterInfo.name = "블러드 골렘";
        monsterInfo.fileName = "BloodGolem";
        monsterInfo.monsterType = (int)TypeData.MonsterType.보스;
        monsterInfo.defence = 30;
        monsterInfo.hp = 10000;
        monsterInfos.Add(monsterInfo.fileName + "_Spawns", monsterInfo);
    }

    // 지역으로 검색해서 받아온 몬스터 정보
    public void SetMonsterData()
    {

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
}