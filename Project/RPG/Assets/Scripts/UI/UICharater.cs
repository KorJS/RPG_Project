using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICharater : MonoBehaviour
{
    private PlayerInfoData      playerInfoData  = null; // 주인공 정보
    private PlayerSlotData      playerSlotData  = null; // 주인공 슬롯 정보

    private EquipmentHandler    equipHandler    = null; // 장비 핸들러

    public  GameObject          weaponObj       = null; // 주무기 
    private GameObject          subWeaponObj    = null; // 보조무기

    [System.Serializable]
    public class CharacterSettings
    {
        public UIPanel characterPanel;  // 케릭터창 판넬
        public UILabel levelNick;       // 레벨, 닉네임
        public UILabel str;             // 공격력
        public UILabel def;             // 방어력
        public UILabel hp;              // 체력
        public UILabel mp;              // 마력

        public AudioClip swordBGM;      // 검 사운드
        public AudioClip bookBGM;       // 책 사운드
        public AudioClip bodyBGM;       // 갑옷 사운드
        public AudioClip handBGM;       // 장갑 사운드
        public AudioClip legBGM;        // 신발 사운드
    }

    [SerializeField]
    public CharacterSettings characterSettings;

    [System.Serializable]
    public struct EquipmentStat
    {
        public int att; // 장비 공격력
        public int def; // 장비 방어력
        public int hp;  // 장비 체력
        public int mp;  // 장비 마력
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
            SoundManager.Instance.PlaySingleSystem(UIManager.Instance.uiSounds.lockBGM);

            return false;
        }

        // 착용 레벨이 안되면 리턴
        if (PlayerInfoData.Instance.infoData.level < tempEquipmentInfo.level)
        {
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
        
        // 알맞는 슬롯에 장비를 장착
        if ((int)tempEquipmentInfo.equipmentType == targetInfo.slotIndex)
        {
            if (targetInfo.isExist)
            {
                playerSlotData.SwapSlotData(currentInfo, targetInfo); // 장착한게 있으면 교체
            }
            else
            {
                playerSlotData.ChangSlotData(currentInfo, targetInfo); // 없으면 착용
            }
        }

        // 스텟 변경
        ChangPlayerStat();

        currentInfo.ReSetting();
        targetInfo.ReSetting();

        return true;
    }

    // 주인공 스텟 변경
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

    // 버프 스텟
    public void SetBuffStat()
    {
        characterSettings.str.text = playerInfoData.infoData.att.ToString() + "  + [00FF00FF]" + equipmentStat.att + playerInfoData.buffAtt;
        characterSettings.def.text = playerInfoData.infoData.def.ToString() + "  + [00FF00FF]" + equipmentStat.def + playerInfoData.buffDef;
        characterSettings.hp.text = playerInfoData.infoData.maxHp.ToString() + "  + [00FF00FF]" + equipmentStat.hp + playerInfoData.buffHp;
        characterSettings.mp.text = playerInfoData.infoData.maxMp.ToString() + "  + [00FF00FF]" + equipmentStat.mp + playerInfoData.buffMp;
    }

    public void CloseWindows()
    {
        characterSettings.characterPanel.alpha = 0f;
    }
}