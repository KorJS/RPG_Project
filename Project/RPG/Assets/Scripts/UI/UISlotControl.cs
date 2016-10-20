using UnityEngine;
using System.Collections;

public class UISlotControl : MonoBehaviour
{
    public enum SlotType
    {
        없음 = -1,
        인벤토리 = 0,
        단축,
        상점,
    };

    [System.Serializable]
    public struct SlotInfoData
    {
        public int slotIndex;
        public TypeData.ItemType itemType;
        public int itemIndex;
    }

    [SerializeField]
    public SlotInfoData slotInfoData;

    [System.Serializable]
    public class SlotSettings
    {
        [Header("-Script-")]
        public UITexture uiIcon;
        public UISprite  uiCoolTime;
        public UILabel   uiQuantity;
    }

    [SerializeField]
    public SlotSettings slotSettings;

    void Awake()
    {

    }
}
