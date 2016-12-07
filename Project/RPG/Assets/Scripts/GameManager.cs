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

    private bool bPaused = false;  // 어플리케이션이 내려진 상태인지 아닌지의 스테이트를 저장하기 위한 변수
    private float showSplashTimout = 2f;
    private bool allowQuitting = false;

    public AudioClip inGameBGM = null;

    private const float fadeTime = 4f;
    public float fadeTimer = 0f;
    public bool isFade = false;

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

        isFade = true;
        fadeTimer = fadeTime;

        //SoundManager.Instance.PlayBackMusic(inGameBGM);

        CreatePlayer();
    }

    void Update()
    {
        Fade();
    }

    // 어플을 내렸으때 실행되는 함수
    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            bPaused = true;
            // todo : 어플리케이션을 내리는 순간에 처리할 행동들 /
        }
        else
        {
            if (bPaused)
            {
                bPaused = false;
                //todo : 내려놓은 어플리케이션을 다시 올리는 순간에 처리할 행동들 
            }
        }
    }

    // 어플을 종료 하는 순간 실행되는 함수
    void OnApplicationQuit()
    {
        StartCoroutine(DelayedQuit());

        if (!allowQuitting)
        {
            Application.CancelQuit();
        }
    }

    // 종료 딜레이 준다.
    IEnumerator DelayedQuit()
    {
        if (PlayerInfoData.Instance.infoData != null)
        {
            Network_PlayerInfo.Instance.RequestSavePlayerInfo();
        }

        yield return new WaitForSeconds(showSplashTimout);

        allowQuitting = true;

        Application.Quit();
    }

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

    public IEnumerator LogoutSavePlayerData()
    {
        yield return new WaitForSeconds(1f);

        Network_PlayerInfo.Instance.RequestSavePlayerInfo();

        gameManager.DataClear(false);

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("LoginScene");
    }

    public IEnumerator GameExitSavePlayerData()
    {
        yield return new WaitForSeconds(2f);

        Application.Quit();
    }
}
