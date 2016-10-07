public class TypeData
{   
    public enum PlayerType
    {
        기사 = 0,
        마법사,
        사제
    };

    // 몬스터 타입
    public enum MonsterType
    {
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

    public enum SlotType
    {
        없음 = -1,
        스킬 = 0,
        아이템
    };

    public enum ItemType
    {
        장비 = 0,
        소모품,
        재료,
        퀘스트템
    };


}