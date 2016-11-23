using UnityEngine;
using System.Collections;

public class UISlotInfo : MonoBehaviour
{
    private UIManager uiManager = null;
    private PlayerSlotData playerSlotData = null;

    public TypeData.SlotType slotType = TypeData.SlotType.없음;

    [System.Serializable]
    public struct SlotInfo
    {
        public int skillIndex;
        public int itemIndex;
        public float coolTime;
        public int quantity;
        public string name;
        public string iconName;
        public TypeData.ItemType itemType;
        public TypeData.SlotInfoType slotInfoType;
    }

    public SlotInfo slotInfo;

    [System.Serializable]
    public class SlotSettings
    {
        public UITexture uiIcon;
        public UILabel uiName;
        public UILabel uiLevel;
        public UISprite uiCoolTime;
        public UILabel uiCoolTimeLabel;
        public UILabel uiQuantity;
        public UILabel uiSellGold;
        public UILabel uiBuyGold;
        public GameObject overlapObj;
        public GameObject upBtnObj;
        public KeyCode slotKeyCode;
    }

    [SerializeField]
    public SlotSettings slotSettings;

    public readonly int QUANTITY_MAX = 99; // 인벤토리 슬롯당 최대 수량 - 소모품, 퀘스트템

    public int slotIndex = 0; // 슬롯 인덱스
    public bool isExist = false;  // 해당 슬롯에 아이템이 있는지
    public bool isAddDiv = false; // 해당 슬롯 아이템이 소모품,재료인경우 합치고 나누는게 가능
    public bool isSkillLearn = false; // 스킬리스트, 배운 스킬인지 아닌지.

    public float coolTimer = 0f;
    public bool isCoolTime = false;

    // TODO : 다음 프로젝트때 쿨타임 구조 신경쓸것
    // 1개인 소모품인 경우 사용시 삭제후 바로 습득하게 되면 쿨타임 초기화 되는 버그 발생.
    // 1개인경우 화면에 안보이게 하고 쿨타임은 적용 - 쿨타임 종료시 슬롯에서 제거

    void Awake()
    {
        uiManager = UIManager.Instance;
        playerSlotData = PlayerSlotData.Instance;

        isExist = false;

        SetSlotIndex(); // 슬롯 인덱스 설정
        CheckType(); // 슬롯 정보 설정
        SetSlotIcon(); // 슬롯 아이콘 설정
        SetSlotName(); // 슬롯 아이콘 이름 설정 - 스킬리스트, 상점리스트

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
                ItemManager.Instance.CheckItemType((TypeData.ItemType)slotInfo.itemType, slotInfo.itemIndex, isCoolTime);
            }
        }
    }
}
