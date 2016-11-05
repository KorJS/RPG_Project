using UnityEngine;
using System.Collections;

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
        if (!(UICamera.currentKey == KeyCode.Mouse0))
        {
            return;
        }

        // 스킬 리스트 슬롯이 아니면 리턴
        if (!(uiSlotInfo.slotType == TypeData.SlotType.스킬리스트))
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
            if (uiSlotInfo.slotInfo.itemType == TypeData.ItemType.소모품)
            {
                // 소모품 사용
            }
        }
    }

    private void ShortCutSlot()
    {
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
                }
                break;

            case TypeData.SlotInfoType.아이템:
                {
                    // 아이템 사용.
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
}
