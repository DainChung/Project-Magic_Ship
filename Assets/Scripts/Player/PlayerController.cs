using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SkillBaseCode;

public class PlayerStat : Unit__Base_Stat {

    public void SampleInit(float mSp, float rSp, int hp, int mp, int pp, int atk, float criR, float criP)
    {
        base.__PUB_Move_Speed = mSp;
        base.__PUB_Rotation_Speed = rSp;

        base.__PUB__Health_Point = hp;
        base.__PUB__Mana_Point = mp;
        base.__PUB__Power_Point = pp;

        base.__PUB_ATK__Val = atk;
        base.__PUB_Critical_Rate = criR;
        base.__PUB_Critical_P = criP;

        //일단 상수 5를 이용하도록 할 것
        //나중에 그냥 Array로 바꾸거나 말거나 아무튼 더 생각할 것
        for (int i = 0; i < 5; i++)
        {
            //시작하자마자 버프나 디버프 받는 일은 아직 상정하지 않았으므로
            //일단 다 false로 초기화.
            base.__PUB_Stat_Locker.Add(false);
        }
    }
}

public class PlayerEngine : Unit__Base_Engine {

    public Unit__Base_Movement_Engine __PLY_M_Engine = new Unit__Base_Movement_Engine();
    public Unit__Base_Combat_Engine __PLY_C_Engine = new Unit__Base_Combat_Engine();
}

public class PlayerController : MonoBehaviour {

    //플레이어의 스탯
    public PlayerStat __PLY_Stat = new PlayerStat();
    //플레이어의 동작을 위한 클래스
    private PlayerEngine __PLY_Engine = new PlayerEngine();
    //그냥 쿨타임
    public UnitCoolTimer __PLY_CoolTimer;

    //플레이어가 사용하기로 선택한 스킬들을 저장하는 변수
    private List<SkillBaseStat> __PLY_Selected_Skills = new List<SkillBaseStat>();

    public Transform playerTransform;

    //앞
    public Transform playerFront;
    //우측
    public Transform playerRight;
    //좌측
    public Transform playerLeft;

    //기본덕인 투사체
    //나중엔 여기서 안 하고 SkillBaseStat에 저장했다가 사용할 수도 있음.
    //DB에서 읽쓰하는 기능이 추가되면 옮기고 수정해야됨. 겁나 귀찮겠다 ㅎㅎ
    public GameObject defaultAmmo;

    //쿨타임 떄문에 임시로 적용한 bool 변수, 개선된 알고리즘이 생각나면 바꿔야 될 것
    private bool _Is_On_CoolTime__Default_ATK;

    private bool _Is_On_CoolTime__Skill_0;
    private bool _Is_On_CoolTime__Skill_1;
    private bool _Is_On_CoolTime__Skill_2;

