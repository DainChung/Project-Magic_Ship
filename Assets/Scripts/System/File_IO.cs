using UnityEngine;
using System;

using System.IO;

using System.Collections;
using System.Collections.Generic;

//using SkillBaseCode;



namespace File_IO {

    public class IO_CSV {

        //임시로 지정된 파일들 경로
        //프로토타입 완성 후 경로들 다양하게 바꿀 것
        private static string filePath = Application.persistentDataPath;

        //파일의 처음부터 읽는 함수
        //fileName은 "/어떠어떠한_파일.csv" 형식으로 입력해야됨
        public static List<string> Reader_CSV(string fileName)
        {
            //읽은 내용 중 쓸모없는 내용을 제거하기 위해 필요한 변수들
            string dummyString = ",,,,,,,,,,,,,,,";

            //filePath뒤에 읽을 파일이름을 더한다.
            fileName = filePath + fileName;

            //CSV파일 읽기
            var reader = new StreamReader(File.OpenRead(fileName), System.Text.Encoding.Default);
            List<string> readList = new List<string>();

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

            //CSV 파일 맨 첫번째 줄 제거
            readList.Remove(readList[0]);

            return readList;
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

        //찾은 내용을 게임 내에서 사용할 수 있도록 가공하는 함수(SkillBaseStat한정)
        public static SkillBaseStat __Get_Searched_SkillBaseStat(string wannaSearch)
        {

            //Debug.Log("WannaSearch: "+wannaSearch);
            //가공이 되지 않은 SkillBaseStat자료를 가공함수에 투입하고 그걸 그대로 반환
            //아래에 작성된 __Get_All_SkillBaseStat 함수로 인해 이 함수를 이 형태로 유지할 것
            //다른 좋은 생각 있으면 추후 수정 요구
            return Set_ResultStat(SearchString_In_File("/Sample__SkillDataBase.csv", wannaSearch));
        }

        //string을 SkillBaseStat으로 가공하는 함수
        private static SkillBaseStat Set_ResultStat(string baseStatString)
        {
            SkillBaseStat resultStat = new SkillBaseStat();

            //중간중간의 ','를 제외하고 따로따로 저장한 1차 가공
            List<string> pieces_OF_BaseStatString = new List<string>();
            string piece = "";

            //작업을 편하게 하기 위한 string 수정
            baseStatString += ',';

            //1차 가공
            while (baseStatString != "")
            {
                //Debug.Log("baseString: " + baseStatString);
                //문자열 맨 처음에서부터 가장 가까운 ','까지 piece로 복사한다.
                piece = baseStatString.Substring(0, baseStatString.IndexOf(','));
                //Debug.Log("piece:" + piece);
                //복사한 문자열을 pieces_OF_BaseStatString에 넣는다.
                pieces_OF_BaseStatString.Add(piece);

                //pieces_OF_BaseStatString에 추가된 내용과 ','을 baseStatString에서 뺸다.
                baseStatString = baseStatString.Remove(0, baseStatString.IndexOf(',') + 1);
            }

            //2차 가공
            float rate = float.Parse(pieces_OF_BaseStatString[2]);
            float coolTime = float.Parse(pieces_OF_BaseStatString[3]);
            float ingTime = float.Parse(pieces_OF_BaseStatString[4]);
            int amount = int.Parse(pieces_OF_BaseStatString[5]);

            SkillBaseCode.SkillCode skillCode = Get_SkillCode_FROM_String(pieces_OF_BaseStatString[6], pieces_OF_BaseStatString[7], pieces_OF_BaseStatString[8]);

            //가공된 내용들을 resultStat에 넣어서 최종 정리한다.
            resultStat.Initialize_Skill(pieces_OF_BaseStatString[1], rate, coolTime, ingTime, amount, skillCode, pieces_OF_BaseStatString[0]);

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
            List<string> base_AllSkills = Reader_CSV("/Sample__SkillDataBase.csv");

            //각 줄마다 SkillBaseStat으로 변경하여 allSkills에 저장
            foreach (string base_SkillString in base_AllSkills)
            {
                //읽은 내용을 하나씩 추가
                allSkills.Add(Set_ResultStat(base_SkillString));
            }

            return allSkills;
        }

    }
}
