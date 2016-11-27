using UnityEngine;
using System.Collections;

public class BallControl : MonoBehaviour
{
    private PlayerMovement playerMovement = null;
    private MonsterMovement monsterMovement = null;

    public enum UserType
    {
        없음 = -1,
        주인공 = 0,
        몬스터
    };

    [System.Serializable]
    public class BallSettings
    {
        public UserType     userType    = UserType.없음;

        public Transform    userT;
        public Vector3      orignT;

        public int          targetLayer;

        public Transform    holder;
        public string       holderPath;

        public GameObject   hitEffectObj;
        public string       hitEffectName;

        public float        limitDis;
        public float        att;
        public float        speed;
    }

    [SerializeField]
    public BallSettings ballSettings;

    public Transform ballT = null;
    public bool isShot = false;

    void Awake()
    {
        ballT = transform;
        ballSettings.holder = GameObject.Find("UnequipEffectPool").transform;
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("col layer " + col.gameObject.layer);
        if (col.gameObject.layer == ballSettings.targetLayer)
        {
            Debug.Log("targetName : " + col.name + " Attack : " + ballSettings.att);

            switch (ballSettings.userType)
            {
                case UserType.주인공:
                    {
                        monsterMovement = col.GetComponent<MonsterMovement>();
                        monsterMovement.SetDamage(ballSettings.userT, -ballSettings.att);
                    }
                    break;

                case UserType.몬스터:
                    {
                        playerMovement = col.GetComponent<PlayerMovement>();
                        playerMovement.SetDamage(-ballSettings.att);
                    }
                    break;
            }

            ballSettings.hitEffectObj.SetActive(true);
            ballSettings.hitEffectObj.transform.position = ballT.position;
            StartCoroutine(HitEffect());

            ResetBall();
        }
    }

    IEnumerator UpdatePos()
    {
        while (true)
        {
            transform.Translate(0f, 0f, Time.deltaTime * ballSettings.speed);

            if (isShot)
            {
                if (Vector3.Distance(ballSettings.orignT, transform.position) > ballSettings.limitDis)
                {
                    ResetBall();
                }
            }

            yield return null;
        }
    }

    IEnumerator HitEffect()
    {
        yield return new WaitForSeconds(1f);

        isShot = false;
        ballSettings.hitEffectObj.SetActive(false);
        ballT.gameObject.SetActive(false);
    }

    // 사용자, ball 처음위치
    public void SetBall(Transform userT, Vector3 orignT, Vector3 lookPos, int speed, int limitDis, float att, string hitEffectName)
    {
        ballSettings.hitEffectObj = Instantiate(Resources.Load("Effect/Hit_" + hitEffectName)) as GameObject;
        ballSettings.hitEffectObj.transform.SetParent(ballT);
        ballSettings.hitEffectObj.SetActive(false);

        ballSettings.userT = userT;
        ballSettings.orignT = orignT;
        ballSettings.speed = speed;
        ballSettings.limitDis = limitDis;
        ballSettings.att = att;

        isShot = true;
        ballT.SetParent(null);
        Vector3 v = lookPos - ballSettings.orignT;
        ballT.rotation = Quaternion.LookRotation(v);

        StartCoroutine(UpdatePos());
    }

    private void ResetBall()
    {
        ballT.SetParent(ballSettings.holder);
        ballT.localPosition = Vector3.zero;
        ballT.gameObject.SetActive(false);
    }
}
