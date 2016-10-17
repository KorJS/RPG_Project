using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterManager : MonoBehaviour
{
    // TODO : 지역으로 검색. 해당 지역에 몬스터 정보를 받아온다.
    [System.Serializable]
    public struct MonsterInfo
    {
        public string name;
        public int monsterType;
        public int defence;
        public float hp;
        public float exp;
        public int glod;
        public int equipmentIndex;
        public int consumableIndex;
        public int materialIndex;
    }

    [HideInInspector]
    public MonsterInfo monsterInfo;

    // TODO : 지역의 각 몬스터들의 스폰위치와 기본정보들 받아와 분배
    // 리스폰은 각 몬스터가. 풀. 활성화 비활성화.
    public Dictionary<TypeData.MonsterType, MonsterInfo> monsterInfos = null; // 몬스터정보

    public List<GameObject> monsterObjs = null; // 모든 몬스터 오브젝트 - 지역이 바뀌면 제거하기 위해서

    public List<Transform> normalSpawns = null; // 일반몬스터 스폰위치
    public List<Transform> eliteSpawns = null;  // 엘리트몬스터 스폰위치
    public List<Transform> bossSpawns = null;   // 보스몬스터 스폰위치

    void Awake()
    {
        monsterInfos = new Dictionary<TypeData.MonsterType, MonsterInfo>();

        Transform world = GameObject.Find("World").transform;

        FindSpawn(normalSpawns, world, "NormalSpawns");
        FindSpawn(eliteSpawns, world, "EliteSpawns");
        FindSpawn(bossSpawns, world, "BossSpawns");
        monsterInfo.name = "BloodGolem";
        monsterInfo.monsterType = (int)TypeData.MonsterType.보스;
        monsterInfo.hp = 1000;
        monsterInfos.Add((TypeData.MonsterType)monsterInfo.monsterType, monsterInfo);
        CreateMonster();
    }

    // 각 몬스터 타입에 맞는 스폰위치들 찾기.
    public void FindSpawn(List<Transform> spawns, Transform worldT, string strSpawn)
    {
        // 타입별 스폰위치 Transform
        Transform tempSpawn = worldT.FindChild(strSpawn);
        int count = tempSpawn.childCount; // 스폰위치 개수

        for (int i = 0; i < count; i++)
        {
            spawns.Add(tempSpawn.GetChild(i)); // 스폰 위치를 리스트에 저장
        }
    }

    // 현 지역에 몬스터 정보
    public void SetMonsterData(MonsterInfo _monsterInfo)
    {
        monsterInfo = _monsterInfo;
        monsterInfos.Add((TypeData.MonsterType)monsterInfo.monsterType, monsterInfo);
    }

    public void CreateMonster()
    {
        string monsterPath = null;
        List<Transform> monsterSpawns = new List<Transform>();

        foreach (KeyValuePair<TypeData.MonsterType, MonsterInfo> monsterInfo in monsterInfos)
        {
            switch (monsterInfo.Key)
            {
                case TypeData.MonsterType.일반:
                    {
                        monsterPath = "Monster/Normal/";
                        monsterSpawns = normalSpawns;
                    }
                    break;

                case TypeData.MonsterType.정예:
                    {
                        monsterPath = "Monster/Elite/";
                        monsterSpawns = eliteSpawns;
                    }
                    break;

                case TypeData.MonsterType.보스:
                    {
                        monsterPath = "Monster/Boss/";
                        monsterSpawns = bossSpawns;
                    }
                    break;

                default:
                    Debug.Log("몬스터 타입이 없음");
                    continue;
            }

            var resource = Resources.Load(monsterPath + monsterInfo.Value.name);
            
            for (int i = 0; i < monsterSpawns.Count; i++)
            {
                GameObject monsterObj = Instantiate(resource) as GameObject;

                // 몬스터가 생성이 되면 스폰지역에 자식설정
                monsterSpawns[i].GetComponent<SpawnControl>().SetChild(monsterObj);

                monsterObj.transform.localPosition = Vector3.zero;
                monsterObj.name = monsterInfo.Value.name; // 이름 (Clone) 제거

                MonsterInfoData infoData = monsterObj.GetComponent<MonsterInfoData>();
                infoData.monsterInfo = monsterInfo.Value; // 몬스터 정보
                infoData.SetCurrentHP(monsterInfo.Value.hp); // 몬스터 HP 설정

                monsterObjs.Add(monsterObj); // 몬스터 등록 - 지역이 바뀌면 제거를 위해서.
            }
        }
    }
}