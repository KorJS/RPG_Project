using UnityEngine;
using System.Collections;

public class UISlotTooltip : MonoBehaviour
{
    private UISlotInfo uiSlotInfo = null;

    [System.Serializable]
    public class ToolTipSettings
    {
        public UITexture uiIcon;        // 아이콘
        public UILabel uiIsEquip;       // 사용가능 여부
        public UILabel uiName;          // 이름
        public UILabel uiLevel;         // 사용가능 레벨
        public UILabel uiPlayerType;    // 사용가능 직업
        public UILabel uiType;          // 타입
        public UILabel uiStat;          // 스텟정보
        public UILabel uiBuyGold;       // 구매금액
        public UILabel uiSellGold;      // 판매금액
        public GameObject buyObj;       // 구매 오브젝트
        public GameObject sellObj;      // 판매 오브젝트

        public float activeTime = 3f;
        public bool isActive = false;
    }

    [SerializeField]
    public ToolTipSettings toolTipSettings;

    private float activeTimer = 0f;

    void OnDisable()
    {
        toolTipSettings.isActive = false;
        activeTimer = 0f;
    }

    void Update()
    {
        ActiveTime();
    }

    // 구매 / 판매 / 단축 / 스킬리스트 제외
    public void SetSlotInfo(UISlotInfo _uiSlotInfo)
    {
        uiSlotInfo = _uiSlotInfo;

        toolTipSettings.isActive = true;
        activeTimer = 0f;

        CheckType();
    }

    private void ActiveTime()
    {
        // 3초 뒤에 사짐 / UI 모드 아닐때 사라짐
        if (!toolTipSettings.isActive)
        {
            return;
        }

        activeTimer += Time.deltaTime;

        if (!UIManager.Instance.isUIMode)
        {
            gameObject.SetActive(false);
            toolTipSettings.isActive = false;
            activeTimer = 0f;
            return;
        }

        if (activeTimer >= toolTipSettings.activeTime)
        {
            gameObject.SetActive(false);
            toolTipSettings.isActive = false;
            activeTimer = 0f;
        }
    }

    private void CheckType()
    {
        toolTipSettings.uiIcon.mainTexture = uiSlotInfo.slotSettings.uiIcon.mainTexture;
        toolTipSettings.uiName.text = uiSlotInfo.slotInfo.name;

        // 스킬
        if (uiSlotInfo.slotInfo.skillIndex != -1)
        {
            toolTipSettings.buyObj.SetActive(false);
            toolTipSettings.sellObj.SetActive(false);


            SetSkill();
            toolTipSettings.uiPlayerType.text = CheckPlayerType((TypeData.PlayerType)PlayerInfoData.Instance.infoData.playerType);
        }
        // 아이템
        else
        {
            Debug.Log("?");
            toolTipSettings.buyObj.gameObject.SetActive(true);
            toolTipSettings.sellObj.gameObject.SetActive(true);

            SetItem();
        }
    }

    // 스킬 스텟 설정
    private void SetSkill()
    {
        SkillData.SkillInfo skillInfo = SkillData.Instance.skillInfos[uiSlotInfo.slotInfo.skillIndex];

        CheckEquipLevel(skillInfo.level);

        float att = skillInfo.attack;
        float coolTime = skillInfo.coolTime;
        float mp = skillInfo.mp;
        float angle = skillInfo.angle;
        float distance = skillInfo.distance;

        toolTipSettings.uiStat.text = "[00FF00FF]공격력 : " + att + "\n[00FF00FF]쿨타임 : " + coolTime + "\n[00FF00FF]마력 : " + mp + "\n[00FF00FF]공격범위 : " + distance + "[00FF00FF]m";
    }

    // 아이템 정보 설정
    private void SetItem()
    {
        switch ((TypeData.ItemType)uiSlotInfo.slotInfo.itemType)
        {
            case TypeData.ItemType.장비:
                {
                    ItemData.EquipmentInfo equipInfo = ItemData.Instance.equipmentInfos[uiSlotInfo.slotInfo.itemIndex];
                    toolTipSettings.uiType.text = ((TypeData.EquipmentType)equipInfo.equipmentType).ToString();
                    CheckEquipLevel(equipInfo.level);

                    int att = equipInfo.att;
                    int def = equipInfo.def;
                    int hp = equipInfo.hp;
                    int mp = equipInfo.mp;

                    toolTipSettings.uiPlayerType.text = CheckPlayerType((TypeData.PlayerType)equipInfo.playerType);
                    toolTipSettings.uiStat.text = "[00FF00FF]공격력 : " + att + "\n[00FF00FF]방어력 : " + def + "\n[00FF00FF]체력 : " + hp + "\n[00FF00FF]마력 : " + mp;
                    toolTipSettings.uiBuyGold.text = equipInfo.buyGold.ToString();
                    toolTipSettings.uiSellGold.text = equipInfo.sellGold.ToString();
                }
                break;

            case TypeData.ItemType.소모품:
                {
                    ItemData.CusomableInfo cuInfo = ItemData.Instance.cusomableInfos[uiSlotInfo.slotInfo.itemIndex];
                    toolTipSettings.uiType.text = TypeData.ItemType.소모품.ToString();
                    CheckEquipLevel(cuInfo.level);

                    float coolTime = cuInfo.coolTime;
                    int att = cuInfo.att;
                    int def = cuInfo.def;
                    int hp = cuInfo.hp;
                    int mp = cuInfo.mp;

                    toolTipSettings.uiPlayerType.text = CheckPlayerType(TypeData.PlayerType.없음);
                    toolTipSettings.uiStat.text = "[00FF00FF]쿨타임 : " + coolTime + "\n[00FF00FF]공격력 : " + att + "\n[00FF00FF]방어력 : " + def + "\n[00FF00FF]체력 : " + hp + "\n[00FF00FF]마력 : " + mp;
                    toolTipSettings.uiBuyGold.text = cuInfo.buyGold.ToString();
                    toolTipSettings.uiSellGold.text = cuInfo.sellGold.ToString();
                }
                break;

            case TypeData.ItemType.퀘스트템:
                {
                    ItemData.QuestItemInfo cuInfo = ItemData.Instance.questItemInfos[uiSlotInfo.slotInfo.itemIndex];

                    toolTipSettings.uiSellGold.text = cuInfo.sellGold.ToString();
                }
                break;
        }
    }

    // 착용레벨 및 사용 가능여부 설정
    private void CheckEquipLevel(int level)
    {
        toolTipSettings.uiLevel.text = level.ToString();

        if (PlayerInfoData.Instance.infoData.level >= int.Parse(toolTipSettings.uiLevel.text))
        {
            toolTipSettings.uiIsEquip.text = "[FFFFFFFF]사용가능";
        }
        else
        {
            toolTipSettings.uiIsEquip.text = "[FF0000FF]사용불가능";
        }
    }

    // 사용가능한 직업 설정
    private string CheckPlayerType(TypeData.PlayerType playerType)
    {
        switch (playerType)
        {
            case TypeData.PlayerType.없음:
                return "공용";

            case TypeData.PlayerType.기사:
                return TypeData.PlayerType.기사.ToString();

            case TypeData.PlayerType.마법사:
                return TypeData.PlayerType.마법사.ToString();

            case TypeData.PlayerType.사제:
                return TypeData.PlayerType.사제.ToString();
        }

        return null;
    }
}
