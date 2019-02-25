using UnityEngine;
using System.Collections;

public class MapManager : MonoBehaviour {

    public GameObject seaObject_DEEP;

    private const int mapMax_X = 80;
    private const int mapMax_Z = 80;

    private const float dist = 1.6f;

	// Use this for initialization
	void Start()
    {
        Vector3 pos = new Vector3(0, 0, 0);

        //일정 거리마다 파도효과용 바다 블럭 배치
        for (int x = 0; x < mapMax_X; x++)
        {
            for (int z = 0; z < mapMax_Z; z++)
            {
                pos.Set(x * dist, Random.Range(-0.1f, 0.7f), z * dist);
                Instantiate(seaObject_DEEP, pos, seaObject_DEEP.transform.rotation);
            }
        }
    }
}
