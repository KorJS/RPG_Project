using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory
{
    private static PlayerInventory playerInventory = null;
    public static PlayerInventory Instance
    {
        get
        {
            if (playerInventory == null)
            {
                playerInventory = new PlayerInventory();
            }

            return playerInventory;
        }
    }

    [System.Serializable]
    public struct InventoryInfo
    {
        public TypeData.ItemType itemType;
        public int equipmentIndex;
        public int cusomableIndex;
        public int materialIndex;
        public int quantity;
    }

    public InventoryInfo inventoryInfo; // 인벤토리 정보

    public Dictionary<int, InventoryInfo> inventoryInfos = null; // 인벤토리 데이터를 로드해서 저장

    public PlayerInventory()
    {
        inventoryInfos = new Dictionary<int, InventoryInfo>();

        inventoryInfo.itemType = TypeData.ItemType.소모품;
        inventoryInfo.cusomableIndex = 0;
        inventoryInfo.quantity = 80;
        inventoryInfos.Add(1, inventoryInfo);

        inventoryInfo.itemType = TypeData.ItemType.소모품;
        inventoryInfo.cusomableIndex = 1;
        inventoryInfo.quantity = 20;
        inventoryInfos.Add(2, inventoryInfo);

        inventoryInfo.itemType = TypeData.ItemType.소모품;
        inventoryInfo.cusomableIndex = 2;
        inventoryInfo.quantity = 10;
        inventoryInfos.Add(5, inventoryInfo);
    }

    public void SetInventoryData()
    {

    }

    public bool GetInventroyDate(int slotIndex, ref UISlotInfo.SlotInfo slotInfo)
    {
        // 해당 슬롯에 정보가 없으면 리턴
        if (!inventoryInfos.ContainsKey(slotIndex))
        {
            return false;
        }

        switch (inventoryInfos[slotIndex].itemType)
        {
            case TypeData.ItemType.장비:
                {
                    slotInfo.itemIndex = inventoryInfos[slotIndex].equipmentIndex;
                    slotInfo.iconName = ItemData.Instance.equipmentInfos[slotInfo.itemIndex].name;
                }
                break;

            case TypeData.ItemType.소모품:
                {
                    slotInfo.itemIndex = inventoryInfos[slotIndex].cusomableIndex;
                    slotInfo.iconName = ItemData.Instance.cusomableInfos[slotInfo.itemIndex].name;
                }
                break;

            case TypeData.ItemType.재료:
                {
                    slotInfo.itemIndex = inventoryInfos[slotIndex].materialIndex;
                    slotInfo.iconName = ItemData.Instance.materialInfos[slotInfo.itemIndex].name;
                }
                break;
        }

        slotInfo.slotInfoType = TypeData.SlotInfoType.아이템;
        slotInfo.quantity = inventoryInfos[slotIndex].quantity;

        return true;
    }
}