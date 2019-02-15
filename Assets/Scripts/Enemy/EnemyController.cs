using UnityEngine;
using System.Collections;

public class EnemyStat : Unit__Base_Stat {

    public int half_HP;

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

        //소수점 이하는 내림해서 값이 결정됨.
        half_HP = (int)(__MAX_Health_Point / 2);
    }
}

public class EnemyEngine : Unit__Base_Engine {
    public Unit__Base_Movement_Engine __ENE_M_Engine = new Unit__Base_Movement_Engine();
    public Unit__Base_Combat_Engine __ENE_C_Engine = new Unit__Base_Combat_Engine();

    //destiTrn을 바라보는 방향 또는 그 반대 방향
    private Quaternion destiQT;
    private bool[] enemy_is_ON_CoolTime = new bool[4];

    public bool[] _PUB_enemy_Is_ON_CoolTime
    {
        get { return enemy_is_ON_CoolTime; }
        set { enemy_is_ON_CoolTime = value; }
    }

    //Timer가 받는 인자가 늘어났기에 추가하는 dummyFloat
    public float[] dummyFloatTime = new float[3];

    public UnitCoolTimer enemyCoolTimer;

    //destinationDir 방향으로 회전하는 함수
    public void Rotate_TO_Direction(float rotate_Speed, ref Transform rotated_OBJ, Transform destiTrn, bool is_RunAway)
    {
        //두 지점 사이의 각도 구하기 (도달해야 되는 각도 => 목표 각도)
        //일단 플레이어를 향하는 각도를 구한다.
        float destiAngle = Mathf.Atan2(destiTrn.position.x - rotated_OBJ.position.x, destiTrn.position.z - rotated_OBJ.position.z) * Mathf.Rad2Deg;

        //어떤 방향으로 돌아야 목표 각도에 빠르게 도달할 수 있는지를 계산하기 위한 변수들 => dir값을 1 또는 -1로 결정
        //현재 Enemy가 바라보고 있는 방향의 각도를 구한다.
        float curAngle = rotated_OBJ.rotation.eulerAngles.y;

        float destiAngle_FOR_dir = 0.0f;

        //시계방향 회전을 기본값으로 한다.
        int dir = 1;

        //curAngle은 0 <= curAgnle < 360 (destiAngle은 -180 < destiAngle <= 180)이기 떄문에 curAngle > 180인 경우 curAngle 값을 보정하도록 한다.
        if (curAngle > 180)
        {
            //curAngle도 -180 < curAngle <= 180으로 변경한다.
            curAngle -= 360;
        }

        if (is_RunAway)
        {
            destiAngle = Get_Opposite_Direction_Angle(destiAngle);
        }
        //방향 계산을 위해 값을 그대로 가져온다.
        destiAngle_FOR_dir = destiAngle;

        //아래 Debug.Log를 통해 확인할 수 있다.
        //Debug.Log(destiAngle);

        //destiAngle_FOR_dir와 curAngle의 부호가 다른 경우
        if ((destiAngle_FOR_dir < 0 && curAngle >= 0) || (destiAngle_FOR_dir >= 0 && curAngle < 0))
        {
            //destiAngle_FOR_dir의 반대방향으로 계산하도록 조정한다.
            destiAngle_FOR_dir = Get_Opposite_Direction_Angle(destiAngle_FOR_dir);

            //dir값을 보정한다.
            dir *= (-1);
        }

        //dir값을 결정한다.
        //각도 차를 구하여 시계방향으로 돌지 반시계방향으로 돌지 결정한다. (1이 시계 방향)
        if (curAngle - destiAngle_FOR_dir >= 0)
        {
            dir *= (-1);
        }

        //목표 각도를 Quaternion으로 바꿔준다.
        destiQT = Quaternion.Euler(0, destiAngle, 0);

        //rotated_OBJ.rotation = destiQT;

        //호출될 때마다 목표 각도와 현재 각도 차 구하기
        float angleComparison = Quaternion.Angle(rotated_OBJ.rotation, destiQT);

        //약간의 오차를 허용한다.
        //목표지점을 바라볼 때까지 회전한다.
        if (!((angleComparison < 1.0f) && (angleComparison > - 1.0f)))
        {
            __ENE_M_Engine.Rotate_OBJ(rotate_Speed, ref rotated_OBJ, dir);
        }

        //Debug.Log(angleComparison);
    }

    //반대 방향을 찾아주는 함수.
    //-180 < angle <= 180의 범위일 때만 유효하다.
    private float Get_Opposite_Direction_Angle(float angle)
    {
        if (angle >= 0)
            angle -= 180;
        else
            angle += 180;

        return angle;
    }

