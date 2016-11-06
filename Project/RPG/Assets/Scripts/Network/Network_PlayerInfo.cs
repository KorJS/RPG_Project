using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;

public class Network_PlayerInfo : MonoBehaviour
{
    public static Network_PlayerInfo network_playerInfo = null;
    public static Network_PlayerInfo Instance
    {
        get
        {
            if (network_playerInfo == null)
            {
                // MonoBehaviour에서는 new를 만들 수 없다.
                Debug.LogError("Singleton is Null");
            }
            return network_playerInfo;
        }
    }

    private class RecvLoadPlayerInfoData
    {
        public string message;
        public bool isSuccess;
        public int timestamp;
        public PlayerInfoData.InfoData playerInfoData;
    }

    private class RecvSavePlayerInfoData
    {
        public string message;
        public bool isSuccess;
        public int timestamp;
    }

    private string load_charInfo_contents = null;
    private string save_charInfo_contents = null;

    void Awake()
    {
        network_playerInfo = this;
        load_charInfo_contents = "characterInfo_load";
        save_charInfo_contents = "characterInfo_save";
    }

    public void RequestLoadPlayerInfo()
    {
        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("contents", load_charInfo_contents);
        sendData.Add("acc_index", PlayerInfoData.Instance.infoData.accIndex);
        sendData.Add("char_index", PlayerInfoData.Instance.infoData.charIndex);

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplyLoadPlayerInfo));
    }

    public void RequestSavePlayerInfo()
    {
        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("contents", save_charInfo_contents);
        sendData.Add("player_info", PlayerInfoData.Instance.infoData);

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplySavePlayerInfo));
    }

    public void ReplyLoadPlayerInfo(string json)
    {
        RecvLoadPlayerInfoData data = JsonReader.Deserialize<RecvLoadPlayerInfoData>(json);

        if (!data.isSuccess)
        {
            Debug.Log(data.message);
            return;
        }

        Debug.Log(data.message);
        PlayerInfoData.Instance.infoData = data.playerInfoData;
    }

    public void ReplySavePlayerInfo(string json)
    {
        RecvSavePlayerInfoData data = JsonReader.Deserialize<RecvSavePlayerInfoData>(json);

        if (!data.isSuccess)
        {
            Debug.Log(data.message);
            return;
        }

        Debug.Log(data.message);
    }
}
