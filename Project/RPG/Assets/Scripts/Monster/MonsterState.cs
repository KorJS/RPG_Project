using UnityEngine;
using System.Collections;

public class MonsterState : MonoBehaviour
{
    // 싱글톤
    private static MonsterState monsterState = null;
    public static MonsterState Instance
    {
        get
        {
            if (monsterState == null)
            {
                Debug.Log("MonsterState Instance Null");
            }
            return monsterState;
        }
    }

    private MonsterMovement monsterMovement = null;

    public TypeData.State currentState = TypeData.State.없음;
    public TypeData.State nextState = TypeData.State.없음;

    void Awake()
    {
        monsterState = this; // 싱글톤
        monsterMovement = GetComponent<MonsterMovement>();

        currentState = TypeData.State.없음;
        nextState = TypeData.State.없음;
    }
}