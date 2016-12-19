using UnityEngine;
using System.Collections;

public class UISlotInfo : MonoBehaviour
{
    private UIManager        uiManager      = null; // UI 매니저
    private PlayerSlotData   playerSlotData = null; // 주인공 슬롯 정보

    public TypeData.SlotType slotType       = TypeData.SlotType.없음; // 슬롯 타입

    [System.Serializable]
    public struct SlotInfo
    {
        public int                      skillIndex;     // 스킬 인덱스
        public int                      itemIndex;      // 아이템 인덱스
        public float                    coolTime;       // 쿨타임
        public int                      quantity;       // 수량
        public string                   name;           // 스킬or아이템 이름
        public string                   iconName;       // 아이콘
        public TypeData.ItemType        itemType;       // 아이템 타입
        public TypeData.SlotInfoType    slotInfoType;   // 슬롯정보 타입
    }

    public SlotInfo slotInfo;

    [System.Serializable]
    public class SlotSettings
    {
        public UITexture    uiIcon;             // 아이콘
        public UILabel      uiName;             // 이름
        public UILabel      uiLevel;            // 레벨
        public UISprite     uiCoolTime;         // 쿨타임 텍스쳐
        public UILabel      uiCoolTimeLabel;    // 쿨타임
        public UILabel      uiQuantity;         // 수량
        public UILabel      uiSellGold;         // 판매 금액
        public UILabel      uiBuyGold;          // 구매 금액
        public GameObject   overlapObj;         // 스킬리스트 배우지 않은 스킬 클릭 막기
        public GameObject   upBtnObj;           // 스킬리스트 배울수 있는 스킬 화살표
        public KeyCode      slotKeyCode;        // 슬롯 키코드(방패막기 같은 지속클릭)
    }

    [SerializeField]
    public SlotSettings slotSettings;

    public readonly int QUANTITY_MAX    = 99;    // 인벤토리 슬롯당 최대 수량 - 소모품, 퀘스트템

    public int          slotIndex       = 0;     // 슬롯 인덱스
    public bool         isExist         = false; // 해당 슬롯에 아이템이 있는지
    public bool         isAddDiv        = false; // 해당 슬롯 아이템이 소모품,재료인경우 합치고 나누는게 가능
    public bool         isSkillLearn    = false; // 스킬리스트, 배운 스킬인지 아닌지.

    public float        coolTimer       = 0f;    // 쿨타임 타이머
    public bool         isCoolTime      = false; // 쿨타임 중인지 여부

    void Awake()
    {
        uiManager       = UIManager.Instance;
        playerSlotData  = PlayerSlotData.Instance;

        isExist         = false;

        SetSlotIndex(); // 슬롯 인덱스 설정
        CheckType();    // 슬롯 정보 설정
        SetSlotIcon();  // 슬롯 아이콘 설정
        SetSlotName();  // 슬롯 아이콘 이름 설정 - 스킬리스트, 상점리스트

        SetSlotMark();
    }

    void OnEnable()
    {
        CheckType(); // 슬롯 정보 설정
        SetSlotIcon(); // 슬롯 아이콘 설정
        SetSlotName(); // 슬롯 아이콘 이름 설정 - 스킬리스트, 상점리스트
    }

    void OnDisable()
    {
        if (slotType == TypeData.SlotType.구매 || slotType == TypeData.SlotType.판매)
        {
            isExist = false;
            SetQuantity();
            slotInfo.itemType = TypeData.ItemType.없음;
            slotInfo.itemIndex = -1;
            slotInfo.quantity = 0;
        }
    }

    void Update()
    {
        if (GameManager.Instance.currentGameState == TypeData.GameState.종료)
        {
            return;
        }

        if (slotSettings.uiCoolTime == null)
        {
            return;
        }

        if (isCoolTime)
        {
            if (slotInfo.coolTime == 0)
            {
                isCoolTime = false;
                return;
            }
            SetCoolTime();
        }
        else if (!isCoolTime && slotSettings.uiCoolTime.gameObject.activeSelf)
        {
            slotSettings.uiCoolTime.gameObject.SetActive(false);
        }
    }

    void OnHover()
    {

    }

    // 슬롯 인덱스 설정
    private void SetSlotIndex()
    {
        string[] strIndex = this.gameObject.name.Split(' ');
        slotIndex = int.Parse(strIndex[1]);
    }

    // 슬롯에 아이콘 설정
    public void SetSlotIcon()
    {
        // 슬롯에 아이템이 없으면 리턴
        if (!isExist)
        {
            slotSettings.uiIcon.gameObject.SetActive(false);
            return;
        }

        slotSettings.uiIcon.gameObject.SetActive(true);
        slotSettings.uiIcon.mainTexture = Resources.Load("Icon/" + slotInfo.iconName) as Texture2D;

        if (slotSettings.uiIcon.mainTexture == null)
        {
            Debug.Log(gameObject.name + " : 맞는 아이템이 존재하지 않습니다.");
        }
    }

