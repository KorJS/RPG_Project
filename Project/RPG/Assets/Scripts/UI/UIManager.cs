using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    private static UIManager uiManager = null;
    public static UIManager Instance
    {
        get
        {
            if (uiManager == null)
            {
                Debug.Log("UIManager Script Null");
            }

            return uiManager;
        }
    }

    private PlayerInput playerInput = null;

    // 윈도우 정보
    [System.Serializable]
    public class WindowSettings
    {
        // 특수키 에 따라 On/Off
        public GameObject characterObj;      // 케릭터창 : P
        public GameObject uiModeObj;         // UIMode : Alt/ESC
        public GameObject inventoryObj ;      // 소지품 : I
        public GameObject questObj;          // 퀘스트 : Q
        public GameObject storeObj;          // 상점 : NPC 근처에 있을때 F

        public string character = "Character_Panel";
        public string uiMode    = "UI_Panel";
        public string inventory = "Inventory_Panel";
        public string quest     = "Quest_Panel";
        public string store     = "Store_Panel";
    }

    [SerializeField]
    public WindowSettings windowSettings;

    // 키 입력 정보
    [System.Serializable]
    public class InputSettings
    {
        public KeyCode character        = KeyCode.P;
        public KeyCode inventory        = KeyCode.I;      // 인벤토리 I
        public KeyCode questList        = KeyCode.L;      // 퀘스트일지 L
        public KeyCode worldMap         = KeyCode.M;        // 월드맵 M
        public KeyCode uiChangeLAlt     = KeyCode.LeftAlt;  // UI 전환 LeftAlt
        public KeyCode uiChangeESC      = KeyCode.Escape;   // UI 전환 ESC
    }

    [SerializeField]
    public InputSettings inputKey;
    
    public Dictionary<int, GameObject> shortCuts = null;
    public List<GameObject> windows = null;

    public bool isUIMode = false;
    public bool isStore = false;
    public bool isNPC = false;

    public GameObject tempDraggingPanel = null; // 드래그중인것 복사한거
    public UITexture tempIcon = null; // 드래그중인 Icon

    void Awake()
    {
        uiManager = this;
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();

        shortCuts = new Dictionary<int, GameObject>();
        // 단축 슬롯들 찾아 등록
        //FindShortCut();

        // window
        FindWindow(ref windowSettings.characterObj, windowSettings.character);
        FindWindow(ref windowSettings.uiModeObj, windowSettings.uiMode);
        FindWindow(ref windowSettings.inventoryObj, windowSettings.inventory);
        FindWindow(ref windowSettings.questObj, windowSettings.quest);
        FindWindow(ref windowSettings.storeObj, windowSettings.store);
    }

    void Update()
    {
        // TODO : 인벤토리, 케릭터창, 상점, 창고 열려있는 상태에 따라 마우스 우클릭하여 아이템 처리방식이 달라짐
        InputSpecialkey();
    }

    private void FindShortCut()
    {
        //// 단축 슬롯 부모
        //Transform shortCutHoler = GameObject.Find("H_Slots").transform;
        //int count = shortCutHoler.childCount;

        //// 등록 되어있다면.
        //if (shortCuts.Count >= count )
        //{
        //    return;
        //}

        //for (int i = 0; i < count; i++)
        //{
        //    shortCuts.Add(shortCutHoler.GetChild(i)); // 단축슬롯 저장
        //}
    }

    private void FindWindow(ref GameObject obj, string objName)
    {
        if (obj != null)
        {
            obj.SetActive(false);
            windows.Add(obj);
            return;
        }

        obj = GameObject.Find(objName);

        if (obj != null)
        {
            windows.Add(obj);
            obj.SetActive(false);
        }
    }

    // 특수키 입력
    private void InputSpecialkey()
    {
        // 케릭터창
        if (Input.GetKeyDown(inputKey.character))
        {

        }

        // 소지품
        if (Input.GetKeyDown(inputKey.inventory))
        {
            UIMode(windowSettings.inventoryObj);
        }

        // 소지품
        if (Input.GetKeyDown(inputKey.questList))
        {
            UIMode(windowSettings.questObj);
        }

        // UI 모드
        if (Input.GetKeyDown(inputKey.uiChangeLAlt) || Input.GetKeyDown(inputKey.uiChangeESC))
        {
            UIMode(windowSettings.uiModeObj);
        }
    }

    public void AllCloseWindow()
    {
        for (int i = 0; i < windows.Count; i++)
        {
            windows[i].SetActive(false);
        }
    }

    private void Store()
    {
        if (!isStore)
        {
            return;
        }

        windowSettings.storeObj.SetActive(true);
    }

    private void UIMode(GameObject obj)
    {
        // UI 모드 아니면 UI모드로
        if (!isUIMode)
        {
            // 마우스 커서 상태
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isUIMode = true;

            playerInput.InputMove(0f, 0f); // 동작(이동, 회전) 멈추게.
            windowSettings.uiModeObj.SetActive(true);
            obj.SetActive(true);
        }
        // UI 모드이면 UI 해제
        else if (isUIMode)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isUIMode = false;

            DisableDragIiem();
            AllCloseWindow();
            windowSettings.uiModeObj.SetActive(false);
            obj.SetActive(false);
        }
    }

    // UI 해제되면 드래그중인 아이템 처리
    private void DisableDragIiem()
    {
        if (tempDraggingPanel == null)
        {
            return;
        }

        tempIcon.alpha = 1f;
        Destroy(tempDraggingPanel); // UI 모드 해제되면 드래그 중인거 제거
    }
}