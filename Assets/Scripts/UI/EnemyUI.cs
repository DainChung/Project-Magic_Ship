using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyUI : MonoBehaviour {
    private EnemyController sEnemyController; // Enemy Information
    private PlayerController sPlayerController; // Player Informtaion
    public UnitCoolTimer sCoolTimer; // Cool Timer

    public Image iSleekBar; // Health Bar Frame Image Sprite
    public Image iHealthBar; // Health Bar Image Sprite
    public TextMesh t3dDamage; // What show Damage

    //EnemyIndicator를 위한 변수
    private bool isEnemyScreenOut;
    public bool _GET_isEnemyScreenOut   { get { return isEnemyScreenOut; } }

    private Vector2 scrPos;

    private bool bCoolTime;
    private bool bTriggered;

    private GameObject oMainCamera;

    private bool isItCritical;
    public bool _SET_isItCritical
    {
        set { isItCritical = value; }
    }

    // Use this for initialization
    void Start () {
        //enemyIndicatorDestiVector = new Vector3();
        //middle_OF_EnemyIndicator = GameObject.Find("Middle_OF_EnemyIndicator").transform;

        //OnGUI에서 하지 않고도 같은 효과를 기대할 수 있다.
        oMainCamera = GameObject.Find("Main Camera");

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
        isEnemyScreenOut = false;
    }

    void Update()
    {
        scrPos = Camera.main.WorldToScreenPoint(transform.position);

        //적이 플레이어 시야 밖으로 나갔거나 두 개체간 거리가 40.0f 이내일 때
        if ((scrPos.x < 0 || scrPos.x > Screen.width || scrPos.y < 0 || scrPos.y > Screen.height)
            && (Vector3.Distance(sPlayerController.transform.position, sEnemyController.transform.position) < 80.0f))
        {
            isEnemyScreenOut = true;
        }
        //적이 플레이어 시야 안에 있거나 두 개체 간 거리가 40.0f 이상일 때
        else
        {
            isEnemyScreenOut = false;
        }



        //    //20190331 enemyIndicator 자체를 폐기하고 다른 방법을 고려해야 될 수도 있음. (카메라 줌인 & 줌아웃 아니면 적이 공격할 때 위험지역이 표시됨)
        //    //기본 AI의 적들의 방향은 거의 알 수 있음. 하지만 랜덤행동 AI의 경우 매우 잘못된 방향을 가리킬 때가 있음. 우선 이 기능은 보류하고 진행할 것
        //    //4월 첫쨰 주 이내로 존속 여부 결정 후 대체 방안을 개발완료할 것

            //    //적의 화면 상 위치를 파악한다.
            //    screenPos = Camera.main.WorldToScreenPoint(transform.position);

            //    //적이 플레이어 시야 밖으로 나가면
            //    if (screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height)
            //    {
            //        //enemyIndicator가 보이도록 한다.
            //        if (!enemyIndicator.GetComponent<Image>().enabled) enemyIndicator.GetComponent<Image>().enabled = true;

            //        //enemyIndicator 위치 값 1차 보정
            //        enemyIndicatorDestiVector = oMainCamera.transform.GetChild(0).position - Vector3.Normalize(sPlayerController.transform.position - sEnemyController.transform.position) * 0.2f;
            //        screenPos = Camera.main.WorldToScreenPoint(enemyIndicatorDestiVector);

            //        //플레이어 주변 말고 MainCamera에 달려있는 Canvas를 중심으로 나타나도록 하기 위한 2차 보정
            //        enemyIndicatorDestiVector.Set(screenPos.x, screenPos.y, 15f);
            //        enemyIndicatorDestiVector = Camera.main.ScreenToWorldPoint(enemyIndicatorDestiVector);

            //        if (Vector3.Distance(enemyIndicatorDestiVector, middle_OF_EnemyIndicator.position) != 2.90f)
            //            enemyIndicatorDestiVector = middle_OF_EnemyIndicator.position - Vector3.Normalize(middle_OF_EnemyIndicator.position - enemyIndicatorDestiVector) * 2.90f;

            //        //enemyIndicator의 위치를 업데이트 해준다.
            //        enemyIndicator.transform.position = Vector3.Lerp(enemyIndicator.transform.position,
            //                    enemyIndicatorDestiVector,
            //                    Time.deltaTime * sPlayerController.__PLY_Stat.__PUB_Move_Speed * 1.5f);

            //        //Debug.Log(Vector3.Distance(enemyIndicatorDestiVector, middle_OF_EnemyIndicator.position));

            //        //Rotation을 업데이트 해준다. (Enemy의 방향을 가리키는 것은 아님)
            //        enemyIndicator.transform.rotation = oMainCamera.GetComponent<Transform>().rotation;
            //    }
            //    //적이 플레이어 시야 안 쪽에 있으면
            //    else
            //    {
            //        //enemyIndicator가 안 보이게 한다.
            //        if (enemyIndicator.GetComponent<Image>().enabled) enemyIndicator.GetComponent<Image>().enabled = false;
            //    }
    }

    private void OnGUI()
    {
        
        iHealthBar.fillAmount = (float)sEnemyController.__ENE_Stat.__PUB__Health_Point / (float)sEnemyController.__ENE_Stat.__GET_Max_HP; // status image udpate
        // 체력바와 데미지가 잘 보이도록 체력바의 Rotation 업데이트
        try
        {
            if (iHealthBar) iHealthBar.transform.rotation = oMainCamera.GetComponent<Transform>().rotation;
            if (iSleekBar) iSleekBar.transform.rotation = oMainCamera.GetComponent<Transform>().rotation;
            if (t3dDamage) t3dDamage.transform.rotation = oMainCamera.GetComponent<Transform>().rotation;

            // 체력바 프레임과 체력바의 이미지 z-index 이슈 해결
            Vector3 CorrectionValue = new Vector3(0.01f, 0.01f, 0.01f);
            CorrectionValue.Scale(oMainCamera.transform.forward);
            if (iSleekBar) iSleekBar.transform.position = iHealthBar.transform.position + CorrectionValue;


        }
        catch (System.Exception e)
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
            t3dDamage.characterSize = 0.35f;
            t3dDamage.color = Color.yellow;
            t3dDamage.text = damage.ToString();
            //특정 시간동안만 표시되도록 Timer를 작동한다.
            StartCoroutine(sCoolTimer.Timer_Do_Once(1.5f, (input) => { bCoolTime = input; bTriggered = true; }, true));
        }
        else // 크리티컬이 아닌 경우
        {
            t3dDamage.characterSize = 0.3f;
            t3dDamage.color = Color.black;
            t3dDamage.text = damage.ToString();
            StartCoroutine(sCoolTimer.Timer_Do_Once(1f, (input) => { bCoolTime = input; bTriggered = true; }, true));
        }
    }
}