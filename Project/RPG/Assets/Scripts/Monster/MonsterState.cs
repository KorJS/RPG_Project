using UnityEngine;
using System.Collections;

public class MonsterState : MonoBehaviour
{
    private MonsterMovement monsterMovement = null;

    public TypeData.State currentState = TypeData.State.없음;
    public TypeData.State nextState = TypeData.State.없음;

    void Awake()
    {
        monsterMovement = GetComponent<MonsterMovement>();

        currentState = TypeData.State.없음;
        nextState = TypeData.State.대기;
    }

    void Update()
    {
        CheckState();
    }

    private void CheckState()
    {
        if (nextState == TypeData.State.없음)
        {
            return;
        }

        currentState = nextState;
        nextState = TypeData.State.없음;

        monsterMovement.SetAniState(currentState);
    }
}