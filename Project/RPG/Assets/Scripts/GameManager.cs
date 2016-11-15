using UnityEngine;
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

    void Awake()
    {
        gameManager = this;
        DontDestroyOnLoad(this);

        CreatePlayer();
    }

    private void CreatePlayer()
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
}
