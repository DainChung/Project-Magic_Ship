using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class Unit__Base_Stat {

    //이동에 관한 변수
    protected float _Move_Speed;
    protected float _Rotation_Speed;

    //전투에 관한 변수
    protected int __MAX_Health_Point;   //배의 최대 체력
    protected int __MAX_Mana_Point;     //배의 최대 마력
    protected int __MAX_Power_Point;    //배의 최대 특수기술 게이지

    protected int __Health_Point;   //배의 체력
    protected int __Mana_Point;     //배의 마력
    protected int __Power_Point;    //특수기술을 사용하기 위한 게이지 (특수기술 게이지)

    protected int _ATK__Val;        //배의 공격력
    protected float _Critical_Rate; //치명타율
    protected float _Critical_P;    //치명타 계수

    //버프 및 디버프 관련 연산 처리를 위한 변수
    //speed, hp, mp, pp, cri 순서로 들어가도록 할 것
    private List<bool> _Stat_Locker = new List<bool>();

    //위 변수들에 접근할 수 있도록 하는 변수들
    public float __PUB_Move_Speed
    {
        get { return _Move_Speed; }
        set { _Move_Speed = value; }
    }
    public float __PUB_Rotation_Speed
    {
        get { return _Rotation_Speed; }
        set { _Rotation_Speed = value; }
    }

    public int __PUB__Health_Point
    {
        get { return __Health_Point; }
        set { __Health_Point = value; }
    }
    public int __PUB__Mana_Point
    {
        get { return __Mana_Point; }
        set { __Mana_Point = value; }
    }
    public int __PUB__Power_Point
    {
        get { return __Power_Point; }
        set { __Power_Point = value; }
    }

    public int __PUB_ATK__Val
    {
        get { return _ATK__Val; }
        set { _ATK__Val = value; }
    }
    public float __PUB_Critical_Rate
    {
        get { return _Critical_Rate; }
        set { _Critical_Rate = value; }
    }
    public float __PUB_Critical_P
    {
        get { return _Critical_P; }
        set { _Critical_P = value; }
    }
    public List<bool> __PUB_Stat_Locker
    {
        get { return _Stat_Locker; }
        set { _Stat_Locker = value; }
    }

    public int __GET_Max_HP
    {
        get { return __MAX_Health_Point; }
    }
    public int __GET_Max_MP
    {
        get { return __MAX_Mana_Point; }
    }
    public int __GET_Max_PP
    {
        get { return __MAX_Power_Point; }
    }

    //스탯에 직접 영향을 주는 모든 함수는 여기에서 처리해야 될 것 같음.
    //이동속도를 제외한 모든 Stat에 대한 일반 디버프(OR 딜), 일반 버프(OR 힐) 함수
    public void __GET_HIT__About_Stat(string stat_Name, int damage, int isHit_OR_Heal)
    {
        
    }

    //이동속도를 제외한 모든 Stat에 대한 도트 디버프(OR 딜), 도트 버프(OR 힐) 함수
    public IEnumerator __GET_Hit__About_Stat_FREQ(string stat_Name, float duringTime, int damage, float freqTime ,int isHit_OR_Heal)
    {

        //체력을 올리고 내리고를 얼마나 반복할 것인지 계산
        int howMany = (int)(duringTime / freqTime);

        for (int i = 0; i < howMany; i++)
        {
            //올리고자 하는 값을 올린다. (isHit_OR_Heal 값에 따라 딜 또는 힐로 연산된다.)
            __GET_HIT__About_Stat(stat_Name, damage, isHit_OR_Heal);

            //위의 작업을 일정 시간 마다 반복한다.
            yield return new WaitForSeconds(freqTime);
        }

        //해당 코루틴 자동 종료
        yield break;
    }

    //__Health_Point에 일정 수치를 가감하기 위한 함수
    //일반 딜, 일반 힐 등 체력에 관한 연산을 한 번만 수행할 때 사용한다.
    public void __GET_HIT__About_Health(int damage, int isHit_OR_Heal)
    {
        //Exception 관련 내용을 넣을지는 isHit_OR_Heal부분을 Enum으로 변경하고나서 생각할 것
        //체력이 존재하는 경우, damage만큼 체력을 깎는다. 또는 회복한다.
        if (__Health_Point > 0)
        {
            __Health_Point -= (damage * isHit_OR_Heal);  
        }

        //따로 if문을 돌려서 계산 후 처리를 쉽게 하도록 한다.
        //체력이 없는 경우, 체력 값을 0으로 유지한다. (나중에 사망 후 제거처리할 것)
        if(__Health_Point <= 0)
        {
            __Health_Point = 0;
            //사망시 처리에 대해서는 각 Controller.cs에서 처리해야 될 것으로 예상됨.
            Debug.Log("Dead");
        }

        //체력 회복 시 이미 최대 체력을 넘긴 상태라면
        if (__Health_Point > __MAX_Health_Point)
        {
            //최대 체력으로 초기화해준다.
            __Health_Point = __MAX_Health_Point;
        }
        
        ////피격 및 계산 여부를 확실히 알기 위해 작성한 내용
        //if (isHit_OR_Heal == 1)
        //{
        //    Debug.Log("Damage: " + damage + ", Remain HP: " + __Health_Point);
        //}
        //else if (isHit_OR_Heal == -1)
        //{
        //    Debug.Log("Heal: " + damage + ", Remain HP: " + __Health_Point);
        //}
        //else { }
    }

    //도트 딜, 도트 힐 등 체력에 관한 연산을 일정시간 마다 한 번 씩 수행할 때 사용한다.
    public IEnumerator __Get_HIT__About_Health_FREQ(float duringTime, float freqTime, int damage, int isHit_OR_Heal)
    {
        //체력을 올리고 내리고를 얼마나 반복할 것인지 계산
        int howMany = (int)(duringTime / freqTime);

        for (int i = 0; i < howMany; i++)
        {
            //올리고자 하는 값을 올린다. (isHit_OR_Heal 값에 따라 딜 또는 힐로 연산된다.)
            __GET_HIT__About_Health(damage, isHit_OR_Heal);
            Debug.Log("Get Heal");
            //위의 작업을 일정 시간 마다 반복한다.
            yield return new WaitForSeconds(freqTime);
        }

        //해당 코루틴 자동 종료
        yield break;
    }

    //마나를 사용할 때, 마나가 충분한지 확인하는 함수
    public bool __Is_Mana_Enough(int damage)
    {
        bool result;

        if (__Mana_Point >= damage)
            result = true;
        else
            result = false;

        return result;
    }

    //마나를 회복하거나 사용하는 함수
    public void __GET_HIT__About_Mana(int damage, int isHit_OR_Heal)
    {
        //Exception 관련 내용을 넣을지는 isHit_OR_Heal부분을 Enum으로 변경하고나서 생각할 것
        //damage만큼 마나를 깎는다. 또는 회복한다.
        __Mana_Point -= (damage * isHit_OR_Heal);

        //따로 if문을 돌려서 계산 후 처리를 쉽게 하도록 한다.
        //체력이 없는 경우, 체력 값을 0으로 유지한다. (나중에 사망 후 제거처리할 것)
        if (__Mana_Point <= 0)
        {
            __Mana_Point = 0;
        }

        //체력 회복 시 이미 최대 체력을 넘긴 상태라면
        if (__Mana_Point > __MAX_Health_Point)
        {
            //최대 체력으로 초기화해준다.
            __Mana_Point = __MAX_Health_Point;
        }

        ////피격 및 계산 여부를 확실히 알기 위해 작성한 내용
        //if (isHit_OR_Heal == 1)
        //{
        //    Debug.Log("Use Mana: " + damage + ", Remain MP: " + __Mana_Point);
        //}
        //else if (isHit_OR_Heal == -1)
        //{
        //    Debug.Log("Heal Mana: " + damage + ", Remain MP: " + __Mana_Point);
        //}
        //else { }
    }
}
