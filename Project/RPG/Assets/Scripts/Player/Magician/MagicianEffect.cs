using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicianEffect : MonoBehaviour
{
    private MagicianSkill magicianSkill = null;
    private PlayerInfoData playerInfoData = null;
    private PlayerState playerState = null;
    private PlayerMovement playerMovement = null;

    [System.Serializable]
    public class MagicianEffectSettings
    {
        public string       fireBall              = "FireBall";
        public string       startMpCondensing     = "StartMpCondensing";
        public string       loopMpCondensing      = "LoopMpCondensing";
        public string       endMpCondensing       = "EndMpCondensing";
        public string       startTeleport         = "StartTeleport";
        public string       endTeleport           = "EndTeleport";
        public string       iceStorm              = "IceStorm";
        public string       meteor                = "Meteor";

        public Transform    leftPoint;          // 왼손
        public Transform    rightPoint;         // 오른손
        public Transform    skillHolder;        // 스킬 이펙트 부모
        public string       effectPath;         // 이펙트 프리펩 경로
        public List<GameObject> fireBallObjs;   // 파이어볼 풀
    }

    [SerializeField]
    public MagicianEffectSettings effectSettings;

    public Dictionary<string, GameObject>   effects         = null; // 이펙트

    private SkinnedMeshRenderer[]           meshes          = null; // 메쉬(텔포때 잠시 투명화)

    private Transform                       teleportPointT  = null; // 순간이동 위치
    public  Vector3                         telWallPos      = Vector3.zero; // 벽에 막혔을때 순간이동 위치

    private int fireBallCount = 0;

    void Awake()
    {
        magicianSkill   = GetComponent<MagicianSkill>();
        playerState     = GetComponent<PlayerState>();
        playerMovement  = GetComponent<PlayerMovement>();
        playerInfoData  = PlayerInfoData.Instance;

        effects         = new Dictionary<string, GameObject>();

        meshes          = GetComponentsInChildren<SkinnedMeshRenderer>();

        teleportPointT  = transform.FindChild("TeleportPoint");

        effectSettings.skillHolder  = transform.FindChild("SkillHolder");
        effectSettings.effectPath   = "Effect/Player/Magician/";

        // 스킬 이펙트 생성
        FireBallEffect();
        ResourceLoad();
    }

    void Update()
    {
        if (GameManager.Instance.currentGameState == TypeData.GameState.종료)
        {
            return;
        }

        if (playerState.currentState == TypeData.State.죽음)
        {
            FireBallResetEffect();
            return;
        }
    }

    // 이펙트 리소스 로드
    private void ResourceLoad()
    {
        GameObject obj = Resources.Load(effectSettings.effectPath + effectSettings.startMpCondensing) as GameObject;
        effects.Add(effectSettings.startMpCondensing, CreateEffectObj(obj, effectSettings.startMpCondensing));

        obj = Resources.Load(effectSettings.effectPath + effectSettings.loopMpCondensing) as GameObject;
        effects.Add(effectSettings.loopMpCondensing, CreateEffectObj(obj, effectSettings.loopMpCondensing));

        obj = Resources.Load(effectSettings.effectPath + effectSettings.endMpCondensing) as GameObject;
        effects.Add(effectSettings.endMpCondensing, CreateEffectObj(obj, effectSettings.endMpCondensing));

        obj = Resources.Load(effectSettings.effectPath + effectSettings.startTeleport) as GameObject;
        effects.Add(effectSettings.startTeleport, CreateEffectObj(obj, effectSettings.startTeleport));

        obj = Resources.Load(effectSettings.effectPath + effectSettings.endTeleport) as GameObject;
        effects.Add(effectSettings.endTeleport, CreateEffectObj(obj, effectSettings.endTeleport));

        obj = Resources.Load(effectSettings.effectPath + effectSettings.iceStorm) as GameObject;
        effects.Add(effectSettings.iceStorm, CreateEffectObj(obj, effectSettings.iceStorm));

        obj = Resources.Load(effectSettings.effectPath + effectSettings.meteor) as GameObject;
        effects.Add(effectSettings.meteor, CreateEffectObj(obj, effectSettings.meteor));

        obj = null;
    }

    // 이펙트 오브젝트 생성
    private GameObject CreateEffectObj(GameObject effectObj, string effectName)
    {
        GameObject obj = Instantiate(effectObj) as GameObject;
        obj.name = effectName;
        obj.transform.SetParent(effectSettings.skillHolder);
        obj.GetComponent<EffectSetting>().infoSettings.effectHoler = effectSettings.skillHolder;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.SetActive(false);

        return obj;
    }

    // 파이어볼 생성
    private void FireBallEffect()
    {
        var resource = Resources.Load("Effect/Player/Magician/FireBall");

        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(resource) as GameObject;
            obj.transform.SetParent(effectSettings.skillHolder);
            obj.transform.localPosition = Vector3.zero;
            obj.SetActive(false);
            effectSettings.fireBallObjs.Add(obj);
        }
    }

    // 파이어볼 리셋
    public void FireBallResetEffect()
    {
        for (int i = 0; i < effectSettings.fireBallObjs.Count; i++)
        {
            if (effectSettings.fireBallObjs[i].activeSelf)
            {
                effectSettings.fireBallObjs[i].SetActive(false);
                effectSettings.fireBallObjs[i].transform.SetParent(effectSettings.skillHolder);
                effectSettings.fireBallObjs[i].transform.localPosition = Vector3.zero;
            }
        }
    }

    // 파이어볼 스킬 사용
    public void FireBall(Vector3 lookPos, float skillAtt)
    {
        if (fireBallCount >= effectSettings.fireBallObjs.Count)
        {
            fireBallCount = 0;
        }

        // 사용자, ball 처음위치, 방향, 스피드, 최대거리, 공격력, 히트이펙트이름
        effectSettings.fireBallObjs[fireBallCount].transform.position = effectSettings.rightPoint.position;
        effectSettings.fireBallObjs[fireBallCount].SetActive(true);

        BallControl ballControl = effectSettings.fireBallObjs[fireBallCount].GetComponent<BallControl>();
        ballControl.SetBall(gameObject.transform, effectSettings.rightPoint.position, lookPos, 15, 15, skillAtt, "FireBall");

        fireBallCount++;
    }

    // 마력응집
    IEnumerator MpCondensingEffect()
    {
        if (effects[effectSettings.startMpCondensing].activeSelf)
        {
            effects[effectSettings.startMpCondensing].SetActive(false);
        }

        if (effects[effectSettings.loopMpCondensing].activeSelf)
        {
            effects[effectSettings.loopMpCondensing].SetActive(false);
        }

        if (effects[effectSettings.endMpCondensing].activeSelf)
        {
            effects[effectSettings.endMpCondensing].SetActive(false);
        }

        effects[effectSettings.startMpCondensing].SetActive(true);

        yield return new WaitForSeconds(0.8f);

        effects[effectSettings.startMpCondensing].SetActive(false);
        effects[effectSettings.loopMpCondensing].SetActive(true);

        yield return new WaitForSeconds(3.9f);

        effects[effectSettings.endMpCondensing].SetActive(true);
        effects[effectSettings.loopMpCondensing].SetActive(false);

        playerMovement.animator.SetTrigger(magicianSkill.magicianAniSettings.isEndMpCondensingTrigger);

        playerInfoData.SetCurrentMp(magicianSkill.skillInfo.mp);

        yield return new WaitForSeconds(1.38f);

        effects[effectSettings.endMpCondensing].SetActive(false);

    }

    // 순간이동 시작
    public void StartTeleportEffect(Vector3 _telWallPos)
    {
        if (effects[effectSettings.startTeleport].activeSelf)
        {
            effects[effectSettings.startTeleport].SetActive(false);
        }

        if (telWallPos != Vector3.zero)
        {
            telWallPos = _telWallPos;
        }

        effects[effectSettings.startTeleport].SetActive(true);
    }

    // 순간이동시 메쉬 감춤
    IEnumerator TeleportMeshDisable()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].enabled = false;
        }
    }

    // 순간이동 끝
    public void EndTeleportEffect()
    {
        if (effects[effectSettings.endTeleport].activeSelf)
        {
            effects[effectSettings.endTeleport].SetActive(false);
        }

        effects[effectSettings.endTeleport].SetActive(true);

        // 플레이어 메쉬 On
        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].enabled = true;
        }

        // 순간이동 범위 내에 벽이 있으면 벽 앞으로 이동
        if (magicianSkill.telWallPos != Vector3.zero)
        {
            transform.localPosition = magicianSkill.telWallPos;
            telWallPos = Vector3.zero;
        }
        // 범위내에 없으면 10m 순간이동
        else
        {
            transform.localPosition = teleportPointT.position;
        }

        playerMovement.charCtrl.enabled = true;
    }

    // 얼음폭풍
    public void IceStormEffect(float skillAngle, float skillDistance, float skillAtt)
    {
        if (effects[effectSettings.iceStorm].activeSelf)
        {
            if (effects[effectSettings.iceStorm].transform.parent != effectSettings.skillHolder)
            {
                effects[effectSettings.iceStorm].transform.SetParent(effectSettings.skillHolder);
            }

            effects[effectSettings.iceStorm].SetActive(false);
        }

        effects[effectSettings.iceStorm].SetActive(true);

        EffectSetting effect = effects[effectSettings.iceStorm].GetComponent<EffectSetting>();
        effect.SetSkillInfo(skillAngle, skillDistance, skillAtt); // 스킬 정보 전달
    }

    // 운석낙하
    public void MeteorEffect(float skillAngle, float skillDistance, float skillAtt)
    {
        if (effects[effectSettings.meteor].activeSelf)
        {
            if (effects[effectSettings.meteor].transform.parent != effectSettings.skillHolder)
            {
                effects[effectSettings.meteor].transform.SetParent(effectSettings.skillHolder);
            }

            effects[effectSettings.meteor].SetActive(false);
        }
        

        effects[effectSettings.meteor].SetActive(true);
     
        EffectSetting effect = effects[effectSettings.meteor].GetComponent<EffectSetting>();
        effect.SetSkillInfo(skillAngle, skillDistance, skillAtt); // 스킬 정보 전달
    }
}
