using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class NPCMovement : MonoBehaviour
{
    [System.Serializable]
    public class AnimationSettings
    {
        public string stateInt = "State";
    }

    [SerializeField]
    public AnimationSettings animationSettings;

    public CharacterController charCtrl = null;
    public Animator animator = null;

    public bool isPlayer = false;

    void Awake()
    {
        charCtrl = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // 상태
    public void SetAniState(TypeData.NPCState state)
    {
        animator.SetInteger(animationSettings.stateInt, (int)state);
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
