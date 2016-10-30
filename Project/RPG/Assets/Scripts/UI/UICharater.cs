using UnityEngine;
using System.Collections;

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

    void Awake()
    {
        playerInfoData = PlayerInfoData.Instance;
        playerSlotData = PlayerSlotData.Instance;

        slotHolder = transform.FindChild("E_Slots");
    }

    void Start()
    {
        uiManager = UIManager.Instance;
    }

    void OnEnable()
    {
        string levelNick = "레벨 " + playerInfoData.infoData.level + " " + playerInfoData.infoData.nick;
        characterSettings.levelNick.text = levelNick;
        characterSettings.str.text = playerInfoData.att.ToString();
        characterSettings.def.text = playerInfoData.def.ToString();
        characterSettings.hp.text = playerInfoData.hp.ToString();
        characterSettings.mp.text = playerInfoData.mp.ToString();
    }
    
    // 슬롯에 아이템 셋팅. - 이 함수가 호출될때 label에 스텟 +
    public void SetSlotInfo(UISlotInfo currentInfo, UISlotInfo targetInfo)
    {
        if (currentInfo.slotInfo.itemType != TypeData.ItemType.장비)
        {
            return;
        }

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

        currentInfo.ReSetting();
        targetInfo.ReSetting();
    }

    public void CloseWindows()
    {
        gameObject.SetActive(false);
    }
}