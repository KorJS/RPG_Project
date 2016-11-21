using UnityEngine;
using System.Collections;

public class MonsterRange : MonoBehaviour
{
    private MonsterInfoData monsterInfoData = null;
    private MonsterMovement monsterMovement = null;
    private MonsterState monsterState = null;
    public PlayerEffect playerEffect = null;

    [System.Serializable]
    public class MonsterSettings
    {
        [Header("- Object -")]
        public Transform    monsterT;           // 몬스터
        public Transform    targetT;            // 타겟
        public Transform    tempTargetT;        // 타겟
        public Vector3      originPos;          // 몬스터 스폰시 위치정보
        public Quaternion   originRot;          // 몬스터 스폰시 회전정보

        [Header("- Layer -")]
        public int          monsterLayer;       // 몬스터 레이어
        public int          targetLayer;        // 타겟 레이어

        [Header("- Rotation -")]
        public float        rotAngle;           // 전방 시야 (범위에 벗어나면 회전)

        [Header("- Other -")]
        public float        aggroRange;         // 몬스터의 애드 범위
        public float        aggroTime;          // 공격을 시작하는 시간, 어글이펙트가 표시 되는 시간
        public float        inactiveAggroDis;   // 어글이 풀리는 거리
        public float        moveDis;            // 일정거리를 벗어나면 상태(State) 이동으로 전환
        public float        atkDis;             // 공격시작 거리
    }

    [SerializeField]
    public MonsterSettings monster;

    private Transform skillPoint = null;
    private Vector3 skillRnage = Vector3.zero;

    public Vector3 mobPos = Vector3.zero;
    public Vector3 tPos = Vector3.zero;

    public Collider[] attTargets;

    // 어글 잡히는 시간을 체크하는 동안 범위안에 존재하면 범위를 벗어나도 타겟(어글)상태임
    public float aggroTimer = 0f; // 어글 타이머
    public bool isTargetAggro = false; // 어글이펙트가 활성화 되어있는지

    void Awake()
    {
        monsterInfoData = GetComponent<MonsterInfoData>();
        monsterMovement = GetComponent<MonsterMovement>();
        monsterState = GetComponent<MonsterState>();

        monster.monsterT = transform;
        

        monster.monsterLayer = LayerMask.NameToLayer("Monster");
        monster.targetLayer = LayerMask.NameToLayer("Player");

        skillPoint = monster.monsterT.FindChild("SkillPoint");
    }

    void Start()
    {
        monster.originPos = transform.position;
        monster.originRot = transform.rotation;
    }

    void Update()
    {
        // 다음에는 상속 꼭 쓰자..
        if (GameManager.Instance.gameState == TypeData.GameState.종료)
        {
            return;
        }

        if (monsterState.currentState == TypeData.MonsterState.죽음)
        {
            return;
        }

        if (monsterState.currentState != TypeData.MonsterState.회전)
        {
            if (monsterMovement.animator.applyRootMotion)
            {
                monsterMovement.animator.applyRootMotion = false;
            }
        }

        if (monsterState.currentState == TypeData.MonsterState.데미지)
        {
            return;
        }

        if (monster.targetT != null)
        {
            if (monster.targetT.GetComponent<PlayerState>().currentState == TypeData.State.죽음)
            {
                Reset();
            }
        }

        if (isTargetAggro)
        {
            if (!playerEffect)
            {
                Debug.Log("PlayerEffect Script Null");
            }
            playerEffect.CheckActiveEffect(TypeData.PlayerEffect.Aggro.ToString(), true);
        }

        SearchTarget();
        CheckAggro();
        CheckRotationRange();
        CheckMoveRange();
        CharckAttackRange();
        CheckOriginDistance();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (string.Compare(hit.collider.tag, "Safe") == 0)
        {
            Debug.Log("OnControllerColliderHit");
            Reset();
        }
    }

    void OnDrawGizmos()
    {
        if (skillPoint == null)
        {
            return;
        }
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Vector3 testPos = new Vector3(skillRnage.x * 2f + 0.1f, skillRnage.y * 2f + 0.1f, skillRnage.z * 2f + 0.1f);
        Gizmos.DrawCube(skillPoint.position, testPos);
    }

