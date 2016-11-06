using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json; 

public class Network_Char : MonoBehaviour
{
    public GameObject loginObj = null;
    public GameObject charObj = null;
    public GameObject createObj = null;
    public GameObject idObj = null;
    public GameObject pwObj = null;
    public GameObject slotsObj = null;

    public UIInput nickname = null;
    public UILabel message = null;
    
    public Dictionary<int, UICharSlotInfo> charSlotInfos = null;
    public List<int> mark = null;
    public List<CharacterInfoData> slotInfos;

    private string create_contents = null;
    private string delete_contents = null;
    private string charInfo_contents = null;
    public int acc_index = 0;
    public int selectSlot = 0;
    public int selectPlayerType = 0;

    // 슬롯 정보 클래스
    public class CharacterInfoData
    {
        public int acc_index;
        public int char_index;
        public string nickname;
        public int player_type;
        public int level;
    }

    private class RecvCreateData
    {
        public int acc_index;
        public string message;
        public bool isSuccess;
        public int timestamp;
        public List<CharacterInfoData> characterInfos = new List<CharacterInfoData>();
    }

    private class RecvInGameData
    {
        public string message;
        public bool isSuccess;
        public int timestamp;
        public PlayerInfoData.InfoData playerInfoData;
    }

    void Awake()
    {
        charSlotInfos = new Dictionary<int, UICharSlotInfo>();
        mark = new List<int>();

        selectSlot = -1;
        selectPlayerType = (int)TypeData.PlayerType.기사;
        create_contents = "character_create";
        delete_contents = "character_delete";
        charInfo_contents = "characterInfo_load";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("selectSlot : " + selectSlot);
            SceneManager.LoadScene("PlayerTest");

        }
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

        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("contents", charInfo_contents);
        sendData.Add("acc_index", acc_index);
        sendData.Add("char_index", char_index);

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

        PlayerInfoData.Instance.infoData = data.playerInfoData;

        Network_Slot.Instance.RequestLoadSlot(TypeData.SlotType.인벤토리);
        Network_Slot.Instance.RequestLoadSlot(TypeData.SlotType.창고);
        Network_Slot.Instance.RequestLoadSlot(TypeData.SlotType.캐릭터);
        Network_Slot.Instance.RequestLoadSlot(TypeData.SlotType.단축키);
        Network_Store.Instance.RequestLoadStore(TypeData.AreaType.여명의정원);

        ItemData.Instance.LoadTalbe();

        StartCoroutine(SceneLoad());
    }

    IEnumerator SceneLoad()
    {
        yield return new WaitForSeconds(5f);

        SceneManager.LoadScene("PlayerTest");
    }

    public void RequestCreate()
    {
        if (nickname.value.Length <= 0)
        {
            message.text = "캐릭생성 불가(닉네임을 정해주세요.)";
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

        mark.Clear();
        nickname.value = null;
        WarriorBtn();
        createObj.SetActive(false);
        CheckSlotinfo(data.characterInfos);
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
        createObj.SetActive(true);
    }

    public void CreateCancelBtn()
    {
        createCharacter.nickName.value = null;
        createObj.SetActive(false);
    }

    public void WarriorBtn()
    {
        selectPlayerType = (int)TypeData.PlayerType.기사;
        createCharacter.warriorObj.SetActive(true);
        createCharacter.magicianObj.SetActive(false);
        createCharacter.priestObj.SetActive(false);
    }

    public void MagicianBtn()
    {
        selectPlayerType = (int)TypeData.PlayerType.마법사;
        createCharacter.warriorObj.SetActive(false);
        createCharacter.magicianObj.SetActive(true);
        createCharacter.priestObj.SetActive(false);
    }

    public void PriestBtn()
    {
        selectPlayerType = (int)TypeData.PlayerType.사제;
        createCharacter.warriorObj.SetActive(false);
        createCharacter.magicianObj.SetActive(false);
        createCharacter.priestObj.SetActive(true);
    }
}