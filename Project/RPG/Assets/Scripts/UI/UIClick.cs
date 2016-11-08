using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIClick : MonoBehaviour
{
    private UIManager uiManager = null;
    private SkillData skillData = null;
    private PlayerInput playerInupt = null;
    private PlayerState playerState = null;
    private PlayerSlotData playerSlotData = null;
    private EquipmentHandler equipHandler = null;

    public UISlotInfo uiSlotInfo = null;
    private UISkillList uiSkillList = null;

    public bool isClick = false;

    void Awake()
    {
        uiSkillList = GameObject.FindGameObjectWithTag("SkillW").GetComponent<UISkillList>();
    }
    
    void Start()
    {
        uiManager = UIManager.Instance;
        skillData = SkillData.Instance;
        playerSlotData = PlayerSlotData.Instance;

        uiSlotInfo = GetComponent<UISlotInfo>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        playerInupt = playerObj.GetComponent<PlayerInput>();
        playerState = playerObj.GetComponent<PlayerState>();
        equipHandler = playerObj.GetComponent<EquipmentHandler>();
    }

    void OnClick()
    {
        // 마우스 왼클릭
        if (UICamera.currentKey == KeyCode.Mouse0)
        {
            SetSkillDescription();
        }

        // 마우스 우클릭
        if (UICamera.currentKey == KeyCode.Mouse1)
        {
            CheckSlotType();
        }
    }

    void OnPress(bool isPress)
    {
        if (uiSlotInfo.slotType == TypeData.SlotType.단축키)
        {
            isClick = isPress;
        }
    }

    private void CheckSlotType()
    {
        switch (uiSlotInfo.slotType)
        {
            case TypeData.SlotType.캐릭터:
                {
                    if (uiSlotInfo.slotIndex == 1)
                    {
                        equipHandler.SetWeapon(null, false);
                        equipHandler.SetSubWeapon(null, false);
                    }
                    playerSlotData.AddSlotData(TypeData.SlotType.인벤토리, uiSlotInfo.slotInfo.itemType, uiSlotInfo.slotInfo.itemIndex, 1);
                    playerSlotData.RemoveSlotData(uiSlotInfo);
                    uiManager.windowSettings.characterObj.GetComponent<UICharater>().ChangPlayerStat();
                    uiSlotInfo.ReSetting();
                }
                break;

            case TypeData.SlotType.인벤토리:
                {
                    InventorySlot();
                }
                break;

            case TypeData.SlotType.단축키:
                {
                    ShortCutSlot();
                }
                break;

            case TypeData.SlotType.창고:
                {
                    StorageSlot();
                }
                break;

            case TypeData.SlotType.상점리스트:
                {
                    if (uiSlotInfo.isItemExist)
                    {
                        uiManager.windowSettings.storeObj.GetComponent<UIStore>().CopySlotInfo(uiSlotInfo, TypeData.SlotType.구매, 1);
                    }
                }
                break;

            case TypeData.SlotType.구매:
            case TypeData.SlotType.판매:
                {
                    BuyAndSellSlot();
                }
                break;
        }
    }

    private void SetSkillDescription()
    {
        // 마우스 왼클릭이 아니면 리턴
        if (UICamera.currentKey != KeyCode.Mouse0)
        {
            return;
        }

        // 스킬 리스트 슬롯이 아니면 리턴
        if (uiSlotInfo.slotType != TypeData.SlotType.스킬리스트)
        {
            return;
        }

        int index = uiSlotInfo.slotInfo.skillIndex;

        SkillData.SkillInfo skillInfo = skillData.skillInfos[index];

        uiSkillList.SetDescription(skillInfo.iconName, skillInfo.name, skillInfo.description);
    }

    private void InventorySlot()
    {
        // 상점창이 열여있는 경우
        if (uiManager.windowSettings.storeObj.activeSelf)
        {
            // 클릭한 슬롯에 아이템이 없으면 리턴
            if (!uiSlotInfo.isItemExist)
            {
                return;
            }

            UIStore uiStore = uiManager.windowSettings.storeObj.GetComponent<UIStore>();

            uiStore.CopySlotInfo(uiSlotInfo, TypeData.SlotType.판매, uiSlotInfo.slotInfo.quantity);

            uiSlotInfo.slotInfo.quantity = 0;

            if (!uiStore.changInvenIndexs.Contains(uiSlotInfo.slotIndex))
            {
                uiStore.changInvenIndexs.Add(uiSlotInfo.slotIndex); // 수량 변화 생긴 슬롯 인덱스 저장 - 정산할때 그 슬롯들 갱신
            }
            Debug.Log(uiSlotInfo.slotIndex);
            uiSlotInfo.isItemExist = false;
            uiSlotInfo.StoreReSetting();
        }
        // 창고창이 열여있는 경우
        else if (uiManager.windowSettings.storageObj.activeSelf) // 인벤 > 창고 수량 전부 넣음
        {
            int quantity = 0;

            Debug.Log("itemType : " + uiSlotInfo.slotInfo.itemType);
            if (uiSlotInfo.slotInfo.itemType == TypeData.ItemType.장비) { quantity = 1; }
            // 소모품, 퀘스트템일경우
            else { quantity = uiSlotInfo.slotInfo.quantity; }
            Debug.Log("quantity : " + quantity);

            playerSlotData.AddSlotData(TypeData.SlotType.창고, uiSlotInfo.slotInfo.itemType, uiSlotInfo.slotInfo.itemIndex, uiSlotInfo.slotInfo.quantity);
            playerSlotData.RemoveSlotData(uiSlotInfo);
            uiSlotInfo.ReSetting();
        }
        // 케릭창이 열여있는 경우
        else if (uiManager.windowSettings.characterObj.activeSelf)
        {
            int index = uiSlotInfo.slotInfo.itemIndex;
            ItemData.EquipmentInfo tempEquipmentInfo = ItemData.Instance.equipmentInfos[index];

            int targetIndex = (int)tempEquipmentInfo.equipmentType;

            UISlotInfo targetInfo = uiManager.characterSlots[targetIndex];

            uiManager.windowSettings.characterObj.GetComponent<UICharater>().SetSlotInfo(uiSlotInfo, targetInfo);
            uiSlotInfo.ReSetting();
        }
        // 그냥 인벤만.
        else
        {
            // 쿨타임 중이면 리턴
            if (uiSlotInfo.isCoolTime)
            {
                return;
            }

            if (uiSlotInfo.slotInfo.itemType == TypeData.ItemType.소모품)
            {
                ShortcutItemCoolTime(false);

                // 소모품 사용
                uiSlotInfo.slotInfo.quantity -= 1;
                playerSlotData.SetSlotData(uiSlotInfo.slotType, uiSlotInfo.slotIndex, ref uiSlotInfo);
                uiSlotInfo.isCoolTime = true;
                ItemManager.Instance.CheckItemType((TypeData.ItemType)uiSlotInfo.slotInfo.itemType, uiSlotInfo.slotInfo.itemIndex, uiSlotInfo.isCoolTime);
            }
        }
    }

    private void ShortCutSlot()
    {
        // 쿨타임 중이면 리턴
        if (uiSlotInfo.isCoolTime)
        {
            return;
        }

        switch (uiSlotInfo.slotInfo.slotInfoType)
        {
            case TypeData.SlotInfoType.스킬:
                {
                    // 무기를 장착하지 않았으면 리턴
                    if (!uiManager.characterSlots[1].isItemExist)
                    {
                        uiManager.SetMessage("무기를 장착하세요.");
                        return;
                    }

                    playerInupt.index = uiSlotInfo.slotInfo.skillIndex;
                    playerInupt.tempKeyCode = uiSlotInfo.slotSettings.slotKeyCode;
                    playerInupt.isClick = isClick;

                    playerState.nextState = TypeData.State.스킬;
                    playerState.nextMode = TypeData.MODE.전투;

                    uiSlotInfo.isCoolTime = true;

                    ShortcutSKillCoolTime();
                }
                break;

            case TypeData.SlotInfoType.아이템:
                {
                    ShortcutItemCoolTime(true);
                    InventoryCoolTime(false);

                    uiSlotInfo.slotInfo.quantity -= 1;
                    playerSlotData.SetSlotData(uiSlotInfo.slotType, uiSlotInfo.slotIndex, ref uiSlotInfo);
                    uiSlotInfo.isCoolTime = true;
                    ItemManager.Instance.CheckItemType((TypeData.ItemType)uiSlotInfo.slotInfo.itemType, uiSlotInfo.slotInfo.itemIndex, uiSlotInfo.isCoolTime);
                }
                break;
        }
    }

    private void StorageSlot()
    {
        int quantity = 0;

        if (uiSlotInfo.slotInfo.itemType == TypeData.ItemType.장비) { quantity = 1; }
        // 소모품, 퀘스트템일경우
        else { quantity = uiSlotInfo.slotInfo.quantity; }

        playerSlotData.AddSlotData(TypeData.SlotType.인벤토리, uiSlotInfo.slotInfo.itemType, uiSlotInfo.slotInfo.itemIndex, uiSlotInfo.slotInfo.quantity);
        playerSlotData.RemoveSlotData(uiSlotInfo);
        uiSlotInfo.ReSetting();
    }

    private void BuyAndSellSlot()
    {
        UIStore uiStore = uiManager.windowSettings.storeObj.GetComponent<UIStore>();
        uiStore.CopySlotInfo(uiSlotInfo, TypeData.SlotType.없음, uiSlotInfo.slotInfo.quantity);

        if (!uiStore.changInvenIndexs.Contains(uiSlotInfo.slotIndex))
        {
            uiStore.changInvenIndexs.Add(uiSlotInfo.slotIndex); // 수량 변화 생긴 슬롯 인덱스 저장 - 정산할때 그 슬롯들 갱신
        }

        uiSlotInfo.isItemExist = false;
        uiSlotInfo.slotInfo.itemIndex = -1;
        uiSlotInfo.slotInfo.itemType = TypeData.ItemType.없음;
        uiSlotInfo.slotInfo.quantity = 0;
        uiSlotInfo.StoreReSetting();
    }

    private void ShortcutSKillCoolTime()
    {
        // 단축창에서 스킬 사용시 쿨타임. - 단축창 같은 스킬에도 같이 쿨타임 상태로
        foreach (KeyValuePair<int, UISlotInfo> shortCut in uiManager.shortCuts)
        {
            // 빈 슬롯인 경우 리턴
            if (!shortCut.Value.isItemExist)
            {
                continue;
            }

            // 자신은 제외
            if (shortCut.Key == uiSlotInfo.slotIndex)
            {
                continue;
            }

            // 같은 타입이 아니면 
            if (shortCut.Value.slotInfo.skillIndex != uiSlotInfo.slotInfo.skillIndex)
            {
                continue;
            }

            shortCut.Value.isCoolTime = true;
        }
    }

    private void ShortcutItemCoolTime(bool isShortcutSlot)
    {
        Dictionary<int, UISlotInfo> tempShortCuts = uiManager.shortCuts;
        // 단축창에서 스킬 사용시 쿨타임. - 단축창 같은 아이템도 쿨타임 상태로
        foreach (var index in tempShortCuts.Keys.ToList())
        {
            // 빈 슬롯인 경우 리턴
            if (!tempShortCuts[index].isItemExist)
            {
                return;
            }

            // 단축창에서 사용한 아이템일때 / 자신은 제외
            if (isShortcutSlot && (index == uiSlotInfo.slotIndex))
            {
                continue;
            }

            // 같은 타입이 아니면 
            if (tempShortCuts[index].slotInfo.itemType != uiSlotInfo.slotInfo.itemType)
            {
                continue;
            }

            // 같은 인덱스가 아니면
            if (tempShortCuts[index].slotInfo.itemIndex != uiSlotInfo.slotInfo.itemIndex)
            {
                continue;
            }

            tempShortCuts[index].slotInfo.quantity -= 1;
            Debug.Log(index);
            UISlotInfo tempUISlotInfo = tempShortCuts[index];
            playerSlotData.SetSlotData(tempShortCuts[index].slotType, tempShortCuts[index].slotIndex, ref tempUISlotInfo);
            tempShortCuts[index] = tempUISlotInfo;
            tempShortCuts[index].isCoolTime = true;
        }
    }

    private void InventoryCoolTime(bool isInventorySlot)
    {
        // 단축창에서 아이템 사용시 쿨타임. - 인벤 같은 아이템도 쿨타임 상태로
        foreach (KeyValuePair<int, UISlotInfo> invenSlot in uiManager.invenSlots)
        {
            // 빈 슬롯인 경우 리턴
            if (!invenSlot.Value.isItemExist)
            {
                continue;
            }

            // 인벤에서 사용한 아이템일때 / 자신은 제외
            if (isInventorySlot && (invenSlot.Key == uiSlotInfo.slotIndex))
            {
                continue;
            }

            // 같은 타입이 아니면 
            if (invenSlot.Value.slotInfo.itemType != uiSlotInfo.slotInfo.itemType)
            {
                continue;
            }

            // 같은 인덱스가 아니면
            if (invenSlot.Value.slotInfo.itemIndex != uiSlotInfo.slotInfo.itemIndex)
            {
                continue;
            }

            invenSlot.Value.slotInfo.quantity -= 1;

            UISlotInfo tempUISlotInfo = uiManager.invenSlots[invenSlot.Key];
            playerSlotData.SetSlotData(invenSlot.Value.slotType, invenSlot.Value.slotIndex, ref tempUISlotInfo);
            uiManager.invenSlots[invenSlot.Key] = tempUISlotInfo;

            invenSlot.Value.isCoolTime = true;
        }
    }
}
