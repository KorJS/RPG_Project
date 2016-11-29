using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicianEffect : MonoBehaviour
{
    private MagicianSkill magicianSkill = null;
    private PlayerState playerState = null;

    [System.Serializable]
    public class MagicianEffectSettings
    {
        public string fireBall = "FireBall";
        public string mpCondensing = "MpCondensing";
        public string startTeleport = "StartTeleport";
        public string endTeleport = "EndTeleport";
        public string iceStorm = "IceStorm";
        public string meteor = "Meteor";

        public Transform leftPoint;
        public Transform rightPoint;
        public Transform skillHolder;
        public string effectPath;
        public List<GameObject> fireBallObjs;
    }

    [SerializeField]
    public MagicianEffectSettings effectSettings;

    public Dictionary<string, GameObject> effects = null;

    private int fireBallCount = 0;

    void Awake()
    {
        magicianSkill = GetComponent<MagicianSkill>();
        playerState = GetComponent<PlayerState>();

        effectSettings.skillHolder = transform.FindChild("SkillHolder");
        effectSettings.effectPath = "Effect/Player/Magician/";

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
        GameObject obj = Resources.Load(effectSettings.effectPath + effectSettings.mpCondensing) as GameObject;
        effects.Add(effectSettings.mpCondensing, CreateEffectObj(obj, effectSettings.mpCondensing));

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

    public void MpCondensingEffect()
    {
        if (effects[effectSettings.mpCondensing].activeSelf)
        {
            effects[effectSettings.mpCondensing].SetActive(false);
        }

        effects[effectSettings.mpCondensing].SetActive(true);
    }

    public void StartTeleportEffect()
    {
        if (effects[effectSettings.startTeleport].activeSelf)
        {
            effects[effectSettings.startTeleport].SetActive(false);
        }

        effects[effectSettings.startTeleport].SetActive(true);
    }

    public void EndTeleportEffect()
    {
        if (effects[effectSettings.endTeleport].activeSelf)
        {
            effects[effectSettings.endTeleport].SetActive(false);
        }

        effects[effectSettings.endTeleport].SetActive(true);
    }

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

    }

    public void MeteorEffect()
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
    }
}
