﻿using UnityEngine;
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
        public GameObject questObj;          // 퀘스트 : Q
        public GameObject storeObj;          // 상점 : NPC 근처에 있을때 F
        public GameObject storageObj;        // 창고 : NPC 근처에 있을때 F

        public string character = "Character_Panel";
        public string uiMode    = "UI_Panel";
        public string inventory = "Inventory_Panel";
        public string quest     = "Quest_Panel";
        public string store     = "Store_Panel";
        public string storage   = "Storage_Panel";
    }

    [SerializeField]
    public WindowSettings windowSettings;

    [System.Serializable]
    public class PopupSettings
    {
        public GameObject divisionPopup; // 분리창
        public GameObject InquirePopup; // 확인창 (ex: 아이템을 버릴때)
    }

    [SerializeField]
    public PopupSettings popupSettings;

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

    public Dictionary<int, GameObject> shortCuts = null; // 키보드 단축키를 눌렀을때를 위해서.
    public List<GameObject> windows = null;

    public bool isUIMode = false;
    public bool isStore = false;
    public bool isNPC = false;

    public int divQuantity = 0; // 분리한 수량

    public GameObject tempDraggingPanel = null; // 드래그중인것 복사한거
    public UITexture tempIcon = null; // 드래그중인 Icon

    void Awake()
    {
        uiManager = this;

        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();

        popupSettings.divisionPopup = GameObject.FindGameObjectWithTag("DivisionPopup");
        popupSettings.divisionPopup.SetActive(false);

        popupSettings.InquirePopup = GameObject.FindGameObjectWithTag("InquirePopup");
        popupSettings.InquirePopup.SetActive(false);

        shortCuts = new Dictionary<int, GameObject>();
        
        // window
        FindWindow(ref windowSettings.characterObj, windowSettings.character);
        FindWindow(ref windowSettings.uiModeObj, windowSettings.uiMode);
        FindWindow(ref windowSettings.inventoryObj, windowSettings.inventory);
        FindWindow(ref windowSettings.questObj, windowSettings.quest);
        FindWindow(ref windowSettings.storeObj, windowSettings.store);
        FindWindow(ref windowSettings.storageObj, windowSettings.storage);
    }

    void Update()
    {
        // TODO : 인벤토리, 케릭터창, 상점, 창고 열려있는 상태에 따라 마우스 우클릭하여 아이템 처리방식이 달라짐
        InputSpecialkey();
    }

    //private void FindShortCut()
    //{
    //    GameObject[] obj = GameObject.FindGameObjectsWithTag("ShortCut");

    //    for (int i = 0; i < obj.Length; i++)
    //    {
    //        string[] strIndex = obj[i].name.Split(' ');
    //        int index = int.Parse(strIndex[1]);

    //        shortCuts.Add(index, obj[i]);
    //    }
    //}

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
        // UI오브젝트가 활성화 상태 일때 - 이건 UI모드상태인깐
        if (windowSettings.uiModeObj.activeSelf)
        {
            // 오브젝트가 비활성화이면
            if (!obj.activeSelf)
            {
                obj.SetActive(true); // 활성화
                return;
            }
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
            obj.SetActive(true);
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
            obj.SetActive(false);
        }
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