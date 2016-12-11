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
        public UIPanel characterPanel;
        public UILabel levelNick;
        public UILabel str;
        public UILabel def;
        public UILabel hp;
        public UILabel mp;

        public AudioClip swordBGM;
        public AudioClip bookBGM;
        public AudioClip bodyBGM;
        public AudioClip handBGM;
        public AudioClip legBGM;
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

        characterSettings.characterPanel = GetComponent<UIPanel>();
    }

    void Start()
    {
        equipHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<EquipmentHandler>();
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
    public bool SetSlotInfo(UISlotInfo currentInfo, UISlotInfo targetInfo)
    {
        if (currentInfo.slotInfo.itemType != TypeData.ItemType.장비)
        {
            SoundManager.Instance.PlaySingleSystem(UIManager.Instance.uiSounds.lockBGM); // 장비 외에꺼는 Lock 사운드 출력
            return false;
        }

        int index = currentInfo.slotInfo.itemIndex;
        ItemData.EquipmentInfo tempEquipmentInfo = ItemData.Instance.equipmentInfos[index]; // 장착하려는 장비

        TypeData.PlayerType playerType = (TypeData.PlayerType)PlayerInfoData.Instance.infoData.playerType;

        // 다른직업 아이템이거나 공용아이템이 아닐때 
        if (((int)playerType != tempEquipmentInfo.playerType) && (tempEquipmentInfo.playerType != (int)TypeData.PlayerType.없음))
        {
            Debug.Log("char " + 1);
            SoundManager.Instance.PlaySingleSystem(UIManager.Instance.uiSounds.lockBGM);
            return false;
        }

        // 착용 레벨이 안되면 리턴
        if (PlayerInfoData.Instance.infoData.level < tempEquipmentInfo.level)
        {
            Debug.Log("char " + 2);
            SoundManager.Instance.PlaySingleSystem(UIManager.Instance.uiSounds.lockBGM);

            return false;
        }

        UIManager.Instance.SetEquipmentSound(tempEquipmentInfo); // 장비 사운드 출력

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

            GameObject resource = Resources.Load("Equipment/" + tempEquipmentInfo.iconName + "_Subweapon") as GameObject;

            if (resource != null)
            {
                subWeaponObj = Instantiate(Resources.Load("Equipment/" + tempEquipmentInfo.iconName + "_Subweapon")) as GameObject;
            }

            // 무기 장착
            equipHandler.SetWeapon(weaponObj, true);
            equipHandler.SetSubWeapon(subWeaponObj, true);
        }
        
        if ((int)tempEquipmentInfo.equipmentType == targetInfo.slotIndex)
        {
            if (targetInfo.isExist)
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

        return true;
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
            equipmentStat.att += tempEquipmentInfo.att;
            equipmentStat.def += tempEquipmentInfo.def;
            equipmentStat.hp += tempEquipmentInfo.hp;
            equipmentStat.mp += tempEquipmentInfo.mp;
    
        }

        // 기본스텟 + 장비스텟
        playerInfoData.SetStat(equipmentStat.att, equipmentStat.def, equipmentStat.hp, equipmentStat.mp);

        characterSettings.str.text = playerInfoData.infoData.att.ToString() + "  + [00FF00FF]" + equipmentStat.att;
        characterSettings.def.text = playerInfoData.infoData.def.ToString() + "  + [00FF00FF]" + equipmentStat.def;
        characterSettings.hp.text = playerInfoData.infoData.maxHp.ToString() + "  + [00FF00FF]" + equipmentStat.hp;
        characterSettings.mp.text = playerInfoData.infoData.maxMp.ToString() + "  + [00FF00FF]" + equipmentStat.mp;
    }

    public void SetBuffStat()
    {
        characterSettings.str.text = playerInfoData.infoData.att.ToString() + "  + [00FF00FF]" + equipmentStat.att + playerInfoData.buffAtt;
        characterSettings.def.text = playerInfoData.infoData.def.ToString() + "  + [00FF00FF]" + equipmentStat.def + playerInfoData.buffDef;
        characterSettings.hp.text = playerInfoData.infoData.maxHp.ToString() + "  + [00FF00FF]" + equipmentStat.hp + playerInfoData.buffHp;
        characterSettings.mp.text = playerInfoData.infoData.maxMp.ToString() + "  + [00FF00FF]" + equipmentStat.mp + playerInfoData.buffMp;
    }

    private void SetSound(ItemData.EquipmentInfo equipmentInfo)
    {
        switch ((TypeData.EquipmentType)equipmentInfo.equipmentType)
        {
            case TypeData.EquipmentType.무기:
                {
                    if (equipmentInfo.playerType == (int)TypeData.PlayerType.기사)
                    {
                        SoundManager.Instance.PlaySingleUI(characterSettings.swordBGM);
                    }
                    else if (equipmentInfo.playerType == (int)TypeData.PlayerType.마법사)
                    {
                        SoundManager.Instance.PlaySingleUI(characterSettings.bookBGM);
                    }
                }
                break;

            case TypeData.EquipmentType.갑옷:
                { SoundManager.Instance.PlaySingleUI(characterSettings.bodyBGM); }
                break;

            case TypeData.EquipmentType.장갑:
                { SoundManager.Instance.PlaySingleUI(characterSettings.handBGM); }
                break;

            case TypeData.EquipmentType.신발:
                { SoundManager.Instance.PlaySingleUI(characterSettings.legBGM); }
                break;
        }
    }

    public void CloseWindows()
    {
        characterSettings.characterPanel.alpha = 0f;
    }
}