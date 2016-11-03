using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

public class ItemManager
{
    // volatile 동시에 실행되는 여러 스레드에 의해 필드가 수정될 수 있음을 나타낸다.
    private static volatile ItemManager itemManager = null;
    private static object _lock = new System.Object();

    public static ItemManager Instance
    {
        get
        {
            if (itemManager == null)
            {
                // lock으로 지정된 블록안의 코드르 하나의 스레드만 접근
                lock (_lock)
                {
                    if (itemManager == null)
                    {
                        itemManager = new ItemManager();
                    }
                }
            }

            return itemManager;
        }
    }

    private ItemManager() { }

    private Dictionary<int, ITEM> table = new Dictionary<int, ITEM>();

    public string pathForDocumentsFile(string fileName)
    {
        // Application.dataPath : 실행 파일이 실행되는 위치의 경로를 얻어온다(=> GetProject Root Path)
        if (Application.platform == RuntimePlatform.IPhonePlayer) // IOS 경로
        {
            string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
            path = path.Substring(0, path.LastIndexOf("/"));
            return Path.Combine(Path.Combine(path, "Documents"), fileName);
        }
        else if (Application.platform == RuntimePlatform.Android) // 안드로이드 경로
        {
            string path = Application.persistentDataPath;
            path = path.Substring(0, path.LastIndexOf("/"));
            return Path.Combine(path, fileName);
        }
        else // PC 또는 Web 경로
        {
            string path = Application.dataPath; // /Assets 경로
            path = path.Substring(0, path.LastIndexOf("/"));
            return Path.Combine(path, fileName);
        }
    }

    public void LoadTalbe()
    {
        string path = pathForDocumentsFile("Tables/item_table.csv");

        if (File.Exists(path) == false)
        {
            Debug.Log("파일이 존재하지 않습니다!!" + path);
            return;
        }

        string str = null;
        table.Clear();

        FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read); // 읽기 전용으로 불러옴
        StreamReader sr = new StreamReader(file);

        while ((str = sr.ReadLine()) != null)
        {
            string[] datas = Regex.Split(str, "\r\n");

            foreach (string data in datas)
            {
                // 데이타가 존재하지 ㅇ낳으면 freach()문을 빠져 나간다.
                if ((data == "") || (data.Length == 0))
                {
                    break;
                }

                // # 문자로 시자갛느 데이타는 무시한다.
                if (data[0] == '#')
                {
                    continue;
                }

                string[] temp = data.Split(',');
                int key = int.Parse(temp[0]);
                table.Add(key, new ITEM
                {
                    id = int.Parse(temp[0]),            // id
                    attack = int.Parse(temp[6]),        // 공격력
                    defence = int.Parse(temp[7]),       // 방어력
                    hp = int.Parse(temp[8]),            // 체력
                    mp = int.Parse(temp[9]),            // 마력
                    cooltime = float.Parse(temp[10]),   // 쿨타임
                    regeneration = int.Parse(temp[11]), // 회복력
                    level = int.Parse(temp[12]),        // 레벨
                    price = int.Parse(temp[13]),        // 가격
                    overlap = int.Parse(temp[14]),      // 중첩 유무
                    name = temp[15],                    // 이름
                    description = temp[16],             // 설명
                    icon = temp[17],                    // 아이콘
                    sound = temp[18],                   // 효과음
                    effect = temp[19]                   // 이펙트
                });

                Debug.Log("table에 데이타 등록 : " + temp[0]);
            } // end foreach
        } // end while

        sr.Close();
        file.Close();

        Debug.Log("파일 읽기 완료 : " + path);
    }

    // 아이템 정보 가져옴 - 보여지기 위한 데이터용
    public ITEM GetItemInfo(int index)
    {
        if (table.ContainsKey(index))
        {
            return table[index];
        }

        return null;
    }
}
public enum ITEM_TYPE_EQIP
{
    WEAPON = 1,
    //HELMET = 2,
    //SHIELD = 3,
    ARMOR = 2,
    SHOES = 4,
    //RING = 6,
    //NECKLACE = 7
}

//public enum ITEM_TYPE_UNIVERSAL
//{
//    MATERIAL = 1,
//    ENCHANT = 2,
//    QUEST = 3,
//    GOLD = 4
//}

public enum ITEM_TYPE_CONSUME
{
    //SKILL = 1,
    HEALING = 0,
    MANA = 1,
    //DEBUFF = 4,
    //BUFF = 5
}

//public enum ITEM_TYPE_COOPON { }

public enum ITEM_CLASS
{
    COMMON = -1,
    WARRIOR = 0,
    MAGICION = 1,
    PRIEST = 2
}

public enum ITEM_GRADE
{
    BEGINNER = 0,   // 일반 (흰)
    JUNIOR = 1,     // 고급 (녹)
    MAJOR = 2,      // 희귀 (파)
                    //RARE = 3,
    LEGEND = 3      // 전설 (노)
}

public class ITEM
{
    public int id;

    public int attack;          // 공격력
    public int defence;         // 방어력
    public int hp;              // 체력
    public int mp;              // 마력
    public float cooltime;      // 쿨타임
    public int regeneration;    // 회복력
    public int level;           // 레벨
    public int price;           // 가격
    public int overlap;        // 중첩 유무
    public string name;         // 이름
    public string description;  // 설명
    public string icon;         // 아이콘
    public string sound;        // 효과음
    public string effect;       // 이펙트
}