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
        public int clas;
        public int level;
        public float exp;
        public int glod;
        public int storageGold;
        public int att;
        public int def;
        public float currentHp;
        public float currentMp;
        public int maxHp;
        public int maxMp;
        public string spawnPos;
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
    }
}