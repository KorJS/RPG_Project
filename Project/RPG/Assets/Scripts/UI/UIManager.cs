using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
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

    public List<GameObject> windows = null;

    public bool isUIMode = false;
    public bool isStore = false;
    public bool isNPC = false;

    void Awake()
    {
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        
        // window
        SetWindowObjList(ref windowSettings.characterObj, windowSettings.character);
        SetWindowObjList(ref windowSettings.uiModeObj, windowSettings.uiMode);
        SetWindowObjList(ref windowSettings.inventoryObj, windowSettings.inventory);
        SetWindowObjList(ref windowSettings.questObj, windowSettings.quest);
        SetWindowObjList(ref windowSettings.storeObj, windowSettings.store);
    }

    void Update()
    {
        // TODO : 인벤토리, 케릭터창, 상점, 창고 열려있는 상태에 따라 마우스 우클릭하여 아이템 처리방식이 달라짐
        InputSpecialkey();
    }

    private void SetWindowObjList(ref GameObject obj, string objName)
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
            CheckWindow(windowSettings.inventoryObj);
        }

        // 소지품
        if (Input.GetKeyDown(inputKey.questList))
        {
            CheckWindow(windowSettings.questObj);
        }

        // UI 모드
        if (Input.GetKeyDown(inputKey.uiChangeLAlt) || Input.GetKeyDown(inputKey.uiChangeESC))
        {
            CheckWindow(windowSettings.uiModeObj);
        }
    }

    private void CheckWindow(GameObject obj)
    {
        if (obj.activeSelf)
        {
            obj.SetActive(false);
        }
        else
        {
            obj.SetActive(true);
        }

        UIMode();
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

    private void UIMode()
    {
        // UI 모드가 아닐때 UI모드로 전환
        if (!isUIMode)
        {
            // 마우스 커서 상태
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isUIMode = true;

            playerInput.InputMove(0f, 0f); // 동작(이동, 회전) 멈추게.
        }
        // UI 모드일때 UI 해제
        else if (isUIMode)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isUIMode = false;
            AllCloseWindow();
        }
    }
}