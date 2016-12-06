using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

public sealed class ItemData
{
    // volatile 동시에 실행되는 여러 스레드에 의해 필드가 수정될 수 있음을 나타낸다.
    private static volatile ItemData itemData = null;
    private static object _lock = new System.Object();

    public static ItemData Instance
    {
        get
        {
            if (itemData == null)
            {
                // lock으로 지정된 블록안의 코드르 하나의 스레드만 접근
                lock (_lock)
                {
                    if (itemData == null)
                    {
                        itemData = new ItemData();
                    }
                }
            }

            return itemData;
        }
    }

    // 장비 정보 - 장비타입 상관없이 통으로 받아옴.
    public struct EquipmentInfo
    {
        public int equipmentType;                       // 장비타입
        public int buyGold;                             // 구매가격
        public int sellGold;                            // 판매가격
        public int playerType;                          // 착용직업
        public int level;                               // 착용레벨
        public int att;                                 // 공격력
        public int def;                                 // 방어력
        public int hp;                                  // 체력
        public int mp;                                  // 마력
        public string name;                             // 이름
        public string iconName;                         // 아이콘 이름
        public string description;                      // 설명
    };

    // 소모품 정보
    public struct CusomableInfo
    {
        public int cusomableType;                       // 소모품타입
        public int buyGold;                             // 구매가격
        public int sellGold;                            // 판매가격
        public int level;                               // 착용레벨
        public int att;                                 // 공격력
        public int def;                                 // 방어력
        public int hp;                                  // 체력
        public int mp;                                  // 마력
        public float coolTime;                          // 쿨타임
        public string name;                             // 이름
        public string iconName;                         // 아이콘 이름
        public string description;                      // 설명
    };

    // 퀘스트템 정보
    public struct QuestItemInfo
    {
        public int sellGold;                            // 판매가격
        public string name;                             // 이름
        public string iconName;                         // 아이콘 이름
        public string description;                      // 설명
    };

    public Dictionary<int, EquipmentInfo> equipmentInfos = null; // 장비 
    public Dictionary<int, CusomableInfo> cusomableInfos = null; // 소모품 
    public Dictionary<int, QuestItemInfo> questItemInfos = null; // 퀘스트템 

    public EquipmentInfo equipmentInfo;  // 장비 정보
    public CusomableInfo cusomableInfo;  // 소모품 정보
    public QuestItemInfo questItemInfo;   // 퀘스트템 정보

    private ItemData()
    {
        equipmentInfos = new Dictionary<int, EquipmentInfo>();
        cusomableInfos = new Dictionary<int, CusomableInfo>();
        questItemInfos = new Dictionary<int, QuestItemInfo>();
    }

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
        string path = "Tables/item_table";

        TextAsset ta = Resources.Load(path) as TextAsset;

        if (ta == null)
        {
            Debug.Log("파일이 존재하지 않습니다!!" + path);
        }

        string[] datas = Regex.Split(ta.text, "\r\n");

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
            int index = int.Parse(temp[0]);

            TypeData.ItemType itemType = (TypeData.ItemType)int.Parse(temp[1]);

            switch (itemType)
            {
                case TypeData.ItemType.장비:
                    {
                        SetEquipmentDate(index, ref temp);
                    }
                    break;

                case TypeData.ItemType.소모품:
                    {
                        SetCusomableData(index, ref temp);
                    }
                    break;

                case TypeData.ItemType.퀘스트템:
                    {
                        SetQuestItemData(index, ref temp);
                    }
                    break;
            }

