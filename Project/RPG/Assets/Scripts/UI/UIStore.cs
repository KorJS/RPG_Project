using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIStore : MonoBehaviour
{
    private UIManager uiManager = null;

    [System.Serializable]
    public class StoreSettings
    {
        public UILabel sellAmount;
    }

    [SerializeField]
    public StoreSettings storeSettings;

    public Dictionary<int, int> originalInfos = null;
    private UISlotInfo currentInfo = null;
    private UISlotInfo targetInfo = null;

    private int quantity = 0;

    void Awake()
    {
        originalInfos = new Dictionary<int, int>();
    }

    void Start()
    {
        uiManager = UIManager.Instance;
    }

    void OnDisable()
    {
        CalculateCancel();
    }

    public void CopySlotInfo(UISlotInfo _currentInfo, UISlotInfo _targetInfo, int copyQuantity)
    {
        if (_currentInfo.slotType == TypeData.SlotType.인벤토리)
        {
            // 인벤슬롯 원본 수량정보 기억해두기
            if (originalInfos.ContainsKey(_currentInfo.slotIndex))
            {
                originalInfos[_currentInfo.slotIndex] += _currentInfo.slotInfo.quantity;
            }
            else
            {
                originalInfos.Add(_currentInfo.slotIndex, _currentInfo.slotInfo.quantity);
            }
        }
        currentInfo = _currentInfo;
        targetInfo = _targetInfo;
        quantity = copyQuantity;
        CheckCopyItem();
    }

    // 정산하면 데이터 처리.
    private void CheckCopyItem()
    {
        switch (targetInfo.slotType)
        {
            case TypeData.SlotType.인벤토리:
                {
                    // 판매목록 수량만큼 감소
                    //AddItem(ref uiManager.sellSlots);
                }
                break;

            case TypeData.SlotType.상점리스트:
                {
                    // 구매목록 수량만큼 감소
                    //AddItem(ref uiManager.buySlots);
                }
                break;

            case TypeData.SlotType.구매:
                {
                    // 상점리스트에서 옴 - 추가
                    AddItem(ref uiManager.buySlots);
                }
                break;

            case TypeData.SlotType.판매:
                {
                    // 인벤에서 옴 - 추가
                    AddItem(ref uiManager.sellSlots);
                }
                break;
        }
    }

    // 목록 수량 추가
    private void AddItem(ref SortedDictionary<int, UISlotInfo> slots)
    {
        Debug.Log(slots.Count);
        bool isExist = false;

        if (currentInfo.slotInfo.itemType == TypeData.ItemType.장비)
        {
            CheckEmptySlot(ref slots);
            return;
        }

        foreach (KeyValuePair<int, UISlotInfo> slotInfo in slots)
        {
            // 목록에 같은 타입이 없으면
            if (currentInfo.slotInfo.itemType != slotInfo.Value.slotInfo.itemType)
            {
                continue;
            }

            // 같은 인덱스가 없으면
            if (currentInfo.slotInfo.itemIndex != slotInfo.Value.slotInfo.itemIndex)
            {
                continue;
            }

            // 존재하면 그 슬롯에 추가
            isExist = true;

            int index = slotInfo.Key;

            // 수량 추가
            slots[index].slotInfo.quantity += quantity;
            slots[index].StoreReSetting();
            
            return;
        }

        // 판매 목록에 아무것도 존재 안하면 빈곳에 추가
        if (!isExist)
        {
            CheckEmptySlot(ref slots);
        }
    }

    private void CheckEmptySlot(ref SortedDictionary<int, UISlotInfo> slots)
    {
        foreach (KeyValuePair<int, UISlotInfo> slotInfo in slots)
        {
            // 타입이 없으면 빈곳.
            if (slotInfo.Value.slotInfo.itemType == TypeData.ItemType.없음)
            {
                int index = slotInfo.Key;

                // 수량 추가
                slots[index].isItemExist = currentInfo.isItemExist;
                slots[index].slotInfo = currentInfo.slotInfo;
                slots[index].slotInfo.quantity = quantity;
                slots[index].StoreReSetting();
                break;
            }
        }
    }

    private void BuyAmount()
    {

    }

    private void SellAmount()
    {

    }

    // 정산
    public void CalculateOK()
    {

    }

    public void CalculateCancel()
    {
        // 취소한 경우 원본 정보 복구
        foreach (KeyValuePair<int, int> originalInfo in originalInfos)
        {
            int index = originalInfo.Key;

            uiManager.invenSlots[index].isItemExist = true;
            uiManager.invenSlots[index].slotInfo.quantity = originalInfo.Value;

            currentInfo.StoreReSetting();
        }

        originalInfos.Clear();

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}