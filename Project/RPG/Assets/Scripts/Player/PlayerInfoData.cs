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
        public string name;
        public int level;
        public string nick;
        public float exp;
        public int glod;
    }

    public InfoData infoData;

    public PlayerInfoData()
    {
        infoData.glod = 10000;
    }
    // TODO : 공격력 / 방어력 / HP / MP 계산
}