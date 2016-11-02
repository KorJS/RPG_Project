public class TypeData
{
    public enum GameDataState
    {
        불러오기 = 0,
        저장
    }

    // Warrior, Magician, Priest
    public enum PlayerType
    {
        없음 = -1,
        기사 = 0,
        마법사,
        사제
    };

    public enum PlayerEffect
    {
        Aggro = 0
    };

    // Normal, Elite, Boss
    public enum MonsterType
    {
        일반 = 0,
        정예,
        보스
    };

    public enum NPCType
    {
        상인 = 0,
        창고,
        퀘스트
    };

    // Peace, Combat
    public enum MODE
    {
        없음 = -1,
        평화 = 0,
        전투
    };

    // idle, move, skill, damage, stun, down, death
    public enum State
    {
        없음 = -1,
        대기 = 0,
        이동,
        스킬,
        데미지,
        스턴,
        다운,
        죽음
    };

    // idle, talk
    public enum NPCState
    {
        없음 = -1,
        대기 = 0,
        대화,
    };

    // idle, move, rotate, skill, damage, stun, down, death
    public enum MonsterState
    {
        없음 = -1,
        대기 = 0,
        이동,
        회전,
        스킬,
        데미지,
        스턴,
        다운,
        죽음
    };

    // Equipment, Cusomable, Material
    public enum ItemType
    {
        없음 = -1,
        장비 = 0,
        소모품,
        퀘스트템
    };

    // Weapon, armor, glove, shoes, ring, necklace
    public enum EquipmentType
    {
        무기 = 1,
        갑옷,
        장갑,
        신발,
    };

    // Inventory, ShortCut, Store, Storage, Skill
    public enum SlotType
    {
        없음 = -1,
        캐릭터 = 0,
        인벤토리,
        단축키,
        창고,
        스킬리스트,
        상점리스트,
        구매,
        판매,
    };

    // Skill, Item, Character
    public enum SlotInfoType
    {
        없음 = -1,
        스킬 = 0,
        아이템
    };

    public enum StoreType
    {
        없음 = -1,
        장비점 = 0,
        소모품점
    };
}