    private void SetSlotName()
    {
        if (!(slotType == TypeData.SlotType.상점리스트 || slotType == TypeData.SlotType.스킬리스트))
        {
            return;
        }

        if (!isExist)
        {
            slotSettings.uiName.text = null;
            return;
        }

        slotSettings.uiName.text = slotInfo.name;

        if (slotType == TypeData.SlotType.스킬리스트)
        {
            slotSettings.uiLevel.text = "Lv " + SkillData.Instance.skillInfos[slotInfo.skillIndex].level.ToString();
        }
    }

    public void SetQuantity()
    {
        if (slotType == TypeData.SlotType.상점리스트 || slotType == TypeData.SlotType.스킬리스트)
        {
            return;
        }

        // 슬롯에 아이템이 없으면 리턴
        if (!isExist)
        {
            slotSettings.uiQuantity.gameObject.SetActive(false);
            return;
        }

        // 수량 표시 2이상이면 표시
        if (slotInfo.quantity > 1)
        {
            slotSettings.uiQuantity.gameObject.SetActive(true);
            slotSettings.uiQuantity.text = slotInfo.quantity.ToString();
        }
        else // 1개 일때
        {
            slotSettings.uiQuantity.gameObject.SetActive(false);
        }
    }

    // 단축키, 상점리스트, 판매목록, 구매목록 슬롯
    private void  SetSlotMark()
    {
        switch (slotType)
        {
            case TypeData.SlotType.캐릭터:
                {
                    uiManager.characterSlots.Add(slotIndex, this);
                }
                break;

            case TypeData.SlotType.인벤토리:
                {
                    uiManager.invenSlots.Add(slotIndex, this);
                }
                break;

            case TypeData.SlotType.단축키:
                {
                    uiManager.shortCuts.Add(slotIndex, this);
                }
                break;

            case TypeData.SlotType.스킬리스트:
                {
                    uiManager.skillListSlots.Add(slotIndex, this);
                }
                break;

            case TypeData.SlotType.상점리스트:
                {
                    uiManager.storeListSlots.Add(slotIndex, this);
                }
                break;

            case TypeData.SlotType.구매:
                {
                    uiManager.buySlots.Add(slotIndex, this);
                }
                break;

            case TypeData.SlotType.판매:
                {
                    uiManager.sellSlots.Add(slotIndex, this);
                }
                break;
        }
    }

    private void CheckType()
    {
        switch (slotType)
        {
            case TypeData.SlotType.캐릭터:
                {
                    isExist = playerSlotData.GetSlotData(slotType, slotIndex, ref slotInfo);
                }
                break;

            case TypeData.SlotType.인벤토리:
            case TypeData.SlotType.단축키:
            case TypeData.SlotType.창고:
                {
                    isExist = playerSlotData.GetSlotData(slotType, slotIndex, ref slotInfo);
                    SetQuantity();
                }
                break;

            case TypeData.SlotType.스킬리스트:
                {
                    isExist = true;
                    isSkillLearn = playerSlotData.GetSlotData(slotType, slotIndex, ref slotInfo);
                    slotSettings.overlapObj.SetActive(!isSkillLearn);
                }
                break;
        }
    }

    public void ReSetting()
    {
        CheckType();
        SetSlotIcon();
    }

    public void StoreReSetting()
    {
        SetSlotIcon();
        SetQuantity();
    }

    public void SkillListUpBtn()
    {
        uiManager.popupSettings.inquirePopup.SetActive(true);
        uiManager.popupSettings.inquirePopup.GetComponent<UIInquirePopup>().SetMessage(this, "스킬을 배우시겠습니까?");
    }

    private void SetCoolTime()
    {
        slotSettings.uiCoolTime.gameObject.SetActive(true);

        coolTimer += Time.deltaTime;

        if (coolTimer > slotInfo.coolTime)
        {
            coolTimer = slotInfo.coolTime;
        }

        float ratio = 1f - (coolTimer / slotInfo.coolTime);

        if (slotSettings.uiCoolTime)
        {
            slotSettings.uiCoolTime.fillAmount = ratio;
            slotSettings.uiCoolTimeLabel.text = (Mathf.RoundToInt(slotInfo.coolTime - coolTimer)).ToString();
        }

        if (ratio <= 0f)
        {
            isCoolTime = false;
            coolTimer = 0f;
            slotSettings.uiCoolTime.gameObject.SetActive(false);

            // 아이템인 경우
            if (slotInfo.itemIndex != -1)
            {
                if ((int)TypeData.CusomableType.버프 == ItemData.Instance.cusomableInfos[slotInfo.itemIndex].cusomableType)
                {
                    ItemManager.Instance.CheckItemType((TypeData.ItemType)slotInfo.itemType, slotInfo.itemIndex, isCoolTime);
                }
            }
        }
    }
}
