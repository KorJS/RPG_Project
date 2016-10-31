using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipmentHandler : MonoBehaviour
{
    [System.Serializable]
    public class UserSettings
    {
        public Transform weaponEquipSpot;
        public Transform weaponUnequipSpot;
        public Transform subWeaponEquipSpot;
        public Transform subWeaponUnequipSpot;
    }

    [SerializeField]
    public UserSettings userSettings;

    public GameObject   weaponObj       = null;
    public Equipment    weapon          = null;

    public GameObject   subWeaponObj    = null;
    public Equipment    subWeapon       = null;

    private PlayerState playerState     = null;

    void Awake()
    {
        playerState = GetComponent<PlayerState>();
    }

    void Update()
    {
        // 스킬을 사용할때 장비 전투모드 위치로.
        if (playerState.currentState == TypeData.State.스킬)
        {
            if (weapon != null)
            {
                weapon.SetLocation();
            }

            if (subWeapon != null)
            {
                subWeapon.SetLocation();
            }
        }
    }

    // 장착하면 매개변수로 그 오브젝트를 받아봐서 생성하기(Instantiate)
    // 나중에 매개변수 변경하자
    public void SetWeapon(GameObject _weaponObj, bool _isWeapon)
    {
        // TODO : 전투모드중이면 장착/ 변경 불가능하게 하기

        // TODO : 프리펩으로 관리해서 생성과 삭제를?

        // 기존 착용장비가 존재하고 
        // 착용할 장비가 기존 장비와 다르면
        // 제거
        if (weaponObj && (weaponObj != _weaponObj))
        {
            // 각 장비 스크립트에서 제거 하기
            weapon.RemoveEquipment();
            weaponObj = null;
            weapon = null;
        }

        // 제거후 착용한 장비가 있다면
        if (_weaponObj != null)
        {
            weaponObj = _weaponObj;
            weapon = weaponObj.GetComponent<Equipment>();

            // 장비 부모 설정
            weapon.locationSettings.equipT = userSettings.weaponEquipSpot;
            weapon.locationSettings.unequipT = userSettings.weaponUnequipSpot;

            weapon.SetEquipeed(_isWeapon);
            weapon.SetEquipHandler(this);
            weapon.CheckActive(playerState.currentMode); // 장비에 모드 설정
            weapon.SetLocation(); // 장비 위치 설정
        }
    }

    public void SetSubWeapon(GameObject _subWeaponObj, bool _isSubWeapon)
    {
        // 기존 착용장비가 존재하고 
        // 착용할 장비가 기존 장비와 다르면
        // 제거
        if (subWeaponObj && (subWeaponObj != _subWeaponObj))
        {
            subWeapon.RemoveEquipment();
            subWeaponObj = null;
            subWeapon = null;
        }

        // 제거후 착용한 장비가 있다면
        if (_subWeaponObj != null)
        {
            subWeaponObj = _subWeaponObj;
            subWeapon = subWeaponObj.GetComponent<Equipment>();

            // 장비 부모 설정
            subWeapon.locationSettings.equipT = userSettings.subWeaponEquipSpot;
            subWeapon.locationSettings.unequipT = userSettings.subWeaponUnequipSpot;

            subWeapon.SetEquipeed(_isSubWeapon);
            subWeapon.SetEquipHandler(this);
            subWeapon.CheckActive(playerState.currentMode); // 장비에 모드 설정
            subWeapon.SetLocation(); // 장비 위치 설정
        }
    }

    // 평화/전투 모드가 변할때 호출되는 함수
    public void ChangeMode(TypeData.MODE mode)
    {
        if (weapon)
        {
            weapon.CheckActive(mode);
        }

        if (subWeapon)
        {
            subWeapon.CheckActive(mode);
        }
    }
}