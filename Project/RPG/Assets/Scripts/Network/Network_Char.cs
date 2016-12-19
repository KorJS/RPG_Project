using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json; 

public class Network_Char : MonoBehaviour
{
    // 슬롯 정보 클래스
    public class CharacterInfoData
    {
        public int      acc_index;      // 계정 인덱스
        public int      char_index;     // 케릭터 인덱스
        public string   nickname;       // 닉네임
        public int      player_type;    // 주인공 타입(직업)
        public int      level;          // 레벨
    }

    private class RecvCreateData
    {
        public int      acc_index;      // 계정 인덱스
        public string   message;        // 메시지
        public bool     isSuccess;      // 성공여부
        public int      timestamp;      // 작동시간
        public List<CharacterInfoData> characterInfos = new List<CharacterInfoData>();  // 케릭터 정보
    }

    private class RecvInGameData
    {
        public string                    message;           // 메시지
        public bool                      isSuccess;         // 성공여부
        public int                       timestamp;         // 작동시간
        public PlayerInfoData.InfoData   playerInfoData;    // 주인공 정보
        public List<int>                 playerSkillInfos;  // 주인공 배운 스킬
        public List<SkillData.SkillInfo> skillInfos;        // 해당 직업 스킬정보
    }

    public GameObject       loginObj    = null; // 로그인
    public GameObject       charObj     = null; // 케릭터
    public GameObject       createObj   = null; // 케릭터 생성
    public GameObject       idObj       = null; // 아이디
    public GameObject       pwObj       = null; // 비밀번호
    public GameObject       slotsObj    = null; // 슬롯

    public UIPanel          rightPanel  = null; // 오른쪽 판넬
    public UIPanel          lodingPanel = null; // 로딩판넬
    public UIProgressBar    lodingBar   = null; // 로딩 프로그래스바

    private const float     lodingTime  = 5f;   // 로딩 시간
    private float           lodingTimer = 0f;   // 로딩 타이머

    public UIInput          nickname    = null; // 닉네임
    public UILabel          message     = null; // 메시지 (서버로 부터 받은 메시지 출력)
    
    public Dictionary<int, UICharSlotInfo>  charSlotInfos   = null; // 케릭터 슬롯 정보
    public List<int>                        mark            = null; // 케릭터 슬롯 마크
    public List<CharacterInfoData>          slotInfos       = null; // 슬롯에 케릭터 정보

    private string          create_contents     = null; // 케릭터 생성 php 파일명
    private string          delete_contents     = null; // 케릭터 삭제 php 파일명
    private string          charInfo_contents   = null; // 케릭터 정보로드 php 파일명
    public int              acc_index           = 0;    // 계정 인덱스
    public int              selectSlot          = 0;    // 선택한 슬롯
    public int              selectPlayerType    = 0;    // 선택한 직업

    public AudioClip        introBGM            = null; // 인트로 사운드
    public AudioClip        warriorBGM          = null; // 기사 사운드
    public AudioClip        magicianBGM         = null; // 마법사 사운드
    public AudioClip        priestBGM           = null; // 사제 사운드

    void Awake()
    {
        charSlotInfos       = new Dictionary<int, UICharSlotInfo>();
        mark                = new List<int>();

        selectSlot          = -1;
        selectPlayerType    = (int)TypeData.PlayerType.기사;
        create_contents     = "character_create";
        delete_contents     = "character_delete";
        charInfo_contents   = "characterInfo_load";

        lodingPanel.alpha   = 0f;
        lodingBar.value     = 0f;
    }

    // 슬롯 표시할 오브젝트
    [System.Serializable]
    public class CreateCharacter
    {
        public GameObject warriorObj;
        public GameObject magicianObj;
        public GameObject priestObj;
        public UIInput nickName;
    }

    public CreateCharacter createCharacter;

