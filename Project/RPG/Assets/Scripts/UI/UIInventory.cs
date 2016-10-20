using UnityEngine;
using System.Collections;

public class UIInventory : MonoBehaviour
{
    public GameObject tempDraggingPanel = null; // 드래그중인것 복사한거
    public UITexture tempIcon = null; // 드래그중인 Icon

    void Awake()
    {

    }

    void OnEnable()
    {

    }

    void OnDisable()
    {
        DisableDragIiem();
    }

    // UI 해제되면 드래그중인 아이템 처리
    private void DisableDragIiem()
    {
        if (tempDraggingPanel == null)
        {
            return;
        }

        tempIcon.alpha = 1f;
        Destroy(tempDraggingPanel); // UI 모드 해제되면 드래그 중인거 제거
    }
}