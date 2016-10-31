using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class MonsterMovement : MonoBehaviour
{
    private MonsterRange monsterRange = null;

    [System.Serializable]
    public class AnimationSettings
    {
        public string modeInt               = "Mode";
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

    //public CharacterController charCtrl = null;
    public Animator animator = null;

    public GameObject skillHolderObj = null;

    public float rotationTime = 1.5f;
    public float rotationTimer = 0f;

    public bool isIdle = false;
    public bool isMove = false;
    public bool isRot = false;
    public bool isPeace = false;

    void Awake()
    {
        monsterRange = GetComponent<MonsterRange>();
        //charCtrl = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        skillHolderObj = transform.FindChild("SkillHolder").gameObject;
        isIdle = true;

        SetAnimator();
    }

    void OnEnable()
    {
        isIdle = true;
    }

    void Update()
    {
        CheckCurrentAnimation();
        Move();
        Rotation();
    }

    // 데미지
    public void SetAniDamage()
    {

    }

    // 다운
    public void SetAniDown()
    {

    }

    // 죽음
    public IEnumerator Death(float destroyTime)
    {
        yield return new WaitForSeconds(destroyTime);
    }

    // 모드
    public void SetAniMode(TypeData.MODE mode)
    {
        animator.SetInteger(animationSettings.modeInt, (int)mode);
    }

    // 상태
    public void SetAniState(TypeData.State state)
    {
        animator.SetInteger(animationSettings.stateInt, (int)state);
    }

    // 스킬
    public void SetAniSkill(int skillType)
    {
        animator.SetInteger(animationSettings.skillTypeInt, skillType);
        isIdle = false;
    }

    // 현재 애니메이션 상태
    private void CheckCurrentAnimation()
    {
        // 현재 실행 중인 애니메이터가 "Idle_Botton" 인지
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("walk"))
        {
            isMove = true;
        }
        else
        {
            isMove = false;
        }

        // 현재 실행 중인 애니메이터가 "Idle_Botton" 인지
        if (!isIdle && animator.GetCurrentAnimatorStateInfo(0).IsName("idle_wait_Bottom"))
        {
            // 스킬이 시전이 끝나면 회전 가능 하게.
            isIdle = true;
        }
    }

    // 이동
    private void Move()
    {
        if (!isMove)
        {
            return;
        }

        transform.Translate(0f, 0f, Time.deltaTime * 2f);
    }

    private void Rotation()
    {
        if (!isRot || !isIdle)
        {
            return;
        }

        Vector3 targetPos = monsterRange.tPos;
        Vector3 monsterPos = monsterRange.mobPos;

        Vector3 pos = targetPos - monsterPos;

        // 이동 중일때 회전
        Quaternion q = Quaternion.LookRotation(pos);
        transform.rotation = Quaternion.Lerp(transform.rotation, q, Time.deltaTime * 2f);

        rotationTimer += Time.deltaTime;
        if (rotationTimer > rotationTime)
        {
            isRot = false;
            rotationTimer = 0f;
        }
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
                    if (string.Compare(anim.transform.parent.name, skillHolderObj.name) == 0)
                    {
                        continue;
                    }

                    animator.avatar = av;
                    Destroy(anim);
                }
            }
        }
    }
}