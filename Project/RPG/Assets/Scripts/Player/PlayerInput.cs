﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour
{
    private PlayerMovement playerMovement = null;
    private PlayerSlotData playerSlotData = null;
    private PlayerState playerState = null;
    private EquipmentHandler equipHandler = null;
    private UIManager uiManager = null;
    private UIInventory uiInventory = null;

    // 키 입력 정보
    [System.Serializable]
    public class InputSettings
    {
        public string vertical      = "Vertical";       // 상하
        public string horizontal    = "Horizontal";     // 좌우

        public KeyCode jump         = KeyCode.Space;    // 점프
        public KeyCode mouse0       = KeyCode.Mouse0;
        public KeyCode mouse1       = KeyCode.Mouse1;
        public KeyCode c            = KeyCode.C;
        public KeyCode alpha1       = KeyCode.Alpha1;
        public KeyCode alpha2       = KeyCode.Alpha2;
        public KeyCode alpha3       = KeyCode.Alpha3;
        public KeyCode alpha4       = KeyCode.Alpha4;
        public KeyCode alpha5       = KeyCode.Alpha5;
        public KeyCode alpha6       = KeyCode.Alpha6;
    }

    [SerializeField]
    public InputSettings inputKey;

    public KeyCode tempKeyCode = KeyCode.Mouse1;

    // Test용
    public GameObject weapon = null;
    public GameObject shield = null;

    public int  index = -1; // 단축키 클릭시 스킬or아이템의 인덱스를 저장해둘 변수
    public bool isClick = false;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        equipHandler = GetComponent<EquipmentHandler>();
        playerState = GetComponent<PlayerState>();
    }

    void Start()
    {
        uiManager = UIManager.Instance;
        playerSlotData = PlayerSlotData.Instance;

        UIManager.Instance.SetSkillListUpActive(PlayerInfoData.Instance.infoData.level);
    }

    void Update()
    {
        InputMove(Input.GetAxis(inputKey.vertical), Input.GetAxis(inputKey.horizontal));

        InputShortCutkey();
        
        // Test
        InputKey();
    }

    // 방향키 입력
    public void InputMove(float inputV, float inputH)
    {
        // UI 모드에서 움직이면 UI모드 해제
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        {
            if (uiManager.isUIMode)
            {
                uiManager.isUIMode = false;
                uiManager.AllCloseWindow();
            }
        }

        playerMovement.SetAniMove(inputV, inputH, false);
        playerMovement.Rotation(inputV, inputH, false);
    }

    // 단축키 입력
    private void InputShortCutkey()
    {
        // UI 모드 이면 리턴
        if (uiManager.isUIMode)
        {
            return;
        }

        // 각 슬롯이 Swap 될수 있음(스킬,아이템, 없음) => 타입 체크.
        if (Input.GetKeyDown(inputKey.alpha1)) { index = CheckSlotType(1, inputKey.alpha1); }

        if (Input.GetKeyDown(inputKey.alpha2)) { index = CheckSlotType(2, inputKey.alpha2); }

        if (Input.GetKeyDown(inputKey.alpha3)) { index = CheckSlotType(3, inputKey.alpha3); }

        if (Input.GetKeyDown(inputKey.alpha4)) { index = CheckSlotType(4, inputKey.alpha4); }

        if (Input.GetKeyDown(inputKey.alpha5)) { index = CheckSlotType(5, inputKey.alpha5); }

        if (Input.GetKeyDown(inputKey.alpha6)) { index = CheckSlotType(6, inputKey.alpha6); }

        if (Input.GetKeyDown(inputKey.c)) { index = CheckSlotType(9, inputKey.c); }

        // 해결 : UI On모드에서의 마우스 클릭처리랑. Off모드 에서 마우스 클릭처리 따로..
        if (UICamera.Raycast(Input.mousePosition)) { return; }

        if (Input.GetKeyDown(inputKey.mouse0)) { index = CheckSlotType(7, inputKey.mouse0); }

        if (Input.GetKeyDown(inputKey.mouse1)) { index = CheckSlotType(8, inputKey.mouse1); }
    }

    // Test 
    private void InputKey()
    {
        // 보유금액 추가
        if (Input.GetKeyDown(KeyCode.F1))
        {
            PlayerInfoData.Instance.infoData.gold += 2000;
            uiManager.SetGoldLabel(false);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            PlayerInfoData.Instance.infoData.gold -= 2000;
            if (PlayerInfoData.Instance.infoData.gold <= 0)
            {
                PlayerInfoData.Instance.infoData.gold = 0;
            }
            uiManager.SetGoldLabel(false);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            PlayerInfoData.Instance.totalAtt += 99999;
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            PlayerInfoData.Instance.SetExp(99999);
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("aaaaaaaaaaaaaa");
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            playerState.nextMode = TypeData.MODE.평화;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            equipHandler.SetWeapon(null, false);
            equipHandler.SetSubWeapon(null, false);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            weapon = Instantiate(Resources.Load("Equipment/Vergos_Lance_Weapon")) as GameObject;
            shield = Instantiate(Resources.Load("Equipment/Vergos_Lance_Subweapon")) as GameObject;
            equipHandler.SetWeapon(weapon, true);
            equipHandler.SetSubWeapon(shield, true);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            playerState.nextMode = TypeData.MODE.전투;
            playerMovement.isHit = true;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            playerMovement.isHit = false;
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerState.nextMode = TypeData.MODE.전투;
            playerMovement.Damage(0);
        }
    }

    // public SlotInfo slotinfo(string inputKey)
    // TODO : 마우스 클릭으로 하는건 UIManager에서 타입 확인
    // 단축키를 눌렀을때 타입 확인
    public int CheckSlotType(int slotIndex, KeyCode keyCode)
    {
        //// 무기를 장착하지 않았으면 리턴
        //if (!uiManager.characterSlots[1].isItemExist)
        //{
        //    uiManager.SetMessage("무기를 장착하세요.");
        //    return -1;
        //}

        int index = -1;

        // 대상이 단축 슬롯에 정보가 없으면
        if (!uiManager.shortCuts[slotIndex].isExist)
        {
            return -1;
        }

        UISlotInfo uiSlotInfo = uiManager.shortCuts[slotIndex].GetComponent<UISlotInfo>();

        // 쿨타임 중이면 리턴
        if (uiSlotInfo.isCoolTime)
        {
            return -1;
        }
        
        uiSlotInfo.isCoolTime = true;

        switch (uiSlotInfo.slotInfo.slotInfoType)
        {
            case TypeData.SlotInfoType.스킬:
                {
                    index = uiSlotInfo.slotInfo.skillIndex;
                    tempKeyCode = keyCode;
                    playerState.nextState = TypeData.State.스킬;
                    playerState.nextMode = TypeData.MODE.전투;

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
                break;

            case TypeData.SlotInfoType.아이템:
                {
                    // 단축창에서 스킬 사용시 쿨타임. - 단축창 같은 아이템도 쿨타임 상태로
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

                    // 단축창에서 아이템 사용시 쿨타임. - 인벤 같은 아이템도 쿨타임 상태로
                    foreach (KeyValuePair<int, UISlotInfo> invenSlot in uiManager.invenSlots)
                    {
                        // 빈 슬롯인 경우 리턴
                        if (!invenSlot.Value.isExist)
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
                        UISlotInfo tempUISlotInfo = invenSlot.Value;
                        playerSlotData.SetSlotData(invenSlot.Value.slotType, invenSlot.Value.slotIndex, ref tempUISlotInfo);
                        uiManager.invenSlots[invenSlot.Key] = tempUISlotInfo;

                        invenSlot.Value.isCoolTime = true;
                    }

                    uiSlotInfo.slotInfo.quantity -= 1;
                    playerSlotData.SetSlotData(uiSlotInfo.slotType, uiSlotInfo.slotIndex, ref uiSlotInfo);
                    ItemManager.Instance.CheckItemType((TypeData.ItemType)uiSlotInfo.slotInfo.itemType, uiSlotInfo.slotInfo.itemIndex, uiSlotInfo.isCoolTime);
                }
                break;
        }

        return index;
    }
}
