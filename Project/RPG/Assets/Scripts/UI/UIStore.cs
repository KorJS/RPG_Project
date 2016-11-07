using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIStore : MonoBehaviour
{
    private UIManager uiManager = null;
    private PlayerInfoData playerInfoData = null;
    private PlayerSlotData playerSlotDate = null;

    [System.Serializable]
    public class StoreSettings
    {
        public UILabel sellAmount;
        public UILabel buyAmount;
        public UILabel changeG;
    }

    [SerializeField]
    public StoreSettings storeSettings;

    public List<int> changInvenIndexs = null;
    private Dictionary<int, int> originalInfos = null;

    private UISlotInfo currentInfo = null;
    private TypeData.SlotType targetType = TypeData.SlotType.없음;

    private int playerGold = 0;
    private int quantity = 0;

    void Awake()
    {
        originalInfos = new Dictionary<int, int>();
        playerInfoData = PlayerInfoData.Instance;
        playerSlotDate = PlayerSlotData.Instance;
    }

    void Start()
    {
        uiManager = UIManager.Instance;
    }

    void OnEnable()
    {
        playerGold = playerInfoData.infoData.gold;
        storeSettings.changeG.text = playerGold.ToString();
    }

    void OnDisable()
    {
        ResetListSlot();
        CloseWindows();
    }

    // 정보 받아옴
    public void CopySlotInfo(UISlotInfo _currentInfo, TypeData.SlotType _targetType, int copyQuantity)
    {
        playerGold = playerInfoData.infoData.gold;

        if (_currentInfo.slotType == TypeData.SlotType.인벤토리)
        {
            // 인벤슬롯 원본 수량정보 기억해두기
            if (originalInfos.ContainsKey(_currentInfo.slotIndex))
            {
                _currentInfo.slotInfo.quantity += copyQuantity;
            }
            else
            {
                int tempQuantity = _currentInfo.slotInfo.quantity;
                if (_currentInfo.slotInfo.itemType == TypeData.ItemType.장비) { tempQuantity = 1; }
                originalInfos.Add(_currentInfo.slotIndex, tempQuantity);
            }
        }
       
        currentInfo = _currentInfo;
        targetType = _targetType;
        quantity = copyQuantity;
        CheckCopyItem();

        SetChangeG();
    }

    // 슬롯 분기
    private void CheckCopyItem()
    {
        if (targetType == TypeData.SlotType.없음)
        {
            if (currentInfo.slotType == TypeData.SlotType.구매)
            {
                BuyAmount(quantity, false);
            }
            else if (currentInfo.slotType == TypeData.SlotType.판매)
            {
                InvenRecoverItem();
                SellAmount(quantity, false);
            }
        }
        else
        {
            switch (targetType)
            {
                case TypeData.SlotType.구매:
                    {
                        // 상점리스트에서 옴 - 추가
                        AddItem(ref uiManager.buySlots);
                        BuyAmount(quantity, true);
                    }
                    break;

                case TypeData.SlotType.판매:
                    {
                        // 인벤에서 옴 - 추가
                        AddItem(ref uiManager.sellSlots);
                        SellAmount(quantity, true);
                    }
                    break;
            }
        }
    }

    // 목록 수량 추가
    private void AddItem(ref SortedDictionary<int, UISlotInfo> slots)
    {
        // 목록에 수량 추가
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
            // 빈 곳이 없으면 경고창 - 빈곳이 있으면 넣고. if문 빠져나옴
            if (!CheckEmptySlot(ref slots))
            {
                // 빈 곳도 없으면 경고창On - 슬롯부족
            }
        }
    }

    // 빈 곳에 추가
    private bool CheckEmptySlot(ref SortedDictionary<int, UISlotInfo> slots)
    {
        foreach (KeyValuePair<int, UISlotInfo> slotInfo in slots)
        {
            // 타입이 없으면 빈곳.
            if (slotInfo.Value.slotInfo.itemType == TypeData.ItemType.없음)
            {
                int index = slotInfo.Key;

                // 수량 추가
                slots[index].isItemExist = true;
                slots[index].slotInfo = currentInfo.slotInfo;
                slots[index].slotInfo.quantity = quantity;
                slots[index].StoreReSetting();

                return true;
            }
        }

        return false;
    }

    // 판매목록>인벤 복구
    private void InvenRecoverItem()
    {
        if (currentInfo.slotInfo.itemType == TypeData.ItemType.장비)
        {
            foreach (KeyValuePair<int, UISlotInfo> invenSlot in uiManager.invenSlots)
            {
                if (invenSlot.Value.isItemExist)
                {
                    Debug.Log("true : " + invenSlot.Key);
                    continue;
                }

                Debug.Log("false : " + invenSlot.Key);

                if (invenSlot.Value.slotInfo.itemType != TypeData.ItemType.장비)
                {
                    Debug.Log("장비x : " + invenSlot.Key);
                    continue;
                }

                Debug.Log("장비o : " + invenSlot.Key);

                if (invenSlot.Value.slotInfo.itemIndex != currentInfo.slotInfo.itemIndex)
                {
                    Debug.Log("장비index x : " + invenSlot.Key);
                    continue;
                }
                Debug.Log("장비index o : " + invenSlot.Key);
                int index = invenSlot.Key;

                invenSlot.Value.isItemExist = true;
                invenSlot.Value.slotInfo.quantity = 1;

                invenSlot.Value.StoreReSetting();
                break;
            }
        }
        else
        {
            foreach (KeyValuePair<int, UISlotInfo> invenSlot in uiManager.invenSlots)
            {
                if (quantity <= 0)
                {
                    return;
                }

                // 같은 타입이 없으면
                if (currentInfo.slotInfo.itemType != invenSlot.Value.slotInfo.itemType)
                {
                    continue;
                }

                // 같은 인덱스가 없으면
                if (currentInfo.slotInfo.itemIndex != invenSlot.Value.slotInfo.itemIndex)
                {
                    continue;
                }

                // 같은 타입에 같은 인덱스가 있으면
                CheckInvenQuantity(ref invenSlot.Value.slotInfo.quantity);

                invenSlot.Value.StoreReSetting();
            }
        }
    }

    // 인벤으로 복구할때 수량체크
    private void CheckInvenQuantity(ref int targetQuantity)
    {
        int total = quantity + targetQuantity;

        if (total > 99)
        {
            quantity = total - 99;
            targetQuantity = 99;
        }
        else if (total <= 99)
        {
            quantity = 0;
            targetQuantity = total;
        }
    }

    // 구매금액 설정
    private void BuyAmount(int quantity, bool isAdd)
    {
        int buyAmount = int.Parse(storeSettings.buyAmount.text);
        int total = 0;
        int buyGold = 0;

        CheckItemData(ref buyGold, true);

        if (isAdd)
        {
            total = buyAmount + (quantity * buyGold);
        }
        else
        {
            total = buyAmount - (quantity * buyGold);
        }

        storeSettings.buyAmount.text = total.ToString();
    }

    // 판매금액 설정
    private void SellAmount(int quantity, bool isAdd)
    {
        int sellAmount = int.Parse(storeSettings.sellAmount.text);
        int total = 0;
        int sellGold = 0;
        CheckItemData(ref sellGold, false);

        if (isAdd)
        {
            total = sellAmount + (quantity * sellGold);
        }
        else
        {
            total = sellAmount - (quantity * sellGold);
        }

        storeSettings.sellAmount.text = total.ToString();
    }

    // 정산 후 금액 설정
    private void SetChangeG()
    {
        int buyAmount = int.Parse(storeSettings.buyAmount.text);
        int sellAmount = int.Parse(storeSettings.sellAmount.text);
        int changeG = playerGold - buyAmount + sellAmount;

        storeSettings.changeG.text = changeG.ToString();
    }

    // 각 아이템 판매,구매 금액정보 가져옴
    private void CheckItemData(ref int itemGold, bool isGold)
    { 
        int buyGold = 0; // true
        int sellGold = 0; // false
        int index = currentInfo.slotInfo.itemIndex;

        switch (currentInfo.slotInfo.itemType)
        {
            case TypeData.ItemType.장비:
                {
                    buyGold = ItemData.Instance.equipmentInfos[index].buyGold;
                    sellGold = ItemData.Instance.equipmentInfos[index].sellGold;
                }
                break;

            case TypeData.ItemType.소모품:
                {
                    buyGold = ItemData.Instance.cusomableInfos[index].buyGold;
                    sellGold = ItemData.Instance.cusomableInfos[index].sellGold;
                }
                break;

            case TypeData.ItemType.퀘스트템:
                {
                    sellGold = ItemData.Instance.questItemInfos[index].sellGold;
                }
                break;
        }

        if (isGold) { itemGold = buyGold; }
        else { itemGold = sellGold; }
    }

    public void Calculate()
    {
        int changG = int.Parse(storeSettings.changeG.text);

        if (changG < 0)
        {
            // TODO : 경고 금액이 부족
            uiManager.popupSettings.warningPopup.SetActive(true);
            uiManager.popupSettings.warningPopup.GetComponent<UIWarningPopup>().SetMessage("보유 금액이 부족합니다.");
            return;
        }

        playerInfoData.infoData.gold = changG; // 주인공 보유골드 갱신
        uiManager.SetHoldingGold(); // 인벤 소지금 갱신

        Network_PlayerInfo.Instance.RequestSavePlayerInfo();

        // 수량 변화가 있던 슬롯 갱신
        for (int i = 0; i < changInvenIndexs.Count; i++)
        {
            int index = changInvenIndexs[i];
            UISlotInfo tempUISlotInfo = uiManager.invenSlots[index];
            playerSlotDate.SetSlotData(tempUISlotInfo.slotType, tempUISlotInfo.slotIndex, ref tempUISlotInfo);
        }

        // 구매한 아이템 인벤에 추가
        foreach (KeyValuePair<int, UISlotInfo> buyInfo in uiManager.buySlots)
        {
            // 아이템 인덱스가 없으면 리턴 - 인덱스가 있으면 아이템이 있다는 뜻.
            if (buyInfo.Value.slotInfo.itemIndex == -1)
            {
                continue;
            }
            TypeData.ItemType itemType = buyInfo.Value.slotInfo.itemType;
            int itemIndex = buyInfo.Value.slotInfo.itemIndex;
            int itemQuantity = buyInfo.Value.slotInfo.quantity;

            playerSlotDate.AddSlotData(TypeData.SlotType.인벤토리, itemType, itemIndex, itemQuantity);

            buyInfo.Value.ReSetting();
            buyInfo.Value.SetQuantity();
        }

        ResetSlot(uiManager.buySlots);
        ResetSlot(uiManager.sellSlots);

        storeSettings.buyAmount.text = "0";
        storeSettings.sellAmount.text = "0";

        originalInfos.Clear();
        changInvenIndexs.Clear();
    }

    // 정산
    public void CalculateOK()
    {
        int buyAmount = int.Parse(storeSettings.buyAmount.text);
        int sellAmount = int.Parse(storeSettings.sellAmount.text);
        int changeG = int.Parse(storeSettings.changeG.text);
        Debug.Log("buyAmount : " + buyAmount + " sellAmount : " + sellAmount + " changeG : " + changeG);
        Network_Store.Instance.RequestCalculate(buyAmount, sellAmount, changeG);
    }

    public void ResetSlot(SortedDictionary<int, UISlotInfo> slotInfos)
    {
        foreach (KeyValuePair<int, UISlotInfo> slotInfo in slotInfos)
        {
            slotInfo.Value.isItemExist = false;
            slotInfo.Value.slotInfo.itemIndex = -1;
            slotInfo.Value.slotInfo.itemType = TypeData.ItemType.없음;
            slotInfo.Value.slotInfo.quantity = 0;

            slotInfo.Value.StoreReSetting();
        }
    }

    public void ResetListSlot()
    {
        foreach (KeyValuePair<int, UISlotInfo> slotList in uiManager.storeListSlots)
        {
            Debug.Log(slotList.Key);
            slotList.Value.isItemExist = false;
            slotList.Value.slotInfo.itemIndex = -1;
            slotList.Value.slotInfo.itemType = TypeData.ItemType.없음;
            slotList.Value.slotInfo.quantity = 0;

            slotList.Value.StoreReSetting();
        }
    }

    public void CloseWindows()
    {
        if (originalInfos.Count > 0)
        {
            // 취소한 경우 원본 정보 복구
            foreach (KeyValuePair<int, int> originalInfo in originalInfos)
            {
                int index = originalInfo.Key;
                Debug.Log("originalInfo.Key : " + originalInfo.Key + ", originalInfo.Value : " + originalInfo.Value);
                uiManager.invenSlots[index].isItemExist = true;
                uiManager.invenSlots[index].slotInfo.quantity = originalInfo.Value;
                uiManager.invenSlots[index].StoreReSetting();
                currentInfo.StoreReSetting();
            }

            originalInfos.Clear();
        }

        changInvenIndexs.Clear();

        storeSettings.buyAmount.text = "0";
        storeSettings.sellAmount.text = "0";
        storeSettings.changeG.text = playerInfoData.infoData.gold.ToString();

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}