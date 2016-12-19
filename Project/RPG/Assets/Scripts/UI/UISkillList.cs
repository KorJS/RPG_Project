using UnityEngine;
using System.Collections;

public class UISkillList : MonoBehaviour
{
    [System.Serializable]
    public class SkillListSettings
    {
        public UIPanel skillListPanel;
        public UITexture skillIcon;
        public UILabel skillName;
        public UILabel skillDescription;
    }

    [SerializeField]
    public SkillListSettings skillListSettings;

    void Awake()
    {
        skillListSettings.skillListPanel = GetComponent<UIPanel>();
    }

    // 활성화되면 첫번째 슬롯 스킬정보 표시
    void OnEnable()
    {
        SkillData.SkillInfo skillInfo = SkillData.Instance.skillInfos[0];

        skillListSettings.skillIcon.mainTexture = Resources.Load("Icon/" + skillInfo.iconName) as Texture2D;
        skillListSettings.skillName.text = skillInfo.name;
        skillListSettings.skillDescription.text = skillInfo.description;
    }

    // 스킬 설명 설정
    public void SetDescription(string iconName, string skillName, string description)
    {
        skillListSettings.skillIcon.mainTexture = Resources.Load("Icon/" + iconName) as Texture2D;
        skillListSettings.skillName.text = skillName;
        skillListSettings.skillDescription.text = description;
    }

    public void CloseWindows()
    {
        skillListSettings.skillListPanel.alpha = 0f;
    }
}