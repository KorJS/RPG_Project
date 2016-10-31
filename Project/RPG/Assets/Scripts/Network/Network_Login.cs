using UnityEngine;
using System; // DateTime
using System.Collections.Generic; // Dictionary
using JsonFx.Json; // JsonReader

public class Network_Login : MonoBehaviour
{
    public UIInput id;
    public UIInput pw;
    public UILabel message;

    private class RecvLoginData
    {
        public int accountIndex;
        public string accountID;
        public int timestamp;
        public string message;
    }

    public void RequestLogin()
    {
        //if(id.value.Length < 4 || pw.value.Length < 4)
        //{

        //}
        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("contents", "login");
        sendData.Add("id", id.value);
        sendData.Add("pw", pw.value);
        sendData.Add("inven", PlayerSlotData.Instance.inventoryInfos);

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplyLogin));
    }

    public void ReplyLogin(string json)
    {
        RecvLoginData data = JsonReader.Deserialize<RecvLoginData>(json);

        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(data.timestamp);

        message.text = data.message;

        Debug.Log((origin.ToLocalTime()).ToString("yyyy년 MM월 dd일의 tt HH시 mm분 s초에 로그인 했습니다."));
    }
}