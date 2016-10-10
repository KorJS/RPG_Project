using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class Test : MonoBehaviour
{
    [System.Serializable]
    public class MonsterSettings
    {
        public Transform monsterT;
        public Transform target;
        public LayerMask monsterLayer;
        public LayerMask targetLayer;

        public Vector3 originPos; // 스폰된 첫 위치

        public float aggroRange = 20f;
        public float aggroTime = 4f;
    }

    [SerializeField]
    public MonsterSettings monster;

    public Vector3 mobPos = Vector3.zero;
    public Vector3 targetPos = Vector3.zero;

    public float dis = 0f;
    public bool isRot = false;

    public float aggroTimer = 0f;
    public bool isTargetAggro = false; // 어글 잡히는 시간을 체크하는 동안 범위안에 존재하면 범위를 벗어나도 타겟(어글)상태임

    void Awake()
    {
        monster.monsterT = transform;
        monster.originPos = monster.monsterT.position;

        monster.monsterLayer = LayerMask.NameToLayer("Monster");
        monster.targetLayer = LayerMask.NameToLayer("Player");

    }

    void Update()
    {
        // 타겟(어글자)가 없으면 타겟을 찾는다.
        SearchTarget();
        
        // 타겟이 정해지면 그 타겟이 범위안에서 지정한 시간동안 있으면 타겟에게 어글표시
        CheckAggro();

        Range();

        if (isRot)
        {
            Rotation();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, monster.aggroRange);
    }

    // TODO : 타겟 찾기 0 ~ 3초 동안 범위안에 있을경우 타겟지정됨.
    private void SearchTarget()
    {
        Vector3 monsterPos = monster.monsterT.position;

        // 20m 범위 안에
        Collider[] colls = Physics.OverlapSphere(monsterPos, monster.aggroRange);

        foreach (var col in colls)
        {
            // 주인공 레이어이면
            if (col.gameObject.layer == monster.targetLayer)
            {
                // TODO : 나중에 2명이상일 경우 거리에 따라 타겟 지정되게 하자.
                // 타겟 지정
                monster.target = col.transform;
                return;
            }
        }
    }

    private void CheckAggro()
    {
        if (isTargetAggro || !monster.target)
        {
            return;
        }

        //TODO : 주인공에게 맞으면 바로 어글 표시.

        // 타겟에 어글이 표시 되는 타임. ()
        aggroTimer += Time.deltaTime;

        if (aggroTimer >= monster.aggroTime)
        {
            isTargetAggro = true;
            // TODO : target의 어글이펙트 활성화
            aggroTimer = 0f;

            return;
        }

        // 시간안에 범위를 벗어나면 타겟 null
        if (Vector3.Distance(monster.target.position, monster.monsterT.position) > monster.aggroRange)
        {
            monster.target = null;
            isTargetAggro = false;
            aggroTimer = 0f;
        }
    }

    private void Range()
    {
        // 어글타겟이 없거나 / 타겟지정이 없으면
        if (!isTargetAggro || !monster.target)
        {
            return;
        }

        Vector3 targetPos = monster.target.position;

        dis = Vector3.Distance(targetPos, monster.monsterT.position);
        //Debug.Log(dis);
        if (dis > 13f)
        {
            return;
        }

        targetPos.y = 1f;

        Vector3 enemyPos = targetPos - monster.monsterT.position;
        float angle = Vector3.Angle(transform.forward, enemyPos);

        // 공격 범위(각도) 90도
        if (angle <= 90f)
        {
            // 주인공 hit
            //Debug.Log("angle : " + angle);
        }

        // 주인공이 80도 이상 벗어나거나 
        // 50도 이상이면서 5m 초과 할시 
        // 주인공 방향으로 회전
        if (angle >= 80f || (angle >= 50f && dis > 5f))
        {
            CheckPosition();
        }
    }

    private void CheckPosition()
    {
        mobPos = transform.position;
        targetPos = monster.target.position;
        isRot = true;
    }

    private void Rotation()
    {
        Vector3 pos = targetPos - mobPos;
        Quaternion q = Quaternion.LookRotation(pos);
        transform.rotation = Quaternion.Lerp(transform.rotation, q, Time.deltaTime);

        if (transform.rotation == q)
        {
            isRot = false;
        }
    }
}