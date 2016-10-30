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
        characterSettings.str.text = playerInfoData.infoData.att.ToString();
        characterSettings.def.text = playerInfoData.infoData.def.ToString();
        characterSettings.hp.text = playerInfoData.infoData.hp.ToString();
        characterSettings.mp.text = playerInfoData.infoData.mp.ToString();
    }

    public void CheckSlotInfo(UISlotInfo currentInfo, UISlotInfo targetInfo)
    {
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
    }

    public void CloseWindows()
    {
        gameObject.SetActive(false);
    }
}