﻿using UnityEngine;
using System.Collections;

public class UIWarningPopup : MonoBehaviour
{
    private UILabel messageT = null;

    void Awake()
    {
        messageT = transform.FindChild("Message").GetComponent<UILabel>();
    }

    // 경고 메시지
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
