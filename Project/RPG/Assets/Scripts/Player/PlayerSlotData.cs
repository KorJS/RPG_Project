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
        public int qusetItemIndex;
        public int quantity;
    };

    public SlotInfoData slotInfoData;

    public Dictionary<int, SlotInfoData> inventoryInfos = null; // 인벤토리슬롯 정보
    public Dictionary<int, SlotInfoData> shortCutInfos = null;  // 단축슬롯 정보
    public Dictionary<int, SlotInfoData> storageInfos = null;   // 창고슬롯 정보

    public Dictionary<int, SlotInfoData> tempCurrentSlotInfoDatas = null;
    public Dictionary<int, SlotInfoData> tempTargetSlotInfoDatas= null;

    public Dictionary<int, UISlotInfo.SlotInfo> invenCusomableMark = null;
    public Dictionary<int, UISlotInfo.SlotInfo> invenQuestItemMark = null;
    public Dictionary<int, UISlotInfo.SlotInfo> storageCusomableMark = null;
    public Dictionary<int, UISlotInfo.SlotInfo> storageQuestItemMark = null;
    public Dictionary<int, UISlotInfo.SlotInfo> emptyInvenMark = null;
    public Dictionary<int, UISlotInfo.SlotInfo> emptyStorageMark = null;

    // TODO : Mark 만들자 아니면 더 좋은방법이 있을까.

    private UISlotInfo currentInfo = null;
    private UISlotInfo targetInfo = null;

    public PlayerSlotData()
    {
        inventoryInfos = new Dictionary<int, SlotInfoData>();
        shortCutInfos = new Dictionary<int, SlotInfoData>();
        storageInfos = new Dictionary<int, SlotInfoData>();

        invenCusomableMark = new Dictionary<int, UISlotInfo.SlotInfo>();
        invenQuestItemMark = new Dictionary<int, UISlotInfo.SlotInfo>();
        storageCusomableMark = new Dictionary<int, UISlotInfo.SlotInfo>();
        storageQuestItemMark = new Dictionary<int, UISlotInfo.SlotInfo>();

        emptyInvenMark = new Dictionary<int, UISlotInfo.SlotInfo>();
        emptyStorageMark = new Dictionary<int, UISlotInfo.SlotInfo>();

        slotInfoData.itemType = TypeData.ItemType.소모품;
        slotInfoData.skillIndex = -1;
        slotInfoData.equipmentIndex = -1;
        slotInfoData.cusomableIndex = 0;
        slotInfoData.qusetItemIndex = -1;
        slotInfoData.quantity = 80;
        inventoryInfos.Add(1, slotInfoData);

        slotInfoData.itemType = TypeData.ItemType.소모품;
        slotInfoData.skillIndex = -1;
        slotInfoData.equipmentIndex = -1;
        slotInfoData.cusomableIndex = 1;
        slotInfoData.qusetItemIndex = -1;
        slotInfoData.quantity = 20;
        inventoryInfos.Add(2, slotInfoData);

        slotInfoData.itemType = TypeData.ItemType.소모품;
        slotInfoData.skillIndex = -1;
        slotInfoData.equipmentIndex = -1;
        slotInfoData.cusomableIndex = 2;
        slotInfoData.qusetItemIndex = -1;
        slotInfoData.quantity = 10;
        inventoryInfos.Add(5, slotInfoData);

        slotInfoData.itemType = TypeData.ItemType.없음;
        slotInfoData.skillIndex = 0;
        slotInfoData.equipmentIndex = -1;
        slotInfoData.cusomableIndex = -1;
        slotInfoData.qusetItemIndex = -1;
        shortCutInfos.Add(7, slotInfoData);
    }

    // 인벤토리 정보 가져오기
    private bool CheckInventoryData(int slotIndex, ref Dictionary<int, SlotInfoData> slotInfoDatas)
    {
        // 해당 슬롯에 정보가 없으면 리턴
        if (!inventoryInfos.ContainsKey(slotIndex))
        {
            slotInfoDatas = inventoryInfos;
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
            slotInfoDatas = shortCutInfos;
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
            slotInfoDatas = storageInfos;
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
    private void CheckInvenQuantity(ref int currentQuantity, ref int targetQuantity, int quantityMAX)
    {
        int total = currentQuantity + targetQuantity;

        // 인벤토리만. 99 창고는 무제한.
        // 90(current) + 10(target) = 100 > MAX : 1(current) 99(target)
        // 10(current) + 92(target) = 102 > MAX : 3(current) 99(target)
        if (total > quantityMAX)
        {
            currentQuantity = total - quantityMAX;
            targetQuantity = quantityMAX;
        }
        // 2(current) + 90(target) = 92 <= MAX : 0(current) 92(target)
        // 9(current) + 90(target) = 99 <= MAX : 0(current) 99(target)
        else if (total <= quantityMAX)
        {
            currentQuantity = 0;
            targetQuantity = total;
        }
    }

    // 인벤>창고, 창고>인벤 이동할때 쓰일 것
    private void SetMark(TypeData.SlotType slotType, int slotIndex, UISlotInfo.SlotInfo slotInfo, ref Dictionary<int, UISlotInfo.SlotInfo> invenMark, ref Dictionary<int, UISlotInfo.SlotInfo> storageMark)
    {
        switch (slotType)
        {
            case TypeData.SlotType.인벤토리:
                {
                    // 이미 등록된게 있으면 지우고 재등록
                    if (invenMark.ContainsKey(slotIndex))
                    {
                        invenMark.Remove(slotIndex);
                    }

                    invenMark.Add(slotIndex, slotInfo);
                }
                break;

            case TypeData.SlotType.창고:
                {
                    // 이미 등록된게 있으면 지우고 재등록
                    if (storageMark.ContainsKey(slotIndex))
                    {
                        storageMark.Remove(slotIndex);
                    }

                    storageMark.Add(slotIndex, slotInfo);
                }
                break;
        }
    }

    // 슬롯에 변화가 생기면 슬롯의 정보로 재 설정 - 수량 변화
    public bool SetSlotData(TypeData.SlotType slotType, int slotIndex, ref UISlotInfo slotInfo)
    {
        if (!CheckSlotType(slotType, slotIndex, ref tempCurrentSlotInfoDatas))
        {
            Debug.Log("현재슬롯에 정보가 없음");
            return false;
        }

        // 슬롯이 0보다 적으면 데이터 삭제
        if (slotInfo.slotInfo.quantity <= 0)
        {
            RemoveSlotData(currentInfo);
            return true;
        }

        slotInfo.slotInfo.quantity = tempCurrentSlotInfoDatas[slotIndex].quantity;

        return true;
    }

    // 슬롯 필요한 정보에 맞게 변환하여 보냄 (인벤토리, 단축키, 창고 정보를 슬롯 정보에 맞게 변환)
    public bool GetSlotData(TypeData.SlotType slotType, int slotIndex, ref UISlotInfo.SlotInfo slotInfo)
    {
        bool isExist = false;

        isExist = CheckSlotType(slotType, slotIndex, ref tempCurrentSlotInfoDatas);
        
        // 정보가 없으면
        if (!isExist)
        {
            slotInfo = new UISlotInfo.SlotInfo();
            slotInfo.slotInfoType = TypeData.SlotInfoType.없음;
            slotInfo.itemType = TypeData.ItemType.없음;
            return false;
        }

        // 아이템 타입이 없으면 스킬이므로
        if (tempCurrentSlotInfoDatas[slotIndex].itemType == TypeData.ItemType.없음)
        {
            if (tempCurrentSlotInfoDatas[slotIndex].skillIndex < 0)
            {
                Debug.Log("Error : " + tempCurrentSlotInfoDatas[slotIndex].skillIndex + " 아이템임");
            }

            slotInfo.skillIndex = tempCurrentSlotInfoDatas[slotIndex].skillIndex; // 스킬 인덱스
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
            if (tempCurrentSlotInfoDatas[slotIndex].skillIndex > -1)
            {
                Debug.Log("Error : " + tempCurrentSlotInfoDatas[slotIndex].skillIndex + " 스킬임");
            }

            switch (tempCurrentSlotInfoDatas[slotIndex].itemType)
            {
                case TypeData.ItemType.장비:
                    {
                        slotInfo.itemIndex = tempCurrentSlotInfoDatas[slotIndex].equipmentIndex;
                        slotInfo.iconName = ItemData.Instance.equipmentInfos[slotInfo.itemIndex].name;
                        slotInfo.quantity = tempCurrentSlotInfoDatas[slotIndex].quantity;
                        slotInfo.itemType = TypeData.ItemType.장비;
                        slotInfo.slotInfoType = TypeData.SlotInfoType.아이템;
                    }
                    break;

                case TypeData.ItemType.소모품:
                    {
                        slotInfo.itemIndex = tempCurrentSlotInfoDatas[slotIndex].cusomableIndex;
                        slotInfo.iconName = ItemData.Instance.cusomableInfos[slotInfo.itemIndex].name;
                        slotInfo.quantity = tempCurrentSlotInfoDatas[slotIndex].quantity;
                        slotInfo.itemType = TypeData.ItemType.소모품;
                        slotInfo.slotInfoType = TypeData.SlotInfoType.아이템;

                        // 인벤토리이면서 수량이 99가 아닐때 마크 - 인벤 슬롯당 최대수량은 99
                        if (!((slotType == TypeData.SlotType.인벤토리) && (slotInfo.quantity >= 99)))
                        {
                            SetMark(slotType, slotIndex, slotInfo, ref invenCusomableMark, ref storageCusomableMark); // 소모품 마크 등록
                        }
                    }
                    break;

                case TypeData.ItemType.퀘스트템:
                    {
                        slotInfo.itemIndex = tempCurrentSlotInfoDatas[slotIndex].qusetItemIndex;
                        slotInfo.iconName = ItemData.Instance.questItemInfos[slotInfo.itemIndex].name;
                        slotInfo.quantity = tempCurrentSlotInfoDatas[slotIndex].quantity;
                        slotInfo.itemType = TypeData.ItemType.퀘스트템;
                        slotInfo.slotInfoType = TypeData.SlotInfoType.아이템;

                        SetMark(slotType, slotIndex, slotInfo, ref invenQuestItemMark, ref storageQuestItemMark);
                    }
                    break;

                default:
                    {
                        // 빈 곳 체크해두자
                        SetMark(slotType, slotIndex, slotInfo, ref emptyInvenMark, ref emptyStorageMark);
                    }
                    break;
            }
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

        // 타겟슬롯에 정보가 존재하는가 / ref로 정보 가져옴
        if (!CheckSlotType(targetInfo.slotType, targetInfo.slotIndex, ref tempTargetSlotInfoDatas))
        {
            Debug.Log("타겟슬롯에 정보가 없음");
            return;
        }

        SlotInfoData currentSlotInfoData = tempCurrentSlotInfoDatas[currentInfo.slotIndex];
        SlotInfoData targetSlotInfoData = tempTargetSlotInfoDatas[targetInfo.slotIndex];

        tempCurrentSlotInfoDatas[currentInfo.slotIndex] = targetSlotInfoData;
        tempTargetSlotInfoDatas[targetInfo.slotIndex] = currentSlotInfoData;
    }

    // 교체
    // 현재타입과 타겟타입이 같으면 타겟에 넘겨주고 현재는 제거 (인벤->인벤 / 단축->단축 / 창고->창고) - 이 경우는 현재와 타겟이 같고 타겟이 빈곳을 경우.
    // 현재타입과 타겟타입이 다르면 타겟 내용교체, 현재는 그대로 ( 인벤 -> 단축 / 스킬창 -> 단축 ) 
    public void ChangSlotData(UISlotInfo currentInfo,  UISlotInfo targetInfo)
    {
        // 현재슬롯에 정보가 존재하는가 / ref로 정보 가져옴
        if (!CheckSlotType(currentInfo.slotType, currentInfo.slotIndex, ref tempCurrentSlotInfoDatas))
        {
            Debug.Log("현재슬롯에 정보가 없음");
            return;
        }

        // 이 경우는 현재와 타겟이 같고 타겟이 빈 곳을 경우.
        if (currentInfo.slotType == targetInfo.slotType)
        {
            tempCurrentSlotInfoDatas[targetInfo.slotIndex] = tempCurrentSlotInfoDatas[currentInfo.slotIndex];
            tempCurrentSlotInfoDatas.Remove(currentInfo.slotIndex);

            return;
        }
        else
        {
            Debug.Log("현재슬롯타입 : " + currentInfo.slotType + ", 타겟슬롯타입 : " + targetInfo.slotType);
        }

        // 타겟슬롯의 정보 가져옴
        CheckSlotType(targetInfo.slotType, targetInfo.slotIndex, ref tempTargetSlotInfoDatas);


    }

    // 복사 ( 인벤 -> 단축 / 스킬창 -> 단축 ) 빈곳
    public void CopySlotData(UISlotInfo currentInfo, UISlotInfo targetInfo)
    {
        // 현재슬롯에 정보가 존재하는가 / ref로 정보 가져옴
        if (!CheckSlotType(currentInfo.slotType, currentInfo.slotIndex, ref tempCurrentSlotInfoDatas))
        {
            Debug.Log("현재슬롯에 정보가 없음");
            return;
        }

        // 타겟슬롯의 정보 가져옴
        CheckSlotType(targetInfo.slotType, targetInfo.slotIndex, ref tempTargetSlotInfoDatas);

        // 현재를 타겟에 복사
        tempTargetSlotInfoDatas[targetInfo.slotIndex] = tempCurrentSlotInfoDatas[currentInfo.slotIndex];
    }

    // 합치기 - 타겟이 있을때 같은 것이 있으면 결합(소모품, 퀘스트템) 인벤>인벤 / 창고>인벤
    public void CombineSlotData(UISlotInfo currentInfo, UISlotInfo targetInfo, int quantityMAX)
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

        int currentQuantity = currentInfo.slotInfo.quantity;
        int targetQuantity = targetInfo.slotInfo.quantity;

        CheckInvenQuantity(ref currentQuantity, ref targetQuantity, quantityMAX);

        SlotInfoData tempCurrentSlotInfoData = tempCurrentSlotInfoDatas[currentInfo.slotIndex];
        SlotInfoData tempTargetSlotInfoData = tempTargetSlotInfoDatas[targetInfo.slotIndex];

        tempCurrentSlotInfoData.quantity = currentQuantity;
        tempTargetSlotInfoData.quantity = targetQuantity;


        tempCurrentSlotInfoDatas[currentInfo.slotIndex] = tempCurrentSlotInfoData;
        tempTargetSlotInfoDatas[targetInfo.slotIndex] = tempTargetSlotInfoData;
    }

    // 나누다, 분리 - 아이템 분리(소모품, 퀘스트템)
    // 타겟이 같은 아이템타입이면 다시 합치고, 다르면 취소, 빈곳이면 그곳에 넣기
    public void DivisionSlotData(UISlotInfo currentInfo, int divQuantity)
    {
        switch (currentInfo.slotInfo.itemType)
        {
            case TypeData.ItemType.소모품:
                {
                    CheckMark(currentInfo, divQuantity, ref invenCusomableMark, ref storageCusomableMark);
                }
                break;

            case TypeData.ItemType.퀘스트템:
                {
                    CheckMark(currentInfo, divQuantity, ref invenQuestItemMark, ref storageQuestItemMark);
                }
                break;
        }
    }

    // 소모품, 퀘스트템 검사
    public void CheckMark(UISlotInfo currentInfo, int divQuantity, ref Dictionary<int, UISlotInfo.SlotInfo> invenMark, ref Dictionary<int, UISlotInfo.SlotInfo> storageMark)
    {
        // 현재슬롯에 정보가 존재하는가 / ref로 정보 가져옴
        if (!CheckSlotType(currentInfo.slotType, currentInfo.slotIndex, ref tempCurrentSlotInfoDatas))
        {
            Debug.Log("현재슬롯에 정보가 없음");
            return;
        }

        switch (currentInfo.slotType)
        {
            case TypeData.SlotType.인벤토리:
                {
                    SlotInfoData tempStorageSlotInfoData;

                    // 창고 - 수량 제한 없음 무족건 합침.
                    foreach (KeyValuePair<int, UISlotInfo.SlotInfo> storageSlotInfo in storageMark)
                    {
                        if (storageSlotInfo.Value.itemIndex == currentInfo.slotInfo.itemIndex)
                        {
                            int tempStorageSlotIndex = storageSlotInfo.Key;
                            tempStorageSlotInfoData = storageInfos[tempStorageSlotIndex];
                            tempStorageSlotInfoData.quantity += divQuantity;

                            storageInfos[tempStorageSlotIndex] = tempStorageSlotInfoData;

                            divQuantity = 0; // 다 합치고 나면 0개
                            GameObject.Find("W_Slot " + tempStorageSlotIndex).GetComponent<UISlotInfo>().ReSetting();
                            break;
                        }
                    }

                    // 위에서 다 합쳤으면 수량 0개이므로 리턴 
                    if (divQuantity <= 0)
                    {
                        return;
                    }

                    // 수량이 남아있으면 (같은 아이템이 아니므로)
                    foreach (KeyValuePair<int, UISlotInfo.SlotInfo> emptySlotInfo in emptyStorageMark)
                    {
                        int tempStorageSlotIndex = emptySlotInfo.Key;
                        // 만약에 빈곳이 아닐경우
                        if (!storageInfos.ContainsKey(emptySlotInfo.Key))
                        {
                            Debug.Log("에러 빈곳이 아님");
                            return;
                        }

                        // 현재 슬롯 정보와 똑같으니 정보를 임시변수에 가져와 수량 변화를 주고 등록.
                        tempStorageSlotInfoData = tempCurrentSlotInfoDatas[tempStorageSlotIndex];
                        tempStorageSlotInfoData.quantity = divQuantity;
                        storageInfos.Add(tempStorageSlotIndex, tempStorageSlotInfoData);
                        GameObject.Find("W_Slot " + tempStorageSlotIndex).GetComponent<UISlotInfo>().ReSetting();
                    }
                }
                break;

            case TypeData.SlotType.창고:
                {
                    SlotInfoData tempInvenSlotInfoData;
                    // 인벤
                    // 창고 - 수량 제한 없음 무족건 합침. - 순서대로 등록되어있겟지?..
                    foreach (KeyValuePair<int, UISlotInfo.SlotInfo> invenSlotInfo in invenMark)
                    {
                        if (invenSlotInfo.Value.itemIndex == currentInfo.slotInfo.itemIndex)
                        {
                            // 같은 곳에 수량이 99인지는 마크 등록할때 했음.

                            // 같은게 있으면 채우고 
                            // 남으면 다시 또 같은게 있으면 채우고.. 
                            int tempInvenSlotIndex = invenSlotInfo.Key;
                            tempInvenSlotInfoData = storageInfos[tempInvenSlotIndex];

                            int targetQuantity = tempInvenSlotInfoData.quantity;

                            CheckInvenQuantity(ref divQuantity, ref targetQuantity, 99);

                            tempInvenSlotInfoData.quantity = targetQuantity;

                            tempCurrentSlotInfoDatas[tempInvenSlotIndex] = tempInvenSlotInfoData;
                            GameObject.Find("I_Slot " + tempInvenSlotIndex).GetComponent<UISlotInfo>().ReSetting();
                        }
                    }

                    // 수량이 없으면 리턴
                    if (divQuantity <= 0)
                    {
                        return;
                    }
                    // 위에서 같은 아이템인덱스에 합치고도 남거나. 아예 같은 타입이 없으면
                    // 수량은 남아있으니.
                    foreach (KeyValuePair<int, UISlotInfo.SlotInfo> emptySlotInfo in emptyInvenMark)
                    {
                        // 수량이 없으면 멈춤
                        if (divQuantity <= 0)
                        {
                            break;
                        }

                        int tempInvenSlotIndex = emptySlotInfo.Key;

                        // 만약에 빈곳이 아닐경우
                        if (!storageInfos.ContainsKey(tempInvenSlotIndex))
                        {
                            Debug.Log("에러 빈곳이 아님");
                            return;
                        }

                        // (0이 될때까지) 추가하고도 수량이 남으면 또 다음 슬롯에 추가.
                        tempInvenSlotInfoData = tempCurrentSlotInfoDatas[tempInvenSlotIndex];

                        int targetQuantity = tempInvenSlotInfoData.quantity;

                        CheckInvenQuantity(ref divQuantity, ref targetQuantity, 99);

                        tempInvenSlotInfoData.quantity = targetQuantity;

                        storageInfos.Add(tempInvenSlotIndex, tempInvenSlotInfoData);
                        GameObject.Find("I_Slot " + tempInvenSlotIndex).GetComponent<UISlotInfo>().ReSetting();
                    }
                }
                break;
        }
    }

    // 추가 - 아이템 습득?
    public void AddSlotData()
    {

    }

    // 제거
    public void RemoveSlotData(UISlotInfo currentInfo)
    {
        // 현재슬롯에 정보가 존재하는가 / ref로 정보 가져옴
        if (!CheckSlotType(currentInfo.slotType, currentInfo.slotIndex, ref tempCurrentSlotInfoDatas))
        {
            Debug.Log("현재슬롯에 정보가 없음");
            return;
        }

        tempCurrentSlotInfoDatas.Remove(currentInfo.slotIndex);
    }
}