using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.Linq;

using PMS_Math;

using File_IO;

public class EnemyStat : Unit__Base_Stat {

    public int half_HP;

    private int ai_Level;
    public int _GET_ai_Level
    {
        get { return ai_Level;  }
    }

    private string enemyName = "";
    public string _GET_enemyName
    {
        get { return enemyName; }
    }

    //파일에서 정상적으로 skillID들을 읽어오는지 알아보기 위한 변수
    //나중에 List<SkillBaseStat>으로 바꾸고 Skill내용들을 읽어와서 저장하도록 할 것
    private List<SkillBaseStat> enemySkillList = new List<SkillBaseStat>();
    public List<SkillBaseStat> _GET_enemySkillList
    {
        get {return enemySkillList; }
    }

    public void SampleInit(float mSp, float rSp, int hp, int mp, int pp, int atk, float criR, float criP, int ai_Lv)
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

        base.FOriginalMoveSpeed = base._Move_Speed;
        base.FOriginalRotateSpeed = base._Rotation_Speed;

        //소수점 이하는 내림해서 값이 결정됨.
        half_HP = (int)(__MAX_Health_Point / 2);

        ai_Level = ai_Lv;

        //버프, 디버프 통제용
        for (int i = 0; i < 5; i++)
        {
            __PUB_Stat_IsCoolTimeOn.Add(false);
            __PUB_Stat_Real_Locker.Add(false);
        }

        //기본 마나 회복
        __PUB_Stat_IsCoolTimeOn[2] = true;
    }

    public void InitialLize_Enemy_Stat(List<string> enemyStatBaseString)
    {
        enemyName = enemyStatBaseString[1];

        base.__PUB_Move_Speed = float.Parse(enemyStatBaseString[2]);
        base.__PUB_Rotation_Speed = float.Parse(enemyStatBaseString[3]);

        base.__MAX_Health_Point = int.Parse(enemyStatBaseString[4]);
        base.__MAX_Mana_Point = int.Parse(enemyStatBaseString[5]);
        base.__MAX_Power_Point = int.Parse(enemyStatBaseString[6]);

        base.__PUB__Health_Point = base.__MAX_Health_Point;
        base.__PUB__Mana_Point = base.__MAX_Mana_Point;
        base.__PUB__Power_Point = 0;

        base.__PUB_ATK__Val = int.Parse(enemyStatBaseString[7]);
        base.__PUB_Critical_Rate = float.Parse(enemyStatBaseString[8]);
        base.__PUB_Critical_P = float.Parse(enemyStatBaseString[9]);

        base.FOriginalMoveSpeed = base._Move_Speed;
        base.FOriginalRotateSpeed = base._Rotation_Speed;

        //소수점 이하는 내림해서 값이 결정됨.
        half_HP = (int)(__MAX_Health_Point / 2);

        ai_Level = int.Parse(enemyStatBaseString[10]);

        //ai_Level = 1;

        //Enemy가 장착한 스킬들
        enemySkillList.Add(IO_CSV.__Get_Searched_SkillBaseStat(enemyStatBaseString[11]));
        enemySkillList.Add(IO_CSV.__Get_Searched_SkillBaseStat(enemyStatBaseString[12]));
        enemySkillList.Add(IO_CSV.__Get_Searched_SkillBaseStat(enemyStatBaseString[13]));

        //버프, 디버프 통제용
        for (int i = 0; i < 5; i++)
        {
            __PUB_Stat_IsCoolTimeOn.Add(false);
            __PUB_Stat_Real_Locker.Add(false);
        }

        //기본 마나 회복
        __PUB_Stat_IsCoolTimeOn[2] = true;
    }
}

//구 EnemyAI
//AI 수준에 따라 다른 행동을 쉽게 할 수 있도록 EnemyAI.cs를 별도로 만들고
//아래 클래스는 EnemyAIEngine으로 명칭 변경
//특정 방향을 바라보기, 목표로부터 일정 거리 이하가 될 때까지 앞으로 이동, 기본 공격 처럼 매우 기본적인 행동에 대한 함수들이 있다.
public class EnemyAIEngine {

    public float angleComp;

    public UnitBaseEngine _unitBaseEngine;

    //destiTrn을 바라보는 방향 또는 그 반대 방향
    private Quaternion destiQT;
    //쿨타임 여부에 따른 행동 관리
    private bool[] enemy_is_ON_CoolTime = new bool[9];

    public bool[] _PUB_enemy_Is_ON_CoolTime
    {
        get { return enemy_is_ON_CoolTime; }
        set { enemy_is_ON_CoolTime = value; }
    }

    public UnitCoolTimer enemyCoolTimer;

