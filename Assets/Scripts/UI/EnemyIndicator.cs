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
    private float newDegree;

    private Vector3 FOriginalLocalScale;

    private const float indicatorDist = 20f;

    public void InitializeEnemyIndicator(Transform enemyTr, Transform playerTr)
    {
        try
        {
            enemy = enemyTr;
            player = playerTr;

            enemyUI = enemy.GetComponent<EnemyUI>();
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
            mainCanvas = mainCamera.GetChild(0);

            FOriginalLocalScale = transform.localScale;
        }
        catch (System.NullReferenceException)
        {
            Destroy(gameObject);
        }
    }

	// Update is called once per frame
	void Update () {

        try
        {
            if (enemyUI._GET_isEnemyScreenOut)
            {
                if (!(transform.GetComponent<Image>().enabled)) transform.GetComponent<Image>().enabled = true;

                //일단 MainCamera를 바라보도록 한다.
                transform.rotation = mainCamera.rotation;

                //enemyIndicator의 화면 상 위치에 대해 보정
                scrPos = Camera.main.WorldToScreenPoint(mainCanvas.position - Vector3.Normalize(mainCanvas.position - enemy.position) * 10f);

                if (scrPos.x <= indicatorDist + 5f) { scrPos.x = indicatorDist + 5f; }
                else if (scrPos.x >= Screen.width - indicatorDist - 5f) { scrPos.x = Screen.width - indicatorDist - 5f; }
                if (scrPos.y <= 20f) { scrPos.y = indicatorDist; }
                else if (scrPos.y >= Screen.height - indicatorDist) { scrPos.y = Screen.height - indicatorDist; }

                //enemyIndicator 위치 마지막 보정
                destiVector3.Set(scrPos.x, scrPos.y, 10f);

                //enemyIndicator가 Enemy 방향을 가리키기 위한 보정
                newDegree = (Quaternion.LookRotation(enemy.position - player.position, Vector3.up)).eulerAngles.y * (-1) + 45f;
                //MainCamera를 바라보면서 Enemy 방향을 가리키기 위해 localRotation을 사용한다.
                transform.localRotation = Quaternion.Euler(0, 0, newDegree);
                transform.localScale = (0.25f * FOriginalLocalScale) / (Vector3.Distance(player.position, enemy.position) * 0.02f);

                //보정된 위치 값 적용
                transform.position = Camera.main.ScreenToWorldPoint(destiVector3);
            }
            else
            {
                transform.GetComponent<Image>().enabled = false;
            }
        }
        //과잉 생성되는 경우 자동 파괴
        catch (System.NullReferenceException)
        { Destroy(gameObject); }
        //갑작스런 Enemy 소멸이 발생했을 때 자동 파괴
        catch (MissingReferenceException)
        { Destroy(gameObject); }
    }
}
