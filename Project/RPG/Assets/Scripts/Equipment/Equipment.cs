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
        방패
    };

    public EquipmentType eqType = EquipmentType.없음;

    [System.Serializable]
    public class LocationSettings
    {
        [Header("-Equip Location-")]
        public Transform equipT;
        public Vector3 equipPosition;
        public Vector3 equipRotation;

        [Header("-Unequip Location-")]
        public Transform unequipT;
        public Vector3 unequipPosition;
        public Vector3 unequipRotation;
    }

    [SerializeField]
    public LocationSettings locationSettings;

    // TODO : 이팩트. 등등 
    public GameObject slashEffect = null;

    public EquipmentHandler equipHandler = null;
    private PlayerState playerState = null;

    private TypeData.MODE currentMode = TypeData.MODE.평화;

    private bool isEquipeed = false;
    private bool isChangMode = false;
    private bool isEffect = false;

    void Awake()
    {
        if (eqType == EquipmentType.검)
        {
            slashEffect = this.transform.FindChild("Slash").gameObject;
        }

        if (GameObject.FindGameObjectWithTag("Player"))
        {
            playerState = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerState>();
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
            yield return new WaitForSeconds(1f);
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
        // TODO : Destory();
        Destroy(this.gameObject);
    }
}
