using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;

public class Network_MonsterInfo : MonoBehaviour
{
    public static Network_MonsterInfo network_MonsterInfo = null;
    public static Network_MonsterInfo Instance
    {
        get
        {
            if (network_MonsterInfo == null)
            {
                // MonoBehaviour에서는 new를 만들 수 없다.
                Debug.LogError("Singleton is Null");
            }
            return network_MonsterInfo;
        }
    }

    private class RecvLoadMonsterData
    {
        public int      area_type;  // 지역
        public int      timestamp;  // 작동 시간
        public string   message;    // 메시지
        public bool     isSuccess;  // 성공여부
        public List<MonsterData.MonsterInfo> monsterInfos = new List<MonsterData.MonsterInfo>();                // 몬스터 정보
        public List<MonsterData.MonsterSkillInfo> monsterSkillInfos = new List<MonsterData.MonsterSkillInfo>(); // 몬스터 스킬 정보
    }

    private string monster_load_contents = null; // 몬스터 로드 php파일명

    void Awake()
    {
        if (network_MonsterInfo == null)
        {
            network_MonsterInfo = this;
        }
        else if (network_MonsterInfo != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);

        monster_load_contents = "monster_load";
    }

    public void RequestLoadMonster(TypeData.AreaType area_type)
    {
        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("contents", monster_load_contents);
        sendData.Add("area_type", (int)area_type);

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplyLoadMonster));
    }

    public void ReplyLoadMonster(string json)
    {
        RecvLoadMonsterData data = JsonReader.Deserialize<RecvLoadMonsterData>(json);

        if (!data.isSuccess)
        {
            Debug.Log(data.message);
            return;
        }

        for (int i = 0; i < data.monsterInfos.Count; i++)
        {
            Debug.Log("fileName : " + data.monsterInfos[i].fileName);
            MonsterData.Instance.monsterInfos.Add(data.monsterInfos[i].fileName + "_Spawns", data.monsterInfos[i]);
        }

        MonsterData.Instance.monsterSkillInfos = data.monsterSkillInfos;

        Debug.Log(MonsterData.Instance.monsterSkillInfos.Count);
        Debug.Log(data.message);
    }
}
