using UnityEngine;
using System.Collections;

public class UISlotInfo : MonoBehaviour
{
    private UIManager uiManager = null;
    private SkillData skillData = null;
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
        public UISprite uiCoolTime;
        public UILabel uiQuantity;
        public UILabel uiSellGold;
        public UILabel uiBuyGold;
        public GameObject overlapObj;
        public KeyCode slotKeyCode;
    }

    [SerializeField]
    public SlotSettings slotSettings;

    public readonly int QUANTITY_MAX = 99; // 인벤토리 슬롯당 최대 수량 - 소모품, 퀘스트템

    public int slotIndex = 0; // 슬롯 인덱스
    public bool isItemExist = false;  // 해당 슬롯에 아이템이 있는지
    public bool isAddDiv = false; // 해당 슬롯 아이템이 소모품,재료인경우 합치고 나누는게 가능
    public bool isSkillLearn = false; // 스킬리스트, 배운 스킬인지 아닌지.

    void Awake()
    {
        uiManager = UIManager.Instance;
        skillData = SkillData.Instance;
        playerSlotData = PlayerSlotData.Instance;

        isItemExist = false;

        SetSlotIndex(); // 슬롯 인덱스 설정
    }

    void Start()
    {
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
            isItemExist = false;
            SetQuantity();
            slotInfo.itemType = TypeData.ItemType.없음;
            slotInfo.itemIndex = -1;
            slotInfo.quantity = 0;
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
        if (!isItemExist)
        {
            slotSettings.uiIcon.gameObject.SetActive(false);
            return;
        }

        slotSettings.uiIcon.gameObject.SetActive(true);
        slotSettings.uiIcon.mainTexture = Resources.Load("Icon/" + slotInfo.iconName) as Texture2D;

        if (slotSettings.uiIcon.mainTexture == null)
        {
            Debug.Log("아이콘명에 맞는 아이템이 존재하지 않습니다.");
        }
    }

    private void SetSlotName()
    {
        if (!(slotType == TypeData.SlotType.상점리스트 || slotType == TypeData.SlotType.스킬리스트))
        {
            return;
        }

        if (!isItemExist)
        {
            slotSettings.uiName.text = null;
            return;
        }

        slotSettings.uiName.text = slotInfo.name;
    }

    public void SetQuantity()
    {
        // 슬롯에 아이템이 없으면 리턴
        if (!isItemExist)
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
        else
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
                    isItemExist = playerSlotData.GetSlotData(slotType, slotIndex, ref slotInfo);
                }
                break;

            case TypeData.SlotType.인벤토리:
            case TypeData.SlotType.단축키:
            case TypeData.SlotType.창고:
                {
                    isItemExist = playerSlotData.GetSlotData(slotType, slotIndex, ref slotInfo);
                    SetQuantity();
                }
                break;

            case TypeData.SlotType.스킬리스트:
                {
                    isItemExist = true;
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
}
