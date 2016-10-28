using UnityEngine;
using System.Collections;

public class UIWindows : MonoBehaviour
{
    public void CloseWindow()
    {
        gameObject.SetActive(false);
    }
}