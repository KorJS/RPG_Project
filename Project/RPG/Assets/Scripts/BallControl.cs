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
    }

    void OnTriggerEnter(Collider col)
    {
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
            //ballSettings.hitEffectObj.transform.position = ballT.position;
            //isShot = false;
            //ballT.gameObject.SetActive(false);
            //StartCoroutine(HitEffect());

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
        yield return new WaitForSeconds(2f);

        isShot = false;
        ballT.gameObject.SetActive(false);
    }

    public void SetBall(Transform userT, Vector3 orignT, Vector3 lookPos, int speed, int limitDis, float att, string hitEffectName)
    {
        ballSettings.userT = userT; // 사용자
        ballSettings.holder = userT.FindChild("SkillHolder");
        ballSettings.orignT = orignT; // 발사 첫 위치
        ballSettings.speed = speed; // 속도
        ballSettings.limitDis = limitDis; // 제한거리
        ballSettings.att = att; // 공격력

        // hit 이펙트
        if (ballSettings.hitEffectObj == null)
        {
            ballSettings.hitEffectObj = Instantiate(Resources.Load("Effect/Hit_" + hitEffectName)) as GameObject;
            ballSettings.hitEffectObj.GetComponent<EffectSetting>().infoSettings.effectHoler = ballSettings.holder;
            ballSettings.hitEffectObj.transform.SetParent(ballT);
            ballSettings.hitEffectObj.SetActive(false);
        }
        
        isShot = true;
        ballT.SetParent(null);
        Vector3 v = lookPos - ballSettings.orignT;
        ballT.rotation = Quaternion.LookRotation(v);

        StartCoroutine(UpdatePos());
    }

    private void ResetBall()
    {
        isShot = false;
        ballT.SetParent(ballSettings.holder);
        ballT.localPosition = Vector3.zero;
        ballT.gameObject.SetActive(false);
    }
}
