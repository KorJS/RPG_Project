using UnityEngine;
using System.Collections;

public class EffectSetting : MonoBehaviour
{
    public enum Type
    {
        없음 = -1,
        장착 = 0,
        비장착
    };

    public Type type = Type.없음;

    [System.Serializable]
    public class InfoSettings
    {
        public Transform effectHoler;
        public Vector3 effectPosition;
        public float activeTime;
    }

    [SerializeField]
    public InfoSettings infoSettings;

    private ParticleSystem particleSys = null;

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
        StartCoroutine(SetActiveAndHolder());
    }

    void Update()
    {
        // Debug
        debugTimer += Time.deltaTime;

        //SetActiveAndHolder();
    }

    private void SetInfo()
    {
        // 부모 설정
        infoSettings.effectHoler = this.transform.parent;

        // 위치 설정
        transform.localPosition = infoSettings.effectPosition;
    }

    private void CheckType()
    {
        if (type != Type.비장착)
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

        // 이펙트가 끝나면 다시 부모로. 
        if (particleSys.isStopped)
        {
            if (type == Type.비장착)
            {
                transform.SetParent(infoSettings.effectHoler);
            }
            gameObject.SetActive(false);
        }
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
