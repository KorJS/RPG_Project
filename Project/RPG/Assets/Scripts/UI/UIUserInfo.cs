using UnityEngine;
using System.Collections;

public class UIUserInfo : MonoBehaviour
{
    private PlayerInfoData playerInfoData = null;

    [System.Serializable]
    public class UserInfoSettings
    {
        public UILabel       levelNick; // 레벨,닉네임

        public UIProgressBar hpBar;     // 체력바
        public UILabel       hpCurrent; // 현제 체력
        public UILabel       hpMAX;     // MAX 체력

        public UIProgressBar mpBar;     // 마력바
        public UILabel       mpCurrent; // 현재 마력
        public UILabel       mpMAX;     // MAX 마력

        public UIProgressBar expBar;    // 경험치 바
        public UILabel       expCurrent;// 현재 경험치
        public UILabel       expMAX;    // MAX 경험치
    }

    [SerializeField]
    public UserInfoSettings userInfoSettings;

    private string  levelNick   = null; // 레벨, 닉네임

    void Awake()
    {
        playerInfoData = PlayerInfoData.Instance;
    }

    void Update()
    {
        if (GameManager.Instance.currentGameState != TypeData.GameState.시작)
        {
            return;
        }

        LevelUpdate();
        HpBarUpdate();
        MpBarUpdate();
        ExpBarUpdate();
    }

    // 레벨 라벨 갱신
    public void LevelUpdate()
    {
        levelNick = "레벨 " + playerInfoData.infoData.level + " " + playerInfoData.infoData.nick;
        userInfoSettings.levelNick.text = levelNick;
    }

    // 체력바 갱신
    public void HpBarUpdate()
    {
        float currentHp = playerInfoData.infoData.currentHp;
        float totalMaxHp = playerInfoData.totalMaxHp;

        userInfoSettings.hpBar.value = currentHp / totalMaxHp;
        userInfoSettings.hpCurrent.text = ((int)currentHp).ToString();
        userInfoSettings.hpMAX.text = ((int)totalMaxHp).ToString();
    }

    // 마력바 갱신
    public void MpBarUpdate()
    {
        float currentMp = playerInfoData.infoData.currentMp;
        float totalMaxMp = playerInfoData.totalMaxMp;

        userInfoSettings.mpBar.value = currentMp / totalMaxMp;
        userInfoSettings.mpCurrent.text = ((int)currentMp).ToString();
        userInfoSettings.mpMAX.text = ((int)totalMaxMp).ToString();
    }

    // 경험치바 갱신
    public void ExpBarUpdate()
    {
        int currentLevel = playerInfoData.infoData.level;
        // 만렙이면 리턴
        if (currentLevel == LevelData.Instance.MAX_Level)
        {
            return;
        }
        
        float currentExp = playerInfoData.infoData.exp;
        // 현재인덱스(다음렙 경험치) = 현재레벨;
        float maxExp = LevelData.Instance.levelInfos[currentLevel].exp;

        userInfoSettings.expBar.value = currentExp / maxExp;
        userInfoSettings.expCurrent.text = ((int)currentExp).ToString();
        userInfoSettings.expMAX.text = ((int)maxExp).ToString();
    }
}