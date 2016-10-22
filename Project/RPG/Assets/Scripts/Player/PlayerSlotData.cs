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

    public Dictionary<int, SlotInfoData> inventoryInfos = null; // 인벤토리슬롯 정보
    public Dictionary<int, SlotInfoData> shortCutInfos = null;  // 단축슬롯 정보
    public Dictionary<int, SlotInfoData> storageInfos = null;   // 창고슬롯 정보

    public Dictionary<int, SlotInfoData> tempSlotInfoDatas;
    public Dictionary<int, SlotInfoData> tempCurrentSlotInfoDatas;
    public Dictionary<int, SlotInfoData> tempTargetSlotInfoDatas;

    private UISlotInfo currentInfo = null;
    private UISlotInfo targetInfo = null;

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
    private bool CheckInventoryData(int slotIndex, ref Dictionary<int, SlotInfoData> slotInfoDatas)
    {
        // 해당 슬롯에 정보가 없으면 리턴
        if (!inventoryInfos.ContainsKey(slotIndex))
        {
            return false;
        }

        slotInfoDatas = inventoryInfos;

        return true;
    }

    // 단축창 정보 가져오기
    private bool CheckShortCutData(int slotIndex, ref Dictionary<int, SlotInfoData> slotInfoDatas)
    {
        // 해당 슬롯에 정보가 없으면 리턴
        if (!shortCutInfos.ContainsKey(slotIndex))
        {
            return false;
        }

        slotInfoDatas = shortCutInfos;
        return true;
    }

    // 창고 정보 가져오기
    private bool CheckStorageData(int slotIndex, ref Dictionary<int, SlotInfoData> slotInfoDatas)
    {
        // 해당 슬롯에 정보가 없으면 리턴
        if (!storageInfos.ContainsKey(slotIndex))
        {
            return false;
        }

        slotInfoDatas = storageInfos;
        return true;
    }

    // 타입을 비교하여 정보를 가져옴
    private bool CheckSlotType(TypeData.SlotType slotType, int slotIndex, ref Dictionary<int, SlotInfoData> slotInfoDatas)
    {
        bool isExist = false;

        switch (slotType)
        {
            case TypeData.SlotType.인벤토리:
                {
                    isExist = CheckInventoryData(slotIndex, ref slotInfoDatas);
                }
                break;

            case TypeData.SlotType.단축키:
                {
                    isExist = CheckShortCutData(slotIndex, ref slotInfoDatas);
                }
                break;

            case TypeData.SlotType.창고:
                {
                    isExist = CheckStorageData(slotIndex, ref slotInfoDatas);
                }
                break;
        }

        return isExist;
    }

    // 아이템 수량 검사
    private void CheckQuantity(int currentIndex, int targetIndex, int currentQuantiry, int targetQuantity)
    {
        
    }

    // 슬롯에 변화가 생기면 슬롯의 정보로 재 설정 - 수량 변화
    public bool SetSlotData(TypeData.SlotType slotType, int slotIndex, ref UISlotInfo.SlotInfo slotInfo)
    {
        bool isExist = false;

        return true;
    }

    // 슬롯 필요한 정보에 맞게 변환하여 보냄 (인벤토리, 단축키, 창고 정보를 슬롯 정보에 맞게 변환)
    public bool GetSlotData(TypeData.SlotType slotType, int slotIndex, ref UISlotInfo.SlotInfo slotInfo)
    {
        bool isExist = false;

        isExist = CheckSlotType(slotType, slotIndex, ref tempSlotInfoDatas);
        
        // 정보가 없으면
        if (!isExist)
        {
            slotInfo = new UISlotInfo.SlotInfo();
            slotInfo.slotInfoType = TypeData.SlotInfoType.없음;
            slotInfo.itemType = TypeData.ItemType.없음;
            return false;
        }

        // 아이템 타입이 없으면 스킬이므로
        if (tempSlotInfoDatas[slotIndex].itemType == TypeData.ItemType.없음)
        {
            if (tempSlotInfoDatas[slotIndex].skillIndex < 0)
            {
                Debug.Log("Error : " + tempSlotInfoDatas[slotIndex].skillIndex + " 아이템임");
            }

            slotInfo.skillIndex = tempSlotInfoDatas[slotIndex].skillIndex; // 스킬 인덱스
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
            if (tempSlotInfoDatas[slotIndex].skillIndex > -1)
            {
                Debug.Log("Error : " + tempSlotInfoDatas[slotIndex].skillIndex + " 스킬임");
            }

            switch (tempSlotInfoDatas[slotIndex].itemType)
            {
                case TypeData.ItemType.장비:
                    {
                        slotInfo.itemIndex = tempSlotInfoDatas[slotIndex].equipmentIndex;
                        slotInfo.iconName = ItemData.Instance.equipmentInfos[slotInfo.itemIndex].name;
                        slotInfo.itemType = TypeData.ItemType.장비;
                    }
                    break;

                case TypeData.ItemType.소모품:
                    {
                        slotInfo.itemIndex = tempSlotInfoDatas[slotIndex].cusomableIndex;
                        slotInfo.iconName = ItemData.Instance.cusomableInfos[slotInfo.itemIndex].name;
                        slotInfo.itemType = TypeData.ItemType.소모품;
                    }
                    break;

                case TypeData.ItemType.퀘스트템:
                    {
                        slotInfo.itemIndex = tempSlotInfoDatas[slotIndex].materialIndex;
                        slotInfo.iconName = ItemData.Instance.questItemInfos[slotInfo.itemIndex].name;
                        slotInfo.itemType = TypeData.ItemType.퀘스트템;
                    }
                    break;

                default:
                    {

                    }
                    break;
            }

            slotInfo.slotInfoType = TypeData.SlotInfoType.아이템;
            slotInfo.quantity = tempSlotInfoDatas[slotIndex].quantity;
        }

        return true;
    }

    // 교환 (타겟이 있으면 교환 : 인벤>인벤 / 인벤>창고 / 창고>창고 / 창고>인벤 / 단축키>단축키)
    public void SwapSlotData(UISlotInfo currentInfo, UISlotInfo targetInfo)
    {
        if (!CheckSlotType(currentInfo.slotType, currentInfo.slotIndex, ref tempCurrentSlotInfoDatas))
        {
            Debug.Log("현재슬롯에 정보가 없음");
            return;
        }

        if (!CheckSlotType(targetInfo.slotType, targetInfo.slotIndex, ref tempTargetSlotInfoDatas))
        {
            Debug.Log("타겟슬롯에 정보가 없음");
            return;
        }

        SlotInfoData currentSlotInfoData = tempSlotInfoDatas[currentInfo.slotIndex];
        SlotInfoData targetSlotInfoData = tempSlotInfoDatas[targetInfo.slotIndex];

        tempSlotInfoDatas[currentInfo.slotIndex] = targetSlotInfoData;
        tempSlotInfoDatas[targetInfo.slotIndex] = currentSlotInfoData;
    }

    // 교체
    // 현재타입과 타겟타입이 같으면 타겟에 넘겨주고 현재는 제거 (인벤->인벤 / 단축->단축 / 창고->창고) - 이 경우는 현재와 타겟이 같고 타겟이 빈곳을 경우.
    // 현재타입과 타겟타입이 다르면 타겟 내용교체, 현재는 그대로 ( 인벤 -> 단축 / 스킬창 -> 단축 ) 
    public void ChangSlotData(UISlotInfo currentInfo, UISlotInfo targetInfo)
    {
        // 현재슬롯에 정보가 존재하는가 / ref로 정보 가져옴
        if (!CheckSlotType(currentInfo.slotType, currentInfo.slotIndex, ref tempCurrentSlotInfoDatas))
        {
            Debug.Log("현재슬롯에 정보가 없음");
            return;
        }
        // 타겟슬롯에 정보가 존재하는가 / ref로 정보 가져옴
        if (!CheckSlotType(targetInfo.slotType, targetInfo.slotIndex, ref tempTargetSlotInfoDatas))
        {
            Debug.Log("타겟슬롯에 정보가 없음");
            return;
        }

        // 이 경우는 현재와 타겟이 같고 타겟이 빈 곳을 경우.
        if (currentInfo.slotType == targetInfo.slotType)
        {
            tempTargetSlotInfoDatas[targetInfo.slotIndex] = tempCurrentSlotInfoDatas[currentInfo.slotIndex];
            tempCurrentSlotInfoDatas.Remove(currentInfo.slotIndex);
        }

        
    }

    // 복사 ( 인벤 -> 단축 / 스킬창 -> 단축 ) 빈곳
    public void CopySlotData(UISlotInfo currentInfo, UISlotInfo targetInfo)
    {

    }

    // 추가 - 아이템 습득?
    public void AddSlotData()
    {

    }

    // 합치기 - 같은 것이 있으면 결합(소모품, 퀘스트템)
    public void CombineSlotData(UISlotInfo currentInfo, UISlotInfo targetInfo)
    {
        // 현재슬롯에 정보가 존재하는가 / ref로 정보 가져옴
        if (!CheckSlotType(currentInfo.slotType, currentInfo.slotIndex, ref tempCurrentSlotInfoDatas))
        {
            Debug.Log("현재슬롯에 정보가 없음");
            return;
        }
        // 타겟슬롯에 정보가 존재하는가 / ref로 정보 가져옴
        if (!CheckSlotType(targetInfo.slotType, targetInfo.slotIndex, ref tempTargetSlotInfoDatas))
        {
            Debug.Log("타겟슬롯에 정보가 없음");
            return;
        }

        // 수량 검사
        CheckQuantity(currentInfo.slotIndex, targetInfo.slotIndex, currentInfo.slotInfo.quantity, targetInfo.slotInfo.quantity);
    }

    // 나누다, 분리 - 아이템 분리(소모품, 퀘스트템)
    // 타겟이 같은 아이템타입이면 다시 합치고, 다르면 취소, 빈곳이면 그곳에 넣기
    public void DivsionSlotData(UISlotInfo currentInfo, UISlotInfo targetInfo)
    {

    }

    // 제거
    public void RemoveSlotData(UISlotInfo currentInfo, UISlotInfo targetInfo)
    {

    }


}

