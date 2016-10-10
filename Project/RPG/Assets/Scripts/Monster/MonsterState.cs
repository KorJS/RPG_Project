using UnityEngine;
using System.Collections;

public class MonsterState : MonoBehaviour
{
    private MonsterMovement monsterMovement = null;

    public TypeData.MODE currentMode = TypeData.MODE.없음;
    public TypeData.MODE nextMode = TypeData.MODE.없음;

    public TypeData.State currentState = TypeData.State.없음;
    public TypeData.State nextState = TypeData.State.없음;

    void Awake()
    {
        monsterMovement = GetComponent<MonsterMovement>();

        currentState = TypeData.State.없음;
        nextState = TypeData.State.대기;

        currentMode = TypeData.MODE.없음;
        nextMode = TypeData.MODE.평화;
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

    private void CheckMode()
    {
        if (nextMode == TypeData.MODE.없음)
        {
            return;
        }

        currentMode = nextMode;
        nextMode = TypeData.MODE.없음;

        monsterMovement.SetAniMode(currentMode);
    }
}