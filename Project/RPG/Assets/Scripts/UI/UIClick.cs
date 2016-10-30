using UnityEngine;
using System.Collections;

public class UIClick : MonoBehaviour
{
    private UIManager uiManager = null;
    private SkillData skillData = null;
    private PlayerInput playerInupt = null;
    private PlayerSlotData playerSlotData = null;

    public UISlotInfo uiSlotInfo = null;
    private UISkillList uiSkillList = null;

    void Awake()
    {
        uiSkillList = GameObject.FindGameObjectWithTag("SkillW").GetComponent<UISkillList>();
    }
    
    void Start()
    {
        uiManager = UIManager.Instance;
        skillData = SkillData.Instance;
        playerSlotData = PlayerSlotData.Instance;
        playerInupt = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        uiSlotInfo = GetComponent<UISlotInfo>();
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
        uiSlotInfo.ReSetting();
    }

    private void CheckSlotType()
    {
        switch (uiSlotInfo.slotType)
        {
            case TypeData.SlotType.캐릭터:
                {
                    playerSlotData.AddSlotData(TypeData.SlotType.인벤토리, uiSlotInfo.slotInfo.itemType, uiSlotInfo.slotInfo.itemIndex, 1);
                    playerSlotData.RemoveSlotData(uiSlotInfo);
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
                    uiManager.windowSettings.storeObj.GetComponent<UIStore>().CopySlotInfo(uiSlotInfo, TypeData.SlotType.구매, 1);
                }
                break;

            case TypeData.SlotType.구매:
            case TypeData.SlotType.판매:
                {
                    uiManager.windowSettings.storeObj.GetComponent<UIStore>().CopySlotInfo(uiSlotInfo, TypeData.SlotType.없음, 1);
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
        if (uiManager.windowSettings.storeObj.activeSelf)
        {
            uiManager.windowSettings.storeObj.GetComponent<UIStore>().CopySlotInfo(uiSlotInfo, TypeData.SlotType.판매, uiSlotInfo.slotInfo.quantity);
        }
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
        }
        else if (uiManager.windowSettings.characterObj.activeSelf)
        {
            int index = uiSlotInfo.slotInfo.itemIndex;
            ItemData.EquipmentInfo tempEquipmentInfo = ItemData.Instance.equipmentInfos[index];

            int targetIndex = (int)tempEquipmentInfo.equipmentType;

            UISlotInfo targetInfo = uiManager.characterSlots[targetIndex];

            uiManager.windowSettings.characterObj.GetComponent<UICharater>().CheckSlotInfo(uiSlotInfo, targetInfo);
        }
    }

    private void ShortCutSlot()
    {
        switch (uiSlotInfo.slotInfo.slotInfoType)
        {
            case TypeData.SlotInfoType.아이템:
                {

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
    }
}
