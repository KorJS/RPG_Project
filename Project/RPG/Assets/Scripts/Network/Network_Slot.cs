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
    // Awake에서 자신을 인스턴스로 등록 한다.
    void Awake()
    {
        network_slot = this;
        // 다른 씬으로 넘어가더라도 메모리에서 삭제하지 않는다.
        DontDestroyOnLoad(this);
    }

    public void Ok()
    {
        RequestSaveSlot(TypeData.SlotType.인벤토리);
    }
    public void RequestLoadSlot(TypeData.SlotType slotType)
    {
        string contents = null;

        CheckLoadSlotType(slotType, ref contents);

        if (contents == null)
        {
            Debug.Log("Contents null");
            return;
        }

        Dictionary<string, object> sendData = new Dictionary<string, object>();

        sendData.Add("contents", contents);
        sendData.Add("char_index", PlayerInfoData.Instance.infoData.charIndex);

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplyLoadSlot));
    }

    public void RequestSaveSlot(TypeData.SlotType slotType)
    {

        Dictionary<int, PlayerSlotData.SlotInfoData> slotInfoDatas = null;
        string contents = null;

        CheckSaveSlotType(slotType, ref slotInfoDatas, ref contents);

        if (contents == null)
        {
            Debug.Log("Contents null");
            return;
        }

        Dictionary<string, object> sendData = new Dictionary<string, object>();

        sendData.Add("contents", contents);
        sendData.Add("char_index", PlayerInfoData.Instance.infoData.charIndex);
        sendData.Add("slotInfoDatas", slotInfoDatas);

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, null));
    }

    public void ReplyLoadSlot(string json)
    {

    }

    private void CheckLoadSlotType(TypeData.SlotType slotType, ref string contents)
    {
        switch (slotType)
        {
            case TypeData.SlotType.캐릭터:
                {
                    contents = "charEquipmenet_load";
                }
                break;

            case TypeData.SlotType.인벤토리:
                {
                    contents = "inventory_load";
                }
                break;

            case TypeData.SlotType.단축키:
                {
                    contents = "shortcut_load";
                }
                break;

            case TypeData.SlotType.창고:
                {
                    contents = "storage_load";
                }
                break;
        }
    }

    private void CheckSaveSlotType(TypeData.SlotType slotType, ref Dictionary<int, PlayerSlotData.SlotInfoData> slotInfoDatas, ref string contents)
    {
        switch (slotType)
        {
            case TypeData.SlotType.캐릭터:
                {
                    slotInfoDatas = PlayerSlotData.Instance.characterInfos;
                    contents = "charEquipmenet_save";
                }
                break;

            case TypeData.SlotType.인벤토리:
                {
                    slotInfoDatas = PlayerSlotData.Instance.inventoryInfos;
                    contents = "inventory_save";
                }
                break;

            case TypeData.SlotType.단축키:
                {
                    slotInfoDatas = PlayerSlotData.Instance.shortCutInfos;
                    contents = "shortcut_save";
                }
                break;

            case TypeData.SlotType.창고:
                {
                    slotInfoDatas = PlayerSlotData.Instance.storageInfos;
                    contents = "storage_save";
                }
                break;
        }
    }
}