public class TypeData
{   
    public enum PlayerType
    {
        기사 = 0,
        마법사,
        사제
    };

    public enum PlayerEffect
    {
        Aggro = 0
    };

    // 몬스터 타입
    public enum MonsterType
    {
        // Normal, Elite, Boss
        일반 = 0,
        정예,
        보스
    };

    // 주인공
    public enum MODE
    {
        없음 = -1,
        평화 = 0,
        전투
    };

    // 주인공 / 몬스터 상태
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

    public enum SlotType
    {
        없음 = -1,
        스킬 = 0,
        아이템
    };

    public enum ItemType
    {
        없음 = -1,
        장비 = 0,
        소모품,
        재료
    };

    // 장비타입
    public enum EquipmentType
    {
        무기 = 0,
        갑옷,
        장갑,
        신발,
        반지,
        목걸이,
    };

    // 재료타입
    public enum MaterialType
    {
        없음 = -1,
        퀘스트템,
        강화,
    };
}