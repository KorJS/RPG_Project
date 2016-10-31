using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICharater : MonoBehaviour
{
    private UIManager uiManager = null;
    private PlayerInfoData playerInfoData = null;
    private PlayerSlotData playerSlotData = null;

    private Transform slotHolder = null;

    [System.Serializable]
    public class CharacterSettings
    {
        public UILabel levelNick;
        public UILabel str;
        public UILabel def;
        public UILabel hp;
        public UILabel mp;
    }

    [SerializeField]
    public CharacterSettings characterSettings;

    [System.Serializable]
    public struct EquipmentStat
    {
        public int att;
        public int def;
        public int hp;
        public int mp;
    }

    [SerializeField]
    public EquipmentStat equipmentStat;

    void Awake()
    {
        playerInfoData = PlayerInfoData.Instance;
        playerSlotData = PlayerSlotData.Instance;

        slotHolder = transform.FindChild("E_Slots");
    }

    void Start()
    {
        uiManager = UIManager.Instance;
        ChangPlayerStat();
    }

    void OnEnable()
    {
        string levelNick = "레벨 " + playerInfoData.infoData.level + " " + playerInfoData.infoData.nick;
        characterSettings.levelNick.text = levelNick;
        characterSettings.str.text = playerInfoData.infoData.att.ToString() + "  + [00FF00FF]" + equipmentStat.att;
        characterSettings.def.text = playerInfoData.infoData.def.ToString() + "  + [00FF00FF]" + equipmentStat.def;
        characterSettings.hp.text = playerInfoData.infoData.hp.ToString() + "  + [00FF00FF]" + equipmentStat.hp;
        characterSettings.mp.text = playerInfoData.infoData.mp.ToString() + "  + [00FF00FF]" + equipmentStat.mp;
    }
    
    // 슬롯에 아이템 셋팅. - 이 함수가 호출될때 label에 스텟 +
    public void SetSlotInfo(UISlotInfo currentInfo, UISlotInfo targetInfo)
    {
        if (currentInfo.slotInfo.itemType != TypeData.ItemType.장비)
        {
            return;
        }
        Debug.Log("?");
        int index = currentInfo.slotInfo.itemIndex;
        ItemData.EquipmentInfo tempEquipmentInfo = ItemData.Instance.equipmentInfos[index];

        if ((int)tempEquipmentInfo.equipmentType == targetInfo.slotIndex)
        {
            if (targetInfo.isItemExist)
            {
                playerSlotData.SwapSlotData(currentInfo, targetInfo);
            }
            else
            {
                playerSlotData.ChangSlotData(currentInfo, targetInfo);
            }
        }
        // 스텟 변경
        ChangPlayerStat();

        currentInfo.ReSetting();
        targetInfo.ReSetting();
    }

    public void ChangPlayerStat()
    {
        equipmentStat.att = 0;
        equipmentStat.def = 0;
        equipmentStat.hp = 0;
        equipmentStat.mp = 0;

        foreach (KeyValuePair<int, PlayerSlotData.SlotInfoData> charSlotInfo in playerSlotData.characterInfos)
        {
            if (charSlotInfo.Value.itemIndex == -1)
            {
                return;
            }

            ItemData.EquipmentInfo tempEquipmentInfo = ItemData.Instance.equipmentInfos[charSlotInfo.Value.itemIndex];
            equipmentStat.att += tempEquipmentInfo.attack;
            equipmentStat.def += tempEquipmentInfo.defence;
            equipmentStat.hp += tempEquipmentInfo.hp;
            equipmentStat.mp += tempEquipmentInfo.mp;
            Debug.Log(tempEquipmentInfo.hp);
        }

        // 기본스텟 + 장비스텟
        playerInfoData.SetStat(equipmentStat.att, equipmentStat.def, equipmentStat.hp, equipmentStat.mp);

        characterSettings.str.text = playerInfoData.infoData.att.ToString() + "  + [00FF00FF]" + equipmentStat.att;
        characterSettings.def.text = playerInfoData.infoData.def.ToString() + "  + [00FF00FF]" + equipmentStat.def;
        characterSettings.hp.text = playerInfoData.infoData.hp.ToString() + "  + [00FF00FF]" + equipmentStat.hp;
        characterSettings.mp.text = playerInfoData.infoData.mp.ToString() + "  + [00FF00FF]" + equipmentStat.mp;
    }

    public void CloseWindows()
    {
        gameObject.SetActive(false);
    }
}