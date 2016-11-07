using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;

public class Network_Store : MonoBehaviour
{
    public static Network_Store network_store = null;
    public static Network_Store Instance
    {
        get
        {
            if (network_store == null)
            {
                // MonoBehaviour에서는 new를 만들 수 없다.
                Debug.LogError("Singleton is Null");
            }
            return network_store;
        }
    }

    private List<SlotInfo> buy_list = null;
    private List<SlotInfo> sell_list = null;

    private int acc_index = 0;
    private int char_index = 0;
    private string store_load_contents = null;
    private string store_calculate_contents = null;

    void Awake()
    {
        network_store = this;
        DontDestroyOnLoad(this);

        buy_list = new List<SlotInfo>();
        sell_list = new List<SlotInfo>();

        store_load_contents = "store_load";
        store_calculate_contents = "store_calculate";
    }

    public struct SlotInfo
    {
        public int item_index;
        public int item_type;
        public int quantity;
    }

    public SlotInfo slotInfo;

    public class RecvLoadStore
    {
        public int area_type;
        public string message;
        public bool isSuccess;
        public int timestamp;
        public List<int> equipments = new List<int>();
        public List<int> cusomables = new List<int>();
    }

    public class RecvCalculateData
    {
        public string message;
        public bool isSuccess;
        public int timestamp;
    }

    public void RequestLoadStore(TypeData.AreaType area_type)
    {
        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("contents", store_load_contents);
        sendData.Add("area_type", (int)area_type);

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplyLoadStore));
    }

    public void ReplyLoadStore(string json)
    {
        RecvLoadStore data = JsonReader.Deserialize<RecvLoadStore>(json);

        if (!data.isSuccess)
        {
            Debug.Log(data.message);
            return;
        }

        StoreItemListData.Instance.SetStore(TypeData.StoreType.장비점, data.equipments);
        StoreItemListData.Instance.SetStore(TypeData.StoreType.소모품점, data.cusomables);

        Debug.Log(data.message);
    }

    public void RequestCalculate(int buyAmount, int sellAmount, int changeG)
    {
        CheckStoreSlot();

        acc_index = PlayerInfoData.Instance.infoData.accIndex;
        char_index = PlayerInfoData.Instance.infoData.charIndex;

        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("contents", store_calculate_contents);
        sendData.Add("acc_index", acc_index);
        sendData.Add("char_index", char_index);
        sendData.Add("area_type", (int)TypeData.AreaType.여명의정원);
        sendData.Add("buy_list", buy_list);
        sendData.Add("sell_list", sell_list);
        sendData.Add("buy_amount", buyAmount);
        sendData.Add("sell_amount", sellAmount);
        sendData.Add("changeG", changeG);

        Debug.Log("buyAmount : " + buyAmount + " sellAmount : " + sellAmount + " changeG : " + changeG);

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplyCalculate));
    }

    public void ReplyCalculate(string json)
    {
        RecvCalculateData data = JsonReader.Deserialize<RecvCalculateData>(json);

        if (!data.isSuccess)
        {
            UIManager.Instance.SetMessage(data.message);
            Debug.Log("실패 : " + data.message);
            return;
        }

        UIManager.Instance.windowSettings.storeObj.GetComponent<UIStore>().Calculate();

        Debug.Log("성공 : " + data.message);
    }

    private void CheckStoreSlot()
    {
        buy_list.Clear();
        sell_list.Clear();

        if (UIManager.Instance.buySlots.Count != 0)
        {
            foreach (KeyValuePair<int, UISlotInfo> buySlot in UIManager.Instance.buySlots)
            {
                if (buySlot.Value.slotInfo.itemIndex == -1)
                {
                    continue;
                }

                slotInfo.item_index = buySlot.Value.slotInfo.itemIndex;
                slotInfo.item_type = (int)buySlot.Value.slotInfo.itemType;
                slotInfo.quantity = buySlot.Value.slotInfo.quantity;
                buy_list.Add(slotInfo);
            }
        }

        if (UIManager.Instance.sellSlots.Count != 0)
        {
            foreach (KeyValuePair<int, UISlotInfo> sellSlot in UIManager.Instance.sellSlots)
            {
                if (sellSlot.Value.slotInfo.itemIndex == -1)
                {
                    continue;
                }

                slotInfo.item_index = sellSlot.Value.slotInfo.itemIndex;
                slotInfo.item_type = (int)sellSlot.Value.slotInfo.itemType;
                slotInfo.quantity = sellSlot.Value.slotInfo.quantity;
                sell_list.Add(slotInfo);
            }
        }
    }
}