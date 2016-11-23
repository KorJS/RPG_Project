using UnityEngine;
using System.Collections;

public class UIMonsterHpBar : MonoBehaviour
{
    [System.Serializable]
    public class HpBarSettings
    {
        public MonsterInfoData targetMonsterInfoData;
        public Transform targetT; // 몬스터
        public UILabel targetName;
        public UIProgressBar targetHpBar;
        public UILabel typeLabel;
    }

    [SerializeField]
    public HpBarSettings hpBarSettings;

    void Awake()
    {
        hpBarSettings.targetName = transform.FindChild("Name").GetComponent<UILabel>();
        hpBarSettings.targetHpBar = transform.FindChild("Hp Bar").GetComponent<UIProgressBar>();

        if (transform.FindChild("Type") != null)
        {
            hpBarSettings.typeLabel = transform.FindChild("Type").GetComponent<UILabel>();
        }
    }

    void Update()
    {
        if (GameManager.Instance.currentGameState == TypeData.GameState.종료)
        {
            return;
        }

        if (hpBarSettings.targetT == null)
        {
            return;
        }

        SetHpBar();
    }

    private void SetHpBar()
    {
        float hp = hpBarSettings.targetMonsterInfoData.currentHP / hpBarSettings.targetMonsterInfoData.monsterInfo.hp;
        hpBarSettings.targetHpBar.value = hp;
    }

    public void SetTarget(Transform targetT)
    {
        hpBarSettings.targetT = targetT;
        hpBarSettings.targetMonsterInfoData = targetT.GetComponent<MonsterInfoData>();
        hpBarSettings.targetName.text = hpBarSettings.targetMonsterInfoData.monsterInfo.name;

        switch ((TypeData.MonsterType)hpBarSettings.targetMonsterInfoData.monsterInfo.monsterType)
        {
            case TypeData.MonsterType.일반:
                {
                    hpBarSettings.typeLabel.text = "일반";
                }
                break;

            case TypeData.MonsterType.정예:
                {
                    hpBarSettings.typeLabel.text = "정예";
                }
                break;
        }
    }
}
