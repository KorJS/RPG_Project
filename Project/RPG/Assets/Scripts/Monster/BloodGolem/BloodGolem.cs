using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BloodGolem : MonoBehaviour
{
    private MonsterMovement monsterMovement = null;
    private MonsterState monsterState = null;

    void Awake()
    {
        monsterMovement = GetComponent<MonsterMovement>();
        monsterState = GetComponent<MonsterState>();
    }
}