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

    private Camera oMainCamera;

    private int scrHeight, scrWidth;
    private Transform mainCameraTransform;
    private float enemyMaxHP;

    private bool isItCritical;
    public bool _SET_isItCritical
    {
        set { isItCritical = value; }
    }

    void Awake()
    {
        scrHeight = Screen.height;
        scrWidth = Screen.width;
    }

    // Use this for initialization
    void Start () {
        //enemyIndicatorDestiVector = new Vector3();
        //middle_OF_EnemyIndicator = GameObject.Find("Middle_OF_EnemyIndicator").transform;

        //OnGUI에서 하지 않고도 같은 효과를 기대할 수 있다.
        oMainCamera = Camera.main;

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

        mainCameraTransform = oMainCamera.transform;
        enemyMaxHP = (float)sEnemyController.enemyStat.__GET_Max_HP;
    }

    void Update()
    {
        scrPos = oMainCamera.WorldToScreenPoint(transform.position);

        try
        {
            //적이 플레이어 시야 밖으로 나갔거나 두 개체간 거리가 80.0f 이내일 때
            if ((scrPos.x < 0 || scrPos.x > scrWidth || scrPos.y < 0 || scrPos.y > scrHeight)
                && (Vector3.Distance(sPlayerController.transform.position, sEnemyController.transform.position) < 80.0f))
            {
                isEnemyScreenOut = true;
            }
            //적이 플레이어 시야 안에 있거나 두 개체 간 거리가 40.0f 이상일 때
            else
            {
                isEnemyScreenOut = false;
            }
        }
        catch (System.Exception)
        {
            isEnemyScreenOut = false;
        }
    }

    private void OnGUI()
    {
        
        iHealthBar.fillAmount = (float)sEnemyController.enemyStat.__PUB__Health_Point / enemyMaxHP; // status image udpate
        // 체력바와 데미지가 잘 보이도록 체력바의 Rotation 업데이트
        try
        {
            if (iHealthBar) iHealthBar.transform.rotation = mainCameraTransform.rotation;
            if (iSleekBar) iSleekBar.transform.rotation = mainCameraTransform.rotation;
            if (t3dDamage) t3dDamage.transform.rotation = mainCameraTransform.rotation;

            // 체력바 프레임과 체력바의 이미지 z-index 이슈 해결
            Vector3 CorrectionValue = new Vector3(0.01f, 0.01f, 0.01f);
            CorrectionValue.Scale(mainCameraTransform.forward);
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