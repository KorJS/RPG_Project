using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIInquirePopup : MonoBehaviour
{
    private PlayerSlotData  playerSlotData  = null; // 주인공 슬롯 정보

    public UILabel          message         = null; // 출력할 메시지
    public UISlotInfo       currentInfo     = null; // 현제 슬롯 정보

    void Awake()
    {
        playerSlotData  = PlayerSlotData.Instance;
        message         = transform.FindChild("Message").GetComponent<UILabel>();
    }

    public void SetMessage(UISlotInfo _currentInfo, string _message)
    {
        currentInfo  = _currentInfo;
        message.text = _message;
    }

    public void InquireOK()
    {
        switch (currentInfo.slotType)
        {
            case TypeData.SlotType.인벤토리:
                {
                    currentInfo.isCoolTime = false;
                    currentInfo.coolTimer = 0f;
                    currentInfo.slotInfo.quantity = 0;
                    playerSlotData.SetSlotData(TypeData.SlotType.인벤토리, currentInfo.slotIndex, ref currentInfo);
                    currentInfo.ReSetting();
                }
                break;

            case TypeData.SlotType.스킬리스트:
                {
                    PlayerSkillData.Instance.AddSkillData(currentInfo.slotInfo.skillIndex);
                    currentInfo.slotSettings.upBtnObj.SetActive(false);
                    currentInfo.ReSetting();
                }
                break;
        }
        
        gameObject.SetActive(false);
    }

    public void InquireCancel()
    {
        gameObject.SetActive(false);
    }
}