    //나중엔 DB에서 긁어온 값을 여기서 초기화할 것.
    void Awake()
    {
        //이속, 회전속도, 체력, 마나, 파워 게이지, 공격력, 크리확률, 크리계수
        __PLY_Stat.SampleInit(10.0f, 30.0f, 10, 10, 10, 1, 10.0f, 1.5f);

        //Unit__Base_Combat_Engine이 Unit__Base_Movement_Engine에 접근 할 수 있도록 한다.
        __PLY_Engine.__PLY_C_Engine.__SET_unit_M_Engine = __PLY_Engine.__PLY_M_Engine;


        //----------------------------------------------------------------------------------------
        //20190117 이 지점에서 PlayerInfoManager를 통해 지정된 Skill들을 읽어온다. (Skill ID 값만 읽기)
        //읽은 Skill ID 값들을 이용하여 해당 Skill들의 정보를 __PLY_Selected_Skills에 추가한다.
        for (int index = 0; index < 3; index++)
        {
            //내용이 설정되지 않은 Skill추가
            __PLY_Selected_Skills.Add(new SkillBaseStat());

            //InitSkill에 변수로 넣을 SkillCode 클래스
            SkillCode skill_Code = new SkillCode();

            //지정된 Skill ID를 읽고 해당 Skill의 정보를 저장
            

            //읽은 Skill정보를 SkillBaseStat으로 전송
        }
        /* 예상되는 코딩
         * for(int  = 0; i < 3; i++)
         * {
         *      //내용이 없는 Skill칸 추가
         *      __PLY_Selected_Skills.Add(new SkillBaseStat());
         *      
         *      //InitSkill에 변수로 넣을 SkillCode 클래스
         *      SkillCode skill_Code = new SkillCode();
         *      
         *      //지정된 Skill ID를 읽고 해당 Skill의 정보를 저장
         *      //이 부분에 대한 세부적인 계획이 필요함
         *      PlayerInfoManager.ReadSkill(i);
         *      
         *      //읽은 Skill정보를 SkillBaseStat으로 전송
         *      __PLY_Selected_Skills[i].Init_Skill(1.0f, 6.0f, 3.0f, 2, skill_Code);
         * }
         * 
         * 
         * 
         */
        //----------------------------------------------------------------------------------------


        //임시로 지정된 임의의 3개 스킬을 사용하도록 지시한다.
        //DB 적용 전에는 여기에서 초기화할 것
        __PLY_Selected_Skills.Add(new SkillBaseStat());
        __PLY_Selected_Skills.Add(new SkillBaseStat());
        __PLY_Selected_Skills.Add(new SkillBaseStat());
        //일단 노가다 식으로 초기화한다.
        //타입, 배율 및 양, 쿨타임, 지속시간, 소비하는 마나(또는 __Power_Point)의 양
        //------------------------------------------------------------------------------------
        //스킬 분류에 관한 변수 초기화
        //임시 과정이므로 나중에 수정 필요함.
        SkillCode SkillCodeForIndex0 = new SkillCode();
        SkillCode SkillCodeForIndex1 = new SkillCode();
        SkillCode SkillCodeForIndex2 = new SkillCode();

        SkillCodeForIndex0._Skill_Code_M = _SKILL_CODE_Main.BUF;
        SkillCodeForIndex0._Skill_Code_S = _SKILL_CODE_Sub.HP;
        SkillCodeForIndex0._Skill_Code_T = _SKILL_CODE_Time.FREQ;

        SkillCodeForIndex1._Skill_Code_M = _SKILL_CODE_Main.ATK;
        SkillCodeForIndex1._Skill_Code_S = _SKILL_CODE_Sub.HP;
        SkillCodeForIndex1._Skill_Code_T = _SKILL_CODE_Time.FREQ;

        SkillCodeForIndex2._Skill_Code_M = _SKILL_CODE_Main.BUF;
        SkillCodeForIndex2._Skill_Code_S = _SKILL_CODE_Sub.SP;
        SkillCodeForIndex2._Skill_Code_T = _SKILL_CODE_Time.NULL;

        //------------------------------------------------------------------------------------
        //1초마다 체력을 1만큼 3초동안 회복하는 쿨타임 6초, 마나 소모량 2짜리 버프 스킬
        __PLY_Selected_Skills[0].Init_Skill(1.0f, 6.0f, 3.0f, 2, SkillCodeForIndex0);
        //1초마다 (공격력 * 2)만큼 2초동안 데미지를 주는 쿨타임 4초, 마나 소모량 3짜리 공격 스킬 
        __PLY_Selected_Skills[1].Init_Skill(2.0f, 2.0f, 4.0f, 3, SkillCodeForIndex1);
        //4초동안 이동속도 및 회전속도를 20만큼 올려주는 쿨타임 7초, 마나 소모량 4짜리 버프 스킬
        __PLY_Selected_Skills[2].Init_Skill(20.0f, 7.0f, 4.0f, 4, SkillCodeForIndex2);
        //------------------------------------------------------------------------------------

        //나중에 수정 필요
        _Is_On_CoolTime__Default_ATK = true;
        _Is_On_CoolTime__Skill_0 = true;
        _Is_On_CoolTime__Skill_1 = true;
        _Is_On_CoolTime__Skill_2 = true;
    }

