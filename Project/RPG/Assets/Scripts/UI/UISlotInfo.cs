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
        public int quantity;
        public string iconName;
        public TypeData.ItemType itemType;
        public TypeData.SlotInfoType slotInfoType;
    }

    public SlotInfo slotInfo;

    [System.Serializable]
    public class SlotSettings
    {
        public UITexture uiIcon;
        public UISprite uiCoolTime;
        public UILabel uiQuantity;
    }

    [SerializeField]
    public SlotSettings slotSettings;

    public int slotIndex = 0; // 슬롯 인덱스
    public bool isItemExist = false;  // 해당 슬롯에 아이템이 있는지

    void Awake()
    {
        uiManager = UIManager.Instance;
        playerSlotData = PlayerSlotData.Instance;

        isItemExist = false;

        SetSlotIndex(); // 슬롯 인덱스 설정
        SetSlotInfo(); // 슬롯 정보 설정
        SetSlotIcon(); // 슬롯 아이콘 설정

        if (slotType == TypeData.SlotType.단축키)
        {
            uiManager.shortCuts.Add(slotIndex, this.gameObject);  // 키보드 단축키를 눌렀을때를 정보 확인.
        }
    }

    // 슬롯 인덱스 설정
    private void SetSlotIndex()
    {
        string[] strIndex = this.gameObject.name.Split(' ');
        slotIndex = int.Parse(strIndex[1]);
    }

    // 슬롯 정보 설정
    private void SetSlotInfo()
    {
        isItemExist = playerSlotData.SetSlotData(slotType, slotIndex, ref slotInfo);
    }

    // 슬롯에 아이콘 설정
    private void SetSlotIcon()
    {
        // 슬롯에 아이템이 없으면 리턴
        if (!isItemExist)
        {
            slotSettings.uiIcon.mainTexture = null;
            return;
        }

        slotSettings.uiIcon.mainTexture = Resources.Load("Icon/" + slotInfo.iconName) as Texture2D;
    }

    public void ReSetting()
    {
        SetSlotInfo();
        SetSlotIcon();
    }
}
