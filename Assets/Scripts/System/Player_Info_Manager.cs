using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using File_IO;

public class Player_Info_Manager : MonoBehaviour {

    //File_IO를 실험하기 위한 임시 코드만 작성
    void Start()
    {
        //플레이어가 장착한 스킬들 모두 정상적으로 출력됨.
        //20190118 다음엔 장착된 스킬들의 정보를 PlayerController에 직접 반영할 것
        List<SkillBaseStat> hello = Read_SkillBaseStat();

        hello[0].Sample__ReadAll();
        hello[1].Sample__ReadAll();
        hello[2].Sample__ReadAll();
        //List<string> output = Read_EquippedSkills();

        //Debug.Log("What");
        //작동 확인을 위한 더미 데이터
        //string[,] samplesmpale =
        //    { { "hello", "Will", "it", "work?" }, { "3","5","2","7" } };

        //일단 쓰는 것만 해보자
        //IO_CSV.Writer_CSV("/IWannaPlayGame.csv", samplesmpale);
        //읽는 방식
        //output = IO_CSV.Reader_CSV("/Sample__SkillDataBase.csv");

        //Excel로 CSV를 열람했을 떄 각 셀 구분은 ","로 하고 줄 단위로 따로따로 읽는 것이 확인되었음.
        //foreach (string outstring in output)
        //{
        //    Debug.Log("output: " + outstring);
        //}
    }

    //Read_EquippedSkills()에서 읽은 ID를 이용하여 Sample__SkilLDataBase.csv에서
    //해당되는 ID값의 SkillStat만 resultSkill에 넣어 반환할 것
    public List<SkillBaseStat> Read_SkillBaseStat()
    {
        List<SkillBaseStat> resultSkills = new List<SkillBaseStat>();
        List<string> equippedSkills = IO_CSV.Reader_CSV("/Sample__PlayerEquippedInfo.csv");

        //필요없는 정보는 지운다.
        equippedSkills.Remove("Skill_ID");

        //플레이어가 장착하고 있는 스킬들의 정보를 읽어온다.
        for (int i = 0; i < equippedSkills.Count; i++)
        {
            resultSkills.Add(IO_CSV.__Get_Searched_SkillBaseStat(equippedSkills[i]));
        }

        return resultSkills;
    }
}
