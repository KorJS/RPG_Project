using UnityEngine;
using System; // DateTime
using System.Collections.Generic; // Dictionary
using JsonFx.Json; // JsonReader

public class Network_Login : MonoBehaviour
{
    private Network_Char networkChar = null;

    public GameObject loginObj = null;
    public GameObject joinObj = null;
    public GameObject charObj = null;
    public GameObject createObj = null;

    private string char_title = null;

    public GameObject idObj = null;
    public GameObject pwObj = null;
    public GameObject slotsObj = null;

    public UIInput id = null;
    public UIInput pw = null;
    public UILabel title = null;

    public UILabel login_message = null;
    public string login_contents = null;
    private string login_title = null;

    public UIInput join_repw = null;
    public UILabel join_message = null;
    public string join_contents = null;
    private string join_title = null;

    private class RecvLoginData
    {
        public int acc_index;
        public string message;
        public bool isSuccess;
        public int timestamp;
        public List<Network_Char.CharacterInfoData> characterInfos = new List<Network_Char.CharacterInfoData>();
    }

    private class RecvJoinData
    {
        public int acc_index;
        public string acc_id;
        public int timestamp;
        public string message;
        public bool isSuccess;
    }

    void Awake()
    {
        networkChar = GameObject.Find("CharManager").GetComponent<Network_Char>();
        login_contents = "login";
        login_title = "로그인";

        join_contents = "join";
        join_title = "신규가입";

        char_title = "캐릭터 선택";

        title.text = login_title;

        joinObj.SetActive(false);
        charObj.SetActive(false);
        createObj.SetActive(false);
    }

    public void RequestLogin()
    {
        if (id.value.Length <= 0 || pw.value.Length <= 0)
        {
            login_message.text = "아이디 / 비밀번호를 입력해주세요.";
            return;
        }

        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("contents", login_contents);
        sendData.Add("acc_id", id.value);
        sendData.Add("acc_pw", pw.value);

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplyLogin));
    }

    public void ReplyLogin(string json)
    {
        RecvLoginData data = JsonReader.Deserialize<RecvLoginData>(json);

        if (!data.isSuccess)
        {
            login_message.text = data.message;
            pw.value = null;
            return;
        }

        // 케릭선택창
        idObj.SetActive(false);
        pwObj.SetActive(false);
        loginObj.SetActive(false);
        charObj.SetActive(true);
        slotsObj.SetActive(true);
        title.text = char_title;
        Debug.Log("data : " + data.characterInfos.Count);
        networkChar.CheckSlotinfo(data.characterInfos);
        networkChar.acc_index = data.acc_index;

        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(data.timestamp);

        Debug.Log((origin.ToLocalTime()).ToString("yyyy년 MM월 dd일의 tt HH시 mm분 s초에 로그인 했습니다."));
    }

    public void RequestJoin()
    {
        if (id.value.Length <= 0 || pw.value.Length <= 0 || join_repw.value.Length <= 0)
        {
            join_message.text = "아이디 / 비밀번호를 입력해주세요.";
            return;
        }

        if (pw.value != join_repw.value)
        {
            join_message.text = "비밀번호가 일치하지 않습니다.";
            return;
        }

        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("contents", join_contents);
        sendData.Add("acc_id", id.value);
        sendData.Add("acc_pw", pw.value);
        sendData.Add("acc_repw", join_repw.value);

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplyJoin));
    }

    public void ReplyJoin(string json)
    {
        RecvJoinData data = JsonReader.Deserialize<RecvJoinData>(json);

        if (!data.isSuccess)
        {
            join_message.text = data.message;
            return;
        }

        joinObj.SetActive(false);
        loginObj.SetActive(true);
        id.value = data.acc_id;
        pw.value = null;
        title.text = login_title;
    }
    public void JoinBtn()
    {
        loginObj.SetActive(false);
        joinObj.SetActive(true);
        id.value = null;
        pw.value = null;
        join_repw.value = null;
        title.text = join_title;
    }

    public void LoginCancelBtn()
    {
        loginObj.SetActive(false);
        id.value = null;
        pw.value = null;
    }

    public void JoinCancelBtn()
    {
        joinObj.SetActive(false);
        loginObj.SetActive(true);
        id.value = null;
        pw.value = null;
        title.text = login_title;
    }
}