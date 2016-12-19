using UnityEngine;
using System.Collections;

public class UIInventory : MonoBehaviour
{
    private PlayerInfoData  playerInfoData = null; // 주인공 슬롯 정보
    private UIPanel         inventoryPanel = null; // 인벤토리 판넬
    public  UILabel         inventoryGold  = null; // 소지금

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

    // 소지금 설정
    public void SetHoldingGold()
    {
        inventoryGold.text = playerInfoData.infoData.gold.ToString();
    }

    public void CloseWindows()
    {
        inventoryPanel.alpha = 0f;
    }
}