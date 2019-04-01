using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using File_IO;

public class Player_Info_Manager : MonoBehaviour {

    public class Player_Infomation {

        private string _ID = "";
        private string name = "";

        private int level;
        private float exp;

        private int gold;
        private int cash;

        //5차 목표 즈음부터 고려할 것
        //플레이어가 장착한 스킨
        private string playerSkinID = "";

        //위 변수들에 대한 Getter
        public string _GET_Player_ID { get { return _ID; } }
        public string _GET_Player_Name { get { return name; } }
        public int _GET_Player_Level { get { return level; } }
        public float _GET_Player_Exp { get { return exp; } }
        public int _GET_Player_Gold { get { return gold; } }
        public int _GET_Player_Cash { get { return cash; } }
        public string _GET_Player_SkinID { get { return playerSkinID; } }

        //name에 대한 Setter
        public string _SET_Player_Name { set { name = value; } }

        public void Set_Player_Infomation(List<string> playerInfomation)
        {
            _ID = playerInfomation[0];
            name = playerInfomation[1];

            level = int.Parse(playerInfomation[10]);
            exp = float.Parse(playerInfomation[11]);

            gold = int.Parse(playerInfomation[12]);
            cash = int.Parse(playerInfomation[13]);

            playerSkinID = playerInfomation[14];

            ////모두 정상적으로 작동함
            //Debug.Log("name: " + name);
            //Debug.Log("level: " + level);
            //Debug.Log("exp: " + exp);
            //Debug.Log("gold: " + gold);
            //Debug.Log("cash: " + cash);
            //Debug.Log("playerSkinID: " + playerSkinID);
        }

        //gold 또는 cash로 거래가 이루어질 때 gold 또는 cash값을 변화시키는 함수
        public void __Buy_OR_Sell__About_Money(string whichMoney, int amount)
        {
            if (whichMoney == "Gold") gold += amount;
            else if (whichMoney == "Cash") cash += amount;
            else
            { }
        }
    }

    private Player_Infomation playerInfo = new Player_Infomation();
    public Player_Infomation __GET_playerInfo { get { return playerInfo; } }
    private PlayerController playerController;

    void Awake()
    {
        playerController = GameObject.Find("SamplePlayer").GetComponent<PlayerController>();

        Read_Player_Info();
        
    }

    //File_IO를 실험하기 위한 임시 코드만 작성
    void Start()
    {
        //Debug용
        //정상 작동함
        //Write_Player_Info();

        //플레이어가 장착한 스킬들 모두 정상적으로 출력됨.
        //20190118 다음엔 장착된 스킬들의 정보를 PlayerController에 직접 반영할 것
        //List<SkillBaseStat> hello = Read_SkillBaseStat();

        //hello[0].Sample__ReadAll();
        //hello[1].Sample__ReadAll();
        //hello[2].Sample__ReadAll();
        //List<string> output = Read_EquippedSkills();

        //Debug.Log("What");
        //작동 확인을 위한 더미 데이터
        //string[,] samplesmpale =
        //    { { "hello", "Will", "it", "work?" }, { "3","5","2","7" } };

        //일단 쓰는 것만 해보자
        //IO_CSV.Writer_CSV("/IWannaPlayGame.csv", samplesmpale);
        //읽는 방식
        //output = IO_CSV.Reader_CSV("/Sample__SkillDataBase.csv");

        //Excel로 CSV를 열람했을 때 각 셀 구분은 ","로 하고 줄 단위로 따로따로 읽는 것이 확인되었음.
        //foreach (string outstring in output)
        //{
        //    Debug.Log("output: " + outstring);
        //}
    }

    //플레이어 스탯과 각종 정보들을 불러오는 함수
    private void Read_Player_Info()
    {
        List<string> playerInfoList = IO_CSV.__Get_pieces_OF_BaseStrings( IO_CSV.Reader_CSV("/Sample__Player_Info.csv")[0] );
        
        playerInfo.Set_Player_Infomation(playerInfoList);
        playerController.__PLY_Stat.Initialize_Player_Stat(playerInfoList);
    }

