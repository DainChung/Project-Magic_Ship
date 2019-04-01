using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyIndicator : MonoBehaviour {

    private Transform enemy;
    private Transform player;

    private EnemyUI enemyUI;
    private Transform mainCamera;
    private Transform mainCanvas;

    private Vector2 scrPos;
    private Vector3 destiVector3;

    private Vector3 FOriginalLocalScale;

    private const float indicatorDist = 20f;

    public void InitializeEnemyIndicator(Transform enemyTr, Transform playerTr)
    {
        enemy = enemyTr;
        player = playerTr;

        enemyUI = enemy.GetComponent<EnemyUI>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        mainCanvas = mainCamera.GetChild(0);

        FOriginalLocalScale = transform.localScale;
    }

	// Update is called once per frame
	void Update () {
        if (enemyUI._GET_isEnemyScreenOut)
        {
            if(!(transform.GetComponent<Image>().enabled))    transform.GetComponent<Image>().enabled = true;

            transform.rotation = mainCamera.rotation;

            scrPos = Camera.main.WorldToScreenPoint(mainCanvas.position - Vector3.Normalize(mainCanvas.position - enemy.position) * 10f);

            if (scrPos.x <= indicatorDist) { scrPos.x = indicatorDist; }
            else if(scrPos.x >= Screen.width - indicatorDist) { scrPos.x = Screen.width - indicatorDist; }
            if (scrPos.y <= 20f) { scrPos.y = indicatorDist; }
            else if(scrPos.y >= Screen.height - indicatorDist) { scrPos.y = Screen.height - indicatorDist; }

            destiVector3.Set(scrPos.x, scrPos.y, 15f);

            transform.localScale = (3f * FOriginalLocalScale) / (Vector3.Distance(player.position, enemy.position) * 0.1f);

            transform.position = Camera.main.ScreenToWorldPoint(destiVector3);
        }
        else
        {
            transform.GetComponent<Image>().enabled = false;
        }
	}
}
