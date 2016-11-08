using UnityEngine;
using System.Collections;

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

    void Awake()
    {
        itemManager = this;
        DontDestroyOnLoad(this);

        itemData = ItemData.Instance;
        playerInfoData = PlayerInfoData.Instance;
    }

    public void CheckItemType(TypeData.ItemType itemType, int itemIndex, bool isCoolTime)
    {
        if (itemType == TypeData.ItemType.소모품)
        {
            if (!itemData.cusomableInfos.ContainsKey(itemIndex))
            {
                Debug.Log("아이템 정보가 없는 " + itemIndex + " 소모품");
                return;
            }

            CusomableItem(itemIndex, isCoolTime);
        }
    }

    private void CusomableItem(int itemIndex, bool isCoolTime)
    {
        ItemData.CusomableInfo cusomableInfo = itemData.cusomableInfos[itemIndex];

        TypeData.CusomableType cusomableType = (TypeData.CusomableType)cusomableInfo.cusomableType;

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
                        playerInfoData.SetBuff(att, def, hp, mp);
                    }
                    else
                    {
                        playerInfoData.SetBuff(-att, -def, -hp, -mp);
                    }
                }
                break;
        }
    }

    public void CreateDropItem(Transform spawnT, TypeData.ItemType itemType, int itemIndex)
    {

    }
}
