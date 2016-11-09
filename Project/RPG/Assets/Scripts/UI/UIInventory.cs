using UnityEngine;
using System.Collections;

public class UIInventory : MonoBehaviour
{
    private PlayerInfoData playerInfoData = null;
    private UIPanel inventoryPanel = null;
    public UILabel inventoryGold = null;

    void Awake()
    {
        playerInfoData = PlayerInfoData.Instance;
        inventoryPanel = GetComponent<UIPanel>();
        inventoryGold = transform.FindChild("Gold").FindChild("Amount").GetComponent<UILabel>();
    }

    void OnEnable()
    {
        SetHoldingGold();
    }

    public void SetHoldingGold()
    {
        inventoryGold.text = playerInfoData.infoData.gold.ToString();
    }

    public void CloseWindows()
    {
        inventoryPanel.alpha = 0f;
    }
}