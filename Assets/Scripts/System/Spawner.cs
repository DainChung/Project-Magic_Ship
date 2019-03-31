using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public GameObject WhatToSpawn; // Spawner instantiate this
    public float SpawningDelay; // Spawn instances after 'SpawningDelay' seconds

	// Use this for initialization
	void Start () {
        // destroy spawner if there is no what to spawn 
	    if(!WhatToSpawn)
        {
            Debug.Log("[Spawner] There is no what to spawn: spawner is destroyed - " + gameObject.name);
            Destroy(gameObject);
        }
	}
    
	// Update is called once per frame
	void Update () {
	    
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player") StartCoroutine(SpawnInstances(5, 10.0f));
    }

    /** Spawn Instances drawing circle
     * @param number how many instances are spawned
     * @param radius radius of circle
     */
    IEnumerator SpawnInstances(uint number, float radius)
    {
        yield return new WaitForSeconds(SpawningDelay);

        for (float i = 0; i < number; i++)
        {
            float angle = 2 * Mathf.PI / number * i;

            Vector3 WhereToSpawn = transform.position + radius * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            Instantiate(WhatToSpawn, WhereToSpawn, transform.rotation);
        }

        Destroy(gameObject);
    }
}
