using UnityEngine;
using System.Collections;

[ExecuteInEditMode] // 게임 플레이를 하지 않아도 스크립트 내용이 적용되는 기능
public class CameraControl : MonoBehaviour
{
    private UIManager uiManager = null;
    private UIJoystick uiJoystick = null;

    [System.Serializable]
    public class CameraSettings
    {
        [Header("-Position-")]
        public Vector3 camPositionOffset = Vector3.zero;    // 카메라 오브셋

        [Header("-Camera Options-")]
        public float mouseXSensitivity      = 5f;           // X축 회전 감도
        public float mouseYSensitivity      = 5f;           // Y축 회전 감도
        public float minAngle               = -80f;         // Y축 최소 회전값
        public float maxAngle               = 50f;          // Y축 최대 회전값
        public float rotationSpeeed         = 6f;           // 회전 속도
        public float maxCheckDist           = 0.1f;         // 카메라 충돌 거리(반경)

        [Header("-Zoom-")]
        public float fieldOfView            = 70f;          // 최소 줌거리
        public float zoomFieldOfView        = 25f;          // 최대 줌거리
        public float zoomSpeed              = 10f;          // 줌 속도

        [Header("-Visual Options-")]
        public float hidMeshWheenDistance   = 0.5f;         // 주인공 매쉬를 숨길 거리

        [Header("-Shake-")]
        private float shakeAmount           = 0f;           // 강도
        private float shakeTimer            = 0f;           // 지속시간
    }

    [SerializeField]
    public CameraSettings cameraSettings;

    [System.Serializable]
    public class InputSettings
    {
        public string verticalAxis          = "Mouse X";
        public string horizontalAxis        = "Mouse Y";
        public string zoomAxis              = "Mouse ScrollWheel";
    }

    [SerializeField]
    public InputSettings input;

    [System.Serializable]
    public class MovementSettings
    {
        public float movementLerpSpeed      = 5f;           // 카메라 이동 속도
    }

    public MovementSettings movement;

    // 주인공
    public  Transform   target                  = null;
    public  bool        autoTargetPlayer        = false;

    public  LayerMask   wallLayers;
    private Camera      mainCamera              = null;
    private Transform   pivot                   = null;
    private float       newX                    = 0f;
    private float       newY                    = 0;


