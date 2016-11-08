using UnityEngine;
using System.Collections;

public class SpawnControl : MonoBehaviour
{
    // TODO : 자식으로 있는 몬스터 리스폰 시간 체크
    private GameObject monsterObj = null;

    private const float NORMAL_TIME = 10f;
    private const float ELITE_TIME = 20f;
    private const float BOSS_TIME = 30f;

    private string spawnTag = null;
    public float respawnTime = 0f;
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
    }

    private void SetRespawnTime()
    {
        switch (spawnTag)
        {
            case "NormalSpawn":
                { respawnTime = NORMAL_TIME; }
                break;
            case "EliteSpawn":
                { respawnTime = ELITE_TIME; }
                break;
            case "BossSpawn":
                { respawnTime = BOSS_TIME; }
                break;
        }
    }

    private void Respawn()
    {
        if (monsterObj.activeSelf)
        {
            return;
        }

        respawnTimer += Time.deltaTime;

        // 리스폰 시간이 되면
        if (respawnTimer > respawnTime)
        {
            // 자식으로 있는 몬스터 활성화
            monsterObj.SetActive(true);
            monsterObj.layer = LayerMask.NameToLayer("Monster");
            respawnTimer = 0f;
            isRespawn = false;
        }
    }
}