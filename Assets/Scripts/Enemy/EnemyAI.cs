using UnityEngine;

using System.Collections;

using System.Collections.Generic;

using File_IO;
using PMS_Math;
using PMS_AISystem;

//EnemyAI에 직접적으로 관련된 클래스를 새로 생성함
public class EnemyAI : MonoBehaviour {

    private EnemyController enemyController;

    private EnemyDataCollector enemyCollector;

    private EnemyStat __ENE_Stat;
    private EnemyAIEngine __ENE_AI_Engine;

    //나중엔 파일에서 읽어오도록 변경될 수도 있음.
    //외부의 딥러닝 프로그램이 함수를 호출할 때 사용하기 위한 함수 List
    private List<System.Action> behaveList_Move = new List<System.Action>();
    private List<System.Action> behaveList_Rotate = new List<System.Action>();
    private List<System.Action> behaveList_Attack = new List<System.Action>();

    //페이즈 구분 용
    private int behaveIndex;
    private int behaveCounter;

    //AI_DeapLearning__Random_Ver에서 사용할 변수들
    private bool isbehaveCoolTimeOn = true;
    public bool __GET_isbehaveCoolTimeOn { get { return isbehaveCoolTimeOn; } }

    private IntVector3 realIndex = new IntVector3(-1,-1,-1);
    private SituationCUR sitCUR = new SituationCUR("NULL", -1f, -1f, -1f, -1f, new IntVector3(-1,-1,-1), -1f);

    private bool isEnd_OF_Stage = false;
    private Transform middle_OF_Stage;

    float beforeDist = -1f;
    string beforeBehaveID = "NULL";
    int beforeDistTOInt = -1;
    string behaveID = "NULL";
    public int hitCounter = 0;
    bool isHitB = false;
    public bool _SET_isHitB { set { isHitB = value; } }

    //SituationCUR sitCUR = new SituationCUR("NULL", -1, -1, -1, -1, new IntVector3(-1, -1, -1), false);
    //SituationAFT sitAFT = new SituationAFT("NULL", -1, -1, -1, -1, "NULL", -1, -1, false, false);

    //20190310
    ////이속 버프 & 디버프 중첩 방지가 작동하는 지 알아보기 위한 임시 변수
    //private SkillBaseStat sampleSkillTest;

    //private bool sampleSkillCoolTime;

    // Use this for initialization
    void Awake () {
        enemyController = transform.GetComponent<EnemyController>();

        __ENE_Stat = enemyController.__ENE_Stat;
        __ENE_AI_Engine = enemyController._GET__ENE_AI_Engine;

        middle_OF_Stage = GameObject.Find("Middle_OF_Stage").transform;

        behaveIndex = 0;

        ////이속 디버프 스킬을 기본으로 사용하도록 임시로 지정한다.
        //sampleSkillTest = IO_CSV.__Get_Searched_SkillBaseStat("00000003");
        //sampleSkillCoolTime = true;

        //이동에 관한 함수 리스트
        //이동 없음
        behaveList_Move.Add(() => AI_DO_Nothing());
        //전방이동
        behaveList_Move.Add(() => __ENE_AI_Engine.__ENE_Engine._unit_Move_Engine.Move_OBJ(__ENE_Stat.__PUB_Move_Speed, ref enemyController.enemyTransform, 1));
        //후방이동
        behaveList_Move.Add(() => __ENE_AI_Engine.__ENE_Engine._unit_Move_Engine.Move_OBJ(__ENE_Stat.__PUB_Move_Speed, ref enemyController.enemyTransform, -1));

        //회전에 관한 함수 리스트
        //회전 없음
        behaveList_Rotate.Add(() => AI_DO_Nothing());
        //시계방향 회전
        behaveList_Rotate.Add(() => __ENE_AI_Engine.__ENE_Engine._unit_Move_Engine.Rotate_OBJ(__ENE_Stat.__PUB_Rotation_Speed, ref enemyController.enemyTransform, 1));
        //반시계방향 회전
        behaveList_Rotate.Add(() => __ENE_AI_Engine.__ENE_Engine._unit_Move_Engine.Rotate_OBJ(__ENE_Stat.__PUB_Rotation_Speed, ref enemyController.enemyTransform, -1));
        //플레이어를 정면으로 바라볼 때까지 회전
        behaveList_Rotate.Add(() => __ENE_AI_Engine.Rotate_TO_Direction(__ENE_Stat.__PUB_Rotation_Speed, ref enemyController.enemyTransform, enemyController.playerTransform, false));
        //플레이어를 우측으로 바라볼 때까지 회전
        behaveList_Rotate.Add(() => __ENE_AI_Engine.Rotate_TO_Direction(__ENE_Stat.__PUB_Rotation_Speed, ref enemyController.enemyTransform, enemyController.playerTransform, false, enemyController.enemy_Right));
        //플레이어를 좌측으로 바라볼 때까지 회전
        behaveList_Rotate.Add(() => __ENE_AI_Engine.Rotate_TO_Direction(__ENE_Stat.__PUB_Rotation_Speed, ref enemyController.enemyTransform, enemyController.playerTransform, false, enemyController.enemy_Left));
        //플레이어 반대 방향 바라볼 때까지 회전
        behaveList_Rotate.Add(() => __ENE_AI_Engine.Rotate_TO_Direction(__ENE_Stat.__PUB_Rotation_Speed, ref enemyController.enemyTransform, enemyController.playerTransform, true));

        //공격에 관한 함수 리스트
        //공격 없음
        behaveList_Attack.Add(() => AI_DO_Nothing());
        //정면 일반 공격
        behaveList_Attack.Add(() => __ENE_AI_Engine.Attack_Default(2.0f, ref enemyController.enemy_Front, __ENE_Stat, 1));
        //우측 일반 공격
        behaveList_Attack.Add(() => __ENE_AI_Engine.Attack_Default(2.0f, ref enemyController.enemy_Right, __ENE_Stat, 1));
        //좌측 일반 공격
        behaveList_Attack.Add(() => __ENE_AI_Engine.Attack_Default(2.0f, ref enemyController.enemy_Left, __ENE_Stat, 1));

        realIndex.InitIntVector3(0,0,0);
    }

