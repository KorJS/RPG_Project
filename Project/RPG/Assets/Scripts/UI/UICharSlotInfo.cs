using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICharSlotInfo : MonoBehaviour
{
    private Network_Char networkChar = null;

    [System.Serializable]
    public class SlotInfo
    {
        public int              slotIndex;  // 슬롯 인덱스
        public List<GameObject> emblems;    // 엠블럼
        public UILabel          nickName;   // 닉네임
        public UILabel          level;      // 레벨
        public GameObject       empty;      // 빈 슬롯
    }

    public SlotInfo slotInfo;

    public bool isExist = false; // 슬롯에 케릭이 존재유무

    void Awake()
    {
        networkChar = GameObject.Find("Network_Char").GetComponent<Network_Char>();

        SetSlotIndex();

        networkChar.charSlotInfos.Add(slotInfo.slotIndex, this);
    }

    public void OnClickSlot()
    {
        networkChar.selectSlot = slotInfo.slotIndex;

        if (!isExist)
        {
            networkChar.message.text = "빈 슬롯 선택 하셨습니다.";

            return;
        }

        networkChar.message.text = slotInfo.nickName.text + " 슬롯 선택 하셨습니다.";
    }

    // 슬롯 인덱스 설정
    private void SetSlotIndex()
    {
        string[] strIndex = this.gameObject.name.Split(' ');
        slotInfo.slotIndex = int.Parse(strIndex[1]);
    }

    // 슬롯 정보 설정
    public void SetSlotInfo(string emblemType, string nickname, string level)
    {
        bool isEmpty = true;

        for (int i = 0; i < slotInfo.emblems.Count; i++)
        {
            if (string.Compare(emblemType, slotInfo.emblems[i].name) == 0)
            {
                slotInfo.emblems[i].SetActive(true);
                isEmpty = false;
            }
            else
            {
                slotInfo.emblems[i].SetActive(false);
            }

            slotInfo.nickName.text = nickname;
            slotInfo.level.text = level;
        }

        slotInfo.empty.SetActive(isEmpty);
    }

}
