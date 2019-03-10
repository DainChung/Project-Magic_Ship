using UnityEngine;
using System.Collections;

public class HailStone : MonoBehaviour {
    /** Hailstone Damage */
    public int Damage;

    /** Player hit condition
     * @true Player gets hurt from hailstone
     * @false Player doesn't get hurt from hailstone
     */
    public bool IsPlayerHurt;

    /** Enemy hit condition
     * @true Enemy gets hurt from hailstone
     * @false Player doesn't get hurt from hailstone
     */
    public bool IsEnemyHurt;

	// Use this for initialization
	void Start () {
        if (Damage < 1)
            Damage = 1;
	}

    public void OnTriggerEnter(Collider other)
    {
        // 벽 충돌 판정
        if (other.transform.tag == "SampleObstacle")
            Destroy(gameObject);

        // 적/플레이어 피격 연산
        if (other.transform.tag == "SampleEnemy" && IsEnemyHurt ||
            other.transform.tag == "Player" && IsPlayerHurt)
        {
            // 데미지 연산
            other.GetComponent<UnitBaseEngine>()._unit_Stat.__GET_HIT__About_Health(Damage, 1);

            // 오브젝트 제거
            Destroy(gameObject);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.transform.name == "SampleBottom")
            Destroy(gameObject);
    }
}
