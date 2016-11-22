using UnityEngine;
using System.Collections;

public class MagicianSkill : MonoBehaviour
{
    private SkillData skillData = null;
    private PlayerInfoData playerInfoData = null;
    private PlayerSkillData playerSkillData = null; // 스킬 정보 받아옴
    private PlayerMovement playerMovement = null;
    private PlayerInput playerInput = null;
    private PlayerState playerState = null;
    private PlayerEffect playerEffect = null;
    private MagicianEffect magicianEffect = null;
    private UIManager uiManager = null;

    // 애니메이션 파라미터명 설정
    [System.Serializable]
    public class MagicianAniSettings
    {
        public string isFireBall = "isFireBall";
    }

    [SerializeField]
    public MagicianAniSettings magicianAniSettings;

    private SkillData.SkillInfo skillInfo; // 스킬 정보

    public enum SkillType
    {
        없음 = -1,
        파이어볼 = 0,
    };

    public SkillType currentSkillTpye = SkillType.없음; // 현재 스킬

    // TODO : 공격 범위.
    public float skillAngle = 0f;
    public float skillDistance = 0f;

    void Awake()
    {
        playerInfoData = PlayerInfoData.Instance;
        playerSkillData = PlayerSkillData.Instance; // 스킬 정보
        playerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();
        playerState = GetComponent<PlayerState>();
        playerEffect = GetComponent<PlayerEffect>();
        magicianEffect = GetComponent<MagicianEffect>();

        currentSkillTpye = SkillType.없음;
    }

    void Start()
    {
        uiManager = UIManager.Instance;
    }

    void Update()
    {
        if (GameManager.Instance.gameState == TypeData.GameState.종료)
        {
            return;

        }

        if (playerState.currentState == TypeData.State.죽음)
        {
            return;
        }

        SwitchSkill();
    }

    // 각 스킬별로 공격범위에 있는 적 검색
    public void SeachSkillRange()
    {
        bool isHit = false;

        Vector3 playerPos = transform.position; // 범위의 중점.

        skillAngle = skillInfo.angle;
        skillDistance = skillInfo.distance;

        Collider[] targets = Physics.OverlapSphere(playerPos, skillDistance);

        foreach (Collider target in targets)
        {
            if (target.gameObject.layer == LayerMask.NameToLayer("Monster"))
            {
                Vector3 enemyPos = target.transform.position - transform.position;

                float angle = Vector3.Angle(transform.forward, enemyPos);

                if (angle <= skillAngle)
                {
                    // 범위 안에 있으면 Hit
                    //Debug.Log(target.name);
                    playerMovement.isHit = true;
                    Damage(target.gameObject);
                }

                isHit = true;
            }
        }
    }

    private void Damage(GameObject enemyObj)
    {
        float attack = playerInfoData.totalAtt * skillInfo.attack;

        uiManager.SetHpBar(enemyObj.transform);

        MonsterMovement mob = enemyObj.GetComponent<MonsterMovement>();

        Debug.Log("attack : " + attack);

        mob.SetDamage(gameObject.transform, -attack);
    }

    private void SwitchSkill()
    {
        // 특정스킬 사용중에는 입력 안들어오게 막기
        if (LockSkill())
        {
            playerInput.index = -1;
            return;
        }

        // 클릭한 곳에 스킬이 없거나
        if (playerInput.index < 0)
        {
            return;
        }

        // 어떤 스킬인지 알아옴
        int index = playerInput.index;

        skillInfo = SkillData.Instance.skillInfos[index]; // 스킬 정보를 받아옴

        currentSkillTpye = (SkillType)index; // 현제 스킬타입 설정
        playerInput.index = -1;

        // mp 사용
        playerInfoData.SetCurrentMp(skillInfo.mp);

        switch (currentSkillTpye)
        {
            case SkillType.파이어볼: { FireBall(); } break;
        }
    }

    // 특정스킬 사용중에는 입력 안들어오게 막기
    private bool LockSkill()
    {
        bool isLock = false;

        switch (currentSkillTpye)
        {
            case SkillType.파이어볼:
                isLock = !playerMovement.isIdle; // 스킬 사용중이므로 idle 상태가 아님 false !false == true
                break;
        }

        return isLock;
    }

    private void FireBall()
    {
        playerMovement.SetAniSkill((int)currentSkillTpye);
        playerMovement.animator.SetTrigger(magicianAniSettings.isFireBall);
    }
}
