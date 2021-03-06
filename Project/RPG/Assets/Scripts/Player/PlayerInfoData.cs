﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInfoData
{
    private static PlayerInfoData playerInfoData = null;
    public static PlayerInfoData Instance
    {
        get
        {
            if (playerInfoData == null)
            {
                playerInfoData = new PlayerInfoData();
            }

            return playerInfoData;
        }
    }

    // TODO :  리스폰 위치
    public class InfoData
    {
        public int      accIndex;    // 계정 인덱스
        public int      charIndex;   // 케릭터 인덱스
        public string   nick;        // 닉네임
        public int      playerType;  // 직업
        public int      level;       // 레벨
        public float    exp;         // 경험치
        public int      gold;        // 소지금
        public int      storageGold; // 창고 보관금
        public int      att;         // 공격력
        public int      def;         // 방어력
        public float    currentHp;   // 현재 체력
        public float    currentMp;   // 현재 마력
        public int      maxHp;       // 최대 체력
        public int      maxMp;       // 최대 마력
        public string   spawnPos;    // 종료시 주인공 위치
        public int      areaType;    // 주인공이 있는 지역
    }

    public InfoData infoData;

    public int      totalAtt    = 0; // 공격력
    public int      totalDef    = 0; // 방어력
    public int      totalMaxHp  = 0; // 체력
    public int      totalMaxMp  = 0; // 마력

    // 케릭터창에 버프 값 출력
    public string   buffAtt     = null;
    public string   buffDef     = null;
    public string   buffHp      = null;
    public string   buffMp      = null;

    // 스텟 설정 (장비 착용에 따른)
    public void SetStat(int _att, int _def, int _hp, int _mp)
    {
        totalAtt = infoData.att + _att;
        totalDef = infoData.def + _def;
        totalMaxHp = infoData.maxHp + _hp;
        totalMaxMp = infoData.maxMp + _mp;

        // 장비 해제시 현재 체력,마력이 총 체력,마력보다 높으면
        {
            if (infoData.currentHp > totalMaxHp)
            {
                infoData.currentHp = totalMaxHp;
            }

            if (infoData.currentMp > totalMaxMp)
            {
                infoData.currentMp = totalMaxMp;
            }
        }
    }

    // 버프 아이템 사용시 상승 - 쿨타임 종료 후 원래대로
    public void SetBuff(int _att, int _def, int _hp, int _mp)
    {
        buffAtt = "\n\t [FFFFFFFF] + [0000FFFF]" + _att.ToString();
        buffDef = "\n\t [FFFFFFFF] + [0000FFFF]" + _def.ToString();
        buffHp = "\n\t [FFFFFFFF] + [0000FFFF]" + _hp.ToString();
        buffMp = "\n\t [FFFFFFFF] + [0000FFFF]" + _mp.ToString();

        if (_att <= 0) { buffAtt = null; }
        if (_def <= 0) { buffDef = null; }
        if (_hp <= 0) { buffHp = null; }
        if (_mp <= 0) { buffMp = null; }

        totalAtt += _att;
        totalDef += _def;
        totalMaxHp += _hp;
        totalMaxMp += _mp;

        // 장비 해제시 현재 체력,마력이 총 체력,마력보다 높으면
        {
            if (infoData.currentHp > totalMaxHp)
            {
                infoData.currentHp = totalMaxHp;
            }

            if (infoData.currentMp > totalMaxMp)
            {
                infoData.currentMp = totalMaxMp;
            }
        }
    }

    // 현재 체력 설정
    public void SetCurrentHp(float hpValue)
    {
        Debug.Log("전 : " + hpValue);

        // 데미지인 경우 음수
        if (hpValue < 0)
        {
            hpValue = hpValue - (totalDef / 100f);
        }
        Debug.Log("후 : " + hpValue);

        infoData.currentHp += hpValue;

        if (infoData.currentHp <= 0f)
        {
            infoData.currentHp = 0f;
        }

        if (infoData.currentHp > totalMaxHp)
        {
            infoData.currentHp = totalMaxHp;
        }
    }

    // 현재 마력 설정
    public void SetCurrentMp(float mpValue)
    {
        infoData.currentMp += mpValue;

        if (infoData.currentMp <= 0f)
        {
            infoData.currentMp = 0f;
        }

        if (infoData.currentMp > totalMaxMp)
        {
            infoData.currentMp = totalMaxMp;
        }
    }

    // 서버의 level_table 에서 비교 후 맞으면 데이터 처리하는 씩으로 바꾸자. 상점처럼
    public bool SetExp(float _exp)
    {
        bool isLevelUp = false;

        if (infoData.level == LevelData.Instance.MAX_Level)
        {
            infoData.exp = 0;
            return isLevelUp;
        }

        Dictionary<int, LevelData.LevelInfo> tempLevelInfo = LevelData.Instance.levelInfos;

        float maxExp = 0f;
        int MAX_LEVEL = LevelData.Instance.MAX_Level;
        int currentLevel = infoData.level;
        float currentExp = infoData.exp;

        // 현재 레벨 부터 만렙까지
        for (int i = currentLevel; i <= MAX_LEVEL; i++)
        {
            maxExp += tempLevelInfo[i].exp;

            // 습득 경험치 + 현재경험치가 다음렙에 필요한 경험치 보다 작을때 i 가 레벨이 됨
            float up_exp = _exp + currentExp;

            if (up_exp <= maxExp)
            {
                infoData.level = i;
                infoData.exp = up_exp;

                // 렙업
                if (i != currentLevel)
                {
                    // lv 3 = 10 + 20 + 70 = 100(maxExp)
                    // ( 50 + 0 ) - ( 100 - 70 ) = 20
                    infoData.exp = up_exp - (maxExp - LevelData.Instance.levelInfos[i].exp);
                    isLevelUp = true;
                }

                if (up_exp == maxExp)
                {
                    int level = i + 1;
                    // 만렙을 초과하면 만렙으로
                    if (level > MAX_LEVEL)
                    {
                        level = MAX_LEVEL; // level = i;
                    }

                    infoData.level = level;
                    infoData.exp = 0;
                }

                UIManager.Instance.SetSkillListUpActive(i);

                break;
            }
        }

        return isLevelUp;
    }

    public void DataClear()
    {
        infoData = null;
    }
}