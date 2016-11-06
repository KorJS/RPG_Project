using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoreItemListData
{
    private static StoreItemListData storeItemListData = null;
    public static StoreItemListData Instance
    {
        get
        {
            if (storeItemListData == null)
            {
                storeItemListData = new StoreItemListData();
            }

            return storeItemListData;
        }
    }

    // TODO : 지역으로 검색 후 타입별로 (나중에. 지역이 여러개라면)
    public List<int> itemIndexs;

    public Dictionary<int, List<int>> itemListInfos = null;

    public StoreItemListData()
    {
        itemIndexs = new List<int>();
        itemListInfos = new Dictionary<int, List<int>>();

        //itemIndexs.Add(0);
        //itemIndexs.Add(1);
        //itemListInfos.Add((int)TypeData.StoreType.소모품점, itemIndexs);
    }

    public void SetStore(TypeData.StoreType storeType, List<int> _itemIndexs)
    {
        itemIndexs = _itemIndexs;
        itemListInfos.Add((int)storeType, itemIndexs);
    }
}