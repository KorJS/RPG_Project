using UnityEngine;
using System.Collections;

public class UIInventory : MonoBehaviour
{
    private PlayerInfoData playerInfoData = null;
    public UILabel inventoryGold = null;

    void Awake()
    {
        playerInfoData = PlayerInfoData.Instance;
        inventoryGold = transform.FindChild("Gold").FindChild("Amount").GetComponent<UILabel>();
    }

    void OnEnable()
    {
        SetHoldingGold();
    }

    public void SetHoldingGold()
    {
        inventoryGold.text = playerInfoData.infoData.glod.ToString();
    }

    public void CloseWindows()
    {
        gameObject.SetActive(false);
    }
}