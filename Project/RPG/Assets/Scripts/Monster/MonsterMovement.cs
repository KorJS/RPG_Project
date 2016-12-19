using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
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

    public CharacterController  charCtrl    = null; 
    public Animator             animator    = null; 
    public NavMeshAgent         nav         = null; 

    public GameObject           effectHolderObj = null; // 이펙트 홀더

    public bool                 isRot       = false;    // 회전 여부
    public bool                 isSkill     = false;    // 스킬 여부
    public bool                 isSkillWait = false;    // 스킬 대기중 여부
    public bool                 isDamage    = false;    // 데미지 맞았는지 여부

    void Awake()
    {
        monsterRange    = GetComponent<MonsterRange>();
        monsterInfoData = GetComponent<MonsterInfoData>();
        monsterState    = GetComponent<MonsterState>();
        charCtrl        = GetComponent<CharacterController>();
        animator        = GetComponent<Animator>();
        nav             = GetComponent<NavMeshAgent>();
        nav.enabled     = false;

        effectHolderObj = transform.FindChild("EffectHolder").gameObject;

        isRot           = false;
        isSkill         = false;
        isSkillWait     = true;

        SetAnimator();  // 몬스터 애니매이션 설정(자식에있는)
    }

    void OnEnable()
    {
        monsterState.nextState  = TypeData.MonsterState.대기;
        monsterState.nextMode   = TypeData.MODE.평화;
        charCtrl.enabled        = true;
        isRot                   = false;
        isDamage                = false;
        isSkill                 = false;
        isSkillWait             = true;
    }

    void Update()
    {
        if (monsterState.currentState == TypeData.MonsterState.죽음)
        {
            return;
        }

        Move();
    }

    public void RushDamage(Transform tempHoleder)
    {
        if (nav.enabled)
        {
            nav.enabled = false;
        }

        monsterState.nextState = TypeData.MonsterState.데미지;
        animator.SetTrigger(animationSettings.isDamageTrigger);
        transform.SetParent(tempHoleder); // 난폭한 돌진 쓴 대상에 부모로 설정
        charCtrl.enabled = false; // 캐릭터 컨트롤러 비활성화
    }

    public void RushEnd()
    {
        monsterState.nextMode = TypeData.MODE.전투;
        monsterState.nextState = TypeData.MonsterState.이동;
        transform.SetParent(monsterInfoData.parentT); // 원래 부모로 재설정
        charCtrl.enabled = true; // 캐릭터 컨트롤러 활성화

        if (!nav.enabled)
        {
            nav.enabled = true; // 네비 활성화
        }
    }

    // 데미지
    public void SetDamage(Transform _targetT, float damage)
    {
        UIManager.Instance.SetDamageTxt(transform, damage, Color.white);
        UIManager.Instance.SetHpBar(transform);
        monsterState.nextMode = TypeData.MODE.전투;
        monsterInfoData.SetCurrentHP(damage);
        isDamage = true;

        // 어글 확정이 안되어있다면
        if (!monsterRange.isTargetAggro)
        {
            monsterRange.monster.targetT = _targetT;
        }
    }

    // 죽음
    public IEnumerator Death(float destroyTime)
    {
        charCtrl.enabled = false;

        yield return new WaitForSeconds(destroyTime);

        monsterInfoData.Reset(true);

        // 죽었을시 타겟에 어그로 이펙트 비활성화
        if (monsterRange.playerEffect != null)
        {
            monsterRange.playerEffect.CheckActiveEffect(monsterRange.playerEffect.effectSettings.aggro, false);
        }
        

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

    // 회전 끝(애니메이션 이벤트로 호출)
    public void RotationEnd()
    {
        isRot = false;
        monsterState.nextState = TypeData.MonsterState.대기;

        if (animator.applyRootMotion)
        {
            animator.applyRootMotion = false;
        }
    }

    // 스킬 대기중이면(애니메이션 이벤트로 호출)
    public void CheckAniSkillWait()
    {
        Debug.Log("스킬 대기중");
        isSkillWait = true;
    }

    // 몬스터 이동(내비로 타겟 위치 설정)
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
                    // 이펙트에 있는 애니매이터는 제외
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