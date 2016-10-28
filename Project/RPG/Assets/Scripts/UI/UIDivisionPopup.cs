using UnityEngine;
using System.Collections;

public class UIDivisionPopup : MonoBehaviour
{
    private UIManager uiManager = null;
    private PlayerSlotData playerSlotData = null;

    private UIInput divQuantity_Input = null; // 입력 수량

    private int divQuantityMAX = 0; // 옮길수 있는 최대 수량(현재슬롯 수량)
    private int divQuantity = 0; // 분리 창에 적은 분리할 수량

    public UISlotInfo currentInfo = null;
    public UISlotInfo targetInfo = null;

    void Awake()
    {
        uiManager = UIManager.Instance;
        playerSlotData = PlayerSlotData.Instance;
        divQuantity_Input = transform.FindChild("Input").GetComponent<UIInput>();
    }

    void OnEnable()
    {
        divQuantity_Input.value = "0";
    }

    // 비활성화 되면 내용 초기화
    void OnDisable()
    {
        currentInfo = null;
        targetInfo = null;
    }

    public void DragAndDropInfo(UISlotInfo _currentSlot, UISlotInfo _targetSlot)
    {
        currentInfo = _currentSlot;
        targetInfo = _targetSlot;

        // 처음 표시 : 현재 옮길수 최대 수량
        divQuantityMAX = currentInfo.slotInfo.quantity;
    }

    // 분리할 수량 변화가 생기면 호출되는 함수
    public void ChangDivPopupText()
    {
        Debug.Log("divQuantityMAX : " + divQuantityMAX + " divQuantity : " + divQuantity);
        divQuantity = int.Parse(divQuantity_Input.label.text);

        if (divQuantity > divQuantityMAX)
        {
            divQuantity_Input.value = divQuantityMAX.ToString();
            divQuantity = divQuantityMAX;
        }
    }

    // 수량 증가버튼 눌렀을시 호출되는 함수
    public void Up_Button()
    {
        divQuantity += 1;
        if (divQuantity >= divQuantityMAX)
        {
            divQuantity_Input.value = divQuantityMAX.ToString();
            divQuantity = divQuantityMAX;
            return;
        }

        divQuantity_Input.value = divQuantity.ToString();
    }

    // 수량 감소버튼 눌렀을시 호출되는 함수
    public void Down_Button()
    {
        divQuantity -= 1;
        if (divQuantity <= 0)
        {
            divQuantity_Input.value = "0";
            divQuantity = 0;
            return;
        }

        divQuantity_Input.value = divQuantity.ToString();
    }

    // 인벤토리 -> 창고 (소모품, 퀘템인경우 : 분리 창 On - 분리창 수량 만큼 수량검사, 
    //                 (                    같은 아이템타입, 같은 아이템인덱스, 합치고, 나머지는 현슬롯에 남김, 다옴기는거면 현슬롯 제거)
    //                 (                    같은 아이템타입, 다른 아이템인덱스, 순차대로 빈곳에 추가후 나머지는 현재슬롯에 남김 - 다 옴기는거면 현재슬롯은 제거)
    //                 (                    다른 아이템타입 - 분리 창 On - 분리 창에 적은 수량만큼 순차대로 빈곳에 추가후 나머지는 현재슬롯에 남김 - 다 옴기는거면 현재슬롯은 제거)
    //                 (장비 인경우 : 빈곳에 추가 - 현재 슬롯 제거)
    // 창고 -> 인벤토리 (소모품, 퀘템인경우 : 같은 아이템타입, 같은 아이템인덱스 - 분리 창 On - 분리창 수량 만큼 수량검사후 합치고, 나머지는 현슬롯에 남김, 다옴기는거면 현슬롯 제거)
    //                 (                    같은 아이템타입, 다른 아이템인덱스 - 분리 창 On - 분리창 수량 만큼 순차대로 빈곳에 추가후 나머지는 현재슬롯에 남김 - 다 옴기는거면 현재슬롯은 제거)
    //                 (                    다른 아이템타입 - 분리 창 On - 분리 창에 적은 수량만큼 순차대로 빈곳에 추가후 나머지는 현재슬롯에 남김 - 다 옴기는거면 현재슬롯은 제거)
    //                 (장비 인경우 : 빈곳에 추가 - 현재 슬롯 제거)
    // 분리 설정후 확인 버튼 눌렀을때 호출되는 데이터 처리할 함수
    public void DivisionOK()
    {
        // 분리수량 0인데 확인 누른경우 걍 종료
        if (divQuantity <= 0)
        {
            DivisionCancel();
            return;
        }
        // TODO : 분리창수량이 창고에 넣었을때 그 수량이 MAX 치 채우고 남으면 빈곳에 추가
        // 수량만 알려주고 PlayerSlotData.DivisionSlotData 에서 알아서 처리
        if (divQuantity == currentInfo.slotInfo.quantity)
        {
            currentInfo.slotInfo.quantity = 0;
        }
        else if (divQuantity < currentInfo.slotInfo.quantity)
        {
            currentInfo.slotInfo.quantity -= divQuantity;
        }

        // 대상 바꿈
        playerSlotData.DivisionSlotData(targetInfo.slotType, currentInfo.slotInfo.itemType, currentInfo.slotInfo.itemIndex, divQuantity);
        // 현재꺼는 따로 수정
        playerSlotData.SetSlotData(currentInfo.slotType, currentInfo.slotIndex, ref currentInfo);

        currentInfo.ReSetting();

        gameObject.SetActive(false);
    }

    // 분리 취소버튼 눌렀을시 호출되는 함수
    public void DivisionCancel()
    {
        uiManager.DisableDragIiem(); // 복사한거 삭제
        gameObject.SetActive(false);
    }
}