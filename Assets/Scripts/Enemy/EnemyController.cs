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
}

public class EnemyController : MonoBehaviour {

    public EnemyStat __ENE_Stat = new EnemyStat();
    private EnemyEngine __ENE_Engine = new EnemyEngine();

    void Awake() {
        //이속, 회전속도, 체력, 마나, 파워 게이지, 공격력, 크리확률, 크리계수
        __ENE_Stat.SampleInit(10.0f, 30.0f, 10, 10, 10, 1, 10.0f, 1.5f);
    }

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void _Enemy_Get_Hit(int damage)
    {
        //isHit_OR_Heal 부분은 나중에 Enum과 같은 요소로 변경하여 넣을 것
        __ENE_Stat.__GET_HIT__About_Health(damage, 1);
    }
}
