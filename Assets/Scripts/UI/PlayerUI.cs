using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using System.Collections.Generic;

public class PlayerUI : MonoBehaviour {
    private PlayerController sPlayerController; // 플레이어의 정보

    public Image iBar_Health;   // 체력 게이지
    public Image iBar_Mana;     // 마나 게이지
    public Image iBar_Power;    // 필살기 게이지

    public Image iSkill1;       // 첫번째 스킬
    public Image iSkill2;       // 두번째 스킬
    public Image iSkill3;       // 세번째 스킬

    private GameObject[] enemys;
    private List<GameObject> enemyIndicators = new List<GameObject>();

    public int enemysNum { get { return enemys.Length; } }

    private void Start()
    {
        // 플레이어 정보 가져오기
        try
        {
            sPlayerController = GameObject.Find("SamplePlayer").GetComponent<PlayerController>();
        }
        catch (System.Exception e)
        {
            Debug.Log("[PlayerUI] " + e.Message);
        }

        SearchEnemys();
    }

    private void OnGUI()
    {
        // 스탯 업데이트 (체력, 마나, 파워)
        iBar_Health.fillAmount = (float)sPlayerController.__PLY_Stat.__PUB__Health_Point / (float)sPlayerController.__PLY_Stat.__GET_Max_HP;
        iBar_Mana.fillAmount = (float)sPlayerController.__PLY_Stat.__PUB__Mana_Point / (float)sPlayerController.__PLY_Stat.__GET_Max_MP;
        iBar_Power.fillAmount = (float)sPlayerController.__PLY_Stat.__PUB__Power_Point / (float)sPlayerController.__PLY_Stat.__GET_Max_PP;

        // 스킬 상태 업데이트
        iSkill1.fillAmount = 1 - sPlayerController.__PLY_Selected_Skills[0].time / sPlayerController.__PLY_Selected_Skills[0].__GET_Skill_Cool_Time;
        iSkill2.fillAmount = 1 - sPlayerController.__PLY_Selected_Skills[1].time / sPlayerController.__PLY_Selected_Skills[1].__GET_Skill_Cool_Time;
        iSkill3.fillAmount = 1 - sPlayerController.__PLY_Selected_Skills[2].time / sPlayerController.__PLY_Selected_Skills[2].__GET_Skill_Cool_Time;
    }

    //함수 호출할 때마다 Scene 상의 모든 Enemy를 추적한다.
    //Enemy가 죽거나, 새로 생성될 때 마다 호출해야 한다.
    public void SearchEnemys()
    {
        int index = 0;

        try
        {
            if (enemyIndicators != null)
            {
                //모두 삭제하고 다시 만든다.
                for (index = 0; index < enemyIndicators.Count; index++)
                {
                    Destroy(enemyIndicators[index]);
                }

                enemyIndicators.Clear();
            }

            GameObject newIndicator;

            enemys = GameObject.FindGameObjectsWithTag("SampleEnemy");

            if (enemys[0] != null)
            {

                index = 0;

                do
                {
                    newIndicator = Instantiate(Resources.Load("Prefabs/UI/EnemyIndicator"), Camera.main.transform.GetChild(0)) as GameObject;
                    newIndicator.GetComponent<EnemyIndicator>().InitializeEnemyIndicator(enemys[index].transform, sPlayerController.transform);

                    enemyIndicators.Add(newIndicator);
                    index++;
                } while (index < enemys.Length);
            }
            else
            {

            }
        }
        catch (System.NullReferenceException){ Debug.Log("Hello?"); }
        catch (System.IndexOutOfRangeException) { }
        catch (System.ArgumentException) { }

    }
}
