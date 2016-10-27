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
        public GameObject inventoryObj ;     // 소지품 : I
        public GameObject skillObj;          // 스킬 : K
        public GameObject questListObj;      // 퀘스트일지 : L
        public GameObject questObj;          // 퀘스트 : NPC 근처에 있을때 F
        public GameObject storeObj;          // 상점 : NPC 근처에 있을때 F
        public GameObject storageObj;        // 창고 : NPC 근처에 있을때 F

        public string characterW = "CharacterW";
        public string uiModeW    = "UIModeW";
        public string inventoryW = "InventoryW";
        public string skillW     = "SkillW";
        public string questListW = "QuestListW";
        public string questW     = "QuestW";
        public string storeW     = "StoreW";
        public string storageW   = "StorageW";
    }

    public WindowSettings windowSettings;

    [System.Serializable]
    public class PopupSettings
    {
        public GameObject divisionPopup; // 분리창
        public GameObject inquirePopup; // 확인창 (ex: 아이템을 버릴때)

        public string divW = "DivisionPopup";
        public string inquireW = "InquirePopup";
    }

    [SerializeField]
    public PopupSettings popupSettings;

    // 키 입력 정보
    [System.Serializable]
    public class InputSettings
    {
        public KeyCode character        = KeyCode.P;        // 캐릭터 P
        public KeyCode inventory        = KeyCode.I;        // 인벤토리 I
        public KeyCode questList        = KeyCode.L;        // 퀘스트일지 L
        public KeyCode skillList        = KeyCode.K;        // 스킬창 L
        public KeyCode worldMap         = KeyCode.M;        // 월드맵 M
        public KeyCode special          = KeyCode.F;        // 대화
        public KeyCode uiChangeLAlt     = KeyCode.LeftAlt;  // UI 전환 LeftAlt
        public KeyCode uiChangeESC      = KeyCode.Escape;   // UI 전환 ESC
    }

    [SerializeField]
    public InputSettings inputKey;

    public Dictionary<int, UISlotInfo> shortCuts = null;        // 키보드 단축키를 눌렀을때를 위해서.
    public Dictionary<int, UISlotInfo> storeListSlots = null;   // 상점리스트
    public Dictionary<int, UISlotInfo> buySlots = null;         // 상점 구매목록슬롯
    public Dictionary<int, UISlotInfo> sellSlots = null;        // 상점 판매목록슬롯
    public List<GameObject> windows = null;
    public List<GameObject> showWindowList = null;

    public bool isUIMode = false;
    public bool isStorage = false;
    public bool isStore = false;
    public bool isQuest = false;

    public int divQuantity = 0; // 분리한 수량

    public GameObject tempDraggingPanel = null; // 드래그중인것 복사한거
    public UITexture tempIcon = null; // 드래그중인 Icon

    void Awake()
    {
        uiManager = this;
        DontDestroyOnLoad(this);

        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();

        shortCuts = new Dictionary<int, UISlotInfo>();
        storeListSlots = new Dictionary<int, UISlotInfo>();
        buySlots = new Dictionary<int, UISlotInfo>();
        sellSlots = new Dictionary<int, UISlotInfo>();
    }

    void Start()
    {
        //// window
        FindWindow(ref windowSettings.characterObj, windowSettings.characterW);
        FindWindow(ref windowSettings.uiModeObj, windowSettings.uiModeW);
        FindWindow(ref windowSettings.inventoryObj, windowSettings.inventoryW);
        FindWindow(ref windowSettings.skillObj, windowSettings.skillW);
        FindWindow(ref windowSettings.questListObj, windowSettings.questListW);
        FindWindow(ref windowSettings.questObj, windowSettings.questW);
        FindWindow(ref windowSettings.storeObj, windowSettings.storeW);
        FindWindow(ref windowSettings.storageObj, windowSettings.storageW);

        FindWindow(ref popupSettings.divisionPopup, popupSettings.divW);
        FindWindow(ref popupSettings.inquirePopup, popupSettings.inquireW);
    }

    void Update()
    {
        // TODO : 인벤토리, 케릭터창, 상점, 창고 열려있는 상태에 따라 마우스 우클릭하여 아이템 처리방식이 달라짐
        InputUIkey();
    }

    private void FindWindow(ref GameObject obj, string objName)
    {
        if (obj != null)
        {
            obj.SetActive(false);
            windows.Add(obj);
            return;
        }

        obj = GameObject.FindGameObjectWithTag(objName);

        if (obj != null)
        {
            windows.Add(obj);
            obj.SetActive(false);
        }
    }

    // UI 키 입력
    // 상점, 창고 - 인벤창도 같이 On
    private void InputUIkey()
    {
        // 소지품
        if (Input.GetKeyDown(inputKey.inventory))
        {
            showWindowList.Add(windowSettings.inventoryObj);
            ShowWindow(showWindowList);
        }

        // 퀘스트일지창
        if (Input.GetKeyDown(inputKey.questList))
        {
            showWindowList.Add(windowSettings.questListObj);
            ShowWindow(showWindowList);
        }

        // 스킬창
        if (Input.GetKeyDown(inputKey.skillList))
        {
            showWindowList.Add(windowSettings.skillObj);
            ShowWindow(showWindowList);
        }

        // UI 모드
        if (Input.GetKeyDown(inputKey.uiChangeLAlt) || Input.GetKeyDown(inputKey.uiChangeESC))
        {
            showWindowList.Add(windowSettings.uiModeObj);
            ShowWindow(showWindowList);
        }

        // 케릭터창
        if (Input.GetKeyDown(inputKey.character))
        {
            showWindowList.Add(windowSettings.characterObj);
            showWindowList.Add(windowSettings.inventoryObj);
            ShowWindow(showWindowList);
        }

        // 퀘스트창
        if (isQuest)
        {
            showWindowList.Add(windowSettings.questObj);
            ShowWindow(showWindowList);
            isQuest = false;
        }

        // 상점
        if (isStore)
        {
            showWindowList.Add(windowSettings.storeObj);
            showWindowList.Add(windowSettings.inventoryObj);
            ShowWindow(showWindowList);
            isStore = false;
        }

        // 창고
        if (isStorage)
        {
            showWindowList.Add(windowSettings.storageObj);
            showWindowList.Add(windowSettings.inventoryObj);
            ShowWindow(showWindowList);
            isStorage = false;
        }
    }

    public void AllCloseWindow()
    {
        for (int i = 0; i < windows.Count; i++)
        {
            if (!windows[i].activeSelf)
            {
                continue;
            }

            windows[i].SetActive(false);
        }
    }

    public void ShowWindow(List<GameObject> winList)
    {
        if (windowSettings.uiModeObj.activeSelf)
        {
            bool isActive = false;

            for (int i = 0; i < winList.Count; i++)
            {
                if (!winList[i].activeSelf)
                {
                    winList[i].SetActive(true);
                    isActive = true;
                }
            }
            winList.Clear();
                
            if (isActive) { return; }
        }

        // UI 모드 아니면 UI모드로
        if (!isUIMode)
        {
            // 마우스 커서 상태
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isUIMode = true;

            playerInput.InputMove(0f, 0f); // 동작(이동, 회전) 멈추게.
            windowSettings.uiModeObj.SetActive(true);
            for (int i = 0; i < winList.Count; i++)
            {
                winList[i].SetActive(true);
            }
            winList.Clear();
        }
        // UI 모드이면 UI 해제
        else if (isUIMode)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isUIMode = false;

            popupSettings.divisionPopup.SetActive(false);
            DisableDragIiem();
            AllCloseWindow();
            windowSettings.uiModeObj.SetActive(false);
            for (int i = 0; i < winList.Count; i++)
            {
                winList[i].SetActive(false);
            }
            winList.Clear();
        }
    }

    private void SetUIMode()
    {

    }

    // UI 해제되면 드래그중인 아이템 처리
    public void DisableDragIiem()
    {
        if (tempDraggingPanel == null)
        {
            return;
        }

        tempIcon.alpha = 1f;
        Destroy(tempDraggingPanel); // UI 모드 해제되면 드래그 중인거 제거
    }
}