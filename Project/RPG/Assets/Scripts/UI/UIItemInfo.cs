using UnityEngine;
using System.Collections;

public class UIItemInfo : MonoBehaviour
{
    private PlayerSlotData playerSlotData = null;
    private ItemControl itemControl = null;

    [System.Serializable]
    public struct ItemInfo
    {
        public TypeData.ItemType itemType;
        public int itemIndex;
        public int quantity;
    }

    public ItemInfo itemInfo;

    private Transform itemT = null;

    public UISprite uiHover = null;

    void Awake()
    {
        uiHover = GetComponent<UISprite>();
        uiHover.alpha = 0.5f;
    }

    void Start()
    {
        playerSlotData = PlayerSlotData.Instance;
    }

    void OnClick()
    {
        // 주인공이 근처에 없으면 리턴
        if (!itemControl.itemSettings.isPlayer)
        {
            return;
        }

        if (UICamera.currentKey == KeyCode.Mouse0)
        {
            DropItemClick();
        }
    }

    void OnHover(bool isHover)
    {
        // 근처에 주인공이 없으면 리턴
        if (!itemControl.itemSettings.isPlayer)
        {
            return;
        }

        if (isHover)
        {
            uiHover.alpha = 1f;
        }
        else
        {
            uiHover.alpha = 0.5f;
        }
    }

    // 아이템 마우스 클릭으로 습득하기 위해서 아이템 정보를 받아온다. - 아이템 생성되면서 이름 생성될때 정보 받아온다.
    public void SetItemInfo(Transform _itemT, ItemControl _itemControl, TypeData.ItemType itemType, int itemIndex, int quantity)
    {
        itemT = _itemT;
        itemControl = _itemControl;
        itemInfo.itemType = itemType;
        itemInfo.itemIndex = itemIndex;
        itemInfo.quantity = quantity;
    }

    // OnClick 에서 왼클릭시 아이템 습득 - 인벤으로
    public void DropItemClick()
    {
        if (!itemControl.itemSettings.isPlayer)
        {
            return;
        }
        
        SoundManager.Instance.PlaySingleUI(UIManager.Instance.pickupBGM);

        playerSlotData.AddSlotData(TypeData.SlotType.인벤토리, itemInfo.itemType, itemInfo.itemIndex, itemInfo.quantity);
        Destroy(itemT.gameObject);
        Destroy(gameObject);
    } 
}
