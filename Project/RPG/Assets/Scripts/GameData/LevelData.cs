using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

public class LevelData
{
    // volatile 동시에 실행되는 여러 스레드에 의해 필드가 수정될 수 있음을 나타낸다.
    private static volatile LevelData levelData = null;
    private static object _lock = new System.Object();

    public static LevelData Instance
    {
        get
        {
            if (levelData == null)
            {
                // lock으로 지정된 블록안의 코드르 하나의 스레드만 접근
                lock (_lock)
                {
                    if (levelData == null)
                    {
                        levelData = new LevelData();
                    }
                }
            }

            return levelData;
        }
    }

    public struct LevelInfo
    {
        public int level;
        public int exp;
    }

    public LevelInfo levelInfo;

    public Dictionary<int, LevelInfo> levelInfos = null;

    public int MAX_Level = 0;

    public LevelData()
    {
        levelInfos = new Dictionary<int, LevelInfo>();
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
        string path = pathForDocumentsFile("Tables/level_table.csv");

        if (File.Exists(path) == false)
        {
            Debug.Log("파일이 존재하지 않습니다!!" + path);
            return;
        }

        string str = null;

        levelInfos.Clear();

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
                int index = int.Parse(temp[0]);

                TypeData.ItemType itemType = (TypeData.ItemType)int.Parse(temp[1]);

                SetLevelDate(index, ref temp);

                Debug.Log("level_table에 데이타 등록 : " + temp[0]);
            } // end foreach
        } // end while

        sr.Close();
        file.Close();

        Debug.Log("파일 읽기 완료 : " + path);

        // TODO : 만렙
        MAX_Level = levelInfos.Count;
    }

    private void SetLevelDate(int index, ref string[] temp)
    {
        levelInfos.Add(index, new LevelInfo
        {
            level = index,              // 레벨
            exp = int.Parse(temp[1])    // 경험치
        });
    }

    public void DataClear()
    {
        levelInfos.Clear();
    }
}
