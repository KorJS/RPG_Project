using UnityEngine;
using System.Collections;

public class Floating3D : MonoBehaviour
{
    public Vector3 positionMult = Vector3.zero;
    public Vector3 posiotionDirection = Vector3.zero;

    private Vector3 positionTemp = Vector3.zero;
    private FloatingText floatingText = null;


    void Start()
    {
        floatingText = GetComponent<FloatingText>();
        positionTemp = transform.position;
    }

    void Update()
    {
        positionTemp += posiotionDirection * Time.deltaTime;
        posiotionDirection += positionMult * Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, positionTemp, 0.5f);
    }
}
