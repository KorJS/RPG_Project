using UnityEngine;
using System.Collections;

public class NPCState : MonoBehaviour
{
    private NPCInfoData npcInfoData = null;
    private NPCMovement npcMovement = null;

    public TypeData.NPCState currentState = TypeData.NPCState.없음;
    public TypeData.NPCState nextState = TypeData.NPCState.없음;

    void Awake()
    {
        npcInfoData = GetComponent<NPCInfoData>();
        npcMovement = GetComponent<NPCMovement>();

        currentState = TypeData.NPCState.없음;
        nextState = TypeData.NPCState.대기;
    }

    void Update()
    {
        CheckState();
    }

    private void CheckState()
    {
        if (nextState == TypeData.NPCState.없음)
        {
            return;
        }

        currentState = nextState;
        nextState = TypeData.NPCState.없음;

        //npcMovement.SetAniState(currentState);
    }

}