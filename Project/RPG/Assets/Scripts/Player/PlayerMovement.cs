﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private PlayerInput playerInput = null;

    // 애니메이션 파라미터명 설정
    [System.Serializable]
    public class AnimationSettings
    {
        public string moveVFloat            = "MoveV";
        public string moveHFloat            = "MoveH";
        public string modeInt               = "Mode";
        public string stateInt              = "State";
        public string skillTpyeInt          = "SkillType";
        public string hitBool               = "isHit";
        public string isDamageTrigger       = "isDamage";
    }

    [SerializeField]
    public AnimationSettings animationSettings;

    public CharacterController characterController = null;
    public Animator animator = null;
    private Camera camera = null;
    private Vector3 rotation = Vector3.zero;

    public bool isHit = false;              // 공격해 맞혔으면 true - 각 스킬스크립트에서 전달 받자
    public bool isDamage = false;           // 공격에 맞았으면 true - 몬스터로부터 맞았으면.
    public bool isIdle = false;             // 스킬 끝나고 idle 상태
    public bool isEndSkillPoint = false;    // 스킬 서브상태머신 빠져왔는지.

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        characterController = this.GetComponent<CharacterController>();
        animator = this.GetComponent<Animator>();
        camera = Camera.main;

        SetAnimator();
    }

    void Update()
    {
        // 공격해 맞혔으면 true
        animator.SetBool(animationSettings.hitBool, isHit);

        if (isDamage)
        {
            animator.SetTrigger(animationSettings.isDamageTrigger);
            isDamage = false;
        }
        // 현재 애니메이션 상태 확인
        CheckCurrentAnimation();
    }

    // 모드
    public void AnimationMode(TypeData.MODE mode)
    {
        animator.SetInteger(animationSettings.modeInt, (int)mode);
    }

    // 상태
    public void AnimationState(TypeData.State state)
    {
        animator.SetInteger(animationSettings.stateInt, (int)state);
    }

    // 스킬
    public void AnimationSkill(int skillTpye)
    {
        isEndSkillPoint = false; // 스킬 서브상태머신 안에 있으므로 false
        isIdle = true; // 스킬 중일때 회전 막기.
        animator.SetInteger(animationSettings.skillTpyeInt, skillTpye);
    }

    // 케릭터 이동
    public void AnimationMove(float v, float h, bool isSkill)
    {
        // 스킬사용중에 입력신호가 스킬이 아닐때(이동신호이면)
        if (isIdle && !isSkill)
        {
            return;
        }

        PlayerState.Instance.nextState = TypeData.State.이동;

        animator.SetFloat(animationSettings.moveVFloat, v);
        animator.SetFloat(animationSettings.moveHFloat, h);
    }

    // 케릭터 방향(회전)
    public void Rotation(float v, float h, bool isSkill)
    {
        // 스킬사용중에 입력신호가 스킬이 아닐때(이동신호이면)
        if (isIdle && !isSkill)
        {
            return;
        }

        Vector3 forword = camera.transform.forward;
        forword.y = 0f;
        forword = forword.normalized;

        Vector3 right = camera.transform.right;
        right.y = 0f;
        right = right.normalized;

        // 카메라 방향에 따라 변하므로 방향 보정
        // TODO : 백터 공부하자.
        Vector3 dir = h * right + v * forword;

        if (dir != Vector3.zero)
        {
            rotation = dir.normalized;
            transform.rotation = Quaternion.LookRotation(rotation);
        }
    }

    private void CheckCurrentAnimation()
    {
        // 현재 실행 중인 애니메이터가 "Skill_End_Point" 인지
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Skill_End_Point"))
        {
            isEndSkillPoint = true;
        }

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
