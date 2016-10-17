using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MonsterMovement))]
[RequireComponent(typeof(MonsterState))]
[RequireComponent(typeof(MonsterRange))]
public class MonsterInfoData : MonoBehaviour
{
    // TODO : 몬스터 스폰될때 몬스터 정보을 이 스크립트에 저장 및 수치계산 관리
    public MonsterManager.MonsterInfo monsterInfo;
    private Transform monsterT;

    public float currentHP = 0f;

    void Awake()
    {
        monsterT = transform;
    }

    void Update()
    {

    }

    void OnEnable()
    {
        currentHP = monsterInfo.hp;
    }

    public void SetCurrentHP(float hp)
    {
        currentHP += hp;
        Debug.Log(currentHP);
    }

    public void Reset()
    {
        monsterT.localPosition = Vector3.zero;
        currentHP = monsterInfo.hp;
    }
}