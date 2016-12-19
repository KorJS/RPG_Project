using UnityEngine;
using System; // DateTime
using System.Collections.Generic; // Dictionary
using JsonFx.Json; // JsonReader

public class Network_Login : MonoBehaviour
{
    private class RecvLoginData
    {
        public int      acc_index;  // 계정 인덱스
        public string   message;    // 메시지
        public bool     isSuccess;  // 성공여부
        public int      timestamp;  // 작동시간
        public List<Network_Char.CharacterInfoData> characterInfos = new List<Network_Char.CharacterInfoData>();    // 케릭터 정보
    }

    private class RecvJoinData
    {
        public int      acc_index;  // 계정 인덱스
        public string   acc_id;     // 아이디
        public string   message;    // 메시지
        public bool     isSuccess;  // 성공여부
        public int      timestamp;  // 작동 시간
    }

    private Network_Char    networkChar     = null; // 케릭터관련 네트워크 스크립트

    public GameObject       loginObj        = null; // 로그인
    public GameObject       joinObj         = null; // 신규가입
    public GameObject       charObj         = null; // 캐릭터

    private string          char_title      = null; // 타이틀

    public GameObject       idObj           = null; // 아이디
    public GameObject       pwObj           = null; // 비밀번호
    public GameObject       slotsObj        = null; // 케릭터 슬롯

    public UIInput          id              = null; // 아이디 입력
    public UIInput          pw              = null; // 비밀번호 입력
    public UILabel          title           = null; // 타이틀

    public UILabel          login_message   = null; // 로그인 메시지
    private string          login_contents  = null; // 로그인 php 파일명
    private string          login_title     = null; // 로그인 타이틀

    public UIInput          join_repw       = null; // 가입시 다시입력 비밀번호
    public UILabel          join_message    = null; // 가입 메시지
    private string          join_contents   = null; // 가입 php 파일명
    private string          join_title      = null; // 가입 타이틀

    public AudioClip        introBGM        = null; // 인트로 사운드

    void Awake()
    {
        networkChar     = GameObject.Find("Network_Char").GetComponent<Network_Char>();
        login_contents  = "login";
        login_title     = "로그인";

        join_contents   = "join";
        join_title      = "신규가입";

        char_title      = "캐릭터 선택";

        title.text      = login_title;

        joinObj.SetActive(false);
        charObj.SetActive(false);
    }

    void Start()
    {
        SoundManager.Instance.PlayBackMusic(introBGM);
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
        login_message.text = "로그인 정보를 입력하세요.";
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

    public void GameExit()
    {
        Application.Quit();
    }
}