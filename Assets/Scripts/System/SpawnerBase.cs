using UnityEngine;
using System.Collections;

/** Spawn 'WhatToSpawn' after collide with player & 'SpawningDelay' seconds */
public class SpawnerBase : MonoBehaviour {
    public GameObject WhatToSpawn; // Spawner instantiate this
    public float SpawningDelay; // Spawn instances after 'SpawningDelay' seconds

    private PlayerUI playerUI;

    // Use this for initialization
    protected void Start () {

        //Enemy를 새로 만들 때마다 EnemyIndicator를 업데이트 하기 위한 연결
        playerUI = GameObject.Find("Main Camera").GetComponent<PlayerUI>();

        // destroy spawner if there is no what to spawn 
        if (!WhatToSpawn)
        {
            Debug.Log("[Spawner] There is no what to spawn: spawner is destroyed - " + gameObject.name);
            Destroy(gameObject);
        }
	}

    // 에디터에서만 보이는 표시
    protected void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.8f, 0.5f, 0.5f, 0.6f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") StartCoroutine(SpawnInstanceOnce());
    }

    /** Spawn Instance after 'SpawningDelay' seconds */
    IEnumerator SpawnInstanceOnce()
    {
        yield return new WaitForSeconds(SpawningDelay);
        Instantiate(WhatToSpawn, transform.position, transform.rotation);

        //Enemy를 새로 생성할 때마다 EnemyIndicator를 업데이트 해준다.
        playerUI.SearchEnemys();

        Destroy(gameObject);
    }
}
