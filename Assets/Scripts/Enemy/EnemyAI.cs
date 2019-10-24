using UnityEngine;

using System.Collections;

using System.Collections.Generic;

using File_IO;
using PMS_Math;
using PMS_AISystem;

//EnemyAI에 직접적으로 관련된 클래스를 새로 생성함
public class EnemyAI : MonoBehaviour
{

    private EnemyController enemyController;

    private EnemyDataCollector enemyCollector;

    private EnemyStat __ENE_Stat;
    private EnemyAIEngine __ENE_AI_Engine;
    private UnitBaseEngine __ENE_Engine;

    //나중엔 파일에서 읽어오도록 변경될 수도 있음.
    //외부의 딥러닝 프로그램이 함수를 호출할 때 사용하기 위한 함수 List
    private List<System.Action> behaveList_Move = new List<System.Action>();
    private List<System.Action> behaveList_Rotate = new List<System.Action>();
    private List<System.Action> behaveList_Attack = new List<System.Action>();

    //페이즈 구분 용
    private int behaveIndex;
    private int behaveCounter;

    //AI_DeepLearning__Random_Ver에서 사용할 변수들
    private bool isbehaveCoolTimeOn = true;
    public bool __GET_isbehaveCoolTimeOn { get { return isbehaveCoolTimeOn; } }

    private IntVector3 realIndex = new IntVector3(-1, -1, -1);
    private SituationCUR sitCUR = new SituationCUR("NULL", -1f, -1f, -1f, -1f, new IntVector3(-1, -1, -1), -1f);

    private bool isEnd_OF_Stage = false;
    private Transform middle_OF_Stage;

    float beforeDist = -1f;
    string beforeBehaveID = "N_U_L_L";
    int beforeDistTOInt = -1;
    float beforeAngleComp = 0.0f;
    Vector2 beforeVec2 = new Vector2(-1f, -1f);
    string behaveID = "NULL";
    [HideInInspector]
    public int hitCounter = 0;
    [HideInInspector]
    int behaveCount_FOR_PPT = 0;
    [HideInInspector]
    int scoreCount_FOR_PPT = 0;
    [HideInInspector]
    int rewardCount = 0;
    [HideInInspector]
    public int getDamagedCounter = 0; bool isHitB = false;
    public bool _SET_isHitB { set { isHitB = value; } }

    public int simpleSpining = 0;

    private float sigma = 0.0f;
    private float moveSpeed = 0.0f;
    private float rotSpeed = 0.0f;
    private bool isHPLOW = false;

    IntVector3 __OLD__forSave = new IntVector3(-1, -1, -1);

    float curDist;
    bool deleteIt = false;
    int qDepth = 0;

    //SituationCUR sitCUR = new SituationCUR("NULL", -1, -1, -1, -1, new IntVector3(-1, -1, -1), false);
    //SituationAFT sitAFT = new SituationAFT("NULL", -1, -1, -1, -1, "NULL", -1, -1, false, false);

    //20190310
    ////이속 버프 & 디버프 중첩 방지가 작동하는 지 알아보기 위한 임시 변수
    //private SkillBaseStat sampleSkillTest;

    //private bool sampleSkillCoolTime;

    // Use this for initialization
    void Awake()
    {
        enemyController = transform.GetComponent<EnemyController>();

        __ENE_Stat = enemyController.__ENE_Stat;
        __ENE_AI_Engine = enemyController._GET__ENE_AI_Engine;
        __ENE_Engine = __ENE_AI_Engine.__ENE_Engine;

        middle_OF_Stage = GameObject.Find("Middle_OF_Stage").transform;

        behaveIndex = 0;

        ////이속 디버프 스킬을 기본으로 사용하도록 임시로 지정한다.
        //sampleSkillTest = IO_CSV.__Get_Searched_SkillBaseStat("00000003");
        //sampleSkillCoolTime = true;
    }

