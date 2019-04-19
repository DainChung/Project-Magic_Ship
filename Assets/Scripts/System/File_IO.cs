using UnityEngine;
using System;

using System.Data;
using System.IO;

using System.Collections;
using System.Collections.Generic;

using Mono.Data.Sqlite;

using PMS_AISystem;

//using SkillBaseCode;



namespace File_IO {

    public class IO_CSV {

        //기본 함수 및 변수===================================================================================================

        //임시로 지정된 파일들 경로
        //프로토타입 완성 후 경로들 다양하게 바꿀 것
        private static string filePath = Application.persistentDataPath;

        //파일의 처음부터 읽는 함수
        //fileName은 "/어떠어떠한_파일.csv" 형식으로 입력해야됨
        public static List<string> Reader_CSV(string fileName)
        {
            //읽은 내용 중 쓸모없는 내용을 제거하기 위해 필요한 변수들
            string dummyString = ",,,,,,,,,,,,,,,";

            List<string> readList = new List<string>();

            //filePath뒤에 읽을 파일이름을 더한다.
            fileName = filePath + fileName;

            try
            {
                //CSV파일 읽기
                var reader = new StreamReader(File.OpenRead(fileName), System.Text.Encoding.Default);


                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    readList.Add(line);
                }

                //CSV 파일 줄이 바뀌기 전에 있는 ','가 여러 개 찍혀있는 지점을 제거하거나
                //NULL_ID를 readList에서 완전히 제외하는 반복문

                //foreach문에서는 List.Remove()가 InvalidOperationException을 발생시키므로 for문에서 작동해야함.
                for (int index = readList.Count - 1; index > 0; index--)
                {
                    //각 searchList마다 dummyString과 같은 내용들을 모두 제거하고 덮어씌운다.
                    try
                    {
                        readList[index] = readList[index].Remove(readList[index].IndexOf(dummyString), dummyString.Length);
                    }
                    //ArgumentOutOfRangeException이 발생하면 dummyString과 같은 내용이 없다는 것이므로 아무것도 하지 않는다.
                    //File_IO에 의해 새로 쓰여진 CSV 파일의 경우 dummyString이 발견되지 않는다.
                    catch (ArgumentOutOfRangeException)
                    { }

                    //읽은 값이 NULL_ID인 경우 아예 readList에 반영하지 않는다.
                    if (readList[index] == "NULL_ID")
                    {
                        readList.Remove(readList[index]);
                    }
                }

                reader.Close();

                //CSV 파일 맨 첫번째 줄 제거
                readList.Remove(readList[0]);

            }
            catch (FileNotFoundException)
            {
                Debug.Log("FileNotFoundException: You Need " + fileName);
                readList[0] = "FileNotFoundException";
            }

            return readList;
        }

        //한 줄만 수정하기 위해 첫 줄까지 읽어오는 함수
        //fileName은 "/어떠어떠한_파일.csv" 형식으로 입력해야됨
        public static List<string> Reader_CSV_WITH_FirstLine(string fileName)
        {
            //읽은 내용 중 쓸모없는 내용을 제거하기 위해 필요한 변수들
            string dummyString = ",,,,,,,,,,,,,,,";

            List<string> readList = new List<string>();

            //filePath뒤에 읽을 파일이름을 더한다.
            fileName = filePath + fileName;

            try
            {
                //CSV파일 읽기
                var reader = new StreamReader(File.OpenRead(fileName), System.Text.Encoding.Default);


                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    readList.Add(line);
                }

                //CSV 파일 줄이 바뀌기 전에 있는 ','가 여러 개 찍혀있는 지점을 제거하거나
                //NULL_ID를 readList에서 완전히 제외하는 반복문

                //foreach문에서는 List.Remove()가 InvalidOperationException을 발생시키므로 for문에서 작동해야함.
                for (int index = readList.Count - 1; index > 0; index--)
                {
                    //각 searchList마다 dummyString과 같은 내용들을 모두 제거하고 덮어씌운다.
                    try
                    {
                        readList[index] = readList[index].Remove(readList[index].IndexOf(dummyString), dummyString.Length);
                    }
                    //ArgumentOutOfRangeException이 발생하면 dummyString과 같은 내용이 없다는 것이므로 아무것도 하지 않는다.
                    //File_IO에 의해 새로 쓰여진 CSV 파일의 경우 dummyString이 발견되지 않는다.
                    catch (ArgumentOutOfRangeException)
                    { }

                    //읽은 값이 NULL_ID인 경우 아예 readList에 반영하지 않는다.
                    if (readList[index] == "NULL_ID")
                    {
                        readList.Remove(readList[index]);
                    }
                }

                reader.Close();
            }
            catch (FileNotFoundException)
            {
                Debug.Log("FileNotFoundException: You Need " + fileName);
                readList[0] = "FileNotFoundException";
            }

