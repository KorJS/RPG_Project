﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIClick : MonoBehaviour
{
    private UIManager        uiManager      = null; // UI 매니저
    private SkillData        skillData      = null; // 스킬 정보
    private PlayerInput      playerInupt    = null; // 주인공 입력
    private PlayerMovement   playerMovement = null; // 주인공 동작
    private PlayerState      playerState    = null; // 주인공 상태
    private PlayerSlotData   playerSlotData = null; // 주인공 슬롯 정보
    private EquipmentHandler equipHandler   = null; // 장비 핸들러

    private UIDragAndDrop    uiDragAndDrop  = null; // 드래그앤드롭
    public  UISlotInfo       uiSlotInfo     = null; // 슬롯 정보
    private UISkillList      uiSkillList    = null; // 스킬리스트

    public bool              isClick        = false; // 클릭여부

    void Awake()
    {
        uiSkillList = GameObject.FindGameObjectWithTag("SkillW").GetComponent<UISkillList>();
    }
    
    void Start()
    {
        uiManager       = UIManager.Instance;
        skillData       = SkillData.Instance;
        playerSlotData  = PlayerSlotData.Instance;

        uiDragAndDrop   = GetComponent<UIDragAndDrop>();
        uiSlotInfo      = GetComponent<UISlotInfo>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        playerInupt     = playerObj.GetComponent<PlayerInput>();
        playerMovement  = playerObj.GetComponent<PlayerMovement>();
        playerState     = playerObj.GetComponent<PlayerState>();
        equipHandler    = playerObj.GetComponent<EquipmentHandler>();
    }

    void OnClick()
    {
        // 마우스 왼클릭
        if (UICamera.currentKey == KeyCode.Mouse0)
        {
            SetSkillDescription();

            if (uiManager.isUIMode)
            {
                // 툴팁
                switch (uiSlotInfo.slotType)
                {
                    case TypeData.SlotType.인벤토리:
                    case TypeData.SlotType.단축키:
                    case TypeData.SlotType.창고:
                    case TypeData.SlotType.상점리스트:
                        {
                            // 빈 슬롯이 아닐경우
                            if (uiSlotInfo.isExist)
                            {
                                uiManager.SetToolTip(uiSlotInfo);
                            }
                        }
                        break;
                }
            }
        }

        // 마우스 우클릭
        if (UICamera.currentKey == KeyCode.Mouse1)
        {
            CheckSlotType();
        }
    }

    void OnPress(bool isPress)
    {
        // 안드로이드에서 클릭
        if (!isPress && !uiDragAndDrop.isDragging && Application.platform == RuntimePlatform.Android)
        {
            SetSkillDescription();
            CheckSlotType();
        }

        if (uiSlotInfo.slotType == TypeData.SlotType.단축키)
        {
            isClick = isPress;
        }
    }

    // 슬롯 타입 체크
    public void CheckSlotType()
    {
        // 클릭한 빈슬롯이면 리턴
        if (!uiSlotInfo.isExist)
        {
            return;
        }

        switch (uiSlotInfo.slotType)
        {
            case TypeData.SlotType.캐릭터:
                {
                    // 무기이면 장착한 장비 제거
                    if (uiSlotInfo.slotIndex == 1)
                    {
                        equipHandler.SetWeapon(null, false);
                        equipHandler.SetSubWeapon(null, false);
                    }
                    
                    uiManager.SetSound(uiSlotInfo.slotInfo); // 사운드 출력

                    // 인벤에 추가
                    playerSlotData.AddSlotData(TypeData.SlotType.인벤토리, uiSlotInfo.slotInfo.itemType, uiSlotInfo.slotInfo.itemIndex, 1);
                    playerSlotData.RemoveSlotData(uiSlotInfo); // 현제 슬롯 제거

                    // 스텟 변경
                    uiManager.windowSettings.characterPanel.gameObject.GetComponent<UICharater>().ChangPlayerStat();
                    uiSlotInfo.ReSetting();// 현제 슬롯 갱신
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
                    if (uiSlotInfo.isExist)
                    {
                        uiManager.SetSound(uiSlotInfo.slotInfo); // 사운드 출력

                        // 구매리스트에 1개씩 추가
                        uiManager.windowSettings.storeObj.GetComponent<UIStore>().CopySlotInfo(uiSlotInfo, TypeData.SlotType.구매, 1);
                    }
                }
                break;

            case TypeData.SlotType.구매:
            case TypeData.SlotType.판매:
                {
                    // 구매>제거 / 판매>인벤으로
                    BuyAndSellSlot();
                }
                break;
        }
    }

    public void SetSkillDescription()
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

    // 인벤 슬롯
    private void InventorySlot()
    {
        // 상점창이 열여있는 경우
        if (uiManager.windowSettings.storeObj.activeSelf)
        {
            uiManager.SetSound(uiSlotInfo.slotInfo); // 사운드 출력

            UIStore uiStore = uiManager.windowSettings.storeObj.GetComponent<UIStore>();

            uiStore.CopySlotInfo(uiSlotInfo, TypeData.SlotType.판매, uiSlotInfo.slotInfo.quantity);

            uiSlotInfo.slotInfo.quantity = 0;

            if (!uiStore.changInvenIndexs.Contains(uiSlotInfo.slotIndex))
            {
                uiStore.changInvenIndexs.Add(uiSlotInfo.slotIndex); // 수량 변화 생긴 슬롯 인덱스 저장 - 정산할때 그 슬롯들 갱신
            }

            Debug.Log(uiSlotInfo.slotIndex);

            uiSlotInfo.isExist = false;
            uiSlotInfo.StoreReSetting();
        }
        // 창고창이 열여있는 경우
        else if (uiManager.windowSettings.storagePanel.alpha == 1f) // 인벤 > 창고 수량 전부 넣음
        {
            uiManager.SetSound(uiSlotInfo.slotInfo); // 사운드 출력

            int quantity = 0;

            Debug.Log("itemType : " + uiSlotInfo.slotInfo.itemType);

            if (uiSlotInfo.slotInfo.itemType == TypeData.ItemType.장비) { quantity = 1; }
            else { quantity = uiSlotInfo.slotInfo.quantity; } // 소모품, 퀘스트템일경우

            Debug.Log("quantity : " + quantity);

            playerSlotData.AddSlotData(TypeData.SlotType.창고, uiSlotInfo.slotInfo.itemType, uiSlotInfo.slotInfo.itemIndex, uiSlotInfo.slotInfo.quantity);
            playerSlotData.RemoveSlotData(uiSlotInfo);
            uiSlotInfo.ReSetting();
        }
        // 케릭창이 열여있는 경우
        else if (uiManager.windowSettings.characterPanel.alpha == 1f)
        {
            int index = uiSlotInfo.slotInfo.itemIndex;
            ItemData.EquipmentInfo tempEquipmentInfo = ItemData.Instance.equipmentInfos[index];

            int targetIndex = (int)tempEquipmentInfo.equipmentType;

            UISlotInfo targetInfo = uiManager.characterSlots[targetIndex];

            if (uiManager.windowSettings.characterPanel.gameObject.GetComponent<UICharater>().SetSlotInfo(uiSlotInfo, targetInfo))
            {
                uiSlotInfo.ReSetting();
            }
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
                // 사용 레벨이 안되면
                if (!uiManager.CheckIsUseItem(uiSlotInfo))
                {
                    SoundManager.Instance.PlaySingleSystem(uiManager.uiSounds.lockBGM);

                    return;
                }

                //쿨타임
                uiSlotInfo.isCoolTime = true;

                // 아이템 기능
                ItemManager.Instance.CheckItemType((TypeData.ItemType)uiSlotInfo.slotInfo.itemType, uiSlotInfo.slotInfo.itemIndex, uiSlotInfo.isCoolTime);

                // 인벤에 사용한 아이템이 단축창에도 있으면 쿨타임
                ShortcutItemCoolTime(false);

                // 인벤에 사용한 아이템이 인벤의 다른 슬롯에도 있으면 쿨타임
                InventoryCoolTime(true);

                // 수량 1개 감소
                uiSlotInfo.slotInfo.quantity -= 1;

                playerSlotData.SetSlotData(uiSlotInfo.slotType, uiSlotInfo.slotIndex, ref uiSlotInfo);
                uiSlotInfo.ReSetting();
            }
        }
    }

    // 단축 슬롯
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
                    if (!uiManager.characterSlots[1].isExist)
                    {
                        uiManager.SetMessage("무기를 장착하세요.");
                        return;
                    }

                    // 스킬 사용중이면 리턴
                    if (!playerMovement.isIdle)
                    {
                        return;
                    }

                    int tempIndex = uiSlotInfo.slotInfo.skillIndex;

                    SkillData.SkillInfo skillInfo = SkillData.Instance.skillInfos[tempIndex];

                    // mp 사용
                    float currentMp = PlayerInfoData.Instance.infoData.currentMp;

                    // mp가 부족하면 리턴
                    if (-skillInfo.mp > currentMp)
                    {
                        playerState.nextState = TypeData.State.이동;
                        uiManager.SetMessage("MP가 부족합니다.");

                        return;
                    }

                    playerInupt.index = tempIndex;
                    playerInupt.tempKeyCode = uiSlotInfo.slotSettings.slotKeyCode;
                    playerInupt.isClick = isClick;

                    playerState.nextState = TypeData.State.스킬;
                    playerState.nextMode = TypeData.MODE.전투;

                    uiSlotInfo.isCoolTime = true;

                    // 단축슬롯에 같은 스킬이 있을경우 쿨타임
                    ShortcutSKillCoolTime();
                }
                break;

            case TypeData.SlotInfoType.아이템:
                {
                    // 쿨타임
                    uiSlotInfo.isCoolTime = true;

                    // 아이템 기능
                    ItemManager.Instance.CheckItemType((TypeData.ItemType)uiSlotInfo.slotInfo.itemType, uiSlotInfo.slotInfo.itemIndex, uiSlotInfo.isCoolTime);

                    // 단축창에 똑같은 아이템이 있으면 쿨타임
                    ShortcutItemCoolTime(true);

                    // 단축창에서 쓴 아이템이 인벤에도 있으면 쿨타임
                    InventoryCoolTime(false);

                    // 수량 1개 감소
                    uiSlotInfo.slotInfo.quantity -= 1;
                    // 단축 - 현재 슬롯 수량 변화 > 인벤도 같이 변화
                    playerSlotData.SetSlotData(uiSlotInfo.slotType, uiSlotInfo.slotIndex, ref uiSlotInfo);
                    uiSlotInfo.ReSetting();
                }
                break;
        }
    }

    private void StorageSlot()
    {
        uiManager.SetSound(uiSlotInfo.slotInfo); // 사운드 출력
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
        uiManager.SetSound(uiSlotInfo.slotInfo); // 사운드 출력

        UIStore uiStore = uiManager.windowSettings.storeObj.GetComponent<UIStore>();
        uiStore.CopySlotInfo(uiSlotInfo, TypeData.SlotType.없음, uiSlotInfo.slotInfo.quantity);

        if (!uiStore.changInvenIndexs.Contains(uiSlotInfo.slotIndex))
        {
            uiStore.changInvenIndexs.Add(uiSlotInfo.slotIndex); // 수량 변화 생긴 슬롯 인덱스 저장 - 정산할때 그 슬롯들 갱신
        }

        uiSlotInfo.isExist = false;
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
            if (!shortCut.Value.isExist)
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

    // 인벤>단축 : isShortcutSlot = falas / 단축>단축 : isShortcutSlot = true
    private void ShortcutItemCoolTime(bool isShortcutSlot)
    {
        // 단축창에서 아이템 사용시 쿨타임. - 인벤 같은 아이템도 쿨타임 상태로
        foreach (KeyValuePair<int, UISlotInfo> shortCut in uiManager.shortCuts)
        {
            // 빈 슬롯인 경우 리턴
            if (!shortCut.Value.isExist)
            {
                continue;
            }

            // 인벤에서 사용한 아이템일때 / 자신은 제외
            if (isShortcutSlot && (shortCut.Key == uiSlotInfo.slotIndex))
            {
                continue;
            }

            // 같은 타입이 아니면 
            if (shortCut.Value.slotInfo.itemType != uiSlotInfo.slotInfo.itemType)
            {
                continue;
            }

            // 같은 인덱스가 아니면
            if (shortCut.Value.slotInfo.itemIndex != uiSlotInfo.slotInfo.itemIndex)
            {
                continue;
            }

            shortCut.Value.isCoolTime = true;
        }
    }

    // 단축>인벤
    private void InventoryCoolTime(bool isInventorySlot)
    {
        // 단축창에서 아이템 사용시 쿨타임. - 인벤 같은 아이템도 쿨타임 상태로
        foreach (KeyValuePair<int, UISlotInfo> invenSlot in uiManager.invenSlots)
        {
            // 빈 슬롯인 경우 리턴
            if (!invenSlot.Value.isExist)
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

            invenSlot.Value.isCoolTime = true;
        }
    }
}
