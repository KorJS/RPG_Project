using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCInfoData : MonoBehaviour
{
    private UIManager uiManager = null;
    private ItemData itemData = null;
    private StoreItemListData storeItemListData = null;
    // NPC - 위치는 정해져 있음
    // 어느 지역인지
    // 어느 상점인지
    // 일정 범위안에 들어오면 이펙트 / Label(대화를 하려면 'F'를 누르십시오.) 활성화
    [System.Serializable]
    public class NPCInfo
    {
        public TypeData.NPCType npcType;
        public TypeData.StoreType storeType;
        public int questIndex;
        public List<int> itemIndexs;
    }

    [SerializeField]
    public NPCInfo npcInfo;

    // 상점창 UI
    public GameObject storeObj = null;
    private UISlotInfo tempSlotInfo = null;
    public bool isPlayer = false;

    void Awake()
    {
        uiManager = UIManager.Instance;
        itemData = ItemData.Instance;
        storeItemListData = StoreItemListData.Instance;
        SetNPCInfo();
    }

    void Update()
    {
        if (!uiManager.isUIMode)
        {
            storeObj.SetActive(false);
        }
    }

    private void SetNPCInfo()
    {
        switch (npcInfo.npcType)
        {
            case TypeData.NPCType.상인:
                {
                    storeObj = GameObject.Find("Store_Panel");
                    npcInfo.itemIndexs = storeItemListData.itemListInfos[npcInfo.storeType];
                    SetStoreSlotInfo();
                }
                break;

            case TypeData.NPCType.퀘스트:
                {
                    // TODO : 퀘스트 정보
                }
                break;
        }
    }

    private void SetStoreSlotInfo()
    {
        int slotIndex = 0;

        foreach (KeyValuePair<int, UISlotInfo> uiSlotInfo in uiManager.storeListSlots)
        {
            slotIndex = uiSlotInfo.Key;
            tempSlotInfo = uiSlotInfo.Value;

            if (npcInfo.storeType == TypeData.StoreType.장비점)
            {
                int itemIndex = npcInfo.itemIndexs[slotIndex];
                tempSlotInfo.slotInfo.iconName = itemData.equipmentInfos[itemIndex].name;
                tempSlotInfo.slotInfo.itemIndex = itemIndex;
                tempSlotInfo.slotInfo.itemType = TypeData.ItemType.장비;
                tempSlotInfo.slotInfo.quantity = 1;
                tempSlotInfo.slotSettings.uiSellGold.text = itemData.equipmentInfos[itemIndex].sellGold.ToString();
            }
            else if (npcInfo.storeType == TypeData.StoreType.소모품점)
            {
                int itemIndex = npcInfo.itemIndexs[slotIndex];
                tempSlotInfo.slotInfo.iconName = itemData.cusomableInfos[itemIndex].name;
                tempSlotInfo.slotInfo.itemIndex = itemIndex;
                tempSlotInfo.slotInfo.itemType = TypeData.ItemType.소모품;
                tempSlotInfo.slotInfo.quantity = 1;
                tempSlotInfo.slotSettings.uiSellGold.text = itemData.cusomableInfos[itemIndex].sellGold.ToString();
            }
        }

        uiManager.storeListSlots[slotIndex] = tempSlotInfo;
    }

    // 주인공이 근처에 왔는지
    private void CheckPlayer()
    {

    }
}
