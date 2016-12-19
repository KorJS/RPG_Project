using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterManager : MonoBehaviour
{
    private MonsterData.MonsterInfo             tempMonsterInfo;        // 임시 몬스터 정보를 담아둘 것
    private List<MonsterData.MonsterSkillInfo>  tempMonsterSkillInfos;  // 임시 몬스터 스킬 정보를 담아둘 것

    public List<GameObject>                     monsterObjs = null;     // 지역 내에 모든 몬스터 - 지역이 바뀌면 제거하기 위해서

    public List<Transform>                      spawns = null;          // 몬스터 스폰지역

    void Awake()
    {
        tempMonsterSkillInfos = new List<MonsterData.MonsterSkillInfo>();
        Transform spawnHolder = GameObject.Find("World").transform.FindChild("Spawns");

        FindSpawn(spawnHolder, spawns); // 스폰지역 찾기
        CreateMonster();
    }

    // 스폰 지역
    private void FindSpawn(Transform holder, List<Transform> _spwans)
    {
        int count = holder.childCount;

        for (int i = 0; i < count; i++)
        {
            _spwans.Add(holder.GetChild(i));
        }
    }

    // 몬스터 생성
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

        Debug.Log(tempMonsterInfo.fileName + " 몬스터 스폰지역 수 : " + monsterSpawns.Count);

        // 몬스터 리소스 생성
        var resource = Resources.Load("Monster/" + tempMonsterInfo.fileName);

        for (int i = 0; i < monsterSpawns.Count; i++)
        {
            GameObject monsterObj = Instantiate(resource) as GameObject;

            // 몬스터가 생성이 되면 스폰지역에 자식설정
            monsterSpawns[i].GetComponent<SpawnControl>().SetChild(monsterObj);

            monsterObj.layer = LayerMask.NameToLayer("Monster");
            monsterObj.name = tempMonsterInfo.fileName; // 이름 (Clone) 제거

            MonsterInfoData infoData = monsterObj.GetComponent<MonsterInfoData>();
            infoData.monsterInfo = tempMonsterInfo; // 몬스터 정보

            Debug.Log("monsterType : " + tempMonsterInfo.monsterType);

            MonsterData.Instance.GetMonsterSkillData(tempMonsterInfo.monsterIndex, ref infoData.monsterSkillInfos);

            Debug.Log("infoData.monsterSkillInfos : " + infoData.monsterSkillInfos.Count);

            infoData.SetCurrentHP(tempMonsterInfo.hp); // 몬스터 HP 설정
            infoData.CheckDropItem(); // 몬스터 드랍아이템 정보

            monsterObjs.Add(monsterObj); // 몬스터 등록 - 지역이 바뀌면 제거를 위해서.
        }

        resource = null;
    }
}