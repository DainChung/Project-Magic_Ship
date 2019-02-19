using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyUI : MonoBehaviour {
    private EnemyController sEnemyController; // Enemy Information
    private PlayerController sPlayerController; // Player Informtaion
    public UnitCoolTimer sCoolTimer; // Cool Timer

    public Image iHealthBar; // Health Bar Image Sprite
    public TextMesh t3dDamage; // What show Damage

    bool bCoolTime;
    bool bTriggered;

    private bool isItCritical;
    public bool _SET_isItCritical
    {
        set { isItCritical = value; }
    }

    // Use this for initialization
    void Start () {
        // 적 정보 가져오기
        try
        {
            sEnemyController = GetComponent<EnemyController>();
            sPlayerController = GameObject.Find("SamplePlayer").GetComponent<PlayerController>();
        }
        catch(System.Exception e)
        {
            Debug.Log("[EnemyUI] " + e.Message);
        }

        // 전역 변수 초기화
        bCoolTime = false;
        bTriggered = false;
    }

    private void OnGUI()
    {
        iHealthBar.fillAmount = (float)sEnemyController.__ENE_Stat.__PUB__Health_Point / 10; // status image udpate
        // 체력바와 데미지가 잘 보이도록 체력바의 Rotation 업데이트
        try
        {
            GameObject oMainCamera = GameObject.Find("Main Camera");

            iHealthBar.transform.rotation = oMainCamera.GetComponent<Transform>().rotation;
            t3dDamage.transform.rotation = oMainCamera.GetComponent<Transform>().rotation;
        }
        catch(System.Exception e)
        {
            Debug.Log("[EnemyUI] +" + e.Message);
        }

        if (bCoolTime || bTriggered)
        {
            t3dDamage.text = "";
            bTriggered = false;
        }
    }

    // 적이 포환에 맞았을 때 피해를 얼마나 입었는지 보여주는 함수
    private void ShowDamage(int damage)
    {
        //크리티컬인 경우
        if (isItCritical)
        {
            //데미지 표시 Text의 사이즈, 색깔, 내용을 정한다.
            t3dDamage.characterSize = 0.4f;
            t3dDamage.color = Color.red;
            t3dDamage.text = damage.ToString();
            //특정 시간동안만 표시되도록 Timer를 작동한다.
            StartCoroutine(sCoolTimer.Timer_Do_Once(1.5f, (input) => { bCoolTime = input; bTriggered = true; }, true));
        }
        else // 크리티컬이 아닌 경우
        {
            t3dDamage.characterSize = 0.25f;
            t3dDamage.color = Color.blue;
            t3dDamage.text = damage.ToString();
            StartCoroutine(sCoolTimer.Timer_Do_Once(1f, (input) => { bCoolTime = input; bTriggered = true; }, true));
        }
    }
}