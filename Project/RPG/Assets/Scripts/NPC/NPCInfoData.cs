using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NPCMovement))]
[RequireComponent(typeof(NPCRange))]
public class NPCInfoData : MonoBehaviour
{
    private UIManager uiManager = null;
    private ItemData itemData = null;
    private StoreItemListData storeItemListData = null;
    private NPCMovement npcMovement = null;
    private NPCRange npcRange = null;

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
    [SerializeField]

    private UISlotInfo tempSlotInfo = null;

    void Awake()
    {
        uiManager = UIManager.Instance;
        itemData = ItemData.Instance;
        storeItemListData = StoreItemListData.Instance;

        npcMovement = GetComponent<NPCMovement>();
        npcRange = GetComponent<NPCRange>();

        SetNPCInfo();
    }

    void Update()
    {
        if (GameManager.Instance.gameState == TypeData.GameState.종료)
        {
            return;
        }

        if (npcRange.isPlayer && Input.GetKeyDown(KeyCode.F))
        {
            npcMovement.SetAniState();

            if (npcInfo.npcType == TypeData.NPCType.상인)
            {
                uiManager.isStore = true;
                SetStoreSlotInfo();
            }
            else if (npcInfo.npcType == TypeData.NPCType.창고)
            {
                uiManager.isStorage = true;
            }
        }
    }

    private void SetNPCInfo()
    {
        switch (npcInfo.npcType)
        {
            case TypeData.NPCType.상인:
                {
                    npcInfo.itemIndexs = storeItemListData.itemListInfos[(int)npcInfo.storeType];
                }
                break;

            case TypeData.NPCType.퀘스트:
                {
                    // TODO : 퀘스트 정보
                }
                break;
        }
    }

    public void SetStoreSlotInfo()
    {
        int slotIndex = 0;

        foreach (KeyValuePair<int, UISlotInfo> uiSlotInfo in uiManager.storeListSlots)
        {
            slotIndex = uiSlotInfo.Key;
            tempSlotInfo = uiSlotInfo.Value;
            Debug.Log("slotIndex : " + slotIndex + " : " + uiSlotInfo.Value);

            if (npcInfo.storeType == TypeData.StoreType.장비점)
            {
                if (slotIndex >= npcInfo.itemIndexs.Count)
                {
                    tempSlotInfo.isExist = false;
                    Debug.Log(slotIndex + " : 슬롯에 들어갈 아이템이 없습니다.");
                    continue;
                }
                Debug.Log("slotIndex : " + slotIndex + " count : " + npcInfo.itemIndexs.Count);
                int itemIndex = npcInfo.itemIndexs[slotIndex];

                if (!itemData.equipmentInfos.ContainsKey(itemIndex))
                {
                    tempSlotInfo.isExist = false;
                    Debug.Log(itemIndex + " : 아이템 정보가 존재 하지 않습니다.");
                    continue;
                }

                tempSlotInfo.isExist = true;
                tempSlotInfo.slotInfo.name = itemData.equipmentInfos[itemIndex].name;
                tempSlotInfo.slotInfo.iconName = itemData.equipmentInfos[itemIndex].iconName;
                tempSlotInfo.slotInfo.itemIndex = itemIndex;
                tempSlotInfo.slotInfo.itemType = TypeData.ItemType.장비;
                tempSlotInfo.slotInfo.quantity = 1;
                tempSlotInfo.slotSettings.uiBuyGold.text = itemData.equipmentInfos[itemIndex].buyGold.ToString();
            }
            else if (npcInfo.storeType == TypeData.StoreType.소모품점)
            {
                if (slotIndex >= npcInfo.itemIndexs.Count)
                {
                    tempSlotInfo.isExist = false;
                    Debug.Log(slotIndex + " : 슬롯에 들어갈 아이템이 없습니다.");
                    continue;
                }
                Debug.Log("slotIndex : " + slotIndex + " count : " + npcInfo.itemIndexs.Count);
                int itemIndex = npcInfo.itemIndexs[slotIndex];

                if (!itemData.cusomableInfos.ContainsKey(itemIndex))
                {
                    tempSlotInfo.isExist = false;
                    Debug.Log(itemIndex + " : 아이템 정보가 존재 하지 않습니다.");
                    continue;
                }

                tempSlotInfo.isExist = true;
                tempSlotInfo.slotInfo.name = itemData.cusomableInfos[itemIndex].name;
                tempSlotInfo.slotInfo.iconName = itemData.cusomableInfos[itemIndex].iconName;
                tempSlotInfo.slotInfo.itemIndex = itemIndex;
                tempSlotInfo.slotInfo.itemType = TypeData.ItemType.소모품;
                tempSlotInfo.slotInfo.quantity = 1;
                tempSlotInfo.slotSettings.uiBuyGold.text = itemData.cusomableInfos[itemIndex].buyGold.ToString();
            }
        }

        uiManager.storeListSlots[slotIndex] = tempSlotInfo;
        uiManager.storeListSlots[slotIndex].ReSetting();
    }
}
