using UnityEngine;
using System.Collections;

public class NPCRange : MonoBehaviour
{
    private Transform npcT = null;
    private GameObject effectObj = null; // NPC 발밑 활성화
    [SerializeField]
    public GameObject inquireTextObj = null; // "대화를 하려면 F키를 누르시오"

    private Transform targetT = null;
    public Collider[] targets;
    private float distance = 0f;
    public bool isPlayer = false;

    void Awake()
    {
        npcT = transform;
        effectObj = transform.FindChild("Aggro").gameObject;
        distance = 5f;
        isPlayer = false;
    }

    void Update()
    {
        isPlayer = CheckPlayer();

        if (!isPlayer)
        {
            targetT = null;
            effectObj.SetActive(false);
            inquireTextObj.SetActive(false);
        }
    }

    private bool CheckPlayer()
    {
        targets = Physics.OverlapSphere(npcT.position, distance);

        foreach (var target in targets)
        {
            // 주인공인 경우
            if (string.Compare(target.tag, "Player") == 0)
            {
                // 타겟 지정
                targetT = target.transform;
                effectObj.SetActive(true);
                inquireTextObj.SetActive(true);

                return true;
            }
        }

        return false;
    }
}
