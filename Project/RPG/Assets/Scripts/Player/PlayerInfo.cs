using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInfo : MonoBehaviour
{
    // TODO : 주인공 데이터 
    // 능력치, 스킬(배운거), 슬롯(스킬, 아이템), 인벤토리 / 창고(장비, 소모품, 재료)
    // 친구목록
    public struct StatInfo
    {

    };

    public struct SkillInfo
    {
        public int index;
        public int level;
    };

    public struct SlotInfo
    {
        public TypeData.SlotType slotType;
        public int index;
        public int quantity;
    };

    public struct InventoryInfo
    {

    };
}
