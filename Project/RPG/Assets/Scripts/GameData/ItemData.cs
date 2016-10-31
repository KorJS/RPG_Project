using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemData
{
    private static ItemData itemData = null;
    public static ItemData Instance
    {
        get
        {
            if (itemData == null)
            {
                itemData = new ItemData();
            }

            return itemData;
        }
    }

    // 장비 정보 - 장비타입 상관없이 통으로 받아옴.
    public struct EquipmentInfo
    {
        public string name;                             // 이름
        public string iconName;                         // 아이콘 이름
        public int equipmentType;    // 장비타입
        public int playerType;          // 착용직업
        public int level;                               // 착용레벨
        public int attack;                              // 공격력
        public int defence;                             // 방어력
        public int hp;                                  // 체력
        public int mp;                                  // 마력
        public int buyGold;                             // 구매가격
        public int sellGold;                            // 판매가격
    };

    // 소모품 정보
    public struct CusomableInfo
    {
        public string name;                             // 이름
        public string iconName;                         // 아이콘 이름
        public string instruction;                      // 설명
        public float increase;                          // 상승치
        public float coolTime;                          // 쿨타임
        public int buyGold;                             // 구매가격
        public int sellGold;                            // 판매가격
    };

    // 퀘스트템 정보
    public struct QuestItemInfo
    {
        public string name;                             // 이름
        public string iconName;                         // 아이콘 이름
        public int buyGold;                             // 판매가격
    };

    public Dictionary<int, EquipmentInfo> equipmentInfos = null; // 장비 데이터를 로드해서 저장
    public Dictionary<int, CusomableInfo> cusomableInfos = null; // 소모품 데이터를 로드해서 저장
    public Dictionary<int, QuestItemInfo> questItemInfos = null; // 퀘스트템 데이터를 로드해서 저장

    public EquipmentInfo equipmentInfo;  // 장비 정보
    public CusomableInfo cusomableInfo;  // 소모품 정보
    public QuestItemInfo questItemInfo;   // 퀘스트템 정보

    public ItemData()
    {
        equipmentInfos = new Dictionary<int, EquipmentInfo>();
        cusomableInfos = new Dictionary<int, CusomableInfo>();
        questItemInfos = new Dictionary<int, QuestItemInfo>();

        cusomableInfo.name = "H 포션";
        cusomableInfo.iconName = "HPPotion";
        cusomableInfo.increase = 100;
        cusomableInfo.coolTime = 10f;
        cusomableInfo.buyGold = 1000;
        cusomableInfo.sellGold = 100;
        cusomableInfos.Add(0, cusomableInfo);

        cusomableInfo.name = "MP 포션";
        cusomableInfo.iconName = "MPPotion";
        cusomableInfo.increase = 100;
        cusomableInfo.coolTime = 10f;
        cusomableInfo.buyGold = 1000;
        cusomableInfo.sellGold = 100;
        cusomableInfos.Add(1, cusomableInfo);

        cusomableInfo.name = "HMP 포션";
        cusomableInfo.iconName = "HMPPotion";
        cusomableInfo.increase = 100;
        cusomableInfo.coolTime = 10f;
        cusomableInfo.buyGold = 2000;
        cusomableInfo.sellGold = 500;
        cusomableInfos.Add(2, cusomableInfo);

        equipmentInfo.name = "고룡갑옷";
        equipmentInfo.iconName = "DragonArmor";
        equipmentInfo.equipmentType = (int)TypeData.EquipmentType.갑옷;
        equipmentInfo.defence = 10000;
        equipmentInfo.hp = 10000;
        equipmentInfo.buyGold = 2000;
        equipmentInfo.sellGold = 1000;
        equipmentInfos.Add(0, equipmentInfo);

        equipmentInfo.name = "발로나갑옷";
        equipmentInfo.iconName = "ValonaArmor";
        equipmentInfo.equipmentType = (int)TypeData.EquipmentType.갑옷;
        equipmentInfo.defence = 150;
        equipmentInfo.buyGold = 1000;
        equipmentInfo.sellGold = 500;
        equipmentInfos.Add(1, equipmentInfo);
    }
}