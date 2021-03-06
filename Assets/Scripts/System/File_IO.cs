﻿using UnityEngine;
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

        //20191222
        //딥러닝 용 데이터 베이스를 제외한 게임 구동에 필요한 파일들이 없을 때 자동으로 생성해주는 함수
        public static void InitFiles()
        {
            List<string> values = new List<string>();

            // 스킬, 적 스탯에 대한 파일은 기본적으로 제공해야 한다.
            try
            {
                values = Reader_CSV("/SaveSlot.csv");

                if (string.Compare(values[0], "FileNotFoundException") == 0)
                    throw new FileNotFoundException();
            }
            //SaveSlot.csv 파일이 지정된 경로에 없다면 새로 생성하고 기본 값을 입력한다.
            catch (FileNotFoundException)
            {
                string[,] baseSaveSlotData = {  {"SlotNum","Date","ID", "Name", "Move Speed", "Rotate Speed", "Health", "Mana", "Power", "Damage", "Critical Rate", "Critical Point", "Level", "Exp", "Gold", "Cash", "Equipped Skin", "EquippedSkill0", "EquippedSkill1", "EquippedSkill2", "Stage0Lock"},
                                                {"0","NULL","1","None","10","30","30", "10", "10", "1", "0.1", "2", "1", "0", "0", "0", "Default Skin", "NORMAL_HP_00", "NORMAL_HP_01", "NORMAL_SP_00", "c000"},
                                                {"1","NULL","1","None","10","30","30", "10", "10", "1", "0.1", "2", "1", "0", "0", "0", "Default Skin", "NORMAL_HP_00", "NORMAL_HP_01", "NORMAL_SP_00", "c000"},
                                                {"2","NULL","1","None","10","30","30", "10", "10", "1", "0.1", "2", "1", "0", "0", "0", "Default Skin", "NORMAL_HP_00", "NORMAL_HP_01", "NORMAL_SP_00", "c000"}};

                Writer_CSV("/SaveSlot.csv", baseSaveSlotData);
            }
        }

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
            //파일이 없을 때
            catch (FileNotFoundException)
            {
                Debug.Log("FileNotFoundException: You Need " + fileName);
                readList.Add("FileNotFoundException");
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

        //20190606 PPT를 위한 파일 쓰기 함수
        public static void Writer_CSV(string fileName, List<int> savingINTData)
        {
            string[,] savingData = new string[savingINTData.Count, 2];
            float temp = 0;

            for (int i = 0; i < savingINTData.Count; i++)
            {
                temp = savingINTData[i];
                //Debug.Log(temp);

                savingData[i, 0] = i.ToString();
                savingData[i, 1] = temp.ToString();
            }


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

            Debug.Log("Save Score Data Done");
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

    //======================================================================================================================================

    public class IO_SqlDB {

        private static string dbPath = Application.persistentDataPath;
        private static string cacheQuery = "NULL";
        private static string infoCacheQuery = "NULL";

        //DB에서 학습이 완료된 인공신경망 중 하나만 읽기 위한 함수
        public static FCNN ReadFCNN_FROM_DB(string fileName, int angle, int dist, double time)
        {
            FCNN result = new FCNN();
            fileName = @"Data Source=" + dbPath + "/" + fileName + ".db";
            string id = "("+angle.ToString()+", "+dist.ToString() + ", "+time.ToString() + ")";
            int index = 0;

            using (var dbConnection = new SqliteConnection(fileName))
            {
                dbConnection.Open();

                using (IDbCommand dbCommand = dbConnection.CreateCommand())
                {
                    //"SELECT 조회할 칼럼 FROM 조회할 테이블"
                    string sqlQuery = "SELECT * FROM INFO";
                    dbCommand.CommandText = sqlQuery;

                    IDataReader reader = dbCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        //id가 일치하면
                        if (id.CompareTo(reader.GetString(0)) == 0)
                        {
                            result = new FCNN(reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetDouble(1));
                            for (int i = 0; i < result.outputNum; i++)
                            {
                                result.outputInfo[i] = reader.GetDouble(i+5);
                            }
                            break;
                        }
                        index++;
                    }
                    reader.Close();

                    List<List<double>> var = new List<List<double>>();
                    int startLine = index * (result.outputNum + 1) + 1, endLine = startLine + result.outputNum;

                    sqlQuery = "SELECT * FROM L1 WHERE rowid >= " + startLine + " AND rowid <= " + endLine;
                    dbCommand.CommandText = sqlQuery;

                    reader = dbCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        //id가 일치하면
                        if (id.CompareTo(reader.GetString(0)) == 0)
                        {
                            var.Add(new List<double>());

                            for (int c = 1; c < result.outputNum + 1; c++)
                                var[var.Count - 1].Add(reader.GetDouble(c));
                        }
                    }

                    try
                    {
                        result.layers[1].CopyMatrix(var);
                        reader.Close();

                        startLine = index * result.outputNum + 1;
                        endLine = startLine + (result.outputNum - 1);
                        int r = 0;
                        for (int d = 2; d < result.depth - 1; d++)
                        {
                            sqlQuery = "SELECT * FROM L" + d.ToString() + " WHERE rowid >= " + startLine + " AND rowid <= " + endLine;
                            dbCommand.CommandText = sqlQuery;

                            reader = dbCommand.ExecuteReader();
                            r = 0;
                            while (reader.Read())
                            {
                                //id가 일치하면
                                if (id.CompareTo(reader.GetString(0)) == 0)
                                {
                                    for (int c = 1; c < result.outputNum + 1; c++)
                                        var[r][c - 1] = reader.GetDouble(c);
                                }
                                r++;
                            }
                            result.layers[d].CopyMatrix(var);

                            reader.Close();
                        }

                    }
                    catch (Exception)
                    {
                        Debug.Log(id+" Not Found");
                    }
                    finally
                    {
                        reader.Close();

                        reader = null;
                        dbCommand.Dispose();
                    }
                    
                    
                }
                dbConnection.Close();
            }

            return result;
        }

        //학습이 완료된 인공신경망을 DB에 저장하기 위한 함수
        //List안에 있는 모든 FCNN의 규격은 동일한 것으로 가정한다.
        //FCNN쓰기
        public static void WriteDB_FCNN(string fileName, List<string> fcnnIDList, List<FCNN> learnedFCNN, bool isFirstWrite)
        {
            fileName = @"Data Source=" + dbPath + "/" + fileName + ".db";

            //Debug.Log(fcnnIDList.Count + ", " + learnedFCNN.Count);
            //비어있으면 작동 안 함
            if (learnedFCNN.Count == 0 || fcnnIDList.Count == 0)
                return;

            using (var dbConnection = new SqliteConnection(fileName))
            {
                dbConnection.Open();
                using (SqliteTransaction dbTranssaction = dbConnection.BeginTransaction())
                {

                    using (IDbCommand dbCommand = dbConnection.CreateCommand())
                    {
                        string sqlQuery = "";
                        string cacheQuery0 = "(id, ";
                        string cacheQuery1 = "(@id, ";
                        string infoCacheQuery0 = ", ";
                        string infoCacheQuery1 = ", ";

                        string wString = "";

                        //TABLE이 없는 상태라면
                        if (isFirstWrite)
                        {
                            //TABLE을 만들어 준다
                            sqlQuery = "CREATE TABLE INFO ( id TEXT, learningRate REAL, depth INTEGER, inputNUM INTEGER, outputNUM INTEGER, ";
                            for (int i = 0; i < learnedFCNN[0].outputNum; i++)
                            {
                                if (i == learnedFCNN[0].outputNum - 1)
                                    sqlQuery += ("out" + i.ToString() + " REAL)");
                                else
                                    sqlQuery += ("out" + i.ToString() + " REAL, ");
                            }

                            dbCommand.CommandText = sqlQuery;
                            dbCommand.ExecuteNonQuery();

                            for (int depth = 1; depth < learnedFCNN[0].layers.Count; depth++)
                            {
                                sqlQuery = "CREATE TABLE L" +depth+" ( id TEXT, ";
                                for (int i = 0; i < learnedFCNN[0].layers[1].col; i++)
                                {
                                    if (i == learnedFCNN[0].layers[1].col - 1)
                                        sqlQuery += ("w" + i.ToString() + " REAL");
                                    else
                                        sqlQuery += ("w" + i.ToString() + " REAL, ");
                                }

                                sqlQuery += ")";

                                dbCommand.CommandText = sqlQuery;
                                dbCommand.ExecuteNonQuery();
                            }
                        }

                        //자주 쓰일 Query문 중 일부분을 미리 작성
                        //(w0, w1, ..., wLast) VALUES (@w0, @w1, ... , @wLast)
                        if (cacheQuery.CompareTo("NULL") == 0)
                        {
                            //cacheQuery를 비운다.
                            cacheQuery = "";
                            infoCacheQuery = "";

                            //cacheQuery 초기화 시작
                            for (int i = 0; i < learnedFCNN[0].layers[1].col; i++)
                            {
                                if (i == learnedFCNN[0].layers[1].col - 1)
                                {
                                    cacheQuery0 += ("w" + i.ToString() + ")");
                                    cacheQuery1 += ("@w" + i.ToString() + ")");
                                    infoCacheQuery0 += ("out" + i.ToString() + ")");
                                    infoCacheQuery1 += ("@out" + i.ToString() + ")");
                                }
                                else
                                {
                                    cacheQuery0 += ("w" + i.ToString() + ", ");
                                    cacheQuery1 += ("@w" + i.ToString() + ", ");
                                    infoCacheQuery0 += ("out" + i.ToString() + ", ");
                                    infoCacheQuery1 += ("@out" + i.ToString() + ", ");
                                }
                            }
                            cacheQuery = cacheQuery0 + " VALUES " + cacheQuery1;
                            infoCacheQuery = infoCacheQuery0 + " VALUES (@id, @learningRate, @depth, @inputNUM, @outputNUM" + infoCacheQuery1;
                        }


                        for (int index = 0; index < fcnnIDList.Count; index++)
                        {
                            //Layer Info에 대한 입력
                            sqlQuery = "INSERT INTO INFO (id, learningRate, depth, inputNUM, outputNUM" + infoCacheQuery;
                            dbCommand.CommandText = sqlQuery;
                            dbCommand.Parameters.Add(new SqliteParameter("@id", fcnnIDList[index]));
                            dbCommand.Parameters.Add(new SqliteParameter("@learningRate", learnedFCNN[index].learningRate));
                            dbCommand.Parameters.Add(new SqliteParameter("@depth", learnedFCNN[index].depth));
                            dbCommand.Parameters.Add(new SqliteParameter("@inputNUM", learnedFCNN[index].inputNum));
                            dbCommand.Parameters.Add(new SqliteParameter("@outputNUM", learnedFCNN[index].outputNum));
                            for (int i = 0; i < learnedFCNN[0].outputNum; i++)
                            {
                                wString = "@out" + i.ToString();

                                dbCommand.Parameters.Add(new SqliteParameter(wString, learnedFCNN[index].outputInfo[i]));
                            }
                            dbCommand.ExecuteNonQuery();

                            //inputLayer를 제외한 모든 Layer에 대한 입력
                            //하나의 FCNN이 36 ~ 37개의 Row를 갖도록 한다...
                            for (int depth = 1; depth < learnedFCNN[index].depth; depth++)
                            {
                                if(depth%6 == 0)
                                    Debug.Log(index + ", "+depth + ": " + learnedFCNN[index].layers[depth].row);
                                for (int r = 0; r < learnedFCNN[index].layers[depth].row; r++)
                                {
                                    sqlQuery = "INSERT INTO L" + depth.ToString() + " " + cacheQuery;
                                    dbCommand.CommandText = sqlQuery;
                                    //파라미터 입력
                                    dbCommand.Parameters.Add(new SqliteParameter("@id", fcnnIDList[index]));
                                    for (int c = 0; c < learnedFCNN[index].layers[depth].col; c++)
                                    {
                                        wString = "@w" + c.ToString();

                                        dbCommand.Parameters.Add(new SqliteParameter(wString, learnedFCNN[index].layers[depth].values[r][c]));
                                    }

                                    dbCommand.ExecuteNonQuery();
                                }
                            }
                        }

                        dbCommand.Dispose();

                    }
                    dbTranssaction.Commit();
                }
                dbConnection.Close();
            }

            //Debug.Log("FCNN Write Done");
        }

        //SitCUR와 SitAFT를 학습 목적으로 읽기 위해 사용하는 함수
        public static List<AIData> ReadAIData_FROM_DB_FOR_Learning(string fileName)
        {
            List<SituationCUR> cur = new List<SituationCUR>();
            List<SituationAFT> aft = new List<SituationAFT>();

            List<AIData> result = new List<AIData>();

            fileName = @"Data Source=" + dbPath + "/" + fileName + ".db";

            int index = 0;

            using (var dbConnection = new SqliteConnection(fileName))
            {
                dbConnection.Open();

                using (IDbCommand dbCommand = dbConnection.CreateCommand())
                {
                    //"SELECT 조회할 칼럼 FROM 조회할 테이블"
                    //CUR_Transform => CURTrn_id, CURTrn_enePosX, CURTrn_enePosZ, CURTrn_dist, CURTrn_angle
                    string sqlQuery = "SELECT * FROM CUR_Transform";
                    dbCommand.CommandText = sqlQuery;

                    IDataReader reader = dbCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        string id = reader.GetString(0);
                        float CURTrn_enePosX = reader.GetFloat(1);
                        float CURTrn_enePosZ = reader.GetFloat(2);
                        float CURTrn_dist = reader.GetFloat(3);
                        float CURTrn_angle = reader.GetFloat(4);

                        int CURDo_mov = reader.GetInt32(5);
                        int CURDo_rot = reader.GetInt32(6);
                        int CURDo_atk = reader.GetInt32(7);

                        float CURBo_Time = reader.GetFloat(8);

                        cur.Add(new SituationCUR("NULL", -1f, -1f, -1f, -1f, new IntVector3(-1, -1, -1), -1f));
                        aft.Add(new SituationAFT("NULL", -1f, -1f, -1f, -1f, "NULL", -1, -1, false));

                        cur[index]._id = id;
                        cur[index]._posX = CURTrn_enePosX;
                        cur[index]._posZ = CURTrn_enePosZ;
                        cur[index]._dist = CURTrn_dist;
                        cur[index]._angleComp = CURTrn_angle;

                        cur[index]._doing = new IntVector3(CURDo_mov, CURDo_rot, CURDo_atk);

                        cur[index]._time = CURBo_Time;

                        index++;
                    }

                    reader.Close();

                    //--------------------------------------------------------------------

                    index = 0;

                    //"SELECT 조회할 칼럼 FROM 조회할 테이블"
                    sqlQuery = "SELECT * FROM AFT_Transform";
                    dbCommand.CommandText = sqlQuery;

                    reader = dbCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        string id = reader.GetString(0);
                        float AFTTrn_enePosX = reader.GetFloat(1);
                        float AFTTrn_enePosZ = reader.GetFloat(2);
                        float AFTTrn_dist = reader.GetFloat(3);
                        float AFTTrn_angle = reader.GetFloat(4);

                        int AFTBo_P = reader.GetInt32(5);
                        int AFTBo_c = reader.GetInt32(6);
                        string AFTBo_bID = reader.GetString(7);
                        int AFTBo_bD = reader.GetInt32(8);

                        bool AFTBo_C = false;

                        if (AFTBo_c == 1)
                            AFTBo_C = true;

                        aft[index]._id = id;
                        aft[index]._posX = AFTTrn_enePosX;
                        aft[index]._posZ = AFTTrn_enePosZ;
                        aft[index]._dist = AFTTrn_dist;
                        aft[index]._angleComp = AFTTrn_angle;

                        aft[index]._hitCounter = AFTBo_P;
                        aft[index]._closer = AFTBo_C;
                        aft[index]._beforeID = AFTBo_bID;
                        aft[index]._beforeDB = AFTBo_bD;

                        index++;
                    }

                    reader.Close();

                    //닫아주고 초기화
                    reader.Close();
                    reader = null;
                    dbCommand.Dispose();
                }

                dbConnection.Close();
            }

            for (int i = 0; i < cur.Count; i++)
            {
                result.Add(new AIData(cur[i], aft[i]));
            }

            return result;
        }


        public static List<AIData> ReadAIData_FROM_DB(string fileName)
        {
            int fileIndex = -1;
            List<SituationCUR> cur = new List<SituationCUR>();
            List<SituationAFT> aft = new List<SituationAFT>();

            List<AIData> result = new List<AIData>();

            if (fileName[fileName.Length - 2] == 'd' && (fileName[fileName.Length - 1] == '0' || fileName[fileName.Length - 1] == '1'))
            {
                fileIndex = (int)(fileName[fileName.Length - 1]);
                fileName = fileName.Remove(fileName.Length - 1);
                Debug.Log(fileName);
            }

            fileName = @"Data Source=" + dbPath + "/" + fileName + ".db";

            using (var dbConnection = new SqliteConnection(fileName))
            {
                dbConnection.Open();

                using (IDbCommand dbCommand = dbConnection.CreateCommand())
                {
                    //"SELECT 조회할 칼럼 FROM 조회할 테이블"
                    //CUR_Transform => CURTrn_id, CURTrn_enePosX, CURTrn_enePosZ, CURTrn_dist, CURTrn_angle
                    string sqlQuery = "SELECT * FROM AFT_Transform";
                    dbCommand.CommandText = sqlQuery;

                    IDataReader reader = dbCommand.ExecuteReader();

                    int index = 0;

                    while (reader.Read())
                    {
                        string id = reader.GetString(0);
                        float AFTTrn_enePosX = reader.GetFloat(1);
                        float AFTTrn_enePosZ = reader.GetFloat(2);
                        float AFTTrn_dist = reader.GetFloat(3);
                        float AFTTrn_angle = reader.GetFloat(4);

                        cur.Add(new SituationCUR("NULL", -1f, -1f, -1f, -1f, new IntVector3(-1, -1, -1), -1f));
                        aft.Add(new SituationAFT("NULL", -1f, -1f, -1f, -1f, "NULL", -1, -1, false));

                        aft[index]._id = id;
                        aft[index]._posX = AFTTrn_enePosX;
                        aft[index]._posZ = AFTTrn_enePosZ;
                        aft[index]._dist = AFTTrn_dist;
                        aft[index]._angleComp = AFTTrn_angle;

                        index++;
                    }

                    reader.Close();

                    index = 0;

                    sqlQuery = "SELECT * FROM AFT_Bools";
                    dbCommand.CommandText = sqlQuery;

                    reader = dbCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        string id = reader.GetString(0);
                        int AFTBo_P = reader.GetInt32(1);
                        int AFTBo_C = reader.GetInt32(2);
                        string AFTBo_bID = reader.GetString(3);
                        int AFTBo_bD = reader.GetInt32(4);

                        bool AFTBo_c = false;

                        if (AFTBo_C == 1)
                            AFTBo_c = true;

                        aft[index]._id = id;
                        aft[index]._hitCounter = AFTBo_P;
                        aft[index]._closer = AFTBo_c;
                        aft[index]._beforeID = AFTBo_bID;
                        aft[index]._beforeDB = AFTBo_bD;

                        index++;
                    }

                    reader.Close();
                    index = 0;
                    //--------------------------------------------------------
                    sqlQuery = "SELECT * FROM CUR_Transform";
                    dbCommand.CommandText = sqlQuery;

                    reader = dbCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        string id = reader.GetString(0);
                        float CURTrn_enePosX = reader.GetFloat(1);
                        float CURTrn_enePosZ = reader.GetFloat(2);
                        float CURTrn_dist = reader.GetFloat(3);
                        float CURTrn_angle = reader.GetFloat(4);

                        cur[index]._id = id;
                        cur[index]._posX = CURTrn_enePosX;
                        cur[index]._posZ = CURTrn_enePosZ;
                        cur[index]._dist = CURTrn_dist;
                        cur[index]._angleComp = CURTrn_angle;

                        index++;
                    }

                    reader.Close();

                    index = 0;


                    sqlQuery = "SELECT * FROM CUR_Doing";
                    dbCommand.CommandText = sqlQuery;

                    reader = dbCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        int CURDo_mov = reader.GetInt32(1);
                        int CURDo_rot = reader.GetInt32(2);
                        int CURDo_atk = reader.GetInt32(3);

                        cur[index]._doing = new IntVector3(CURDo_mov, CURDo_rot, CURDo_atk);

                        index++;
                    }

                    reader.Close();

                    index = 0;

                    sqlQuery = "SELECT * FROM CUR_Bools";
                    dbCommand.CommandText = sqlQuery;

                    reader = dbCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        float CURBo_Time = reader.GetFloat(1);

                        cur[index]._time = CURBo_Time;

                        index++;
                    }

                    reader.Close();

                    //닫아주고 초기화
                    reader.Close();
                    reader = null;
                    dbCommand.Dispose();
                }

                dbConnection.Close();
            }

            for (int i = 0; i < cur.Count; i++)
            {
                result.Add(new AIData(cur[i], aft[i]));

                if (fileIndex == -1)
                {
                    if (Mathf.Abs(result[i].sitCUR._angleComp + 90) < 10 && result[i].sitCUR._doing.vecZ != 3 && result[i].sitCUR._doing.vecZ != 0)
                        result[i].sitCUR._doing.vecZ = 3;
                    else if (Mathf.Abs(result[i].sitCUR._angleComp - 90) < 10 && result[i].sitCUR._doing.vecZ != 2 && result[i].sitCUR._doing.vecZ != 0)
                        result[i].sitCUR._doing.vecZ = 2;
                    else if (Mathf.Abs(result[i].sitCUR._angleComp) < 10 && result[i].sitCUR._doing.vecZ != 1 && result[i].sitCUR._doing.vecZ != 0)
                        result[i].sitCUR._doing.vecZ = 1;
                    else if ((result[i].sitCUR._angleComp >= 100 || result[i].sitCUR._angleComp <= -100) && result[i].sitCUR._doing.vecZ != 0)
                        result[i].sitCUR._doing.vecZ = 0;

                    if (result[i].sitCUR._dist >= 40.1f)
                    {
                        if (((result[i].sitCUR._angleComp + 180) > 270) || ((result[i].sitCUR._angleComp + 180) < 90))
                            result[i].sitCUR._doing = new IntVector3(2, 6, 0);
                        else if (result[i].sitCUR._angleComp < 30 && result[i].sitCUR._angleComp >= 0)
                            result[i].sitCUR._doing = new IntVector3(1, 4, 0);
                        else if (result[i].sitCUR._angleComp > -30 && result[i].sitCUR._angleComp < 0)
                            result[i].sitCUR._doing = new IntVector3(1, 5, 0);
                        else
                            result[i].sitCUR._doing = new IntVector3(1, 3, 0);
                    }
                    else if (result[i].sitCUR._dist >= 20.1f)
                        result[i].sitCUR._doing.vecZ = 0;
                }
                else if (fileIndex == 0)
                {
                    if (Mathf.Abs(result[i].sitCUR._angleComp + 90) < 10 && result[i].sitCUR._doing.vecZ != 3 && result[i].sitCUR._doing.vecZ != 0)
                        result[i].sitCUR._doing.vecZ = 1;
                    else if (Mathf.Abs(result[i].sitCUR._angleComp - 90) < 10 && result[i].sitCUR._doing.vecZ != 2 && result[i].sitCUR._doing.vecZ != 0)
                        result[i].sitCUR._doing.vecZ = 3;
                    else if (Mathf.Abs(result[i].sitCUR._angleComp) < 10 && result[i].sitCUR._doing.vecZ != 1 && result[i].sitCUR._doing.vecZ != 0)
                        result[i].sitCUR._doing.vecZ = 2;
                    else if ((result[i].sitCUR._angleComp >= 100 || result[i].sitCUR._angleComp <= -100) && result[i].sitCUR._doing.vecZ != 0)
                        result[i].sitCUR._doing.vecZ = 0;

                    if (result[i].sitCUR._dist >= 40.1f)
                    {
                        if ((result[i].sitCUR._angleComp + 180) > 270)
                            result[i].sitCUR._doing = new IntVector3(1, 5, 1);
                        else if((result[i].sitCUR._angleComp + 180) < 90)
                            result[i].sitCUR._doing = new IntVector3(1, 4, 2);
                        else if (result[i].sitCUR._angleComp < 30 && result[i].sitCUR._angleComp >= 0)
                            result[i].sitCUR._doing = new IntVector3(2, 2, 1);
                        else if (result[i].sitCUR._angleComp > -30 && result[i].sitCUR._angleComp < 0)
                            result[i].sitCUR._doing = new IntVector3(1, 1, 2);
                        else
                            result[i].sitCUR._doing = new IntVector3(1, 2, 1);
                    }
                }
                else if (fileIndex == 1)
                {
                    if (Mathf.Abs(result[i].sitCUR._angleComp + 90) < 10 && result[i].sitCUR._doing.vecZ != 3 && result[i].sitCUR._doing.vecZ != 0)
                        result[i].sitCUR._doing.vecZ = 3;
                    else if (Mathf.Abs(result[i].sitCUR._angleComp - 90) < 10 && result[i].sitCUR._doing.vecZ != 2 && result[i].sitCUR._doing.vecZ != 0)
                        result[i].sitCUR._doing.vecZ = 3;
                    else if (Mathf.Abs(result[i].sitCUR._angleComp) < 10 && result[i].sitCUR._doing.vecZ != 1 && result[i].sitCUR._doing.vecZ != 0)
                        result[i].sitCUR._doing.vecZ = 1;
                    else if ((result[i].sitCUR._angleComp >= 100 || result[i].sitCUR._angleComp <= -100) && result[i].sitCUR._doing.vecZ != 0)
                        result[i].sitCUR._doing.vecZ = 0;

                    if (result[i].sitCUR._dist >= 40.1f)
                    {
                        if ((result[i].sitCUR._angleComp + 180) > 270)
                            result[i].sitCUR._doing = new IntVector3(1, 5, 1);
                        else if ((result[i].sitCUR._angleComp + 180) < 90)
                            result[i].sitCUR._doing = new IntVector3(1, 4, 2);
                        else if (result[i].sitCUR._angleComp < 30 && result[i].sitCUR._angleComp >= 0)
                            result[i].sitCUR._doing = new IntVector3(2, 2, 0);
                        else if (result[i].sitCUR._angleComp > -30 && result[i].sitCUR._angleComp < 0)
                            result[i].sitCUR._doing = new IntVector3(1, 2, 3);
                        else
                            result[i].sitCUR._doing = new IntVector3(1, 1, 2);
                    }
                    else if (result[i].sitCUR._dist >= 20.1f)
                        result[i].sitCUR._doing.vecZ = 0;
                }
            }

            Debug.Log("Read Done");
            return result;
        }

        //"behaveData0"처럼 뒤의 숫자도 써줘야 됨
        //이제 안 써도 됨, 모든 데이터 "behaveData"로 통합
        public static List<SituationCUR> ReadSitCUR_FROM_DB(string fileName)
        {

            List<SituationCUR> cur = new List<SituationCUR>();

            fileName = @"Data Source=" + dbPath + "/" + fileName + ".db";

            using (var dbConnection = new SqliteConnection(fileName))
            {
                dbConnection.Open();

                using (IDbCommand dbCommand = dbConnection.CreateCommand())
                {
                    
                    string sqlQuery = "SELECT * FROM CUR_Transform";
                    dbCommand.CommandText = sqlQuery;

                    IDataReader reader = dbCommand.ExecuteReader();

                    int index = 0;

                    while (reader.Read())
                    {
                        string id = reader.GetString(0);
                        float CURTrn_enePosX = reader.GetFloat(1);
                        float CURTrn_enePosZ = reader.GetFloat(2);
                        float CURTrn_dist = reader.GetFloat(3);
                        float CURTrn_angle = reader.GetFloat(4);

                        cur.Add(new SituationCUR("NULL", -1f, -1f, -1f, -1f, new IntVector3(-1, -1, -1), -1f));

                        cur[index]._id = id;
                        cur[index]._posX = CURTrn_enePosX;
                        cur[index]._posZ = CURTrn_enePosZ;
                        cur[index]._dist = CURTrn_dist;
                        cur[index]._angleComp = CURTrn_angle;

                        index++;
                    }

                    reader.Close();

                    index = 0;

                    sqlQuery = "SELECT * FROM CUR_Doing";
                    dbCommand.CommandText = sqlQuery;

                    reader = dbCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        int CURDo_mov = reader.GetInt32(1);
                        int CURDo_rot = reader.GetInt32(2);
                        int CURDo_atk = reader.GetInt32(3);

                        cur[index]._doing = new IntVector3(CURDo_mov, CURDo_rot, CURDo_atk);

                        index++;
                    }

                    reader.Close();

                    index = 0;

                    sqlQuery = "SELECT * FROM CUR_Bools";
                    dbCommand.CommandText = sqlQuery;

                    reader = dbCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        float time = reader.GetFloat(1);
                        cur[index]._time = time;

                        index++;
                    }

                    //닫아주고 초기화
                    reader.Close();
                    reader = null;
                    dbCommand.Dispose();
                }

                dbConnection.Close();
            }

            return cur;
        }

        //"behaveData0"처럼 뒤의 숫자도 써줘야 됨
        //이제 안 써도 됨, 모든 데이터 "behaveData"로 통합
        public static List<SituationAFT> ReadSitAFT_FROM_DB(string fileName)
        {

            List<SituationAFT> aft = new List<SituationAFT>();

            fileName = @"Data Source=" + dbPath + "/" + fileName + ".db";

            int index = 0;
            //AFT 클래스로 저장되지만 AFT와는 다른 목적의 정보도 읽어오기 위해 구별해주는 변수
            bool checker = true;

            using (var dbConnection = new SqliteConnection(fileName))
            {
                dbConnection.Open();

                using (IDbCommand dbCommand = dbConnection.CreateCommand())
                {
                    //"SELECT 조회할 칼럼 FROM 조회할 테이블"
                    string sqlQuery = "SELECT * FROM AFT_Transform";
                    dbCommand.CommandText = sqlQuery;

                    IDataReader reader = dbCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        string id = reader.GetString(0);
                        float AFTTrn_enePosX = reader.GetFloat(1);
                        float AFTTrn_enePosZ = reader.GetFloat(2);
                        float AFTTrn_dist = reader.GetFloat(3);
                        float AFTTrn_angle = reader.GetFloat(4);

                        aft.Add(new SituationAFT("NULL", -1f, -1f, -1f, -1f, "NULL", -1, -1, false));

                        aft[index]._id = id;
                        aft[index]._posX = AFTTrn_enePosX;
                        aft[index]._posZ = AFTTrn_enePosZ;
                        aft[index]._dist = AFTTrn_dist;
                        aft[index]._angleComp = AFTTrn_angle;

                        checker = false;

                        index++;
                    }

                    reader.Close();

                    index = 0;

                    sqlQuery = "SELECT * FROM AFT_Bools";
                    dbCommand.CommandText = sqlQuery;

                    reader = dbCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        string id = reader.GetString(0);
                        int AFTBo_P = reader.GetInt32(1);
                        int AFTBo_C = reader.GetInt32(2);
                        string AFTBo_bID = reader.GetString(3);
                        int AFTBo_bD = reader.GetInt32(4);

                        bool AFTBo_c = false;

                        if (AFTBo_C == 1)
                            AFTBo_c = true;

                        if (checker)
                        {
                            aft.Add(new SituationAFT("NULL", -1f, -1f, -1f, -1f, "NULL", -1, -1, false));

                            aft[index]._id = id;
                            aft[index]._hitCounter = AFTBo_P;
                            aft[index]._closer = AFTBo_c;
                            aft[index]._beforeID = AFTBo_bID;
                            aft[index]._beforeDB = AFTBo_bD;
                        }
                        else
                        {
                            aft[index]._hitCounter = AFTBo_P;
                            aft[index]._closer = AFTBo_c;
                            aft[index]._beforeID = AFTBo_bID;
                            aft[index]._beforeDB = AFTBo_bD;
                        }

                        index++;
                    }

                    //닫아주고 초기화
                    reader.Close();
                    reader = null;
                    dbCommand.Dispose();
                }

                dbConnection.Close();
            }

            return aft;
        }

        //startLine부터 endLine까지만 읽는 함수
        public static List<AIData> ReadAIData_FROM_DB(string fileName, int startLine, int endLine)
        {
            List<AIData> result = new List<AIData>();

            List<SituationCUR> cur = ReadSitCUR_FROM_DB(fileName, startLine, endLine);
            List<SituationAFT> aft = ReadSitAFT_FROM_DB(fileName, startLine, endLine);

            for (int i = 0; i < cur.Count; i++)
            {
                result.Add(new AIData(cur[i], aft[i]));
            }


            return result;
        }

        //startLine부터 endLine까지만 읽는 함수
        public static List<SituationCUR> ReadSitCUR_FROM_DB(string fileName, int startLine, int endLine)
        {

            List<SituationCUR> cur = new List<SituationCUR>();

            fileName = @"Data Source=" + dbPath + "/" + fileName + ".db";

            int index = 0;

            using (var dbConnection = new SqliteConnection(fileName))
            {
                dbConnection.Open();

                using (IDbCommand dbCommand = dbConnection.CreateCommand())
                {
                    string sqlQuery = "SELECT * FROM CUR_Transform WHERE rowid >= " + startLine + " AND rowid <= " + endLine;
                    dbCommand.CommandText = sqlQuery;

                    IDataReader reader = dbCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        string id = reader.GetString(0);
                        float CURTrn_enePosX = reader.GetFloat(1);
                        float CURTrn_enePosZ = reader.GetFloat(2);
                        float CURTrn_dist = reader.GetFloat(3);
                        float CURTrn_angle = reader.GetFloat(4);

                        cur.Add(new SituationCUR("NULL", -1f, -1f, -1f, -1f, new IntVector3(-1, -1, -1), -1f));

                        cur[index]._id = id;
                        cur[index]._posX = CURTrn_enePosX;
                        cur[index]._posZ = CURTrn_enePosZ;
                        cur[index]._dist = CURTrn_dist;
                        cur[index]._angleComp = CURTrn_angle;

                        index++;
                    }

                    reader.Close();

                    index = 0;

                    sqlQuery = "SELECT * FROM CUR_Doing WHERE rowid >= " + startLine + " AND rowid <= " + endLine;
                    dbCommand.CommandText = sqlQuery;

                    reader = dbCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        int CURDo_mov = reader.GetInt32(1);
                        int CURDo_rot = reader.GetInt32(2);
                        int CURDo_atk = reader.GetInt32(3);

                        cur[index]._doing = new IntVector3(CURDo_mov, CURDo_rot, CURDo_atk);

                        index++;
                    }

                    reader.Close();

                    //readIndex = 0;
                    index = 0;

                    //sqlQuery = "SELECT * FROM CUR_Bools";
                    sqlQuery = "SELECT * FROM CUR_Bools WHERE rowid >= " + startLine + " AND rowid <= " + endLine;
                    dbCommand.CommandText = sqlQuery;

                    reader = dbCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        float time = reader.GetFloat(1);

                        cur[index]._time = time;

                        index++;
                    }

                    //닫아주고 초기화
                    reader.Close();
                    reader = null;
                    dbCommand.Dispose();
                }

                dbConnection.Close();
            }

            return cur;
        }

        //startLine부터 endLine까지만 읽는 함수
        public static List<SituationAFT> ReadSitAFT_FROM_DB(string fileName, int startLine, int endLine)
        {

            List<SituationAFT> aft = new List<SituationAFT>();

            fileName = @"Data Source=" + dbPath + "/" + fileName + ".db";

            int index = 0;
            //AFT 클래스로 저장되지만 AFT와는 다른 목적의 정보도 읽어오기 위해 구별해주는 변수
            bool checker = true;

            using (var dbConnection = new SqliteConnection(fileName))
            {
                dbConnection.Open();

                using (IDbCommand dbCommand = dbConnection.CreateCommand())
                {
                    //"SELECT 조회할 칼럼 FROM 조회할 테이블"
                    //string sqlQuery = "SELECT * FROM AFT_Transform";
                    string sqlQuery = "SELECT * FROM AFT_Transform WHERE rowid >= " + startLine + " AND rowid <= " + endLine;
                    dbCommand.CommandText = sqlQuery;

                    IDataReader reader = dbCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        string id = reader.GetString(0);
                        float AFTTrn_enePosX = reader.GetFloat(1);
                        float AFTTrn_enePosZ = reader.GetFloat(2);
                        float AFTTrn_dist = reader.GetFloat(3);
                        float AFTTrn_angle = reader.GetFloat(4);

                        aft.Add(new SituationAFT("NULL", -1f, -1f, -1f, -1f, "NULL", -1, -1, false));

                        aft[index]._id = id;
                        aft[index]._posX = AFTTrn_enePosX;
                        aft[index]._posZ = AFTTrn_enePosZ;
                        aft[index]._dist = AFTTrn_dist;
                        aft[index]._angleComp = AFTTrn_angle;

                        index++;
                        checker = false;
                    }

                    reader.Close();

                    index = 0;

                    sqlQuery = "SELECT * FROM AFT_Bools WHERE rowid >= " + startLine + " AND rowid <= " + endLine;
                    //sqlQuery = "SELECT * FROM AFT_Bools";
                    dbCommand.CommandText = sqlQuery;

                    reader = dbCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        string id = reader.GetString(0);
                        int AFTBo_P = reader.GetInt32(1);
                        int AFTBo_C = reader.GetInt32(2);
                        string AFTBo_bID = reader.GetString(3);
                        int AFTBo_bD = reader.GetInt32(4);

                        bool AFTBo_c = false;

                        if (AFTBo_C == 1)
                            AFTBo_c = true;

                        if (checker)
                        {
                            aft.Add(new SituationAFT("NULL", -1f, -1f, -1f, -1f, "NULL", -1, -1, false));

                            aft[index]._id = id;
                            aft[index]._hitCounter = AFTBo_P;
                            aft[index]._closer = AFTBo_c;
                            aft[index]._beforeID = AFTBo_bID;
                            aft[index]._beforeDB = AFTBo_bD;
                        }
                        else
                        {
                            aft[index]._hitCounter = AFTBo_P;
                            aft[index]._closer = AFTBo_c;
                            aft[index]._beforeID = AFTBo_bID;
                            aft[index]._beforeDB = AFTBo_bD;
                        }

                        index++;
                    }

                    //닫아주고 초기화
                    reader.Close();
                    reader = null;
                    dbCommand.Dispose();
                }

                dbConnection.Close();
            }

            return aft;
        }

        //angle 이상, (angle+1)미만인 CUR_Trn_Angle 값을 갖는 데이터만 읽기
        public static List<AIData> ReadAIData_FROM_DB(string fileName, int angle)
        {
            List<AIData> result = new List<AIData>();
            List<string> idList = new List<string>();

            int startLine = 999999, endLine = 0;

            idList = GetStart_AND_End_BY_Angle(fileName, angle, ref startLine, ref endLine, 1);

            List<SituationCUR> cur = ReadSitCUR_FROM_DB(fileName, startLine, endLine, idList);
            List<SituationAFT> aft = ReadSitAFT_FROM_DB(fileName, startLine, endLine, idList);

            for (int i = 0; i < cur.Count; i++)
            {
                result.Add(new AIData(cur[i], aft[i]));
            }


            return result;
        }

        //angle 이상, (angle+num)미만인 CUR_Trn_Angle 값을 갖는 데이터만 읽기
        public static List<AIData> ReadAIData_FROM_DB_Angle(string fileName, int angle, int num)
        {
            List<AIData> result = new List<AIData>();
            List<string> idList = new List<string>();

            int startLine = 999999, endLine = 0;

            Debug.Log(angle + "~" + (angle + num) + " Read");

            idList = GetStart_AND_End_BY_Angle(fileName, angle, ref startLine, ref endLine, num);

            List<SituationCUR> cur = ReadSitCUR_FROM_DB(fileName, startLine, endLine, idList);
            List<SituationAFT> aft = ReadSitAFT_FROM_DB(fileName, startLine, endLine, idList);

            for (int i = 0; i < cur.Count; i++)
            {
                result.Add(new AIData(cur[i], aft[i]));
            }


            return result;
        }

        //CUR_Trn_Angle 값이 angle 이상, (angle+numA)미만이고 CUR_Trn_Dist 값이 dist 이상, (dist+numD)미만인 데이터만 읽기
        public static List<AIData> ReadAIData_FROM_DB_Angle_AND_Dist(string fileName, int angle, int numA, int dist, int numD)
        {
            List<AIData> result = new List<AIData>();
            List<string> idList = new List<string>();

            int startLine = 999999, endLine = 0;

            idList = GetStart_AND_End_BY_Angle(fileName, angle, ref startLine, ref endLine, numA);

            List<SituationCUR> cur = ReadSitCUR_FROM_DB(fileName, startLine, endLine, idList);
            List<SituationAFT> aft = ReadSitAFT_FROM_DB(fileName, startLine, endLine, idList);

            for (int i = 0; i < cur.Count; i++)
            {
                if (cur[i]._dist >= dist && cur[i]._dist < dist + numD)
                    result.Add(new AIData(cur[i], aft[i]));
            }

            for (int i = 0; i < cur.Count; i++)
            {
                cur[i].Dispose();
                aft[i].Dispose();
            }

            return result;
        }

        //위 함수를 보조하기 위한 함수
        private static List<string> GetStart_AND_End_BY_Angle(string fileName, int angle, ref int start, ref int end, int num)
        {
            List<string> result = new List<string>();

            fileName = @"Data Source=" + dbPath + "/" + fileName + ".db";

            using (var dbConnection = new SqliteConnection(fileName))
            {
                dbConnection.Open();

                using (IDbCommand dbCommand = dbConnection.CreateCommand())
                {
                    string sqlQuery = "SELECT rowid, CUR_Trn_id FROM CUR_Transform WHERE CUR_Trn_angle >= " + angle + " AND CUR_Trn_angle < " + (angle+num);
                    dbCommand.CommandText = sqlQuery;

                    IDataReader reader = dbCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        if (start >= reader.GetInt32(0))
                        {
                            start = reader.GetInt32(0);
                        }

                        if (end <= reader.GetInt32(0))
                        {
                            end = reader.GetInt32(0);
                        }

                        result.Add(reader.GetString(1));
                    }

                    //닫아주고 초기화
                    reader.Close();
                    reader = null;
                    dbCommand.Dispose();
                }

                dbConnection.Close();
            }

            return result;
        }

        //startLine부터 endLine까지만 읽는 + idList의 id와 일치하는 것만 함수 
        public static List<SituationCUR> ReadSitCUR_FROM_DB(string fileName, int startLine, int endLine, List<string> idList)
        {
            List<SituationCUR> cur = new List<SituationCUR>();

            fileName = @"Data Source=" + dbPath + "/" + fileName + ".db";

            int index = 0;

            //정상 작동일 때
            if (idList.Count > 0)
            {
                using (var dbConnection = new SqliteConnection(fileName))
                {
                    dbConnection.Open();

                    using (IDbCommand dbCommand = dbConnection.CreateCommand())
                    {
                        string sqlQuery = "SELECT * FROM CUR_Transform WHERE rowid >= " + startLine + " AND rowid <= " + endLine;
                        dbCommand.CommandText = sqlQuery;

                        IDataReader reader = dbCommand.ExecuteReader();

                        while (reader.Read())
                        {
                            string id = reader.GetString(0);

                            if (index >= idList.Count)
                                break;

                            if (idList[index] == id)
                            {
                                float CURTrn_enePosX = reader.GetFloat(1);
                                float CURTrn_enePosZ = reader.GetFloat(2);
                                float CURTrn_dist = reader.GetFloat(3);
                                float CURTrn_angle = reader.GetFloat(4);

                                cur.Add(new SituationCUR("NULL", -1f, -1f, -1f, -1f, new IntVector3(-1, -1, -1), -1f));

                                cur[index]._id = id;
                                cur[index]._posX = CURTrn_enePosX;
                                cur[index]._posZ = CURTrn_enePosZ;
                                cur[index]._dist = CURTrn_dist;
                                cur[index]._angleComp = CURTrn_angle;

                                index++;
                            }
                        }

                        reader.Close();

                        index = 0;

                        sqlQuery = "SELECT * FROM CUR_Doing WHERE rowid >= " + startLine + " AND rowid <= " + endLine;
                        dbCommand.CommandText = sqlQuery;

                        reader = dbCommand.ExecuteReader();

                        while (reader.Read())
                        {
                            string id = reader.GetString(0);

                            if (index >= idList.Count)
                                break;

                            if (idList[index] == id)
                            {
                                int CURDo_mov = reader.GetInt32(1);
                                int CURDo_rot = reader.GetInt32(2);
                                int CURDo_atk = reader.GetInt32(3);

                                cur[index]._doing = new IntVector3(CURDo_mov, CURDo_rot, CURDo_atk);

                                index++;
                            }
                        }

                        reader.Close();

                        //readIndex = 0;
                        index = 0;

                        //sqlQuery = "SELECT * FROM CUR_Bools";
                        sqlQuery = "SELECT * FROM CUR_Bools WHERE rowid >= " + startLine + " AND rowid <= " + endLine;
                        dbCommand.CommandText = sqlQuery;

                        reader = dbCommand.ExecuteReader();

                        while (reader.Read())
                        {
                            string id = reader.GetString(0);

                            if (index >= idList.Count)
                                break;

                            if (idList[index] == id)
                            {
                                float time = reader.GetFloat(1);

                                cur[index]._time = time;

                                index++;
                            }
                        }

                        //닫아주고 초기화
                        reader.Close();
                        reader = null;
                        dbCommand.Dispose();
                    }

                    dbConnection.Close();
                }
            }
            //idList 내용이 아예 없을 때
            else
            {
                Debug.Log("Empty idList");
            }

            return cur;
        }

        //startLine부터 endLine까지만 읽는 함수
        public static List<SituationAFT> ReadSitAFT_FROM_DB(string fileName, int startLine, int endLine, List<string> idList)
        {

            List<SituationAFT> aft = new List<SituationAFT>();

            fileName = @"Data Source=" + dbPath + "/" + fileName + ".db";

            int index = 0;

            using (var dbConnection = new SqliteConnection(fileName))
            {
                dbConnection.Open();

                using (IDbCommand dbCommand = dbConnection.CreateCommand())
                {
                    //"SELECT 조회할 칼럼 FROM 조회할 테이블"
                    //string sqlQuery = "SELECT * FROM AFT_Transform";
                    string sqlQuery = "SELECT * FROM AFT_Transform WHERE rowid >= " + startLine + " AND rowid <= " + endLine;
                    dbCommand.CommandText = sqlQuery;

                    IDataReader reader = dbCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        string id = reader.GetString(0);

                        if (index >= idList.Count)
                            break;

                        if (idList[index] == id)
                        {
                            float AFTTrn_enePosX = reader.GetFloat(1);
                            float AFTTrn_enePosZ = reader.GetFloat(2);
                            float AFTTrn_dist = reader.GetFloat(3);
                            float AFTTrn_angle = reader.GetFloat(4);

                            aft.Add(new SituationAFT("NULL", -1f, -1f, -1f, -1f, "NULL", -1, -1, false));

                            aft[index]._id = id;
                            aft[index]._posX = AFTTrn_enePosX;
                            aft[index]._posZ = AFTTrn_enePosZ;
                            aft[index]._dist = AFTTrn_dist;
                            aft[index]._angleComp = AFTTrn_angle;

                            index++;
                        }
                    }

                    reader.Close();

                    index = 0;

                    sqlQuery = "SELECT * FROM AFT_Bools WHERE rowid >= " + startLine + " AND rowid <= " + endLine;
                    //sqlQuery = "SELECT * FROM AFT_Bools";
                    dbCommand.CommandText = sqlQuery;

                    reader = dbCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        string id = reader.GetString(0);

                        if (index >= idList.Count)
                            break;

                        if (idList[index] == id)
                        {
                            int AFTBo_P = reader.GetInt32(1);
                            int AFTBo_C = reader.GetInt32(2);
                            string AFTBo_bID = reader.GetString(3);
                            int AFTBo_bD = reader.GetInt32(4);

                            bool AFTBo_c = false;

                            if (AFTBo_C == 1)
                                AFTBo_c = true;

                            aft[index]._hitCounter = AFTBo_P;
                            aft[index]._closer = AFTBo_c;
                            aft[index]._beforeID = AFTBo_bID;
                            aft[index]._beforeDB = AFTBo_bD;

                            index++;
                        }
                    }

                    //닫아주고 초기화
                    reader.Close();
                    reader = null;
                    dbCommand.Dispose();
                }

                dbConnection.Close();
            }

            return aft;
        }

        //fileName은 "BehaveDB" 이런식으로만 입력할 것 (예상 실제 DB명 == "BehaveDB15.3sdb", 행동을 시작할 때 거리가 15이상 16미만일 때의 DB)
        //현재 거리, 현재 각도와 가장 유사했을 때 가장 좋았던 행동들을 출력하는 함수
        public static IntVector3 Search_Doing_IN_DB(string fileName, float posX, float posZ, float eneTOplyDist, float eneTOplyAngle)
        {
            fileName = @"Data Source=" + dbPath + "/" + fileName + ".db";
            //fileName = @"Data Source=" + dbPath + "/" + fileName + ".db";

            //Debug.Log("file: " + fileName);

            //행동만 출력
            //result.x == -1이면 DB에 적합한 행동이 없었음을 의미
            IntVector3 result = new IntVector3(-1,-1,-1);

            using (var dbConnection = new SqliteConnection(fileName))
            {

                dbConnection.Open();

                using (IDbCommand dbCommand = dbConnection.CreateCommand())
                {

                    //"SELECT 조회할 칼럼 FROM 조회할 테이블"
                    //CUR_Transform => CURTrn_id, CURTrn_enePosX, CURTrn_enePosZ, CURTrn_dist, CURTrn_angle
                    string sqlQuery = "SELECT * FROM CUR_Transform";
                    dbCommand.CommandText = sqlQuery;

                    IDataReader reader = dbCommand.ExecuteReader();


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
                    //CUR_Doing => CURDo_id, CURDo_Mov, CURDo_Rot, CURDo_Atk
                    sqlQuery = "SELECT * FROM CUR_Doing";
                    dbCommand.CommandText = sqlQuery;
                    reader = dbCommand.ExecuteReader();

                    //Debug.Log("ID: " + id);

                    while (reader.Read())
                    {
                        if (reader.GetString(0) == id)
                        {
                            result.InitIntVector3(reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3));
                            break;
                        }
                    }

                    //닫아주고 초기화
                    reader.Close();
                    reader = null;
                    dbCommand.Dispose();
                }
                dbConnection.Close();
            }

            return result;
        }

        public static void WriteDB_AIDatas(string fileName, List<AIData> listAIData)
        {
            List<SituationCUR> cur = new List<SituationCUR>();
            List<SituationAFT> aft = new List<SituationAFT>();

            for (int i = 0; i < listAIData.Count; i++)
            {
                cur.Add(new SituationCUR( listAIData[i].sitCUR));
                aft.Add(new SituationAFT( listAIData[i].sitAFT));
            }

            WriteDB_CUR(fileName, cur);
            WriteDB_AFT(fileName, aft);

        }

        //dataID = (행동 시작 시점에서의 System.DateTime.Now) + ": " + (gameObject.GetInstanceID);
        //CUR전용 함수
        public static void WriteDB_CUR(string fileName, List<SituationCUR> sitCUR)
        {
            /* 예상 데이터 분류 (()안은 AD, BD가 들어갈 자리 => id로 AD, BD를 구분할 필요 없음)
             * TABLE: CUR_Transform, AFT_Transform, CUR_Doing, CUR_Bools, AFT_Bools
             * ()_Transform => ()Trn_id, ()Trn_enePosX, ()Trn_enePosZ, ()Trn_dist, ()Trn_angles
             * ()_Doing => ()Do_id, ()Do_Mov, ()Do_Rot, ()Do_Atk
             * ()_Bools => ()Bo_id, ()Bo_plyGetHit, ()Bo_plyCloser, ()Bo_hitBoundary
             */
            //테이블 총 5개

            fileName = @"Data Source=" + dbPath + "/" + fileName + ".db";
            //fileName = @"Data Source=" + dbPath + "/" + fileName + ".db";

            using (var dbConnection = new SqliteConnection(fileName))
            {

                dbConnection.Open();


                using (SqliteTransaction dbTransaction = dbConnection.BeginTransaction())
                {

                    using (IDbCommand dbCommand = dbConnection.CreateCommand())
                    {
                        for (int i = 0; i < sitCUR.Count; i++)
                        {
                            //CUR_Transform 테이블에 대한 Query문 준비
                            string sqlQuery = "INSERT INTO CUR_Transform (CUR_Trn_id, CUR_Trn_posX, CUR_Trn_posZ, CUR_Trn_dist, CUR_Trn_angle) VALUES (@id, @posX, @posZ, @_ene_TO_plyDist, @_ene_TO_plyAngles);";

                            dbCommand.CommandText = sqlQuery;

                            //파라미터 입력
                            dbCommand.Parameters.Add(new SqliteParameter("@id", sitCUR[i]._id));
                            dbCommand.Parameters.Add(new SqliteParameter("@posX", sitCUR[i]._posX));
                            dbCommand.Parameters.Add(new SqliteParameter("@posZ", sitCUR[i]._posZ));
                            dbCommand.Parameters.Add(new SqliteParameter("@_ene_TO_plyDist", sitCUR[i]._dist));
                            dbCommand.Parameters.Add(new SqliteParameter("@_ene_TO_plyAngles", sitCUR[i]._angleComp));

                            dbCommand.ExecuteNonQuery();
                            //CUR_Doing 테이블에 대한 Query문 준비
                            sqlQuery = "INSERT INTO CUR_Doing (CUR_Do_id, CUR_Do_mov, CUR_Do_rot, CUR_Do_atk) VALUES (@id, @_doingMov, @_doingRot, @_doingAtk);";

                            dbCommand.CommandText = sqlQuery;
                            
                            //파라미터 입력
                            dbCommand.Parameters.Add(new SqliteParameter("@id", sitCUR[i]._id));
                            dbCommand.Parameters.Add(new SqliteParameter("@_doingMov", sitCUR[i]._doing.vecX));
                            dbCommand.Parameters.Add(new SqliteParameter("@_doingRot", sitCUR[i]._doing.vecY));
                            dbCommand.Parameters.Add(new SqliteParameter("@_doingAtk", sitCUR[i]._doing.vecZ));

                            dbCommand.ExecuteNonQuery();

                            //CUR_Bools 테이블에 대한 Query문 준비
                            sqlQuery = "INSERT INTO CUR_Bools (CUR_Bo_id, CUR_Bo_time)VALUES (@id, @time);";

                            dbCommand.CommandText = sqlQuery;

                            //파라미터 입력
                            dbCommand.Parameters.Add(new SqliteParameter("@id", sitCUR[i]._id));
                            dbCommand.Parameters.Add(new SqliteParameter("@time", sitCUR[i]._time));

                            dbCommand.ExecuteNonQuery();

                            
                        }

                        dbCommand.Dispose();
                    }


                    dbTransaction.Commit();
                }
                dbConnection.Close();
            }
        }

        //AFT전용 함수
        //쓰기 예시
        public static void WriteDB_AFT(string fileName, List<SituationAFT> sitAFT)
        {
            /* 예상 데이터 분류 (()안은 AD, BD가 들어갈 자리 => id로 AD, BD를 구분할 필요 없음)
             * TABLE: CUR_Transform, AFT_Transform, CUR_Doing, CUR_Bools, AFT_Bools
             * ()_Transform => ()Trn_id, ()Trn_enePosX, ()Trn_enePosZ, ()Trn_dist, ()Trn_angles
             * ()_Doing => ()Do_id, ()Do_Mov, ()Do_Rot, ()Do_Atk
             * ()_Bools => ()Bo_id, ()Bo_plyGetHit, ()Bo_plyCloser, ()Bo_hitBoundary
             */
            //테이블 총 5개

            fileName = @"Data Source=" + dbPath + "/" + fileName + ".db";
            //fileName = @"Data Source=" + dbPath + "/" + fileName + ".db";

            using (var dbConnection = new SqliteConnection(fileName))
            {

                dbConnection.Open();

                using (SqliteTransaction dbTranssaction = dbConnection.BeginTransaction())
                {

                    using (IDbCommand dbCommand = dbConnection.CreateCommand())
                    {
                        for (int i = 0; i < sitAFT.Count; i++)
                        {
                            //AFT_Transform 테이블에 대한 Query문 준비
                            string sqlQuery = "INSERT INTO AFT_Transform (AFT_Trn_id, AFT_Trn_posX, AFT_Trn_posZ, AFT_Trn_dist, AFT_Trn_angle) VALUES (@id, @posX, @posZ, @_ene_TO_plyDist, @_ene_TO_plyAngles);";

                            dbCommand.CommandText = sqlQuery;

                            //파라미터 입력
                            dbCommand.Parameters.Add(new SqliteParameter("@id", sitAFT[i]._id));
                            dbCommand.Parameters.Add(new SqliteParameter("@posX", sitAFT[i]._posX));
                            dbCommand.Parameters.Add(new SqliteParameter("@posZ", sitAFT[i]._posZ));
                            dbCommand.Parameters.Add(new SqliteParameter("@_ene_TO_plyDist", sitAFT[i]._dist));
                            dbCommand.Parameters.Add(new SqliteParameter("@_ene_TO_plyAngles", sitAFT[i]._angleComp));

                            //Query 전송 및 수행
                            dbCommand.ExecuteNonQuery();

                            //AFT_Bools 테이블에 대한 Query문 준비
                            sqlQuery = "INSERT INTO AFT_Bools (AFT_Bo_id, AFT_Bo_getHitCount, AFT_Bo_gettingCloser, AFT_Bo_beforeBehaveID, AFT_Bo_beforeBehaveDB)VALUES (@id, @_isPlayerGetHitCount, @getCloser, @beforeID, @beforeDB);";

                            int getCloser = 0;

                            if (sitAFT[i]._closer) getCloser = 1;

                            dbCommand.CommandText = sqlQuery;

                            //파라미터 입력
                            dbCommand.Parameters.Add(new SqliteParameter("@id", sitAFT[i]._id));
                            dbCommand.Parameters.Add(new SqliteParameter("@_isPlayerGetHitCount", sitAFT[i]._hitCounter));
                            dbCommand.Parameters.Add(new SqliteParameter("@getCloser", getCloser));
                            dbCommand.Parameters.Add(new SqliteParameter("@beforeID", sitAFT[i]._beforeID));
                            dbCommand.Parameters.Add(new SqliteParameter("@beforeDB", sitAFT[i]._beforeDB));

                            dbCommand.ExecuteNonQuery();

                            
                        }

                        dbCommand.Dispose();

                    }
                    dbTranssaction.Commit();
                }
                dbConnection.Close();
            }
        }

        //AFT전용 함수
        public static void WriteDB_AFT_FOR_GreatTEMP(string fileName, List<SituationAFT> sitAFT)
        {
            /* 예상 데이터 분류 (()안은 AD, BD가 들어갈 자리 => id로 AD, BD를 구분할 필요 없음)
             * TABLE: CUR_Transform, AFT_Transform, CUR_Doing, CUR_Bools, AFT_Bools
             * ()_Transform => ()Trn_id, ()Trn_enePosX, ()Trn_enePosZ, ()Trn_dist, ()Trn_angles
             * ()_Doing => ()Do_id, ()Do_Mov, ()Do_Rot, ()Do_Atk
             * ()_Bools => ()Bo_id, ()Bo_plyGetHit, ()Bo_plyCloser, ()Bo_hitBoundary
             */
            //테이블 총 5개

            fileName = @"Data Source=" + dbPath + "/" + fileName + ".db";
            //fileName = @"Data Source=" + dbPath + "/" + fileName + ".db";

            using (var dbConnection = new SqliteConnection(fileName))
            {

                dbConnection.Open();

                using (SqliteTransaction dbTranssaction = dbConnection.BeginTransaction())
                {

                    using (IDbCommand dbCommand = dbConnection.CreateCommand())
                    {
                        for (int i = 0; i < sitAFT.Count; i++)
                        {
                            string sqlQuery;

                            //AFT_Bools 테이블에 대한 Query문 준비
                            sqlQuery = "INSERT INTO AFT_Bools (AFT_Bo_id, AFT_Bo_getHitCount, AFT_Bo_gettingCloser, AFT_Bo_beforeBehaveID, AFT_Bo_beforeBehaveDB)VALUES (@id, @_isPlayerGetHitCount, @getCloser, @beforeID, @beforeDB);";

                            int getCloser = 0;

                            if (sitAFT[i]._closer) getCloser = 1;

                            dbCommand.CommandText = sqlQuery;

                            //파라미터 입력
                            dbCommand.Parameters.Add(new SqliteParameter("@id", sitAFT[i]._id));
                            dbCommand.Parameters.Add(new SqliteParameter("@_isPlayerGetHitCount", sitAFT[i]._hitCounter));
                            dbCommand.Parameters.Add(new SqliteParameter("@getCloser", getCloser));
                            dbCommand.Parameters.Add(new SqliteParameter("@beforeID", sitAFT[i]._beforeID));
                            dbCommand.Parameters.Add(new SqliteParameter("@beforeDB", sitAFT[i]._beforeDB));

                            dbCommand.ExecuteNonQuery();

                            dbCommand.Dispose();
                        }

                    }
                    dbTranssaction.Commit();
                }
                dbConnection.Close();
            }
        }
    }
}
