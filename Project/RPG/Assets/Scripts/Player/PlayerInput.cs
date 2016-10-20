using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour
{
    private PlayerMovement playerMovement = null;
    private PlayerState playerState = null;
    private EquipmentHandler equipHandler = null;
    private UIManager uiManager = null;

    // 키 입력 정보
    [System.Serializable]
    public class InputSettings
    {
        public string vertical      = "Vertical";       // 상하
        public string horizontal    = "Horizontal";     // 좌우

        public KeyCode jump         = KeyCode.Space;           // 점프
        public KeyCode special      = KeyCode.F;        // 특수동작 F ( 대화, 줍기, 채집 )
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

    public int  index = -1; // 단축키 클릭시 스킬or아이템의 인덱스를 저장해둘 변수

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        equipHandler = GetComponent<EquipmentHandler>();
        playerState = GetComponent<PlayerState>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

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
        InputMove(Input.GetAxis(inputKey.vertical), Input.GetAxis(inputKey.horizontal));

        // UI 모드가 아닐때
        if (!uiManager.isUIMode)
        {
            InputShortCutkey();
        }

        // Test
        InputKey();
    }

    // 방향키 입력
    public void InputMove(float inputV, float inputH)
    {
        // UI 모드에서 움직이면 UI모드 해제
        if (inputH != 0 || inputV != 0)
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
        if (Input.GetKeyDown(inputKey.alpha1))
        {
            index = CheckSlotType(inputKey.alpha1);
        }

        if (Input.GetKeyDown(inputKey.alpha2))
        {
            index = CheckSlotType(inputKey.alpha2);
        }

        // TODO : UI On모드에서의 마우스 클릭처리랑. Off모드 에서 마우스 클릭처리 따로..(나중에)
        // 각 슬롯이 Swap 될수 있음(스킬,아이템, 없음) => 타입 체크.
        if (UICamera.Raycast(Input.mousePosition))
        {
            return;
        }
        if (Input.GetKeyDown(inputKey.mouse0))
        {
            // 타입 체크후 타입에 인덱스 값을 가져옴.( 어떤 스킬이고, 어떤 아이템인지.) = 스킬 정보자체를 넘겨서. 워리어가 알아서 할까.. 
            index = CheckSlotType(inputKey.mouse0);
        }

        if (Input.GetKeyDown(inputKey.mouse1))
        {
            index = CheckSlotType(inputKey.mouse1);
        }
    }

    // Test 
    private void InputKey()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            playerState.nextMode = TypeData.MODE.평화;
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            playerState.nextMode = TypeData.MODE.전투;
            playerMovement.Damage();
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
            playerMovement.Damage();
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
                        playerState.nextState = TypeData.State.스킬;
                        playerState.nextMode = TypeData.MODE.전투;
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
