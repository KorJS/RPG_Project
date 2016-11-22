using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    private static ItemManager itemManager = null;
    public static ItemManager Instance
    {
        get
        {
            if (itemManager == null)
            {
                Debug.Log("ItemManager Script Null");
            }

            return itemManager;
        }
    }

    private ItemData itemData = null;
    private PlayerInfoData playerInfoData = null;

    private List<int> buffStates = null;

    void Awake()
    {
        itemManager = this;
        DontDestroyOnLoad(this);

        itemData = ItemData.Instance;
        playerInfoData = PlayerInfoData.Instance;

        buffStates = new List<int>();
    }

    public void CheckItemType(TypeData.ItemType itemType, int itemIndex, bool isCoolTime)
    {
        Debug.Log("itemType : " + itemType + " itemIndex : " + itemIndex + " isCoolTime : " + isCoolTime);
        if (itemType == TypeData.ItemType.소모품)
        {
            if (!itemData.cusomableInfos.ContainsKey(itemIndex))
            {
                Debug.Log("아이템 정보가 없는 " + itemIndex + " 소모품");
                return;
            }
            Debug.Log("itemIndex : " + itemIndex);
            CusomableItem(itemIndex, isCoolTime);
        }
    }

    private void CusomableItem(int itemIndex, bool isCoolTime)
    {
        ItemData.CusomableInfo cusomableInfo = itemData.cusomableInfos[itemIndex];

        TypeData.CusomableType cusomableType = (TypeData.CusomableType)cusomableInfo.cusomableType;
        Debug.Log("cusomableType : " + cusomableType);
        switch (cusomableType)
        {
            case TypeData.CusomableType.회복:
                {
                    playerInfoData.SetCurrentHp(cusomableInfo.hp);
                    playerInfoData.SetCurrentMp(cusomableInfo.mp);
                }
                break;

            case TypeData.CusomableType.버프:
                {
                    int att = cusomableInfo.att;
                    int def = cusomableInfo.def;
                    int hp = cusomableInfo.hp;
                    int mp = cusomableInfo.mp;

                    if (isCoolTime)
                    {
                        Debug.Log("????????");
                        // 같은 아이템의 버프인경우 리턴
                        if (buffStates.Contains(itemIndex))
                        {
                            return;
                        }

                        playerInfoData.SetBuff(att, def, hp, mp);
                        buffStates.Add(itemIndex);

                        UIManager.Instance.windowSettings.characterPanel.GetComponent<UICharater>().SetBuffStat();
                    }
                    else
                    {
                        // 같은 아이템의 버프가 없는 경우 리턴
                        if (!buffStates.Contains(itemIndex))
                        {
                            return;
                        }

                        playerInfoData.SetBuff(-att, -def, -hp, -mp);
                        buffStates.Remove(itemIndex);

                        UIManager.Instance.windowSettings.characterPanel.GetComponent<UICharater>().SetBuffStat();
                    }
                }
                break;
        }
    }

    // 드랍 아이템 생성
    public void CreateDropItem(Transform monsterT, MonsterData.MonsterInfo monsterInfo, Dictionary<int, MonsterInfoData.DropItem> dropItems)
    {
        foreach (KeyValuePair<int, MonsterInfoData.DropItem> dropItem in dropItems)
        {
            string itemName = null;
            string iconName = null;
            TypeData.ItemType itemType = dropItem.Value.itemType;
            int itemIndex = dropItem.Value.itemIndex;
            int quantity = dropItem.Value.quantity;
            Debug.Log("type : " + itemType + " index : " + itemIndex + " quantity : " + quantity);

            GameObject resources = null;

            switch (itemType)
            {
                case TypeData.ItemType.장비:
                    {
                        itemName = itemData.equipmentInfos[itemIndex].name;
                        iconName = itemData.equipmentInfos[itemIndex].iconName;
                        resources = Resources.Load("Item/EquipmentBox") as GameObject;
                    }
                    break;

                case TypeData.ItemType.소모품:
                    {
                        itemName = itemData.cusomableInfos[itemIndex].name;
                        iconName = itemData.cusomableInfos[itemIndex].iconName;

                        if ((TypeData.CusomableType)itemData.cusomableInfos[itemIndex].cusomableType == TypeData.CusomableType.회복)
                        {
                            resources = Resources.Load("Item/Potion") as GameObject;
                        }
                        else
                        {
                            resources = Resources.Load("Item/CusomableBox") as GameObject;
                        }
                    }
                    break;

                case TypeData.ItemType.퀘스트템:
                    {
                        itemName = itemData.questItemInfos[itemIndex].name;
                        iconName = itemData.questItemInfos[itemIndex].iconName;
                        resources = Resources.Load("Item/Scroll") as GameObject;
                    }
                    break;
            }

            // 몬스터 위치 기준으로 랜덤위치에 스폰되게 설정
            Vector3 spawnPos = monsterT.position;
            float ran_x = Random.Range((int)(spawnPos.x - 2f), (int)(spawnPos.x + 2f));
            float y = spawnPos.y + 2f;
            float ran_z = Random.Range((int)(spawnPos.z - 2f), (int)(spawnPos.z + 2f));

            spawnPos = new Vector3(ran_x, y, ran_z);

            GameObject itemObj = Instantiate(resources, spawnPos, Quaternion.identity) as GameObject;

            // 아이템 정보 설정
            ItemControl.ItemSettings itemSettings = itemObj.GetComponent<ItemControl>().itemSettings;
            itemSettings.itemName = itemName;
            itemSettings.itemType = itemType;
            itemSettings.itemIndex = itemIndex;
            itemSettings.quantity = quantity;

            resources = null;
        }

    }
}