    void Start()
    {
        enemyCollector = GameObject.FindGameObjectWithTag("GameController").GetComponent<EnemyDataCollector>();

        sigma = enemyCollector.sigmaValue;
        //체력 관리
        //enemyController.__ENE_Stat.__PUB__Health_Point = 4;


        moveSpeed = __ENE_Stat.__PUB_Move_Speed;
        rotSpeed = __ENE_Stat.__PUB_Rotation_Speed;

        //이동에 관한 함수 리스트
        //이동 없음
        behaveList_Move.Add(() => AI_DO_Nothing());
        //전방이동
        behaveList_Move.Add(() => __ENE_AI_Engine.__ENE_Engine._unit_Move_Engine.Move_OBJ(moveSpeed, ref enemyController.enemyTransform, 1));
        //후방이동
        behaveList_Move.Add(() => __ENE_AI_Engine.__ENE_Engine._unit_Move_Engine.Move_OBJ(moveSpeed, ref enemyController.enemyTransform, -1));

        //회전에 관한 함수 리스트
        //회전 없음
        behaveList_Rotate.Add(() => AI_DO_Nothing());
        //시계방향 회전
        behaveList_Rotate.Add(() => __ENE_AI_Engine.__ENE_Engine._unit_Move_Engine.Rotate_OBJ(rotSpeed, ref enemyController.enemyTransform, 1));
        //반시계방향 회전
        behaveList_Rotate.Add(() => __ENE_AI_Engine.__ENE_Engine._unit_Move_Engine.Rotate_OBJ(rotSpeed, ref enemyController.enemyTransform, -1));
        //플레이어를 정면으로 바라볼 때까지 회전
        behaveList_Rotate.Add(() => __ENE_AI_Engine.Rotate_TO_Direction(rotSpeed, ref enemyController.enemyTransform, enemyController.playerTransform, false));
        //플레이어를 우측으로 바라볼 때까지 회전
        behaveList_Rotate.Add(() => __ENE_AI_Engine.Rotate_TO_Direction(rotSpeed, ref enemyController.enemyTransform, enemyController.playerTransform, false, enemyController.enemy_Right));
        //플레이어를 좌측으로 바라볼 때까지 회전
        behaveList_Rotate.Add(() => __ENE_AI_Engine.Rotate_TO_Direction(rotSpeed, ref enemyController.enemyTransform, enemyController.playerTransform, false, enemyController.enemy_Left));
        //플레이어 반대 방향 바라볼 때까지 회전
        behaveList_Rotate.Add(() => __ENE_AI_Engine.Rotate_TO_Direction(rotSpeed, ref enemyController.enemyTransform, enemyController.playerTransform, true));

        //공격에 관한 함수 리스트
        //공격 없음
        behaveList_Attack.Add(() => AI_DO_Nothing());
        //정면 일반 공격
        behaveList_Attack.Add(() => __ENE_AI_Engine.Attack_Default(2.0f, ref enemyController.enemy_Front, __ENE_Stat, 1));
        //우측 일반 공격(2번, angleComp > 0)
        behaveList_Attack.Add(() => __ENE_AI_Engine.Attack_Default(2.0f, ref enemyController.enemy_Right, __ENE_Stat, 1));
        //좌측 일반 공격(3번, angleComp < 0)
        behaveList_Attack.Add(() => __ENE_AI_Engine.Attack_Default(2.0f, ref enemyController.enemy_Left, __ENE_Stat, 1));

        realIndex.InitIntVector3(0, 0, 0);


        beforeBehaveID = "NULL";

        try
        {   beforeDist = Vector3.Distance(transform.position, enemyController.playerTransform.position);}
        catch (System.Exception)
        {}
        beforeAngleComp = enemyController._GET__ENE_AI_Engine.angleComp;
        beforeVec2 = new Vector2(transform.position.x, transform.position.z);
    }

