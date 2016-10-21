using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterManager : MonoBehaviour
{
    private MonsterData.MonsterInfo tempMonsterInfo; // 임시 몬스터 정보를 담아둘 것

    public List<GameObject> monsterObjs = null; // 지역 내에 모든 몬스터 - 지역이 바뀌면 제거하기 위해서

    public List<Transform> spawns = null;

    void Awake()
    {
        Transform spawnHolder = GameObject.Find("World").transform.FindChild("Spawns");

        FindSpawn(spawnHolder, spawns);
        CreateMonster();
    }

    private void FindSpawn(Transform holder, List<Transform> _spwans)
    {
        int count = holder.childCount;

        for (int i = 0; i < count; i++)
        {
            _spwans.Add(holder.GetChild(i));
        }
    }

    public void CreateMonster()
    {
        for (int i = 0; i < spawns.Count; i++)
        {
            Transform tempSpawn = spawns[i];

            // 몬스터 정보를 찾는다
            if (!MonsterData.Instance.GetMonsterData(tempSpawn.name, ref tempMonsterInfo))
            {
                Debug.Log(tempSpawn.name + " 몬스터 정보가 없습니다. MonsterData 확인바람");
                continue;
            }

            // 해당 몬스터의 스폰 지역에 배치
            SetMonsterSpawn(tempSpawn);
        }
    }

    // 몬스터 스폰지역 설정
    private void SetMonsterSpawn(Transform tempSpawn)
    {
        List<Transform> monsterSpawns = new List<Transform>();

        FindSpawn(tempSpawn, monsterSpawns);

        Debug.Log(tempMonsterInfo.name + " 몬스터 스폰지역 수 : " + monsterSpawns.Count);

        // 몬스터 리소스 생성
        var resource = Resources.Load("Monster/" + tempMonsterInfo.name);

        for (int i = 0; i < monsterSpawns.Count; i++)
        {
            GameObject monsterObj = Instantiate(resource) as GameObject;

            // 몬스터가 생성이 되면 스폰지역에 자식설정
            monsterSpawns[i].GetComponent<SpawnControl>().SetChild(monsterObj);

            monsterObj.transform.localPosition = Vector3.zero;
            monsterObj.name = tempMonsterInfo.name; // 이름 (Clone) 제거

            MonsterInfoData infoData = monsterObj.GetComponent<MonsterInfoData>();
            infoData.monsterInfo = tempMonsterInfo; // 몬스터 정보
            infoData.SetCurrentHP(tempMonsterInfo.hp); // 몬스터 HP 설정

            monsterObjs.Add(monsterObj); // 몬스터 등록 - 지역이 바뀌면 제거를 위해서.
        }

        resource = null;
    }

    //// 각 몬스터 타입에 맞는 스폰위치들 찾기.
    //public void FindSpawn(List<Transform> spawns, Transform worldT, string strSpawn)
    //{
    //    // 타입별 스폰위치 Transform
    //    Transform tempSpawn = worldT.FindChild(strSpawn);
    //    int count = tempSpawn.childCount; // 스폰위치 개수

    //    for (int i = 0; i < count; i++)
    //    {
    //        spawns.Add(tempSpawn.GetChild(i)); // 스폰 위치를 리스트에 저장
    //    }
    //}

    //public void CreateMonster()
    //{
    //    string monsterPath = null;
    //    List<Transform> monsterSpawns = new List<Transform>();

    //    foreach (KeyValuePair<string, MonsterData.MonsterInfo> monsterInfo in MonsterData.Instance.monsterInfos)
    //    {
    //        switch (monsterInfo.Key)
    //        {
    //            case TypeData.MonsterType.일반:
    //                {
    //                    monsterPath = "Monster/Normal/";
    //                    monsterSpawns = normalSpawns;
    //                }
    //                break;

    //            case TypeData.MonsterType.정예:
    //                {
    //                    monsterPath = "Monster/Elite/";
    //                    monsterSpawns = eliteSpawns;
    //                }
    //                break;

    //            case TypeData.MonsterType.보스:
    //                {
    //                    monsterPath = "Monster/Boss/";
    //                    monsterSpawns = bossSpawns;
    //                }
    //                break;

    //            default:
    //                Debug.Log("몬스터 타입이 없음");
    //                continue;
    //        }

    //        var resource = Resources.Load(monsterPath + monsterInfo.Value.name);

    //        for (int i = 0; i < monsterSpawns.Count; i++)
    //        {
    //            GameObject monsterObj = Instantiate(resource) as GameObject;

    //            // 몬스터가 생성이 되면 스폰지역에 자식설정
    //            monsterSpawns[i].GetComponent<SpawnControl>().SetChild(monsterObj);

    //            monsterObj.transform.localPosition = Vector3.zero;
    //            monsterObj.name = monsterInfo.Value.name; // 이름 (Clone) 제거

    //            MonsterInfoData infoData = monsterObj.GetComponent<MonsterInfoData>();
    //            infoData.monsterInfo = monsterInfo.Value; // 몬스터 정보
    //            infoData.SetCurrentHP(monsterInfo.Value.hp); // 몬스터 HP 설정

    //            monsterObjs.Add(monsterObj); // 몬스터 등록 - 지역이 바뀌면 제거를 위해서.
    //        }
    //    }
    //}
}