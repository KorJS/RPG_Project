using UnityEngine;
using System.Collections;

public class PlayerState : MonoBehaviour
{
    private PlayerMovement playerMovement = null;   // 주인공 동작 관련
    private EquipmentHandler equipHandler = null;   // 장비핸들러

    public TypeData.MODE currentMode = TypeData.MODE.없음; // 현재 모드 상태
    public TypeData.MODE nextMode = TypeData.MODE.없음; // 변화한 모드 상태

    public TypeData.State currentState = TypeData.State.없음;
    public TypeData.State nextState = TypeData.State.없음;

    private float noHitCombatTime = 5f; // 전투모드에서 전투가 없을때 평화모드로 전환될 시간
    public float combatTimer = 0f;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        equipHandler = GetComponent<EquipmentHandler>();

        currentMode = TypeData.MODE.평화;
        nextMode = TypeData.MODE.없음;

        currentState = TypeData.State.없음;
        nextState = TypeData.State.없음;
    }

    void Start()
    {
        playerMovement.SetAniMode(currentMode);
        equipHandler.ChangeMode(currentMode);
    }

    void Update()
    {
        if (GameManager.Instance.currentGameState != TypeData.GameState.시작)
        {
            return;
        }

        CheckState();
        CheckMode();
        PeaceFromCombat();
    }

    private void CheckState()
    {
        if (nextState == TypeData.State.없음)
        {
            return;
        }

        currentState = nextState;
        nextState = TypeData.State.없음;

        playerMovement.SetAniState(currentState);
    }

    private void CheckMode()
    {
        if (nextMode == TypeData.MODE.없음)
        {
            return;
        }

        currentMode = nextMode;
        nextMode = TypeData.MODE.없음;

        playerMovement.SetAniMode(currentMode);
        equipHandler.ChangeMode(currentMode);
    }

    // 전투모드에서 5초동안 hit가 없다면 평화모드로
    private void PeaceFromCombat()
    {
        if (currentMode != TypeData.MODE.전투 || currentState == TypeData.State.스킬)
        {
            combatTimer = 0f;
            return;
        }

        combatTimer += Time.deltaTime;

        // 데미지를 받았을때도 초기화하자
        if (playerMovement.isDamage)
        {
            combatTimer = 0f;
            playerMovement.isDamage = false;
        }

        // Hit 되면 타이머 초기화
        if (playerMovement.isHit)
        {
            combatTimer = 0f;
            playerMovement.isHit = false;
        }

        if (combatTimer >= noHitCombatTime)
        {
            playerMovement.isHit = false;
            nextMode = TypeData.MODE.평화;
            combatTimer = 0f;
        }
    }
}