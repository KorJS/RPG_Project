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
        levelNick = "레벨 " + playerInfoData.infoData.level + " " + playerInfoData.infoData.nick;
        userInfoSettings.levelNick.text = levelNick;

        hp = playerInfoData.infoData.currentHp / playerInfoData.totalMaxHp;
        userInfoSettings.hpBar.value = hp;

        userInfoSettings.hpCurrent.text = playerInfoData.infoData.currentHp.ToString();
        userInfoSettings.hpMAX.text = playerInfoData.totalMaxHp.ToString();

        mp = playerInfoData.infoData.currentMp / playerInfoData.totalMaxMp;
        userInfoSettings.mpBar.value = mp;

        userInfoSettings.mpCurrent.text = playerInfoData.infoData.currentMp.ToString();
        userInfoSettings.mpMAX.text = playerInfoData.totalMaxMp.ToString();
    }
}