using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICopyPopup : MonoBehaviour
{
    private PlayerSlotData  playerSlotData  = null; // 주인공 슬롯 정보
    private UIManager       uiManager       = null; // UI 매니저
    private UIStore         uiStore         = null; // 상점
    private UIInput         copyQuantity_Input = null; // 입력 수량

    private int             copyQuantityMAX = 0;    // 옮길수 있는 최대 수량(현재슬롯 수량)
    private int             copyQuantity    = 0;    // 분리 창에 적은 분리할 수량

    private UISlotInfo      currentInfo     = null; // 현재 슬롯정보
    private UISlotInfo      targetInfo      = null; // 타겟 슬롯정보

    void Awake()
    {
        playerSlotData      = PlayerSlotData.Instance;
        uiManager           = UIManager.Instance;
        copyQuantity_Input  = transform.FindChild("Input").GetComponent<UIInput>();
    }

    void Start()
    {
        uiStore = uiManager.windowSettings.storeObj.GetComponent<UIStore>();
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
        currentInfo = null;
        targetInfo = null;

        currentInfo = _currentInfo;
        targetInfo = _targetInfo;

        if (_currentInfo.slotType == TypeData.SlotType.상점리스트)
        {
            copyQuantityMAX = 999;
            return;
        }
        // 처음 표시 : 현재 옮길수 최대 수량
        copyQuantityMAX = currentInfo.slotInfo.quantity;

        Debug.Log("DragAndDropInfo : " + currentInfo.slotInfo.quantity);
    }

    // 분리할 수량 변화가 생기면 호출되는 함수
    public void ChangCopyPopupText()
    {
        copyQuantity = int.Parse(copyQuantity_Input.label.text);

        if (copyQuantity > copyQuantityMAX)
        {
            copyQuantity_Input.value = copyQuantityMAX.ToString();
            copyQuantity = copyQuantityMAX;
        }

        Debug.Log("copyQuantityMAX : " + copyQuantityMAX + " copyQuantity : " + copyQuantity);
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

    // 완료
    public void CopyOk()
    {
        // 분리수량 0인데 확인 누른경우 걍 종료
        if (copyQuantity <= 0)
        {
            CopyCancel();
            return;
        }

        CheckCopyItem();
    }

    // 취소
    public void CopyCancel()
    {
        uiManager.DisableDragIiem();
        gameObject.SetActive(false);
    }
    
    // 아이템 체크
    private void CheckCopyItem()
    {
        switch (currentInfo.slotType)
        {
            case TypeData.SlotType.인벤토리:
                {
                    // 판매목록 수량만큼 추가
                    uiStore.CopySlotInfo(currentInfo, targetInfo.slotType, copyQuantity);

                    // 인벤 수량 변화
                    currentInfo.slotInfo.quantity -= copyQuantity;

                    if (!uiStore.changInvenIndexs.Contains(currentInfo.slotIndex))
                    {
                        uiStore.changInvenIndexs.Add(currentInfo.slotIndex); // 수량 변화 생긴 슬롯 인덱스 저장 - 정산할때 그 슬롯들 갱신
                    }

                    if (currentInfo.slotInfo.quantity == 0)
                    {
                        currentInfo.isExist = false;
                        currentInfo.StoreReSetting();
                        CopyCancel();
                        return;
                    }
                    currentInfo.SetQuantity();
                }
                break;

            case TypeData.SlotType.상점리스트:
                {
                    // 구매목록 수량만큼 추가
                    uiStore.CopySlotInfo(currentInfo, targetInfo.slotType, copyQuantity);
                }
                break;

            case TypeData.SlotType.구매:
            case TypeData.SlotType.판매:
                {
                    // 수량 만큼 제거
                    uiStore.CopySlotInfo(currentInfo, TypeData.SlotType.없음, copyQuantity);

                    // 구매, 판매목록 수량 변화
                    currentInfo.slotInfo.quantity -= copyQuantity;

                    Debug.Log("currentInfo.slotInfo.quantity  : " + currentInfo.slotInfo.quantity);

                    if (currentInfo.slotInfo.quantity == 0)
                    {
                        currentInfo.isExist = false;
                        currentInfo.slotInfo.itemIndex = -1;
                        currentInfo.slotInfo.itemType = TypeData.ItemType.없음;
                        currentInfo.slotInfo.quantity = 0;
                        currentInfo.StoreReSetting();
                        CopyCancel();
                        return;
                    }

                    currentInfo.SetQuantity();
                }
                break;
        }

        CopyCancel();
    }
}
