using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerShortCut
{
    private static PlayerShortCut playerShortCut = null;
    public static PlayerShortCut Instance
    {
        get
        {
            if (playerShortCut == null)
            {
                playerShortCut = new PlayerShortCut();
            }

            return playerShortCut;
        }
    }

    [System.Serializable]
    public struct ShortCutInfo
    {
        public TypeData.ItemType itemType;
        public int skillIndex;
        public int equipmentIndex;
        public int cusomableIndex;
        public int materialIndex;
        public int quantity;
    }

    public ShortCutInfo shortCutInfo;

    public Dictionary<int, ShortCutInfo> shortCutInfos = null;

    public PlayerShortCut()
    {
        shortCutInfos = new Dictionary<int, ShortCutInfo>();
        shortCutInfo.itemType = TypeData.ItemType.없음;
        shortCutInfo.skillIndex = 0;
        shortCutInfos.Add(7, shortCutInfo);
    }

    public void SetShortCutData()
    {

    }

    public bool GetShortCutData(int slotIndex, ref UISlotInfo.SlotInfo slotInfo)
    {
        // 해당 슬롯에 정보가 없으면 리턴
        if (!shortCutInfos.ContainsKey(slotIndex))
        {
            return false;
        }

        // 아이템 타입이 없으면 스킬이므로
        if (shortCutInfos[slotIndex].itemType == TypeData.ItemType.없음)
        {
            slotInfo.skillIndex = shortCutInfos[slotIndex].skillIndex; // 스킬 인덱스
            // 예외처리 - 혹시나 배운 스킬이 아닐경우
            if (!PlayerSkillData.Instance.GetSkillData(slotInfo.skillIndex))
            {
                Debug.Log("배우지 않은 스킬이 슬롯에 있음");
                return false;
            }

            slotInfo.iconName = SkillData.Instance.skillInfos[slotInfo.skillIndex].name; // 스킬 이름
            slotInfo.slotInfoType = TypeData.SlotInfoType.스킬;
        }
        else
        {
            switch (shortCutInfos[slotIndex].itemType)
            {
                case TypeData.ItemType.장비:
                    {
                        slotInfo.itemIndex = shortCutInfos[slotIndex].equipmentIndex;
                        slotInfo.iconName = ItemData.Instance.equipmentInfos[slotInfo.itemIndex].name;
                    }
                    break;

                case TypeData.ItemType.소모품:
                    {
                        slotInfo.itemIndex = shortCutInfos[slotIndex].cusomableIndex;
                        slotInfo.iconName = ItemData.Instance.cusomableInfos[slotInfo.itemIndex].name;
                    }
                    break;

                case TypeData.ItemType.재료:
                    {
                        slotInfo.itemIndex = shortCutInfos[slotIndex].materialIndex;
                        slotInfo.iconName = ItemData.Instance.materialInfos[slotInfo.itemIndex].name;
                    }
                    break;
            }

            slotInfo.slotInfoType = TypeData.SlotInfoType.아이템;
            slotInfo.quantity = shortCutInfos[slotIndex].quantity;
        }

        return true;
    }
}