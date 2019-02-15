using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SkillBaseCode;

public class PlayerStat : Unit__Base_Stat {

    public void SampleInit(float mSp, float rSp, int hp, int mp, int pp, int atk, float criR, float criP)
    {
        base.__PUB_Move_Speed = mSp;
        base.__PUB_Rotation_Speed = rSp;

        base.__MAX_Health_Point = hp;
        base.__MAX_Mana_Point = mp;
        base.__MAX_Power_Point = pp;

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
    public List<SkillBaseStat> __PLY_Selected_Skills = new List<SkillBaseStat>();

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

    public float playerMoveSpeed;

    //쿨타임 떄문에 임시로 적용한 bool 변수, 개선된 알고리즘이 생각나면 바꿔야 될 것
    private bool _Is_On_CoolTime__Default_ATK;

    private bool[] _Is_On_CoolTime_Skill = new bool[3];

    //Defualt_ATK 자체를 SkillBaseStat 형태로 사용하기 전까진 임시로 사용할 것
    private float default_ATK_Remained_Time;

    //밖에서 값을 읽기 위한 용도
    public bool[] _GET_Is_On_CoolTime_Skill
    {
        get { return _Is_On_CoolTime_Skill; }
    }

    //나중엔 DB에서 긁어온 값을 여기서 초기화할 것.
    void Awake()
    {
        //이속, 회전속도, 체력, 마나, 파워 게이지, 공격력, 크리확률, 크리계수
        __PLY_Stat.SampleInit(10.0f, 30.0f, 10, 10, 10, 1, 10.0f, 1.5f);

        playerMoveSpeed = __PLY_Stat.__PUB_Move_Speed;

        //Unit__Base_Combat_Engine이 Unit__Base_Movement_Engine에 접근 할 수 있도록 한다.
        __PLY_Engine.__PLY_C_Engine.__SET_unit_M_Engine = __PLY_Engine.__PLY_M_Engine;


        //----------------------------------------------------------------------------------------
        //20190117 이 지점에서 PlayerInfoManager를 통해 지정된 Skill들을 읽어온다. (Skill ID 값만 읽기)
        //읽은 Skill ID 값들을 이용하여 해당 Skill들의 정보를 __PLY_Selected_Skills에 추가한다.
        __PLY_Selected_Skills = Player_Info_Manager.Read_Equipped_SkillBaseStat();

        //확인용. 정상적으로 작동함.
        foreach (SkillBaseStat forDebug in __PLY_Selected_Skills)
        {
            Debug.Log("ID: " + forDebug.__Get_Skill_ID + ", Name: " + forDebug.__GET_Skill_Name + ", Rate:" + forDebug.__GET_Skill_Rate);
            Debug.Log("CoolT: " + forDebug.__GET_Skill_Cool_Time + ", IngT: " + forDebug.__GET_Skill_ING_Time + ", UseAmount:" + forDebug.__GET_Skill_Use_Amount);
        }

        _Is_On_CoolTime__Default_ATK = true;

        for (int i = 0; i < _Is_On_CoolTime_Skill.Length; i++)
        {
            _Is_On_CoolTime_Skill[i] = true;
        }
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
            __PLY_Engine.__PLY_M_Engine.Move_OBJ(__PLY_Stat.__PUB_Move_Speed, ref playerTransform, -1f);
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

        //20190130 아래 코드를 응용해서 아래의 주석 내용을 구현할 것
        //공격 형식을 바꿔야 될 수 도 있음
        //Q로 측면, 전면을 교체하고 마우스 좌클릭(GetMouseButtonDown OR GetMouseButton 모두 실험해볼 것) 상태동안 방향 조절하고
        //마우스 좌클릭을 그만둘 때 (GetMouseButtonUp) 탄환 발사 형식으로 조절 고려 중
        //20190215 => 일단 고려만 해둘 것

        //이를 구현하기 전에[ 우선 카메라가 쿼터뷰가 되도록 해야할 것
        if (Input.GetMouseButton(1))
        {
            //임시코드(GUI - CH)
            __PLY_Stat.__PUB__Health_Point -= 1;
            __PLY_Stat.__PUB__Mana_Point -= 1;
            __PLY_Stat.__PUB__Power_Point -= 1;
            Debug.Log("Click Right");
            //임시코드(GUI - CH)
        }

        if (Input.GetMouseButtonUp(1))
        {
            Debug.Log("Mouse Up!");
        }

        //기본 공격
        //마우스 좌클릭 -> 전면 공격
        if (Input.GetMouseButtonDown(0) && _Is_On_CoolTime__Default_ATK)
        {
            __PLY_Engine.__PLY_C_Engine.Default_ATK(ref defaultAmmo, ref playerFront, __PLY_Stat.__PUB_ATK__Val);
            //쿨타임을 사용하기 위한 코루틴. 따로 외부 클래스 제작함. 상세 항목은 해당 클래스 참조
            //나중에 쿨타임 값 같은 것도 따로 관리할 것
            __PLY_CoolTimer.StartCoroutine(__PLY_CoolTimer.Timer(1.0f, (input) => { _Is_On_CoolTime__Default_ATK = input; }, _Is_On_CoolTime__Default_ATK, (input) => { default_ATK_Remained_Time = input; }));
        }
        //마우스 우클릭 -> 측면 공격
        //나중에 구현하자
        if (Input.GetMouseButtonDown(1))
        {
            
        }

        //Debug.Log(__PLY_Selected_Skills[0].time);

        //1번 스킬
        if (Input.GetKey(KeyCode.Alpha1) && _Is_On_CoolTime_Skill[0])
        {
            __PLY_Engine.__PLY_C_Engine.Using_Skill(ref defaultAmmo, ref playerFront, __PLY_Selected_Skills[0], __PLY_Stat, this);
            __PLY_CoolTimer.StartCoroutine(__PLY_CoolTimer.Timer(__PLY_Selected_Skills[0].__GET_Skill_Cool_Time, (input) => { _Is_On_CoolTime_Skill[0] = input; }, _Is_On_CoolTime_Skill[0], (input) => { __PLY_Selected_Skills[0].time = input; }));
        }
        //2번 스킬
        else if (Input.GetKey(KeyCode.Alpha2) && _Is_On_CoolTime_Skill[1])
        {
            __PLY_Engine.__PLY_C_Engine.Using_Skill(ref defaultAmmo, ref playerFront, __PLY_Selected_Skills[1], __PLY_Stat, this);
            __PLY_CoolTimer.StartCoroutine(__PLY_CoolTimer.Timer(__PLY_Selected_Skills[1].__GET_Skill_Cool_Time, (input) => { _Is_On_CoolTime_Skill[1] = input; }, _Is_On_CoolTime_Skill[1], (input) => { __PLY_Selected_Skills[1].time = input; }));
        }
        //3번 스킬
        else if (Input.GetKey(KeyCode.Alpha3) && _Is_On_CoolTime_Skill[2])
        {
            __PLY_Engine.__PLY_C_Engine.Using_Skill(ref defaultAmmo, ref playerFront, __PLY_Selected_Skills[2], __PLY_Stat, this);
            __PLY_CoolTimer.StartCoroutine(__PLY_CoolTimer.Timer(__PLY_Selected_Skills[2].__GET_Skill_Cool_Time, (input) => { _Is_On_CoolTime_Skill[2] = input; }, _Is_On_CoolTime_Skill[2], (input) => { __PLY_Selected_Skills[2].time = input; }));
        }
        else
        { }

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
