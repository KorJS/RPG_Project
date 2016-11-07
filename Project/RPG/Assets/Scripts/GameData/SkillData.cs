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

        //skillinfo.skillIndex = 0;
        //skillinfo.name = "연속 공격";
        //skillinfo.iconName = "Combo";
        //skillinfo.description = "근접한 대상에게 타격을 입히고 일정량의 MP를 회복합니다. 스킬 버튼을 연속적으로 누르면 공격이 이어지면서 더 강한 타격을 입힐 수 있습니다."; // 형식 포맷 사용하자
        //skillinfo.level = 1;
        //skillinfo.coolTime = 1f;
        //skillinfo.attack = 1f;
        //skillinfo.mp = 5f;
        //skillinfo.angle = 60f;
        //skillinfo.distance = 3f;

        //skillInfos.Add(skillinfo.skillIndex, skillinfo);

        //skillinfo.skillIndex = 1;
        //skillinfo.name = "방패막기";
        //skillinfo.iconName = "Block";
        //skillinfo.description = "방패를 들어 전방으로부터의 공격을 방어합니다. 방어 가능한 최대 피해량은 장비의 성능에 따라 달라집니다. 스킬 버튼을 누르고 있는 동안에만 효과가 발휘.";
        //skillinfo.level = 1;
        //skillinfo.coolTime = 8f;
        //skillinfo.attack = 0f;
        //skillinfo.mp = -6f;
        //skillinfo.angle = 90f;
        //skillinfo.distance = 5f;

        //skillInfos.Add(skillinfo.skillIndex, skillinfo);

        //skillinfo.skillIndex = 2;
        //skillinfo.name = "긴급 회피";
        //skillinfo.iconName = "Tumbling";
        //skillinfo.description = "재빠르게 몸을 날려 적의 공격을 회피합니다.";
        //skillinfo.level = 10;
        //skillinfo.coolTime = 5f;
        //skillinfo.attack = 0f;
        //skillinfo.mp = -100f;
        //skillinfo.angle = 0f;
        //skillinfo.distance = 0f;

        //skillInfos.Add(skillinfo.skillIndex, skillinfo);

        //skillinfo.skillIndex = 3;
        //skillinfo.name = "난폭한 돌진";
        //skillinfo.iconName = "Rush";
        //skillinfo.description = "전방으로 빠르게 돌진하여 부딪히는 적들을 밀고 나갑니다.";
        //skillinfo.level = 20;
        //skillinfo.coolTime = 10f;
        //skillinfo.attack = 0.3f;
        //skillinfo.mp = -50f;
        //skillinfo.angle = 90f;
        //skillinfo.distance = 2f;

        //skillInfos.Add(skillinfo.skillIndex, skillinfo);

        //skillinfo.skillIndex = 4;
        //skillinfo.name = "압도";
        //skillinfo.iconName = "Overpower";
        //skillinfo.description = "전방을 향해 점프하여 검를 땅으로 내려찍어 공격을 가하고 다운 시킵니다.";
        //skillinfo.level = 30;
        //skillinfo.coolTime = 10f;
        //skillinfo.attack = 5f;
        //skillinfo.mp = -50f;
        //skillinfo.angle = 90f;
        //skillinfo.distance = 5f;

        //skillInfos.Add(skillinfo.skillIndex, skillinfo);
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
        string path = pathForDocumentsFile("Tables/skill_table.csv");

        if (File.Exists(path) == false)
        {
            Debug.Log("파일이 존재하지 않습니다!!" + path);
            return;
        }

        string str = null;

        skillInfos.Clear();

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

                SetSkillDate(index, ref temp);

                Debug.Log("skill_table에 데이타 등록 : " + temp[0]);
            } // end foreach
        } // end while

        sr.Close();
        file.Close();

        Debug.Log("파일 읽기 완료 : " + path);

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
}