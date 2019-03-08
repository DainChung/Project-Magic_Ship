using UnityEngine;

using System.Collections;

using System.Collections.Generic;

//EnemyAI에 직접적으로 관련된 클래스를 새로 생성함
public class EnemyAI : MonoBehaviour {

    private EnemyController enemyController;

    private EnemyStat __ENE_Stat;
    private EnemyAIEngine __ENE_AI_Engine;

    //나중엔 파일에서 읽어오도록 변경될 수도 있음.
    //외부의 딥러닝 프로그램이 함수를 호출할 때 사용하기 위한 함수 List
    private List<System.Action> behaveList = new List<System.Action>();
    public List<System.Action> _GET_behaveList
    {
        get { return behaveList; }
    }

    //AI_DeapLearning__Random_Ver에서 사용할 변수들
    private bool isbehaveCoolTimeOn = true;
    private int realIndex = 3;

    // Use this for initialization
    void Awake () {
        enemyController = transform.GetComponent<EnemyController>();
        __ENE_Stat = enemyController.__ENE_Stat;
        __ENE_AI_Engine = enemyController._GET__ENE_AI_Engine;

        //기본 공격=====
        //정면
        behaveList.Add(() => __ENE_AI_Engine.Attack_Default(2.0f + Random.Range(0.0f, 1.0f), ref enemyController.enemy_Front, __ENE_Stat, 1));
        //우측
        behaveList.Add(() => __ENE_AI_Engine.Attack_Default(2.0f + Random.Range(0.0f, 1.0f), ref enemyController.enemy_Right, __ENE_Stat, 1));
        //좌측
        behaveList.Add(() => __ENE_AI_Engine.Attack_Default(2.0f + Random.Range(0.0f, 1.0f), ref enemyController.enemy_Left, __ENE_Stat, 1));
        //============
        //플레이어와 일정 거리 이하가 될 때까지 앞으로 이동
        behaveList.Add(() => __ENE_AI_Engine.Go_TO_Foward_UNTIL_RayHit(__ENE_Stat.__PUB_Move_Speed, ref enemyController.enemyTransform, enemyController.playerTransform));
        //제자리에서 플레이어의 반대 방향을 바라보는 것
        behaveList.Add(() => __ENE_AI_Engine.Rotate_TO_Direction(enemyController.__ENE_Stat.__PUB_Rotation_Speed, ref enemyController.enemyTransform, enemyController.playerTransform, true));
        //제자리에서 플레이어를 바라보는 것
        behaveList.Add(() => __ENE_AI_Engine.Rotate_TO_Direction(enemyController.__ENE_Stat.__PUB_Rotation_Speed, ref enemyController.enemyTransform, enemyController.playerTransform, false));
        //플레이어를 바라보면서 앞으로 이동(isRunning == false) 또는 플레이어로부터 도망(isRunning == false)
        behaveList.Add(() => this.AI_DeapLearning__Move_AND_Rotate_TO_SomeWhere(false));
        behaveList.Add(() => this.AI_DeapLearning__Move_AND_Rotate_TO_SomeWhere(true));
        //플레이어를 바라보면서 앞으로 이동(isRunning == false) 또는 플레이어로부터 도망(isRunning == false) 하면서 기본 공격
        behaveList.Add(() => this.AI_DeapLearning__Move_AND_Rotate_TO_SomeWhere_WITH_Default_Attack(false, 0));
        behaveList.Add(() => this.AI_DeapLearning__Move_AND_Rotate_TO_SomeWhere_WITH_Default_Attack(false, 1));
        behaveList.Add(() => this.AI_DeapLearning__Move_AND_Rotate_TO_SomeWhere_WITH_Default_Attack(false, 2));

        behaveList.Add(() => this.AI_DeapLearning__Move_AND_Rotate_TO_SomeWhere_WITH_Default_Attack(true, 0));
        behaveList.Add(() => this.AI_DeapLearning__Move_AND_Rotate_TO_SomeWhere_WITH_Default_Attack(true, 1));
        behaveList.Add(() => this.AI_DeapLearning__Move_AND_Rotate_TO_SomeWhere_WITH_Default_Attack(true, 2));
        //조건과 관계없이 앞으로 이동하면서 기본 공격
        behaveList.Add(() => AI_DeapLearning__Move_TO_Forward_OR_Back_WITH_Default_Attack(1, 0));
        behaveList.Add(() => AI_DeapLearning__Move_TO_Forward_OR_Back_WITH_Default_Attack(1, 1));
        behaveList.Add(() => AI_DeapLearning__Move_TO_Forward_OR_Back_WITH_Default_Attack(1, 2));
        //조건과 관계없이 뒤로 이동하면서 기본 공격
        behaveList.Add(() => AI_DeapLearning__Move_TO_Forward_OR_Back_WITH_Default_Attack(-1, 0));
        behaveList.Add(() => AI_DeapLearning__Move_TO_Forward_OR_Back_WITH_Default_Attack(-1, 1));
        behaveList.Add(() => AI_DeapLearning__Move_TO_Forward_OR_Back_WITH_Default_Attack(-1, 2));
        //제자리에서 플레이어를 바라보면서 기본 공격
        behaveList.Add(() => AI_DeapLearning__Rotate_TO_Player_WITH_Default_Attack(0));
        behaveList.Add(() => AI_DeapLearning__Rotate_TO_Player_WITH_Default_Attack(1));
        behaveList.Add(() => AI_DeapLearning__Rotate_TO_Player_WITH_Default_Attack(2));
    }

