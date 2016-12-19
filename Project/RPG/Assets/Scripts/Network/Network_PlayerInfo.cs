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
        public string   message;    // 메시지
        public bool     isSuccess;  // 성공여부
        public int      timestamp;  // 작동시간
        public PlayerInfoData.InfoData playerInfoData;  // 주인공 정보
        public List<int> playerSkillInfos; // 주인공 배운 스킬 정보
    }

    private class RecvSavePlayerInfoData
    {
        public string   message;    // 메시지
        public bool     isSuccess;  // 성공여부
        public int      timestamp;  // 작동시간
    }

    private string load_charInfo_contents = null; // 불러오기 php 파일명
    private string save_charInfo_contents = null; // 저장 php 파일명

    void Awake()
    {
        if (network_playerInfo == null)
        {
            network_playerInfo = this;
        }
        else if (network_playerInfo != this)
        {
            Destroy(gameObject);
        }

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
        sendData.Add("player_skill_info", PlayerSkillData.Instance.skillInfos);

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
        PlayerSkillData.Instance.skillInfos = data.playerSkillInfos;
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
