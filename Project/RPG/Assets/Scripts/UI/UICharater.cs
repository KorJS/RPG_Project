using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICharater : MonoBehaviour
{
    private PlayerInfoData playerInfoData = null;
    private PlayerSlotData playerSlotData = null;

    private EquipmentHandler equipHandler = null;

    public GameObject weaponObj = null;
    private GameObject subWeaponObj = null;

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

        equipHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<EquipmentHandler>();
    }

    void Start()
    {
        ChangPlayerStat();
    }

    void OnEnable()
    {
        string levelNick = "레벨 " + playerInfoData.infoData.level + " " + playerInfoData.infoData.nick;
        characterSettings.levelNick.text = levelNick;
        characterSettings.str.text = playerInfoData.infoData.att.ToString() + "  + [00FF00FF]" + equipmentStat.att;
        characterSettings.def.text = playerInfoData.infoData.def.ToString() + "  + [00FF00FF]" + equipmentStat.def;
        characterSettings.hp.text = playerInfoData.infoData.maxHp.ToString() + "  + [00FF00FF]" + equipmentStat.hp;
        characterSettings.mp.text = playerInfoData.infoData.maxMp.ToString() + "  + [00FF00FF]" + equipmentStat.mp;
    }
    
    // 슬롯에 아이템 셋팅. - 이 함수가 호출될때 label에 스텟 +
    public void SetSlotInfo(UISlotInfo currentInfo, UISlotInfo targetInfo)
    {
        if (currentInfo.slotInfo.itemType != TypeData.ItemType.장비)
        {
            return;
        }
        
        int index = currentInfo.slotInfo.itemIndex;
        ItemData.EquipmentInfo tempEquipmentInfo = ItemData.Instance.equipmentInfos[index]; // 장착하려는 장비

        // 무기
        if (targetInfo.slotIndex == 1)
        {
            Debug.Log(targetInfo.slotIndex);
            if (weaponObj != null || subWeaponObj != null)
            {
                equipHandler.SetWeapon(null, false);
                equipHandler.SetSubWeapon(null, false);
            }
            Debug.Log("Equipment/" + tempEquipmentInfo.iconName + "_Weapon");
            weaponObj = Instantiate(Resources.Load("Equipment/" + tempEquipmentInfo.iconName + "_Weapon")) as GameObject;
            subWeaponObj = Instantiate(Resources.Load("Equipment/" + tempEquipmentInfo.iconName + "_Subweapon")) as GameObject;

            // 무기 장착
            equipHandler.SetWeapon(weaponObj, true);
            equipHandler.SetSubWeapon(subWeaponObj, true);
        }
        
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
        characterSettings.hp.text = playerInfoData.infoData.maxHp.ToString() + "  + [00FF00FF]" + equipmentStat.hp;
        characterSettings.mp.text = playerInfoData.infoData.maxMp.ToString() + "  + [00FF00FF]" + equipmentStat.mp;
    }

    public void CloseWindows()
    {
        gameObject.SetActive(false);
    }
}