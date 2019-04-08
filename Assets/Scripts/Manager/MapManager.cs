using UnityEngine;
using System.Collections;

public class MapManager : MonoBehaviour {

    public GameObject seaObject_DEEP;

    public int mapWidth;
    public int mapHeight;

    private int mapMax_X = 80;
    private int mapMax_Z = 80;

    //Scene 상에 남은 적의 수
    private int remainedEnemys = 0;
    private int remainedEnemySpawners = 0;

    private PlayerUI playerUI;

    private const float dist = 1.6f;

    void Awake()
    {
        mapMax_X = mapWidth;
        mapMax_Z = mapHeight;
    }

	// Use this for initialization
	void Start()
    {
        Vector3 pos = new Vector3(0, 0, 0);

        playerUI = GameObject.Find("Main Camera").GetComponent<PlayerUI>();

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

    public void __SET_remainedEnemys()
    {
        playerUI.SearchEnemys();

        remainedEnemySpawners = GameObject.FindGameObjectsWithTag("Spawner").Length;

        //남아있는 적의 수 업데이트
        remainedEnemys = playerUI.enemysNum;
        //파괴된 Enemys가 일정시간 동안 메모리 상에 남아있기 때문에 1만큼 빼줘야 한다.
        remainedEnemys--;
        if (playerUI.enemysNum != (remainedEnemys + 1)) { Debug.Log("잘못된 입력"); }

        //남아있는 Spawner와 적이 없으면 스테이지 클리어로 인정된다.
        if ((remainedEnemys <= 0) && (remainedEnemySpawners <= 0)) { remainedEnemys = 0; remainedEnemySpawners = 0;  Debug.Log("스테이지 클리어"); }
    }

    public void PlayerDead()
    {
        Debug.Log("실패");
    }
}
