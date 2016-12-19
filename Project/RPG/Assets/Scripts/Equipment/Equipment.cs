using UnityEngine;
using System.Collections;

public class Equipment : MonoBehaviour
{
    public enum EquipmentType
    {
        없음 = -1,
        검 = 0,
        마법책,
        지팡이,
        보조무기
    };

    public EquipmentType eqType = EquipmentType.없음; // 무기 타입

    [System.Serializable]
    public class LocationSettings
    {
        [Header("-Equip Location-")]
        public Transform    equipT;             // 장비 착용시 부모
        public Vector3      equipPosition;      // 장비 착용시 위치
        public Vector3      equipRotation;      // 장비 착용시 회전

        [Header("-Unequip Location-")]
        public Transform    unequipT;           // 장비 해제시 부모
        public Vector3      unequipPosition;    // 장비 해제시 위치
        public Vector3      unequipRotation;    // 장비 해제시 회전
    }

    [SerializeField]
    public LocationSettings locationSettings;

    public GameObject       slashEffect     = null; // 검기 이펙트

    public EquipmentHandler equipHandler    = null; // 장비 핸들러 스크립트
    private PlayerState     playerState     = null; // 주인공 상태

    private TypeData.MODE   currentMode     = TypeData.MODE.평화; // 평화, 전투 모드

    private bool            isEquipeed      = false;
    private bool            isChangMode     = false;
    private bool            isEffect        = false;

    void Awake()
    {
        if (eqType == EquipmentType.검)
        {
            slashEffect = this.transform.FindChild("Slash").gameObject;
        }
    }

    void Start()
    {
        playerState = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerState>();
        if (playerState == null)
        {
            Debug.Log("PlayerState Null");
        }
    }

    void Update()
    {
        // 모드가 바꼈고, 핸들러가 있고, 착용한거고
        if (isChangMode && equipHandler && isEquipeed)
        {
            SetLocation(); // 장비 위치 설정
        }

        if (eqType == EquipmentType.검 && playerState.currentState == TypeData.State.스킬)
        {
            slashEffect.SetActive(true);
            isEffect = true;
        }
        else if (isEffect && eqType == EquipmentType.검 && playerState.currentState != TypeData.State.스킬)
        {
            slashEffect.SetActive(false);
            isEffect = false;
        }
    }

    public void SetLocation()
    {
        //Debug.Log(eqType + " " + currentMode);
        // 타입에 맞게 부모지정
        switch (currentMode)
        {
            case TypeData.MODE.평화:
                {
                    StartCoroutine(this.Peace());
                }
                break;

            case TypeData.MODE.전투:
                {
                    StopCoroutine(this.Peace()); // 평화모드 코루틴 실행중이면 정지

                    this.transform.SetParent(locationSettings.equipT);

                    this.transform.localPosition = Vector3.zero;
                    this.transform.localEulerAngles = Vector3.zero;

                    // 지정한 위치 / 회전 값으로 설정
                    locationSettings.equipT.localPosition = locationSettings.equipPosition;
                    Quaternion equipRot = Quaternion.Euler(locationSettings.equipRotation);
                    locationSettings.equipT.localRotation = equipRot;
                }
                break;
        }

        isChangMode = false;
    }

    IEnumerator Peace()
    {
        if (isChangMode)
        {
            yield return new WaitForSeconds(0.8f);
        }

        this.transform.SetParent(locationSettings.unequipT);

        this.transform.localPosition = Vector3.zero;
        this.transform.localEulerAngles = Vector3.zero;

        // 지정한 위치 / 회전 값으로 설정
        locationSettings.unequipT.localPosition = locationSettings.unequipPosition;
        Quaternion unequipRot = Quaternion.Euler(locationSettings.unequipRotation);
        locationSettings.unequipT.localRotation = unequipRot;
    }

    public void CheckActive(TypeData.MODE nextMode)
    {
        // 착용하지 않았으면 리턴
        if (!isEquipeed)
        {
            return;
        }

        // 현재 상태와 다음 상태가 같으면 리턴
        if (currentMode == nextMode)
        {
            isChangMode = false;
            return;
        }

        currentMode = nextMode;
        isChangMode = true;
    }


    public void SetEquipeed(bool _isEquipeed)
    {
        isEquipeed = _isEquipeed;
    }

    public void SetEquipHandler(EquipmentHandler eh)
    {
        equipHandler = eh;
    }

    public void RemoveEquipment()
    {
        Destroy(this.gameObject);
    }
}
