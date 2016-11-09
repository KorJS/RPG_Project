using UnityEngine;
using System.Collections;

public class NPCRange : MonoBehaviour
{
    private UIManager uiManager = null;

    private Transform npcT = null;
    private GameObject effectObj = null; // NPC 발밑 활성화
    [SerializeField]
    public GameObject inquireObj = null; // "대화를 하려면 F키를 누르시오"

    private Transform targetT = null;
    private float distance = 0f;
    public bool isPlayer = false;

    void Awake()
    {
        npcT = transform;
        effectObj = transform.FindChild("Aggro").gameObject;
        distance = 5f;
        isPlayer = false;
    }

    void Start()
    {
        uiManager = UIManager.Instance;
        CreateMessage();
    }

    void Update()
    {
        isPlayer = CheckPlayer();

        if (!isPlayer)
        {
            targetT = null;
            effectObj.SetActive(false);
            inquireObj.SetActive(false);
        }
    }

    private bool CheckPlayer()
    {
        Collider[] targets = Physics.OverlapSphere(npcT.position, distance);

        foreach (var target in targets)
        {
            // 주인공인 경우
            if (string.Compare(target.tag, "Player") == 0)
            {
                // 타겟 지정
                targetT = target.transform;
                effectObj.SetActive(true);
                inquireObj.SetActive(true);

                return true;
            }
        }

        return false;
    }

    private void CreateMessage()
    {
        inquireObj = Instantiate(Resources.Load("UI/Inquire_msg")) as GameObject;
        inquireObj.layer = UICamera.mainCamera.gameObject.layer;
        inquireObj.transform.SetParent(GameObject.FindGameObjectWithTag("InquireHolder").transform);
        inquireObj.transform.localScale = Vector3.one;
        inquireObj.transform.localRotation = Quaternion.identity;
        inquireObj.transform.localPosition = Vector3.one;
        
        inquireObj.SetActive(false);
    }
}
