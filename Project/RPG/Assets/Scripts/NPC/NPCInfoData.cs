using UnityEngine;
using System.Collections;

public class NPCInfoData : MonoBehaviour
{
    // NPC - 위치는 정해져 있음
    // 어느 지역인지
    // 어느 상점인지
    // 일정 범위안에 들어오면 이펙트 / Label(대화를 하려면 'F'를 누르십시오.) 활성화
    [System.Serializable]
    public class NPCInfo
    {
        public TypeData.NPCType npcType;

    }

    [SerializeField]
    public NPCInfo npcInfo;

    // 상점창 UI
    public GameObject storeObj = null;
    
    void Awake()
    {

    }
}
