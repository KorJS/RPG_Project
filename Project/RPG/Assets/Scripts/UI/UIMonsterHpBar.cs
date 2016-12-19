using UnityEngine;
using System.Collections;

public class UIMonsterHpBar : MonoBehaviour
{
    [System.Serializable]
    public class HpBarSettings
    {
        public MonsterInfoData  targetMonsterInfoData;  // 몬스터 정보
        public Transform        targetT;                // 몬스터
        public UILabel          targetName;             // 몬스터 이름
        public UIProgressBar    targetHpBar;            // 몬스터 체력바
        public UILabel          typeLabel;              // 몬스터 타입
    }

    [SerializeField]
    public HpBarSettings hpBarSettings;

    void Awake()
    {
        hpBarSettings.targetName  = transform.FindChild("Name").GetComponent<UILabel>();
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

    // 몬스터 체력바 게이지 설정
    private void SetHpBar()
    {
        float hp = hpBarSettings.targetMonsterInfoData.currentHP / hpBarSettings.targetMonsterInfoData.monsterInfo.hp;
        hpBarSettings.targetHpBar.value = hp;
    }

    // 타겟 설정
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
