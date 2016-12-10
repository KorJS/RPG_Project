using UnityEngine;
using System.Collections;

public class Floating3D : MonoBehaviour
{
    public Vector3 positionMult = Vector3.zero;
    public Vector3 posiotionDirection = Vector3.zero;

    public Vector3 positionTemp = Vector3.zero;
    private FloatingText floatingText = null;
    public Vector3 origin = Vector3.zero;

    void Start()
    {
        floatingText = GetComponent<FloatingText>();
        positionTemp = transform.position;
    }

    void OnDisable()
    {
        positionTemp = Vector3.zero;
        posiotionDirection = new Vector3(3f, 7f, 0f);
        transform.localPosition = Vector3.zero;
    }

    void Update()
    {
        positionTemp += posiotionDirection * Time.deltaTime;
        posiotionDirection += positionMult * Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, positionTemp, 0.5f);
    }
}
