using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerEffect))]
[RequireComponent(typeof(PlayerState))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    private PlayerInput playerInput = null;
    private PlayerState playerState = null;

    // 애니메이션 파라미터명 설정
    [System.Serializable]
    public class AnimationSettings
    {
        public string moveVFloat            = "MoveV";
        public string moveHFloat            = "MoveH";
        public string modeInt               = "Mode";
        public string stateInt              = "State";
        public string skillTpyeInt          = "SkillType";
        public string hitBool               = "isHit";
        public string isBlockTrigger        = "isBlock";
        public string isDamageTrigger       = "isDamage";
        public string isDeathTrigger        = "isDeath";
    }

    [SerializeField]
    public AnimationSettings animationSettings;

    public CharacterController charCtrl = null;
    private CameraControl cameraCtrl = null;
    public Animator animator = null;
    private Camera mainCamera = null;
    private Vector3 rotation = Vector3.zero;

    public bool isHit = false;              // 공격해 맞혔으면 true - 각 스킬스크립트에서 전달 받자
    public bool isDamage = false;           // 공격에 맞았으면 true - 몬스터로부터 맞았으면.
    public bool isIdle = false;             // 스킬 끝나고 idle 상태
    public bool isBlock = false;            // 방패막기 중에는 공격 안받음

    private GameObject respawnObj = null;
    private float deathTimer = 10;
    private bool isDeath = false;

    public AudioClip deathBGM = null;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerState = GetComponent<PlayerState>();
        charCtrl = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        cameraCtrl = GameObject.FindGameObjectWithTag("CameraCtrl").GetComponent<CameraControl>();

        isHit = isDamage = false;
        isIdle = true; // 대기상태

        //SetAnimator();

        // 마지막에 저장된 위치에 스폰.
        string[] strPos = PlayerInfoData.Instance.infoData.spawnPos.Split(',');

        float x = float.Parse(strPos[0]);
        float y = float.Parse(strPos[1]);
        float z = float.Parse(strPos[2]);
        
        transform.position = new Vector3(x, y, z);

        respawnObj = GameObject.Find("PlayerRespawn");
    }

    void Update()
    {
        if (GameManager.Instance.currentGameState != TypeData.GameState.시작)
        {
            return;
        }

        if (!isDeath && PlayerInfoData.Instance.infoData.currentHp <= 0)
        {
            cameraCtrl.cameraSettings.deathFilter.enabled = true;
            isDeath = true;
            gameObject.layer = LayerMask.NameToLayer("Default");
            playerState.nextState = TypeData.State.죽음;
            animator.SetTrigger(animationSettings.isDeathTrigger);
            SoundManager.Instance.PlaySingleVoice(deathBGM);
            charCtrl.enabled = false;
            StartCoroutine(Death());
        }

        Vector3 playerPos = transform.position;

        PlayerInfoData.Instance.infoData.spawnPos = playerPos.x + "," + playerPos.y + "," + playerPos.z;

        // 공격해 맞혔으면 true
        animator.SetBool(animationSettings.hitBool, isHit);
    }

    IEnumerator Death()
    {
        while (isDeath)
        {
            UIManager.Instance.SetMessage("부활까지 남은 시간 : " + Mathf.RoundToInt(deathTimer));

            deathTimer -= Time.deltaTime;

            if (deathTimer <= 0)
            {
                SoundManager.Instance.voiceSource = null;

                PlayerInfoData.Instance.infoData.currentHp = PlayerInfoData.Instance.totalMaxHp * 0.40f;
                deathTimer = 10f;
                transform.position = respawnObj.transform.position;
                transform.rotation = respawnObj.transform.rotation;
                playerState.nextState = TypeData.State.대기;
                playerState.nextMode = TypeData.MODE.평화;
                gameObject.layer = LayerMask.NameToLayer("Player");
                charCtrl.enabled = true;
                isDeath = false;
                cameraCtrl.cameraSettings.deathFilter.enabled = false;

                UIManager.Instance.windowSettings.fadePanel.alpha = 1f;
                GameManager.Instance.isFade = true;
            }

            yield return null;
        }

    }

    // 데미지
    public void SetDamage(Transform monsterT, float damage)
    {
        //if (playerState.currentMode == TypeData.MODE.평화 || playerState.currentState == TypeData.State.스킬)
        {
        }

        if (isBlock)
        {
            if (CheckRange(monsterT))
            {
                animator.SetTrigger(animationSettings.isBlockTrigger);
                return;
            }
            else
            {
                isBlock = false;
            }
        }

        animator.SetTrigger(animationSettings.isDamageTrigger);

        Debug.Log(damage);
        PlayerInfoData.Instance.SetCurrentHp(damage);

        UIManager.Instance.SetDamageTxt(transform, damage, Color.red);

        isDamage = true;
        playerState.nextMode = TypeData.MODE.전투;
    }

    // 모드
    public void SetAniMode(TypeData.MODE mode)
    {
        animator.SetInteger(animationSettings.modeInt, (int)mode);
    }

    // 상태
    public void SetAniState(TypeData.State state)
    {
        animator.SetInteger(animationSettings.stateInt, (int)state);
    }

    // 스킬
    public void SetAniSkill(int skillTpye)
    {
        isIdle = false; // 스킬 중일때 회전 / 스킬 막기.
        animator.SetInteger(animationSettings.skillTpyeInt, skillTpye);
    }

    // 이동
    public void SetAniMove(float v, float h, bool isSkill)
    {
        // 스킬사용중에 입력신호가 스킬이 아닐때(이동신호이면)
        if (!isIdle && !isSkill)
        {
            return;
        }

        if (v != 0 || h != 0)
        {
            playerState.nextState = TypeData.State.이동;
        }

        animator.SetFloat(animationSettings.moveVFloat, v);
        animator.SetFloat(animationSettings.moveHFloat, h);
    }

    // 케릭터 방향(회전)
    public void Rotation(float v, float h, bool isSkill)
    {
        // 스킬사용중에 입력신호가 스킬이 아닐때(이동신호이면)
        if (!isIdle && !isSkill)
        {
            return;
        }

        Vector3 forword = mainCamera.transform.forward;
        forword.y = 0f;
        forword = forword.normalized;

        Vector3 right = mainCamera.transform.right;
        right.y = 0f;
        right = right.normalized;

        // 카메라 방향에 따라 변하므로 방향 보정
        // TODO : 백터 공부하자.
        Vector3 dir = h * right + v * forword;

        if (dir != Vector3.zero)
        {
            rotation = dir.normalized;
            transform.rotation = Quaternion.LookRotation(rotation);
        }
    }

    // 스킬이 시전이 끝나면 회전 가능 하게.
    public void IdleBotton()
    {
        playerState.nextState = TypeData.State.대기;
        animator.ResetTrigger(animationSettings.isDamageTrigger);
        isIdle = true;
    }

    // 방패막기
    private bool CheckRange(Transform monsterT)
    {
        Vector3 heading = transform.TransformDirection(Vector3.forward);
        Vector3 other = monsterT.position - transform.position;

        heading.y = 0f;
        other.y = 0f;

        heading.Normalize();
        other.Normalize();

        float dp = Vector3.Dot(heading, other);

        if (dp > Mathf.Cos(75f * Mathf.Deg2Rad))
        {
            Debug.Log("방어성공");
            return true;
        }

        Debug.Log("방어실패");
        return false;
    }
}
