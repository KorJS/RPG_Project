using UnityEngine;
using System.Collections;

public class MonsterRange : MonoBehaviour
{
    private MonsterMovement monsterMovement = null;
    private MonsterState monsterState = null;
    private PlayerEffect playerEffect = null;

    [System.Serializable]
    public class MonsterSettings
    {
        [Header("- Object -")]
        public Transform    monsterT;           // 몬스터
        public Transform    targetT;            // 타겟
        public Vector3      originPos;          // 몬스터 스폰시 위치정보
        public Quaternion   originRot;          // 몬스터 스폰시 회전정보

        [Header("- Layer -")]
        public int          monsterLayer;       // 몬스터 레이어
        public int          targetLayer;        // 타겟 레이어

        [Header("- Rotation -")]
        public float        rotRangeMax;        // 전방 시야 Max(범위에 벗어나면 회전)
        public float        rotRangeMin;        // 전방 시야 Min
        public float        rotRangDis;         // 시야 거리

        [Header("- Other -")]
        public float        aggroRange;         // 몬스터의 애드 범위
        public float        aggroTime;          // 공격을 시작하는 시간, 어글이펙트가 표시 되는 시간
        public float        inactiveAggroDis;   // 어글이 풀리는 거리
        public float        moveDis;            // 일정거리를 벗어나면 상태(State) 이동으로 전환
        public float        atkDis;             // 공격시작 거리
    }

    [SerializeField]
    public MonsterSettings monster;

    public Vector3 mobPos = Vector3.zero;
    public Vector3 tPos = Vector3.zero;

    // 어글 잡히는 시간을 체크하는 동안 범위안에 존재하면 범위를 벗어나도 타겟(어글)상태임
    public float aggroTimer = 0f; // 어글 타이머
    public bool isTargetAggro = false; // 어글이펙트가 활성화 되어있는지

    void Awake()
    {
        monsterMovement = GetComponent<MonsterMovement>();
        monsterState = GetComponent<MonsterState>();

        monster.monsterT = transform;
        monster.originPos = transform.position;
        monster.originRot = transform.rotation;

        monster.monsterLayer = LayerMask.NameToLayer("Monster");
        monster.targetLayer = LayerMask.NameToLayer("Player");
    }

    void Update()
    {
        SearchTarget();
        CheckAggro();
        CheckOriginDistance();
        CheckForward();
    }

    // 스폰위치로부터 거리를 체크 - 지정된 거리가 되면 원위치로 돌아감 / 어글 풀림
    private void CheckOriginDistance()
    {
        if (!isTargetAggro || !monster.targetT)
        {
            return;
        }

        if (monsterState.currentMode == TypeData.MODE.평화)
        {
            return;
        }

        Vector3 monsterPos = monster.monsterT.position;
        Vector3 targetPos = monster.targetT.position;

        // 일정거리를 벗어나면 상태를 이동으로 - 타겟과 몬스터의 거리
        float moveDis = Vector3.Distance(targetPos, monsterPos);

        if (moveDis > monster.moveDis)
        {
            monsterState.nextState = TypeData.State.이동;
        }

        if (moveDis < monster.atkDis)
        {
            monsterState.nextState = TypeData.State.스킬;
        }

        // 어글이 풀리는 거리(평화모드로) - 스폰위치로부터 몬스터 현재 거리
        float peaceDis = Vector3.Distance(monsterPos, monster.originPos);

        if (peaceDis > monster.inactiveAggroDis)
        {
            // TODO : 어글이펙트 비활성화.
            if (!playerEffect)
            {
                Debug.Log("PlayerEffect Script Null");
            }
            playerEffect.CheckActiveEffect(TypeData.PlayerEffect.Aggro.ToString(), false);
            monster.targetT = null;
            isTargetAggro = false;
            monster.monsterT.position = monster.originPos;
            monster.monsterT.rotation = monster.originRot;
            monsterState.nextState = TypeData.State.대기;
            monsterState.nextMode = TypeData.MODE.평화;
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
                playerEffect = monster.targetT.GetComponent<PlayerEffect>();
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
            if (!playerEffect)
            {
                Debug.Log("PlayerEffect Script Null");
            }
            playerEffect.CheckActiveEffect(TypeData.PlayerEffect.Aggro.ToString(), true);

            isTargetAggro = true;
            aggroTimer = 0f;
            monsterState.nextMode = TypeData.MODE.전투;
            monsterState.nextState = TypeData.State.이동;

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

    public void CheckForward()
    {
        if (!isTargetAggro || !monster.targetT)
        {
            return;
        }

        Vector3 targetPos = monster.targetT.position;
        Vector3 monsterPos = monster.monsterT.position;

        targetPos.y = 1f;

        Vector3 disPos = targetPos - monster.monsterT.position;
        Vector3 monsterForward = monster.monsterT.forward;

        float angle = Vector3.Angle(monsterForward, disPos);
        float distance = Vector3.Distance(targetPos, monsterPos);

        //Debug.Log("angle : " + angle + " distance : " + distance);
        if (angle >= monster.rotRangeMax || (angle >= monster.rotRangeMin && distance > monster.rotRangDis))
        {
            // 스킬 상태일때
            if (monsterMovement.animator.GetInteger(monsterMovement.animationSettings.stateInt) 
                == (int)TypeData.State.스킬)
            {
                // 스킬이 끝나면
                if (monsterMovement.isIdle)
                {
                    monsterMovement.isRot = true;
                }
            }
            // 스킬 상태가 아닐때
            else
            {
                monsterMovement.isRot = true;
            }

            mobPos = monster.monsterT.position;
            tPos = monster.targetT.position;
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
                target.GetComponent<PlayerMovement>().isHit = true;
            }
        }
    }
}