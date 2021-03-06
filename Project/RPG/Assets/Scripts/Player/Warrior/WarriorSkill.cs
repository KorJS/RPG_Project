﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarriorSkill : MonoBehaviour
{
    private SkillData       skillData       = null; // 해당 클레스의 모든 스킬 정보
    private PlayerInfoData  playerInfoData  = null; // 주인공 정보
    private PlayerSkillData playerSkillData = null; // 주인공이 배운 스킬 정보 받아옴
    private PlayerMovement  playerMovement  = null; // 주인공 동작
    private PlayerInput     playerInput     = null; // 주인공 입력
    private PlayerState     playerState     = null; // 주인공 상태
    private PlayerEffect    playerEffect    = null; // 주인공 이펙트 
    private WarriorEffect   warriorEffect   = null; // 기사 이펙트
    private WarriorSound    warriorSound    = null; // 기사 사운드
    private UIManager       uiManager       = null; // ui 매니저
    private CameraControl   cameraCtrl      = null; // 카메라 컨트롤(흔들림)

    // 애니메이션 파라미터명 설정
    [System.Serializable]
    public class WarriorAniSettings
    {
        public string isEndComboTrigger     = "isEndCombo";
        public string isCombo01Trigger      = "isCombo01";
        public string isCombo02Trigger      = "isCombo02";
        public string isTumblingTrigger     = "isTumbling";
        public string isOverpowerTrigger    = "isOverpower";
        public string isRushBool            = "isRush";
        public string isStartBlockTrigger   = "isStartBlock";
        public string isEndBlockTrigger     = "isEndBlock";
    }

    public WarriorAniSettings warriorAniSettings;

    private SkillData.SkillInfo skillInfo; // 스킬 정보

    public enum SkillType
    {
        없음 = -1,
        연속공격 = 0,
        방패막기,
        긴급회피,
        난폭한돌진,
        압도,
    };

    public SkillType currentSkillTpye = SkillType.없음; // 현재 스킬

    private const float             COMBOTIME       = 1.5f;         // 연속공격 입력타임
    private const float             RUSHTIME        = 2.5f;         // 난폭한 돌진 지속타임

    public float                    comboTimer      = 0f;           // 연속공격 입력타이머
    public bool                     isComboTime     = false;        // 콤보 타임이 지났는지
    public bool                     isCombo         = false;        // 몇번 콤보 공격인지

    private KeyCode                 blockKeyCode    = KeyCode.None; // 방패막기 단축키
    public  bool                    isBlock         = false;        // 방패막기 중인지

    // 스킬 범위.
    public float                    skillAngle      = 0f;           // 각도
    public float                    skillDistance   = 0f;           // 거리


    // 난폭한 돌진
    public  List<MonsterMovement>   rushMob         = null;         // 난폭한 돌진중에 붙딛힌 몬스터 스크립트
    private Transform               rushHolder      = null;         // 난폭한 돌진중에 붙딛힌 몬스터의 부모
    public  float                   rushTimer       = RUSHTIME;     // 난폭한 돌진 지속타이머
    public  bool                    isRush          = false;        // 난폭한 돌진 중인지

    public AudioClip[]              hitsBGN         = null;         // 히트 사운드

    void Awake()
    {
        playerInfoData  = PlayerInfoData.Instance;
        playerSkillData = PlayerSkillData.Instance;
        playerMovement  = GetComponent<PlayerMovement>();
        playerInput     = GetComponent<PlayerInput>();
        playerState     = GetComponent<PlayerState>();
        playerEffect    = GetComponent<PlayerEffect>();
        warriorEffect   = GetComponent<WarriorEffect>();
        warriorSound    = GetComponent<WarriorSound>();
        cameraCtrl      = GameObject.FindGameObjectWithTag("CameraCtrl").GetComponent<CameraControl>();

        currentSkillTpye = SkillType.없음;

        rushHolder      = transform.FindChild("RushHolder");
        rushMob         = new List<MonsterMovement>();
    }

    void Start()
    {
        uiManager = UIManager.Instance;
    }

    void Update()
    {
        if (GameManager.Instance.currentGameState != TypeData.GameState.시작)
        {
            return;
        }

        if (playerState.currentState == TypeData.State.죽음)
        {
            return;
        }

        if (playerMovement.isDamage)
        {
            playerMovement.animator.ResetTrigger(warriorAniSettings.isCombo01Trigger);
            playerMovement.animator.ResetTrigger(warriorAniSettings.isCombo02Trigger);
            playerMovement.isDamage = false;
        }

        CheckCurrentAniState();
        SwitchSkill();
        CheckComboTime();
        ChekRushTime();
    }

    private void CheckCurrentAniState()
    {
        if (!playerMovement.animator)
        {
            Debug.Log("playerMovement.animator Null");
            return;
        }

        if (isBlock)
        {
            if (Input.GetKeyUp(blockKeyCode) || playerInput.isClick || !playerMovement.isBlock)
            {
                blockKeyCode = KeyCode.None;

                if (playerMovement.isBlock)
                {
                    playerMovement.animator.SetTrigger(warriorAniSettings.isEndBlockTrigger);
                }

                isBlock = false;
                playerMovement.isBlock = isBlock;
            }

            
        }
    }

    // 각 스킬별로 공격범위에 있는 적 검색
    public void SeachSkillRange()
    {
        bool _isHit = false;

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

                    _isHit = true;
                }
            }
        }

        if (_isHit)
        {
            warriorEffect.ComboHitEffect(true);
            SoundManager.Instance.RandomEfx(hitsBGN);
        }
    }

    private void Damage(GameObject enemyObj)
    {
        float attack = playerInfoData.totalAtt * skillInfo.attack;

        //uiManager.SetHpBar(enemyObj.transform);

        MonsterMovement mob = enemyObj.GetComponent<MonsterMovement>();

        Debug.Log("attack : " + attack);
        
        // TODO : SKillInfo의 데미지를 적용
        if (currentSkillTpye == SkillType.난폭한돌진)
        {
            // 난폭한 돌진에 맞은 몬스터 리스트에 추가
            if (!rushMob.Contains(mob))
            {
                rushMob.Add(mob);
            }

            mob.RushDamage(rushHolder);

            return;
        }

        mob.SetDamage(gameObject.transform, -attack);
    }

    // TODO : 다음에는 Queue와 입력시간 Timer를 사용해서 지정한 Time 안에 입력이 없을시 연속으로 안나가게 막하볼까..
    // Queue를 사용해서 입력한것을 순서대로 넣고 빼서 스킬 시전을 하면.. 이상하려나..
    private void SwitchSkill()
    {
        // 특정스킬 연속사용하기
        UnLockSkill();

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

        playerInfoData.SetCurrentMp(skillInfo.mp);

        switch (currentSkillTpye)
        {
            case SkillType.연속공격: { ComboAttack(); } break;
            case SkillType.방패막기: { ShieldBlock(); } break;
            case SkillType.긴급회피: { Tumbling(); } break;
            case SkillType.압도: { Overpower(); } break;
            case SkillType.난폭한돌진: { Rush(); } break;
        }
    }

    // 특정스킬 연속 사용하게 하기
    private void UnLockSkill()
    {
        switch (currentSkillTpye)
        {
            case SkillType.연속공격:
                playerMovement.isIdle = true;
                break;
        }
    }

    // 연속공격
    private void ComboAttack()
    {
        playerMovement.SetAniSkill((int)currentSkillTpye);

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
        comboTimer = 0f;
    }

    // 연속공격 타임 체크
    // 연속공격에 1.5초안에 입력이 안들어오면 대기상태로. 다시입력햇을때 1콤 모션 상태로 초기화
    private void CheckComboTime()
    {
        // 콤보타임중이 아닐때 / 스킬이 끝나고 idle(서브상태머신 안에있는) 상태일떄 / 스킬상태가 아닐때
        if (!isComboTime || !playerMovement.isIdle || playerState.currentState != TypeData.State.스킬)
        {
            comboTimer = 0f; // 콤보타임 초기화
            return;
        }

        comboTimer += Time.deltaTime;

        // 1.5초 안에 클릭 못할시. 1콤에서 끝
        if (comboTimer > COMBOTIME)
        {
            playerMovement.animator.SetTrigger(warriorAniSettings.isEndComboTrigger);
            comboTimer = 0f;
            isComboTime = false;
            isCombo = false; // 1콤보에서 끊겨서 다음 모션초기화 1콤으로
        }
    }

    // 방패 막기
    private void ShieldBlock()
    {
        blockKeyCode = playerInput.tempKeyCode;

        // TODO : 나중에 변경
        skillAngle = 90f;
        skillDistance = 2f;

        playerMovement.SetAniSkill((int)currentSkillTpye);
        playerMovement.animator.SetTrigger(warriorAniSettings.isStartBlockTrigger);
        isBlock = true;
        playerMovement.isBlock = isBlock;

        playerMovement.Rotation(1f, 0f, true); // 전방 방향

        StartCoroutine(ContinueMPBlock()); // 방패막기 중에 mp 지속 감소
    }

    // 방패막기 중에 mp 지속 감소
    IEnumerator ContinueMPBlock()
    {
        while (isBlock)
        {
            yield return new WaitForSeconds(1.5f);

            playerInfoData.SetCurrentMp(skillInfo.mp / 10f);
        }
    }

    // 긴급회피
    private void Tumbling()
    {
        // 회피기, 무적 이므로 캐릭터 컨트롤러 비활성화
        playerMovement.charCtrl.enabled = false;
        playerMovement.SetAniSkill((int)currentSkillTpye);
        playerMovement.animator.SetTrigger(warriorAniSettings.isTumblingTrigger);
    }

    public void TumblingEnd()
    {
        playerMovement.charCtrl.enabled = true;
    }

    // 압도
    private void Overpower()
    {
        skillAngle = 70f;
        skillDistance = 4f;

        playerMovement.SetAniSkill((int)currentSkillTpye);
        playerMovement.animator.SetTrigger(warriorAniSettings.isOverpowerTrigger);

        playerMovement.Rotation(1f, 0f, true); // 전방 방향
    }

    // 난폭한 돌진
    private void Rush()
    {
        skillAngle = 90f;
        skillDistance = 4f;

        warriorSound.SetRushBGM();

        playerMovement.SetAniSkill((int)currentSkillTpye);
        playerMovement.animator.SetBool(warriorAniSettings.isRushBool, isRush);

        isRush = true;
    }

    // 난폭한 돌진 유지시간 체크
    private void ChekRushTime()
    {
        if (!isRush)
        {
            rushTimer = RUSHTIME;
            return;
        }

        if (currentSkillTpye == SkillType.방패막기)
        {
            RushMonsterOut(); // 한번 호출
            rushTimer = RUSHTIME;
            isRush = false;
            return;
        }

        SeachSkillRange();
        rushTimer -= Time.deltaTime;
        playerMovement.Rotation(1f, 0f, true); // 전방 방향 지속

        if (rushTimer < 0f)
        {
            RushMonsterOut();
            rushTimer = RUSHTIME;
            isRush = false;
            playerMovement.animator.SetBool(warriorAniSettings.isRushBool, isRush);
        }
    }

    // 난폭한돌진 하는동안 붙어있던 몬스터들 때어놓는다
    private void RushMonsterOut()
    {
        for (int i = 0; i < rushMob.Count; i++)
        {
            rushMob[i].RushEnd();
        }
    }

    public void Shake()
    {
        cameraCtrl.Shake(0.2f, 0.2f, 0.2f);
    }
}