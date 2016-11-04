using UnityEngine;
using System.Collections;

public class UIInquirePopup : MonoBehaviour
{
    private PlayerSlotData playerSlotData = null;

    public UILabel message = null;
    public UISlotInfo currentInfo = null;

    void Awake()
    {
        playerSlotData = PlayerSlotData.Instance;

        message = transform.FindChild("Message").GetComponent<UILabel>();
    }

    public void SetMessage(UISlotInfo _currentInfo, string _message)
    {
        currentInfo = _currentInfo;
        message.text = _message;
    }

    public void InquireOK()
    {
        playerSlotData.RemoveSlotData(currentInfo);
        Network_Slot.Instance.RequestDeleteSlot(currentInfo.slotType, currentInfo.slotIndex);
        currentInfo.ReSetting();
        gameObject.SetActive(false);
    }

    public void InquireCancel()
    {
        gameObject.SetActive(false);
    }
}