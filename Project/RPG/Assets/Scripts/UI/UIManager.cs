using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager = null;
    private PlayerInput playerInput = null;
    private PlayerInfoData playerInfoData = null;

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

    // 윈도우 정보
    [System.Serializable]
    public class WindowSettings
    {
        // 특수키 에 따라 On/Off
        public UIPanel characterPanel;          // 케릭터창 : P
        public UIPanel uiModePanel;             // UIMode : Alt/ESC
        public UIPanel inventoryPanel;          // 소지품 : I
        public UIPanel skillPanel;              // 스킬 : K
        public UIPanel questListPanel;          // 퀘스트일지 : L
        public UIPanel questPanel;              // 퀘스트 : NPC 근처에 있을때 F
        public UIPanel storagePanel;            // 창고 : NPC 근처에 있을때 F
        public UIPanel shortCutPanel;           // 단축슬롯
        public UIPanel optionPanel;             // 옵션 : O
        public GameObject storeObj;             // 상점 : NPC 근처에 있을때 F

        public string characterW = "CharacterW";
        public string uiModeW    = "UIModeW";
        public string inventoryW = "InventoryW";
        public string skillW     = "SkillW";
        public string questListW = "QuestListW";
        public string questW     = "QuestW";
        public string storeW     = "StoreW";
        public string storageW   = "StorageW";
        public string shortCutW  = "ShortCutW";
        public string optionW    = "OptionW";

        public bool isCharacterW = false;
        public bool isInventoryW = false;
        public bool isSkillW = false;
        public bool isQuestListW = false;
        public bool isOptionW = false;
        public bool isUIModeW = false;
    }

    public WindowSettings windowSettings;

    [System.Serializable]
    public class PopupSettings
    {
        public UILabel    notice_message;       // 공지, 기본 메시지
        public UILabel    inquire_message;      // NPC와 대화를 하려면 'F'키를 누르시오.
        public GameObject itemDivisionPopup;    // 아이템분리창
        public GameObject goldDivisionPopup;    // 골드분리창
        public GameObject inquirePopup;         // 확인창 (ex: 아이템을 버릴때)
        public GameObject copyPopup;            // 상점에 구매목록 판매목록
        public GameObject warningPopup;         // 경고창

        public string itemDivW = "ItemDivisionPopup";
        public string goldDivW = "GoldDivisionPopup";
        public string inquireW = "InquirePopup";
        public string copyW = "CopyPopup";
        public string warningW = "WarningPopup";
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
        public KeyCode option           = KeyCode.O;        // 옵션 O
        public KeyCode special          = KeyCode.F;        // 대화
        public KeyCode uiChangeLAlt     = KeyCode.LeftAlt;  // UI 전환 LeftAlt
        public KeyCode uiChangeESC      = KeyCode.Escape;   // UI 전환 ESC
    }

    [SerializeField]
    public InputSettings inputKey;

    public Dictionary<int, UISlotInfo> shortCuts = null;        // 키보드 단축키를 눌렀을때를 위해서.
    public Dictionary<int, UISlotInfo> storeListSlots = null;   // 상점리스트
    public Dictionary<int, UISlotInfo> characterSlots = null;   // 케릭터 슬롯
    public Dictionary<int, UISlotInfo> skillListSlots = null;   // 스킬리스트 슬롯
    public SortedDictionary<int, UISlotInfo> invenSlots = null; // 인벤토리
    public SortedDictionary<int, UISlotInfo> buySlots = null;   // 상점 구매목록슬롯
    public SortedDictionary<int, UISlotInfo> sellSlots = null;  // 상점 판매목록슬롯

    public List<GameObject> wObjects = null;
    public List<UIPanel> windows = null;
    public List<UIPanel> showWindowList = null;
    public List<AudioClip> audioList = null;

    public GameObject bossHpBarObj = null;
    public GameObject mobHpBarObj = null;

    public UILabel playerGold = null;
    public UILabel storageGold = null;

    public bool isUIMode = false;
    public bool isStorage = false;
    public bool isStore = false;
    public bool isQuest = false;
    public bool isFKey = false;

    private const float     MESSAGETIME = 2f;
    private float           messageTimer = 0f;
    private bool            isMessage = false;

    public int              divQuantity = 0;            // 분리한 수량

    public GameObject       tempDraggingPanel = null;   // 드래그중인것 복사한거
    public UITexture        tempIcon = null;            // 드래그중인 Icon

    private GameObject      damageTxtHolder = null;     // 데미지 텍스트 부모
    public List<GameObject> damageTxtObjs = null;       // 데미지 텍스트 풀
    private int             damageTxtCount = 0;         // 풀 카운트

    // 방어 성공 UI이펙트
    public GameObject       blockTxtObj = null;
    public TweenScale       blockTween = null;

    public AudioClip        windowsOpenBGM = null;
    public AudioClip        windowsCloseBGM = null;

    private Transform       playerT = null;             // 주인공
    public GameObject       crossHair = null;           // 크로스헤어
    public UILabel          chDistance = null;          // 거리

    void Awake()
    {
        if (uiManager == null)
        {
            uiManager = this;
        }
        else if (uiManager != this)
        {
            Destroy(uiManager);
        }

        playerT = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerInput = playerT.GetComponent<PlayerInput>();

        shortCuts = new Dictionary<int, UISlotInfo>();
        storeListSlots = new Dictionary<int, UISlotInfo>();
        characterSlots = new Dictionary<int, UISlotInfo>();
        skillListSlots = new Dictionary<int, UISlotInfo>();
        buySlots = new SortedDictionary<int, UISlotInfo>();
        sellSlots = new SortedDictionary<int, UISlotInfo>();
        invenSlots = new SortedDictionary<int, UISlotInfo>();

        bossHpBarObj = GameObject.Find("BossPanel");
        mobHpBarObj = GameObject.Find("MonsterPanel");
        bossHpBarObj.SetActive(false);
        mobHpBarObj.SetActive(false);

        blockTxtObj = GameObject.Find("BlockTxtEffect");
        blockTween = blockTxtObj.transform.GetChild(0).GetComponent<TweenScale>();
        blockTxtObj.SetActive(false);

        damageTxtHolder = GameObject.Find("DamageHolder");
        damageTxtObjs = new List<GameObject>();
        CreateDamageTxt();
    }

    void Start()
    {
        gameManager = GameManager.Instance;
        playerInfoData = PlayerInfoData.Instance;

        //// window
        FindWindow(ref windowSettings.characterPanel, windowSettings.characterW);
        FindWindow(ref windowSettings.uiModePanel, windowSettings.uiModeW);
        FindWindow(ref windowSettings.inventoryPanel, windowSettings.inventoryW);
        FindWindow(ref windowSettings.skillPanel, windowSettings.skillW);
        FindWindow(ref windowSettings.questListPanel, windowSettings.questListW);
        FindWindow(ref windowSettings.questPanel, windowSettings.questW);
        FindWindow(ref windowSettings.storagePanel, windowSettings.storageW);
        FindWindow(ref windowSettings.optionPanel, windowSettings.optionW);

        FindObject(ref windowSettings.storeObj, windowSettings.storeW);
        FindObject(ref popupSettings.itemDivisionPopup, popupSettings.itemDivW);
        FindObject(ref popupSettings.goldDivisionPopup, popupSettings.goldDivW);
        FindObject(ref popupSettings.inquirePopup, popupSettings.inquireW);
        FindObject(ref popupSettings.copyPopup, popupSettings.copyW);
        FindObject(ref popupSettings.warningPopup, popupSettings.warningW);

        playerGold = windowSettings.inventoryPanel.gameObject.transform.FindChild("Gold").FindChild("Amount").GetComponent<UILabel>();
        storageGold = windowSettings.storagePanel.gameObject.transform.FindChild("Gold").FindChild("Amount").GetComponent<UILabel>();
        SetGoldLabel(true);
    }

    void Update()
    {
        if (GameManager.Instance.currentGameState == TypeData.GameState.종료)
        {
            return;
        }

        // TODO : 인벤토리, 케릭터창, 상점, 창고 열려있는 상태에 따라 마우스 우클릭하여 아이템 처리방식이 달라짐
        InputUIkey();

        MessageTime(); // 메시지 표시 시간

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Network_PlayerInfo.Instance.RequestSavePlayerInfo();
        }
    }

    public void DataClear()
    {
        shortCuts.Clear();
        storeListSlots.Clear();
        characterSlots.Clear();
        skillListSlots.Clear();
        buySlots.Clear();
        sellSlots.Clear();
        invenSlots.Clear();
    }

    // 데미지 텍스트 생성
    private void CreateDamageTxt()
    {
        var resource = Resources.Load("UI/DamageTxt");

        for (int i = 0; i < 20; i++)
        {
            GameObject obj = Instantiate(resource) as GameObject;
            obj.layer = UICamera.mainCamera.gameObject.layer;
            obj.transform.SetParent(damageTxtHolder.transform);
            obj.transform.localScale = Vector3.one;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localPosition = Vector3.one;

            obj.SetActive(false);
            damageTxtObjs.Add(obj);
        }
    }
    
    // 데이미 텍스트 배치
    public void SetDamageTxt(Transform targetT, float damage)
    {
        if (damageTxtCount == damageTxtObjs.Count)
        {
            damageTxtCount = 0;
        }

        damageTxtObjs[damageTxtCount].SetActive(true);
        damageTxtObjs[damageTxtCount].GetComponent<UILabel>().text = ((int)damage).ToString();
        Vector3 p = Camera.main.WorldToViewportPoint(targetT.position);
        damageTxtObjs[damageTxtCount].transform.position = UICamera.mainCamera.ViewportToWorldPoint(p);

        p = damageTxtObjs[damageTxtCount].transform.localPosition;
        p.x = Mathf.RoundToInt(p.x + 1f);
        p.y = Mathf.RoundToInt(p.y + 2f);
        p.z = 0f;

        damageTxtObjs[damageTxtCount].transform.localPosition = p;

        damageTxtCount++;
    }

    private void FindWindow(ref UIPanel uiPanel, string name)
    {
        if (uiPanel != null)
        {
            uiPanel.alpha = 0f;
            windows.Add(uiPanel);
            return;
        }

        uiPanel = GameObject.FindGameObjectWithTag(name).GetComponent<UIPanel>();

        if (uiPanel != null)
        {
            windows.Add(uiPanel);
            uiPanel.alpha = 0f;
        }
    }

    private void FindObject(ref GameObject obj, string objName)
    {
        if (obj != null)
        {
            obj.SetActive(false);
            wObjects.Add(obj);
            return;
        }

        obj = GameObject.FindGameObjectWithTag(objName);

        if (obj != null)
        {
            wObjects.Add(obj);
            obj.SetActive(false);
        }
    }

    // UI 키 입력
    // 상점, 창고 - 인벤창도 같이 On
    private void InputUIkey()
    {
        // 소지품
        if (windowSettings.isInventoryW || Input.GetKeyDown(inputKey.inventory))
        {
            windowSettings.isInventoryW = false;
            showWindowList.Add(windowSettings.inventoryPanel);
            ShowWindow(showWindowList);
        }

        // 퀘스트일지창
        if (windowSettings.isQuestListW || Input.GetKeyDown(inputKey.questList))
        {
            windowSettings.isQuestListW = false;
            showWindowList.Add(windowSettings.questListPanel);
            ShowWindow(showWindowList);
        }

        // 스킬창
        if (windowSettings.isSkillW || Input.GetKeyDown(inputKey.skillList))
        {
            windowSettings.isSkillW = false;
            showWindowList.Add(windowSettings.skillPanel);
            ShowWindow(showWindowList);
        }

        // 케릭터창
        if (windowSettings.isCharacterW || Input.GetKeyDown(inputKey.character))
        {
            windowSettings.isCharacterW = false;
            showWindowList.Add(windowSettings.characterPanel);
            showWindowList.Add(windowSettings.inventoryPanel);
            ShowWindow(showWindowList);
        }

        //  옵션
        if (windowSettings.isOptionW || Input.GetKeyDown(inputKey.option))
        {
            windowSettings.isOptionW = false;
            showWindowList.Add(windowSettings.optionPanel);
            ShowWindow(showWindowList);
        }

        // UI 모드
        if (windowSettings.isUIModeW || Input.GetKeyDown(inputKey.uiChangeLAlt) || Input.GetKeyDown(inputKey.uiChangeESC))
        {
            showWindowList.Add(windowSettings.uiModePanel);
            ShowWindow(showWindowList);
            windowSettings.isUIModeW = false;
        }

        // 퀘스트창
        if (isQuest)
        {
            showWindowList.Add(windowSettings.questPanel);
            ShowWindow(showWindowList);
            isQuest = false;
        }

        // 상점
        if (isStore)
        {
            ShowStore();
            showWindowList.Add(windowSettings.inventoryPanel);
            ShowWindow(showWindowList);
            isStore = false;
        }

        // 창고
        if (isStorage)
        {
            showWindowList.Add(windowSettings.storagePanel);
            showWindowList.Add(windowSettings.inventoryPanel);
            ShowWindow(showWindowList);
            isStorage = false;
        }
    }

    public void AllCloseWindow()
    {
        for (int i = 0; i < windows.Count; i++)
        {
            if (windows[i].alpha == 0f)
            {
                continue;
            }

            windows[i].alpha = 0f;
        }

        for (int i = 0; i < wObjects.Count; i++)
        {
            if (!wObjects[i].activeSelf)
            {
                continue;
            }
            wObjects[i].SetActive(false);
        }

        Network_Slot.Instance.RequestSaveSlot(TypeData.SlotType.캐릭터);
        Network_Slot.Instance.RequestSaveSlot(TypeData.SlotType.인벤토리);
        Network_Slot.Instance.RequestSaveSlot(TypeData.SlotType.창고);
        Network_Slot.Instance.RequestSaveSlot(TypeData.SlotType.단축키);
    }

    public void ShowWindow(List<UIPanel> winList)
    {
        if (windowSettings.uiModePanel.alpha == 1f)
        {
            bool isActive = false;

            for (int i = 0; i < winList.Count; i++)
            {
                if (winList[i].alpha == 0f)
                {
                    SoundManager.Instance.PlaySingleUI(windowsOpenBGM);
                    winList[i].alpha = 1f;
                    isActive = true;
                }
            }
            winList.Clear();

            if (isActive) { return; }
        }

        // UI 모드 아니면 UI모드로
        if (!isUIMode)
        {
            SoundManager.Instance.PlaySingleUI(windowsOpenBGM);

            // 마우스 커서 상태
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            isUIMode = true;

            playerInput.InputMove(0f, 0f); // 동작(이동, 회전) 멈추게.
            windowSettings.uiModePanel.alpha = 1f;
            for (int i = 0; i < winList.Count; i++)
            {
                winList[i].alpha = 1f;
            }
            winList.Clear();
        }
        // UI 모드이면 UI 해제
        else if (isUIMode)
        {
            SoundManager.Instance.PlaySingleUI(windowsCloseBGM);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isUIMode = false;

            DisableDragIiem();
            AllCloseWindow();
            windowSettings.uiModePanel.alpha = 0f;
            for (int i = 0; i < winList.Count; i++)
            {
                winList[i].alpha = 0f;
            }
            winList.Clear();
        }
    }

    public void ShowStore()
    {
        if (windowSettings.uiModePanel.alpha == 1f)
        {
            if (!windowSettings.storeObj.activeSelf)
            {
                windowSettings.storeObj.SetActive(true);
                return;
            }
        }

        if (windowSettings.storeObj.activeSelf)
        {
            windowSettings.storeObj.SetActive(false);
        }
        else
        {
            windowSettings.storeObj.SetActive(true);
        }
    }

    public void SetMessage(string _message)
    {
        popupSettings.notice_message.alpha = 1f;
        popupSettings.notice_message.text = _message;
        isMessage = true;
        messageTimer = 0f;
    }

    private void MessageTime()
    {
        if (!isMessage)
        {
            return;
        }

        messageTimer += Time.deltaTime;

        if (messageTimer >= MESSAGETIME)
        {
            popupSettings.notice_message.alpha = 0f;
            popupSettings.notice_message.text = null;
            messageTimer = 0f;
            isMessage = false;
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

    public void SetHpBar(Transform targetT)
    {
        GameObject tempHpBarObj = null;

        if (string.Compare(targetT.tag, "Boss") == 0)
        {
            tempHpBarObj = bossHpBarObj;
        }
        else
        {
            tempHpBarObj = mobHpBarObj;
        }

        if (tempHpBarObj.activeSelf)
        {
            return;
        }

        tempHpBarObj.gameObject.SetActive(true);
        tempHpBarObj.GetComponent<UIMonsterHpBar>().SetTarget(targetT);
    }

    public void SetGoldLabel(bool isInit)
    {
        playerGold.text = playerInfoData.infoData.gold.ToString();
        storageGold.text = playerInfoData.infoData.storageGold.ToString();

        // 처음 설정
        if (!isInit)
        {
            Network_PlayerInfo.Instance.RequestSavePlayerInfo();
        }
    }

    public void SetSkillListUpActive(int currentLevel)
    {
        PlayerSkillData playerSkilldata = PlayerSkillData.Instance;
        Dictionary<int, SkillData.SkillInfo> skillInfos = SkillData.Instance.skillInfos;

        foreach (KeyValuePair<int, SkillData.SkillInfo> skillInfo in skillInfos)
        {
            // 현재 레벨에 배울수 있는 스킬이 아니면 
            if (skillInfo.Value.level > currentLevel)
            {
                Debug.Log("배울수 없는 스킬 : " +  skillInfo.Value.name);
                continue;
            }

            // 배운 스킬 이면
            if (playerSkilldata.GetSkillData(skillInfo.Key))
            {
                Debug.Log("배운스킬 : " + skillInfo.Value.name);
                if (skillListSlots[skillInfo.Key].slotSettings.upBtnObj.activeSelf)
                {
                    skillListSlots[skillInfo.Key].slotSettings.upBtnObj.SetActive(false);
                }
                continue;
            }

            skillListSlots[skillInfo.Key].slotSettings.upBtnObj.SetActive(true);
            Debug.Log("배울수 있는 : " + skillInfo.Value.name);
        }
    }

    // 로그아웃
    public void Logout()
    {
        gameManager.nextGameState = TypeData.GameState.종료;

        StartCoroutine(gameManager.LogoutSavePlayerData());
    }
    
    // 게임종료
    public void GameExit()
    {
        gameManager.nextGameState = TypeData.GameState.종료;

        StartCoroutine(gameManager.GameExitSavePlayerData());
    }

    public void ShowFKey()
    {
        isFKey = true;
    }

    public void ShowUIModeBtn()
    {
        windowSettings.isUIModeW = true;
    }

    public void ShowCharacterBtn()
    {
        windowSettings.isCharacterW = true;
    }

    public void ShowInventoryBtn()
    {
        windowSettings.isInventoryW = true;
    }

    public void ShowSkillBtn()
    {
        windowSettings.isSkillW = true;
    }

    public void ShowQuestListBtn()
    {
        windowSettings.isQuestListW = true;
    }

    public void ShowOptionBtn()
    {
        windowSettings.isOptionW = true;
    }
}