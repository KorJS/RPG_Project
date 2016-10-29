using UnityEngine;
using System.Collections;

public class UIClick : MonoBehaviour
{
    private UIManager uiManager = null;
    private SkillData skillData = null;
    private PlayerSlotData playerSlotData = null;

    private UISlotInfo uiSlotInfo = null;
    private UISkillList uiSkillList = null;

    void Awake()
    {
        uiSkillList = GameObject.FindGameObjectWithTag("SkillW").GetComponent<UISkillList>();
        uiSlotInfo = GetComponent<UISlotInfo>();
    }
    
    void Start()
    {
        uiManager = UIManager.Instance;
        skillData = SkillData.Instance;
        playerSlotData = PlayerSlotData.Instance;
    }

    void OnClick()
    {
        // 마우스 왼클릭
        if (UICamera.currentKey == KeyCode.Mouse0)
        {
            SetSkillDescription();
        }

        // 마우스 우클릭
        if (UICamera.currentKey == KeyCode.Mouse1)
        {
            CheckSlotType();
        }
    }

    private void CheckSlotType()
    {
        switch (uiSlotInfo.slotType)
        {
            case TypeData.SlotType.캐릭터:
                {

                }
                break;

            case TypeData.SlotType.인벤토리:
                {

                }
                break;

            case TypeData.SlotType.단축키:
                {

                }
                break;

            case TypeData.SlotType.창고:
                {

                }
                break;

            case TypeData.SlotType.상점리스트:
                {

                }
                break;

            case TypeData.SlotType.구매:
                {

                }
                break;

            case TypeData.SlotType.판매:
                {

                }
                break;
        }
    }

    private void SetSkillDescription()
    {
        // 마우스 왼클릭이 아니면 리턴
        if (!(UICamera.currentKey == KeyCode.Mouse0))
        {
            return;
        }

        // 스킬 리스트 슬롯이 아니면 리턴
        if (!(uiSlotInfo.slotType == TypeData.SlotType.스킬리스트))
        {
            return;
        }

        int index = uiSlotInfo.slotInfo.skillIndex;

        SkillData.SkillInfo skillInfo = skillData.skillInfos[index];

        uiSkillList.SetDescription(skillInfo.iconName, skillInfo.name, skillInfo.description);
    }
}
