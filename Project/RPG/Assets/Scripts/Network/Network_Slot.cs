using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json; // JsonReader

public class Network_Slot : MonoBehaviour
{
    public static Network_Slot network_slot = null;
    public static Network_Slot Instance
    {
        get
        {
            if (network_slot == null)
            {
                // MonoBehaviour에서는 new를 만들 수 없다.
                Debug.LogError("Singleton is Null");
            }
            return network_slot;
        }
    }

    private class RecvLoadSlotData
    {
        public string table_name;
        public int timestamp;
        public string message;
        public bool isSuccess;
        public int slot_type;
        public List<SlotInfoData> slotInfoDatas = new List<SlotInfoData>();
    }

    private class RecvSaveSlotData
    {
        public string table_name;
        public int timestamp;
        public string message;
        public bool isSuccess;
    }

    public class SlotInfoData
    {
        public int slot_index;
        public int item_type;
        public int skill_index;
        public int item_index;
        public int quantity;
    }

    private string load_contents = null;
    private string save_contents = null;
    private string delete_contents = null;

    // Awake에서 자신을 인스턴스로 등록 한다.
    void Awake()
    {
        network_slot = this;
        // 다른 씬으로 넘어가더라도 메모리에서 삭제하지 않는다.
        DontDestroyOnLoad(this);

        load_contents = "slot_load";
        save_contents = "slot_save";
        delete_contents = "slot_delete";
    }

    public void Ok()
    {
        RequestSaveSlot(TypeData.SlotType.인벤토리);
    }

    public void RequestLoadSlot(TypeData.SlotType slotType)
    {
        string table_name = null;

        CheckTable(slotType, ref table_name);

        Dictionary<string, object> sendData = new Dictionary<string, object>();

        sendData.Add("contents", load_contents);
        sendData.Add("acc_index", PlayerInfoData.Instance.infoData.accIndex);
        sendData.Add("char_index", PlayerInfoData.Instance.infoData.charIndex);
        sendData.Add("table_name", table_name);

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplyLoadSlot));
    }

    public void ReplyLoadSlot(string json)
    {
        RecvLoadSlotData data = JsonReader.Deserialize<RecvLoadSlotData>(json);

        if (!data.isSuccess)
        {
            UIManager.Instance.popupSettings.message.text = data.table_name + " 의 " + data.message;
            return;
        }

        if (data.slotInfoDatas.Count <= 0)
        {
            return;
        }

        Debug.Log(data.table_name + " 의 " + data.message);

        CheckSetSlotType(data.table_name, ref data.slotInfoDatas);
    }

    public void RequestSaveSlot(TypeData.SlotType slotType)
    {

        Dictionary<int, PlayerSlotData.SlotInfoData> slotInfoDatas = null;
        string table_name = null;

        CheckSaveSlotType(slotType, ref slotInfoDatas);
        CheckTable(slotType, ref table_name);

        Dictionary<string, object> sendData = new Dictionary<string, object>();

        sendData.Add("contents", save_contents);
        sendData.Add("acc_index", PlayerInfoData.Instance.infoData.accIndex);
        sendData.Add("char_index", PlayerInfoData.Instance.infoData.charIndex);
        sendData.Add("slotInfoDatas", slotInfoDatas);
        sendData.Add("table_name", table_name);

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplySaveSlot));
    }

    public void ReplySaveSlot(string json)
    {
        Debug.Log(json);
        RecvSaveSlotData data = JsonReader.Deserialize<RecvSaveSlotData>(json);

        if (!data.isSuccess)
        {
            UIManager.Instance.popupSettings.message.text = data.table_name + " 의 " + data.message;
            return;
        }
        Debug.Log(data.table_name + " 의 " + data.message);
    }

    public void RequestDeleteSlot(TypeData.SlotType slotType, int slotIndex)
    {
        Dictionary<string, object> sendData = new Dictionary<string, object>();

        string table_name = null;

        CheckTable(slotType, ref table_name);

        sendData.Add("contents", delete_contents);
        sendData.Add("acc_index", PlayerInfoData.Instance.infoData.accIndex);
        sendData.Add("char_index", PlayerInfoData.Instance.infoData.charIndex);
        sendData.Add("slot_index", slotIndex);

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplyDeleteSlot));
    }

    public void ReplyDeleteSlot(string json)
    {
        RecvSaveSlotData data = JsonReader.Deserialize<RecvSaveSlotData>(json);

        if (!data.isSuccess)
        {
            UIManager.Instance.popupSettings.message.text = data.table_name + " 의 " + data.message;
            return;
        }

        Debug.Log(data.table_name + " 의 " + data.message);
    }

    private void CheckTable(TypeData.SlotType slotType, ref string table_name)
    {
        switch (slotType)
        {
            case TypeData.SlotType.캐릭터:
                {
                    table_name = "charEquipment_table";
                }
                break;

            case TypeData.SlotType.인벤토리:
                {
                    table_name = "inventory_table";
                }
                break;

            case TypeData.SlotType.단축키:
                {
                    table_name = "shortcut_table";
                }
                break;

            case TypeData.SlotType.창고:
                {
                    table_name = "storage_table";
                }
                break;
        }
    }

    private void CheckSaveSlotType(TypeData.SlotType slotType, ref Dictionary<int, PlayerSlotData.SlotInfoData> slotInfoDatas)
    {
        switch (slotType)
        {
            case TypeData.SlotType.캐릭터:
                {
                    slotInfoDatas = PlayerSlotData.Instance.characterInfos;
                }
                break;

            case TypeData.SlotType.인벤토리:
                {
                    slotInfoDatas = PlayerSlotData.Instance.inventoryInfos;
                }
                break;

            case TypeData.SlotType.단축키:
                {
                    slotInfoDatas = PlayerSlotData.Instance.shortCutInfos;
                }
                break;

            case TypeData.SlotType.창고:
                {
                    slotInfoDatas = PlayerSlotData.Instance.storageInfos;
                }
                break;
        }
    }

    private void CheckSetSlotType(string table_name, ref List<SlotInfoData> slotInfoDatas)
    {
        switch (table_name)
        {
            case "charEquipment_table":
                {
                    SetSlotInfoData(ref slotInfoDatas, ref PlayerSlotData.Instance.characterInfos);
                }
                break;

            case "inventory_table":
                {
                    SetSlotInfoData(ref slotInfoDatas, ref PlayerSlotData.Instance.inventoryInfos);
                }
                break;

            case "shortcut_table":
                {
                    SetSlotInfoData(ref slotInfoDatas, ref PlayerSlotData.Instance.shortCutInfos);
                }
                break;

            case "storage_table":
                {
                    SetSlotInfoData(ref slotInfoDatas, ref PlayerSlotData.Instance.storageInfos);
                }
                break;
        }
    }

    private void SetSlotInfoData(ref List<SlotInfoData> slotInfoDatas, ref Dictionary<int, PlayerSlotData.SlotInfoData> targetSlotInfoDatas)
    {
        PlayerSlotData.SlotInfoData tempSlotInfoData;

        targetSlotInfoDatas.Clear();

        for (int i = 0; i < slotInfoDatas.Count; i++)
        {
            tempSlotInfoData.itemType = slotInfoDatas[i].item_type;
            tempSlotInfoData.skillIndex = slotInfoDatas[i].skill_index;
            tempSlotInfoData.itemIndex = slotInfoDatas[i].item_index;
            tempSlotInfoData.quantity = slotInfoDatas[i].quantity;

            targetSlotInfoDatas.Add(slotInfoDatas[i].slot_index, tempSlotInfoData);
        }
    }
}