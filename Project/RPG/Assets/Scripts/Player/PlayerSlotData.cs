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

    public Dictionary<int, SlotInfoData> tempSlotInfoDatas;

    private int currentIndex = 0;
    private int targetIndex = 0;

    private int currentQuantity = 0;
    private int targetQuantity = 0;

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
    public bool CheckInventoryData(ref Dictionary<int, SlotInfoData> slotInfoDatas)
    {
        // 해당 슬롯에 정보가 없으면 리턴
        if (!inventoryInfos.ContainsKey(currentIndex))
        {
            return false;
        }

        slotInfoDatas = inventoryInfos;

        return true;
    }

    // 단축창 정보 가져오기
    public bool CheckShortCutData(ref Dictionary<int, SlotInfoData> slotInfoDatas)
    {
        // 해당 슬롯에 정보가 없으면 리턴
        if (!shortCutInfos.ContainsKey(currentIndex))
        {
            return false;
        }

        slotInfoDatas = shortCutInfos;
        return true;
    }

    // 창고 정보 가져오기
    public bool CheckStorageData(ref Dictionary<int, SlotInfoData> slotInfoDatas)
    {
        // 해당 슬롯에 정보가 없으면 리턴
        if (!storageInfos.ContainsKey(currentIndex))
        {
            return false;
        }

        slotInfoDatas = storageInfos;
        return true;
    }

    // 타입을 비교하여 정보를 가져옴
    public bool CheckSlotType(TypeData.SlotType slotType, ref Dictionary<int, SlotInfoData> slotInfoDatas)
    {
        bool isExist = false;

        switch (slotType)
        {
            case TypeData.SlotType.인벤토리:
                {
                    isExist = CheckInventoryData(ref slotInfoDatas);
                }
                break;

            case TypeData.SlotType.단축키:
                {
                    isExist = CheckShortCutData(ref slotInfoDatas);
                }
                break;

            case TypeData.SlotType.창고:
                {
                    isExist = CheckStorageData(ref slotInfoDatas);
                }
                break;
        }

        return isExist;
    }

    // 인벤토리, 단축키, 창고 정보를 슬롯 정보에 맞게 변환
    public bool SetSlotData(TypeData.SlotType slotType, int _slotIndex, ref UISlotInfo.SlotInfo slotInfo)
    {
        bool isExist = false;
        currentIndex = _slotIndex;

        isExist = CheckSlotType(slotType, ref tempSlotInfoDatas);
        
        // 정보가 없으면
        if (!isExist)
        {
            slotInfo = new UISlotInfo.SlotInfo();
            slotInfo.slotInfoType = TypeData.SlotInfoType.없음;
            slotInfo.itemType = TypeData.ItemType.없음;
            return false;
        }

        // 아이템 타입이 없으면 스킬이므로
        if (tempSlotInfoDatas[currentIndex].itemType == TypeData.ItemType.없음)
        {
            if (tempSlotInfoDatas[currentIndex].skillIndex < 0)
            {
                Debug.Log("Error : " + tempSlotInfoDatas[currentIndex].skillIndex + " 아이템임");
            }

            slotInfo.skillIndex = tempSlotInfoDatas[currentIndex].skillIndex; // 스킬 인덱스
            // 예외처리 - 혹시나 배운 스킬이 아닐경우
            if (!PlayerSkillData.Instance.GetSkillData(slotInfo.skillIndex))
            {
                Debug.Log("배우지 않은 스킬이 슬롯에 있음");
                return false;
            }

            slotInfo.iconName = SkillData.Instance.skillInfos[slotInfo.skillIndex].name; // 스킬 이름
            slotInfo.slotInfoType = TypeData.SlotInfoType.스킬;
            slotInfo.itemType = TypeData.ItemType.없음;
        }
        else
        {
            if (tempSlotInfoDatas[currentIndex].skillIndex > -1)
            {
                Debug.Log("Error : " + tempSlotInfoDatas[currentIndex].skillIndex + " 스킬임");
            }

            switch (tempSlotInfoDatas[currentIndex].itemType)
            {
                case TypeData.ItemType.장비:
                    {
                        slotInfo.itemIndex = tempSlotInfoDatas[currentIndex].equipmentIndex;
                        slotInfo.iconName = ItemData.Instance.equipmentInfos[slotInfo.itemIndex].name;
                        slotInfo.itemType = TypeData.ItemType.장비;
                    }
                    break;

                case TypeData.ItemType.소모품:
                    {
                        slotInfo.itemIndex = tempSlotInfoDatas[currentIndex].cusomableIndex;
                        slotInfo.iconName = ItemData.Instance.cusomableInfos[slotInfo.itemIndex].name;
                        slotInfo.itemType = TypeData.ItemType.소모품;
                    }
                    break;

                case TypeData.ItemType.재료:
                    {
                        slotInfo.itemIndex = tempSlotInfoDatas[currentIndex].materialIndex;
                        slotInfo.iconName = ItemData.Instance.materialInfos[slotInfo.itemIndex].name;
                        slotInfo.itemType = TypeData.ItemType.재료;
                    }
                    break;
            }

            slotInfo.slotInfoType = TypeData.SlotInfoType.아이템;
            slotInfo.quantity = tempSlotInfoDatas[currentIndex].quantity;
        }

        return true;
    }
    
    // 교환
    public void SwapSlotData(TypeData.SlotType slotType, int _currentIndex, int _targetIndex, int _currentQuantity, int _targetQuantity,  bool isTargetItemExist)
    {
        currentIndex = _currentIndex;
        targetIndex = _targetIndex;
        currentQuantity = _currentQuantity;
        targetQuantity = _targetQuantity;

         // TODO : 스킬창이면 제거하자

        if (!CheckSlotType(slotType, ref tempSlotInfoDatas))
        {
            Debug.Log("현재슬롯에 정보가 없음");
            return;
        }
        
        // 타겟 슬롯에 정보가 존재 하지않으면 현재슬롯 정보를 타겟슬롯에 넣고 현재슬롯 정보 제거
        if (!isTargetItemExist)
        {
            tempSlotInfoDatas[targetIndex] = tempSlotInfoDatas[currentIndex];
            tempSlotInfoDatas.Remove(currentIndex);
            return;
        }

        SlotInfoData currentSlotInfoData = tempSlotInfoDatas[currentIndex];
        SlotInfoData targetSlotInfoData = tempSlotInfoDatas[targetIndex];

        // 혹시나 데이터 수량과 슬롯에 수량이 다르다면 있다면.
        if ((currentSlotInfoData.quantity != currentQuantity) || (targetSlotInfoData.quantity != targetQuantity))
        {
            currentSlotInfoData.quantity = currentQuantity;
            targetSlotInfoData.quantity = targetQuantity;
        }

        tempSlotInfoDatas[currentIndex] = targetSlotInfoData;
        tempSlotInfoDatas[targetIndex] = currentSlotInfoData;
    }
}