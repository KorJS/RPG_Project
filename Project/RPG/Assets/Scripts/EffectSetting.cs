using UnityEngine;
using System.Collections;

// 비장착일 경우의 실행하는 스크립트
public class EffectSetting : MonoBehaviour
{
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
        public Transform effectHoler;
        public Vector3 effectPosition;
        public float activeTime;
        public float shotTime;

        // TODO : 비창작/지속인 스킬 - 다음프로젝트때는 잘생각하고 하자
        public float skillAngle;
        public float skillDis;
        public float skillAtt;
    }

    [SerializeField]
    public InfoSettings infoSettings;

    public ParticleSystem particleSys = null;
    public Animator animator = null;

    public float debugTimer = 0;

    void Awake()
    {
        particleSys = GetComponent<ParticleSystem>();
    }
    
    // 활성화 될때 이펙트를 지정된 장소에 배치
    void OnEnable()
    {
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

        //SetActiveAndHolder();

        CheckEffectRange(); // 지속, 비장착 스킬 (메태오, 얼음폭풍) 직접 범위 내에 적 검사후 데미지 처리
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

    private void CheckEffectRange()
    {
        if (equipType != EquipType.비장착 && continueType != ContinueType.지속)
        {
            return;
        }

        // 비장착이고 지속인 이펙트
    }

    //// TODO : 여러개의 파티클이 있을경우 부모가 끝났고 자식은 아직 실행중일 경우. 버그 발생할듯?..  각 활성화 타이머 지정
    //private void SetActiveAndHolder()
    //{
    //    if (particleSys.isPlaying)
    //    {
    //        return;
    //    }

    //    // 이펙트가 끝나면 다시 부모로. 
    //    if (particleSys.isStopped)
    //    {
    //        if (type == Type.비장착)
    //        {
    //            transform.SetParent(infoSettings.effectHoler);
    //        }
    //        gameObject.SetActive(false);
    //    }
    //}
}
