using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NPCMovement))]
[RequireComponent(typeof(NPCRange))]
public class NPCInfoData : MonoBehaviour
{
    private UIManager           uiManager           = null; // UI 매니저
    private ItemData            itemData            = null; // 아이템 정보
    private StoreItemListData   storeItemListData   = null; // 상점 리스트 정보
    private NPCMovement         npcMovement         = null; // NPC 동작
    private NPCRange            npcRange            = null; // NPC 범위

    // NPC - 위치는 정해져 있음
    // 어느 지역인지
    // 어느 상점인지
    // 일정 범위안에 들어오면 이펙트 / Label(대화를 하려면 'F'를 누르십시오.) 활성화
    [System.Serializable]
    public class NPCInfo
    {
        public TypeData.NPCType     npcType;    // NPC 타입
        public TypeData.StoreType   storeType;  // 상점 타입
        public List<int>            itemIndexs; // 아이템 인덱스 리스트
        public AudioClip[]          talkBGM;    // NPC 대화 사운드
    }

    [SerializeField]
    public NPCInfo npcInfo;

    // 상점창 UI
    [SerializeField]

    private UISlotInfo tempSlotInfo = null;

    void Awake()
    {
        uiManager           = UIManager.Instance;
        itemData            = ItemData.Instance;
        storeItemListData   = StoreItemListData.Instance;

        npcMovement         = GetComponent<NPCMovement>();
        npcRange            = GetComponent<NPCRange>();

        SetNPCInfo();
    }

    void Update()
    {
        if (GameManager.Instance.currentGameState == TypeData.GameState.종료)
        {
            return;
        }

        // 주인공이 범위에 있을떄 F키를 누르면
        if (npcRange.isPlayer && (Input.GetKeyDown(KeyCode.F) || uiManager.isFKey))
        {
            // 대화 모션 애니매이션
            npcMovement.SetAniState();

            // 사운드 출력
            SoundManager.Instance.RandomVoice(npcInfo.talkBGM);

            // NPC 타입이 상인이면
            if (npcInfo.npcType == TypeData.NPCType.상인)
            {
                uiManager.isStore = true;
                SetStoreSlotInfo();
            }
            // NPC 타입이 창고이면
            else if (npcInfo.npcType == TypeData.NPCType.창고)
            {
                uiManager.isStorage = true;
            }

            uiManager.isFKey = false;
        }
    }

    // NPC 정보 설정
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

    // 상점리스트 슬롯 정보 설정
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
                tempSlotInfo.slotInfo.skillIndex = -1;
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
