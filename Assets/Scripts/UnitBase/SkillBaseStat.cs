using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    private List<int> isItLocked = new List<int>();

    //스킬 이름
    private string _Skill_Name;

    //스킬 ID
    private string _Skill_ID;

    //Getter
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
        get { return _Skill_ID; }
    }

    public List<int> __GET_isItLocked
    {
        get { return isItLocked; }
    }

    //Setter
    public float __SET_Skill_Rate
    {
        set { _Skill_Rate = value; }
    }

    public List<int> __SET_isItLocked
    {
        set { isItLocked = value; }
    }

    //20190215 남은 쿨타임에 따른 UI 효과를 보여주기 위해 추가된 변수
    public float time;

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

        //20190215
        //쿨타임과 같은 값에서 1초씩 값을 줄인다.
        time = coolT;
    }

    public void Initialize_Skill(string name, float rate, float coolT, float ingT, int amount, SkillCode skill_Code, string skillID, List<int> _isItLocked)
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

        isItLocked = _isItLocked;
    }

    //Debug.Log로 확인하는 용도
    public void Sample__ReadAll()
    {
        Debug.Log("name: "+_Skill_Name + ", rate: " + _Skill_Rate + ", cT: " + _Skill_Cool_Time + ", iT: " + _Skill_ING_Time);
        Debug.Log("amount: "+_Skill_Use_Amount + ", codeM: " + _Skill_Code_M + ", codeS: " + _Skill_Code_S + ", codeT: " + _Skill_Code_T);
    }
}