    void Awake()
    {
        //uiJoystick = GameObject.FindGameObjectWithTag("RotJoystick").GetComponent<UIJoystick>();

        mainCamera = Camera.main;
        pivot = this.transform.GetChild(0);
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        }
        autoTargetPlayer = true;
    }

    void Update()
    {
        if (GameManager.Instance.currentGameState != TypeData.GameState.시작)
        {
            return;
        }

        if (target)
        {
            if (Application.isPlaying)
            {
                if (UICamera.Raycast(Input.mousePosition))
                {
                    return;
                }

                // UI 모드가 아닐때 회전가능
                if (!uiManager.isUIMode)
                {
                    RotateCamera();
                }

                CheckWall();
                CheckMeshRenderer();
                Zoom(Input.GetAxis(input.zoomAxis));
            }
        }
    }

    void LateUpdate()
    {
        if (!target)
        {
            TargetPlayer();
        }
        else
        {
            Vector3 targetPosition = target.position;
            Quaternion targetRotation = target.rotation;

            FollowTarget(targetPosition, targetRotation);
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
    private void FollowTarget(Vector3 targetPosition, Quaternion targetRotation)
    {
        if (!Application.isPlaying)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
        else
        {
            Vector3 newPos = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * movement.movementLerpSpeed);
            transform.position = newPos;
        }
    }

    // 카메라 회전
    private void RotateCamera()
    {
        if (!pivot)
        {
            return;
        }

        newX += cameraSettings.mouseXSensitivity * Input.GetAxis(input.verticalAxis);
        newY -= cameraSettings.mouseYSensitivity * Input.GetAxis(input.horizontalAxis);

        //newX += cameraSettings.mouseXSensitivity * uiJoystick.joyStickPosX;
        //newY -= cameraSettings.mouseYSensitivity * uiJoystick.joyStickPosY;

        Vector3 eulerAngleAxis = new Vector3();
        eulerAngleAxis.x = newY;
        eulerAngleAxis.y = newX;

        newX = Mathf.Repeat(newX, 360); // X축 360 회전
        newY = Mathf.Clamp(newY, cameraSettings.minAngle, cameraSettings.maxAngle); // Y축 설정한 각도로만 회전

        Quaternion newRotation = Quaternion.Slerp(pivot.localRotation, Quaternion.Euler(eulerAngleAxis), Time.deltaTime * cameraSettings.rotationSpeeed);

        pivot.localRotation = newRotation;
    }

    // 카메라 충돌
    private void CheckWall()
    {
        if (!pivot || !mainCamera)
        {
            return;
        }

        RaycastHit hit;

        Transform mainCamT = mainCamera.transform;
        Vector3 mainCamPos = mainCamT.position;
        Vector3 pivotPos = pivot.position;

        Vector3 start = pivotPos;
        Vector3 dir = mainCamPos - pivotPos;

        float dist = Mathf.Abs(cameraSettings.camPositionOffset.z);

        if (Physics.SphereCast(start, cameraSettings.maxCheckDist, dir, out hit, dist, wallLayers))
        {
            // 충돌하면 거리 보정
            MoveCamUp(hit, pivotPos, dir, mainCamT);
        }
        else
        {   // 충돌 하지 않으면 offset 위치로
            PositionCamera(cameraSettings.camPositionOffset);
        }
    }

    // pivot 위치와 충돌처리된 위치 거리 보정
    private void MoveCamUp(RaycastHit hit, Vector3 pivotPos, Vector3 dir, Transform cameraT)
    {
        float hitDist = hit.distance;
        Vector3 sphereCastCenter = pivotPos + (dir.normalized * hitDist);
        cameraT.position = sphereCastCenter;
    }

    // offset 설정된 위치로
    private void PositionCamera(Vector3 camerPos)
    {
        if (!mainCamera)
        {
            return;
        }
        Transform mainCamT = mainCamera.transform;
        Vector3 mainCamPos = mainCamT.localPosition;
        Vector3 newPos = Vector3.Lerp(mainCamPos, camerPos, Time.deltaTime * movement.movementLerpSpeed);
        mainCamT.localPosition = newPos;
    }

    // 타켓 매쉬 숨기기
    private void CheckMeshRenderer()
    {
        if (!mainCamera || !target)
        {
            return;
        }

        SkinnedMeshRenderer[] meshes = target.GetComponentsInChildren<SkinnedMeshRenderer>(); // 타겟에 매쉬정보를 가져옴
        Transform mainCamT = mainCamera.transform;
        Vector3 mainCamPos = mainCamT.position;
        Vector3 targetPos = target.position;

        float dist = Vector3.Distance(mainCamPos, (targetPos + target.up)); // 거리 측정

        if (meshes.Length > 0)
        {
            for (int i = 0; i < meshes.Length; i++)
            {
                // 카메라와 주인공의 거리가 설정한 값보다 작으면 숨기고 아니면 보이게 한다.
                if (dist <= cameraSettings.hidMeshWheenDistance)
                {
                    meshes[i].enabled = false;
                }
                else
                {
                    meshes[i].enabled = true;
                }
            }
        }
    }
    
    // 카메라 줌
    private void Zoom(float zoom)
    {
        if (!mainCamera)
        {
            return;
        }
        
        if (zoom > 0)
        {
            float newFieldOfView = Mathf.Lerp(mainCamera.fieldOfView, cameraSettings.zoomFieldOfView, Time.deltaTime * cameraSettings.zoomSpeed);
            mainCamera.fieldOfView = newFieldOfView;
        }
        else if (zoom < 0)
        {
            float originalFieldOfView = Mathf.Lerp(mainCamera.fieldOfView, cameraSettings.fieldOfView, Time.deltaTime * cameraSettings.zoomSpeed);
            mainCamera.fieldOfView = originalFieldOfView;
        }
    }

    // 카메라 흔들림
    public void Shake(float x, float y, float z)
    {
        float randX = Random.Range(x, -x);
        float randY = Random.Range(y, -y);
        float randZ = Random.Range(z, -z);

        transform.position += new Vector3(randX, randY, randZ);
    }
}