    //시계방향 또는 반시계방향으로 돌아야된다는 걸 알려주는 함수
    //true면 시계방향, false면 반시계방향
    public bool GET_RotataionDir(float curAngle, float destiAngle)
    {
        return (curAngle - destiAngle < 0);
    }

    //destinationDir 방향으로 회전하는 함수
    public void Rotate_TO_Direction(float rotate_Speed, ref Transform rotated_OBJ, Transform destiTrn, bool is_RunAway)
    {
        //시계방향 회전을 기본값으로 한다.
        int dir = GetAngleComp(rotated_OBJ, destiTrn);

        //약간의 오차를 허용한다.
        //목표지점을 바라볼 때까지 회전한다.
        if ( !( (angleComp < 1.0f) && (angleComp > - 1.0f) ) )
        {
            _unitBaseEngine._unit_Move_Engine.Rotate_OBJ(rotate_Speed, ref rotated_OBJ, dir);
        }

        //Debug.Log(angleComparison);
    }

    //측면으로 목표를 바라보도록 하는 함수
    public void Rotate_TO_Direction(float rotate_Speed, ref Transform rotated_OBJ, Transform destiTrn, bool is_Left_OR_Right, Transform right_Side)
    {
        //시계방향 회전을 기본값으로 한다.
        int dir = GetAngleComp(right_Side, destiTrn);

        //약간의 오차를 허용한다.
        //목표지점을 바라볼 때까지 회전한다.
        if (!((angleComp < 1.0f) && (angleComp > -1.0f)))
        {
            _unitBaseEngine._unit_Move_Engine.Rotate_OBJ(rotate_Speed, ref rotated_OBJ, dir);
        }

        //Debug.Log(angleComparison);
    }

    public int GetAngleComp(Transform enemy, Transform destiTrn)
    {
        //두 지점 사이의 각도 구하기 (도달해야 되는 각도 => 목표 각도)
        //일단 플레이어를 향하는 각도를 구한다.
        float _destiAngle = Mathf.Atan2(destiTrn.position.x - enemy.position.x, destiTrn.position.z - enemy.position.z) * Mathf.Rad2Deg;

        //어떤 방향으로 돌아야 목표 각도에 빠르게 도달할 수 있는지를 계산하기 위한 변수들 => dir값을 1 또는 -1로 결정
        //현재 Enemy가 바라보고 있는 방향의 각도를 구한다.
        //curAngle은 0 <= curAgnle < 360 (destiAngle은 -180 < destiAngle <= 180)이기 떄문에 curAngle > 180인 경우 curAngle 값을 보정하도록 한다.
        //curAngle도 -180 < curAngle <= 180으로 변경한다.
        float curAngle = Rotation_Math.Angle360_TO_Angle180(enemy.rotation.eulerAngles.y);

        //float destiAngle_FOR_dir = 0.0f;

        float dirAngle;

        int dir = 1;

        //방향 계산을 위해 값을 그대로 가져온다.
        dirAngle = _destiAngle;

        //destiAngle_FOR_dir와 curAngle의 부호가 다른 경우
        if ((dirAngle < 0 && curAngle >= 0) || (dirAngle >= 0 && curAngle < 0))
        {
            //destiAngle_FOR_dir의 반대방향으로 계산하도록 조정한다.
            dirAngle = Rotation_Math.Get_Opposite_Direction_Angle(dirAngle);

            //dir값을 보정한다.
            dir *= (-1);
        }

        //dir값을 결정한다.
        //각도 차를 구하여 시계방향으로 돌지 반시계방향으로 돌지 결정한다. (1이 시계 방향)
        if (!(GET_RotataionDir(curAngle, dirAngle)))
        {
            dir *= (-1);
        }
        //목표 각도를 Quaternion으로 바꿔준다.
        destiQT = Quaternion.Euler(0, _destiAngle, 0);

        //호출될 때마다 목표 각도와 현재 각도 차 구하기
        angleComp = Quaternion.Angle(enemy.rotation, destiQT) * dir;

        return dir;
    }

    //충분히 가까울 때까지 앞으로 이동하는 함수
    public void Go_TO_Foward_UNTIL_RayHit(float speed, ref Transform mover, Transform target)
    {
        if (Vector3.Distance(target.position, mover.position) >= 17.0f)
        {
            //전방 이동
            _unitBaseEngine._unit_Move_Engine.Move_OBJ(speed, ref mover, 1);
        }
        //느리게 전방 이동 (보다 세밀한 움직임을 위해서)
        else if (Vector3.Distance(target.position, mover.position) < 17.0f && Vector3.Distance(target.position, mover.position) >= 16.0f)
        {
            _unitBaseEngine._unit_Move_Engine.Move_OBJ(speed / 3, ref mover, 1);
        }
        else if (Vector3.Distance(target.position, mover.position) < 16.0f && Vector3.Distance(target.position, mover.position) >= 15.0f)
        {
            _unitBaseEngine._unit_Move_Engine.Move_OBJ(speed / 8, ref mover, 1);
        }
    }

