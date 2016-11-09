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

    public UIButton uiH = null;

    void Awake()
    {
        uiHover = transform.FindChild("BG").GetComponent<UISprite>();
        uiHover.alpha = 0.5f;
    }

    void Start()
    {
        playerSlotData = PlayerSlotData.Instance;
    }

    void OnClick()
    {
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
        if (!itemControl.itemSettings.isPlayer)
        {
            return;
        }
        Debug.Log(itemControl.itemSettings.isPlayer);

        if (isHover)
        {
            uiHover.alpha = 1f;
        }
        else
        {
            uiHover.alpha = 0.5f;
        }
    }

    public void SetItemInfo(Transform _itemT, ItemControl _itemControl, TypeData.ItemType itemType, int itemIndex, int quantity)
    {
        itemT = _itemT;
        itemControl = _itemControl;
        itemInfo.itemType = itemType;
        itemInfo.itemIndex = itemIndex;
        itemInfo.quantity = quantity;
    }

    public void DropItemClick()
    {
        if (!itemControl.itemSettings.isPlayer)
        {
            return;
        }
        playerSlotData.AddSlotData(TypeData.SlotType.인벤토리, itemInfo.itemType, itemInfo.itemIndex, itemInfo.quantity);
        Destroy(itemT.gameObject);
        Destroy(gameObject);
    }

    private void SetPosition()
    {
        Vector3 p = Camera.main.WorldToViewportPoint(itemT.position);
        transform.position = UICamera.mainCamera.ViewportToWorldPoint(p);

        p = transform.localPosition;
        p.x = Mathf.RoundToInt(p.x);
        p.y = Mathf.RoundToInt(p.y);
        p.z = 0f;

        transform.localPosition = p;
    }

  
}
