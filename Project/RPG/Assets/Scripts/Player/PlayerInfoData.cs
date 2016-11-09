using UnityEngine;
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
        public int accIndex;
        public int charIndex;
        public string nick;
        public int playerType;
        public int level;
        public float exp;
        public int gold;
        public int storageGold;
        public int att;
        public int def;
        public float currentHp;
        public float currentMp;
        public int maxHp;
        public int maxMp;
        public string spawnPos;
        public int areaType;
    }

    public InfoData infoData;

    public int totalAtt = 0;
    public int totalDef = 0;
    public int totalMaxHp = 0;
    public int totalMaxMp = 0;

    public PlayerInfoData()
    {
        //infoData.accIndex = 1;
        //infoData.charIndex = 1;
        //infoData.nick = "테라";
        //infoData.clas = (int)TypeData.PlayerType.기사;
        //infoData.level = 10;
        //infoData.exp = 0;
        //infoData.glod = 10000;
        //infoData.att = 100;
        //infoData.def = 100;
        //infoData.currentHp = 1000;
        //infoData.currentMp = 1000;
        //totalMaxHp = infoData.maxHp = 1000;
        //totalMaxMp = infoData.maxMp = 1000;
    }

    // TODO : 공격력 / 방어력 / HP / MP 계산
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

    public void SetCurrentHp(float hpValue)
    {
        // 데미지인 경우 음수
        if (hpValue < 0)
        {
            hpValue = hpValue - (totalDef / 100f);
        }

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
    public void SetExp(float _exp)
    {
        if (infoData.level == LevelData.Instance.MAX_Level)
        {
            infoData.exp = 0;
            return;
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
    }
}