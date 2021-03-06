﻿using UnityEngine;
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
        //파워 포인트는 0에서부터 시작한다
        base.__PUB__Power_Point = 0;

        base.__PUB_ATK__Val = atk;
        base.__PUB_Critical_Rate = criR;
        base.__PUB_Critical_P = criP;

        base.FOriginalMoveSpeed = base._Move_Speed;
        base.FOriginalRotateSpeed = base._Rotation_Speed;

        //일단 상수 5를 이용하도록 할 것
        //나중에 그냥 Array로 바꾸거나 말거나 아무튼 더 생각할 것
        for (int i = 0; i < 5; i++)
        {
            //시작하자마자 버프나 디버프 받는 일은 아직 상정하지 않았으므로
            //일단 다 false로 초기화.
            base.__PUB_Stat_IsCoolTimeOn.Add(false);
            base.__PUB_Stat_Real_Locker.Add(false);
        }

        //기본 마나 회복 작동을 위해 이것만 true로 초기화 한다.
        __PUB_Stat_IsCoolTimeOn[2] = true;
    }

    public void Initialize_Player_Stat(List<string> playerStatBaseString)
    {
        base.__PUB_Move_Speed = float.Parse(playerStatBaseString[4]);
        base.__PUB_Rotation_Speed = float.Parse(playerStatBaseString[5]);

        base.__MAX_Health_Point = int.Parse(playerStatBaseString[6]);
        base.__MAX_Mana_Point = int.Parse(playerStatBaseString[7]);
        base.__MAX_Power_Point = int.Parse(playerStatBaseString[8]);

        base.__PUB__Health_Point = base.__MAX_Health_Point;
        base.__PUB__Mana_Point = base.__MAX_Mana_Point;
        //파워 포인트는 0에서부터 시작한다
        base.__PUB__Power_Point = 0;

        base.__PUB_ATK__Val = int.Parse(playerStatBaseString[9]);
        base.__PUB_Critical_Rate = float.Parse(playerStatBaseString[10]);
        base.__PUB_Critical_P = float.Parse(playerStatBaseString[11]);

        base.FOriginalMoveSpeed = base._Move_Speed;
        base.FOriginalRotateSpeed = base._Rotation_Speed;

        //일단 상수 5를 이용하도록 할 것
        //나중에 그냥 Array로 바꾸거나 말거나 아무튼 더 생각할 것
        for (int i = 0; i < 5; i++)
        {
            //시작하자마자 버프나 디버프 받는 일은 아직 상정하지 않았으므로
            //일단 다 false로 초기화.
            base.__PUB_Stat_IsCoolTimeOn.Add(false);
            base.__PUB_Stat_Real_Locker.Add(false);
        }

        //기본 마나 회복 작동을 위해 이것만 true로 초기화 한다.
        __PUB_Stat_IsCoolTimeOn[2] = true;
    }
}

public class PlayerController : MonoBehaviour {

    //플레이어의 스탯
    public PlayerStat __PLY_Stat = new PlayerStat();
    //플레이어의 동작을 위한 클래스
    private UnitBaseEngine __PLY_Engine;
    public UnitBaseEngine __GET__PLY_Engine
    {
        get { return __PLY_Engine; }
    }

    //조작을 막아야 되는 상황이나 Scene에서 사용하기 위한 변수
    public bool playerMustBeFreeze;

    private bool isPlayerHitUltimateSkill = false;
    public bool _GET_isPlayerHitUltimateSkill { get { return isPlayerHitUltimateSkill; } }

    //그냥 쿨타임
    [HideInInspector]
    public UnitCoolTimer __PLY_CoolTimer;

    //플레이어가 사용하기로 선택한 스킬들을 저장하는 변수
    public List<SkillBaseStat> __PLY_Selected_Skills = new List<SkillBaseStat>();

    public Transform playerTransform;

    //플레이어의 앞, 우측, 좌측을 담당할 변수
    //0: 앞, 1: 우측, 2: 좌측
    public Transform[] playerAttackerTransforms;
    public ParticleSystem movingEffect;
    private int index_OF_playerAttackerTransforms;

    //투사체를 발사할 위치 => 키 입력에 따라 앞, 우측, 좌측 값을 저장한다.
    private Transform playerAttacker;

