﻿using UnityEngine;
using System.Collections;

public class EffectSetting : MonoBehaviour
{
    private CameraControl cameraCtrl = null;

    public enum EquipType
    {
        없음 = -1,
        장착 = 0,
        비장착
    };

    public EquipType equipType = EquipType.없음;

    public enum ContinueType
    {
        없음 = -1,
        지속 = 0,
        비지속
    };

    public ContinueType continueType = ContinueType.없음;

    public enum AniExist
    {
        없음 = -1,
        있음 = 0,
    }

    public AniExist aniExist = AniExist.없음;

    [System.Serializable]
    public class InfoSettings
    {
        public Transform    effectHoler;    // 이펙트 부모
        public Vector3      effectPosition; // 이펙트 위치
        public float        activeTime;     // 활성화 타임
        public float        shotTime;       // 발사 타임

        public float        skillAngle;     // 스킬 각도
        public float        skillDis;       // 스킬 거리
        public float        skillAtt;       // 스킬 공격력
    }

    [SerializeField]
    public InfoSettings infoSettings;

    public ParticleSystem   particleSys = null;
    public Animator         animator    = null;

    public float            debugTimer  = 0;    // 디버그 타이머(활성화타임 알기위해)

    void Awake()
    {
        particleSys = GetComponent<ParticleSystem>();

        cameraCtrl = GameObject.FindGameObjectWithTag("CameraCtrl").GetComponent<CameraControl>();
    }

    // 활성화 될때 이펙트를 지정된 장소에 배치
    void OnEnable()
    {
        debugTimer = 0f;

        SetInfo();
        CheckType();

        if (continueType == ContinueType.비지속)
        {
            StartCoroutine(SetActiveAndHolder());
        }

        if (aniExist == AniExist.있음)
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            StartCoroutine(SetShotTime()); // 애니매이터 붙어있는 이펙트(메태오) 트리거 작동
        }
    }

    void Update()
    {
        // Debug
        debugTimer += Time.deltaTime;
    }

    private void SetInfo()
    {
        // 위치 설정
        transform.localPosition = infoSettings.effectPosition;
    }

    public void SetSkillInfo(float skillAngle, float skillDis, float skillAtt)
    {
        infoSettings.skillAngle = skillAngle;
        infoSettings.skillDis = skillDis;
        infoSettings.skillAtt = skillAtt;
    }

    private void CheckType()
    {
        if (equipType != EquipType.비장착)
        {
            return;
        }

        // 비창작 이펙트인 경우 그자리 고정이므로 부모로부터 독립.
        transform.SetParent(null);
    }

    // 지정된 활성화시간 지난후에 비활성화.
    IEnumerator SetActiveAndHolder()
    {
        yield return new WaitForSeconds(infoSettings.activeTime);

        if (equipType == EquipType.비장착)
        {
            transform.SetParent(infoSettings.effectHoler);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        gameObject.SetActive(false);
    }


    IEnumerator SetShotTime()
    {
        yield return new WaitForSeconds(infoSettings.shotTime);

        animator.SetTrigger("isShot");
    }

    // 운석낙하(각 운석마다 데미지 - 애니매이션에서 이벤트함수로)
    public void Attack()
    {
        if (equipType != EquipType.비장착 && continueType != ContinueType.비지속)
        {
            return;
        }

        // 비장착이고 지속인 이펙트

        Collider[] targets = Physics.OverlapSphere(transform.position, infoSettings.skillDis);

        foreach (Collider target in targets)
        {
            if (target.gameObject.layer == LayerMask.NameToLayer("Monster"))
            {
                // 범위 안에 있으면 Hit
                //Debug.Log(target.name);
                MonsterMovement mob = target.GetComponent<MonsterMovement>();

                mob.SetDamage(gameObject.transform, -infoSettings.skillAtt);
            }
        }
    }

    // 얼음폭풍 같은 도트데미지.(애니매이션에서 이벤트함수로)
    public IEnumerator DotAttack()
    {
        if (equipType != EquipType.비장착 && continueType != ContinueType.비지속)
        {
            yield return null;  
        }

        float count = 0f;

        while (count <= 3)
        {
            Attack();

            yield return new WaitForSeconds(1f);

            count++;
        }
    }

    public void Shake()
    {
        cameraCtrl.Shake(0.7f, 0.7f, 0.7f);
    }
}
