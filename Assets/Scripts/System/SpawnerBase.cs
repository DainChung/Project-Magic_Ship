using UnityEngine;
using System.Collections;

/** Spawn 'WhatToSpawn' after collide with player & 'SpawningDelay' seconds */
public class SpawnerBase : MonoBehaviour {
    public GameObject WhatToSpawn; // Spawner instantiate this
    public float SpawningDelay; // Spawn instances after 'SpawningDelay' seconds
    public bool SpawnAfterColliding; // Spawn instance after colliding with player(true)


    // Use this for initialization
    protected void Start () {

        // destroy spawner if there is no what to spawn 
        if (!WhatToSpawn)
        {
            Debug.Log("[Spawner] There is no what to spawn: spawner is destroyed - " + gameObject.name);
            Destroy(gameObject);
        }

        if (!SpawnAfterColliding)
            StartCoroutine(SpawnInstanceOnce());
	}

    // 에디터에서만 보이는 표시
    protected void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.8f, 0.5f, 0.5f, 0.6f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (SpawnAfterColliding && other.tag == "Player") StartCoroutine(SpawnInstanceOnce());
    }

    /** Spawn Instance after 'SpawningDelay' seconds */
    IEnumerator SpawnInstanceOnce()
    {
        yield return new WaitForSeconds(SpawningDelay);
        Instantiate(WhatToSpawn, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}
