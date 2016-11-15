using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KumasSkill : MonoBehaviour
{
    private MonsterInfoData monsterInfoData = null;
    private MonsterState monsterState = null;
    private MonsterRange monsterRange = null;
    private MonsterMovement monsterMovement = null;

    [System.Serializable]
    public class KumasEffectSettings
    {
        public Transform leftPoint_s;
        public Transform rightPoint_s;
        public Transform leftPoint_e;
        public Transform rightPoint_e;
        public Transform mouthPoint;
        public Transform effectHolder;

        public List<GameObject> iceBallObjs;
        public GameObject breathObj;
    }

    [SerializeField]
    public KumasEffectSettings effects;

    public enum SkillType
    {
        없음 = -1,
        att01 = 0,
        att02, combo01,
        att05,
        
        MAX
    }

    public SkillType skillType = SkillType.없음;

    private Vector3 skillPos = Vector3.zero;
    private Vector3 skillRange = Vector3.zero;
    private float skillAtt = 0f;

    private int iceBallCount = 0;
    private bool isBreath = false;

    void Awake()
    {
        monsterInfoData = GetComponent<MonsterInfoData>();
        monsterState = GetComponent<MonsterState>();
        monsterRange = GetComponent<MonsterRange>();
        monsterMovement = GetComponent<MonsterMovement>();

        effects.effectHolder = GameObject.Find("MonsterEffectPool").transform;
        skillType = SkillType.없음;

        // 스킬 이펙트 생성
        CreateEffect();
    }

    void OnEnable()
    {
        skillType = SkillType.없음;
    } 

    void Update()
    {
        if (monsterState.currentState == TypeData.MonsterState.죽음)
        {
            DeathResetEffect();
            return;
        }

        if (monsterState.currentState == TypeData.MonsterState.데미지)
        {
            return;
        }

        if (monsterState.currentMode == TypeData.MODE.평화)
        {
            ResetSkill();
            return;
        }

        if (monsterState.currentState != TypeData.MonsterState.스킬)
        {
            return;
        }

        if (monsterMovement.isSkill)
        {
            monsterMovement.isSkill = false;
            StartCoroutine(Attack());
        }
    }

    private void CreateEffect()
    {
        var resource = Resources.Load("Effect/Monster/IceBall");

        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(resource) as GameObject;
            obj.transform.SetParent(effects.effectHolder);
            obj.transform.position = Vector3.zero;
            obj.SetActive(false);
            effects.iceBallObjs.Add(obj);
        }

        resource = Resources.Load("Effect/Monster/Breath");
        effects.breathObj = Instantiate(resource) as GameObject;
        effects.breathObj.transform.SetParent(effects.effectHolder);
        effects.breathObj.transform.position = Vector3.zero;
        effects.breathObj.SetActive(false);
    }


    public void DeathResetEffect()
    {
        for (int i = 0; i < effects.iceBallObjs.Count; i++)
        {
            if (effects.iceBallObjs[i].activeSelf)
            {
                effects.iceBallObjs[i].transform.SetParent(effects.effectHolder);
                effects.iceBallObjs[i].transform.position = Vector3.zero;
                effects.iceBallObjs[i].SetActive(false);
            }
        }

        if (effects.breathObj.activeSelf)
        {
            effects.breathObj.transform.SetParent(effects.effectHolder);
            effects.breathObj.transform.position = Vector3.zero;
            effects.breathObj.SetActive(false);
        }
    }

    // 애니메이션 이벤트 - 스킬이 끝날때쯤 호출
    public void ResetSkill()
    {
        skillType = SkillType.없음;
    }

    IEnumerator Attack()
    {
        while (monsterState.currentState == TypeData.MonsterState.스킬)
        {
            if (monsterMovement.isSkillWait)
            {
                Debug.Log("스킬 Go");
                skillType++;

                if (skillType == SkillType.MAX)
                {
                    skillType = SkillType.att01;
                }

                monsterMovement.SetAniSkill((int)skillType);
            }

            yield return new WaitForSeconds(2f);
        }
    }

    private Vector3 stringToVector3(string data)
    {
        Vector3 pos = Vector3.zero;

        string[] values = data.Split(',');

        pos.x = float.Parse(values[0]);
        pos.y = float.Parse(values[1]);
        pos.z = float.Parse(values[2]);

        return pos;
    }

    private void Hit(int index)
    {
        skillPos = stringToVector3(monsterInfoData.monsterSkillInfos[index].skillPoint);
        skillRange = stringToVector3(monsterInfoData.monsterSkillInfos[index].range);
        skillAtt = monsterInfoData.monsterSkillInfos[index].att;

        monsterRange.HitRange(skillPos, skillRange, skillAtt);
    }

    // 양손공격
    public void Att01()
    {
        // 데미지
        Hit((int)SkillType.att01);

        // 이펙트
    }

    // 내려찍기
    public void Att02()
    {
        Hit((int)SkillType.att02);
    }

    // 브레스
    public void Att05()
    {
        isBreath = true;
        StartCoroutine(Breath());
    }
    
    public IEnumerator Breath()
    {
        
        while (isBreath)
        {
            yield return new WaitForSeconds(0.5f);

            if (!isBreath)
            {
                break;
            }

            Hit((int)SkillType.att05);
        }
    }

    // 연속 아이스볼
    public void Combo01()
    {
        if (iceBallCount >= effects.iceBallObjs.Count)
        {
            iceBallCount = 0;
        }
        Debug.Log(iceBallCount);
        Transform tempT_s = null;
        Transform tempT_e = null;

        if (iceBallCount % 2 == 0)
        {
            tempT_s = effects.rightPoint_s;
            tempT_e = effects.rightPoint_e;
        }
        else
        {
            tempT_s = effects.leftPoint_s;
            tempT_e = effects.leftPoint_e;
        }

        effects.iceBallObjs[iceBallCount].transform.position = tempT_s.position;
        effects.iceBallObjs[iceBallCount].SetActive(true);

        BallControl ballControl = effects.iceBallObjs[iceBallCount].GetComponent<BallControl>();
        ballControl.SetBall(tempT_s, tempT_e.position, skillPos, skillRange, skillAtt);

        iceBallCount++;
    }

    public void StartBreath()
    {
        effects.breathObj.transform.SetParent(effects.mouthPoint);
        effects.breathObj.transform.localPosition = Vector3.zero;
        effects.breathObj.transform.localRotation = Quaternion.identity;
        effects.breathObj.SetActive(true);
    }

    public void EndBreath()
    {
        if (effects.breathObj.activeSelf)
        {
            isBreath = false;
            effects.breathObj.transform.SetParent(effects.effectHolder);
            effects.breathObj.SetActive(false);
        }
    }
}