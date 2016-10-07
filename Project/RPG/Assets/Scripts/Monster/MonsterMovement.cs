using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class MonsterMovement : MonoBehaviour
{
    [System.Serializable]
    public class AnimationSettings
    {
        public string stateInt          = "State";
        public string skillTypeInt      = "SkillType";
    }

    [SerializeField]
    public AnimationSettings animationSettings;
}