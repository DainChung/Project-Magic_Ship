using UnityEngine;
using System.Collections;

public class ONLY_FOR_RANDOM_DATA_MINING_Boundary : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SampleEnemy")
        {
            other.GetComponent<Collider>().isTrigger = false;
            other.GetComponent<EnemyAI>()._SET_isHitB = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "SampleEnemy")
        {
            other.GetComponent<Collider>().isTrigger = false;
            other.GetComponent<EnemyAI>()._SET_isHitB = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "SampleEnemy")
        {
            other.GetComponent<Collider>().isTrigger = true;
            other.GetComponent<EnemyAI>()._SET_isHitB = false;
        }
    }
}
