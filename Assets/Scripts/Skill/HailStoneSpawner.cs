using UnityEngine;
using System.Collections;

public class HailStoneSpawner : MonoBehaviour {
    /** Hailstone Prefab */
    public GameObject HailstonePrefab;

    /** Location that spawn area is put on */
    public float FSpawnDistance;

    /** Hailstone is spawned randomly per 'FSpawnTime' */
    public float FSpawnTime;

    /** Hailstone is spawned as many as 'FSpawnNumber' */
    public int FSpawnNumber;

    /** Spawn Area Length */
    private float FXRange;
    private float FZRange;
    
    // Use this for initialization
    void Start () {
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
     */
    IEnumerator SpawnHailstone()
    {
        // 우박 프리팹 가져오기
        GameObject HailstonePrefab = (GameObject)Resources.Load("Prefabs/Hailstone");

        // 주기적으로 우박 생성
        for (int ISpawnedNumber = 0; ISpawnedNumber < FSpawnNumber; ISpawnedNumber++)
        {
            Instantiate(HailstonePrefab, GetRandomSpawnLocation(), new Quaternion());
            yield return new WaitForSeconds(FSpawnTime);
        }

        Destroy(gameObject);
        yield break;
    }

    Vector3 GetRandomSpawnLocation()
    {
        float FRandomX = transform.position.x + Random.Range(-FXRange, FXRange);
        float FRandomZ = transform.position.z + Random.Range(-FZRange, FZRange);
        float FHeight = transform.position.y;

        return new Vector3(FRandomX, FHeight, FRandomZ);
    }
}
