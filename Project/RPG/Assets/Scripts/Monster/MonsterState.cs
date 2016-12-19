using UnityEngine;
using System.Collections;

public class MonsterState : MonoBehaviour
{
    private MonsterMovement monsterMovement = null; // 몬스터 동작

    // 몬스터 모드(평화, 전투)
    public TypeData.MODE            currentMode     = TypeData.MODE.없음;
    public TypeData.MODE            nextMode        = TypeData.MODE.없음;

    // 몬스터 상태
    public TypeData.MonsterState    currentState    = TypeData.MonsterState.없음;
    public TypeData.MonsterState    nextState       = TypeData.MonsterState.없음;

    void Awake()
    {
        monsterMovement = GetComponent<MonsterMovement>();

        currentState    = TypeData.MonsterState.없음;
        nextState       = TypeData.MonsterState.대기;

        currentMode     = TypeData.MODE.없음;
        nextMode        = TypeData.MODE.평화;
    }

    void OnEnable()
    {
        currentState    = TypeData.MonsterState.없음;
        nextState       = TypeData.MonsterState.대기;

        currentMode     = TypeData.MODE.없음;
        nextMode        = TypeData.MODE.평화;
    }

    void Update()
    {
        CheckState();
        CheckMode();
    }

    // 상태 체크
    private void CheckState()
    {
        if (nextState == TypeData.MonsterState.없음)
        {
            return;
        }

        currentState = nextState;
        nextState = TypeData.MonsterState.없음;

        monsterMovement.SetAniState(currentState);
    }

    // 모드 체크
    private void CheckMode()
    {
        if (nextMode == TypeData.MODE.없음)
        {
            return;
        }

        currentMode = nextMode;
        nextMode = TypeData.MODE.없음;
    }
}