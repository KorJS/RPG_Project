using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class ItemData
{
    // volatile 동시에 실행되는 여러 스레드에 의해 필드가 수정될 수 있음을 나타낸다.
    private static volatile ItemData itemData = null;
    private static object _lock = new System.Object();

    public static ItemData Instance
    {
        get
        {
            if (itemData == null)
            {
                // lock으로 지정된 블록안의 코드르 하나의 스레드만 접근
                lock (_lock)
                {
                    if (itemData == null)
                    {
                        itemData = new ItemData();
                    }
                }
            }

            return itemData;
        }
    }

    // 장비 정보 - 장비타입 상관없이 통으로 받아옴.
    public struct EquipmentInfo
    {
        public string name;                             // 이름
        public string iconName;                         // 아이콘 이름
        public string description;                      // 설명
        public int equipmentType;                       // 장비타입
        public int playerType;                          // 착용직업
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
        public string description;                      // 설명
        public float regeneration;                      // 상승치
        public float coolTime;                          // 쿨타임
        public int buyGold;                             // 구매가격
        public int sellGold;                            // 판매가격
    };

    // 퀘스트템 정보
    public struct QuestItemInfo
    {
        public string name;                             // 이름
        public string iconName;                         // 아이콘 이름
        public string description;                      // 설명
        public int buyGold;                             // 판매가격
    };

    public Dictionary<int, EquipmentInfo> equipmentInfos = null; // 장비 
    public Dictionary<int, CusomableInfo> cusomableInfos = null; // 소모품 
    public Dictionary<int, QuestItemInfo> questItemInfos = null; // 퀘스트템 

    public EquipmentInfo equipmentInfo;  // 장비 정보
    public CusomableInfo cusomableInfo;  // 소모품 정보
    public QuestItemInfo questItemInfo;   // 퀘스트템 정보

    private ItemData()
    {
        equipmentInfos = new Dictionary<int, EquipmentInfo>();
        cusomableInfos = new Dictionary<int, CusomableInfo>();
        questItemInfos = new Dictionary<int, QuestItemInfo>();

        cusomableInfo.name = "HP 포션";
        cusomableInfo.iconName = "HPPotion";
        cusomableInfo.regeneration = 100;
        cusomableInfo.coolTime = 10f;
        cusomableInfo.buyGold = 1000;
        cusomableInfo.sellGold = 100;
        cusomableInfos.Add(0, cusomableInfo);

        cusomableInfo.name = "MP 포션";
        cusomableInfo.iconName = "MPPotion";
        cusomableInfo.regeneration = 100;
        cusomableInfo.coolTime = 10f;
        cusomableInfo.buyGold = 1000;
        cusomableInfo.sellGold = 100;
        cusomableInfos.Add(1, cusomableInfo);

        cusomableInfo.name = "HMP 포션";
        cusomableInfo.iconName = "HMPPotion";
        cusomableInfo.regeneration = 100;
        cusomableInfo.coolTime = 10f;
        cusomableInfo.buyGold = 2000;
        cusomableInfo.sellGold = 500;
        cusomableInfos.Add(2, cusomableInfo);

        equipmentInfo.name = "베르고스 검";
        equipmentInfo.iconName = "Vergos_Lance";
        equipmentInfo.equipmentType = (int)TypeData.EquipmentType.무기;
        equipmentInfo.attack = 10000;
        equipmentInfo.hp = 10000;
        equipmentInfo.buyGold = 10000;
        equipmentInfo.sellGold = 5000;
        equipmentInfos.Add(0, equipmentInfo);

        equipmentInfo.name = "발로나 검";
        equipmentInfo.iconName = "Val_Lance";
        equipmentInfo.equipmentType = (int)TypeData.EquipmentType.무기;
        equipmentInfo.attack = 5000;
        equipmentInfo.hp = 1000;
        equipmentInfo.mp = 500;
        equipmentInfo.buyGold = 4000;
        equipmentInfo.sellGold = 2000;
        equipmentInfos.Add(1, equipmentInfo);

        equipmentInfo.name = "베르고스 갑옷";
        equipmentInfo.iconName = "Vergos_Body";
        equipmentInfo.equipmentType = (int)TypeData.EquipmentType.갑옷;
        equipmentInfo.defence = 10000;
        equipmentInfo.hp = 10000;
        equipmentInfo.buyGold = 2000;
        equipmentInfo.sellGold = 1000;
        equipmentInfos.Add(2, equipmentInfo);

        equipmentInfo.name = "발로나갑옷";
        equipmentInfo.iconName = "Val_Body";
        equipmentInfo.equipmentType = (int)TypeData.EquipmentType.갑옷;
        equipmentInfo.defence = 1500;
        equipmentInfo.hp = 500;
        equipmentInfo.buyGold = 1000;
        equipmentInfo.sellGold = 500;
        equipmentInfos.Add(3, equipmentInfo);

        equipmentInfo.name = "베르고스 장갑";
        equipmentInfo.iconName = "Vergos_Hand";
        equipmentInfo.equipmentType = (int)TypeData.EquipmentType.장갑;
        equipmentInfo.defence = 2000;
        equipmentInfo.hp = 200;
        equipmentInfo.mp = 200;
        equipmentInfo.buyGold = 2000;
        equipmentInfo.sellGold = 1000;
        equipmentInfos.Add(4, equipmentInfo);

        equipmentInfo.name = "발로나 장갑";
        equipmentInfo.iconName = "Val_Hand";
        equipmentInfo.equipmentType = (int)TypeData.EquipmentType.장갑;
        equipmentInfo.defence = 2000;
        equipmentInfo.hp = 150;
        equipmentInfo.mp = 150;
        equipmentInfo.buyGold = 1000;
        equipmentInfo.sellGold = 500;
        equipmentInfos.Add(5, equipmentInfo);

        equipmentInfo.name = "베르고스 신발";
        equipmentInfo.iconName = "Vergos_Leg";
        equipmentInfo.equipmentType = (int)TypeData.EquipmentType.신발;
        equipmentInfo.defence = 1000;
        equipmentInfo.hp = 200;
        equipmentInfo.mp = 200;
        equipmentInfo.buyGold = 2000;
        equipmentInfo.sellGold = 1000;
        equipmentInfos.Add(6, equipmentInfo);

        equipmentInfo.name = "발로나 신발";
        equipmentInfo.iconName = "Val_Leg";
        equipmentInfo.equipmentType = (int)TypeData.EquipmentType.신발;
        equipmentInfo.defence = 1000;
        equipmentInfo.hp = 150;
        equipmentInfo.mp = 150;
        equipmentInfo.buyGold = 1000;
        equipmentInfo.sellGold = 500;
        equipmentInfos.Add(7, equipmentInfo);
    }


}