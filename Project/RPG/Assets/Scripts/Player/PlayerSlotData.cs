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
        public int itemType;
        public int itemIndex;
        public int skillIndex;
        public int quantity;
    };

    public SlotInfoData slotInfoData;

    public Dictionary<int, SlotInfoData> characterInfos = null; // 케릭터슬롯 정보
    public Dictionary<int, SlotInfoData> inventoryInfos = null; // 인벤토리슬롯 정보
    public Dictionary<int, SlotInfoData> shortCutInfos = null;  // 단축슬롯 정보
    public Dictionary<int, SlotInfoData> storageInfos = null;   // 창고슬롯 정보
    public Dictionary<int, SlotInfoData> skillListInfos = null; // 스킬창 리스트 정보

    private Dictionary<int, SlotInfoData> tempCurrentSlotInfoDatas = null;
    private Dictionary<int, SlotInfoData> tempTargetSlotInfoDatas= null;

    private SortedDictionary<int, UISlotInfo.SlotInfo> invenCusomableMark = null;
    private SortedDictionary<int, UISlotInfo.SlotInfo> invenQuestItemMark = null;
    private SortedDictionary<int, UISlotInfo.SlotInfo> storageCusomableMark = null;
    private SortedDictionary<int, UISlotInfo.SlotInfo> storageQuestItemMark = null;

    private SortedDictionary<int, UISlotInfo.SlotInfo> shortCutMark = null;
    private SortedDictionary<int, UISlotInfo.SlotInfo> emptyInvenMark = null;
    private SortedDictionary<int, UISlotInfo.SlotInfo> emptyStorageMark = null;

    private SlotInfoData tempSlotInfoData;

    public PlayerSlotData()
    {
        characterInfos = new Dictionary<int, SlotInfoData>();
        inventoryInfos = new Dictionary<int, SlotInfoData>();
        shortCutInfos = new Dictionary<int, SlotInfoData>();
        storageInfos = new Dictionary<int, SlotInfoData>();
        skillListInfos = new Dictionary<int, SlotInfoData>();

        invenCusomableMark = new SortedDictionary<int, UISlotInfo.SlotInfo>();
        invenQuestItemMark = new SortedDictionary<int, UISlotInfo.SlotInfo>();
        storageCusomableMark = new SortedDictionary<int, UISlotInfo.SlotInfo>();
        storageQuestItemMark = new SortedDictionary<int, UISlotInfo.SlotInfo>();
        shortCutMark = new SortedDictionary<int, UISlotInfo.SlotInfo>();

        emptyInvenMark = new SortedDictionary<int, UISlotInfo.SlotInfo>();
        emptyStorageMark = new SortedDictionary<int, UISlotInfo.SlotInfo>();

        //slotInfoData.itemType = (int)TypeData.ItemType.소모품;
        //slotInfoData.skillIndex = -1;
        //slotInfoData.itemIndex = 0;
        //slotInfoData.quantity = 90;
        //inventoryInfos.Add(1, slotInfoData);

        //slotInfoData.itemType = (int)TypeData.ItemType.소모품;
        //slotInfoData.skillIndex = -1;
        //slotInfoData.itemIndex = 1;
        //slotInfoData.quantity = 20;
        //inventoryInfos.Add(2, slotInfoData);

        //slotInfoData.itemType = (int)TypeData.ItemType.소모품;
        //slotInfoData.skillIndex = -1;
        //slotInfoData.itemIndex = 0;
        //slotInfoData.quantity = 10;
        //inventoryInfos.Add(4, slotInfoData);

        //slotInfoData.itemType = (int)TypeData.ItemType.장비;
        //slotInfoData.skillIndex = -1;
        //slotInfoData.itemIndex = 0;
        //slotInfoData.quantity = 1;
        //inventoryInfos.Add(6, slotInfoData);

        //slotInfoData.itemType = (int)TypeData.ItemType.장비;
        //slotInfoData.skillIndex = -1;
        //slotInfoData.itemIndex = 1;
        //slotInfoData.quantity = 1;
        //inventoryInfos.Add(7, slotInfoData);

        //slotInfoData.itemType = (int)TypeData.ItemType.장비;
        //slotInfoData.skillIndex = -1;
        //slotInfoData.itemIndex = 2;
        //slotInfoData.quantity = 1;
        //inventoryInfos.Add(10, slotInfoData);

        //slotInfoData.itemType = (int)TypeData.ItemType.장비;
        //slotInfoData.skillIndex = -1;
        //slotInfoData.itemIndex = 3;
        //slotInfoData.quantity = 1;
        //inventoryInfos.Add(9, slotInfoData);

        //slotInfoData.itemType = (int)TypeData.ItemType.장비;
        //slotInfoData.skillIndex = -1;
        //slotInfoData.itemIndex = 4;
        //slotInfoData.quantity = 1;
        //inventoryInfos.Add(3, slotInfoData);

        //slotInfoData.itemType = (int)TypeData.ItemType.장비;
        //slotInfoData.skillIndex = -1;
        //slotInfoData.itemIndex = 5;
        //slotInfoData.quantity = 1;
        //inventoryInfos.Add(11, slotInfoData);

        //slotInfoData.itemType = (int)TypeData.ItemType.장비;
        //slotInfoData.skillIndex = -1;
        //slotInfoData.itemIndex = 6;
        //slotInfoData.quantity = 1;
        //inventoryInfos.Add(12, slotInfoData);

        //slotInfoData.itemType = (int)TypeData.ItemType.장비;
        //slotInfoData.skillIndex = -1;
        //slotInfoData.itemIndex = 7;
        //slotInfoData.quantity = 1;
        //inventoryInfos.Add(16, slotInfoData);

        //slotInfoData.itemType = (int)TypeData.ItemType.없음;
        //slotInfoData.skillIndex = 0;
        //slotInfoData.itemIndex = -1;
        //shortCutInfos.Add(7, slotInfoData);

        //slotInfoData.itemType = (int)TypeData.ItemType.소모품;
        //slotInfoData.skillIndex = -1;
        //slotInfoData.itemIndex = 2;
        //slotInfoData.quantity = 10;
        //storageInfos.Add(2, slotInfoData);

        //slotInfoData.itemType = (int)TypeData.ItemType.장비;
        //slotInfoData.skillIndex = -1;
        //slotInfoData.itemIndex = 0;
        //slotInfoData.quantity = 1;
        //storageInfos.Add(1, slotInfoData);

        slotInfoData.itemType = (int)TypeData.ItemType.없음;
        slotInfoData.skillIndex = 0;
        slotInfoData.itemIndex = -1;
        skillListInfos.Add(0, slotInfoData);

        slotInfoData.itemType = (int)TypeData.ItemType.없음;
        slotInfoData.skillIndex = 1;
        slotInfoData.itemIndex = -1;
        skillListInfos.Add(1, slotInfoData);

        slotInfoData.itemType = (int)TypeData.ItemType.없음;
        slotInfoData.skillIndex = 2;
        slotInfoData.itemIndex = -1;
        skillListInfos.Add(2, slotInfoData);

        slotInfoData.itemType = (int)TypeData.ItemType.없음;
        slotInfoData.skillIndex = 3;
        slotInfoData.itemIndex = -1;
        skillListInfos.Add(3, slotInfoData);

        slotInfoData.itemType = (int)TypeData.ItemType.없음;
        slotInfoData.skillIndex = 4;
        slotInfoData.itemIndex = -1;
        skillListInfos.Add(4, slotInfoData);

        //Network_Slot.Instance.RequestSaveSlot(TypeData.SlotType.캐릭터);
        //Network_Slot.Instance.RequestSaveSlot(TypeData.SlotType.단축키);
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

    // 스킬리스트 정보 가져오기
    private bool CheckSkillListData(int slotIndex, ref Dictionary<int, SlotInfoData> slotInfoDatas)
    {
        // 해당 슬롯에 정보가 없으면 리턴
        if (!skillListInfos.ContainsKey(slotIndex))
        {
            slotInfoDatas = skillListInfos;
            return false;
        }

        slotInfoDatas = skillListInfos;
        return true;
    }

    // 스킬리스트 정보 가져오기
    private bool CheckCharacterData(int slotIndex, ref Dictionary<int, SlotInfoData> slotInfoDatas)
    {
        // 해당 슬롯에 정보가 없으면 리턴
        if (!characterInfos.ContainsKey(slotIndex))
        {
            slotInfoDatas = characterInfos;
            return false;
        }

        slotInfoDatas = characterInfos;
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

            case TypeData.SlotType.스킬리스트:
                {
                    isExist = CheckSkillListData(slotIndex, ref slotInfoDatas);
                }
                break;

            case TypeData.SlotType.캐릭터:
                {
                    isExist = CheckCharacterData(slotIndex, ref slotInfoDatas);
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

    // 마크 지움
    private void RemoveMark(TypeData.SlotType slotType, int slotIndex, 
        ref SortedDictionary<int, UISlotInfo.SlotInfo> invenMark, ref SortedDictionary<int, UISlotInfo.SlotInfo> storageMark, ref SortedDictionary<int, UISlotInfo.SlotInfo> shortCutMark, string temp)
    {
        switch (slotType)
        {
            case TypeData.SlotType.인벤토리:
                {
                    Debug.Log(temp + " Inven Remove : " + slotType + " ok : " + slotIndex);
                    invenMark.Remove(slotIndex);
                }
                break;

            case TypeData.SlotType.창고:
                {
                    Debug.Log(temp + " Storage Remove : " + slotType + " ok : " + slotIndex);
                    storageMark.Remove(slotIndex);
                }
                break;

            case TypeData.SlotType.단축키:
                {
                    Debug.Log(temp + " ShortCut Remove : " + slotType + " ok : " + slotIndex);
                    shortCutMark.Remove(slotIndex);
                }
                break;
        }
    }
    
    // 마크 등록 : 인벤>창고, 창고>인벤 이동할때 쓰일 것
    private void SetMark(TypeData.SlotType slotType, int slotIndex, UISlotInfo.SlotInfo slotInfo, 
        ref SortedDictionary<int, UISlotInfo.SlotInfo> invenMark, ref SortedDictionary<int, UISlotInfo.SlotInfo> storageMark, ref SortedDictionary<int, UISlotInfo.SlotInfo> shortCutMark, string temp)
    {
        switch (slotType)
        {
            case TypeData.SlotType.인벤토리:
                {
                    if (invenMark.ContainsKey(slotIndex))
                    {
                        // TODO : 여기에 지우고 밑에 다시 추가하려고햇는데 빈번한 삭제 추가는 오류 발생함
                        //Debug.Log(temp + " Inven Exist : " + slotType + " ok : " + slotIndex);
                        return;
                    }
                    //Debug.Log(temp + " Inven Add : " + slotType + " ok : " + slotIndex);
                    invenMark.Add(slotIndex, slotInfo);
                }
                break;

            case TypeData.SlotType.창고:
                {
                    // 이미 등록된게 있으면 리턴
                    if (storageMark.ContainsKey(slotIndex))
                    {
                        //Debug.Log(temp + " Storage Exist : " + slotType + " ok : " + slotIndex);
                        return;
                    }
                    //Debug.Log(temp + " Storage Add : " + slotType + " ok : " + slotIndex);
                    storageMark.Add(slotIndex, slotInfo);
                }
                break;

            case TypeData.SlotType.단축키:
                {
                    if (shortCutMark.ContainsKey(slotIndex))
                    {
                        Debug.Log(temp + " ShortCut Exist : " + slotType + " ok : " + slotIndex);
                        return;
                    }
                    Debug.Log(temp + " ShortCut Add : " + slotType + " ok : " + slotIndex);
                    shortCutMark.Add(slotIndex, slotInfo);
                }
                break;
        }
    }

    // 장비, 소모품, 퀘스트템 정보 생성 (인벤>창고 / 창고>인벤 에 추가할때 슬롯 정보가 있어야되므로 생성)
    private void CreateSlotInfoData(TypeData.SlotType targetSlotType, TypeData.ItemType currentItemType, int currentItemIndex, int divQuantity, ref SlotInfoData tempSlotInfoData)
    {
        tempSlotInfoData.itemType = (int)currentItemType;
        tempSlotInfoData.skillIndex = -1;
        tempSlotInfoData.quantity = divQuantity;
        tempSlotInfoData.itemIndex = currentItemIndex;
    }

    // 소모품, 퀘스트템 검사
    private void CheckCAndQMark(TypeData.SlotType targetSlotType, TypeData.ItemType currentItemType,
        int currentItemIndex, int divQuantity, ref SortedDictionary<int, UISlotInfo.SlotInfo> invenMark, ref SortedDictionary<int, UISlotInfo.SlotInfo> storageMark)
    {
        // 타겟
        switch (targetSlotType)
        {
            case TypeData.SlotType.인벤토리:
                {
                    // 인벤
                    foreach (KeyValuePair<int, UISlotInfo.SlotInfo> invenSlotInfo in invenMark)
                    {
                        if (invenSlotInfo.Value.itemIndex != currentItemIndex)
                        {
                            continue;
                        }

                        // 수량이 없으면 멈춤
                        if (divQuantity <= 0)
                        {
                            break;
                        }

                        // 같은 곳에 수량이 99인지는 마크 등록할때 했음.
                        // 같은게 있으면 채우고
                        // 남으면 다시 또 같은게 있으면 채우고..
                        int tempInvenSlotIndex = invenSlotInfo.Key;
                        Debug.Log("tempInvenSlotIndex : " + tempInvenSlotIndex);

                        tempSlotInfoData = inventoryInfos[tempInvenSlotIndex];

                        // 수량이 99개 이면 다음꺼로.
                        if (inventoryInfos[tempInvenSlotIndex].quantity >= 99) { continue; }

                        //int targetQuantity = tempSlotInfoData.quantity;
                        Debug.Log("전 tempSlotInfoData.quantity : " + tempSlotInfoData.quantity);
                        CheckInvenQuantity(ref divQuantity, ref tempSlotInfoData.quantity, 99);
                        Debug.Log("후 tempSlotInfoData.quantity : " + tempSlotInfoData.quantity);
                        //tempSlotInfoData.quantity = targetQuantity;

                        inventoryInfos[tempInvenSlotIndex] = tempSlotInfoData;
                        GameObject.Find("I_Slot " + tempInvenSlotIndex).GetComponent<UISlotInfo>().ReSetting();
                    }

                    // 수량이 없으면 리턴
                    if (divQuantity <= 0)
                    {
                        return;
                    }

                    // 옴겨질 아이템 정보 만듬
                    CreateSlotInfoData(targetSlotType, currentItemType, currentItemIndex, divQuantity, ref tempSlotInfoData);
                    List<int> tempIndex = new List<int>();
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

                        // 만약에 빈곳이 아닐경우 리턴
                        if (inventoryInfos.ContainsKey(tempInvenSlotIndex))
                        {
                            Debug.Log("에러 빈곳이 아님");
                            return;
                        }

                        // (0이 될때까지) 추가하고도 수량이 남으면 또 다음 슬롯에 추가.
                        int targetQuantity = 0;

                        tempIndex.Add(tempInvenSlotIndex);

                        CheckInvenQuantity(ref divQuantity, ref targetQuantity, 99);
                        
                        tempSlotInfoData.quantity = targetQuantity;
                        inventoryInfos.Add(tempInvenSlotIndex, tempSlotInfoData); // 추가
                        GameObject.Find("I_Slot " + tempInvenSlotIndex).GetComponent<UISlotInfo>().ReSetting();
                    }

                    if (tempIndex.Count > 0)
                    {
                        for (int i = 0; i < tempIndex.Count; i++)
                        {
                            emptyInvenMark.Remove(tempIndex[i]);
                        }
                    }
                }
                break;

            case TypeData.SlotType.창고:
                {
                    // 창고 - 수량 제한 없음 무족건 합침.
                    foreach (KeyValuePair<int, UISlotInfo.SlotInfo> storageSlotInfo in storageMark)
                    {
                        if (storageSlotInfo.Value.itemIndex != currentItemIndex)
                        {
                            continue;
                        }

                        int tempStorageSlotIndex = storageSlotInfo.Key;
                        tempSlotInfoData = storageInfos[tempStorageSlotIndex];
                        tempSlotInfoData.quantity += divQuantity;
                        storageInfos[tempStorageSlotIndex] = tempSlotInfoData;

                        divQuantity = 0; // 다 합치고 나면 0개
                        GameObject.Find("W_Slot " + tempStorageSlotIndex).GetComponent<UISlotInfo>().ReSetting();

                        break;
                    }

                    // 위에서 다 합쳤으면 수량 0개이므로 리턴 
                    if (divQuantity <= 0)
                    {
                        return;
                    }

                    //옴길 아이템 정보
                    CreateSlotInfoData(targetSlotType, currentItemType, currentItemIndex, divQuantity, ref tempSlotInfoData);

                    // 같은 아이템이 아니므로
                    foreach (KeyValuePair<int, UISlotInfo.SlotInfo> emptySlotInfo in emptyStorageMark)
                    {
                        int tempStorageSlotIndex = emptySlotInfo.Key;
                        // 만약에 빈곳이 경우 아닐경우 리턴
                        if (storageInfos.ContainsKey(tempStorageSlotIndex))
                        {
                            Debug.Log("에러 빈곳이 아님");
                            return;
                        }

                        // 수량 변화후 등록
                        tempSlotInfoData.quantity = divQuantity;
                        emptyStorageMark.Remove(tempStorageSlotIndex);
                        storageInfos.Add(tempStorageSlotIndex, tempSlotInfoData); // 추가
                        GameObject.Find("W_Slot " + tempStorageSlotIndex).GetComponent<UISlotInfo>().ReSetting();

                        break;
                    }
                }
                break;
        }
    }

    // 장비 검사
    private void CheckEquipmentMark(TypeData.SlotType targetSlotType, TypeData.ItemType currentItemType,
        int currentItemIndex, int divQuantity)
    {
        // 옴겨길 템 정보
        CreateSlotInfoData(targetSlotType, currentItemType, currentItemIndex, divQuantity, ref tempSlotInfoData);
        Debug.Log("currentItemIndex : " + currentItemIndex);
        switch (targetSlotType)
        {
            case TypeData.SlotType.인벤토리:
                {
                    foreach (KeyValuePair<int, UISlotInfo.SlotInfo> emptySlotInfo in emptyInvenMark)
                    {
                        // 빈곳이겟지?
                        int tempSlotIndex = emptySlotInfo.Key;
                        Debug.Log("tempSlotInfoData : " + tempSlotInfoData.itemIndex);
                        tempSlotInfoData.quantity = divQuantity;
                        emptyInvenMark.Remove(tempSlotIndex); // 이제 빈곳이 마크에서 아니므로 제거
                        inventoryInfos.Add(tempSlotIndex, tempSlotInfoData); // 추가
                        GameObject.Find("I_Slot " + tempSlotIndex).GetComponent<UISlotInfo>().ReSetting();
                        break;
                    }
                }
                break;
            case TypeData.SlotType.창고:
                {
                    foreach (KeyValuePair<int, UISlotInfo.SlotInfo> emptySlotInfo in emptyStorageMark)
                    {
                        // 빈곳이겟지?
                        int tempSlotIndex = emptySlotInfo.Key;

                        tempSlotInfoData.quantity = divQuantity;
                        emptyStorageMark.Remove(tempSlotIndex); // 이제 빈곳이 마크에서 아니므로 제거
                        storageInfos.Add(tempSlotIndex, tempSlotInfoData); // 추가
                        GameObject.Find("W_Slot " + tempSlotIndex).GetComponent<UISlotInfo>().ReSetting();
                        break;
                    }
                }
                break;
        }

    }
    
    // 단축창 수량 검사(소모품, 퀘스트템)
    private void CheckShorCutMark(TypeData.SlotType slotType, UISlotInfo slotInfo)
    {
        // 만약에 장비인 경우
        if (slotInfo.slotInfo.itemType == TypeData.ItemType.장비)
        {
            return;
        }

        // 단축창에서 수량 변화가 생기면 인벤에도 변화를 / 인벤에서 수량 변화가 생기면 단축창에도 변화를
        int quantity = 0;

        switch (slotType)
        {
            // 인벤에서 수량 변화가 생기면 단축창에도 변화를 > 아이템 습득, 소모
            case TypeData.SlotType.인벤토리:
                {
                    foreach (KeyValuePair<int, UISlotInfo.SlotInfo> shorCutInfo in shortCutMark)
                    {
                        // 같은 타입이 아니면 
                        if (slotInfo.slotInfo.itemType != shorCutInfo.Value.itemType)
                        {
                            continue;
                        }

                        // 같은 인덱스가 아니면
                        if (slotInfo.slotInfo.itemIndex != shorCutInfo.Value.itemIndex)
                        {
                            continue;
                        }

                        // 같은 아이템타입에 같은 아이텝인덱스인경우 - 인벤에 아이템에 수량을 전부 알아옴
                        if (slotInfo.slotInfo.itemType == TypeData.ItemType.소모품)
                        {
                            quantity = CheckInvenQuantity(slotInfo.slotInfo.itemIndex, invenCusomableMark);
                            Debug.Log("? " + quantity);

                        }
                        else if (slotInfo.slotInfo.itemType == TypeData.ItemType.퀘스트템)
                        {
                            quantity = CheckInvenQuantity(slotInfo.slotInfo.itemIndex, invenQuestItemMark);
                        }

                        // 단축창에 수량 변화시켜준다.
                        int tempShorCutIndex = shorCutInfo.Key;
                        SlotInfoData tempSlotInfoData = shortCutInfos[tempShorCutIndex];
                        tempSlotInfoData.quantity = quantity;
                        shortCutInfos[tempShorCutIndex] = tempSlotInfoData;

                        UISlotInfo uislotInfo = GameObject.Find("H_Slot " + tempShorCutIndex).GetComponent<UISlotInfo>();

                        if (quantity == 0)
                        {
                            RemoveSlotData(uislotInfo);
                            Network_Slot.Instance.RequestDeleteSlot(uislotInfo.slotType, uislotInfo.slotIndex);
                            uislotInfo.ReSetting();
                            return;
                        }

                        uislotInfo.ReSetting();
                    }
                }
                break;

            // 단축창에서 수량 변화가 생기면 인벤에도 변화를 > 아이템 소모
            case TypeData.SlotType.단축키:
                {
                    List<int> invenIndex = new List<int>();

                    if (slotInfo.slotInfo.itemType == TypeData.ItemType.소모품)
                    {
                        foreach (KeyValuePair<int, UISlotInfo.SlotInfo> cusomableInfo in invenCusomableMark)
                        {
                            if (slotInfo.slotInfo.itemIndex != cusomableInfo.Value.itemIndex)
                            {
                                continue;
                            }

                            int index = cusomableInfo.Key;

                            invenIndex.Add(index);

                            quantity += inventoryInfos[index].quantity;
                        }
                    }
                    else if (slotInfo.slotInfo.itemType == TypeData.ItemType.퀘스트템)
                    {
                        foreach (KeyValuePair<int, UISlotInfo.SlotInfo> questItemInfo in invenQuestItemMark)
                        {
                            if (slotInfo.slotInfo.itemIndex != questItemInfo.Value.itemIndex)
                            {
                                continue;
                            }

                            int index = questItemInfo.Key;

                            invenIndex.Add(index);

                            quantity += inventoryInfos[index].quantity;
                        }
                    }

                    // 같은 아이템이 하나도 없으면 리턴
                    if (invenIndex.Count == 0)
                    {
                        return;
                    }

                    // 단축창의 수량하고 인벤에 수량합계랑 다르면
                    if (slotInfo.slotInfo.quantity != quantity)
                    {
                        // 단축 수량이 변화가 생긴거니깐.
                        // 인벤수량 - 단축수량
                        int resultQuantity = quantity - slotInfo.slotInfo.quantity;

                        // 수량이 적은 곳 부터 수량 감소
                        for (int i = invenIndex.Count - 1; i >= 0; i--)
                        {
                            if (resultQuantity <= 0)
                            {
                                break;
                            }

                            int index = invenIndex[i];

                            SlotInfoData tempSlotInfoData = inventoryInfos[index];
                            Debug.Log(tempSlotInfoData.quantity);
                            
                            // 소모3 슬롯2 > if문으로 > 소모1 슬롯99 > else문으로 > 소모 0 끝. 
                            // 소모된 수량보다 슬롯에 수량이 작거나 큰경우
                            if (tempSlotInfoData.quantity <= resultQuantity)
                            {
                                // 소모된 수량 조정후
                                resultQuantity -= tempSlotInfoData.quantity;

                                // 슬롯 제거
                                UISlotInfo uiSlotInfo = GameObject.Find("I_Slot " + index).GetComponent<UISlotInfo>();
                                RemoveSlotData(uiSlotInfo);
                                Network_Slot.Instance.RequestDeleteSlot(slotType, index);
                                uiSlotInfo.ReSetting();
                            }
                            // 소모된 수량보다 슬롯에 수량이 큰 경우
                            else
                            {
                                tempSlotInfoData.quantity -= resultQuantity;
                                resultQuantity = 0;
                                inventoryInfos[index] = tempSlotInfoData;
                                GameObject.Find("I_Slot " + index).GetComponent<UISlotInfo>().ReSetting();
                            }
                        }
                    }

                    invenIndex.Clear();
                }
                break;
        }
    }

    // 인벤>단축 소모품이나 퀘스트템 복사시 인벤에 똑같은 템이 다수 존재 하면 수량을 단축창에 모두 표시.
    private int CheckInvenQuantity(int currentItemIndex, SortedDictionary<int, UISlotInfo.SlotInfo> invenMark)
    {
        int quantity = 0;

        foreach (KeyValuePair<int, UISlotInfo.SlotInfo> invenInfo in invenMark)
        {
            if (currentItemIndex == invenInfo.Value.itemIndex)
            {
                int index = invenInfo.Key;
                quantity += inventoryInfos[index].quantity;
            }
        }

        return quantity;
    }

    // 슬롯에 변화가 생기면 슬롯의 정보로 재 설정 - 수량 변화
    public bool SetSlotData(TypeData.SlotType slotType, int slotIndex, ref UISlotInfo slotInfo)
    {
        if (!CheckSlotType(slotType, slotIndex, ref tempCurrentSlotInfoDatas))
        {
            Debug.Log("현재슬롯에 정보가 없음");
            return false;
        }

        // 슬롯정보에 있는 수량을 주인공데이터의 수량에 등록
        SlotInfoData tempSlotInfoData = tempCurrentSlotInfoDatas[slotIndex];
        tempSlotInfoData.quantity = slotInfo.slotInfo.quantity;
        tempCurrentSlotInfoDatas[slotIndex] = tempSlotInfoData;

        // 슬롯이 0보다 적으면 데이터 삭제
        if (slotInfo.slotInfo.quantity <= 0)
        {
            // 인벤, 단축창 수량 변화
            CheckShorCutMark(slotType, slotInfo);

            RemoveSlotData(slotInfo);
            Network_Slot.Instance.RequestDeleteSlot(slotInfo.slotType, slotInfo.slotIndex);

            return true;
        }


        // 인벤, 단축창 수량 변화
        CheckShorCutMark(slotType, slotInfo);

        return true;
    }

    // 슬롯 필요한 정보에 맞게 변환하여 보냄 (인벤토리, 단축키, 창고 정보를 슬롯 정보에 맞게 변환)
    public bool GetSlotData(TypeData.SlotType slotType, int slotIndex, ref UISlotInfo.SlotInfo slotInfo)
    {
        bool isExist = false;

        isExist = CheckSlotType(slotType, slotIndex, ref tempCurrentSlotInfoDatas);
        Debug.Log(tempCurrentSlotInfoDatas.Count);
        // 정보가 없으면
        if (!isExist)
        {
            slotInfo = new UISlotInfo.SlotInfo();
            slotInfo.slotInfoType = TypeData.SlotInfoType.없음;
            slotInfo.itemType = TypeData.ItemType.없음;

            if ((slotType == TypeData.SlotType.인벤토리) || (slotType == TypeData.SlotType.창고))
            {
                // 인벤, 창고 빈 곳 체크해두자
                SetMark(slotType, slotIndex, slotInfo, ref emptyInvenMark, ref emptyStorageMark, ref shortCutMark, "빈곳");
            }
            return false;
        }

        if (slotType == TypeData.SlotType.캐릭터)
        {
            // slotIndex : 1 - 무기 / 2 - 갑옷 / 3 - 장갑 / 4 - 신발
            slotInfo.itemIndex = tempCurrentSlotInfoDatas[slotIndex].itemIndex;
            slotInfo.itemType = TypeData.ItemType.장비;
            slotInfo.iconName = ItemData.Instance.equipmentInfos[slotInfo.itemIndex].iconName;
            slotInfo.name = ItemData.Instance.equipmentInfos[slotInfo.itemIndex].name;
            slotInfo.quantity = 1;

            return true;
        }

        // 아이템 타입이 없으면 스킬이므로
        if ((TypeData.ItemType)tempCurrentSlotInfoDatas[slotIndex].itemType == TypeData.ItemType.없음)
        {
            if (tempCurrentSlotInfoDatas[slotIndex].skillIndex < 0)
            {
                Debug.Log("Error : " + tempCurrentSlotInfoDatas[slotIndex].skillIndex + " 아이템임");
            }

            slotInfo.skillIndex = tempCurrentSlotInfoDatas[slotIndex].skillIndex; // 스킬 인덱스
            slotInfo.itemIndex = -1;
            slotInfo.name = SkillData.Instance.skillInfos[slotInfo.skillIndex].name; // 스킬 이름
            slotInfo.iconName = SkillData.Instance.skillInfos[slotInfo.skillIndex].iconName; // 스킬 아이콘 이름
            slotInfo.coolTime = SkillData.Instance.skillInfos[slotInfo.skillIndex].coolTime;
            slotInfo.slotInfoType = TypeData.SlotInfoType.스킬;
            slotInfo.itemType = TypeData.ItemType.없음;

            if (slotType == TypeData.SlotType.스킬리스트)
            {
                return PlayerSkillData.Instance.GetSkillData(slotIndex);
            }
        }
        else
        {
            if (tempCurrentSlotInfoDatas[slotIndex].skillIndex > -1)
            {
                Debug.Log("Error : " + tempCurrentSlotInfoDatas[slotIndex].skillIndex + " 스킬임");
            }
            slotInfo.skillIndex = -1;
            switch ((TypeData.ItemType)tempCurrentSlotInfoDatas[slotIndex].itemType)
            {
                case TypeData.ItemType.장비:
                    {
                        slotInfo.itemIndex = tempCurrentSlotInfoDatas[slotIndex].itemIndex;
                        slotInfo.name = ItemData.Instance.equipmentInfos[slotInfo.itemIndex].name;
                        slotInfo.iconName = ItemData.Instance.equipmentInfos[slotInfo.itemIndex].iconName;
                        slotInfo.quantity = tempCurrentSlotInfoDatas[slotIndex].quantity;
                        slotInfo.itemType = TypeData.ItemType.장비;
                        slotInfo.slotInfoType = TypeData.SlotInfoType.아이템;
                    }
                    break;

                case TypeData.ItemType.소모품:
                    {
                        slotInfo.itemIndex = tempCurrentSlotInfoDatas[slotIndex].itemIndex;
                        slotInfo.name = ItemData.Instance.cusomableInfos[slotInfo.itemIndex].name;
                        slotInfo.iconName = ItemData.Instance.cusomableInfos[slotInfo.itemIndex].iconName;
                        slotInfo.quantity = tempCurrentSlotInfoDatas[slotIndex].quantity;
                        slotInfo.itemType = TypeData.ItemType.소모품;
                        slotInfo.slotInfoType = TypeData.SlotInfoType.아이템;

                        SetMark(slotType, slotIndex, slotInfo, ref invenCusomableMark, ref storageCusomableMark, ref shortCutMark, "소모품"); // 소모품 마크 등록
                    }
                    break;

                case TypeData.ItemType.퀘스트템:
                    {
                        slotInfo.itemIndex = tempCurrentSlotInfoDatas[slotIndex].itemIndex;
                        slotInfo.name = ItemData.Instance.questItemInfos[slotInfo.itemIndex].name;
                        slotInfo.iconName = ItemData.Instance.questItemInfos[slotInfo.itemIndex].iconName;
                        slotInfo.quantity = tempCurrentSlotInfoDatas[slotIndex].quantity;
                        slotInfo.itemType = TypeData.ItemType.퀘스트템;
                        slotInfo.slotInfoType = TypeData.SlotInfoType.아이템;

                        SetMark(slotType, slotIndex, slotInfo, ref invenQuestItemMark, ref storageQuestItemMark, ref shortCutMark, "퀘템"); // 퀘템 마크 등록
                    }
                    break;
            }
        }

        return true;
    }

    // 교환 (타겟이 있으면 교환 : 인벤>인벤 / 인벤>창고 / 인벤>케릭 / 창고>창고 / 창고>인벤 / 단축키>단축키)
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
    // 인벤>케릭 - 타겟슬롯타입이 케릭이면 케릭창에 넣고 현재는 제거
    // 현재슬롯타입과 타겟슬롯타입이 같으면 타겟에 넘겨주고 현재는 제거 (인벤->인벤 / 단축->단축 / 창고->창고) - 이 경우는 현재와 타겟이 같고 타겟이 빈곳을 경우.
    // 현재슬롯타입과 타겟슬롯타입이 다르면 타겟 내용교체, 현재는 그대로 ( 인벤 -> 단축 / 스킬창 -> 단축 ) 
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

            if (currentInfo.slotType == TypeData.SlotType.단축키)
            {
                RemoveMark(currentInfo.slotType, currentInfo.slotIndex, ref invenCusomableMark, ref storageCusomableMark, ref shortCutMark, "단축제거");
                return;
            }

            // 버그 - 갱신 전이라서 수량이 그대로..
            // 타겟으로 교체하면 현재는 빈곳이므로 inven, storage Mark에서 지우자
            if (currentInfo.slotInfo.itemType == TypeData.ItemType.소모품)
            {
                RemoveMark(currentInfo.slotType, currentInfo.slotIndex, ref invenCusomableMark, ref storageCusomableMark, ref shortCutMark, "소모품제거");
            }
            else if (currentInfo.slotInfo.itemType == TypeData.ItemType.퀘스트템)
            {
                RemoveMark(currentInfo.slotType, currentInfo.slotIndex, ref invenQuestItemMark, ref storageQuestItemMark, ref shortCutMark, "퀘템제거");
            }
            return;
        }
        
        // 인벤 > 단축 / 스킬창 > 단축 / 인벤 > 케릭
        // 타겟슬롯의 정보 가져옴
        CheckSlotType(targetInfo.slotType, targetInfo.slotIndex, ref tempTargetSlotInfoDatas);

        tempTargetSlotInfoDatas[targetInfo.slotIndex] = tempCurrentSlotInfoDatas[currentInfo.slotIndex];

        if (currentInfo.slotType == TypeData.SlotType.인벤토리 && targetInfo.slotType == TypeData.SlotType.캐릭터)
        {
            tempCurrentSlotInfoDatas.Remove(currentInfo.slotIndex);
        }
    }

    // 복사 ( 인벤 > 단축 / 스킬창 > 단축) 빈곳
    public void CopySlotData(UISlotInfo currentInfo, UISlotInfo targetInfo)
    {
        // 현재슬롯에 정보가 존재하는가 / ref로 정보 가져옴
        if (!CheckSlotType(currentInfo.slotType, currentInfo.slotIndex, ref tempCurrentSlotInfoDatas))
        {
            Debug.Log("현재슬롯에 정보가 없음");
            return;
        }

        if (currentInfo.slotInfo.itemType == TypeData.ItemType.장비)
        {
            Debug.Log("장비 입니다. 단축 슬롯에 넣을 수 없습니다.");
            return;
        }

        // 타겟슬롯의 정보 가져옴
        CheckSlotType(targetInfo.slotType, targetInfo.slotIndex, ref tempTargetSlotInfoDatas);

        int quantity = 0;

        if (currentInfo.slotInfo.itemType == TypeData.ItemType.소모품)
        {
            quantity = CheckInvenQuantity(currentInfo.slotInfo.itemIndex, invenCusomableMark);
        }
        else if (currentInfo.slotInfo.itemType == TypeData.ItemType.퀘스트템)
        {
            quantity = CheckInvenQuantity(currentInfo.slotInfo.itemIndex, invenQuestItemMark);
        }

        SlotInfoData tempSlotInfoData = tempCurrentSlotInfoDatas[currentInfo.slotIndex];
        tempSlotInfoData.quantity = quantity;
        // 현재를 타겟에 복사
        tempTargetSlotInfoDatas[targetInfo.slotIndex] = tempSlotInfoData;
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

        // 옴기고 현재 수량이 0개이면 슬롯 제거
        if (currentQuantity <= 0)
        {
            RemoveSlotData(currentInfo);
            Network_Slot.Instance.RequestDeleteSlot(currentInfo.slotType, currentInfo.slotIndex);
        }
    }

    // 나누다, 분리 - 아이템 분리(소모품, 퀘스트템)
    // 타겟이 같은 아이템타입이면 다시 합치고, 다르면 취소, 빈곳이면 순차대로 
    public void DivisionSlotData(TypeData.SlotType targetSlotType, TypeData.ItemType currentItemType, int currentItemIndex, int divQuantity)
    {
        switch (currentItemType)
        {
            case TypeData.ItemType.소모품:
                {
                    CheckCAndQMark(targetSlotType, currentItemType, currentItemIndex, divQuantity, ref invenCusomableMark, ref storageCusomableMark);
                }
                break;

            case TypeData.ItemType.퀘스트템:
                {
                    CheckCAndQMark(targetSlotType,  currentItemType, currentItemIndex, divQuantity, ref invenQuestItemMark, ref storageQuestItemMark);
                }
                break;
        }
    }

    // 추가 - 아이템 습득(장비, 소모품, 퀘스트템) / 인벤>창고(장비) / 창고>인벤(장비) / 케릭>인벤. 안벤에 추가 되면 단축키에 복사된거면 같이 수량증가.(소모품, 퀘스트템)
    public void AddSlotData(TypeData.SlotType targetSlotType, TypeData.ItemType currentItemType, int currentItemIndex, int divQuantity)
    {
        if (currentItemType == TypeData.ItemType.장비)
        {
            Debug.Log("playerSlotData Add");
            CheckEquipmentMark(targetSlotType, currentItemType, currentItemIndex, divQuantity);
            return;
        }

        if (currentItemType == TypeData.ItemType.소모품)
        {
            CheckCAndQMark(targetSlotType, currentItemType, currentItemIndex, divQuantity, ref invenCusomableMark, ref storageCusomableMark);
            return;
        }

        if (currentItemType == TypeData.ItemType.퀘스트템)
        {
            CheckCAndQMark(targetSlotType, currentItemType, currentItemIndex, divQuantity, ref invenQuestItemMark, ref storageQuestItemMark);
            return;
        }
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

        if (currentInfo.slotType == TypeData.SlotType.단축키)
        {
            if (currentInfo.slotInfo.skillIndex == -1)
            {
                RemoveMark(currentInfo.slotType, currentInfo.slotIndex, ref invenCusomableMark, ref storageCusomableMark, ref shortCutMark, "단축제거");
            }
            return;
        }

        // 타겟으로 교체하면 현재는 빈곳이므로 inven, storage Mark에서 지우자
        if (currentInfo.slotInfo.itemType == TypeData.ItemType.소모품)
        {
            // 여기는 제거이기때문에 인벤토리에 수량이 남아있을경우가 없음.
            RemoveMark(currentInfo.slotType, currentInfo.slotIndex, ref invenCusomableMark, ref storageCusomableMark, ref shortCutMark, "소모품제거");
        }
        else if (currentInfo.slotInfo.itemType == TypeData.ItemType.퀘스트템)
        {
            RemoveMark(currentInfo.slotType, currentInfo.slotIndex, ref invenQuestItemMark, ref storageQuestItemMark, ref shortCutMark, "퀘템제거");
        }
    }
}