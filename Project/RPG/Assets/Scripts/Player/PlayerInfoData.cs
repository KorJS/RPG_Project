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
    public struct InfoData
    {
        public int index;
        public string nick;
        public int level;
        public float exp;
        public int glod;
        public int att;
        public int def;
        public int hp;
        public int mp;
    }

    public InfoData infoData;

    public int totalAtt = 0;
    public int totalDef = 0;
    public int totalHp = 0;
    public int totalMp = 0;

    public PlayerInfoData()
    {
        infoData.index = 1;
        infoData.nick = "테라";
        infoData.level = 10;
        infoData.exp = 0;
        infoData.glod = 10000;
        infoData.att = 100;
        infoData.def = 100;
        infoData.hp = 1000;
        infoData.mp = 1000;
    }

    // TODO : 공격력 / 방어력 / HP / MP 계산
    public void SetStat(int _att, int _def, int _hp, int _mp)
    {
        totalAtt = infoData.att + _att;
        totalDef = infoData.def + _def;
        totalHp = infoData.hp + _hp;
        totalMp = infoData.mp + _mp;
    }
}