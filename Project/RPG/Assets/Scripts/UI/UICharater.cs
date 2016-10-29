using UnityEngine;
using System.Collections;

public class UICharater : MonoBehaviour
{
    // slotIndex : 1 - 무기 / 2 - 갑옷 / 3 - 장갑 / 4 - 신발
    // 인벤>케릭 - 케릭슬롯에 아이템이 존재하면 교환 / 존재하지않으면 교체
    // 케릭>인벤 - 빈곳에 추가
    public void CloseWindow()
    {
        gameObject.SetActive(false);
    }
}