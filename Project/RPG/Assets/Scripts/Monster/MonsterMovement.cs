using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(MonsterState))]
public class MonsterMovement : MonoBehaviour
{
    [System.Serializable]
    public class AnimationSettings
    {
        public string stateInt          = "State";
        public string skillTypeInt      = "SkillType";
        public string isDwonTrigger     = "isDwon";
        public string isDamageTrigger   = "isDamage";
        public string isDeathTrigger    = "isDeath";
    }

    [SerializeField]
    public AnimationSettings animationSettings;

    public CharacterController charCtrl = null;
    public Animator animator = null;

    public bool isIdle = false;

    void Awake()
    {
        charCtrl = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        
        isIdle = false;

        SetAnimator();
    }

    void Update()
    {

    }

    // 데미지
    public void Damage()
    {

    }

    // 죽음
    IEnumerator Death(float destroyTime)
    {
        yield return new WaitForSeconds(destroyTime);
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
        isIdle = true;
    }

    // 이동
    public void SetAniMove()
    {

    }

    // 현재 애니메이션 상태
    private void CheckCurrentAnimation()
    {
        // 현재 실행 중인 애니메이터가 "Idle_Botton" 인지
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("idle_Botton"))
        {
            // 스킬이 시전이 끝나면 회전 가능 하게.
            isIdle = false;
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
                    animator.avatar = av;
                    Destroy(anim);
                }
            }
        }
    }
}