using UnityEngine;
using System.Collections;

public class UIDamage : MonoBehaviour
{
    public TweenAlpha twAlpha = null;
    public TweenPosition twPos = null;

    void Awake()
    {
        twAlpha = GetComponent<TweenAlpha>();
        twPos = GetComponent<TweenPosition>();
    }

    void OnEnable()
    {
        twAlpha.Play();
        twPos.Play();
    }

    void OnDisable()
    {
        twAlpha.ResetToBeginning();
        twPos.ResetToBeginning();
    }

    void Update()
    {
        if (!twAlpha.enabled)
        {
            gameObject.SetActive(false);
        }
    }
}