    // 슬롯 정보 확인
    public void CheckSlotinfo(List<CharacterInfoData> characterInfos)
    {
        Debug.Log(characterInfos.Count);

        if (slotInfos != null)
        {
            slotInfos.Clear();
        }

        slotInfos = characterInfos;

        for (int i = 0; i < 3; i++)
        {
            if (slotInfos.Count - 1 < i)
            {
                mark.Add(i);
                SetSlotInfo(i, false);
                continue;
            }
            SetSlotInfo(i, true);
        }
    }

    // 확인한 슬롯 정보 슬롯에 설정
    private void SetSlotInfo(int slotIndex, bool isExist)
    {
        UICharSlotInfo charSlotInfo = charSlotInfos[slotIndex];

        // 슬롯에 정보가 하나도 없을경우
        if (!isExist)
        {
            charSlotInfo.SetSlotInfo(null, null, null);
            charSlotInfo.isExist = false;
            return;
        }

        switch ((TypeData.PlayerType)slotInfos[slotIndex].player_type)
        {
            case TypeData.PlayerType.기사:
                {
                    charSlotInfo.SetSlotInfo("Warrior", slotInfos[slotIndex].nickname, "Lv " + slotInfos[slotIndex].level.ToString());
                    charSlotInfo.isExist = true;
                }
                break;

            case TypeData.PlayerType.마법사:
                {
                    charSlotInfo.SetSlotInfo("Magician", slotInfos[slotIndex].nickname, "Lv " + slotInfos[slotIndex].level.ToString());
                    charSlotInfo.isExist = true;
                }
                break;

            case TypeData.PlayerType.사제:
                {
                    charSlotInfo.SetSlotInfo("Priest", slotInfos[slotIndex].nickname, "Lv " + slotInfos[slotIndex].level.ToString());
                    charSlotInfo.isExist = true;
                }
                break;

            default:
                {
                    charSlotInfo.SetSlotInfo(null, null, null);
                    charSlotInfo.isExist = false;
                }
                break;
        }
    }

