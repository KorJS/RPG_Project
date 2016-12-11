using UnityEngine;
using System.Collections;

public class UIUserInfo : MonoBehaviour
{
    private PlayerInfoData playerInfoData = null;

    [System.Serializable]
    public class UserInfoSettings
    {
        public UILabel levelNick;

        public UIProgressBar hpBar;
        public UILabel hpCurrent;
        public UILabel hpMAX;

        public UIProgressBar mpBar;
        public UILabel mpCurrent;
        public UILabel mpMAX;

        public UIProgressBar expBar;
        public UILabel expCurrent;
        public UILabel expMAX;
    }

    [SerializeField]
    public UserInfoSettings userInfoSettings;

    private string levelNick = null;
    private float hp = 0;
    private float mp = 0;

    void Awake()
    {
        playerInfoData = PlayerInfoData.Instance;
    }

    // TODO : 레벨은 변경될때 바꾸게 하자
    void Update()
    {
        LevelUpdate();
        HpBarUpdate();
        MpBarUpdate();
        ExpBarUpdate();
    }

    public void LevelUpdate()
    {
        levelNick = "레벨 " + playerInfoData.infoData.level + " " + playerInfoData.infoData.nick;
        userInfoSettings.levelNick.text = levelNick;
    }

    public void HpBarUpdate()
    {
        float currentHp = playerInfoData.infoData.currentHp;
        float totalMaxHp = playerInfoData.totalMaxHp;

        userInfoSettings.hpBar.value = currentHp / totalMaxHp;
        userInfoSettings.hpCurrent.text = ((int)currentHp).ToString();
        userInfoSettings.hpMAX.text = ((int)totalMaxHp).ToString();
    }

    public void MpBarUpdate()
    {
        float currentMp = playerInfoData.infoData.currentMp;
        float totalMaxMp = playerInfoData.totalMaxMp;

        userInfoSettings.mpBar.value = currentMp / totalMaxMp;
        userInfoSettings.mpCurrent.text = ((int)currentMp).ToString();
        userInfoSettings.mpMAX.text = ((int)totalMaxMp).ToString();
    }

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