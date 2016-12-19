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
        public int      monsterIndex;   // 몬스터 인덱스
        public int      monsterType;    // 몬스터 타입
        public int      areaTpye;       // 지역타입
        public int      def;            // 방어력
        public float    hp;             // 체력
        public int      exp;            // 경험치
        public int      gold;           // 드랍 골드
        public string   itemType;       // 드랍 아이템타입
        public string   itemIndex;      // 드랍 아이템인덱스
        public string   quantity;       // 드랍 아이템수량
        public string   name;           // 몬스터 이름
        public string   fileName;       // 몬스터 리소스파일 이름
    }

    public MonsterInfo monsterInfo;

    [System.Serializable]
    public class MonsterSkillInfo
    {
        public int      monsterIndex;   // 몬스터 인덱스
        public int      skillIndex;     // 스킬 인덱스
        public string   skillPoint;     // 스킬 범위 기준점
        public int      att;            // 스킬 공격력
        public string   range;          // 스킬 범위
        public string   aniName;        // 스킬 애니매이션 이름
    }

    public MonsterSkillInfo monsterSkillInfo;

    public Dictionary<string, MonsterInfo>  monsterInfos        = null;
    public List<MonsterSkillInfo>           monsterSkillInfos   = null;

    public MonsterData()
    {
        monsterInfos = new Dictionary<string, MonsterInfo>();
    }

    // 몬스터 정보 설정
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

    // 해당 몬스터에 스킬정보 설정
    public bool GetMonsterSkillData(int monsterIndex, ref Dictionary<int, MonsterSkillInfo> _monsterSkillInfos)
    {
        if (monsterSkillInfos.Count <= 0)
        {
            Debug.Log("몬스터 스킬 정보가 없습니다.");
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
            Debug.Log("해당 몬스터의 스킬 정보가 없습니다");
        }

        return isExist;
    }

    public void DataClear()
    {
        monsterInfos.Clear();
        monsterSkillInfos.Clear();
    }
}