    public void RequestInGame()
    {
        if (selectSlot < 0)
        {
            message.text = "선택한 슬롯이 없습니다.";
            return;
        }

        if (mark.Contains(selectSlot))
        {
            message.text = "캐릭터를 선택 하세요.";
            return;
        }

        // Login에 acc_index, char_index 로 정보 가져옴
        // 케릭정보, 케릭창 슬롯, 단축슬롯, 인벤토리 슬롯, 창고슬롯
        int char_index = slotInfos[selectSlot].char_index;
        int player_type = slotInfos[selectSlot].player_type;

        Debug.Log("player_type : " + player_type);

        Dictionary <string, object> sendData = new Dictionary<string, object>();
        sendData.Add("contents", charInfo_contents);
        sendData.Add("acc_index", acc_index);
        sendData.Add("char_index", char_index);
        sendData.Add("player_type", player_type);

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplyInGame));
    }

    public void ReplyInGame(string json)
    {
        RecvInGameData data = JsonReader.Deserialize<RecvInGameData>(json);

        if (!data.isSuccess)
        {
            message.text = data.message;
            return;
        }

        if (createObj.activeSelf)
        {
            createObj.SetActive(false);
        }

        if (rightPanel.alpha == 1f)
        {
            rightPanel.alpha = 0f;
        }

        SoundManager.Instance.PlayBackMusic(introBGM);

        PlayerInfoData.Instance.infoData = data.playerInfoData;
        PlayerSkillData.Instance.skillInfos = data.playerSkillInfos;
        SkillData.Instance.SetSkillInfo(data.skillInfos);

        Network_Slot.Instance.RequestLoadSlot(TypeData.SlotType.인벤토리);
        Network_Slot.Instance.RequestLoadSlot(TypeData.SlotType.창고);
        Network_Slot.Instance.RequestLoadSlot(TypeData.SlotType.캐릭터);
        Network_Slot.Instance.RequestLoadSlot(TypeData.SlotType.단축키);
        // 플레이어 마지막으로 종료한 지역으로 로드
        Network_Store.Instance.RequestLoadStore(TypeData.AreaType.여명의정원);
        Network_MonsterInfo.Instance.RequestLoadMonster(TypeData.AreaType.여명의정원);

        ItemData.Instance.LoadTalbe();

        StartCoroutine(SceneLoad());
    }

    // 로딩, 씬로드
    IEnumerator SceneLoad()
    {
        yield return new WaitForSeconds(1f);

        lodingPanel.alpha = 1f;

        while (true)
        {
            lodingBar.value = lodingTimer / lodingTime;

            lodingTimer += Time.deltaTime;

            if (lodingTimer >= lodingTime)
            {
                lodingTimer = 0f;
                SceneManager.LoadScene("PlayerTest");

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.isFade = true;
                }
            }

            yield return null;
        }
    }

    public void RequestCreate()
    {
        if (nickname.value.Length <= 0)
        {
            message.text = "캐릭생성 불가(닉네임을 정해주세요.)";
            return;
        }

        if (selectPlayerType == (int)TypeData.PlayerType.사제)
        {
            message.text = "선택하신 캐릭터는 개발중입니다.";
            return;
        }

        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("contents", create_contents);
        sendData.Add("acc_index", acc_index);
        sendData.Add("nickname", nickname.value);
        sendData.Add("player_type", selectPlayerType);

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplyCreate));
    }

    public void ReplyCreate(string json)
    {
        RecvCreateData data = JsonReader.Deserialize<RecvCreateData>(json);

        if (!data.isSuccess)
        {
            message.text = data.message;
            return;
        }

        SoundManager.Instance.PlayBackMusic(introBGM);
        mark.Clear();
        nickname.value = null;
        WarriorBtn();
        createObj.SetActive(false);
        CheckSlotinfo(data.characterInfos);

        message.text = data.message;
    }

    public void RequestDelete()
    {
        if (selectSlot == -1)
        {
            message.text = "삭제할 캐릭을 선택 해주세요.";
            return;
        }

        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("contents", delete_contents);
        sendData.Add("acc_index", acc_index);
        sendData.Add("char_index", slotInfos[selectSlot].char_index);

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplyCreate));
    }

    public void CharCancelBtn()
    {
        mark.Clear();
        loginObj.SetActive(true);
        idObj.SetActive(true);
        pwObj.SetActive(true);
        charObj.SetActive(false);
        createObj.SetActive(false);
        slotsObj.SetActive(false);
    }

    public void CreateOnBtn()
    {
        if (mark.Count <= 0)
        {
            message.text = "캐릭생성 불가(슬롯이 부족합니다.)";
            return;
        }

        SoundManager.Instance.PlayBackMusic(warriorBGM);
        createObj.SetActive(true);
    }

    public void CreateCancelBtn()
    {
        SoundManager.Instance.PlayBackMusic(introBGM);
        createCharacter.nickName.value = null;
        createObj.SetActive(false);
    }

    public void WarriorBtn()
    {
        SoundManager.Instance.PlayBackMusic(warriorBGM);
        message.text = "기사 캐릭터를 선택 하셨습니다.";
        selectPlayerType = (int)TypeData.PlayerType.기사;
        createCharacter.warriorObj.SetActive(true);
        createCharacter.magicianObj.SetActive(false);
        createCharacter.priestObj.SetActive(false);
    }

    public void MagicianBtn()
    {
        SoundManager.Instance.PlayBackMusic(magicianBGM);
        message.text = "마법사 캐릭터를 선택 하셨습니다.";
        selectPlayerType = (int)TypeData.PlayerType.마법사;
        createCharacter.warriorObj.SetActive(false);
        createCharacter.magicianObj.SetActive(true);
        createCharacter.priestObj.SetActive(false);
    }

    public void PriestBtn()
    {
        SoundManager.Instance.PlayBackMusic(priestBGM);
        message.text = "사제 캐릭터는 개발중입니다.";
        selectPlayerType = (int)TypeData.PlayerType.사제;
        createCharacter.warriorObj.SetActive(false);
        createCharacter.magicianObj.SetActive(false);
        createCharacter.priestObj.SetActive(true);
    }
}