    //가장 단순한 수준의 AI
    public void AI_Simple_Level0()
    {
        float sampleTime = 2.0f + Random.Range(0.0f, 1.0f);

        isbehaveCoolTimeOn = false;
        if (behaveCount_FOR_PPT == 100)
        {
            enemyCollector.listScore_FOR_PPT.Add(scoreCount_FOR_PPT);
            Destroy(gameObject);
        }

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
                    __ENE_AI_Engine.Attack_Default(sampleTime, ref enemyController.enemy_Front, __ENE_Stat, 1);

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

        //20190607 임시
        if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[8])
        {
            behaveCount_FOR_PPT++;
            //Debug.Log(behaveCount_FOR_PPT);

            
            if (hitCounter >= 2)
                hitCounter = 1;
            AI_Score_Cal(hitCounter);
            //Debug.Log(scoreCount_FOR_PPT);
            hitCounter = 0;
            StartCoroutine(enemyController.enemyCoolTimer.Timer(sampleTime, (input) => { __ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[8] = input; }, true));

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

    //void OnTriggerExit(Collider other)
    //{
    //    if (other.transform.tag == "Boundary")
    //    {
    //        //StartCoroutine(enemyController.enemyCoolTimer.Timer_Do_Once(6.0f, (input) => { isEnd_OF_Stage = input; }, true));
    //    }
    //}

    private void EnemyUsingSkill(int index, Transform enemyAttacker, SkillBaseStat whichSkill)
    {
        //쿨타임이 지났을 때
        if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[index])
        {
            //마나가 충분할 때
            if (__ENE_Stat.__Is_Mana_Enough(whichSkill))
            {
                //필요한 마나 또는 궁극기 게이지 소모
                if (whichSkill.__GET_Skill_Code_M != SkillBaseCode._SKILL_CODE_Main.FIN) __ENE_Stat.__Get_HIT__About_Mana(whichSkill.__GET_Skill_Use_Amount, 1);
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
    //함수 폐기
    public void AI_DeepLearning__Random_Ver()
    {
        //임시로 추가한 것, P를 누르면 모든 데이터 수집 개체 삭제 및 데이터 저장
        if (Input.GetKeyDown(KeyCode.P))
        {
            Destroy(gameObject);
        }

        curDist = Vector3.Distance(transform.position, enemyController.playerTransform.position);

        int listIndex_Move = (int)(Random.Range(0.0f, (float)(behaveList_Move.Count - 0.1f)));
        int listIndex_Rotate = (int)(Random.Range(0.0f, (float)(behaveList_Rotate.Count - 0.1f)));
        int listIndex_Attack = (int)(Random.Range(0.0f, (float)(behaveList_Attack.Count - 0.1f)));

        if (beforeBehaveID == "N_U_L_L")
        {
            float time = 0.5f + Random.Range(0.0f, 1.0f);

            beforeDist = curDist;
            beforeAngleComp = enemyController._GET__ENE_AI_Engine.angleComp;
            beforeVec2 = new Vector2(transform.position.x, transform.position.z);

            realIndex.InitIntVector3(listIndex_Move, listIndex_Rotate, listIndex_Attack);

            StartCoroutine(enemyController.enemyCoolTimer.Timer(time, (input) => { isbehaveCoolTimeOn = input; }, true));
            beforeBehaveID = "NULL";
        }

        if (isbehaveCoolTimeOn)
        {
            float time = 0.5f + Random.Range(0.0f, 1.0f);
            bool isCloser = false;

            behaveID = System.DateTime.Now.ToString() + ": " + gameObject.GetInstanceID().ToString();
            //int t = System.DateTime.Now.Second;
            //Debug.Log("ID: "+behaveID);
            //Debug.Log("Before: "+beforeBehaveID);
            if (beforeDist > curDist) isCloser = true;

            beforeDistTOInt = ((int)(beforeDist / 10f));

            //알고리즘 상의 한계로 발생하는 오류 정정
            //공격을 하지도 않았는데 공격이 성공한 것처럼 기록되는 경우, 이전 행동에서 한 공격이 다음 행동 시작 지점에서 성공했을 때 발생하는 것으로 추정됨.
            //행동의 처음부터 끝까지 플레이어가 Enemy의 좌측에 있었으나 우측에서 발사한 공격이 명중했거나
            //행동의 처음부터 끝까지 플레이어가 Enemy의 우측에 있었으나 좌측에서 발사한 공격이 명중했다면 오류, 발생 원인을 자세하게는 모르겠으나 지속적으로 관찰됨. => 저장하려는 단계에서 처리하도록 변경
            //이런 유형의 지난 데이터들을 모두 찾아서 폐기해야됨
            if (realIndex.vecZ == 0)
            {
                hitCounter = 0;
                //Debug.Log("Purifying");
            }

            //데이터 모으기
            //공격에 성공한 데이터를 따로 모은다
            if (hitCounter != 0)
            {
                enemyCollector.listSitCUR0.Add(new SituationCUR(behaveID, beforeVec2.x, beforeVec2.y, beforeDist, beforeAngleComp, realIndex, time));
                enemyCollector.listSitAFT0.Add(new SituationAFT(behaveID, transform.position.x, transform.position.z, curDist, enemyController._GET__ENE_AI_Engine.angleComp, beforeBehaveID, beforeDistTOInt, hitCounter, isCloser));
            }

            //사거리 외 & 이내 데이터 모두 한 곳에 모은다
            enemyCollector.listSitCUR.Add(new SituationCUR(behaveID, beforeVec2.x, beforeVec2.y, beforeDist, beforeAngleComp, realIndex, time));
            enemyCollector.listSitAFT.Add(new SituationAFT(behaveID, transform.position.x, transform.position.z, curDist, enemyController._GET__ENE_AI_Engine.angleComp, beforeBehaveID, beforeDistTOInt, hitCounter, isCloser));

            realIndex.InitIntVector3(listIndex_Move, listIndex_Rotate, listIndex_Attack);

            //0.5~1.5초 마다 행동을 갱신한다 (값 변경될 수 있음)
            StartCoroutine(enemyController.enemyCoolTimer.Timer(time, (input) => { isbehaveCoolTimeOn = input; }, true));

            beforeBehaveID = behaveID;
            beforeDist = curDist;
            beforeAngleComp = enemyController._GET__ENE_AI_Engine.angleComp;
            beforeVec2 = new Vector2(transform.position.x, transform.position.z);

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

    //강화학습을 위한 형태로 개조됨
    //강화학습 데이터대로 행동했지만 결과가 좋지 못한 경우(행동 점수 <= 0) 임의 행동을 한 뒤 별도의 DB에 저장할 수 있도록 한다.
    //별도의 DB에 저장된 데이터를 일단 수동으로 학습하도록 한다.(프로그래머가 수동으로 변수나 코드를 바꿔줘야 하는 방식)
    public void AI_DeepLearning__BigData_Ver()
    {
        if (isbehaveCoolTimeOn)
        {
            curDist = Vector3.Distance(transform.position, enemyController.playerTransform.position);
            isHPLOW = true;//enemyController.__ENE_Stat.__PUB__Health_Point < enemyController.__ENE_Stat.half_HP;

            //sitCUR = enemyCollector.SearchGoodSitCUR(curDist, enemyController._GET__ENE_AI_Engine.angleComp, isHPLOW, true);

            //if (qDepth >= 1)
            //{
                //sitCUR = enemyCollector.SearchGoodSitCUR(curDist, enemyController._GET__ENE_AI_Engine.angleComp, isHPLOW, true);
            //    if (qDepth >= 3) qDepth = -1;
            //}
            //else
            sitCUR = enemyCollector.SearchGoodSitCUR(curDist, enemyController._GET__ENE_AI_Engine.angleComp, isHPLOW, false);

            //qDepth = 0;

            //20190606 임시
            if (sitCUR._doing.vecZ != 0)
            {
                behaveCount_FOR_PPT++;
            }

            bool isCloser = false;

            behaveID = System.DateTime.Now.ToString() + ": " + gameObject.GetInstanceID().ToString();
            if (beforeDist > curDist) isCloser = true;

            if (simpleSpining == 1)
            {
                sitCUR._doing.vecZ = 0;
            }
            else
            {
                //이전 상황과 행동에 대한 결과 저장
                IntVector3 forSave = new IntVector3(sitCUR._doing.vecX, sitCUR._doing.vecY, sitCUR._doing.vecZ);

                SituationCUR cur_FOR_PPT = new SituationCUR(behaveID, beforeVec2.x, beforeVec2.y, beforeDist, beforeAngleComp, forSave, sitCUR._time);
                SituationAFT aft_FOR_PPT = new SituationAFT(behaveID, transform.position.x, transform.position.z, curDist, enemyController._GET__ENE_AI_Engine.angleComp, beforeBehaveID, 0, hitCounter, isCloser);

                AI_Score_Cal(hitCounter);
                AI_Score_Cal(isHPLOW, cur_FOR_PPT, aft_FOR_PPT);
                aft_FOR_PPT._beforeDB = scoreCount_FOR_PPT;


                if (rewardCount <= 1)
                    sitCUR = enemyCollector.SearchGoodSitCUR(curDist, enemyController._GET__ENE_AI_Engine.angleComp, isHPLOW, true);

                if (forSave.vecZ != 0)
                {
                    enemyCollector.listSitCUR.Add(cur_FOR_PPT);
                    enemyCollector.listSitAFT.Add(aft_FOR_PPT);
                }

                if (Random.Range(0.0f, 1.0f) < sigma)
                {
                    sitCUR._doing.InitIntVector3((int)(Random.Range(0, 2)), (int)(Random.Range(0, 6)), (int)(Random.Range(0, 3)));
                    sitCUR._time = Random.Range(0.5f, 1.5f);
                }

                rewardCount = 0;
            }

            //DataBase에서 긁어온 정보의 time동안 행동한다.
            StartCoroutine(enemyController.enemyCoolTimer.Timer(sitCUR._time, (input) => { isbehaveCoolTimeOn = input; }, true));

            beforeBehaveID = behaveID;
            beforeDist = curDist;
            beforeAngleComp = enemyController._GET__ENE_AI_Engine.angleComp;
            beforeVec2 = new Vector2(transform.position.x, transform.position.z);

            hitCounter = 0;
            getDamagedCounter = 0;
        }

        //1차적으로 일반 공격만 생각하도록 한다.
        //공격 쿨타임도 끝났고 공격을 하려는 경우
        if (sitCUR._doing.vecZ != 0)
        {
            if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[1])// && sitCUR._doing.vecZ < behaveList_Attack.Count && curDist < 30f)
            {
                //주어진 공격 수행
                behaveList_Attack[sitCUR._doing.vecZ]();
            }
        }

        ////주어진 이동 수행
        //if (sitCUR._doing.vecX != 0)
        //    behaveList_Move[sitCUR._doing.vecX]();
        ////주어진 회전 수행
        //if(sitCUR._doing.vecY != 0)
        //    behaveList_Rotate[sitCUR._doing.vecY]();

        behaveList_Move[sitCUR._doing.vecX]();
        behaveList_Rotate[sitCUR._doing.vecY]();

        if (deleteIt)
        {
            enemyCollector.listScore_FOR_PPT.Add(scoreCount_FOR_PPT);
            Destroy(gameObject);
        }

        //임시로 추가한 것, P을 누르면 모든 데이터 수집 개체 삭제 및 데이터 저장
        if ((Input.GetKeyDown(KeyCode.P) || behaveCount_FOR_PPT < -1) && simpleSpining != 1)
            deleteIt = true;
    }

    //비교를 위해 추가된 예전 버전의 Bigdata함수
    public void __OLD__AI_DeepLearning__BigData_Ver()
    {
        ////임시로 추가한 것, P을 누르면 모든 데이터 수집 개체 삭제 및 데이터 저장
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    Destroy(gameObject);
        //}

        //20190606 임시
        //if (behaveCount_FOR_PPT == 100)
        //{
        //    enemyCollector.listScore_FOR_PPT.Add(scoreCount_FOR_PPT);
        //    Destroy(gameObject);
        //}

        float dummy = 0f;
        float curDist = Vector3.Distance(transform.position, enemyController.playerTransform.position);

        if (beforeBehaveID == "N_U_L_L")
        {
            beforeBehaveID = "NULL";

            beforeDist = curDist;
            beforeAngleComp = enemyController._GET__ENE_AI_Engine.angleComp;
            beforeVec2 = new Vector2(transform.position.x, transform.position.z);
        }

        if (isbehaveCoolTimeOn)
        {

            //20190606 임시
            behaveCount_FOR_PPT++;

            float time = sitCUR._time;

            behaveID = System.DateTime.Now.ToString() + ": " + gameObject.GetInstanceID().ToString();
            //if (beforeDist > curDist) isCloser = true;

            //20190606 임시
            SituationCUR cur_FOR_PPT = new SituationCUR(behaveID, beforeVec2.x, beforeVec2.y, beforeDist, beforeAngleComp, __OLD__forSave, time);
            SituationAFT aft_FOR_PPT = new SituationAFT(behaveID, transform.position.x, transform.position.z, curDist, enemyController._GET__ENE_AI_Engine.angleComp, beforeBehaveID, getDamagedCounter, hitCounter, false);

            enemyCollector.listSitCUR.Add(cur_FOR_PPT);
            enemyCollector.listSitAFT.Add(aft_FOR_PPT);

            sitCUR = enemyCollector.SearchGoodSitCUR(curDist, enemyController._GET__ENE_AI_Engine.angleComp, false, false);

            //DataBase에서 긁어온 정보의 time동안 행동한다.
            StartCoroutine(enemyController.enemyCoolTimer.Timer(sitCUR._time, (input) => { isbehaveCoolTimeOn = input; }, true, (input) => { dummy = input; }));

            //20190606 임시
            //AI_Score_Cal(false, cur_FOR_PPT, aft_FOR_PPT);
            //AI_Score_Cal(hitCounter);

            beforeBehaveID = behaveID;
            beforeDist = curDist;
            beforeAngleComp = enemyController._GET__ENE_AI_Engine.angleComp;
            beforeVec2 = new Vector2(transform.position.x, transform.position.z);

            hitCounter = 0;
            getDamagedCounter = 0;
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

    //제대로된 강화학습을 위한 랜덤행동 버전
    public void AI_ReinforceLearn_RandomBehave_Ver()
    {
        ////임시로 추가한 것, P를 누르면 모든 데이터 수집 개체 삭제 및 데이터 저장
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    Destroy(gameObject);
        //}

        float curDist = Vector3.Distance(transform.position, enemyController.playerTransform.position);
        //Timer 코루틴을 돌리기 위한 더미 변수
        float dummy = 0.0f;

        int listIndex_Move = (int)(Random.Range(0.0f, (float)(behaveList_Move.Count - 0.1f)));
        int listIndex_Rotate = (int)(Random.Range(0.0f, (float)(behaveList_Rotate.Count - 0.1f)));
        int listIndex_Attack = (int)(Random.Range(0.0f, (float)(behaveList_Attack.Count - 0.1f)));

        if (beforeBehaveID == "N_U_L_L")
        {
            float time = 0.5f + Random.Range(0.0f, 1.0f);

            beforeDist = curDist;
            beforeAngleComp = enemyController._GET__ENE_AI_Engine.angleComp;
            beforeVec2 = new Vector2(transform.position.x, transform.position.z);

            realIndex.InitIntVector3(listIndex_Move, listIndex_Rotate, listIndex_Attack);

            StartCoroutine(enemyController.enemyCoolTimer.Timer(time, (input) => { isbehaveCoolTimeOn = input; }, true, (input) => { dummy = input; }));
            beforeBehaveID = "NULL";
        }

        if (isbehaveCoolTimeOn)
        {
            float time = 0.5f + Random.Range(0.0f, 1.0f);
            //bool isCloser = false;

            //behaveID = System.DateTime.Now.ToString() + ": " + gameObject.GetInstanceID().ToString();
            //if (beforeDist > curDist) isCloser = true;

            //if (realIndex.vecZ == 0)
            //{
            //    hitCounter = 0;
            //}


            //int behaveScore = (hitCounter * 2) - getDamagedCounter;

            //if (curDist > 50f && enemyController.__ENE_Stat.__PUB__Health_Point > 5)
            //    behaveScore -= (int)((curDist - 50) / 10);
            //else if (curDist > 50f && enemyController.__ENE_Stat.__PUB__Health_Point <= 5)
            //    behaveScore += (int)((curDist - 50) / 10);

            //IntVector3 forSave = new IntVector3(realIndex.vecX, realIndex.vecY, realIndex.vecZ);
            //Debug.Log(behaveScore);

            //모든 데이터 통합 관리
            //if (enemyController.__ENE_Stat.__PUB__Health_Point <= 5)
            //{
            //    enemyCollector.listSitCUR0.Add(new SituationCUR(behaveID, beforeVec2.x, beforeVec2.y, beforeDist, beforeAngleComp, forSave, time));
            //    enemyCollector.listSitAFT0.Add(new SituationAFT(behaveID, transform.position.x, transform.position.z, curDist, enemyController._GET__ENE_AI_Engine.angleComp, beforeBehaveID, behaveScore, hitCounter, isCloser));
            //}
            //else
            //{
            //    enemyCollector.listSitCUR.Add(new SituationCUR(behaveID, beforeVec2.x, beforeVec2.y, beforeDist, beforeAngleComp, forSave, time));
            //    enemyCollector.listSitAFT.Add(new SituationAFT(behaveID, transform.position.x, transform.position.z, curDist, enemyController._GET__ENE_AI_Engine.angleComp, beforeBehaveID, behaveScore, hitCounter, isCloser));
            //}

            realIndex.InitIntVector3(listIndex_Move, listIndex_Rotate, listIndex_Attack);

            //beforeBehaveID = behaveID;
            //beforeDist = curDist;
            //beforeAngleComp = enemyController._GET__ENE_AI_Engine.angleComp;
            //beforeVec2 = new Vector2(transform.position.x, transform.position.z);

            //0.5~1.5초 마다 행동을 갱신한다 (값 변경될 수 있음)
            StartCoroutine(enemyController.enemyCoolTimer.Timer(time, (input) => { isbehaveCoolTimeOn = input; }, true, (input) => { dummy = input; }));

            //hitCounter = 0;
            //getDamagedCounter = 0;
        }

        ////임시
        //if(simpleSpining == 0)
        //    realIndex = new IntVector3(1, 1,0);

        //1차적으로 일반 공격만 생각하도록 한다.
        //공격 쿨타임도 끝났고 공격을 하려는 경우
        //if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[1] && realIndex.vecZ != 0)
        //{
        //    //주어진 공격 수행
        //    behaveList_Attack[realIndex.vecZ]();
        //}

        //주어진 이동 수행
        behaveList_Move[realIndex.vecX]();
        //주어진 회전 수행
        behaveList_Rotate[realIndex.vecY]();
    }

    //랜덤 행동 중 아무것도 안 하는 함수
    private void AI_DO_Nothing() { }

    //행동 점수의 총합을 구하기 위한 함수
    //후에 행동점수를 결정하는 단일함수로 사용할 예정
    private void AI_Score_Cal(bool isHPLOW, SituationCUR cur, SituationAFT aft)
    {
        int resultScore = aft._hitCounter;

        //체력 절반 이하, 절반 초과에 따라 다른 행동점수 가산점 OR 불이익 정책 사용
        if (!isHPLOW)
        {
            if (cur._doing.vecX != 0 && cur._doing.vecY != 0)
                resultScore += (aft._hitCounter * 7);

            if (cur._dist >= 30f && cur._doing.vecX != 0 && cur._doing.vecY != 0
                && aft._closer == true && aft._dist + 10f < cur._dist)
            {
                resultScore += 10;
            }

            if (cur._dist >= 30f && cur._doing.vecX == 0 && cur._doing.vecY == 0
                && aft._closer == false && aft._dist >= cur._dist)
            {
                resultScore -= (int)(20 + aft._dist);
            }

            if (cur._dist < 30f && aft._hitCounter > 0)
            {
                resultScore += 40;
            }

            if (aft._hitCounter == 0 && cur._doing.vecZ != 0)
            {
                aft._beforeDB -= 10;
            }
        }
        else
        {
            if (cur._doing.vecX != 0 && cur._doing.vecY != 0)
                resultScore--;

            if (aft._dist > cur._dist)
                resultScore++;

            if (cur._dist >= 30f && cur._doing.vecX == 0 && cur._doing.vecY == 0
                && aft._closer == true && aft._dist <= cur._dist)
            {
                resultScore -= 3;
            }

            if (cur._dist >= 30f && cur._doing.vecX != 0 && cur._doing.vecY != 0
                && aft._closer == false && aft._dist > cur._dist)
            {
                resultScore += 3;
            }

            if (cur._dist <= 30f && cur._doing.vecX == 0 && cur._doing.vecY == 0
                && aft._closer == true && aft._dist <= cur._dist)
            {
                resultScore -= 3;
            }

            if (cur._dist <= 30f && cur._doing.vecX != 0 && cur._doing.vecY != 0
                && aft._closer == true && aft._dist > cur._dist)
            {
                resultScore += 3;
            }
        }

        rewardCount += resultScore;
    }

    private void AI_Score_Cal(int hitCounter)
    {
        if (hitCounter >= 2)
            hitCounter = 1;

        scoreCount_FOR_PPT += hitCounter;
    }
}