    //쿨타임 때문에 임시로 적용한 bool 변수, 개선된 알고리즘이 생각나면 바꿔야 될 것
    private bool _Is_On_CoolTime__Default_ATK;

    private bool[] _Is_On_CoolTime_Skill = new bool[4];

    //_SKILL_CODE_Main.SPW 스킬 중 _SKILL_CODE_Sub.MOS값을 갖는 스킬 사용 시 마우스 사용을 위해 투입된 bool 값
    private bool _SPW_MOS_Skill_Activated = false;
    //_SPW_MOS_Skill_Activated 변수의 값을 외부에서 설정하기 위한 변수
    public bool _Set_SPW_MOS_Skill_Activated
    {
        set { _SPW_MOS_Skill_Activated = value; }
    }

    //Defualt_ATK 자체를 SkillBaseStat 형태로 사용하기 전까진 임시로 사용할 것
    private float default_ATK_Remained_Time;

    //밖에서 값을 읽기 위한 용도
    public bool[] _GET_Is_On_CoolTime_Skill
    {
        get { return _Is_On_CoolTime_Skill; }
    }

    //궁극기 임시 조치
    SkillBaseStat debugUltimateSkill = new SkillBaseStat();

    public SpriteRenderer[] attackDIR;

    public void Initialize_Player_EquippedSkills(List<string> playerInfo)
    {
        for (int i = 0; i < 3; i++)
        {
            __PLY_Selected_Skills.Add(File_IO.IO_CSV.__Get_Searched_SkillBaseStat(playerInfo[i + 17]));
        }
    }

    public void ReInit_Player_EquippedSkills(List<SkillBaseStat> playerSkillInfo)
    {
        for (int i = 0; i < 3; i++)
        {
            __PLY_Selected_Skills[i] = playerSkillInfo[i];
        }

        transform.GetComponent<Player_Info_Manager>().Write_Player_Info(transform.GetComponent<Player_Info_Manager>().__GET_playerInfo._GET_saveSlotNum);
    }

    //나중엔 DB에서 긁어온 값을 여기서 초기화할 것.
    void Awake()
    {
        
        //이속, 회전속도, 체력, 마나, 파워 게이지, 공격력, 크리확률, 크리계수
        //__PLY_Stat.SampleInit(10.0f, 30.0f, 10, 10, 10, 1, 0.1f, 2.0f);

        //UnitBaseEngine에 Player라고 알려준다.
        __PLY_Engine = transform.GetComponent<UnitBaseEngine>();

        //Unit__Base_Combat_Engine이 Unit__Base_Movement_Engine과 __PLY_SKill_Engine에 접근 할 수 있도록 한다.
        __PLY_Engine.playerController = this;
        __PLY_Engine._unit_Combat_Engine.__SET_unit_Base_Engine = __PLY_Engine;
        __PLY_Engine._unit_Move_Engine._SET_unit_Base_Engine = __PLY_Engine;
        __PLY_Engine._unit_Move_Engine.movingEffect = this.movingEffect;
        //UnitBaseEngine이 Unit__Base_Stat 내용에 접근할 수 있도록 한다.
        __PLY_Engine._unit_Stat = __PLY_Stat;

        //----------------------------------------------------------------------------------------

        ////확인용. 정상적으로 작동함.
        //foreach (SkillBaseStat forDebug in __PLY_Selected_Skills)
        //{
        //    Debug.Log("ID: " + forDebug.__Get_Skill_ID + ", Name: " + forDebug.__GET_Skill_Name + ", Rate:" + forDebug.__GET_Skill_Rate);
        //    Debug.Log("CoolT: " + forDebug.__GET_Skill_Cool_Time + ", IngT: " + forDebug.__GET_Skill_ING_Time + ", UseAmount:" + forDebug.__GET_Skill_Use_Amount);
        //    Debug.Log("Code_Main: " + forDebug.__GET_Skill_Code_M + ", Code_Sub: " + forDebug.__GET_Skill_Code_S + ", Code_Time:" + forDebug.__GET_Skill_Code_T);
        //}

        //궁극기 임시 조치
        debugUltimateSkill = File_IO.IO_CSV.__Get_Searched_SkillBaseStat("FIN000");

        _Is_On_CoolTime__Default_ATK = true;

        for (int i = 0; i < _Is_On_CoolTime_Skill.Length; i++)
        {
            _Is_On_CoolTime_Skill[i] = true;
        }
    }

