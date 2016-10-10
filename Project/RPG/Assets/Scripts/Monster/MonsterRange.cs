using UnityEngine;
using System.Collections;

public class MonsterRange : MonoBehaviour
{
    private MonsterState monsterState = null;

    [System.Serializable]
    public class MonsterSettings
    {
        public Transform    monsterT;           // 몬스터
        public Transform    targetT;            // 타겟
        public LayerMask    monsterLayer;       // 몬스터 레이어
        public LayerMask    targetLayer;        // 타겟 레이어

        public Vector3      originPos;          // 몬스터 스폰 위치

        public float        aggroRange;         // 몬스터의 애드 범위
        public float        aggroTime;          // 공격을 시작하는 시간, 어글이펙트가 표시 되는 시간
        public float        inactiveAggroDis;   // 어글이 풀리는 거리
    }

    [SerializeField]
    public MonsterSettings monster;

    // 어글 잡히는 시간을 체크하는 동안 범위안에 존재하면 범위를 벗어나도 타겟(어글)상태임
    public float aggroTimer = 0f; // 어글 타이머
    public bool isTargetAggro = false; // 어글이펙트가 활성화 되어있는지

    void Awake()
    {
        monsterState = GetComponent<MonsterState>();

        monster.monsterT = transform;
        monster.originPos = transform.position;

        monster.monsterLayer = LayerMask.NameToLayer("Monster");
        monster.targetLayer = LayerMask.NameToLayer("Player");
    }

    void Update()
    {
        SearchTarget();
        CheckAggro();
        CheckOriginDistance();
    }

    // 스폰위치로부터 거리를 체크 - 지정된 거리가 되면 원위치로 돌아감 / 어글 풀림
    private void CheckOriginDistance()
    {
        if (!isTargetAggro || !monster.targetT)
        {
            return;
        }

        Vector3 monsterPos = monster.monsterT.position;
        Vector3 targetPos = monster.targetT.position;

        float distance = Vector3.Distance(targetPos, monsterPos);

        if (distance > monster.inactiveAggroDis)
        {
            monster.targetT = null;
            isTargetAggro = false;
            // TODO : 어글이펙트 비활성화.
        }
    }

    // 범위내의 타겟이 있는지 검색후 임시 타켓지정
    private void SearchTarget()
    {
        // 타겟이 있으면 리턴
        if (monster.targetT)
        {
            return;
        }

        Vector3 monsterPos = monster.monsterT.position;

        // 자신의 위치에서 어글범위내에 있는 콜리더 검색
        Collider[] targets = Physics.OverlapSphere(monsterPos, monster.aggroRange);

        foreach (var target in targets)
        {
            // 주인공인 경우
            if (target.gameObject.layer == monster.targetLayer)
            {
                // TODO : 2명이상일 경우 거리에 따라 타겟 지정.
                // 타겟 지정
                monster.targetT = target.transform;
            }
        }
    }

    // 타겟 확정. 어글이펙트 활성화
    private void CheckAggro()
    {
        // 어글이펙트 활성화 되어있거나 타겟이 없으면 
        if (isTargetAggro || !monster.targetT)
        {
            aggroTimer = 0f;
            return;
        }

        aggroTimer += Time.deltaTime;

        // 타겟에 어글이 표시 되는 타임. ()
        if (aggroTimer >= monster.aggroTime)
        {
            isTargetAggro = true;
            aggroTimer = 0f;
            monsterState.nextMode = TypeData.MODE.전투;

            return;
        }

        Vector3 monsterPos = monster.monsterT.position;
        Vector3 targetPos = monster.targetT.position;

        // 시간안에 범위를 벗어나면 타겟 null
        if (Vector3.Distance(targetPos, monsterPos) > monster.aggroRange)
        {
            monster.targetT = null;
            isTargetAggro = false;
            aggroTimer = 0f;
            monsterState.nextMode = TypeData.MODE.평화;
        }
    }

    // 공격범위 - 각 스킬별로 위치, 거리, 각도로 타겟 hit판정.
    public void HitRange(Vector3 skillPos, float skillDistance, float skillAngle)
    {
        // 어글자랑 타겟은 무족건 1명은 있으므로.
        // 어글타겟이 없거나 / 타겟지정이 없으면
        if (!isTargetAggro || !monster.targetT)
        {
            return;
        }

        Vector3 monsterPos = monster.monsterT.position;
        Vector3 targetPos2 = monster.targetT.position;

        Collider[] targets = Physics.OverlapSphere(skillPos, skillDistance);

        foreach (var target in targets)
        {
            // 주인공이 아닌경우
            if (target.gameObject.layer != monster.targetLayer)
            {
                return;
            }

            Vector3 targetPos = target.transform.position;

            float distance = Vector3.Distance(targetPos, skillPos); // 타겟과 스킬 거리

            // 스킬 거리(범위)에서 벗어나면 hit대상 제외
            if (distance > skillDistance)
            {
                continue;
            }

            targetPos.y = 1f;

            Vector3 disPos = targetPos - monsterPos; // 타겟과의 몬스터 방향
            Vector3 forward = monster.monsterT.forward; // 몬스터의 전방 forward

            float angle = Vector3.Angle(forward, disPos); // 각도

            // skill범위 각도에 들어오는 경우.
            if (angle <= skillAngle)
            {
                // 주인공 Hit
            }
        }
    }
}