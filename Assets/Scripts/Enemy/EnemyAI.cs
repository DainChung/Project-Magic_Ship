using UnityEngine;
using System.Collections;

//EnemyAI에 직접적으로 관련된 클래스를 새로 생성함
public class EnemyAI : MonoBehaviour {

    private EnemyController enemyController;

    private EnemyStat __ENE_Stat;
    private EnemyAIEngine __ENE_AI_Engine;

	// Use this for initialization
	void Awake () {
        enemyController = transform.GetComponent<EnemyController>();
        __ENE_Stat = enemyController.__ENE_Stat;
        __ENE_AI_Engine = enemyController._GET__ENE_AI_Engine;

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
}
