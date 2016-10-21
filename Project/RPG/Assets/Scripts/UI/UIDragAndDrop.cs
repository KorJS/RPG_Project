using UnityEngine;
using System.Collections;

public class UIDragAndDrop : MonoBehaviour
{
    private UIManager uiManager = null;
    private PlayerSlotData playerSlotData = null;
    public UISlotInfo uiSlotInfo = null;

    private UIRoot root = null;
    private Transform parent = null;
    private GameObject draggedObject = null;     // 드래그중인 icon
    private GameObject tempDraggingPanel = null; // 드래그중인 panel

    private int dropSlotIndex = 0;               // 드롭하는 위치

    private bool isPressed = false;              // 마우스를 클릭해제했는지
    private bool isDragging = false;             // 드래그중인지

    void Awake()
    {
        uiManager = UIManager.Instance;
        playerSlotData = PlayerSlotData.Instance;

        uiSlotInfo = this.GetComponent<UISlotInfo>();
        parent = this.transform.parent;
        root = NGUITools.FindInParents<UIRoot>(parent);
    }

    void OnDragStart()
    {
        // 눌린 곳에 아이템이 없으면 리턴
        if (uiSlotInfo.slotSettings.uiIcon.mainTexture == null)
        {
            return;
        }

        isDragging = true; // 드래그 시작

        uiSlotInfo.slotSettings.uiIcon.alpha = 0.5f; // 드래그시작하면 원래있던건 반투명하게
        uiManager.tempIcon = uiSlotInfo.slotSettings.uiIcon;

        // icon 복사본 생성 - 마우스 따라 다닐거
        GameObject icon = CreateTemporary();
        draggedObject = icon;

        NGUITools.MarkParentAsChanged(this.gameObject);
    }

    void OnDrag(Vector2 delta)
    {
        // OnDragStart 에서 드래그 시작 안하면 리턴
        // 이 조건이 없으면 아무대나 클릭후 드래그가 된다?
        if (!isDragging)
        {
            return;
        }

        OnDragDropMove(delta * root.pixelSizeAdjustment);
    }

    void OnDragDropMove(Vector2 delta)
    {
        if (draggedObject != null)
        {
            draggedObject.transform.localPosition += (Vector3)delta;
        }
    }

    void OnDragEnd()
    {
        // 드래그 끝날때
        if (draggedObject == null)
        {
            return;
        }
        StopDragging(UICamera.hoveredObject); // 타겟 오브젝트.
    }

    void StopDragging(GameObject targetObj)
    {
        if (isDragging)
        {
            isDragging = false;
            OnDragDropRelease(targetObj);
        }
    }

    void OnDragDropRelease(GameObject targetObj)
    {
        // 드래그중에 복사한 임시 오브젝트 제거
        DestoryTemporary();
        
        // 빈 곳에 버릴겨우 targetObj는 UIRoot로 잡힘
        // TODO : 타겟이 없으면 인벤토리인경우 경고메시지(파괴할지 말지)
        if (string.Compare(targetObj.name, "UI Root") == 0)
        {
            uiSlotInfo.slotSettings.uiIcon.alpha = 1f;
            return;
        }

        UITexture targetTexture = targetObj.transform.GetChild(0).GetComponent<UITexture>(); // 타겟 아이콘
        UISlotInfo targetSlot = targetObj.GetComponent<UISlotInfo>(); // 타겟 정보

        CheckSlotType(targetSlot);
    }

    // 인벤토리 -> 인벤토리 (교환) / 창고 -> 창고 (교환) / 단축키 -> 단축키 (교환)
    private void CheckSlotType(UISlotInfo targetSlot)
    {
        // 인벤토리 -> 인벤토리 (교환) / 창고 -> 창고 (교환) / 단축키 -> 단축키 (교환)
        // 스킬창 -> 스킬창 (삭제)
        if (this.uiSlotInfo.slotType == targetSlot.slotType)
        {
            playerSlotData.SwapSlotData(uiSlotInfo.slotType, uiSlotInfo.slotIndex, targetSlot.slotIndex, targetSlot.isItemExist);
            uiSlotInfo.ReSetting();
            targetSlot.ReSetting();
        }
        // 인벤토리 
        else
        {

        }

        uiSlotInfo.slotSettings.uiIcon.alpha = 1f;
    }

    // 아이템을 드래그 할때 복제
    private GameObject CreateTemporary()
    {
        GameObject icon = Instantiate(uiSlotInfo.slotSettings.uiIcon.gameObject) as GameObject;
        icon.layer = gameObject.layer;

        // 복제한 아이콘에 TweenColor가 있으면 제거
        TweenColor cTween = icon.GetComponent<TweenColor>();
        if (cTween != null)
        {
            cTween.enabled = false;
            NGUITools.Destroy(cTween);
        }

        // 복제한 아이콘에 UITexture가 있으면 Color를 White로
        UITexture tex = icon.GetComponent<UITexture>();
        if (tex != null)
        {
            tex.color = Color.white;
        }

        if (UIDragDropRoot.root != null)
        {
            icon.transform.parent = UIDragDropRoot.root;
        }
        else
        {
            GameObject panelObj = new GameObject("_TemporaryPanel");
            panelObj.layer = gameObject.layer;
            panelObj.transform.parent = NGUITools.GetRoot(gameObject).transform;
            panelObj.transform.localScale = Vector3.one;
            panelObj.transform.localRotation = Quaternion.identity;
            panelObj.transform.localPosition = Vector3.one;

            tempDraggingPanel = panelObj;
            uiManager.tempDraggingPanel = panelObj;

            UIPanel panel = panelObj.AddComponent<UIPanel>();
            panel.depth = UIPanel.nextUnusedDepth;
            icon.transform.parent = panelObj.transform;
        }

        // 현재 마우스 위치에로 설정
        icon.transform.position = NGUITools.FindCameraForLayer(gameObject.layer).ScreenToWorldPoint(Input.mousePosition);
        icon.transform.rotation = transform.rotation;
        icon.transform.localScale = transform.localScale;

        return icon;
    }

    private void DestoryTemporary()
    {
        // 드래그중 / UI모드 해제시 - 복사한 임시 오브젝트 제거
        if (tempDraggingPanel != null)
        {
            NGUITools.Destroy(tempDraggingPanel);
        }
        else if (draggedObject != null)
        {
            Debug.Log("mDraggedObject");
            NGUITools.Destroy(draggedObject);
        }
    }

    private void SwapSlotInfo()
    {

    }
}