    private void Reset()
    {
        Debug.Log("리셋");

        if (playerEffect != null)
        {
            // TODO : 어글이펙트 비활성화
            playerEffect.CheckActiveEffect(TypeData.PlayerEffect.Aggro.ToString(), false);
        }
      
        monster.targetT = null;
        monster.tempTargetT = null;
        isTargetAggro = false;
        aggroTimer = 0f;
        if (!monsterMovement.nav.enabled)
        {
            monsterMovement.nav.enabled = true;
        }
        monsterMovement.nav.ResetPath();
        monsterInfoData.Reset(false);
        monsterState.nextState = TypeData.MonsterState.대기;
        monsterState.nextMode = TypeData.MODE.평화;
    }

    // 범위내의 타겟이 있는지 검색후 임시 타켓지정
    private void SearchTarget()
    {
        // 확정타겟이나 임시 타겟이 있으면 리턴
        if (monster.targetT || monster.tempTargetT)
        {
            return;
        }

        Vector3 monsterPos = monster.monsterT.position;

        // 자신의 위치에서 어글범위내에 있는 콜리더 검색
        Collider[] tempTargets = Physics.OverlapSphere(monsterPos, monster.aggroRange);

        foreach (var tempTarget in tempTargets)
        {
            // 주인공인 경우
            if (tempTarget.gameObject.layer == monster.targetLayer)
            {
                // 임시 타겟 지정
                monster.tempTargetT = tempTarget.transform;
            }
        }
    }

    // 타겟 확정. 어글이펙트 활성화
    private void CheckAggro()
    {
        // 어글이펙트 활성화 되어있거나 
        // 확정 타겟이 있거나
        // 임시 타겟이 없으면
        if (isTargetAggro || !monster.tempTargetT)
        {
            aggroTimer = 0f;
            return;
        }

        aggroTimer += Time.deltaTime;

        // 1. 지정된 시간이 되기 전에 맞은경우
        //    그 플레이어를 확정 타겟으로. 어글이펙트 활성화
        // 2. 지정된 시간동안 임시 타겟 외에 다른 플레이어한테 공격 받지 않으면
        //    임시 타겟을 확정 타겟으로. 어글이펙트 활성화

        Transform tempT = monster.tempTargetT;

        if (monsterMovement.isDamage)
        {
            tempT = monster.targetT;
        }
        
        if (monsterMovement.isDamage || aggroTimer >= monster.aggroTime)
        {
            playerEffect = tempT.GetComponent<PlayerEffect>();

            if (playerEffect != null)
            {
                playerEffect.CheckActiveEffect(TypeData.PlayerEffect.Aggro.ToString(), true);
            }

            monster.targetT = tempT; // 타겟 확정
            isTargetAggro = true; // 타겟 어그로
            aggroTimer = 0f;
            monsterState.nextMode = TypeData.MODE.전투;
            monsterState.nextState = TypeData.MonsterState.이동;
            monsterMovement.isDamage = false;

            return;
        }

        // 임시 타겟이 시간안에 범위에서 사라지면
        Vector3 monsterPos = monster.monsterT.position;
        Vector3 tempTargetPos = monster.tempTargetT.position;

        float dis = Vector3.Distance(tempTargetPos, monsterPos);

        if (dis > monster.aggroRange)
        {
            monster.tempTargetT = null;
            isTargetAggro = false;
            aggroTimer = 0f;
            if (!monsterMovement.nav.enabled)
            {
                monsterMovement.nav.enabled = true;
            }
            monsterMovement.nav.ResetPath();
            monsterState.nextMode = TypeData.MODE.평화;
        }
    }

    private void CheckRotationRange()
    {
        // 확정,임시 타겟이 없을경우 리턴
        if (!monster.targetT || !monster.tempTargetT)
        {
            return;
        }

        // 회전 중이면 리턴
        if (monsterState.currentState == TypeData.MonsterState.회전)
        {
            return;
        }

        // 대기 / 스킬 상태가 아닐때 리턴
        if (monsterState.currentState != TypeData.MonsterState.대기 && monsterState.currentState != TypeData.MonsterState.스킬)
        {
            return;
        }

        // 스킬중이면 리턴
        if (monsterMovement.isSkill || !monsterMovement.isSkillWait)
        {
            return;
        }

        Vector3 v1 = monster.monsterT.position - monster.targetT.position;
        Vector3 v2 = monster.monsterT.right;
        float dp = Vector3.Dot(v1, v2);

        v2 = monster.monsterT.forward;
        float angle = Vector3.Angle(v1, v2);

        //Debug.Log("회전 체크 : " + angle);
        // 지정한 angle 안에 없으면 리턴
        if (angle > monster.rotAngle)
        {
            return;
        }

        monsterState.nextState = TypeData.MonsterState.회전;
        monsterMovement.isRot = true;

        if (!monsterMovement.animator.applyRootMotion)
        {
            monsterMovement.animator.applyRootMotion = true;
        }

        // 왼쪽 회전
        if (dp > 0)
        {
            monsterMovement.animator.SetTrigger(monsterMovement.animationSettings.isLeftTurnTrigger);
        }
        // 오른쪽 회전
        else
        {
            monsterMovement.animator.SetTrigger(monsterMovement.animationSettings.isRightTurnTrigger);
        }
    }

