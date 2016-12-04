using UnityEngine;
using System.Collections;

public class MiniMapCameraControl : MonoBehaviour
{
    // 주인공
    public Transform target = null;
    public bool autoTargetPlayer = false;

    void LateUpdate()
    {
        if (!target)
        {
            TargetPlayer();
        }
        else
        {
            Vector3 targetPosition = target.position;

            targetPosition.y = 20f;

            FollowTarget(targetPosition);
        }
    }

    // 주인공 자동 타겟 설정
    private void TargetPlayer()
    {
        // 자동 타켓 설정
        if (autoTargetPlayer)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            // 타켓이 설정 됬으면
            if (player)
            {
                Transform playerT = player.transform;
                target = playerT;
            }
        }
    }

    // 타켓 따라 다니는 함수
    private void FollowTarget(Vector3 targetPosition)
    {
        if (!Application.isPlaying)
        {
            transform.position = target.position;
        }
        else
        {
            transform.position = targetPosition;
        }
    }
}
