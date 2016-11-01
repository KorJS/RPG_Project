﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MonsterMovement))]
[RequireComponent(typeof(MonsterState))]
[RequireComponent(typeof(MonsterRange))]
public class MonsterInfoData : MonoBehaviour
{
    // TODO : 몬스터 스폰될때 몬스터 정보을 이 스크립트에 저장 및 수치계산 관리
    public MonsterData.MonsterInfo monsterInfo;
    private MonsterMovement monsterMovemnet = null;
    private MonsterState monsterState = null;
    private Transform monsterT;

    public float currentHP = 0f;
    public bool isDeath = false;

    void Awake()
    {
        monsterT = transform;
        monsterMovemnet = GetComponent<MonsterMovement>();
        monsterState = GetComponent<MonsterState>();
    }

    void OnEnable()
    {
        currentHP = monsterInfo.hp;
    }

    void Update()
    {
        if (isDeath)
        {
            monsterState.nextState = TypeData.State.죽음;
            monsterMovemnet.animator.SetTrigger(monsterMovemnet.animationSettings.isDeathTrigger);
            StartCoroutine(monsterMovemnet.Death(1f));
            isDeath = false;
        }
    }

    public void SetCurrentHP(float hp)
    {
        if (hp < 0)
        {
            Debug.Log("monsterInfo.defence : " + monsterInfo.defence + " hp : " + hp);
            hp /= monsterInfo.defence;
        }
        Debug.Log(currentHP + " : " + hp);
        currentHP += hp;
        Debug.Log(currentHP + " : " + hp);
        if (currentHP <= 0)
        {
            isDeath = true;
            gameObject.layer = LayerMask.NameToLayer("Default");
        }

        Debug.Log("현재 HP : " + currentHP);
    }

    private void CreateHUD()
    {

    }

    public void Reset()
    {
        monsterT.localPosition = Vector3.zero;
        currentHP = monsterInfo.hp;
    }
}