    private void CheckMoveRange()
    {
        // 어글 상태가 아니거라 확정 타겟이 없을경우 리턴
        if (!isTargetAggro || !monster.targetT)
        {
            return;
        }

        // 전투 모드가 아니면 리턴
        if (monsterState.currentMode != TypeData.MODE.전투)
        {
            return;
        }

        if (monsterState.currentState == TypeData.MonsterState.회전)
        {
            return;
        }

        if (monsterState.currentState == TypeData.MonsterState.이동)
        {
            return;
        }

        // 스킬중이면 리턴
        if (monsterMovement.isSkill || !monsterMovement.isSkillWait)
        {
            return;
        }

        Vector3 monsterPos = monster.monsterT.position;
        Vector3 targetPos = monster.targetT.position;

        float dis = Vector3.Distance(monsterPos, targetPos);
        //Debug.Log("이동 거리 체크 : " + dis);
        // 10m 이상 떨어지면 이동
        if (dis >= monster.moveDis)
        {   
            monsterState.nextState = TypeData.MonsterState.이동;
        }
    }

    private void CharckAttackRange()
    {
        // 어글 상태가 아니거라 확정 타겟이 없을경우 리턴
        if (!isTargetAggro || !monster.targetT)
        {
            return;
        }

        // 전투 모드가 아니면 리턴
        if (monsterState.currentMode != TypeData.MODE.전투)
        {
            return;
        }

        if (monsterState.currentState == TypeData.MonsterState.회전)
        {
            return;
        }

        if (monsterState.currentState == TypeData.MonsterState.스킬)
        {
            return;
        }

        Vector3 monsterPos = monster.monsterT.position;
        Vector3 targetPos = monster.targetT.position;
       
        float dis = Vector3.Distance(monsterPos, targetPos);

        if (dis <= monster.atkDis)
        {
            monsterMovement.nav.stoppingDistance = monster.atkDis;
            monsterState.nextState = TypeData.MonsterState.스킬;
            monsterMovement.isSkill = true;
        }
    }

    // 스폰위치로부터 거리를 체크 - 지정된 거리가 되면 원위치로 돌아감 / 어글 풀림
    private void CheckOriginDistance()
    {
        // 확정 타겟이 없으면 어글 상태가 아니거나 리턴
        if (!isTargetAggro || !monster.targetT)
        {
            return;
        }

        // 평화 상태이면 리턴
        if (monsterState.currentMode == TypeData.MODE.평화)
        {
            return;
        }

        Vector3 monsterPos = monster.monsterT.position;

        // 어글이 풀리는 거리(평화모드로) - 스폰위치로부터 몬스터 현재 거리
        float peaceDis = Vector3.Distance(monsterPos, monster.originPos);

        if (peaceDis > monster.inactiveAggroDis)
        {
            Reset();
        }
    }

    // 공격범위 - 각 스킬별로 위치, 거리, 각도로 타겟 hit판정.
    public void HitRange(Vector3 skillPos, Vector3 _skillRnage, float skillAtt)
    {
        // 어글자랑 타겟은 무족건 1명은 있으므로.
        // 어글타겟이 없거나 / 타겟지정이 없으면
        if (!isTargetAggro || !monster.targetT)
        {
            return;
        }

        Vector3 monsterPos = monster.monsterT.position;

        skillPoint.localPosition = new Vector3(skillPos.x, 1f, skillPos.z);
        skillRnage = _skillRnage;

        Collider[] _attTargets = Physics.OverlapBox(skillPoint.position, skillRnage);

        foreach (Collider target in _attTargets)
        {
            // 주인공이 아닌경우
            if (target.gameObject.layer != monster.targetLayer)
            {
                continue;
            }

            Debug.Log("targetName : " + target.name + " Attack : " + skillAtt);
            // 주인공 Hit
            PlayerMovement playerMovement = target.GetComponent<PlayerMovement>();
            playerMovement.Damage(-skillAtt);
        }
    }
}