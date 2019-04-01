using UnityEngine;
using System.Collections;

public class Spawner : SpawnerBase {
	// Use this for initialization
	protected void Start () {
        base.Start();
	}
    

    protected void OnTriggerEnter(Collider other)
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
