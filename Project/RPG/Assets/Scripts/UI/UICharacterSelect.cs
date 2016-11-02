using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICharacterSelect : MonoBehaviour
{
    [System.Serializable]
    public class CharacterSelect
    {
        public List<GameObject> slots = new List<GameObject>();
        public GameObject CreateObj;
    }

    [SerializeField]
    public CharacterSelect characterSelect;
}