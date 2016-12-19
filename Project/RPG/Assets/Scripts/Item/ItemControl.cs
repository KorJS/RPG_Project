using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemControl : MonoBehaviour
{
    private PlayerSlotData playerSlotData = null;

    // 아이템 정보 / NPC처럼 F 기능
    [System.Serializable]
    public class ItemSettings
    {
        [Header("- Item Info -")]
        public string               itemName;       // 아이템 이름
        public TypeData.ItemType    itemType;       // 아이템 타입
        public int                  itemIndex;      // 아이템 인덱스
        public int                  quantity;       // 수량

        [Header("- Item Control -")]
        public Transform            itemT;          // 아이템
        public Transform            targetT;        // 타겟
        public float                distance;       // 거리 - 주인공 검사
        public bool                 isPlayer;       // 주인공이 있는지 여부

        [Header("- Item UI -")]
        public GameObject           uiItemNameObj;  // 아이템 이름 UI
        public UILabel              uiItemName;     // 아이템 이름
    }

    public ItemSettings itemSettings;

    private GameObject  itemNameHolder  = null;   // 아이템 이름 부모

    private bool        isUIName        = false;  // 아이템이 카레마에 보이는지 여부

    void Awake()
    {
        itemSettings.itemT      = transform;
        itemSettings.distance   = 5f;
        itemSettings.isPlayer   = false;
    }

    void Start()
    {
        playerSlotData = PlayerSlotData.Instance;
        itemNameHolder = GameObject.Find("ItemNameHolder");

        CreateUIName();
    }

    void Update()
    {
        // 주변에 주인공이 있는지 검사
        itemSettings.isPlayer = CheckPlayer();

        // 주인공이 없으면 타겟 제거
        if (!itemSettings.isPlayer)
        {
            itemSettings.targetT = null;
        }

        // 주인공이 주변에 있고 F키를 클릭하면 아이템 습득처리 - 인벤으로
        if (itemSettings.isPlayer && Input.GetKeyDown(KeyCode.F))
        {
            SoundManager.Instance.PlaySingleUI(UIManager.Instance.uiSounds.pickupBGM);

            Debug.Log(itemSettings.quantity);
            playerSlotData.AddSlotData(TypeData.SlotType.인벤토리, itemSettings.itemType, itemSettings.itemIndex, itemSettings.quantity);

            Destroy(itemSettings.uiItemNameObj);
            Destroy(gameObject);
        }

        // 아이템 이름이 있으면 카메라에 비추어질때 활성화 사라지면 비활성화
        if (itemSettings.uiItemNameObj != null)
        {
            itemSettings.uiItemNameObj.SetActive(isUIName);
        }
    }

    // 아이템 주변에 주인공이 있는지 검사
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

    // 아이템 이름 UI 생성
    private void CreateUIName()
    {
        itemSettings.uiItemNameObj = Instantiate(Resources.Load("UI/ItemName")) as GameObject;
        itemSettings.uiItemNameObj.layer = UICamera.mainCamera.gameObject.layer;
        itemSettings.uiItemNameObj.transform.SetParent(itemNameHolder.transform);
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
        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.y = 0f;

        float dis = Vector3.Distance(cameraPos, itemSettings.itemT.position);

        // 아이템 이름이 활성화 상태에서 거리가 15m 멀어지면 비활성화
        if (itemSettings.uiItemNameObj.activeSelf && dis > 20f)
        {
            isUIName = false;
        }
        // 아이템 이름이 비활성화 상태에서 거리가 15m 내에 들어오면 활성화
        else if (!itemSettings.uiItemNameObj.activeSelf && dis <= 20f)
        {
            isUIName = true;
        }

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