    //가장 단순한 수준의 AI
    public void AI_Simple_Level0()
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

            //일단 공격을 시켜보자
            //예상대로 기본 공격은 플레이어가 요령껏 피하기 쉽다
            if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[1])
            {
                //쿨타임에 랜덤변수를 더해서 난이도를 조금 올린다.
                __ENE_AI_Engine.Attack_Default(2.0f + Random.Range(0.0f, 1.0f), ref enemyController.enemy_Front, __ENE_Stat, 1);
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

    //데이터 수집 목적인 랜덤한 행동을 지시하는 함수
    //아직 데이터를 수집하는 스크립트는 없음
    //어느 스크립트에서 데이터를 DB에 전송할 것인지는 천천히 생각해볼것
    public void AI_DeapLearning__Random_Ver()
    {
        int listIndex = (int)(Random.Range(0.0f, (float)(_GET_behaveList.Count)));
        //Timer 코루틴을 돌리기 위한 더미 변수
        float dummy = 0.0f;


        if (isbehaveCoolTimeOn)
        {
            realIndex = listIndex;
            //0.5~1.5초 마다 행동을 갱신한다 (값 변경될 수 있음)
            StartCoroutine(enemyController.enemyCoolTimer.Timer(0.5f + Random.Range(0.0f, 1.0f), (input) => { isbehaveCoolTimeOn = input; }, true, (input) => { dummy = input; }));
        }

        //Debug.Log(realIndex);

        //일반 공격은 쿨타임이 지났을 때만 사용할 수 있도록 한다.
        if (realIndex >= 0 && realIndex <= 2)
        {
            if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[1])
                behaveList[realIndex]();
        }
        else
        {
            behaveList[realIndex]();
        }
    }

    //플레이어를 바라보면서 앞으로 이동(isRunning == false) 또는 플레이어로부터 도망(isRunning == false)
    private void AI_DeapLearning__Move_AND_Rotate_TO_SomeWhere(bool isRunning)
    {
        __ENE_AI_Engine.Rotate_TO_Direction(enemyController.__ENE_Stat.__PUB_Rotation_Speed, ref enemyController.enemyTransform, enemyController.playerTransform, isRunning);
        __ENE_AI_Engine.Go_TO_Foward_UNTIL_RayHit(__ENE_Stat.__PUB_Move_Speed, ref enemyController.enemyTransform, enemyController.playerTransform);
    }

    //플레이어를 바라보면서 앞으로 이동(isRunning == false) 또는 플레이어로부터 도망(isRunning == false) 하면서 기본공격
    private void AI_DeapLearning__Move_AND_Rotate_TO_SomeWhere_WITH_Default_Attack(bool isRunning, int attackSpot)
    {
        //공격 가능한 상태일때만 공격
        if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[1])
            behaveList[attackSpot]();
        AI_DeapLearning__Move_AND_Rotate_TO_SomeWhere(isRunning);
    }

    //조건과 관계없이 앞 (dir == 1)이나 뒤 (dir == -1)로 이동하면서 기본 공격
    private void AI_DeapLearning__Move_TO_Forward_OR_Back_WITH_Default_Attack(int dir, int attackSpot)
    {
        //공격 가능한 상태일때만 공격
        if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[1])
            behaveList[attackSpot]();
        __ENE_AI_Engine.__ENE_Engine._unit_Move_Engine.Move_OBJ(__ENE_Stat.__PUB_Move_Speed, ref enemyController.enemyTransform, dir);
    }

    //제자리에서 플레이어를 바라보면서 기본 공격
    private void AI_DeapLearning__Rotate_TO_Player_WITH_Default_Attack(int attackSpot)
    {
        if (__ENE_AI_Engine._PUB_enemy_Is_ON_CoolTime[1])
            behaveList[attackSpot]();
        __ENE_AI_Engine.Rotate_TO_Direction(enemyController.__ENE_Stat.__PUB_Rotation_Speed, ref enemyController.enemyTransform, enemyController.playerTransform, true);
    }
}
