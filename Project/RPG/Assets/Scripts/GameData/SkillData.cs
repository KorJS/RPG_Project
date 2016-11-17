using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

public class SkillData
{
    // volatile 동시에 실행되는 여러 스레드에 의해 필드가 수정될 수 있음을 나타낸다.
    private static volatile SkillData skillData = null;
    private static object _lock = new System.Object();

    public static SkillData Instance
    {
        get
        {
            if (skillData == null)
            {
                // lock으로 지정된 블록안의 코드르 하나의 스레드만 접근
                lock (_lock)
                {
                    if (skillData == null)
                    {
                        skillData = new SkillData();
                    }
                }
            }

            return skillData;
        }
    }

    // TODO : 데이터를 받아올때 각 스킬별로 스킬렙업시 상승 효과도 같이 받아와서 PlayerSkillData에서 처리
    // 케릭 직업에 맞게 데이터를 받아온다
    public struct SkillInfo
    {
        public int skillIndex;      // 인덱스
        public int level;           // 습득 필요레벨
        public float coolTime;      // 쿨타임
        public float attack;        // 공격력
        public float mp;            // 소모MP / 습득MP
        public float angle;         // 각도
        public float distance;      // 거리
        public string name;         // 이름
        public string iconName;     // 아이콘 이름
        public string description;  // 스킬 설정
    };

    public SkillInfo skillinfo;

    public Dictionary<int, SkillInfo> skillInfos = null;

    public SkillData()
    {
        skillInfos = new Dictionary<int, SkillInfo>();
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
        string path = "Tables/skill_table";

        TextAsset ta = Resources.Load(path) as TextAsset;

        if (ta == null)
        {
            Debug.Log("파일이 존재하지 않습니다!!" + path);
        }

        skillInfos.Clear();

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

            SetSkillDate(index, ref temp);

            Debug.Log("skill_table에 데이타 등록 : " + temp[0]);

        } // end foreach

        //string path = pathForDocumentsFile("Tables/skill_table.csv");

        //if (File.Exists(path) == false)
        //{
        //    Debug.Log("파일이 존재하지 않습니다!!" + path);
        //    return;
        //}

        //string str = null;

        //skillInfos.Clear();

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

        //        SetSkillDate(index, ref temp);

        //        Debug.Log("skill_table에 데이타 등록 : " + temp[0]);
        //    } // end foreach
        //} // end while

        //sr.Close();
        //file.Close();

        Debug.Log("파일 읽기 완료 : " + path);

        PlayerSlotData.Instance.SetSkillListSlot();
        LevelData.Instance.LoadTalbe();
    }

    private void SetSkillDate(int index, ref string[] temp)
    {
        skillInfos.Add(index, new SkillInfo
        {
            skillIndex = int.Parse(temp[0]),    // 스킬인덱스
            level = int.Parse(temp[2]),         // 습득레벨
            coolTime = float.Parse(temp[3]),    // 쿨타임
            attack = float.Parse(temp[4]),      // 공격력
            mp = float.Parse(temp[5]),          // 소모, 습득 마력
            angle = float.Parse(temp[6]),       // 각도
            distance = float.Parse(temp[7]),    // 거리
            name = temp[8],                     // 이름
            iconName = temp[9],                 // 아이콘 이름
            description = temp[10]              // 설명
        });
    }

    public void DataClear()
    {
        skillInfos.Clear();
    }
}