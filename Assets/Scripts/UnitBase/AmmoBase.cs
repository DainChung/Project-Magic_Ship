using UnityEngine;
using System.Collections;

public class AmmoBase : MonoBehaviour {

    private float __Ammo_Speed;

    //누가 쐈냐
    private string __Who_Shot = "";

    private int __Ammo_Damage;

    private Vector3 addedForce;

    //해당 투사체가 적에게 명중 시 어떤 디버프를 걸 수 있는 지에 대한 정보도 필요함.

    void Start()
    {
        addedForce = transform.forward * __Ammo_Speed;

        transform.GetComponent<Rigidbody>().AddForce(addedForce, ForceMode.Impulse);
    }

    //투사체가 수행하는 연산을 위한 값 초기화
    public void __Init_Ammo(float speed, string tag, int damage)
    {
        __Ammo_Speed = speed;
        __Who_Shot = tag;
        __Ammo_Damage = damage;
    }

    void OnTriggerEnter(Collider other)
    {
        //쏜 애가 Player고 맞은 애가 Enemy면
        if (__Who_Shot == "Player" && other.transform.tag == "SampleEnemy")
        {
            //피격 관련 연산을 하고
            other.GetComponent<EnemyController>()._Enemy_Get_Hit(__Ammo_Damage);

            //if()

            //투사체를 제거한다.
            Destroy(gameObject);
        }

        //쏜 애가 Enemy고 맞은 애가 Player면
        if (__Who_Shot == "SampleEnemy" && other.transform.tag == "Player")
        {
            //피격 관련 연산을 하고 (아직 구현 안 됨)
            Debug.Log("Player Get Hit");
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