    void Start()
    {
        enemyCollector = GameObject.FindGameObjectWithTag("GameController").GetComponent<EnemyDataCollector>();
    }

    //가장 단순한 수준의 AI
    public void AI_Simple_Level0()
    {
        //일반적인 상황일 때
        if (!isEnd_OF_Stage)
        {
            //체력이 최대 체력의 절반 이하일 때 (도망가야할 때)
            if (enemyController.__ENE_Stat.__PUB__Health_Point <= enemyController.__ENE_Stat.half_HP)
            {
                //플레이어 반대 방향을 보도록 한다.
                __ENE_AI_Engine.Rotate_TO_Direction(enemyController.__ENE_Stat.__PUB_Rotation_Speed, ref enemyController.enemyTransform, enemyController.playerTransform, true);

                //일정 시간동안 해당 유닛의 전방을 향해 이동한다.
                if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[0])
                {
                    __ENE_AI_Engine.__ENE_Engine._unit_Move_Engine.Move_OBJ(enemyController.__ENE_Stat.__PUB_Move_Speed, ref enemyController.enemyTransform, 1);
                    //4초 동안 퇴각
                    StartCoroutine(enemyController.enemyCoolTimer.Timer_Do_Once(4.0f, (input) => { __ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[0] = input; }, true));
                }
                else
                {
                    //16초 동안 정지
                    StartCoroutine(enemyController.enemyCoolTimer.Timer_Do_Once(16.0f, (input) => { __ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[0] = input; }, false));
                    //회복 패턴은 정예 선박만 넣는것이 좋을 것 같다
                    //StartCoroutine(__ENE_Stat.__Get_HIT__About_Health_FREQ(2.0f, 1.0f, 1, -1));
                }

            }
            //체력이 최대 체력의 절반을 초과할 때 (공격해야할 때)
            else
            {

                //플레이어를 바라보도록 한다.
                __ENE_AI_Engine.Rotate_TO_Direction(__ENE_Stat.__PUB_Rotation_Speed, ref enemyController.enemyTransform, enemyController.playerTransform, false);

                //전방으로 이동한다.
                __ENE_AI_Engine.Go_TO_Foward_UNTIL_RayHit(__ENE_Stat.__PUB_Move_Speed, ref enemyController.enemyTransform, enemyController.playerTransform);

                //각도 차에 따라 다른 위치로 발사한다. (아직 미구현)
                if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[1])
                {


                    //플레이어를 거의 정면으로 바라보고 있을 때
                    //if(curAngleComparison <= 10.0f)
                    //쿨타임에 랜덤변수를 더해서 난이도를 조금 올린다.
                    __ENE_AI_Engine.Attack_Default(2.0f + Random.Range(0.0f, 1.0f), ref enemyController.enemy_Front, __ENE_Stat, 1);

                    ////20190310
                    ////이속 디버프 스킬을 기본 공격으로 사용하도록 임시로 테스트 한다.
                    ////같은 스탯에 대한 버프, 디버프 중첩 방지 성공
                    //if (sampleSkillCoolTime)
                    //{
                    //    __ENE_AI_Engine.__ENE_Engine._unit_Combat_Engine.Using_Skill(ref enemyController.enemy_Front, sampleSkillTest, true);

                    //    //쿨타임 관련 처리를 해준다
                    //    StartCoroutine(
                    //        enemyController.enemyCoolTimer.Timer(sampleSkillTest.__GET_Skill_Cool_Time,
                    //        (input) => { sampleSkillCoolTime = input; }, sampleSkillCoolTime,
                    //        (input) => { sampleSkillTest.time = input; })
                    //        );
                    //}

                    //플레이어가 적의 좌측에 있을 때
                    //else if(curAngleComparison <= 100.0f && curAngleComparison >= 80.0f && GET_RotataionDir(curAngle, destiAngle))
                    //__ENE_AI_Engine.Attack_Default(2.0f + Random.Range(0.0f, 1.0f), ref enemyController.enemy_Left, __ENE_Stat, 1);
                    //플레이어가 적의 우측에 있을 때
                    //else if(  curAngleComparison <= 100.0f && curAngleComparison >= 80.0f && !( GET_RotataionDir(curAngle, destiAngle) )  )
                    //__ENE_AI_Engine.Attack_Default(2.0f + Random.Range(0.0f, 1.0f), ref enemyController.enemy_Right, __ENE_Stat, 1);
                }
                //측면 공격
                //if (__ENE_Engine._PUB_enemy_Is_ON_CoolTime[2])
                //{
                //    __ENE_Engine.Attack_Default(2.0f, ref default_Ammo, ref enemy_Right, __ENE_Stat.__PUB_ATK__Val, 1);
                //}
                //if (__ENE_Engine._PUB_enemy_Is_ON_CoolTime[3])
                //{
                //    __ENE_Engine.Attack_Default(2.0f, ref default_Ammo, ref enemy_Left, __ENE_Stat.__PUB_ATK__Val, 2);
                //}
            }
        }
        //스테이지 경계선과 접촉했을 때
        else
        {
            Go_INTO_Stage();
            //7초 후엔 스테이지 경계선과 접촉하지 않은 것으로 간주한다.
            StartCoroutine(enemyController.enemyCoolTimer.Timer_Do_Once(7.0f, (input) => { isEnd_OF_Stage = input; }, true));
        }
    }

    //체력과 상관없이 Player를 계속 공격하는 AI_Simple_Level0기반의 AI
    public void AI_Simple_Level0_WITH_BOSS()
    {

        //일반적인 상황일 때
        if (!isEnd_OF_Stage)
        {
            //플레이어를 바라보도록 한다.
            __ENE_AI_Engine.Rotate_TO_Direction(__ENE_Stat.__PUB_Rotation_Speed, ref enemyController.enemyTransform, enemyController.playerTransform, false);

            //전방으로 이동한다.
            __ENE_AI_Engine.Go_TO_Foward_UNTIL_RayHit(__ENE_Stat.__PUB_Move_Speed, ref enemyController.enemyTransform, enemyController.playerTransform);

            //각도 차에 따라 다른 위치로 발사한다. (아직 미구현)
            if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[1])
            {


                //플레이어를 거의 정면으로 바라보고 있을 때
                //if(curAngleComparison <= 10.0f)
                //쿨타임에 랜덤변수를 더해서 난이도를 조금 올린다.
                __ENE_AI_Engine.Attack_Default(2.0f + Random.Range(0.0f, 1.0f), ref enemyController.enemy_Front, __ENE_Stat, 1);

                ////20190310
                ////이속 디버프 스킬을 기본 공격으로 사용하도록 임시로 테스트 한다.
                ////같은 스탯에 대한 버프, 디버프 중첩 방지 성공
                //if (sampleSkillCoolTime)
                //{
                //    __ENE_AI_Engine.__ENE_Engine._unit_Combat_Engine.Using_Skill(ref enemyController.enemy_Front, sampleSkillTest, true);

                //    //쿨타임 관련 처리를 해준다
                //    StartCoroutine(
                //        enemyController.enemyCoolTimer.Timer(sampleSkillTest.__GET_Skill_Cool_Time,
                //        (input) => { sampleSkillCoolTime = input; }, sampleSkillCoolTime,
                //        (input) => { sampleSkillTest.time = input; })
                //        );
                //}

                //플레이어가 적의 좌측에 있을 때
                //else if(curAngleComparison <= 100.0f && curAngleComparison >= 80.0f && GET_RotataionDir(curAngle, destiAngle))
                //__ENE_AI_Engine.Attack_Default(2.0f + Random.Range(0.0f, 1.0f), ref enemyController.enemy_Left, __ENE_Stat, 1);
                //플레이어가 적의 우측에 있을 때
                //else if(  curAngleComparison <= 100.0f && curAngleComparison >= 80.0f && !( GET_RotataionDir(curAngle, destiAngle) )  )
                //__ENE_AI_Engine.Attack_Default(2.0f + Random.Range(0.0f, 1.0f), ref enemyController.enemy_Right, __ENE_Stat, 1);
            }
            //측면 공격
            //if (__ENE_Engine._PUB_enemy_Is_ON_CoolTime[2])
            //{
            //    __ENE_Engine.Attack_Default(2.0f, ref default_Ammo, ref enemy_Right, __ENE_Stat.__PUB_ATK__Val, 1);
            //}
            //if (__ENE_Engine._PUB_enemy_Is_ON_CoolTime[3])
            //{
            //    __ENE_Engine.Attack_Default(2.0f, ref default_Ammo, ref enemy_Left, __ENE_Stat.__PUB_ATK__Val, 2);
            //}
        }
        //스테이지 경계선과 접촉했을 때
        else
        {
            Go_INTO_Stage();
            //7초 후엔 스테이지 경계선과 접촉하지 않은 것으로 간주한다.
            StartCoroutine(enemyController.enemyCoolTimer.Timer_Do_Once(7.0f, (input) => { isEnd_OF_Stage = input; }, true));
        }
    }

    public void AI_Simple_Level0_BOSS()
    {  
        //일반적인 상황일 때
        if (!isEnd_OF_Stage)
        {
            //체력이 최대 체력의 절반 초과일 때 (1페이즈)
            //AI_Simple_Level0 + 측면 
            if (enemyController.__ENE_Stat.__PUB__Health_Point > enemyController.__ENE_Stat.half_HP)
            {
                //플레이어에게 접근 후 정면 공격
                if (behaveIndex == 0)
                {
                    //플레이어를 바라보도록 한다.
                    __ENE_AI_Engine.Rotate_TO_Direction(__ENE_Stat.__PUB_Rotation_Speed, ref enemyController.enemyTransform, enemyController.playerTransform, false);

                    //전방으로 이동한다.
                    __ENE_AI_Engine.Go_TO_Foward_UNTIL_RayHit(__ENE_Stat.__PUB_Move_Speed, ref enemyController.enemyTransform, enemyController.playerTransform);

                    //정면 공격 수행
                    if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[2])
                    {
                        __ENE_AI_Engine.Attack_Default(2.0f + Random.Range(0.0f, 1.0f), ref enemyController.enemy_Front, __ENE_Stat, 2);
                        behaveCounter++;
                    }

                    //정면 공격 3회 수행 후 행동 패턴 변경
                    if (behaveCounter >= 3) { behaveIndex = 1; behaveCounter = 0; }
                }
                //전방으로 2초 동안 이동 후 우현으로 플레이어 공격
                else if (behaveIndex == 1)
                {
                    //전방으로 2초 동안 이동
                    if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[1])
                    {
                        __ENE_AI_Engine.__ENE_Engine._unit_Move_Engine.Move_OBJ(__ENE_Stat.__PUB_Move_Speed, ref enemyController.enemyTransform, 1);
                        StartCoroutine(transform.GetComponent<UnitCoolTimer>().Timer_Do_Once(2.0f, (input) => { __ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[1] = input; }, true));
                    }
                    else
                    {
                        //플레이어를 바라보도록 한다.
                        __ENE_AI_Engine.Rotate_TO_Direction(__ENE_Stat.__PUB_Rotation_Speed, ref enemyController.enemyTransform, enemyController.playerTransform, false, enemyController.enemy_Right);

                        //충분히 가까워질 때까지 느리게 전방으로 이동한다.
                        __ENE_AI_Engine.Go_TO_Foward_UNTIL_RayHit(__ENE_Stat.__PUB_Move_Speed * 0.3f, ref enemyController.enemyTransform, enemyController.playerTransform);

                        //우현 기본 공격
                        if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[2] && !(__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[3]))
                        {
                            __ENE_AI_Engine.Attack_Default(2.0f + Random.Range(0.0f, 1.0f), ref enemyController.enemy_Right, __ENE_Stat, 2);
                        }
                        //우현 산탄 스킬
                        else if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[3])
                        {
                            EnemyUsingSkill(3, enemyController.enemy_Right, __ENE_Stat._GET_enemySkillList[0]);
                            behaveCounter++;
                        }

                        //우현 산탄 스킬 3회 시전 후 행동 패턴을 behave == 0으로 변경, behaveCounter 초기화, 일정 시간동안 전방이동 bool 초기화
                        if (behaveCounter >= 3) { behaveIndex = 2; behaveCounter = 0; __ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[1] = true; }
                    }
                }
                //전방으로 2초 동안 이동 후 좌현으로 플레이어 공격
                else if (behaveIndex == 2)
                {
                    if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[1])
                    {
                        __ENE_AI_Engine.__ENE_Engine._unit_Move_Engine.Move_OBJ(__ENE_Stat.__PUB_Move_Speed, ref enemyController.enemyTransform, 1);
                        StartCoroutine(transform.GetComponent<UnitCoolTimer>().Timer_Do_Once(2.0f, (input) => { __ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[1] = input; }, true));
                    }
                    else
                    {
                        //플레이어를 바라보도록 한다.
                        __ENE_AI_Engine.Rotate_TO_Direction(__ENE_Stat.__PUB_Rotation_Speed, ref enemyController.enemyTransform, enemyController.playerTransform, false, enemyController.enemy_Left);

                        //충분히 가까워질 때까지 느리게 전방으로 이동한다.
                        __ENE_AI_Engine.Go_TO_Foward_UNTIL_RayHit(__ENE_Stat.__PUB_Move_Speed * 0.3f, ref enemyController.enemyTransform, enemyController.playerTransform);

                        //좌현 기본 공격
                        if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[2] && !(__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[3]))
                        {
                            __ENE_AI_Engine.Attack_Default(2.0f + Random.Range(0.0f, 1.0f), ref enemyController.enemy_Left, __ENE_Stat, 2);
                        }
                        //좌현 산탄 스킬
                        else if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[3])
                        {
                            EnemyUsingSkill(3, enemyController.enemy_Left, __ENE_Stat._GET_enemySkillList[0]);
                            behaveCounter++;
                        }

                        //좌현 산탄 스킬 3회 시전 후 행동 패턴을 behave == 0으로 변경, behaveCounter 초기화, 일정 시간동안 전방이동 bool 초기화
                        if (behaveCounter >= 3) { behaveIndex = 0; behaveCounter = 0; __ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[1] = true; }
                    }
                }
                else
                {

                }
            }
            //체력이 최대 체력의 절반 이하할 때 (2페이즈)
            //기모으기 + 폭풍우
            else
            {
                //Debug.Log(__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[4]);
                //4초 동안 해당 유닛의 전방을 향해 이동한다.
                if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[0])
                {
                    //플레이어 반대 방향을 보도록 한다.
                    __ENE_AI_Engine.Rotate_TO_Direction(enemyController.__ENE_Stat.__PUB_Rotation_Speed, ref enemyController.enemyTransform, enemyController.playerTransform, true);

                    __ENE_AI_Engine.__ENE_Engine._unit_Move_Engine.Move_OBJ(enemyController.__ENE_Stat.__PUB_Move_Speed, ref enemyController.enemyTransform, 1);
                    //4초 동안 퇴각
                    StartCoroutine(enemyController.enemyCoolTimer.Timer_Do_Once(4.0f, (input) => { __ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[0] = input; }, true));
                }
                else
                {
                    if (behaveIndex != 4)
                    {
                        //궁극기 게이지가 부족하고 스킬을 사용할 수 있으면
                        if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[4] && __ENE_Stat.__PUB__Power_Point < __ENE_Stat.__GET_Max_PP)
                        {
                            //게이지를 모은다.
                            EnemyUsingSkill(4, enemyController.enemy_Left, __ENE_Stat._GET_enemySkillList[1]);
                        }
                        //게이지 모을 때까지 쿨타임이 남았다면
                        else if (!__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[4] && __ENE_Stat.__PUB__Power_Point < __ENE_Stat.__GET_Max_PP)
                        {
                            //도망친다.
                            __ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[0] = true;
                        }
                        //궁극기 게이지가 충분하고 궁극기를 사용할 수 있으면
                        else if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[5] && __ENE_Stat.__PUB__Power_Point == __ENE_Stat.__GET_Max_PP)
                        {
                            //플레이어와 일정거리 이하일 때
                            if (Vector3.Distance(transform.position, enemyController.playerTransform.position) <= 10f)
                            {
                                //궁극기를 사용한다.
                                EnemyUsingSkill(5, enemyController.enemy_Front, __ENE_Stat._GET_enemySkillList[2]);
                                StartCoroutine(enemyController.enemyCoolTimer.Timer(__ENE_Stat._GET_enemySkillList[2].__GET_Skill_ING_Time, (input) => { __ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[6] = input; }, true));
                                behaveIndex = 4;
                            }
                            //플레이어와 일정거리 이상일 때
                            else
                            {
                                //플레이어에게 접근한다.
                                __ENE_AI_Engine.Rotate_TO_Direction(enemyController.__ENE_Stat.__PUB_Rotation_Speed, ref enemyController.enemyTransform, enemyController.playerTransform, false);

                                __ENE_AI_Engine.__ENE_Engine._unit_Move_Engine.Move_OBJ(enemyController.__ENE_Stat.__PUB_Move_Speed, ref enemyController.enemyTransform, 1);
                            }
                        }
                    }
                    else if (behaveIndex == 4)
                    {
                        //플레이어가 궁극기에서 벗어났거나 궁극기가 끝났으면
                        if (!(enemyController.playerTransform.GetComponent<PlayerController>()._GET_isPlayerHitUltimateSkill) || __ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[6])
                        {
                            //궁극기 게이지를 모으도록 한다.
                            behaveIndex = 0;
                            //행동 bool 변수 초기화
                            __ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[0] = true;
                            __ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[6] = true;
                        }
                        //플레이어가 궁극기에서 못 벗어나면
                        else
                        {
                            //플레이어를 정면으로 바라보면서
                            __ENE_AI_Engine.Rotate_TO_Direction(enemyController.__ENE_Stat.__PUB_Rotation_Speed, ref enemyController.enemyTransform, enemyController.playerTransform, false);

                            //정면 기본 공격
                            if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[2] && !(__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[3]))
                            {
                                __ENE_AI_Engine.Attack_Default(2.0f + Random.Range(0.0f, 1.0f), ref enemyController.enemy_Front, __ENE_Stat, 2);
                            }
                            //정면 산탄 스킬
                            else if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[3])
                            {
                                EnemyUsingSkill(3, enemyController.enemy_Front, __ENE_Stat._GET_enemySkillList[0]);
                            }
                        }
                    }
                }
            }
        }
        //스테이지 경계선과 접촉했을 때
        else
        {
            Go_INTO_Stage();
            //7초 후엔 스테이지 경계선과 접촉하지 않은 것으로 간주한다.
            StartCoroutine(enemyController.enemyCoolTimer.Timer_Do_Once(7.0f, (input) => { isEnd_OF_Stage = input; }, true));
        }
    }

    //Boundary와 충돌했을 때 스테이지 중앙 근처로 이동시키는 함수
    //이름이 "Middle_OF_Stage"인 오브젝트가 있어야 정상 작동한다.
    void Go_INTO_Stage()
    {
        //3초 동안 Middle_OF_Stage 방향으로 회전하면서 후진한다.
        if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[1])
        {
            __ENE_AI_Engine.Rotate_TO_Direction(__ENE_Stat.__PUB_Rotation_Speed, ref enemyController.enemyTransform, middle_OF_Stage, false);

            __ENE_AI_Engine.__ENE_Engine._unit_Move_Engine.Move_OBJ(__ENE_Stat.__PUB_Move_Speed, ref enemyController.enemyTransform, -1);

            StartCoroutine(enemyController.enemyCoolTimer.Timer_Do_Once(3.0f, (input) => { __ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[1] = input; }, true));
        }
        //3초 동안 전방으로 이동한다.
        else
        {
            __ENE_AI_Engine.__ENE_Engine._unit_Move_Engine.Move_OBJ(__ENE_Stat.__PUB_Move_Speed, ref enemyController.enemyTransform, 1);
            StartCoroutine(enemyController.enemyCoolTimer.Timer_Do_Once(3.0f, (input) => { __ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[1] = input; }, false));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Boundary")
        {
            isEnd_OF_Stage = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Boundary")
        {
            //StartCoroutine(enemyController.enemyCoolTimer.Timer_Do_Once(6.0f, (input) => { isEnd_OF_Stage = input; }, true));
        }
    }

    private void EnemyUsingSkill(int index, Transform enemyAttacker, SkillBaseStat whichSkill)
    {
        //쿨타임이 지났을 때
        if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[index])
        {
            //마나가 충분할 때
            if (__ENE_Stat.__Is_Mana_Enough(whichSkill))
            {
                //필요한 마나 또는 궁극기 게이지 소모
                if(whichSkill.__GET_Skill_Code_M != SkillBaseCode._SKILL_CODE_Main.FIN) __ENE_Stat.__Get_HIT__About_Mana(whichSkill.__GET_Skill_Use_Amount, 1);
                else __ENE_Stat.__Get_HIT__About_Power(whichSkill.__GET_Skill_Use_Amount, 1);

                //UnitBaseEngine.Using_Skill에서 스킬 기능 처리
                __ENE_AI_Engine.__ENE_Engine._unit_Combat_Engine.Using_Skill(ref enemyAttacker, whichSkill, true);
                //쿨타임 관련 처리
                StartCoroutine(
                    enemyController.enemyCoolTimer.Timer(whichSkill.__GET_Skill_Cool_Time,
                    (input) => { __ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[index] = input; }, __ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[index])
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

    //데이터 수집 목적인 랜덤한 행동을 지시하는 함수
    //2019.04.20
    public void AI_DeapLearning__Random_Ver()
    {
        //임시로 추가한 것, P를 누르면 모든 데이터 수집 개체 삭제 및 데이터 저장
        if (Input.GetKeyDown(KeyCode.P))
        {
            Destroy(gameObject);
        }

        int listIndex_Move = (int)(Random.Range(0.0f, (float)(behaveList_Move.Count - 0.1f)));
        int listIndex_Rotate = (int)(Random.Range(0.0f, (float)(behaveList_Rotate.Count - 0.1f)));
        int listIndex_Attack = (int)(Random.Range(0.0f, (float)(behaveList_Attack.Count - 0.1f)));
        //Timer 코루틴을 돌리기 위한 더미 변수
        float dummy = 0.0f;

        float curDist = Vector3.Distance(transform.position, enemyController.playerTransform.position);

        if (isbehaveCoolTimeOn)
        {
            float time = 0.5f + Random.Range(0.0f, 1.0f);
            bool isCloser = false;

            behaveID = System.DateTime.Now.ToString() + ": " + gameObject.GetInstanceID().ToString();
            //Debug.Log("ID: "+behaveID);
            //Debug.Log("Before: "+beforeBehaveID);
            if (beforeDist > curDist) isCloser = true;

            beforeDistTOInt = ((int)(beforeDist / 10f));

            //알고리즘 상의 한계로 발생하는 오류 정정
            //공격을 하지도 않았는데 공격이 성공한 것처럼 기록되는 경우, 이전 행동에서 한 공격이 다음 행동 시작 지점에서 성공했을 때 발생하는 것으로 추정됨.
            //이런 유형의 지난 데이터들을 모두 찾아서 폐기해야됨
            if (hitCounter != 0 && (realIndex.vecZ == 0 || curDist > 17f))
            {
                hitCounter = 0;
                //Debug.Log("Purifying");
            }

            //데이터 모으기
            //사거리 이내 데이터는 따로 복사해서 모은다
            switch ((int)(curDist / 10f))
            {
                case 0:
                    enemyCollector.listSitCUR0.Add(new SituationCUR(behaveID, transform.position.x, transform.position.z, curDist, enemyController._GET__ENE_AI_Engine.angleComp, realIndex, time));
                    enemyCollector.listSitAFT0.Add(new SituationAFT(behaveID, transform.position.x, transform.position.z, curDist, enemyController._GET__ENE_AI_Engine.angleComp, beforeBehaveID, beforeDistTOInt, hitCounter, isCloser));
                    break;
                case 1:
                    enemyCollector.listSitCUR1.Add(new SituationCUR(behaveID, transform.position.x, transform.position.z, curDist, enemyController._GET__ENE_AI_Engine.angleComp, realIndex, time));
                    enemyCollector.listSitAFT1.Add(new SituationAFT(behaveID, transform.position.x, transform.position.z, curDist, enemyController._GET__ENE_AI_Engine.angleComp, beforeBehaveID, beforeDistTOInt, hitCounter, isCloser));
                    break;
            }
            //사거리 외 & 이내 데이터 모두 한 곳에 모은다
            enemyCollector.listSitCUR.Add(new SituationCUR(behaveID, transform.position.x, transform.position.z, curDist, enemyController._GET__ENE_AI_Engine.angleComp, realIndex, time));
            enemyCollector.listSitAFT.Add(new SituationAFT(behaveID, transform.position.x, transform.position.z, curDist, enemyController._GET__ENE_AI_Engine.angleComp, beforeBehaveID, beforeDistTOInt, hitCounter, isCloser));

            realIndex.InitIntVector3(listIndex_Move, listIndex_Rotate, listIndex_Attack);

            beforeBehaveID = behaveID;
            beforeDist = curDist;
            //0.5~1.5초 마다 행동을 갱신한다 (값 변경될 수 있음)
            StartCoroutine(enemyController.enemyCoolTimer.Timer(time, (input) => { isbehaveCoolTimeOn = input; }, true, (input) => { dummy = input; }));

            hitCounter = 0;
        }

        //1차적으로 일반 공격만 생각하도록 한다.
        //공격 쿨타임도 끝났고 공격을 하려는 경우
        if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[1] && realIndex.vecZ != 0)
        {
            //주어진 공격 수행
            behaveList_Attack[realIndex.vecZ]();
        }

        //주어진 이동 수행
        behaveList_Move[realIndex.vecX]();
        //주어진 회전 수행
        behaveList_Rotate[realIndex.vecY]();
    }

    public void AI_DeapLearning__BigData_Ver()
    {
        //임시로 추가한 것, P를 누르면 모든 데이터 수집 개체 삭제 및 데이터 저장
        if (Input.GetKeyDown(KeyCode.M))
        {
            Destroy(gameObject);
        }

        float dummy = 0f;

        if (isbehaveCoolTimeOn)
        {
            float curDist = Vector3.Distance(transform.position, enemyController.playerTransform.position);

            if (hitCounter != 0 && curDist <= 17f && sitCUR._doing.vecZ != 0)
            {
                enemyCollector.listSitCUR.Add(sitCUR);
                //Debug.Log(enemyCollector.listSitCUR[(enemyCollector.listSitCUR.Count - 1)]._id);
            }

            sitCUR = enemyCollector.SearchGoodSitCUR(curDist, enemyController._GET__ENE_AI_Engine.angleComp);
           
            //DataBase에서 긁어온 정보의 time동안 행동한다.
            StartCoroutine(enemyController.enemyCoolTimer.Timer(sitCUR._time, (input) => { isbehaveCoolTimeOn = input; }, true, (input) => { dummy = input; }));
            hitCounter = 0;
        }

        //1차적으로 일반 공격만 생각하도록 한다.
        //공격 쿨타임도 끝났고 공격을 하려는 경우
        if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[1] && sitCUR._doing.vecZ != 0 && sitCUR._doing.vecZ < behaveList_Attack.Count)
        {
            //주어진 공격 수행
            behaveList_Attack[sitCUR._doing.vecZ]();
        }

        //주어진 이동 수행
        behaveList_Move[sitCUR._doing.vecX]();
        //주어진 회전 수행
        behaveList_Rotate[sitCUR._doing.vecY]();
    }

    //랜덤 행동 중 아무것도 안 하는 함수
    private void AI_DO_Nothing(){}
}
