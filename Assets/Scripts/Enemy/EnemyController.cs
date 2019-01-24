using UnityEngine;
using System.Collections;

public class EnemyStat : Unit__Base_Stat {

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
    }
}

public class EnemyEngine : Unit__Base_Engine {
    public Unit__Base_Movement_Engine __ENE_M_Engine = new Unit__Base_Movement_Engine();
    public Unit__Base_Combat_Engine __ENE_C_Engine = new Unit__Base_Combat_Engine();

    //destinationDir 방향으로 회전하는 함수
    public void Rotate_TO_Direction(float rotate_Speed, ref Transform rotated_OBJ, int dir, Transform destiTrn, int is_NOT_RunAway)
    {
        //두 지점 사이의 각도 구하기 (도달해야 되는 각도)
        //도망가는 경우 플레이어의 반대 방향으로 도망간다.
        float destiAngle = Mathf.Atan2(destiTrn.position.x - rotated_OBJ.position.x * (float)(is_NOT_RunAway), destiTrn.position.z - rotated_OBJ.position.z * (float)(is_NOT_RunAway)) * Mathf.Rad2Deg;

        //rotated_OBJ.Rotate(0, destiAngle,0);

        //각도가 다음과 같을 때는 시계 반대방향으로 도는 것이 더 빠르다.
        //직접 실험해본 결과, 시간이 더 오래 걸리는 방향으로 도는 경우가 포착되었음
        //방향이나 아래 알고리즘에 관하여 추가적인 연구가 필요할 것으로 보임.
        if (destiAngle < 0f)
        {
            dir = -1;
        }

        Quaternion destiQT = Quaternion.Euler(0, destiAngle, 0);

        //rotated_OBJ.rotation = destiQT;

        //호출될 때마다 각도 구하기
        float angleComparison = Quaternion.Angle(rotated_OBJ.rotation, destiQT);

        //약간의 오차를 허용한다.
        //목표지점을 바라볼 때까지 회전한다.
        if (!((angleComparison < 1.0f) && (angleComparison > - 1.0f)))
        {
            __ENE_M_Engine.Rotate_OBJ(rotate_Speed, ref rotated_OBJ, dir);
        }

        //Debug.Log(angleComparison);
    }
}

public class EnemyController : MonoBehaviour {

    private EnemyStat __ENE_Stat = new EnemyStat();
    private EnemyEngine __ENE_Engine = new EnemyEngine();

    public Transform enemyTransform;
    public Transform playerTransform;

    void Awake() {
        //이속, 회전속도, 체력, 마나, 파워 게이지, 공격력, 크리확률, 크리계수
        __ENE_Stat.SampleInit(10.0f, 30.0f, 10, 10, 10, 1, 10.0f, 1.5f);
    }

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        //도망가야할 때
        __ENE_Engine.Rotate_TO_Direction(__ENE_Stat.__PUB_Rotation_Speed, ref enemyTransform, 1, playerTransform, -1);
        //도망가지 않을 때
        //__ENE_Engine.Rotate_TO_Direction(__ENE_Stat.__PUB_Rotation_Speed, ref enemyTransform, 1, playerTransform, 1);
    }

    public void _Enemy_Get_Hit(int damage)
    {
        //isHit_OR_Heal 부분은 나중에 Enum과 같은 요소로 변경하여 넣을 것
        __ENE_Stat.__GET_HIT__About_Health(damage, 1);
    }
}
