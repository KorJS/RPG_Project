using UnityEngine;
using System.Collections;

public class UIStore : MonoBehaviour
{
    [System.Serializable]
    public class StoreSettings
    {
    }

    [SerializeField]
    public StoreSettings storeSettings;

    public UISlotInfo currentInfo = null;
    public UISlotInfo targetInfo = null;

    // 인벤 > 판매로
    public void DragAndDropInfo(UISlotInfo _currentSlot, UISlotInfo _targetInfo)
    {
        currentInfo = _currentSlot;
        targetInfo = _targetInfo;
    }

    private void BuyAmount()
    {

    }

    private void SellAmount()
    {

    }
}