    //충분히 가까울 때까지 앞으로 이동하는 함수
    public void Go_TO_Foward_UNTIL_RayHit(float speed, ref Transform mover, Transform target)
    {
        if (Vector3.Distance(target.position, mover.position) >= 17.0f)
        {
            //전방 이동
            __ENE_M_Engine.Move_OBJ(speed, ref mover, 1);
        }
        //느리게 전방 이동 (보다 세밀한 움직임을 위해서)
        else if (Vector3.Distance(target.position, mover.position) < 17.0f && Vector3.Distance(target.position, mover.position) >= 16.0f)
        {
            __ENE_M_Engine.Move_OBJ(speed / 3, ref mover, 1);
        }
        else if (Vector3.Distance(target.position, mover.position) < 16.0f && Vector3.Distance(target.position, mover.position) >= 15.0f)
        {
            __ENE_M_Engine.Move_OBJ(speed / 8, ref mover, 1);
        }
    }

    public void Attack_Default(float coolTime, ref GameObject threw_Ammo, ref Transform attacker, int damage, int boolIndex)
    {
        //공격
        __ENE_C_Engine.Default_ATK(ref threw_Ammo, ref attacker, damage);

        //딜레이
        enemyCoolTimer.StartCoroutine(enemyCoolTimer.Timer(coolTime, (input) => { enemy_is_ON_CoolTime[boolIndex] = input; }, true, (input) => { dummyFloatTime[0] = input; }));
    }
}

public class EnemyController : MonoBehaviour {

    public EnemyStat __ENE_Stat = new EnemyStat();
    private EnemyEngine __ENE_Engine = new EnemyEngine();

    public UnitCoolTimer enemyCoolTimer;

    public Transform enemyTransform;
    public Transform playerTransform;

    public Transform enemy_Front;
    public Transform enemy_Right;
    public Transform enemy_Left;

    public GameObject default_Ammo;

    void Awake() {
        //이속, 회전속도, 체력, 마나, 파워 게이지, 공격력, 크리확률, 크리계수
        __ENE_Stat.SampleInit(10.0f, 30.0f, 10, 10, 10, 1, 10.0f, 1.5f);
        __ENE_Engine.enemyCoolTimer = enemyCoolTimer;

        //쿨타임을 위한 부울 변수들 초기화
        for (int index = 0; index < __ENE_Engine._PUB_enemy_Is_ON_CoolTime.Length; index++)
        {
            __ENE_Engine._PUB_enemy_Is_ON_CoolTime[index] = true;
        }
    }

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {

        //나중에 EnemyAI 클래스를 따로 만들어서 이하 내용과 같은 기능을 하도록 넣을 것.
        //체력이 최대 체력의 절반 이하일 때 (도망가야할 때)
        if (__ENE_Stat.__PUB__Health_Point <= __ENE_Stat.half_HP)
        {
            //플레이어 반대 방향을 보도록 한다.
            __ENE_Engine.Rotate_TO_Direction(__ENE_Stat.__PUB_Rotation_Speed, ref enemyTransform, playerTransform, true);

            //일정 시간동안 해당 유닛의 전방을 향해 이동한다.
            if (__ENE_Engine._PUB_enemy_Is_ON_CoolTime[0])
            {
                __ENE_Engine.__ENE_M_Engine.Move_OBJ(__ENE_Stat.__PUB_Move_Speed, ref enemyTransform, 1);
                //4초 동안 퇴각
                StartCoroutine(enemyCoolTimer.Timer_Do_Once(4.0f, (input) => { __ENE_Engine._PUB_enemy_Is_ON_CoolTime[0] = input; }, true));
            }
            else
            {
                //16초 동안 정지
                StartCoroutine(enemyCoolTimer.Timer_Do_Once(16.0f, (input) => { __ENE_Engine._PUB_enemy_Is_ON_CoolTime[0] = input; }, false));
                //회복 패턴은 정예 선박만 넣는것이 좋을 것 같다
                //StartCoroutine(__ENE_Stat.__Get_HIT__About_Health_FREQ(2.0f, 1.0f, 1, -1));
            }
        }
        //체력이 최대 체력의 절반을 초과할 때 (공격해야할 때)
        else
        {
            //플레이어를 바라보도록 한다.
            __ENE_Engine.Rotate_TO_Direction(__ENE_Stat.__PUB_Rotation_Speed, ref enemyTransform, playerTransform, false);

            //전방으로 이동한다.
            __ENE_Engine.Go_TO_Foward_UNTIL_RayHit(__ENE_Stat.__PUB_Move_Speed, ref enemyTransform, playerTransform);

            //일단 공격을 시켜보자
            //예상대로 기본 공격은 플레이어가 요령껏 피하기 쉽다
            if (__ENE_Engine._PUB_enemy_Is_ON_CoolTime[1])
            {
                //쿨타임에 랜덤변수를 더해서 난이도를 조금 올린다.
                __ENE_Engine.Attack_Default(2.0f + Random.Range(0.0f, 1.0f), ref default_Ammo, ref enemy_Front, __ENE_Stat.__PUB_ATK__Val, 1);
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

    public void _Enemy_Get_Hit(int damage)
    {
        //isHit_OR_Heal 부분은 나중에 Enum과 같은 요소로 변경하여 넣을 것
        __ENE_Stat.__GET_HIT__About_Health(damage, 1);
    }
}
