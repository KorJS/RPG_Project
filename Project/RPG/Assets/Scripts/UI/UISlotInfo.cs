using UnityEngine;
using System.Collections;

public class UISlotInfo : MonoBehaviour
{
    private UIManager uiManager = null;
    private PlayerInventory playerInventory = null;
    private PlayerShortCut playerShortCut = null;

    public TypeData.SlotType slotTpye = TypeData.SlotType.없음;

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

    [SerializeField]
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
        playerInventory = PlayerInventory.Instance;
        playerShortCut = PlayerShortCut.Instance;
        isItemExist = false;

        SetSlotIndex(); // 슬롯 인덱스 설정
        SetSlotInfo(); // 슬롯 정보 설정
        SetSlotIcon(); // 슬롯 아이콘 설정

        uiManager.shortCuts.Add(slotIndex, this.gameObject);
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
        switch (slotTpye)
        {
            case TypeData.SlotType.인벤토리:
                {
                    isItemExist = playerInventory.GetInventroyDate(slotIndex, ref slotInfo);
                }
                break;

            case TypeData.SlotType.단축키:
                {
                    isItemExist = playerShortCut.GetShortCutData(slotIndex, ref slotInfo);
                }
                break;

            case TypeData.SlotType.상점:
                {

                }
                break;

            case TypeData.SlotType.창고:
                {

                }
                break;
        }
    }

    // 슬롯에 아이콘 설정
    private void SetSlotIcon()
    {
        // 슬롯에 아이템이 없으면 리턴
        if (!isItemExist)
        {
            return;
        }

        slotSettings.uiIcon.mainTexture = Resources.Load("Icon/" + slotInfo.iconName) as Texture2D;
    }
}
