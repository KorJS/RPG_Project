﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICopyPopup : MonoBehaviour
{
    private UIManager uiManager = null;
    private UIInput copyQuantity_Input = null; // 입력 수량

    private int copyQuantityMAX = 0; // 옮길수 있는 최대 수량(현재슬롯 수량)
    private int copyQuantity = 0; // 분리 창에 적은 분리할 수량

    private UISlotInfo currentInfo = null;
    private UISlotInfo targetInfo = null;

    void Awake()
    {
        copyQuantity_Input = transform.FindChild("Input").GetComponent<UIInput>();
    }

    void Start()
    {
        uiManager = UIManager.Instance;
    }

    void OnEnable()
    {
        copyQuantity_Input.value = "0";
    }

    // 비활성화 되면 내용 초기화
    void OnDisable()
    {
        currentInfo = null;
        targetInfo = null;
    }

    public void DragAndDropInfo(UISlotInfo _currentInfo, UISlotInfo _targetInfo)
    {
        currentInfo = _currentInfo;
        targetInfo = _targetInfo;

        if (_currentInfo.slotType == TypeData.SlotType.상점리스트)
        {
            copyQuantityMAX = 999;
            return;
        }
        // 처음 표시 : 현재 옮길수 최대 수량
        copyQuantityMAX = currentInfo.slotInfo.quantity;
    }

    // 분리할 수량 변화가 생기면 호출되는 함수
    public void ChangCopyPopupText()
    {
        Debug.Log("copyQuantityMAX : " + copyQuantityMAX + " copyQuantity : " + copyQuantity);
        copyQuantity = int.Parse(copyQuantity_Input.label.text);

        if (copyQuantity > copyQuantityMAX)
        {
            copyQuantity_Input.value = copyQuantityMAX.ToString();
            copyQuantity = copyQuantityMAX;
        }
    }

    // 수량 증가버튼 눌렀을시 호출되는 함수
    public void Up_Button()
    {
        copyQuantity += 1;
        if (copyQuantity >= copyQuantityMAX)
        {
            copyQuantity_Input.value = copyQuantityMAX.ToString();
            copyQuantity = copyQuantityMAX;
            return;
        }

        copyQuantity_Input.value = copyQuantity.ToString();
    }

    // 수량 감소버튼 눌렀을시 호출되는 함수
    public void Down_Button()
    {
        copyQuantity -= 1;
        if (copyQuantity <= 0)
        {
            copyQuantity_Input.value = "0";
            copyQuantity = 0;
            return;
        }

        copyQuantity_Input.value = copyQuantity.ToString();
    }

    public void CopyOk()
    {
        CheckCopyItem();
    }

    public void CopyCancel()
    {
        uiManager.DisableDragIiem();
        gameObject.SetActive(false);
    }
    
    // 정산하면 데이터 처리.
    private void CheckCopyItem()
    {
        switch (currentInfo.slotType)
        {
            case TypeData.SlotType.인벤토리:
                {
                    // 판매목록 수량만큼 보냄
                    uiManager.windowSettings.storeObj.GetComponent<UIStore>().CopySlotInfo(currentInfo, targetInfo, copyQuantity);

                    currentInfo.slotInfo.quantity -= copyQuantity;

                    if (currentInfo.slotInfo.quantity == 0)
                    {
                        currentInfo.isItemExist = false;
                        currentInfo.StoreReSetting();
                        CopyCancel();
                        return;
                    }
                    currentInfo.SetQuantity();
                }
                break;

            case TypeData.SlotType.상점리스트:
                {
                    Debug.Log("?");
                    // 구매목록 수량만큼 보냄
                    uiManager.windowSettings.storeObj.GetComponent<UIStore>().CopySlotInfo(currentInfo, targetInfo, copyQuantity);
                }
                break;

            case TypeData.SlotType.구매:
            case TypeData.SlotType.판매:
                {
                    // 수량 만큼 제거
                    //currentInfo.slotInfo.quantity -= copyQuantity;
                }
                break;
        }

        CopyCancel();
    }
}
