using UnityEngine;
using System.Collections;

public class SpawnControl : MonoBehaviour
{
    private MonsterInfoData monsterInfoData = null;

    // TODO : 자식으로 있는 몬스터 리스폰 시간 체크
    private GameObject monsterObj = null;

    private const float NORMAL_TIME = 60f;
    private const float ELITE_TIME = 120f;
    private const float BOSS_TIME = 5f;

    private string spawnTag = null;
    private float respawnTime = 0f;
    public float respawnTimer = 0f;

    public bool isRespawn = false; // 몬스터가 죽으면 true가 되면서 리스폰 타이머 시작

    void Awake()
    {
        spawnTag = this.tag;
        SetRespawnTime();
    }

    void Update()
    {
        Respawn();
    }

    public void SetChild(GameObject child)
    {
        child.transform.SetParent(transform);
        monsterObj = child;
        monsterInfoData = monsterObj.GetComponent<MonsterInfoData>();
    }

    private void SetRespawnTime()
    {
        switch (spawnTag)
        {
            case "Normal":
                { respawnTime = NORMAL_TIME; }
                break;
            case "Elite":
                { respawnTime = ELITE_TIME; }
                break;
            case "Boss":
                { respawnTime = BOSS_TIME; }
                break;
        }
    }

    private void Respawn()
    {
        if (!isRespawn)
        {
            return;
        }

        respawnTimer += Time.deltaTime;

        // 리스폰 시간이 되면
        if (respawnTimer > respawnTime)
        {
            // 자식으로 있는 몬스터 활성화
            monsterObj.SetActive(true);
            respawnTimer = 0f;
            isRespawn = false;
        }
    }
}