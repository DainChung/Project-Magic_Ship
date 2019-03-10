using UnityEngine;
using System.Collections;

public class HailStone : MonoBehaviour {

    private int damage;

    private bool isPlayerUsing;

    public void __Init_HailStone(int damage_input, bool isPlayerUsing_input)
    {
        damage = damage_input;
        isPlayerUsing = isPlayerUsing_input;
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.transform.tag);

        // 벽 충돌 판정
        if (other.transform.tag == "SampleObstacle")
        {
            Destroy(gameObject);
        }

        // 적/플레이어 피격 연산
        if (    (other.transform.tag == "SampleEnemy" && isPlayerUsing) ||
            (other.transform.tag == "Player" && !(isPlayerUsing))   )
        {
            // 데미지 연산
            other.GetComponent<UnitBaseEngine>()._unit_Stat.__GET_HIT__About_Health(damage, 1);

            // 오브젝트 제거
            Destroy(gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.name == "SampleBottom")
        {
            Destroy(gameObject);
        }
    }
}
