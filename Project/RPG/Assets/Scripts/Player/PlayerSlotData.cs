using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSlotData
{
    private static PlayerSlotData playerSlotData = null;
    public static PlayerSlotData Instance
    {
        get
        {
            if (playerSlotData == null)
            {
                playerSlotData = new PlayerSlotData();
            }

            return playerSlotData;
        }
    }

    //[System.Serializable]
    //public class InventoryInfo
    //{
    //    public TypeData.ItemType itemType;
    //    public int equipmentIndex;
    //    public int cusomableIndex;
    //    public int materialIndex;
    //    public int quantity;
    //}

    //public InventoryInfo inventoryInfo; // 인벤토리 정보

    //[System.Serializable]
    //public class ShortCutInfo
    //{
    //    public TypeData.ItemType itemType;
    //    public int skillIndex;
    //    public int equipmentIndex;
    //    public int cusomableIndex;
    //    public int materialIndex;
    //    public int quantity;
    //}

    //public ShortCutInfo shortCutInfo;

    [System.Serializable]
    public struct SlotInfoData
    {
        public TypeData.SlotType slotType;
        public TypeData.ItemType itemType;
        public int skillIndex;
        public int equipmentIndex;
        public int cusomableIndex;
        public int materialIndex;
        public int quantity;
    };

    public SlotInfoData slotInfoData;

    public Dictionary<int, SlotInfoData> inventoryInfos = null; // 인벤토리 데이터를 로드해서 저장
    public Dictionary<int, SlotInfoData> shortCutInfos = null;
    public Dictionary<int, SlotInfoData> storageInfos = null;

    public SlotInfoData tempSlotInfoData;
    public SlotInfoData currentSlotInfoData;
    public SlotInfoData targetSlotInfoData;

    public PlayerSlotData()
    {
        inventoryInfos = new Dictionary<int, SlotInfoData>();
        shortCutInfos = new Dictionary<int, SlotInfoData>();
        storageInfos = new Dictionary<int, SlotInfoData>();

        slotInfoData.itemType = TypeData.ItemType.소모품;
        slotInfoData.skillIndex = -1;
        slotInfoData.cusomableIndex = 0;
        slotInfoData.quantity = 80;
        inventoryInfos.Add(1, slotInfoData);

        slotInfoData.itemType = TypeData.ItemType.소모품;
        slotInfoData.skillIndex = -1;
        slotInfoData.cusomableIndex = 1;
        slotInfoData.quantity = 20;
        inventoryInfos.Add(2, slotInfoData);

        slotInfoData.itemType = TypeData.ItemType.소모품;
        slotInfoData.skillIndex = -1;
        slotInfoData.cusomableIndex = 2;
        slotInfoData.quantity = 10;
        inventoryInfos.Add(5, slotInfoData);

        slotInfoData.itemType = TypeData.ItemType.없음;
        slotInfoData.skillIndex = 0;
        shortCutInfos.Add(7, slotInfoData);
    }

    // 인벤토리 정보 가져오기
    public bool GetInventoryData(int slotIndex, ref SlotInfoData _slotInfoData)
    {
        // 해당 슬롯에 정보가 없으면 리턴
        if (!inventoryInfos.ContainsKey(slotIndex))
        {
            return false;
        }

        _slotInfoData = inventoryInfos[slotIndex];

        return true;
    }

    // 단축창 정보 가져오기
    public bool GetShortCutData(int slotIndex, ref SlotInfoData _slotInfoData)
    {
        // 해당 슬롯에 정보가 없으면 리턴
        if (!shortCutInfos.ContainsKey(slotIndex))
        {
            return false;
        }

        _slotInfoData = shortCutInfos[slotIndex];
        return true;
    }

    // 창고 정보 가져오기
    public bool GetStorageData(int slotIndex, ref SlotInfoData _slotInfoData)
    {
        // 해당 슬롯에 정보가 없으면 리턴
        if (!storageInfos.ContainsKey(slotIndex))
        {
            return false;
        }

        _slotInfoData = storageInfos[slotIndex];
        return true;
    }

    // 타입을 비교하여 정보를 가져옴
    public bool CheckSlotType(TypeData.SlotType slotType, int slotIndex, ref SlotInfoData _slotInfoData)
    {
        bool isExist = false;

        switch (slotType)
        {
            case TypeData.SlotType.인벤토리:
                {
                    isExist = GetInventoryData(slotIndex, ref _slotInfoData);
                }
                break;

            case TypeData.SlotType.단축키:
                {
                    isExist = GetShortCutData(slotIndex, ref _slotInfoData);
                }
                break;

            case TypeData.SlotType.창고:
                {
                    isExist = GetStorageData(slotIndex, ref _slotInfoData);
                }
                break;
        }

        return isExist;
    }
    
    // 인벤토리, 창고 정보를 슬롯 정보에 맞게 변환
    public bool ConvertWindowSlotData(TypeData.SlotType slotType, int slotIndex, ref UISlotInfo.SlotInfo slotInfo)
    {
        bool isExist = false;

        isExist = CheckSlotType(slotType, slotIndex, ref tempSlotInfoData);
        
        // 정보가 없으면
        if (!isExist)
        {
            return false;
        }

        // 아이템 타입이 없으면 스킬이므로
        if (tempSlotInfoData.itemType == TypeData.ItemType.없음)
        {
            if (tempSlotInfoData.skillIndex < 0)
            {
                Debug.Log("Error : " + tempSlotInfoData.skillIndex + " 아이템임");
            }

            slotInfo.skillIndex = tempSlotInfoData.skillIndex; // 스킬 인덱스
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
            if (tempSlotInfoData.skillIndex > -1)
            {
                Debug.Log("Error : " + tempSlotInfoData.skillIndex + " 스킬임");
            }

            switch (tempSlotInfoData.itemType)
            {
                case TypeData.ItemType.장비:
                    {
                        slotInfo.itemIndex = tempSlotInfoData.equipmentIndex;
                        slotInfo.iconName = ItemData.Instance.equipmentInfos[slotInfo.itemIndex].name;
                    }
                    break;

                case TypeData.ItemType.소모품:
                    {
                        slotInfo.itemIndex = tempSlotInfoData.cusomableIndex;
                        slotInfo.iconName = ItemData.Instance.cusomableInfos[slotInfo.itemIndex].name;
                    }
                    break;

                case TypeData.ItemType.재료:
                    {
                        slotInfo.itemIndex = tempSlotInfoData.materialIndex;
                        slotInfo.iconName = ItemData.Instance.materialInfos[slotInfo.itemIndex].name;
                    }
                    break;
            }

            slotInfo.slotInfoType = TypeData.SlotInfoType.아이템;
            slotInfo.quantity = tempSlotInfoData.quantity;
        }

        return true;
    }
    
    // 교환
    public void SwapSlotData(TypeData.SlotType slotType, int currentIndex, int targetIndex, bool isTargetItemExist)
    {
        // TODO : 존재하지 않는경우.

        switch (slotType)
        {
            case TypeData.SlotType.인벤토리:
                {
                    currentSlotInfoData = inventoryInfos[currentIndex];
                    if (isTargetItemExist)
                    {
                        targetSlotInfoData = inventoryInfos[targetIndex];
                        inventoryInfos[currentIndex] = targetSlotInfoData;
                    }
                    else
                    {
                        inventoryInfos.Remove(currentIndex);
                    }
                  
                    inventoryInfos[targetIndex] = currentSlotInfoData;
                }
                break;

            case TypeData.SlotType.단축키:
                {

                }
                break;

            case TypeData.SlotType.창고:
                {

                }
                break;
        }
        
    }
}