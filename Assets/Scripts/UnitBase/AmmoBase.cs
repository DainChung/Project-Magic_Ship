using UnityEngine;
using System.Collections;

public class AmmoBase : MonoBehaviour {

    private float __Ammo_Speed;

    //누가 쐈냐
    private string __Who_Shot = "";

    private int __Ammo_Damage;
    public int _GET__Ammo_Damage
    {
        get { return __Ammo_Damage; }
    }

    private Vector3 addedForce;

    //크리 여부를 확인하고 값이 결정됨 true면 크리, 아니면 일반
    public bool isItCritical;

    private SkillBaseStat whichSkill;

    //해당 투사체가 적에게 명중 시 어떤 디버프를 걸 수 있는 지에 대한 정보도 필요함.

    void Start()
    {
        addedForce = transform.forward * __Ammo_Speed;

        transform.GetComponent<Rigidbody>().AddForce(addedForce, ForceMode.Impulse);
    }

    //투사체가 수행하는 연산을 위한 값 초기화
    public void __Init_Ammo(float speed, string tag, int damage, float criRate, float criPoint, SkillBaseStat skillStat)
    {
        __Ammo_Speed = speed;
        __Who_Shot = tag;
        __Ammo_Damage = damage;

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

    void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "SampleObstacle")
        {
            Destroy(gameObject);
        }

        //쏜 애가 Player고 맞은 애가 Enemy면
        if (__Who_Shot == "Player" && other.transform.tag == "SampleEnemy")
        {
            //피격 관련 연산을 하고
            other.GetComponent<EnemyController>()._Enemy_Get_Hit(__Ammo_Damage, isItCritical);

            //디버프가 딸린 투사체의 경우
            if (whichSkill != null)
            {
                Debug.Log("Enemy Get DeBuff");
                //디버프를 준다.
                other.GetComponent<EnemyController>()._Enemy__GET_DeBuff(whichSkill);
            }

            //if (isItCritical)
            //{
            //    Debug.Log("Is Critical");
            //}

            //투사체를 제거한다.
            Destroy(gameObject);
        }

        //쏜 애가 Enemy고 맞은 애가 Player면
        if (__Who_Shot == "SampleEnemy" && other.transform.tag == "Player")
        {
            //피격 관련 연산을 하고 (아직 구현 안 함)
            //Debug.Log("Player Get Hit");

            //디버프가 딸린 투사체의 경우
            if (whichSkill != null)
            {
                //디버프를 준다. (아직 구현 안 함)
                Debug.Log("Player Get DeBuff");
            }

            //투사체를 제거한다.
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
