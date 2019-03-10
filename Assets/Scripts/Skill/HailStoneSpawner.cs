using UnityEngine;
using System.Collections;

public class HailStoneSpawner : MonoBehaviour {
    /** 우박 프리팹 */
    public GameObject HailstonePrefab;

    /** FSpawnTime마다 우박을 소환한다. */
    public float FSpawnTime;

    /** FSpawnNumber의 크기만큼 우박을 소환한다. */
    public int FSpawnNumber;

    /** 우박 소환 지역의 크기 */
    private float FXRange;
    private float FZRange;

    private int skill_Damage;
    private bool isPlayerUsing;

    public void __Init_HailStoneSpawner(SkillBaseStat whichSkill, int damage, bool isPlayerUsing_input)
    {
        FSpawnNumber = (int)(whichSkill.__GET_Skill_ING_Time / FSpawnTime);
        skill_Damage = (int)(damage * whichSkill.__GET_Skill_Rate);
        isPlayerUsing = isPlayerUsing_input;
    }

    // Use this for initialization
    void Start () {
        // 우박 프리팹 가져오기
        if(!HailstonePrefab)    HailstonePrefab = (GameObject)Resources.Load("Prefabs/Bullet/Hailstone");

        FXRange = transform.localScale.x / 2.0f;
        FZRange = transform.localScale.z / 2.0f;

        StartCoroutine(SpawnHailstone());
    }

    // 에디터에서만 보이는 표시
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.8f, 0.5f, 0.5f, 0.6f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }

    /** 우박을 모두 소환한 후 인스턴스 삭제
     * @param FallModifier 낙하 속도     
     */
    IEnumerator SpawnHailstone(float FallModifier = 6)
    {
        // 주기적으로 우박 생성, 임의의 방향으로 떨어짐
        for (int ISpawnedNumber = 0; ISpawnedNumber < FSpawnNumber; ISpawnedNumber++)
        {
            GameObject HailstoneInstance = (GameObject) Instantiate(HailstonePrefab, GetRandomSpawnLocation(), new Quaternion());
            HailstoneInstance.GetComponent<HailStone>().__Init_HailStone(skill_Damage, isPlayerUsing);

            HailstoneInstance.GetComponent<Rigidbody>().AddForce(GetRandomForce(FallModifier), ForceMode.Impulse);

            yield return new WaitForSeconds(FSpawnTime);
        }

        Destroy(gameObject);
        yield break;
    }

    // 낙하할 위치를 임의로 지정
    Vector3 GetRandomSpawnLocation()
    {
        float FRandomX = transform.position.x + Random.Range(-FXRange, FXRange);
        float FRandomZ = transform.position.z + Random.Range(-FZRange, FZRange);
        float FHeight = transform.position.y;

        return new Vector3(FRandomX, FHeight, FRandomZ);
    }

    /** 우박의 낙하 방향을 임의로 지정
     * @param FallModifier 낙하 속도
     */ 
    Vector3 GetRandomForce(float FallModifier)
    {
        float Angle = Random.Range(0, 360);

        float FRandomX = Mathf.Cos(Angle);
        float FRandomZ = Mathf.Sin(Angle);

        if (FallModifier < 0)
            FallModifier = -FallModifier;

        return new Vector3(FRandomX, -FallModifier, FRandomZ);
    }
}