            return readList;
        }


        //파일의 한 줄만 수정하고 덮어써버리는 함수
        //fileName은 "/어떠어떠한_파일.csv" 형식으로 입력해야됨
        public static void Writer_CSV(string fileName, string[] savingData)
        {
            List<string> readData_0 = Reader_CSV_WITH_FirstLine(fileName);

            List<string> readData = __Get_pieces_OF_BaseStrings(readData_0[0]);
            List<string> readData_1 = __Get_pieces_OF_BaseStrings(readData_0[2]);
            List<string> readData_2 = __Get_pieces_OF_BaseStrings(readData_0[3]);
            readData_0 = __Get_pieces_OF_BaseStrings(readData_0[1]);

            string[,] newData = new string[4,21];

            for (int y = 0; y <= newData.GetUpperBound(1); y++)
            {
                newData[0, y] = readData[y];

                if (int.Parse(savingData[0]) == 0){ newData[1, y] = savingData[y];}
                else{   newData[1, y] = readData_0[y];}

                if (int.Parse(savingData[0]) == 1){ newData[2, y] = savingData[y];}
                else{   newData[2, y] = readData_1[y];}

                if (int.Parse(savingData[0]) == 2){ newData[3, y] = savingData[y];}
                else{   newData[3, y] = readData_2[y];}
            }

            Writer_CSV(fileName, newData);
        }

        //파일의 처음부터 덮어써버리는 함수 (입력할 내용이 많으면 일부분이 입력되지 않을 수가 있음)
        //fileName은 "/어떠어떠한_파일.csv" 형식으로 입력해야됨
        public static void Writer_CSV(string fileName, string[,] savingData)
        {
            //Debug.Log(fileName);
            fileName = filePath + fileName;
            //Debug.Log(fileName);

            string textWriter = "";
            int upperBound = savingData.GetUpperBound(1);

            //x는 행
            for (int x = 0; x <= savingData.GetUpperBound(0); x++)
            {
                //y는 열
                for (int y = 0; y <= savingData.GetUpperBound(1); y++)
                {
                    //한 행의 내용을 먼저 입력하고
                    textWriter += savingData[x, y];
                    if (y != upperBound) textWriter += ",";

                    //Debug.Log(textWriter);
                }
                //다음 행으로 넘어간다.
                textWriter += Environment.NewLine;
            }

            //Debug.Log("Fin: "+ textWriter);

            File.WriteAllText(fileName, textWriter);
        }

        //특정한 내용을 검색해서 바로 반환하는 함수
        private static string SearchString_In_File(string fileName, string wannaSearch)
        {
            List<string> readList = Reader_CSV(fileName);
            string foundString = "";

            //파일의 모든 내용을 읽은 다음 찾고자 하는 단어가 있는 string을 반환한다.
            foreach (string line in readList)
            {
                //Debug.Log("?????: " + line + ", HelloWhatThe: " + wannaSearch);
                if (line.Contains(wannaSearch))
                {
                    //Debug.Log("Hello: " + wannaSearch + ", WhatThe: " + line);
                    foundString = line;
                    //하나만 찾고 바로 탈출한다.
                    break;
                }
            }

            //원하는 내용을 찾지 못했을 때
            if (foundString == "")
            {
                //오류 문구로 변환한다.
                //나중에 namespace PMS_Exception을 만들 때 상수 str로 넣어두고 try catch를 활용할 것
                foundString = "CANNOT_FIND";
            }

            return foundString;
        }

        //,로 구분된 정보들을 List<string>에 나누어 넣어서 반환하는 1차 가공 함수
        public static List<string> __Get_pieces_OF_BaseStrings(string baseString)
        {
            List<string> pieces_OF_BaseString = new List<string>();
            string piece = "";

            //작업을 편하게 하기 위한 string 수정
            baseString += ',';

            //1차 가공
            while (baseString != "")
            {
                //Debug.Log("baseString: " + baseString);
                //문자열 맨 처음에서부터 가장 가까운 ','까지 piece로 복사한다.
                piece = baseString.Substring(0, baseString.IndexOf(','));
                //Debug.Log("piece:" + piece);
                //복사한 문자열을 pieces_OF_BaseString에 넣는다.
                pieces_OF_BaseString.Add(piece);

                //pieces_OF_BaseString에 추가된 내용과 ','을 baseString에서 뺸다.
                baseString = baseString.Remove(0, baseString.IndexOf(',') + 1);
            }

            return pieces_OF_BaseString;
        }

        //==================================================================================================================

        //==================================================================================================================
        //SkillBaseStat 관련 읽기 함수=======================================================================================

        //찾은 내용을 게임 내에서 사용할 수 있도록 가공하는 함수(SkillBaseStat한정)
        public static SkillBaseStat __Get_Searched_SkillBaseStat(string wannaSearch)
        {

            //Debug.Log("WannaSearch: "+wannaSearch);
            //가공이 되지 않은 SkillBaseStat자료를 가공함수에 투입하고 그걸 그대로 반환
            //아래에 작성된 __Get_All_SkillBaseStat 함수로 인해 이 함수를 이 형태로 유지할 것
            //다른 좋은 생각 있으면 추후 수정 요구
            return Set_ResultStat(SearchString_In_File("/SkillDataBase.csv", wannaSearch));
        }
        
        //string을 SkillBaseStat으로 가공하는 함수
        private static SkillBaseStat Set_ResultStat(string baseStatString)
        {
            SkillBaseStat resultStat = new SkillBaseStat();

            //중간중간의 ','를 제외하고 따로따로 저장한 1차 가공
            List<string> pieces_OF_BaseStatString = new List<string>();

            if (baseStatString != "CANNOT_FIND")
            {
                //1차 가공
                pieces_OF_BaseStatString = __Get_pieces_OF_BaseStrings(baseStatString);

                //2차 가공
                float rate = float.Parse(pieces_OF_BaseStatString[2]);
                float coolTime = float.Parse(pieces_OF_BaseStatString[3]);
                float ingTime = float.Parse(pieces_OF_BaseStatString[4]);
                int amount = int.Parse(pieces_OF_BaseStatString[5]);

                SkillBaseCode.SkillCode skillCode = Get_SkillCode_FROM_String(pieces_OF_BaseStatString[6], pieces_OF_BaseStatString[7], pieces_OF_BaseStatString[8]);

                List<int> isItLocked = new List<int>();
                isItLocked.Add(int.Parse(pieces_OF_BaseStatString[9]));
                isItLocked.Add(int.Parse(pieces_OF_BaseStatString[10]));
                isItLocked.Add(int.Parse(pieces_OF_BaseStatString[11]));

                //가공된 내용들을 resultStat에 넣어서 최종 정리한다.
                resultStat.Initialize_Skill(pieces_OF_BaseStatString[1], rate, coolTime, ingTime, amount, skillCode, pieces_OF_BaseStatString[0], isItLocked);
            }
            else
            {
                SkillBaseCode.SkillCode nullCode = Get_SkillCode_FROM_String("FIN", "NULL", "NULL");

                List<int> errorLocked = new List<int>();
                errorLocked.Add(-1);
                errorLocked.Add(-1);
                errorLocked.Add(-1);

                resultStat.Initialize_Skill("NULL", 0,0,0,0, nullCode, "NULL", errorLocked);
            }

            return resultStat;
        }

        //string을 SkillCode의 enum 자료형들로 변환하는 함수
        private static SkillBaseCode.SkillCode Get_SkillCode_FROM_String(string str_FOR_Main, string str_FOR_Sub, string str_FOR_Time)
        {
            SkillBaseCode.SkillCode result = new SkillBaseCode.SkillCode();

            //_SKILL_CODE_Main의 값을 지정하기
            if (str_FOR_Main == "BUF")
                result._Skill_Code_M = SkillBaseCode._SKILL_CODE_Main.BUF;
            else if (str_FOR_Main == "DBF")
                result._Skill_Code_M = SkillBaseCode._SKILL_CODE_Main.DBF;
            else if (str_FOR_Main == "ATK")
                result._Skill_Code_M = SkillBaseCode._SKILL_CODE_Main.ATK;
            else if (str_FOR_Main == "FIN")
                result._Skill_Code_M = SkillBaseCode._SKILL_CODE_Main.FIN;
            else if (str_FOR_Main == "SPW")
                result._Skill_Code_M = SkillBaseCode._SKILL_CODE_Main.SPW;
            //오류
            else
            {}

            //_SKILL_CODE_Sub의 값을 지정하기
            if (str_FOR_Sub == "NULL")
                result._Skill_Code_S = SkillBaseCode._SKILL_CODE_Sub.NULL;
            else if (str_FOR_Sub == "HP")
                result._Skill_Code_S = SkillBaseCode._SKILL_CODE_Sub.HP;
            else if (str_FOR_Sub == "MP")
                result._Skill_Code_S = SkillBaseCode._SKILL_CODE_Sub.MP;
            else if (str_FOR_Sub == "PP")
                result._Skill_Code_S = SkillBaseCode._SKILL_CODE_Sub.PP;
            else if (str_FOR_Sub == "SP")
                result._Skill_Code_S = SkillBaseCode._SKILL_CODE_Sub.SP;
            else if (str_FOR_Sub == "MOS")
                result._Skill_Code_S = SkillBaseCode._SKILL_CODE_Sub.MOS;
            //오류
            else
            { }

            //_SKILL_CODE_Time의 값 지정하기
            if (str_FOR_Time == "NULL")
                result._Skill_Code_T = SkillBaseCode._SKILL_CODE_Time.NULL;
            else if (str_FOR_Time == "FREQ")
                result._Skill_Code_T = SkillBaseCode._SKILL_CODE_Time.FREQ;
            //오류
            else
            { }


            return result;
        }

        //SkillDB의 모든 내용을 읽고 모두 SkillBaseStat으로 가공해서 반환하는 함수
        public static List<SkillBaseStat> __Get_All_SkillBaseStat()
        {
            List<SkillBaseStat> allSkills = new List<SkillBaseStat>();
            List<string> base_AllSkills = Reader_CSV("/SkillDataBase.csv");

            //각 줄마다 SkillBaseStat으로 변경하여 allSkills에 저장
            foreach (string base_SkillString in base_AllSkills)
            {
                //읽은 내용을 하나씩 추가
                allSkills.Add(Set_ResultStat(base_SkillString));
            }

            return allSkills;
        }

        //==================================================================================================================

        //==================================================================================================================
        //Enemy의 Unit__Base_Stat 관련 읽기 함수=============================================================================

        //외부에서 특정 Enemy의 스탯을 읽어오기 위한 함수, 찾을 Enemy의 ID값을 넣어서 찾는다.
        //읽은 정보를 가공하는 건 EnemyStat에서 직접 수행한다.
        public static List<string> __Get_Searched_EnemyBaseStat(string wannaSearch)
        {
            return __Get_pieces_OF_BaseStrings(SearchString_In_File("/Enemy_Stat_DataBase.csv", wannaSearch));
        }

        //==================================================================================================================
    }

    public class IO_SqlDB {

        private static string dbPath = Application.persistentDataPath;

        //fileName은 "BehaveDB" 이런식으로만 입력할 것 (예상 실제 DB명 == "BehaveDB15.3sdb", 행동을 시작할 때 거리가 15이상 16미만일 때의 DB)
        //현재 거리, 현재 각도와 가장 유사했을 때 가장 좋았던 행동들을 출력하는 함수
        //아직 실험할 단계가 아님
        public static Vector3 Search_Doing_IN_DB(string fileName, float posX, float posZ, float eneTOplyDist, float eneTOplyAngle)
        {
            //fileName = @"Data Source=" + dbPath + "/" + fileName + ((int)eneTOplyDist) + ".db";
            fileName = @"Data Source=" + dbPath + "/" + fileName + ".db";

            var dbConnection = new SqliteConnection(fileName);

            dbConnection.Open();

            IDbCommand dbCommand = dbConnection.CreateCommand();

            //"SELECT 조회할 칼럼 FROM 조회할 테이블"
            //CUR_TRANSFORM => CURTrn_id, CURTrn_enePosX, CURTrn_enePosZ, CURTrn_dist, CURTrn_angle
            string sqlQuery = "SELECT * FROM CUR_TRANSFORM";
            dbCommand.CommandText = sqlQuery;

            IDataReader reader = dbCommand.ExecuteReader();

            //행동만 출력
            //result.x == -1이면 DB에 적합한 행동이 없었음을 의미
            Vector3 result = new Vector3(-1, -1, -1);
            //result._SET_Situation("", enemyTransform, new Vector3(0,0,0), "", new Vector3(0,0,0), false, false);

            string id = "";
            float minX, minZ, minDist, minAngle;

            minX = 999.0f;
            minZ = 999.0f;
            minDist = 99.0f;
            minAngle = 100.0f;
            

            while (reader.Read())
            {
                float CURTrn_enePosX = reader.GetFloat(1);
                float CURTrn_enePosZ = reader.GetFloat(2);
                float CURTrn_dist = reader.GetFloat(3);
                float CURTrn_angle = reader.GetFloat(4);

                //현재 상태(beforeDoing)의 위치, 각도, 거리에 가장 근접한 데이터를 찾을 때마다
                if (Mathf.Abs(CURTrn_dist - eneTOplyDist) <= minDist && Mathf.Abs(CURTrn_angle - eneTOplyAngle) <= minAngle
                    && Mathf.Abs(CURTrn_enePosX - posX) <= minX && Mathf.Abs(CURTrn_enePosZ - posZ) <= minZ)
                {
                    //값을 업데이트 해준다.
                    minX = Mathf.Abs(CURTrn_enePosX - posX);
                    minZ = Mathf.Abs(CURTrn_enePosZ - posZ);
                    minDist = Mathf.Abs(CURTrn_dist - eneTOplyDist);
                    minAngle = Mathf.Abs(CURTrn_angle - eneTOplyAngle);
                    id = reader.GetString(0);
                }
            }

            //닫고
            reader.Close();

            //string id를 이용하여 과거 수행했던 행동 중 최적의 행동을 반환한다.
            //AFT_Doing => AFTDo_id, AFTDo_Mov, AFTDo_Rot, AFTDo_Atk
            sqlQuery = "SELECT * FROM AFT_Doing";
            dbCommand.CommandText = sqlQuery;
            reader = dbCommand.ExecuteReader();

            while (reader.Read())
            {
                if (reader.GetString(0) == id)
                {
                    result = new Vector3(reader.GetFloat(1), reader.GetFloat(2), reader.GetFloat(3));
                    break;
                }
            }

            //닫아주고 초기화
            reader.Close();
            reader = null;
            dbCommand.Dispose();
            dbCommand = null;
            dbConnection.Close();
            dbConnection = null;

            return result;
        }

        //Situation을 사용할 가능성이 적음
        public static void WriteDB(string fileName, Situation situation)
        {

        }

        //이 함수를 사용할 가능성이 큼
        //이것 먼저 처리해야 됨
        //dataID = (행동 시작 시점에서의 System.DateTime.Now) + ": " + (gameObject.GetInstanceID);
        //_isPlayerGetHit이 bool에서 명중한 횟수로 변경될 수 있음
        //CUR전용 함수
        public static void WriteDB_CUR(string fileName, string _dataID, Vector3 _enePos, float _ene_TO_plyDist, float _ene_TO_plyAngles, Vector3 _doing, int _isPlayerGetHitCount, bool _isPlayerGettingCloser, bool _isHitTOBoundary)
        {
            /* 예상 데이터 분류 (()안은 AD, BD가 들어갈 자리 => id로 AD, BD를 구분할 필요 없음)
             * TABLE: CUR_Transform, AFT_Transform, CUR_Doing, AFT_Doing, CUR_Bools, AFT_Bools
             * ()_Transform => ()Trn_id, ()Trn_enePosX, ()Trn_enePosZ, ()Trn_dist, ()Trn_angles
             * ()_Doing => ()Do_id, ()Do_Mov, ()Do_Rot, ()Do_Atk
             * ()_Bools => ()Bo_id, ()Bo_plyGetHit, ()Bo_plyCloser, ()Bo_hitBoundary
             */
            //테이블 총 6개

            //fileName = @"Data Source=" + dbPath + "/" + fileName + ((int)_ene_TO_plyDist) + ".db";
            fileName = @"Data Source=" + dbPath + "/" + fileName + ".db";

            using (var dbConnection = new SqliteConnection(fileName))
            {

                dbConnection.Open();

                using (IDbCommand dbCommand = dbConnection.CreateCommand())
                {

                    //CUR_Transform 테이블에 대한 Query문 준비
                    string sqlQuery = "INSERT INTO CUR_Transform (CUR_Trn_id, CUR_Trn_posX, CUR_Trn_posZ, CUR_Trn_dist, CUR_Trn_angle) VALUES (@id, @posX, @posZ, @_ene_TO_plyDist, @_ene_TO_plyAngles);";

                    dbCommand.CommandText = sqlQuery;

                    //파라미터 입력
                    dbCommand.Parameters.Add(new SqliteParameter("@id", _dataID));
                    dbCommand.Parameters.Add(new SqliteParameter("@posX", _enePos.x));
                    dbCommand.Parameters.Add(new SqliteParameter("@posZ", _enePos.z));
                    dbCommand.Parameters.Add(new SqliteParameter("@_ene_TO_plyDist", _ene_TO_plyDist));
                    dbCommand.Parameters.Add(new SqliteParameter("@_ene_TO_plyAngles", _ene_TO_plyAngles));

                    dbCommand.ExecuteNonQuery();

                    //CUR_Doing 테이블에 대한 Query문 준비
                    sqlQuery = "INSERT INTO CUR_Doing (CUR_Do_id, CUR_Do_mov, CUR_Do_rot, CUR_Do_atk) VALUES (@id, @_doingMov, @_doingRot, @_doingAtk);";

                    dbCommand.CommandText = sqlQuery;

                    //파라미터 입력
                    dbCommand.Parameters.Add(new SqliteParameter("@id", _dataID));
                    dbCommand.Parameters.Add(new SqliteParameter("@_doingMov", (int)(_doing.x)));
                    dbCommand.Parameters.Add(new SqliteParameter("@_doingRot", (int)(_doing.y)));
                    dbCommand.Parameters.Add(new SqliteParameter("@_doingAtk", (int)(_doing.z)));

                    dbCommand.ExecuteNonQuery();

                    //CUR_Bools 테이블에 대한 Query문 준비
                    sqlQuery = "INSERT INTO CUR_Bools (CUR_Bo_id, CUR_Bo_getHitCount, CUR_Bo_gettingCloser, CUR_Bo_hitBoundary)VALUES (@id, @_isPlayerGetHitCount, @getCloser, @hitBound);";

                    int getCloser = 0;
                    int hitBound = 0;

                    if (_isPlayerGettingCloser) getCloser = 1;
                    if (_isHitTOBoundary) hitBound = 1;

                    dbCommand.CommandText = sqlQuery;

                    //파라미터 입력
                    dbCommand.Parameters.Add(new SqliteParameter("@id", _dataID));
                    dbCommand.Parameters.Add(new SqliteParameter("@_isPlayerGetHitCount", _isPlayerGetHitCount));
                    dbCommand.Parameters.Add(new SqliteParameter("@getCloser", getCloser));
                    dbCommand.Parameters.Add(new SqliteParameter("@hitBound", hitBound));

                    dbCommand.ExecuteNonQuery();

                    dbCommand.Dispose();
                }
                dbConnection.Close();
            }
        }

        //AFT전용 함수
        public static void WriteDB_AFT(string fileName, string _dataID, Vector3 _enePos, float _ene_TO_plyDist, float _ene_TO_plyAngles, Vector3 _doing, int _isPlayerGetHitCount, bool _isPlayerGettingCloser, bool _isHitTOBoundary)
        {
            /* 예상 데이터 분류 (()안은 AD, BD가 들어갈 자리 => id로 AD, BD를 구분할 필요 없음)
             * TABLE: CUR_Transform, AFT_Transform, CUR_Doing, AFT_Doing, CUR_Bools, AFT_Bools
             * ()_Transform => ()Trn_id, ()Trn_enePosX, ()Trn_enePosZ, ()Trn_dist, ()Trn_angles
             * ()_Doing => ()Do_id, ()Do_Mov, ()Do_Rot, ()Do_Atk
             * ()_Bools => ()Bo_id, ()Bo_plyGetHit, ()Bo_plyCloser, ()Bo_hitBoundary
             */
            //테이블 총 6개

            //fileName = @"Data Source=" + dbPath + "/" + fileName + ((int)_ene_TO_plyDist) + ".db";
            fileName = @"Data Source=" + dbPath + "/" + fileName + ".db";

            using (var dbConnection = new SqliteConnection(fileName))
            {

                dbConnection.Open();

                using (IDbCommand dbCommand = dbConnection.CreateCommand())
                {

                    //AFT_Transform 테이블에 대한 Query문 준비
                    string sqlQuery = "INSERT INTO AFT_Transform (AFT_Trn_id, AFT_Trn_posX, AFT_Trn_posZ, AFT_Trn_dist, AFT_Trn_angle) VALUES (@id, @posX, @posZ, @_ene_TO_plyDist, @_ene_TO_plyAngles);";

                    dbCommand.CommandText = sqlQuery;

                    //파라미터 입력
                    dbCommand.Parameters.Add(new SqliteParameter("@id", _dataID));
                    dbCommand.Parameters.Add(new SqliteParameter("@posX", _enePos.x));
                    dbCommand.Parameters.Add(new SqliteParameter("@posZ", _enePos.z));
                    dbCommand.Parameters.Add(new SqliteParameter("@_ene_TO_plyDist", _ene_TO_plyDist));
                    dbCommand.Parameters.Add(new SqliteParameter("@_ene_TO_plyAngles", _ene_TO_plyAngles));

                    //Query 전송 및 수행
                    dbCommand.ExecuteNonQuery();

                    //AFT_Doing 테이블에 대한 Query문 준비
                    sqlQuery = "INSERT INTO AFT_Doing (AFT_Do_id, AFT_Do_mov, AFT_Do_rot, AFT_Do_atk) VALUES (@id, @_doingMov, @_doingRot, @_doingAtk);";

                    dbCommand.CommandText = sqlQuery;

                    //파라미터 입력
                    dbCommand.Parameters.Add(new SqliteParameter("@id", _dataID));
                    dbCommand.Parameters.Add(new SqliteParameter("@_doingMov", (int)(_doing.x)));
                    dbCommand.Parameters.Add(new SqliteParameter("@_doingRot", (int)(_doing.y)));
                    dbCommand.Parameters.Add(new SqliteParameter("@_doingAtk", (int)(_doing.z)));

                    dbCommand.ExecuteNonQuery();

                    //AFT_Bools 테이블에 대한 Query문 준비
                    sqlQuery = "INSERT INTO AFT_Bools (AFT_Bo_id, AFT_Bo_getHitCount, AFT_Bo_gettingCloser, AFT_Bo_hitBoundary)VALUES (@id, @_isPlayerGetHitCount, @getCloser, @hitBound);";

                    int getCloser = 0;
                    int hitBound = 0;

                    if (_isPlayerGettingCloser) getCloser = 1;
                    if (_isHitTOBoundary) hitBound = 1;

                    dbCommand.CommandText = sqlQuery;

                    //파라미터 입력
                    dbCommand.Parameters.Add(new SqliteParameter("@id", _dataID));
                    dbCommand.Parameters.Add(new SqliteParameter("@_isPlayerGetHitCount", _isPlayerGetHitCount));
                    dbCommand.Parameters.Add(new SqliteParameter("@getCloser", getCloser));
                    dbCommand.Parameters.Add(new SqliteParameter("@hitBound", hitBound));

                    dbCommand.ExecuteNonQuery();

                    dbCommand.Dispose();
                }

                dbConnection.Close();
            }
        }

        //private static Behave Data_TO_Situation(IDataReader data)
        //{
        //    Behave result = new Behave();



        //    return result;
        //}
    }
}
