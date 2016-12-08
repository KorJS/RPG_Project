using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour
{
    private PlayerInfoData playerInfoData = null;
    private PlayerMovement playerMovement = null;
    private PlayerSlotData playerSlotData = null;
    private PlayerState playerState = null;
    private PlayerEffect playerEffect = null;
    private EquipmentHandler equipHandler = null;
    private UIManager uiManager = null;
    private UIInventory uiInventory = null;
    public UIJoystick uiJoystick = null;

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

    public Vector3 targetPos = Vector3.zero;
    private Transform playerT = null;
    private Camera mainCamera = null;
    private int layerMark = 0;
    private Ray rayTest;

    private GameObject miniPlayer = null;
    private GameObject miniMapCam = null;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        equipHandler = GetComponent<EquipmentHandler>();
        playerState = GetComponent<PlayerState>();
        playerEffect = GetComponent<PlayerEffect>();

        playerT = GetComponent<Transform>();
        mainCamera = Camera.main;
        layerMark = (-1) - (1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Environment"));
        //uiJoystick = GameObject.FindGameObjectWithTag("PosJoystick").GetComponent<UIJoystick>();
    }

    void Start()
    {
        uiManager = UIManager.Instance;
        playerInfoData = PlayerInfoData.Instance;
        playerSlotData = PlayerSlotData.Instance;

        UIManager.Instance.SetSkillListUpActive(PlayerInfoData.Instance.infoData.level);

        miniPlayer = Instantiate(Resources.Load("UI/MinimapPlayer")) as GameObject;
        miniPlayer.transform.SetParent(transform);
        miniPlayer.transform.localPosition = new Vector3(0f, 20f, 0f);

        miniMapCam = Instantiate(Resources.Load("UI/MinimapCam")) as GameObject;
        miniMapCam.transform.localPosition = new Vector3(0f, 50f, 0f);
        miniMapCam.SetActive(false);
    }

    void Update()
    {
        if (GameManager.Instance.currentGameState != TypeData.GameState.시작)
        {
            return;
        }

        if (playerState.currentState == TypeData.State.죽음)
        {
            return;
        }

        if (GameManager.Instance.currentGameState == TypeData.GameState.시작)
        {
            if (!miniMapCam.activeSelf)
            {
                miniMapCam.SetActive(true);
            }
        }

        //InputMove(uiJoystick.joyStickPosY, uiJoystick.joyStickPosX);
        InputMove(Input.GetAxis(inputKey.vertical), Input.GetAxis(inputKey.horizontal));

        InputShortCutkey();
        
        // Test
        InputKey();

        CheckCrossHairDistance();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (mainCamera != null)
        {
            Gizmos.DrawLine(mainCamera.transform.position, targetPos);
        }
    }

    // 크로스헤어 거리측정
    private void CheckCrossHairDistance()
    {
        if (uiManager.crossHair == null || uiManager.chDistance == null)
        {
            Debug.LogError("CrossHair Object Or Label Null");
            return;
        }

        bool isTarget = false;
        int x = Screen.width / 2;
        int y = Screen.height / 2;

        Ray ray = mainCamera.ScreenPointToRay(new Vector3(x, y));

        RaycastHit hit;
        rayTest = ray;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMark))
        {
            targetPos = hit.point;

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Monster"))
            {
                float dis = Vector3.Distance(playerT.position, hit.point);
                uiManager.chDistance.text = Mathf.RoundToInt(dis).ToString() + "m";
                isTarget = true;
            }
        }

        if (!isTarget)
        {
            uiManager.chDistance.text = "0m";
        }
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
                uiManager.showWindowList.Clear();
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
            PlayerInfoData.Instance.infoData.gold += 999999;
            uiManager.SetGoldLabel(false);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            PlayerInfoData.Instance.infoData.gold -= 999999;
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
            PlayerInfoData.Instance.totalAtt -= 99999;
        }


        if (Input.GetKeyDown(KeyCode.F5))
        {
            // 주인공 경험치 습득
            if (PlayerInfoData.Instance.SetExp(99999))
            {
                playerEffect.CheckActiveEffect(playerEffect.effectSettings.levelup, true);
            }
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            PlayerInfoData.Instance.infoData.currentHp = 1;
            PlayerInfoData.Instance.infoData.currentMp = 1;
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            PlayerInfoData.Instance.infoData.currentHp = PlayerInfoData.Instance.totalMaxHp;
            PlayerInfoData.Instance.infoData.currentMp = PlayerInfoData.Instance.totalMaxMp;
        }

        if (Input.GetKeyDown(KeyCode.F8))
        {
            playerState.nextMode = TypeData.MODE.평화;
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            playerState.nextMode = TypeData.MODE.전투;
            playerMovement.SetDamage(null, 0);
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("aaaaaaaaaaaaaa");
        }
    }

    // public SlotInfo slotinfo(string inputKey)
    // TODO : 마우스 클릭으로 하는건 UIManager에서 타입 확인
    // 단축키를 눌렀을때 타입 확인
    public int CheckSlotType(int slotIndex, KeyCode keyCode)
    {
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

        switch (uiSlotInfo.slotInfo.slotInfoType)
        {
            case TypeData.SlotInfoType.스킬:
                {
                    // 무기를 장착하지 않았으면 리턴
                    if (!uiManager.characterSlots[1].isExist)
                    {
                        uiManager.SetMessage("무기를 장착하세요.");
                        return -1;
                    }

                    // 스킬 사용중이면 리턴
                    if (!playerMovement.isIdle)
                    {
                        return -1;
                    }

                    int tempIndex = uiSlotInfo.slotInfo.skillIndex;

                    SkillData.SkillInfo skillInfo = SkillData.Instance.skillInfos[tempIndex];

                    // mp 사용
                    float currentMp = playerInfoData.infoData.currentMp;

                    // mp가 부족하면 리턴
                    if (-skillInfo.mp > currentMp)
                    {
                        playerState.nextState = TypeData.State.이동;
                        uiManager.SetMessage("MP가 부족합니다.");
                        return -1;
                    }

                    index = tempIndex;

                    uiSlotInfo.isCoolTime = true;

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
                    // 사용 레벨이 안되면
                    if (!uiManager.CheckIsUseItem(uiSlotInfo))
                    {
                        SoundManager.Instance.PlaySingleSystem(uiManager.uiSounds.lockBGM);

                        return -1;
                    }

                    uiSlotInfo.isCoolTime = true;

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

                    ItemManager.Instance.CheckItemType((TypeData.ItemType)uiSlotInfo.slotInfo.itemType, uiSlotInfo.slotInfo.itemIndex, uiSlotInfo.isCoolTime);

                    uiSlotInfo.slotInfo.quantity -= 1;

                    playerSlotData.SetSlotData(uiSlotInfo.slotType, uiSlotInfo.slotIndex, ref uiSlotInfo);
                    uiSlotInfo.ReSetting();
                }
                break;
        }

        return index;
    }
}
