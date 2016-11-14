using UnityEngine;
using System.Collections;

public class BallControl : MonoBehaviour
{
    [System.Serializable]
    public class BallSettings
    {
        public Transform orignT;
        public Transform holder;
        public GameObject hitEffectObj;

        public Vector3 skillPos;
        public Vector3 skillRange;
        public float att;
        public float speed;
    }

    [SerializeField]
    public BallSettings ballSettings;

    public Transform ballT = null;
    public bool isShot = false;

    void Awake()
    {
        ballT = transform;
        ballSettings.holder = GameObject.Find("MonsterEffectPool").transform;
        ballSettings.speed = 5f;
        ballSettings.hitEffectObj = Instantiate(Resources.Load("Effect/Monster/B_Hit")) as GameObject;
        ballSettings.hitEffectObj.transform.SetParent(ballT);
        ballSettings.hitEffectObj.SetActive(false);
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Ball");
            Debug.Log("targetName : " + coll.name + " Attack : " + ballSettings.att);
            // 주인공 Hit
            PlayerMovement playerMovement = coll.GetComponent<PlayerMovement>();
            playerMovement.Damage(-ballSettings.att);

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
                if (Vector3.Distance(ballSettings.orignT.position, transform.position) > 15)
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

    public void SetBall(Transform orignT, Vector3 lookPos, Vector3 skillPos, Vector3 skillRange, float att)
    {
        ballSettings.orignT = orignT;
        ballSettings.skillPos = skillPos;
        ballSettings.skillRange = skillRange;
        ballSettings.att = att;

        isShot = true;
        ballT.SetParent(null);
        Vector3 v = lookPos - ballSettings.orignT.position;
        ballT.rotation = Quaternion.LookRotation(v);

        StartCoroutine(UpdatePos());
    }

    private void ResetBall()
    {
        ballT.SetParent(ballSettings.holder);
        ballT.localPosition = Vector3.zero;
    }
}
