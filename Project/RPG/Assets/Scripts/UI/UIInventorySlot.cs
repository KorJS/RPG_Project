using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIInventorySlot : MonoBehaviour
{
    public PlayerInventory playerInventory = null;

    [System.Serializable]
    public struct SlotInfo
    {
        public int slotIndex;
        public string itemName;
        public PlayerInventory.InventoryInfo inventoryInfo;
    }

    [SerializeField]
    public SlotInfo slotInfo;

    [System.Serializable]
    public class SlotSettings
    {
        public UITexture uiIcon;
        public UISprite  uiCoolTime;
        public UILabel   uiQuantity;
    }

    [SerializeField]
    public SlotSettings slotSettings;

    private bool            isItemExist             = false;  // 해당 슬롯에 아이템이 있는지

    // 드래그 앤 드롭.
    private UIRoot          mRoot                   = null;
    private Transform       mParent                 = null;
    private GameObject      mDraggedObject          = null;     // 드래그중인 icon
    private GameObject      mTempDraggingPanel      = null;     // 드래그중인 panel
    private bool            isPressed               = false;    // 마우스를 클릭해제했는지
    private bool            isDragging              = false;    // 드래그중인지

    void Awake()
    {
        playerInventory = PlayerInventory.Instance;

        mParent = this.transform.parent;
        mRoot = NGUITools.FindInParents<UIRoot>(mParent);

        SetSlotIndex(); // 슬롯 인덱스 설정
        SetSlotInfo(); // 슬롯 정보 설정
        SetSlotIcon(); // 슬롯 아이콘 설정
    }

    void OnClick()
    {

    }

    void OnDragStart()
    {
        // 눌린 곳에 아이템이 없으면 리턴
        if (slotSettings.uiIcon.mainTexture == null)
        {
            return;
        }

        isDragging = true;

        slotSettings.uiIcon.alpha = 0.5f; // 드래그시작하면 원래있던건 반투명하게

        // icon 복사본 생성 - 마우스 따라 다닐거
        GameObject icon = CreateTemporary();
        mDraggedObject = icon;

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

        OnDragDropMove(delta * mRoot.pixelSizeAdjustment);
    }

    void OnDragDropMove(Vector2 delta)
    {
        if (mDraggedObject != null)
        {
            mDraggedObject.transform.localPosition += (Vector3)delta;
        }
    }

    void OnDragEnd()
    {
        // 드래그 끝날때
        if (mDraggedObject == null)
        {
            return;
        }
        StopDragging(UICamera.hoveredObject);
    }

    void StopDragging(GameObject go)
    {
        if (isDragging)
        {
            isDragging = false;
            OnDragDropRelease(go);
        }
    }

    void OnDragDropRelease(GameObject surface)
    {
        if (mTempDraggingPanel != null)
        {
            NGUITools.Destroy(mTempDraggingPanel);
        }
        else if (mDraggedObject != null)
        {
            NGUITools.Destroy(mDraggedObject);
        }

        UITexture targetTexture = surface.transform.GetChild(0).GetComponent<UITexture>();
        UIInventorySlot targetSlot = surface.GetComponent<UIInventorySlot>();

        // TODO : 밖에 놓았을때 경고창 버릴건지


    }

    // 슬롯 인덱스 설정
    private void SetSlotIndex()
    {
        string[] strIndex = this.gameObject.name.Split(' ');
        slotInfo.slotIndex = int.Parse(strIndex[1]);
    }

    // 슬롯 정보 설정
    private void SetSlotInfo()
    {
        isItemExist = playerInventory.GetInventroyDate(slotInfo.slotIndex, ref slotInfo.itemName, ref slotInfo.inventoryInfo);
    }

    // 슬롯에 아이콘 설정
    private void SetSlotIcon()
    {
        // 슬롯에 아이템이 없으면 리턴
        if (!isItemExist)
        {
            return;
        }

        slotSettings.uiIcon.mainTexture = Resources.Load("Icon/Item/" + slotInfo.itemName) as Texture2D;
    }

    // 아이템을 드래그 할때 복제
    private GameObject CreateTemporary()
    {
        GameObject icon = Instantiate(slotSettings.uiIcon.gameObject) as GameObject;
        icon.layer = this.gameObject.layer;

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
            panelObj.layer = this.gameObject.layer;
            panelObj.transform.parent = NGUITools.GetRoot(this.gameObject).transform;
            panelObj.transform.localScale = Vector3.one;
            panelObj.transform.localRotation = Quaternion.identity;
            panelObj.transform.localPosition = Vector3.one;

            this.mTempDraggingPanel = panelObj;

            UIPanel panel = panelObj.AddComponent<UIPanel>();
            panel.depth = UIPanel.nextUnusedDepth;
            icon.transform.parent = panelObj.transform;
        }

        // 현재 마우스 위치에로 설정
        icon.transform.position = NGUITools.FindCameraForLayer(this.gameObject.layer).ScreenToWorldPoint(Input.mousePosition);
        icon.transform.rotation = this.transform.rotation;
        icon.transform.localScale = this.transform.localScale;

        return icon;

    } 

}