    //플레이어 스탯과 각종 정보들을 저장하는 함수
    public void Write_Player_Info()
    {
        string[,] playerInfoString = { {"ID", "Name", "Move Speed", "Rotate Speed", "Health", "Mana", "Power", "Damage", "Critical Rate", "Critical Point", "Level", "Exp", "Gold", "Cash", "Equipped Skin"},
                                        {"", "", "", "", "", "", "", "", "", "", "", "", "", "", ""} };

        ////값이 반영됨. 저장 성공
        ////제대로 저장되는 지 확인하기 위한 코딩
        //playerInfo._SET_Player_Name = "Writing Success?";
        //playerInfo.__Buy_OR_Sell__About_Money("Gold", 1);

        //ID와 이름
        playerInfoString[1, 0] = playerInfo._GET_Player_ID;
        playerInfoString[1, 1] = playerInfo._GET_Player_Name;
        //이동속도, 회전속도
        playerInfoString[1, 2] = playerController.__PLY_Stat.__GET_FOriginalMoveSpeed.ToString();
        playerInfoString[1, 3] = playerController.__PLY_Stat.__GET_FOriginalRotateSpeed.ToString();
        //체력, 마나, 파워
        playerInfoString[1, 4] = playerController.__PLY_Stat.__GET_Max_HP.ToString();
        playerInfoString[1, 5] = playerController.__PLY_Stat.__GET_Max_MP.ToString();
        playerInfoString[1, 6] = playerController.__PLY_Stat.__GET_Max_PP.ToString();
        //공격력, 크리티컬 확률, 크리티컬 계수
        playerInfoString[1, 7] = playerController.__PLY_Stat.__PUB_ATK__Val.ToString();
        playerInfoString[1, 8] = playerController.__PLY_Stat.__PUB_Critical_Rate.ToString();
        playerInfoString[1, 9] = playerController.__PLY_Stat.__PUB_Critical_P.ToString();
        //레벨, 경험치
        playerInfoString[1, 10] = playerInfo._GET_Player_Level.ToString();
        playerInfoString[1, 11] = playerInfo._GET_Player_Exp.ToString();
        //골드와 캐쉬
        playerInfoString[1, 12] = playerInfo._GET_Player_Gold.ToString();
        playerInfoString[1, 13] = playerInfo._GET_Player_Cash.ToString();
        //장착 중인 스킨 ID
        playerInfoString[1, 14] = playerInfo._GET_Player_SkinID;

        IO_CSV.Writer_CSV("/Sample__Player_Info.csv", playerInfoString);
    }

    //Read_EquippedSkills()에서 읽은 ID를 이용하여 Sample__SkilLDataBase.csv에서
    //해당되는 ID값의 SkillStat만 resultSkill에 넣어 반환할 것
    public static List<SkillBaseStat> Read_Equipped_SkillBaseStat()
    {
        List<SkillBaseStat> resultSkills = new List<SkillBaseStat>();
        List<string> equippedSkills = IO_CSV.Reader_CSV("/Sample__PlayerEquippedInfo.csv");

        //나중에 Exception 처리할 것
        if (equippedSkills[0] == "FileNotFoundException") Debug.Log("Read_Equipped_SkillBaseStat: " + equippedSkills[0]+ ", Sample__PlayerEquippedInfo.csv");

        //필요없는 정보는 지운다. Reader_CSV에서 직접 지우도록 변경됨
        //equippedSkills.Remove("Skill_ID");

        //플레이어가 장착하고 있는 스킬들의 정보를 읽어온다.
        for (int i = 0; i < equippedSkills.Count; i++)
        {
            resultSkills.Add(IO_CSV.__Get_Searched_SkillBaseStat(equippedSkills[i]));
        }

        return resultSkills;
    }

    //Player_Info에 저장될 내용에 따라 해당 알고리즘은 변할 수 있음.
    public static void Write_Equipped_SkillBaseStat(List<SkillBaseStat> equippedSkills)
    {
        string[,] writer = { { "Skill_ID"}, { "" }, { "" }, { "" } };

        //일단 최대 장착가능한 스킬만큼 저장한다.
        for (int i = 0; i < 3; i++)
        {
            try
            {
                writer[(i + 1), 0] = equippedSkills[i].__Get_Skill_ID;
            }
            //2개 이하의 스킬만 장착되어 있는 경우
            catch (System.ArgumentOutOfRangeException)
            {
                //비어있음을 알린다.
                writer[(i + 1), 0] = "NULL_ID";
            }
        }

        IO_CSV.Writer_CSV("/Sample__PlayerEquippedInfo.csv", writer);
    }
}