    public void Attack_Default(float coolTime, ref Transform attacker, Unit__Base_Stat unitStat, int boolIndex)
    {
        //공격
        _unitBaseEngine._unit_Combat_Engine.Default_ATK(ref attacker, (SkillBaseStat)null);

        //딜레이
        enemyCoolTimer.StartCoroutine(enemyCoolTimer.Timer(coolTime, (input) => { enemy_is_ON_CoolTime[boolIndex] = input; }, true));
    }
}

public class EnemyController : MonoBehaviour {

    public EnemyStat enemyStat = new EnemyStat();

    private EnemyAIEngine enemyAIEngine = new EnemyAIEngine();
    public EnemyAIEngine GET_enemyAIEngine
    {
        get { return enemyAIEngine; }
    }

    private EnemyAI enemyAI;
    public EnemyAI PUB_enemyAI { get { return enemyAI; } set { enemyAI = value; } }

    private EnemyUI sEnemyUI;

    public UnitCoolTimer enemyCoolTimer;

    public Transform enemyTransform;
    public Transform playerTransform;

    public Transform enemy_Front;
    public Transform enemy_Right;
    public Transform enemy_Left;
    public ParticleSystem movingEffect;
    public string unitID;
    private int aiLV;

    Vector3 beforePos;

    //AI 레벨에 따라 완벽하게 다른 행동을 할 수 있도록 밑작업
    private List<System.Action> _AI_FuncList = new List<System.Action>();

    void Awake() {
        //이속, 회전속도, 체력, 마나, 파워 게이지, 공격력, 크리확률, 크리계수, AI레벨
        //enemyStat.SampleInit(10.0f, 30.0f, 10, 10, 10, 1, 0.1f, 2.0f, 0);
        //enemyStat.SampleInit(10.0f, 30.0f, 10, 10, 10, 1, 0.1f, 2.0f, sample_AI_Level);
        enemyStat.InitialLize_Enemy_Stat(IO_CSV.__Get_Searched_EnemyBaseStat(unitID));

        //UnitBaseEngine에 Enemy라고 알려준다.
        enemyAIEngine._unitBaseEngine = transform.GetComponent<UnitBaseEngine>();

        enemyAIEngine.enemyCoolTimer = enemyCoolTimer;

        //CombatEngine에서 해당 클래스에 접근할 수 있도록 밑작업
        enemyAIEngine._unitBaseEngine.enemyController = this;
        enemyAIEngine._unitBaseEngine._unit_Combat_Engine.__SET_unit_Base_Engine = enemyAIEngine._unitBaseEngine;
        enemyAIEngine._unitBaseEngine._unit_Move_Engine._SET_unit_Base_Engine = enemyAIEngine._unitBaseEngine;

        enemyAIEngine._unitBaseEngine._unit_Move_Engine.movingEffect = this.movingEffect;

        //Unit__Base_Engine이 Unit__Base_Stat 내용에 접근할 수 있도록 한다.
        enemyAIEngine._unitBaseEngine._unit_Stat = enemyStat;


        enemyAI = transform.GetComponent<EnemyAI>();

        for (int index = 0; index < enemyAIEngine._PUB_enemy_Is_ON_CoolTime.Length; index++)
        {
            enemyAIEngine._PUB_enemy_Is_ON_CoolTime[index] = true;
        }

        _AI_FuncList.Add(() => enemyAI.AI_Simple_Level0());
        _AI_FuncList.Add(() => enemyAI.AI_Simple_Level0_WITH_BOSS());
        _AI_FuncList.Add(() => enemyAI.AI_Simple_Level0_BOSS());
        _AI_FuncList.Add(() => enemyAI.AI_ReinforceLearn_RandomBehave_Ver());
        _AI_FuncList.Add(() => enemyAI.__OLD__AI_DeepLearning__BigData_Ver());
        _AI_FuncList.Add(() => enemyAI.AI_DeepLearning__BigData_Ver());

        aiLV = enemyStat._GET_ai_Level;
    }

