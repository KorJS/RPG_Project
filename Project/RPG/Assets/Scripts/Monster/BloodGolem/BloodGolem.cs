using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BloodGolem : MonoBehaviour
{
    private MonsterMovement monsterMovement = null;
    private MonsterState monsterState = null;

    [System.Serializable]
    public class BloodGolemSettings
    {
        // TODO : 블러드골렘 공격력, 방어력, HP, 드랍 아이템 등 정보.
    }

    [SerializeField]
    public BloodGolemSettings bloodGolem;

    void Awake()
    {
        monsterMovement = GetComponent<MonsterMovement>();
        monsterState = GetComponent<MonsterState>();
    }
}