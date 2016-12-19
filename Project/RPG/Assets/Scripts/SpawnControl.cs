using UnityEngine;
using System.Collections;

public class SpawnControl : MonoBehaviour
{
    private GameObject  monsterObj      = null; // 몬스터

    private const float NORMAL_TIME     = 10f;  // 일반몬스터 스폰 시간
    private const float ELITE_TIME      = 20f;  // 정예몬스터 스폰 시간
    private const float BOSS_TIME       = 30f;  // 보스몬스터 스폰 시간

    private string      spawnTag        = null; // 스폰지역 태그(일반,정예,보스)
    public float        respawnTime     = 0f;   // 리스폰 타임
    public float        respawnTimer    = 0f;   // 리스폰 타이머

    public bool         isRespawn       = false; // 몬스터가 죽으면 true가 되면서 리스폰 타이머 시작

    void Awake()
    {
        spawnTag = this.tag;
        SetRespawnTime();
    }

    void Update()
    {
        Respawn();
    }

    // 자식 설정
    public void SetChild(GameObject child)
    {
        child.transform.SetParent(transform);
        monsterObj = child;
        child.transform.localPosition = Vector3.zero;
        child.transform.rotation = transform.rotation;
    }

    // 리스폰 타임 설정
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

    // 리스폰
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