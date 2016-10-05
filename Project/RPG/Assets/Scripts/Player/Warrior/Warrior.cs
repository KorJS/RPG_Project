using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Warrior : MonoBehaviour
{
    private PlayerMovement playerMovement = null;
    private PlayerInput playerInput = null;

    // 워리어 애니메이션 파라미터명 설정
    [System.Serializable]
    public class WarriorAniSettings
    {
        public string isEndComboTrigger     = "isEndCombo";
        public string isCombo01Trigger      = "isCombo01";
        public string isCombo02Trigger      = "isCombo02";
        public string isOverpowerTrigger    = "isOverpower";
        public string isRushBool            = "isRush";
        public string isStartBlockTrigger   = "isStartBlock";
        public string isEndBlockTrigger     = "isEndBlock";
    }

    [SerializeField]
    public WarriorAniSettings warriorAniSettings;

    public enum SkillType
    {
        없음 = -1,
        연속공격 = 0,
        방패막기,
        압도,
        난폭한돌진
    };

    public SkillType currentSkillTpye = SkillType.없음; // 현재 스킬

    Dictionary<int, SKillData.SkillInfo> skillInfos = null; // 스킬 정보 받아옴

    private const float COMBOTIME = 1.5f;   // 연속공격 입력타임
    private const float RUSHTIME = 1.5f;    // 난폭한 돌진 지속타임

    public float comboTimer = COMBOTIME;    // 연속공격 입력타이머
    public bool isComboTime = false;        // 콤보 타임이 지났는지
    public bool isCombo = false;            // 몇번 콤보 공격인지

    public float rushTimer = RUSHTIME;      // 난폭한 돌진 지속타이머
    public bool isRush = false;             // 난폭한 돌진 중인지
    public bool isBlock = false;            // 방패막기 중인지

    // TODO : 공격 범위.
    public float skillAngle = 0f;
    public float skillRange = 0f;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();

        currentSkillTpye = SkillType.없음;

        skillInfos = SKillData.Instance.CheckPlayerType(TypeData.PlayerType.기사); // 기사 스킬 정보
    }

    void Update()
    {
        // 스킬 서브상태머신을 빠져나오면 콤보 모션초기화
        if (playerMovement.isEndSkillPoint)
        {
            isCombo = false;
        }

        CheckCurrentAnimation();
        SwitchSkill();
        CheckComboTime();
        ChekRuchTime();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, skillRange);
    }

    private void CheckCurrentAnimation()
    {
        if (!playerMovement.animator)
        {
            Debug.Log("playerMovement.animator Null");
            return;
        }

        if (isBlock)
        {
            if (Input.GetMouseButtonUp(1))
            {
                playerMovement.animator.SetTrigger(warriorAniSettings.isEndBlockTrigger);
                isBlock = false;
            }
        }
    }

    // 각 스킬별로 공격범위에 있는 적 검색
    public void SeachSkillRange()
    {
        Vector3 playerPos = transform.position;
        
        Collider[] colls = Physics.OverlapSphere(playerPos, skillRange);

        foreach (Collider col in colls)
        {
            if (string.Compare(col.tag, "Enemy") == 0)
            {
                Vector3 enemyPos = col.transform.position - transform.position;

                float angle = Vector3.Angle(transform.forward, enemyPos);

                if (angle <= skillAngle)
                {
                    Debug.Log(angle);
                }
            }
        }
    }

    private void SwitchSkill()
    {
        // 특정스킬 사용중에는 입력 안들어오게 막기
        if (LockSkill())
        {
            playerInput.index = -1;
            return;
        }

        // 클릭한 곳에 스킬이 없거나 / 스킬 상태가 아닐때
        if (playerInput.index < 0 && PlayerState.Instance.currentState != TypeData.State.스킬)
        {
            return;
        }

        // 어떤 스킬인지 알아옴
        currentSkillTpye = (SkillType)playerInput.index;
        playerInput.index = -1;

        switch (currentSkillTpye)
        {
            case SkillType.연속공격: { ComboAttack(); } break;
            case SkillType.방패막기: { ShieldBlock(); } break;
            case SkillType.압도: { Overpower(); } break;
            case SkillType.난폭한돌진:{ Rush(); } break;
        }
    }

    // 특정스킬 사용중에는 입력 안들어오게 막기
    private bool LockSkill()
    {
        bool isLock = false;

        switch (currentSkillTpye)
        {
            case SkillType.압도:
                isLock = playerMovement.isIdle;
                break;
        }

        return isLock;
    }

    // 연속공격
    private void ComboAttack()
    {
        skillAngle = 60f;
        skillRange = 3f;
        SeachSkillRange();

        playerMovement.AnimationSkill((int)currentSkillTpye);

        // 콤보 공격 방향 성정
        float h = Input.GetAxisRaw(playerInput.inputKey.horizontal);
        float v = Input.GetAxisRaw(playerInput.inputKey.vertical);

        playerMovement.Rotation(v, h, true); // 입력된 방향으로 회전

        if (!isCombo)
        {
            playerMovement.animator.SetTrigger(warriorAniSettings.isCombo01Trigger);
            isCombo = true;
        }
        else if (isCombo)
        {
            playerMovement.animator.SetTrigger(warriorAniSettings.isCombo02Trigger);
            isCombo = false;
        }

        isComboTime = true;
        comboTimer = COMBOTIME;
    }

    // 연속공격 타임 체크
    // 연속공격에 1.5초안에 입력이 안들어오면 대기상태로. 다시입력햇을때 1콤 모션 상태로 초기화
    private void CheckComboTime()
    {
        // 콤보타임중이 아닐때 / 스킬이 끝나고 idle(서브상태머신 안에있는) 상태일떄 / 스킬상태가 아닐때
        if (!isComboTime || playerMovement.isIdle || PlayerState.Instance.currentState != TypeData.State.스킬)
        {
            comboTimer = COMBOTIME; // 콤보타임 초기화
            return;
        }

        comboTimer -= Time.deltaTime;

        // 3초 안에 클릭 못할시. 1콤에서 끝
        if (comboTimer < 0f)
        {
            playerMovement.animator.SetTrigger(warriorAniSettings.isEndComboTrigger);
            comboTimer = COMBOTIME;
            isComboTime = false;
            isCombo = false; // 1콤보에서 끊겨서 다음 모션초기화 1콤으로
            PlayerState.Instance.nextState = TypeData.State.대기;
        }
    }

    // 방패 막기
    private void ShieldBlock()
    {
        // TODO : 나중에 변경
        skillAngle = 90f;
        skillRange = 2f;
        SeachSkillRange();

        playerMovement.AnimationSkill((int)currentSkillTpye);
        playerMovement.animator.SetTrigger(warriorAniSettings.isStartBlockTrigger);
        isBlock = true;

        playerMovement.Rotation(1f, 0f, true); // 전방 방향
    }

    // 압도
    private void Overpower()
    {
        playerMovement.AnimationSkill((int)currentSkillTpye);
        playerMovement.animator.SetTrigger(warriorAniSettings.isOverpowerTrigger);

        playerMovement.Rotation(1f, 0f, true); // 전방 방향
    }

    // 난폭한 돌진
    private void Rush()
    {
        skillAngle = 90f;
        skillRange = 4f;
        SeachSkillRange();

        playerMovement.AnimationSkill((int)currentSkillTpye);
        isRush = true;
        playerMovement.animator.SetBool(warriorAniSettings.isRushBool, isRush);
    }

    // 난폭한 돌진 유지시간 체크
    private void ChekRuchTime()
    {
        if (!isRush || currentSkillTpye == SkillType.방패막기)
        {
            rushTimer = RUSHTIME;
            isRush = false;
            return;
        }

        rushTimer -= Time.deltaTime;
        playerMovement.Rotation(1f, 0f, true); // 전방 방향 지속

        if (rushTimer < 0f)
        {
            rushTimer = RUSHTIME;
            isRush = false;
            playerMovement.animator.SetBool(warriorAniSettings.isRushBool, isRush);
        }
    }
}