    // Use this for initialization
    void Start ()
    {
        //해당 스크립트와 CombatEngine에서 모두 사용하기 위해 이렇게 초기화하여 전달한다.
        __PLY_CoolTimer = transform.GetComponent<UnitCoolTimer>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //기본 이동
        if (Input.GetKey(KeyCode.W))
        {
            __PLY_Engine.__PLY_M_Engine.Move_OBJ(__PLY_Stat.__PUB_Move_Speed, ref playerTransform, 1);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            __PLY_Engine.__PLY_M_Engine.Move_OBJ(__PLY_Stat.__PUB_Move_Speed, ref playerTransform, -0.1f);
        }
        else
        { }

        if (Input.GetKey(KeyCode.A))
        {
            __PLY_Engine.__PLY_M_Engine.Rotate_OBJ(__PLY_Stat.__PUB_Rotation_Speed, ref playerTransform, -1);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            __PLY_Engine.__PLY_M_Engine.Rotate_OBJ(__PLY_Stat.__PUB_Rotation_Speed, ref playerTransform, 1);
        }
        else
        { }

        //기본 공격
        //마우스 좌클릭 -> 전면 공격
        if (Input.GetMouseButtonDown(0) && _Is_On_CoolTime__Default_ATK)
        {
            __PLY_Engine.__PLY_C_Engine.Default_ATK(ref defaultAmmo, ref playerFront, __PLY_Stat.__PUB_ATK__Val);
            //쿨타임을 사용하기 위한 코루틴. 따로 외부 클래스 제작함. 상세 항목은 해당 클래스 참조
            //나중에 쿨타임 값 같은 것도 따로 관리할 것
            __PLY_CoolTimer.StartCoroutine(__PLY_CoolTimer.Timer(1.0f, (input) => { _Is_On_CoolTime__Default_ATK = input; }, _Is_On_CoolTime__Default_ATK));
        }
        //마우스 우클릭 -> 측면 공격
        //나중에 구현하자
        if (Input.GetMouseButtonDown(1))
        {
            // 임시코드(GUI - CH)
            __PLY_Stat.__PUB__Health_Point -= 1;
            __PLY_Stat.__PUB__Mana_Point -= 2;
            __PLY_Stat.__PUB__Power_Point -= 3;
            // 임시코드(GUI - CH)
        }

        //체력 버프 스킬
        if (Input.GetKey(KeyCode.Alpha1) && _Is_On_CoolTime__Skill_0)
        {
            __PLY_Engine.__PLY_C_Engine.Using_Skill(ref defaultAmmo, ref playerFront, __PLY_Selected_Skills[0], __PLY_Stat, this);
            __PLY_CoolTimer.StartCoroutine(__PLY_CoolTimer.Timer(__PLY_Selected_Skills[0].__GET_Skill_Cool_Time, (input) => { _Is_On_CoolTime__Skill_0 = input; }, _Is_On_CoolTime__Skill_0));
        }
        //디버프 공격 스킬
        else if (Input.GetKey(KeyCode.Alpha2) && _Is_On_CoolTime__Skill_1)
        {
            __PLY_Engine.__PLY_C_Engine.Using_Skill(ref defaultAmmo, ref playerFront, __PLY_Selected_Skills[1], __PLY_Stat, this);
            __PLY_CoolTimer.StartCoroutine(__PLY_CoolTimer.Timer(__PLY_Selected_Skills[1].__GET_Skill_Cool_Time, (input) => { _Is_On_CoolTime__Skill_1 = input; }, _Is_On_CoolTime__Skill_1));
        }
        //스피드 버프 스킬
        else if (Input.GetKey(KeyCode.Alpha3) && _Is_On_CoolTime__Skill_2)
        {
            __PLY_Engine.__PLY_C_Engine.Using_Skill(ref defaultAmmo, ref playerFront, __PLY_Selected_Skills[2], __PLY_Stat, this);
            __PLY_CoolTimer.StartCoroutine(__PLY_CoolTimer.Timer(__PLY_Selected_Skills[2].__GET_Skill_Cool_Time, (input) => { _Is_On_CoolTime__Skill_2 = input; }, _Is_On_CoolTime__Skill_2));
        }
        else
        { }

        //쿨타임 안내용
        /*
        if (_Is_On_CoolTime__Skill_2)
            Debug.Log("You Can Use Speed BUFF");
        */

        //스피드 버프 OR 디버프 지속시간 종료 여부
        if (__PLY_Stat.__PUB_Stat_Locker[0])
        {
            //스피드 버프 해제
            //스킬 넣는 부분은 일단 저런식으로 하는 수 밖에 없음
            //나중에 스킬 위치를 마음대로 바꿀 수 있도록 변경할 때 고민이 필요함
            __PLY_Engine.__PLY_M_Engine.Init_Speed_BUF_Amount();
            //스피드 버프 해제로 일단 간주
            __PLY_Stat.__PUB_Stat_Locker[0] = false;
        }
        //체력 버프 OR 디버프 지속시간 종료 여부
        if (__PLY_Stat.__PUB_Stat_Locker[1])
        {

        }
        //마나 버프 OR 디버프 지속시간 종료 여부
        if (__PLY_Stat.__PUB_Stat_Locker[2])
        {

        }
        //PP 버프 OR 디버프 지속시간 종료 여부
        if (__PLY_Stat.__PUB_Stat_Locker[3])
        {

        }
        //크리티컬 버프 OR 디버프 지속시간 종료 여부
        if (__PLY_Stat.__PUB_Stat_Locker[4])
        {

        }
    }
}
