using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyUI : MonoBehaviour {
    private EnemyController sEnemyController; // Enemy Information

    public Image iHealthBar; // Health Bar Image Sprite

	// Use this for initialization
	void Start () {
        // 적 정보 가져오기
        try
        {
            sEnemyController = GetComponent<EnemyController>();
        }
        catch(System.Exception e)
        {
            Debug.Log("[EnemyUI] " + e.Message);
        }

    }

    private void OnGUI()
    {
        iHealthBar.fillAmount = (float)sEnemyController.__ENE_Stat.__PUB__Health_Point / 10; // status image udpate
        // 체력바가 잘 보이도록 체력바의 Rotation 업데이트
        try
        {
            iHealthBar.transform.rotation = GameObject.Find("Main Camera").GetComponent<Transform>().rotation;
        }
        catch(System.Exception e)
        {
            Debug.Log("[EnemyUI] +" + e.Message);
        }
    }
}
