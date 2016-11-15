using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NavMeshAgent))]
public class MonsterMovement : MonoBehaviour
{
    private MonsterInfoData monsterInfoData = null;
    private MonsterRange monsterRange = null;
    private MonsterState monsterState = null;

    [System.Serializable]
    public class AnimationSettings
    {
        public string stateInt              = "State";
        public string skillTypeInt          = "SkillType";
        public string isDwonTrigger         = "isDwon";
        public string isDamageTrigger       = "isDamage";
        public string isDeathTrigger        = "isDeath";
        public string isLeftTurnTrigger     = "isLeftTurn";
        public string isRightTurnTrigger    = "isRightTurn";
    }

    [SerializeField]
    public AnimationSettings animationSettings;

    public Animator animator = null;
    public NavMeshAgent nav = null;

    public GameObject effectHolderObj = null;

    public bool isRot = false;
    public bool isSkill = false;
    public bool isSkillWait = false;
    public bool isDamage = false;

    void Awake()
    {
        monsterRange = GetComponent<MonsterRange>();
        monsterInfoData = GetComponent<MonsterInfoData>();
        monsterState = GetComponent<MonsterState>();
        animator = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        nav.enabled = false;

        effectHolderObj = transform.FindChild("EffectHolder").gameObject;

        isRot = false;
        isSkill = false;
        isSkillWait = true;

        SetAnimator();
    }

    void OnEnable()
    {
        isRot = false;
        isDamage = false;
        isSkill = false;
        isSkillWait = true;
    }

    void Update()
    {
        if (monsterState.currentState == TypeData.MonsterState.죽음)
        {
            return;
        }

        //if (!isRot)
        //{
        //    animator.applyRootMotion = isRot;
        //}

        Move();
        CheckCurrentAnimation();
    }

    public void RushDamage(Transform tempHoleder)
    {
        if (nav.enabled)
        {
            nav.enabled = false;
        }

        monsterState.nextState = TypeData.MonsterState.스턴;

        transform.SetParent(tempHoleder);
    }

    public void RushEnd()
    {
        monsterState.nextState = TypeData.MonsterState.대기;
        transform.SetParent(monsterInfoData.parentT);

        if (!nav.enabled)
        {
            nav.enabled = true;
        }
    }

    // 데미지
    public void SetDamage(Transform _targetT, float damage)
    {
        monsterState.nextMode = TypeData.MODE.전투;
        monsterInfoData.SetCurrentHP(damage);
        isDamage = true;

        // 어글 확정이 안되어있다면
        if (!monsterRange.isTargetAggro)
        {
            monsterRange.monster.targetT = _targetT;
        }
    }

    // 다운
    public void SetAniDown()
    {

    }

    // 죽음
    public IEnumerator Death(float destroyTime)
    {
        yield return new WaitForSeconds(destroyTime);
        monsterInfoData.Reset(true);
        monsterRange.playerEffect.CheckActiveEffect(TypeData.PlayerEffect.Aggro.ToString(), false);
        monsterRange.monster.targetT = null;
        monsterRange.isTargetAggro = false;
        monsterState.nextState = TypeData.MonsterState.대기;
        monsterState.nextMode = TypeData.MODE.평화;
        gameObject.SetActive(false);
    }

    // 상태
    public void SetAniState(TypeData.MonsterState state)
    {
        animator.SetInteger(animationSettings.stateInt, (int)state);
    }

    // 스킬
    public void SetAniSkill(int skillType)
    {
        animator.SetInteger(animationSettings.skillTypeInt, skillType);
        isSkillWait = false;
    }

    // 현재 애니메이션 상태
    private void CheckCurrentAnimation()
    {
        //// 현재 실행 중인 애니메이터가 "skill_wait" 인지
        //if (isRot && animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        //{
        //    // 스킬이 시전이 끝나면 회전 가능 하게.
        //    isRot = false;
        //    monsterState.nextState = TypeData.MonsterState.대기;
        //    Debug.Log("?");
        //}

        // 현재 실행 중인 애니메이터가 "skill_wait" 인지
        if (!isSkillWait && animator.GetCurrentAnimatorStateInfo(0).IsName("skill_wait"))
        {
            // 스킬이 시전이 끝나면 회전 가능 하게.
        }
    }

    public void RotationEnd()
    {
        isRot = false;
        monsterState.nextState = TypeData.MonsterState.대기;

        if (animator.applyRootMotion)
        {
            animator.applyRootMotion = false;
        }
    }

    // 스킬 대기중이면
    public void CheckAniSkillWait()
    {
        Debug.Log("스킬 대기중");
        isSkillWait = true;
    }

    private void Move()
    {
        if (!monsterRange.monster.targetT)
        {
            return;
        }

        if (monsterState.currentMode != TypeData.MODE.전투)
        {
            return;
        }

        if (monsterState.currentState != TypeData.MonsterState.이동)
        {
            return;
        }

        Vector3 targetPos = monsterRange.monster.targetT.position;

        if (!nav.enabled)
        {
            nav.enabled = true;
        }

        nav.SetDestination(targetPos);
        //nav.destination = targetPos;
    }

    // 자식에 아바타를 받아옴
    private void SetAnimator()
    {
        Animator[] animators = GetComponentsInChildren<Animator>();

        if (animators.Length > 0)
        {
            for (int i = 0; i < animators.Length; i++)
            {
                Animator anim = animators[i];
                Avatar av = anim.avatar;

                if (anim != animator)
                {
                    if (string.Compare(anim.transform.parent.name, effectHolderObj.name) == 0)
                    {
                        continue;
                    }

                    animator.avatar = av;
                    Destroy(anim);
                }
            }
        }
    }

    // 이동
    //private void Move()
    //{
    //    if (!isMove)
    //    {
    //        return;
    //    }

    //    transform.Translate(0f, 0f, Time.deltaTime * 2f);
    //}

    //private void Rotation()
    //{
    //    if (!isRot || !isIdle)
    //    {
    //        return;
    //    }

    //    Vector3 targetPos = monsterRange.tPos;
    //    Vector3 monsterPos = monsterRange.mobPos;

    //    Vector3 pos = targetPos - monsterPos;

    //    // 이동 중일때 회전
    //    Quaternion q = Quaternion.LookRotation(pos);
    //    transform.rotation = Quaternion.Lerp(transform.rotation, q, Time.deltaTime * 2f);

    //    rotationTimer += Time.deltaTime;
    //    if (rotationTimer > rotationTime)
    //    {
    //        isRot = false;
    //        rotationTimer = 0f;
    //    }
    //}
}