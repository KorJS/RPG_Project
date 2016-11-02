using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Network_Char : MonoBehaviour
{
    public GameObject loginObj = null;
    public GameObject charObj = null;
    public GameObject createObj = null;
    public GameObject idObj = null;
    public GameObject pwObj = null;
    public GameObject slotsObj = null;

    public UILabel message = null;
    
    [System.Serializable]
    public class Slot
    {
        public GameObject warriorObj;
        public GameObject magicianObj;
        public GameObject priestObj;
        public GameObject emptyObj;
        public UILabel nickName;
        public UILabel level;
    }

    public List<Slot> slots = new List<Slot>();

    [System.Serializable]
    public class CreateCharacter
    {
        public GameObject warriorObj;
        public GameObject magicianObj;
        public GameObject priestObj;
        public UIInput nickName;
    }

    public CreateCharacter createCharacter;

    public void RequestInGame()
    {

    }

    public void ReplyInGame()
    {

    }

    public void CharCancelBtn()
    {
        loginObj.SetActive(true);
        idObj.SetActive(true);
        pwObj.SetActive(true);
        charObj.SetActive(false);
        createObj.SetActive(false);
        slotsObj.SetActive(false);
    }

    public void CreateOnBtn()
    {
        createObj.SetActive(true);
    }

    public void CreateCancelBtn()
    {
        createCharacter.nickName.value = null;
        createObj.SetActive(false);
    }

    public void WarriorBtn()
    {
        createCharacter.warriorObj.SetActive(true);
        createCharacter.magicianObj.SetActive(false);
        createCharacter.priestObj.SetActive(false);
    }

    public void MagicianBtn()
    {
        createCharacter.warriorObj.SetActive(false);
        createCharacter.magicianObj.SetActive(true);
        createCharacter.priestObj.SetActive(false);
    }

    public void PriestBtn()
    {
        createCharacter.warriorObj.SetActive(false);
        createCharacter.magicianObj.SetActive(false);
        createCharacter.priestObj.SetActive(true);
    }
}