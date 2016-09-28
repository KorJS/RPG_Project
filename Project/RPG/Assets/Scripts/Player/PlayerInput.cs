﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour
{
    public PlayerMovement playerMovement = null;
    public EquipmentHandler equipHandler = null;

    // 키 입력 정보
    [System.Serializable]
    public class InputSettings
    {
        public string vertical      = "Vertical";       // 상하
        public string horizontal    = "Horizontal";     // 좌우
        public KeyCode jump         = KeyCode.Space;           // 점프
        public KeyCode inventory    = KeyCode.I;      // 인벤토리 I
        public KeyCode special      = KeyCode.F;        // 특수동작 F ( 대화, 줍기, 채집 )
        public KeyCode questList    = KeyCode.L;      // 퀘스트일지 L
        public KeyCode worldMap     = KeyCode.M;        // 월드맵 M
        public KeyCode uiChangeLAlt = KeyCode.LeftAlt;  // UI 전환 LeftAlt
        public KeyCode uiChangeESC  = KeyCode.Escape;   // UI 전환 ESC

        public KeyCode mouse0       = KeyCode.Mouse0;
        public KeyCode mouse1       = KeyCode.Mouse1;
        public KeyCode alpha1       = KeyCode.Alpha1;
        public KeyCode alpha2       = KeyCode.Alpha2;
    }

    [SerializeField]
    public InputSettings inputKey;

    // Test용
    public GameObject weapon = null;
    public GameObject shield = null;
    public enum Equipment
    {
        Sword_01 = 1,
        Axe_01,
        Spear_01,
        Hammer_01
    }

    public string swordName = "Sword_01";
    public string shieldName = "Shield_01";

    // 슬롯 타입
    public enum SlotType
    {
        없음 = -1,
        스킬 = 0,
        아이템
    };

    public SlotType slotType = SlotType.없음;

    // 나중에 
    // 슬롯 정보
    public struct SlotInfo
    {
        public SlotType slotType;
        public int index;
    };

    public SlotInfo slotInfo;

    public Dictionary<KeyCode, SlotInfo> slotInfos = null; // 모든 슬롯 정보

    public int  index = -1;             // 단축키 클릭시 스킬or아이템의 인덱스를 저장해둘 변수

    public bool isUIMode = false;

    void Awake()
    {
        if (!playerMovement)
        {
            playerMovement = GetComponent<PlayerMovement>();
        }

        if (!equipHandler)
        {
            equipHandler = GetComponent<EquipmentHandler>();
        }

        slotInfos = new Dictionary<KeyCode, SlotInfo>();

        slotInfo.slotType = SlotType.스킬;
        slotInfo.index = 0;
        slotInfos.Add(KeyCode.Mouse0, slotInfo);

        slotInfo.slotType = SlotType.스킬;
        slotInfo.index = 1;
        slotInfos.Add(KeyCode.Mouse1, slotInfo);

        slotInfo.slotType = SlotType.스킬;
        slotInfo.index = 2;
        slotInfos.Add(KeyCode.Alpha1, slotInfo);

        slotInfo.slotType = SlotType.스킬;
        slotInfo.index = 3;
        slotInfos.Add(KeyCode.Alpha2, slotInfo);
    }

    void Update()
    {
        // UI 모드가 아닐때
        if(!isUIMode)
        {
            InputMove();
            InputShortCutkey();
        }

        InputSpecialkey();

        // Test
        InputKey();

    }

    // 방향키 입력
    private void InputMove()
    {
        float inputV = Input.GetAxis(inputKey.vertical);
        float inputH = Input.GetAxis(inputKey.horizontal);

        playerMovement.AnimationMove(inputV, inputH, false);
        playerMovement.Rotation(inputV, inputH, false);
    }

    // 단축키 입력
    private void InputShortCutkey()
    {
        // TODO : UI On모드에서의 마우스 클릭처리랑. Off모드 에서 마우스 클릭처리 따로..(나중에)
        // 각 슬롯이 Swap 될수 있음(스킬,아이템, 없음) => 타입 체크.
        if (Input.GetKeyDown(inputKey.mouse0))
        {
            // 타입 체크후 타입에 인덱스 값을 가져옴.( 어떤 스킬이고, 어떤 아이템인지.) = 스킬 정보자체를 넘겨서. 워리어가 알아서 할까.. 
            index = CheckSlotType(inputKey.mouse0);
        }

        if (Input.GetKeyDown(inputKey.mouse1))
        {
            index = CheckSlotType(inputKey.mouse1);
        }

        if (Input.GetKeyDown(inputKey.alpha1))
        {
            index = CheckSlotType(inputKey.alpha1);
        }

        if (Input.GetKeyDown(inputKey.alpha2))
        {
            index = CheckSlotType(inputKey.alpha2);
        }
    }

    // 특수키 입력
    private void InputSpecialkey()
    {
        // UI 모드
        if (Input.GetKeyDown(inputKey.uiChangeLAlt) || Input.GetKeyDown(inputKey.uiChangeESC))
        {
            // UI 모드로 전환
            if (!isUIMode)
            {
                // 마우스 커서 상태
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                isUIMode = true;
            }
            // UI 모드 해제
            else if(isUIMode)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                isUIMode = false;
            }
        }
    }

    // Test 
    private void InputKey()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            PlayerState.Instance.nextMode = TypeData.MODE.평화;
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            PlayerState.Instance.nextMode = TypeData.MODE.전투;
            playerMovement.isDamage = true;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            equipHandler.SetWeapon(null, false);
            equipHandler.SetShield(null, false);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            weapon = Instantiate(Resources.Load("Equipment/" + swordName)) as GameObject;
            shield = Instantiate(Resources.Load("Equipment/" + shieldName)) as GameObject;
            equipHandler.SetWeapon(weapon, true);
            equipHandler.SetShield(shield, true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            swordName = Equipment.Sword_01.ToString();
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            swordName = Equipment.Spear_01.ToString();
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            swordName = Equipment.Hammer_01.ToString();
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            swordName = Equipment.Axe_01.ToString();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            PlayerState.Instance.nextMode = TypeData.MODE.전투;
            playerMovement.isHit = true;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            playerMovement.isHit = false;
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            PlayerState.Instance.nextMode = TypeData.MODE.전투;
            playerMovement.isDamage = true;
        }
    }

    // public SlotInfo slotinfo(string inputKey)
    // TODO : 마우스 클릭으로 하는건 UIManager에서 타입 확인
    // 단축키를 눌렀을때 타입 확인
    public int CheckSlotType(KeyCode inputKeyCode)
    {
        int index = 0;

        if (slotInfos.ContainsKey(inputKeyCode))
        {
            index = slotInfos[inputKeyCode].index;

            switch (slotInfos[inputKeyCode].slotType)
            {
                case SlotType.스킬:
                    {
                        PlayerState.Instance.nextState = TypeData.State.스킬;
                        PlayerState.Instance.nextMode = TypeData.MODE.전투;
                    }
                    break;

                case SlotType.아이템:
                    {
                        // TODO : 아이템 이벤트 처리 직접적으로.
                    }
                    break;
            }

            return index;
        }

        return -1;
    }
}
