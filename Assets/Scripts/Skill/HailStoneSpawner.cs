using UnityEngine;
using System.Collections;

public class HailstoneSpawner : MonoBehaviour {
    /** Hailstone Prefab */
    public GameObject HailstonePrefab;

    /** Hailstone is spawned randomly per 'FSpawnTime' */
    public float FSpawnTime;

    /** Hailstone is spawned as many as 'FSpawnNumber' */
    public int FSpawnNumber;

    /** Spawn Area Length */
    private float FXRange;
    private float FZRange;
    
    // Use this for initialization
    void Start () {
        // 우박 프리팹 가져오기
        if(!HailstonePrefab)
        HailstonePrefab = (GameObject)Resources.Load("Prefabs/Hailstone");

        FXRange = transform.localScale.x / 2.0f;
        FZRange = transform.localScale.z / 2.0f;

        StartCoroutine(SpawnHailstone());
    }

    // Visualize Spawn Area on Editor
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.8f, 0.5f, 0.5f, 0.6f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }

    /** 우박을 모두 소환한 후 인스턴스 삭제
     * @param Radius Force of horizontal direction
     * @param FallModifier Fall speed modifier     
     */
    IEnumerator SpawnHailstone(float Radius = 2, float FallModifier = 6)
    {
        // 주기적으로 우박 생성, 임의의 방향으로 떨어짐
        for (int ISpawnedNumber = 0; ISpawnedNumber < FSpawnNumber; ISpawnedNumber++)
        {
            GameObject HailstoneInstance = (GameObject) Instantiate(HailstonePrefab, GetRandomSpawnLocation(), new Quaternion());
            HailstoneInstance.GetComponent<Rigidbody>().AddForce(GetRandomForce(Radius, FallModifier), ForceMode.Impulse);
            yield return new WaitForSeconds(FSpawnTime);
        }

        Destroy(gameObject);
        yield break;
    }

    // Get random spawn location of hailstone
    Vector3 GetRandomSpawnLocation()
    {
        float FRandomX = transform.position.x + Random.Range(-FXRange, FXRange);
        float FRandomZ = transform.position.z + Random.Range(-FZRange, FZRange);
        float FHeight = transform.position.y;

        return new Vector3(FRandomX, FHeight, FRandomZ);
    }

    /** Get random force direction of hailstone
     * @param Radius Force of horizontal direction
     * @param FallModifier Fall speed modifier
     */ 
    Vector3 GetRandomForce(float Radius, float FallModifier)
    {
        float Angle = Random.Range(0, 360);

        float FRandomX = Mathf.Cos(Angle);
        float FRandomZ = Mathf.Sin(Angle);

        if (FallModifier < 0)
            FallModifier = -FallModifier;

        return new Vector3(FRandomX, -FallModifier, FRandomZ);
    }
}