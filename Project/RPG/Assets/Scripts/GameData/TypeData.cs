public class TypeData
{   
    public enum PlayerType
    {
        기사 = 0,
        마법사,
        사제
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

    public enum MODE
    {
        없음 = -1,
        평화 = 0,
        전투
    };

    public enum State
    {
        없음 = -1,
        대기 = 0,
        이동,
        스킬,
        데미지,
        죽음,
    };
}