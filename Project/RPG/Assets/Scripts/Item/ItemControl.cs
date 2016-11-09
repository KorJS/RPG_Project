using UnityEngine;
using System.Collections;

public class ItemControl : MonoBehaviour
{
    private PlayerSlotData playerSlotData = null;

    // 아이템 정보 / NPC처럼 F 기능
    [System.Serializable]
    public class ItemSettings
    {
        [Header("- Item Info -")]
        public string itemName;
        public TypeData.ItemType itemType;
        public int itemIndex;
        public int quantity;

        [Header("- Item Control -")]
        public Transform itemT;
        public Transform targetT;
        public float distance;
        public bool isPlayer;

        [Header("- Item UI -")]
        public GameObject uiItemNameObj;
        public UILabel uiItemName;
    }

    public ItemSettings itemSettings;

    private bool isUIName = false;

    void Awake()
    {
        itemSettings.itemT = transform;
        itemSettings.distance = 5f;
        itemSettings.isPlayer = false;
    }

    void Start()
    {
        playerSlotData = PlayerSlotData.Instance;
        CreateUIName();
    }

    void Update()
    {
        itemSettings.isPlayer = CheckPlayer();

        if (!itemSettings.isPlayer)
        {
            itemSettings.targetT = null;
        }

        if (itemSettings.isPlayer && Input.GetKeyDown(KeyCode.F))
        {
            playerSlotData.AddSlotData(TypeData.SlotType.인벤토리, itemSettings.itemType, itemSettings.itemIndex, itemSettings.quantity);
            Destroy(itemSettings.uiItemNameObj);
            Destroy(gameObject);
        }

        if (itemSettings.uiItemNameObj != null)
        {
            itemSettings.uiItemNameObj.SetActive(isUIName);
        }
    }

    private bool CheckPlayer()
    {
        Collider[] targets = Physics.OverlapSphere(itemSettings.itemT.position, itemSettings.distance);

        foreach (var target in targets)
        {
            // 주인공인 경우
            if (string.Compare(target.tag, "Player") == 0)
            {
                // 타겟 지정
                itemSettings.targetT = target.transform;

                return true;
            }
        }

        return false;
    }

    private void CreateUIName()
    {
        itemSettings.uiItemNameObj = Instantiate(Resources.Load("UI/ItemName")) as GameObject;
        itemSettings.uiItemNameObj.layer = UICamera.mainCamera.gameObject.layer;
        itemSettings.uiItemNameObj.transform.SetParent(GameObject.Find("ItemNameHolder").transform);
        itemSettings.uiItemNameObj.transform.localScale = Vector3.one;
        itemSettings.uiItemNameObj.transform.localRotation = Quaternion.identity;
        itemSettings.uiItemNameObj.transform.localPosition = Vector3.one;
        // 이름 클릭 했을때 아이템 정보필요
        itemSettings.uiItemNameObj.GetComponent<UIItemInfo>().SetItemInfo(itemSettings.itemT, this, itemSettings.itemType, itemSettings.itemIndex, itemSettings.quantity);
        // 아이템 이름 라벨
        itemSettings.uiItemName = itemSettings.uiItemNameObj.transform.FindChild("Name").GetComponent<UILabel>();
        itemSettings.uiItemName.text = itemSettings.itemName;
    }

    // 아이템 이름 위치 설정
    private void SetPosition()
    {
        Vector3 p = Camera.main.WorldToViewportPoint(itemSettings.itemT.position);
        itemSettings.uiItemNameObj.transform.position = UICamera.mainCamera.ViewportToWorldPoint(p);

        p = itemSettings.uiItemNameObj.transform.localPosition;
        p.x = Mathf.RoundToInt(p.x);
        p.y = Mathf.RoundToInt(p.y);
        p.z = 0f;

        itemSettings.uiItemNameObj.transform.localPosition = p;
    }

    // 해당 오브젝트가 보이면 호출
    void OnWillRenderObject()
    {
        SetPosition();
    }

    // 해당 오브젝트가 보일때 한번 호출
    void OnBecameVisible()
    {
        isUIName = true;
    }

    // 해당 오브젝트가 사라질때 한번 호출
    void OnBecameInvisible()
    {
        isUIName = false;
    }
}