            Debug.Log("item_table에 데이타 등록 : " + temp[0]);
        } // end foreach

        //string path = pathForDocumentsFile("Tables/item_table.csv");

        //if (File.Exists(path) == false)
        //{
        //    Debug.Log("파일이 존재하지 않습니다!!" + path);
        //    return;
        //}

        //string str = null;
        //equipmentInfos.Clear();
        //cusomableInfos.Clear();
        //questItemInfos.Clear();

        //FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read); // 읽기 전용으로 불러옴
        //StreamReader sr = new StreamReader(file);

        //while ((str = sr.ReadLine()) != null)
        //{
        //    string[] datas = Regex.Split(str, "\r\n");

        //    foreach (string data in datas)
        //    {
        //        // 데이타가 존재하지 ㅇ낳으면 freach()문을 빠져 나간다.
        //        if ((data == "") || (data.Length == 0))
        //        {
        //            break;
        //        }

        //        // # 문자로 시자갛느 데이타는 무시한다.
        //        if (data[0] == '#')
        //        {
        //            continue;
        //        }

        //        string[] temp = data.Split(',');
        //        int index = int.Parse(temp[0]);

        //        TypeData.ItemType itemType = (TypeData.ItemType)int.Parse(temp[1]);

        //        switch (itemType)
        //        {
        //            case TypeData.ItemType.장비:
        //                {
        //                    SetEquipmentDate(index, ref temp);
        //                }
        //                break;

        //            case TypeData.ItemType.소모품:
        //                {
        //                    SetCusomableData(index, ref temp);
        //                }
        //                break;

        //            case TypeData.ItemType.퀘스트템:
        //                {
        //                    SetQuestItemData(index, ref temp);
        //                }
        //                break;
        //        }

        //        Debug.Log("item_table에 데이타 등록 : " + temp[0]);
        //    } // end foreach
        //} // end while

        //sr.Close();
        //file.Close();

        Debug.Log("파일 읽기 완료 : " + path);

        LevelData.Instance.LoadTalbe();

        //SkillData.Instance.LoadTalbe();
    }

    private void SetEquipmentDate(int index, ref string[] temp)
    {
        equipmentInfos.Add(index, new EquipmentInfo
        {
            equipmentType   = int.Parse(temp[2]),   // 장비타입
            buyGold         = int.Parse(temp[4]),   // 구매가격
            sellGold        = int.Parse(temp[5]),   // 판매가격
            playerType      = int.Parse(temp[6]),   // 착용직업
            level           = int.Parse(temp[7]),   // 착용레벨
            att             = int.Parse(temp[8]),   // 공격력
            def             = int.Parse(temp[9]),   // 방어력
            hp              = int.Parse(temp[10]),  // 체력
            mp              = int.Parse(temp[11]),  // 마력
            name            = temp[13],             // 이름
            iconName        = temp[14],             // 아이콘 이름
            description     = temp[15]              // 설명
        });
    }

    private void SetCusomableData(int index, ref string[] temp)
    {
        cusomableInfos.Add(index, new CusomableInfo
        {
            cusomableType   = int.Parse(temp[3]),   // 소모품타입
            buyGold         = int.Parse(temp[4]),   // 구매가격
            sellGold        = int.Parse(temp[5]),   // 판매가격
            level           = int.Parse(temp[7]),   // 착용레벨
            att             = int.Parse(temp[8]),   // 공격력
            def             = int.Parse(temp[9]),   // 방어력
            hp              = int.Parse(temp[10]),  // 체력
            mp              = int.Parse(temp[11]),  // 마력
            coolTime        = float.Parse(temp[12]),// 쿨타임
            name            = temp[13],             // 이름
            iconName        = temp[14],             // 아이콘 이름
            description     = temp[15]              // 설명
        });
    }

    private void SetQuestItemData(int index, ref string[] temp)
    {
        questItemInfos.Add(index, new QuestItemInfo
        {
            sellGold        = int.Parse(temp[5]),   // 판매가격
            name            = temp[13],             // 이름
            iconName        = temp[14],             // 아이콘 이름
            description     = temp[15]              // 설명
        });
    }

    public void DataClear()
    {
        equipmentInfos.Clear();
        cusomableInfos.Clear();
        questItemInfos.Clear();
    }
}