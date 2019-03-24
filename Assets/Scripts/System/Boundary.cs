using UnityEngine;
using System.Collections;

public class Boundary : MonoBehaviour {

    public float addForceValueFOR_ENTER_SET;
    public float addForceValueFOR_STAY_SET;
    public Vector3 velocityFOR_EXIT_SET;

    private float addForceValueFOR_ENTER = -30f;
    private float addForceValueFOR_STAY = -10f;
    private Vector3 velocityFOR_EXIT = new Vector3(0,0,-10.0f);

    void Awake()
    {
        addForceValueFOR_ENTER = addForceValueFOR_ENTER_SET;
        addForceValueFOR_STAY = addForceValueFOR_STAY_SET;
        velocityFOR_EXIT = velocityFOR_EXIT_SET;
    }

    void Update()
    {
        addForceValueFOR_ENTER = addForceValueFOR_ENTER_SET;
        addForceValueFOR_STAY = addForceValueFOR_STAY_SET;
        velocityFOR_EXIT = velocityFOR_EXIT_SET;
    }

    //살짝 닿았을 때
    void OnTriggerEnter(Collider other)
    {
        try
        {
            if(other.tag != "Boundary")
                other.GetComponent<Rigidbody>().AddForce(addForceValueFOR_ENTER * transform.forward, ForceMode.Impulse);
        }
        //이 물체와 부딪친 물체에 RigidBody가 없으면
        catch (System.NullReferenceException)
        {
            //아무것도 안 한다.
        }
    }

    //계속해서 접촉을 시도했을때
    void OnTriggerStay(Collider other)
    {
        try
        {
            if (other.tag != "Boundary")
                other.GetComponent<Rigidbody>().AddForce(addForceValueFOR_STAY * transform.forward, ForceMode.Acceleration);
        }
        //이 물체와 부딪친 물체에 RigidBody가 없으면
        catch (System.NullReferenceException)
        {
            //아무것도 안 한다.
        }
    }

    //튕겨져 나갈 때
    void OnTriggerExit(Collider other)
    {
        try
        {
            if (other.tag != "Boundary")
                other.GetComponent<Rigidbody>().velocity = velocityFOR_EXIT;
        }
        //이 물체와 부딪친 물체에 RigidBody가 없으면
        catch (System.NullReferenceException)
        {
            //아무것도 안 한다.
        }
    }
}
