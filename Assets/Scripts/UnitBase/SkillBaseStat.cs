using UnityEngine;
using System.Collections;

using SkillBaseCode;

public class SkillBaseStat {

    //공격(예시: 공격력 * "2.5"), 힐(예시: 1초당 "5"씩 회복)
    private float _Skill_Rate;

    //스킬 쿨 타임
    private float _Skill_Cool_Time;
    //스킬 지속시간
    private float _Skill_ING_Time;

    //스킬이 필요로 하는 마나의 양 (특수 스킬의 경우에는 __Power_Point)
    private int _Skill_Use_Amount;

    //스킬을 분류하기 위한 코드
    private _SKILL_CODE_Main _Skill_Code_M;
    private _SKILL_CODE_Sub _Skill_Code_S;
    private _SKILL_CODE_Time _Skill_Code_T;

    //스킬 이름
    private string _Skill_Name;

    //스킬 ID
    private string _Skill_ID;

    public float __GET_Skill_Rate
    {
        get { return _Skill_Rate; }
    }

    public float __GET_Skill_Cool_Time
    {
        get { return _Skill_Cool_Time; }
    }
    public float __GET_Skill_ING_Time
    {
        get { return _Skill_ING_Time; }
    }
    public int __GET_Skill_Use_Amount
    {
        get { return _Skill_Use_Amount; }
    }

    public _SKILL_CODE_Main __GET_Skill_Code_M
    {
        get { return _Skill_Code_M; }
    }
    public _SKILL_CODE_Sub __GET_Skill_Code_S
    {
        get { return _Skill_Code_S; }
    }
    public _SKILL_CODE_Time __GET_Skill_Code_T
    {
        get { return _Skill_Code_T; }
    }

    public string __GET_Skill_Name
    {
        get { return _Skill_Name; }
    }

    public string __Get_Skill_ID
    {
        get {return _Skill_ID; }
    }

    //값을 초기화하기 위한 함수
    public void Init_Skill(float rate, float coolT, float ingT, int amount, SkillCode skill_Code)
    {
        _Skill_Rate = rate;

        _Skill_Cool_Time = coolT;
        _Skill_ING_Time = ingT;

        _Skill_Use_Amount = amount;

        _Skill_Code_M = skill_Code._Skill_Code_M;
        _Skill_Code_S = skill_Code._Skill_Code_S;
        _Skill_Code_T = skill_Code._Skill_Code_T;
    }

    public void Initialize_Skill(string name, float rate, float coolT, float ingT, int amount, SkillCode skill_Code, string skillID)
    {
        _Skill_Name = name;

        _Skill_Rate = rate;

        _Skill_Cool_Time = coolT;
        _Skill_ING_Time = ingT;

        _Skill_Use_Amount = amount;

        _Skill_Code_M = skill_Code._Skill_Code_M;
        _Skill_Code_S = skill_Code._Skill_Code_S;
        _Skill_Code_T = skill_Code._Skill_Code_T;

        _Skill_ID = skillID;
    }

    //Debug.Log로 확인하는 용도
    public void Sample__ReadAll()
    {
        Debug.Log("name: "+_Skill_Name + ", rate: " + _Skill_Rate + ", cT: " + _Skill_Cool_Time + ", iT: " + _Skill_ING_Time);
        Debug.Log("amount: "+_Skill_Use_Amount + ", codeM: " + _Skill_Code_M + ", codeS: " + _Skill_Code_S + ", codeT: " + _Skill_Code_T);
    }
}
