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

    // 장비 정보
    public struct EquipmentInfo
    {
        public string name;                             // 이름
        public TypeData.EquipmentType equipmentType;    // 장비타입
        public int level;                               // 착용레벨
        public int attack;                              // 공격력
        public int defence;                             // 방어력
        public int hp;                                  // 체력
        public int buyGold;
        public int SellGold;
    };

    // 소모품 정보 - 우선 hp만.. 
    public struct CusomableInfo
    {
        public string name;                             // 이름
        public float increase;                          // 상승치
        public int buyGold;
        public int SellGold;
    };

    // 재료 정보
    public struct MaterialInfo
    {
        public string name;                             // 이름
        public TypeData.MaterialType materialType;      // 재료 타입
        public int buyGold;
        public int SellGold;
    };

    public Dictionary<int, EquipmentInfo> equipmentInfos = null; // 장비 데이터를 로드해서 저장
    public Dictionary<int, CusomableInfo> cusomableInfos = null; // 소모품 데이터를 로드해서 저장
    public Dictionary<int, MaterialInfo> materialInfos = null; // 재료 데이터를 로드해서 저장

    public EquipmentInfo equipmentInfo;  // 장비 정보
    public CusomableInfo cusomableInfo;  // 소모품 정보
    public MaterialInfo materialInfo;   // 재료 정보

    public ItemData()
    {
        equipmentInfos = new Dictionary<int, EquipmentInfo>();
        cusomableInfos = new Dictionary<int, CusomableInfo>();
        materialInfos = new Dictionary<int, MaterialInfo>();

        cusomableInfo.name = "HPPotion";
        cusomableInfo.increase = 100;
        cusomableInfos.Add(0, cusomableInfo);

        cusomableInfo.name = "MPPotion";
        cusomableInfo.increase = 100;
        cusomableInfos.Add(1, cusomableInfo);

        cusomableInfo.name = "HMPPotion";
        cusomableInfo.increase = 100;
        cusomableInfos.Add(2, cusomableInfo);
    }
}