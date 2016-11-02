using UnityEngine;
using System.Collections;

public class UICharacterSelect : MonoBehaviour
{
    [System.Serializable]
    public class CharacterSelect
    {

        public GameObject CreateObj;
    }

    [SerializeField]
    public CharacterSelect characterSelect;
}