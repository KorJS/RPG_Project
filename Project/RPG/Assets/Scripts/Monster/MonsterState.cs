using UnityEngine;
using System.Collections;

public class MonsterState : MonoBehaviour
{
    // 싱글톤
    private static MonsterState monsterState = null;
    public static MonsterState Instance
    {
        get
        {
            if (monsterState == null)
            {
                Debug.Log("MonsterState Instance Null");
            }
            return monsterState;
        }
    }
}