	// Use this for initialization
	void Start () {

        beforePos = transform.position;
        //Enemy가 생성될 때 이를 PlayerUI에 알려준다
        GameObject.Find("Main Camera").GetComponent<PlayerUI>().SearchEnemys();

        sEnemyUI = GetComponent<EnemyUI>();

        //생성된 후 자동으로 플레이어를 추적
        while (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
	
	// Update is called once per frame
	void Update () {

        try
        {
            enemyAIEngine.GetAngleComp(transform, playerTransform);

            //아직 살아있을 때
            if (enemyStat.__PUB__Health_Point > 0)
            {
                //나중에 EnemyAI 클래스를 따로 만들어서 이하 내용과 같은 기능을 하도록 넣을 것.
                _AI_FuncList[aiLV]();

                if (Vector3.Distance(beforePos, transform.position) < 0.1 && movingEffect.isPlaying)
                    movingEffect.Stop();

                enemyAIEngine._unitBaseEngine.ManageBUF();
                /*
                //스피드 버프 OR 디버프 지속시간 종료 여부
                if (enemyStat.__PUB_Stat_IsCoolTimeOn[0])
                {
                    //스피드 버프 OR 디버프 해제
                    enemyAIEngine._unitBaseEngine._unit_Move_Engine.Init_Speed_BUF_Amount();
                    //스피드 버프 해제로 일단 간주
                    enemyStat.__PUB_Stat_IsCoolTimeOn[0] = false;
                }

                //체력 버프 OR 디버프 지속시간 종료 여부
                if (enemyStat.__PUB_Stat_IsCoolTimeOn[1])
                {

                }

                //기본 마나 회복 지속시간 종료 여부
                if (enemyStat.__PUB_Stat_IsCoolTimeOn[2])
                {
                    //일단 1씩 회복한다.
                    enemyStat.__Get_HIT__About_Mana(1, -1);
                    //다음 기본 마나 회복 시간까지 대기 
                    enemyStat.__PUB_Stat_IsCoolTimeOn[2] = false;

                    //일단 10초마다 마나를 회복하도록 결정
                    enemyAIEngine.enemyCoolTimer.StartCoroutine(
                        enemyAIEngine.enemyCoolTimer.Timer_Do_Once(10.0f,
                        (input) => { enemyStat.__PUB_Stat_IsCoolTimeOn[2] = input; },
                        false)
                        );
                }

                //PP 버프 OR 디버프 지속시간 종료 여부
                if (enemyStat.__PUB_Stat_IsCoolTimeOn[3])
                {

                }

                //크리티컬 버프 OR 디버프 지속시간 종료 여부
                if (enemyStat.__PUB_Stat_IsCoolTimeOn[4])
                {

                }
                */
            }
            //죽었을 때
            else
            {
                //남아있는 적의 수를 확인한다.
                GameObject.Find("GameManager").GetComponent<MapManager>().__SET_remainedEnemys();

                //파괴
                Destroy(gameObject.GetComponent<MeshFilter>().mesh);
                Destroy(gameObject);
            }
        }
        catch (System.Exception)
        {

        }
    }

    //Enemy가 피격받을 때의 함수
    public void _Enemy_Get_Hit(int damage, bool isItCritical)
    {
        //isHit_OR_Heal 부분은 나중에 Enum과 같은 요소로 변경하여 넣을 것
        enemyStat.__GET_HIT__About_Health(damage, 1);
        //크리티컬 여부에 따라 UI 형식이 달라지기 때문에 관련 정보 송신
        sEnemyUI._SET_isItCritical = isItCritical;
        sEnemyUI.SendMessage("ShowDamage", damage);
    }

    //Enemy가 디버프 스킬에 피격받았을 때의 함수
    public void _Enemy__GET_DeBuff(SkillBaseStat whichDeBuffSkill_Hit_Enemy)
    {
        enemyAIEngine._unitBaseEngine._unit_Combat_Engine.Using_Skill(ref enemy_Front, whichDeBuffSkill_Hit_Enemy, false);
        //_unitBaseEngine.__ENE_C_Engine.Using_Skill_ENE(ref enemy_Front, whichDeBuffSkill_Hit_Enemy, enemyStat, this, false);
    }

    //랜덤 데이터 얻을 때에 한해서만 필요한 함수들
    void OnTriggerEnter(Collider other)
    {
        //if ((enemyStat._GET_ai_Level == 3 || enemyStat._GET_ai_Level == 4 || enemyStat._GET_ai_Level == 5) && transform.GetComponent<Collider>().isTrigger && other.transform.tag == "Player")
        if ((enemyStat._GET_ai_Level == 3 || enemyStat._GET_ai_Level == 4) && transform.GetComponent<Collider>().isTrigger && other.transform.tag == "Player")
        {
            transform.GetComponent<Collider>().isTrigger = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        //if ((enemyStat._GET_ai_Level == 3 || enemyStat._GET_ai_Level == 4 || enemyStat._GET_ai_Level == 5) && !transform.GetComponent<Collider>().isTrigger && other.transform.tag == "Player")
        if ((enemyStat._GET_ai_Level == 3 || enemyStat._GET_ai_Level == 4) && !transform.GetComponent<Collider>().isTrigger && other.transform.tag == "Player")
        {
            transform.GetComponent<Collider>().isTrigger = true;
        }
    }
}
