using UnityEngine;
using System.Collections;

public class AmmoBase : MonoBehaviour {

    protected float __Ammo_Speed;

    //누가 쐈냐
    protected string __Who_Shot = "";

    string whoHit;

    protected int __Ammo_Damage;
    public int _GET__Ammo_Damage
    {
        get { return __Ammo_Damage; }
    }

    protected Vector3 addedForce;

    //크리 여부를 확인하고 값이 결정됨 true면 크리, 아니면 일반
    public bool isItCritical;

    protected SkillBaseStat whichSkill;

    //딥러닝 AI를 위한 정보
    private EnemyAI enemyAI;

    //해당 투사체가 적에게 명중 시 어떤 디버프를 걸 수 있는 지에 대한 정보도 필요함.

    protected void Start()
    {
        addedForce = transform.forward * __Ammo_Speed;

        transform.GetComponent<Rigidbody>().AddForce(addedForce, ForceMode.Impulse);
    }

    //투사체가 수행하는 연산을 위한 값 초기화
    public void __Init_Ammo(float speed, string tag, int damage, float criRate, float criPoint, SkillBaseStat skillStat = null)
    {
        __Ammo_Speed = speed;
        __Who_Shot = tag;
        __Ammo_Damage = damage;

        //디버프가 딸린 투사체면
        if (skillStat != null)
        {
            //디버프 정보를 가져온다.
            whichSkill = skillStat;

            if (whichSkill.__Get_Skill_ID == "NORMAL_HP_01")
                __Ammo_Damage = (int)(whichSkill.__GET_Skill_Rate);
        }
        //평범한 투사체면
        else
        {
            //해당 정보를 비운다.
            whichSkill = null;
        }

        //투사체가 생성되는 시점에서 크리티컬 여부를 판별한다.
        if (Random.Range(0.0f, 1.0f) <= criRate)
        {
            __Ammo_Damage = (int)(__Ammo_Damage * criPoint);

            isItCritical = true;
        }
        else
        {
            isItCritical = false;
        }
    }

    //투사체가 수행하는 연산을 위한 값 초기화
    public void __Init_Ammo(EnemyController _enemy, float speed, string tag, int damage, float criRate, float criPoint, SkillBaseStat skillStat = null)
    {
        __Ammo_Speed = speed;
        __Who_Shot = tag;
        __Ammo_Damage = damage;

        enemyAI = _enemy.PUB_enemyAI;

        if (_enemy.PUB_enemyAI == null) Debug.Log(_enemy.GetInstanceID());

        if (enemyAI == null) Debug.Log(enemyAI.GetInstanceID());

        //디버프가 딸린 투사체면
        if (skillStat != null)
        {
            //디버프 정보를 가져온다.
            whichSkill = skillStat;
        }
        //평범한 투사체면
        else
        {
            //해당 정보를 비운다.
            whichSkill = null;
        }

        //투사체가 생성되는 시점에서 크리티컬 여부를 판별한다.
        if (Random.Range(0.0f, 1.0f) <= criRate)
        {
            __Ammo_Damage = (int)(__Ammo_Damage * criPoint);

            isItCritical = true;
        }
        else
        {
            isItCritical = false;
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        whoHit = other.transform.tag;

        if (whoHit == "SampleObstacle")
        {
            Destroy(gameObject.GetComponent<MeshFilter>().mesh);
            Destroy(gameObject);
        }

        //쏜 애가 Player고 맞은 애가 Enemy면
        if (__Who_Shot == "PlayerAttack" && whoHit == "SampleEnemy")
        {
            //피격 관련 연산을 하고
            other.GetComponent<EnemyController>()._Enemy_Get_Hit(__Ammo_Damage, isItCritical);

            other.GetComponent<Rigidbody>().drag = 1;
            //넉백
            other.GetComponent<Rigidbody>().AddForce(transform.forward * 7.0f, ForceMode.Impulse);

            //0.5 ~ 1.5초 간 얼마나 많이 피격되었는지 알려준다.
            //행동을 갱신하는 동안은 예외로 둔다.
            if (!(other.GetComponent<EnemyAI>().__GET_isbehaveCoolTimeOn)) other.GetComponent<EnemyAI>().getDamagedCounter++;

            //디버프가 딸린 투사체의 경우
            if (whichSkill != null && whichSkill.__GET_Skill_Code_S != SkillBaseCode._SKILL_CODE_Sub.NULL)
            {
                Debug.Log("Enemy Get DeBuff");
                //디버프를 준다.
                other.GetComponent<EnemyController>()._Enemy__GET_DeBuff(whichSkill);
            }

            //if (isItCritical)
            //{
            //    Debug.Log("Is Critical");
            //}

            other.GetComponent<Rigidbody>().drag = 5;

            //투사체를 제거한다.
            Destroy(gameObject.GetComponent<MeshFilter>().mesh);
            Destroy(gameObject);
        }

        //쏜 애가 Enemy고 맞은 애가 Player면
        if (__Who_Shot == "EnemyAttack" && whoHit == "Player")
        {
            //피격 관련 연산을 하고 (아직 구현 안 함)
            //Debug.Log("Player Get Hit");
            other.GetComponent<PlayerController>()._Player_Get_Hit(__Ammo_Damage);

            other.GetComponent<Rigidbody>().drag = 1;
            //넉백
            other.GetComponent<Rigidbody>().AddForce(transform.forward * 4f, ForceMode.Impulse);

            //0.5 ~ 1.5초 간 얼마나 많이 명중했는지 알려준다.
            //행동을 갱신하는 동안은 예외로 둔다.
            if(!enemyAI.__GET_isbehaveCoolTimeOn)   enemyAI.hitCounter++;

            //디버프가 딸린 투사체의 경우
            if (whichSkill != null && whichSkill.__GET_Skill_Code_S != SkillBaseCode._SKILL_CODE_Sub.NULL)
            {
                //디버프를 준다.
                //Debug.Log("Player Get DeBuff");

                other.GetComponent<PlayerController>()._Player_GET_DeBuff(whichSkill);
            }

            other.GetComponent<Rigidbody>().drag = 5;
            //투사체를 제거한다.
            Destroy(gameObject.GetComponent<MeshFilter>().mesh);
            Destroy(gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        //만약 지면과 닿으면 제거한다.
        if (other.transform.tag == "SampleBottom")
        {
            Destroy(gameObject);
        }
    }
}
