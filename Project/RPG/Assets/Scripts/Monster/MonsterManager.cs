using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterManager : MonoBehaviour
{
    // TODO : 지역으로 검색. 해당 지역에 몬스터 정보를 받아온다.
    public struct MonsterInfo
    {

    }

    public MonsterInfo monsterInfo;

    // TODO : 지역의 각 몬스터들의 스폰위치와 기본정보들 받아와 분배
    // 리스폰은 각 몬스터가. 풀. 활성화 비활성화.
    public Dictionary<TypeData.MonsterType, MonsterInfo> monsterInfos = null; // 몬스터정보

    public List<Transform> normalSpawns = null; // 노멀몬스터 스폰위치
    public List<Transform> eliteSpawns = null;  // 엘리트몬스터 스폰위치
    public List<Transform> bossSpawns = null;   // 보스몬스터 스폰위치

    public Dictionary<string, Transform> spawns = null;

    void Awake()
    {
        Transform world = GameObject.Find("World").transform;

        FindSpawn(normalSpawns, world, "NormalSpawns");
        FindSpawn(eliteSpawns, world, "EliteSpawns");
        FindSpawn(bossSpawns, world, "BossSpawns");

        spawns = new Dictionary<string, Transform>();
    }

    // 각 몬스터 타입에 맞는 스폰위치들 찾기.
    public void FindSpawn(List<Transform> spawns, Transform worldT, string strSpawn)
    {
        Transform tempSpawn = worldT.FindChild(strSpawn);
        int count = tempSpawn.childCount;

        for (int i = 0; i < count; i++)
        {
            spawns.Add(tempSpawn.GetChild(i));
        }
    }
}