    // Use this for initialization
    void Start ()
    {
        //transform.GetComponent<AudioSource>().Stop();
        try
        {
            attackDIR[0].enabled = true;
            attackDIR[1].enabled = false;
            attackDIR[2].enabled = false;
        }
        catch (System.Exception) { }

        //플레이어가 투사체를 발사할 위치를 앞부분으로 우선 설정한다.
        index_OF_playerAttackerTransforms = 0;
        playerAttacker = playerAttackerTransforms[index_OF_playerAttackerTransforms];

        //해당 스크립트와 CombatEngine에서 모두 사용하기 위해 이렇게 초기화하여 전달한다.
        __PLY_CoolTimer = transform.GetComponent<UnitCoolTimer>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //플레이어가 조작을 해도 되는 상황일 때
        if (!playerMustBeFreeze)
        {
            if (__PLY_Stat.__PUB__Health_Point > 0)
            {
                //투사체를 발사하는 위치 변경, Q키를 눌러 전환한다.
                //4~5차 단계에서 UI를 추가할 것
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    transform.GetComponent<AudioSource>().Play();
                    //플레이어가 투사체를 발사할 위치가 앞 또는 우측인 경우
                    if (index_OF_playerAttackerTransforms < 2 && index_OF_playerAttackerTransforms >= 0)
                    {
                        //인덱스 값을 더해서 다음 위치로 바꾼다.
                        index_OF_playerAttackerTransforms++;
                    }
                    //플레이어가 투사체를 발사할 위치가 좌측인 경우
                    else
                    {
                        //다시 앞쪽으로 돌아간다.
                        index_OF_playerAttackerTransforms = 0;
                    }

                    try
                    {
                        for (int i = 0; i < 3; i++)
                            attackDIR[i].enabled = false;

                        attackDIR[index_OF_playerAttackerTransforms].enabled = true;
                    }
                    catch (System.Exception) { }

                    //설정된 투사체 발사 위치를 적용한다.
                    playerAttacker = playerAttackerTransforms[index_OF_playerAttackerTransforms];
                }

                if (transform.position.y >= 5.0f)
                {
                    transform.position.Set(transform.position.x, 5.0f, transform.position.z);
                }

                //기본 이동
                if (Input.GetKey(KeyCode.W))
                {
                    __PLY_Engine._unit_Move_Engine.Move_OBJ(__PLY_Stat.__PUB_Move_Speed, ref playerTransform, 1);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    __PLY_Engine._unit_Move_Engine.Move_OBJ(__PLY_Stat.__PUB_Move_Speed, ref playerTransform, -1f);
                }
                else
                {
                    if(movingEffect.isPlaying)
                        movingEffect.Stop();
                }

                if (Input.GetKey(KeyCode.A))
                {
                    __PLY_Engine._unit_Move_Engine.Rotate_OBJ(__PLY_Stat.__PUB_Rotation_Speed, ref playerTransform, -1);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    __PLY_Engine._unit_Move_Engine.Rotate_OBJ(__PLY_Stat.__PUB_Rotation_Speed, ref playerTransform, 1);
                }
                else
                { }

                //if (Input.GetMouseButtonDown(1))
                //{
                //    //임시코드(GUI - CH)
                //    __PLY_Stat.__PUB__Health_Point -= 1;
                //    __PLY_Stat.__PUB__Mana_Point -= 1;
                //    __PLY_Stat.__PUB__Power_Point -= 1;
                //    Debug.Log("Click Right");
                //    //임시코드(GUI - CH)
                //}

                //if (Input.GetMouseButtonUp(1))
                //{
                //    Debug.Log("Mouse Up!");
                //}

                //기본 공격
                //마우스 좌클릭 -> 전면 공격
                //마우스 좌클릭 && 기본 공격 쿨타임 끝남 && 마우스로 구조물 설치하는 스킬을 사용하고 있지 않을 때
                if (Input.GetMouseButtonDown(0) && _Is_On_CoolTime__Default_ATK && !(_SPW_MOS_Skill_Activated))
                {

                    __PLY_Engine._unit_Combat_Engine.Default_ATK(ref playerAttacker, (SkillBaseStat)null);

                    //쿨타임을 사용하기 위한 코루틴. 따로 외부 클래스 제작함. 상세 항목은 해당 클래스 참조
                    //나중에 쿨타임 값 같은 것도 따로 관리할 것
                    __PLY_CoolTimer.StartCoroutine(
                        __PLY_CoolTimer.Timer(
                            1.0f,
                            (input) => { _Is_On_CoolTime__Default_ATK = input; },
                            _Is_On_CoolTime__Default_ATK,
                            (input) => { default_ATK_Remained_Time = input; }
                            )
                        );
                }

                //Debug.Log(__PLY_Selected_Skills[0].time);

                //마우스로 무언가를 설치해야 되는 스킬을 사용했고 아직 설치가 안 됬다면 모든 스킬을 사용할 수 없음
                //1번 스킬
                if (Input.GetKeyDown(KeyCode.Alpha1) && !(_SPW_MOS_Skill_Activated))
                {
                    PLY_Controller_Using_Skill(0);
                }
                //2번 스킬
                else if (Input.GetKeyDown(KeyCode.Alpha2) && !(_SPW_MOS_Skill_Activated))
                {
                    PLY_Controller_Using_Skill(1);
                }
                //3번 스킬
                else if (Input.GetKeyDown(KeyCode.Alpha3) && !(_SPW_MOS_Skill_Activated))
                {
                    PLY_Controller_Using_Skill(2);
                }
                //// for test
                //else if (Input.GetKeyDown(KeyCode.Alpha4) && !(_SPW_MOS_Skill_Activated))
                //{
                //    __PLY_Engine._unit_Combat_Engine.Using_Skill(ref playerAttacker, debugUltimateSkill, true);
                //}
                else
                { }

                __PLY_Engine.ManageBUF();
            }
            //플레이어 체력이 0이하이면
            else
            {
                //플레이어가 죽었다고 알려주고 파괴한다.
                GameObject.Find("GameManager").GetComponent<MapManager>().PlayerDead();
                Destroy(gameObject);
            }
        }
    }

    private void PLY_Controller_Using_Skill(int index)
    {
        //쿨타임이 지났을 때
        if (_Is_On_CoolTime_Skill[index])
        {
            //마나가 충분할 때
            if (__PLY_Stat.__Is_Mana_Enough(__PLY_Selected_Skills[index]))
            {
                //필요한 마나 소모
                __PLY_Stat.__Get_HIT__About_Mana(__PLY_Selected_Skills[index].__GET_Skill_Use_Amount, 1);

                //UnitBaseEngine.Using_Skill에서 스킬 기능 처리
                __PLY_Engine._unit_Combat_Engine.Using_Skill(ref playerAttacker, __PLY_Selected_Skills[index], true);
                //쿨타임 관련 처리
                __PLY_CoolTimer.StartCoroutine(
                    __PLY_CoolTimer.Timer(  __PLY_Selected_Skills[index].__GET_Skill_Cool_Time,
                    (input) => { _Is_On_CoolTime_Skill[index] = input; }, _Is_On_CoolTime_Skill[index],
                    (input) => { __PLY_Selected_Skills[index].time = input; }  )
                    );
            }
            //마나가 부족할 때
            else
            {
                Debug.Log("Not Enough Mana");
            }
        }
        //쿨타임이 아직 안 지났을 때
        else
        {
            //Debug.Log("CoolTime is not Over");
        }
    }

    //플레이어가 디버프 스킬에 피격받았을 때의 함수
    public void _Player_GET_DeBuff(SkillBaseStat whichDeBuffSkill_Hit_Player)
    {
        //Debug.Log("================================================================");
        __PLY_Engine._unit_Combat_Engine.Using_Skill(ref playerAttacker, whichDeBuffSkill_Hit_Player, false);
        isPlayerHitUltimateSkill = true;
    }

    public void _Player_GetOut_FROM_UltimateSkill()
    {
        isPlayerHitUltimateSkill = false;
    }

    //플레이어가 피격 받을 때
    public void _Player_Get_Hit(int damage)
    {
        Debug.Log("Player Get Hit");
        __PLY_Stat.__GET_HIT__About_Health(damage, 1);
    }
}
