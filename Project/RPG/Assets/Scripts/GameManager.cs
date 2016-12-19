using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private static GameManager gameManager = null;
    public static GameManager Instance
    {
        get
        {
            if (gameManager == null)
            {
                Debug.Log("GameManager Script Null");
            }

            return gameManager;
        }
    }

    public TypeData.GameState currentGameState = TypeData.GameState.없음;

    private float       delayQuitTime       = 2f;       // 종료 딜레이 타임
    private bool        isQuit              = false;    // 종료인지 여부

    public  AudioClip   inGameBGM           = null;     // 인트로 사운드

    private const float fadeTime            = 4f;       // 페이드인 타임
    public float        fadeTimer           = 0f;       // 페이드인 타이머
    public bool         isFade              = false;    // 페이드인 인지 여부

    void Awake()
    {
        if (gameManager == null)
        {
            gameManager = this;
        }
        else if (gameManager != this)
        {
            Destroy(gameObject);
        }
          
        DontDestroyOnLoad(this);

        isFade      = true;
        fadeTimer   = fadeTime;

        SoundManager.Instance.PlayBackMusic(inGameBGM);

        CreatePlayer();
    }

    void Update()
    {
        Fade();
    }

    // 어플을 종료 하는 순간 실행되는 함수
    void OnApplicationQuit()
    {
        StartCoroutine(DelayedQuit());

        if (!isQuit)
        {
            Application.CancelQuit();
        }
    }

    // 종료 딜레이 준다.
    IEnumerator DelayedQuit()
    {
        // 종료하기 전에 주인공 정보 저장
        if (PlayerInfoData.Instance.infoData != null)
        {
            Network_PlayerInfo.Instance.RequestSavePlayerInfo();
        }

        yield return new WaitForSeconds(delayQuitTime);

        isQuit = true;

        Application.Quit();
    }

    // 페이드 인
    public void Fade()
    {
        if (!isFade)
        {
            return;
        }

        UIManager.Instance.windowSettings.fadePanel.alpha = fadeTimer / fadeTime;

        fadeTimer -= Time.deltaTime;

        if (fadeTimer <= 0f)
        {
            currentGameState = TypeData.GameState.시작;

            isFade = false;
            fadeTimer = fadeTime;
        }
    }

    // 주인공 프리펩 생성
    public void CreatePlayer()
    {
        TypeData.PlayerType playerType = (TypeData.PlayerType)PlayerInfoData.Instance.infoData.playerType;

        GameObject playerObj = null;

        switch (playerType)
        {
            case TypeData.PlayerType.기사:
                {
                    playerObj = Instantiate(Resources.Load("Player/Warrior")) as GameObject;
                    playerObj.name = "Warrior";
                }
                break;

            case TypeData.PlayerType.마법사:
                {
                    playerObj = Instantiate(Resources.Load("Player/Magician")) as GameObject;
                    playerObj.name = "Magician";
                }
                break;

            case TypeData.PlayerType.사제:
                {
                    playerObj = Instantiate(Resources.Load("Player/Preist")) as GameObject;
                    playerObj.name = "Preis";
                }
                break;
        }
    }

    public void DataClear(bool isAreaData)
    {
        if (isAreaData)
        {
            MonsterData.Instance.DataClear();
            StoreItemListData.Instance.DataClear();
            return;
        }

        ItemData.Instance.DataClear();
        LevelData.Instance.DataClear();
        SkillData.Instance.DataClear();
        PlayerInfoData.Instance.DataClear();
        PlayerSlotData.Instance.DataClear();
        PlayerSkillData.Instance.DataClear();
        UIManager.Instance.DataClear();
        MonsterData.Instance.DataClear();
        StoreItemListData.Instance.DataClear();
    }

    // 로그아웃시 주인공 정보 저장
    public IEnumerator LogoutSavePlayerData()
    {
        yield return new WaitForSeconds(1f);

        Network_PlayerInfo.Instance.RequestSavePlayerInfo();

        gameManager.DataClear(false);

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("LoginScene");
    }

    // 종료
    public IEnumerator GameExitSavePlayerData()
    {
        yield return new WaitForSeconds(2f);

        Application.Quit();
    }
}
