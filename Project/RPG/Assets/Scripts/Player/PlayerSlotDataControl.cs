using UnityEngine;
using System.Collections;

public class PlayerSlotDataControl : MonoBehaviour
{
    private PlayerSlotData playerSlotData = null;

    void Awake()
    {
        playerSlotData = PlayerSlotData.Instance;
    }


}