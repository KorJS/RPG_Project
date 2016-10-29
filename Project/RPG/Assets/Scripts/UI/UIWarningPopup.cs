using UnityEngine;
using System.Collections;

public class UIWarningPopup : MonoBehaviour
{
    private UIManager uiManager = null;

    private UILabel messageT = null;

    void Awake()
    {
        messageT = transform.FindChild("Message").GetComponent<UILabel>();
    }

    void Start()
    {
        uiManager = UIManager.Instance;
    }

    public void SetMessage(string _message)
    {
        messageT.text = _message;
    }

    public void ClosePopup()
    {
        messageT.text = null;
        gameObject.SetActive(false);
    }
}
