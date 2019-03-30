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

    public Image enemyIndicator; //enemy가 어떤 방향에 있는지 알려주는 UI
    private bool isEnemyScreenOut;
    private Vector2 screenPos;

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
        //적의 화면 상 위치를 파악한다.
        screenPos = Camera.main.WorldToScreenPoint(transform.position);

        //적이 플레이어 시야 밖으로 나가면
        if (screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height)
        {
            //enemyIndicator가 보이도록 한다.
            if (!enemyIndicator.GetComponent<Image>().enabled) enemyIndicator.GetComponent<Image>().enabled = true;

            //Rotation을 업데이트 해준다. (Enemy의 방향을 가리키는 것은 아님)
            enemyIndicator.transform.rotation = oMainCamera.GetComponent<Transform>().rotation;

            //enemyIndicator의 위치를 업데이트 해준다.
            //하지만 이 방식은 섬 같은 지형이 주변에 있면 enemyIndicator가 지형에 의해 가려질 수 있다.
            //추가적인 연구와 보강이 필요함. 경우에 따라 이 상태로 남겨두고 Enemy 방향을 가리키도록 바꾸기만 하고 넘어갈 수도 있음.
            enemyIndicator.transform.position = Vector3.Lerp(enemyIndicator.transform.position,
                        sPlayerController.transform.position - Vector3.Normalize(sPlayerController.transform.position - sEnemyController.transform.position) * 10.0f,
                        Time.deltaTime * sPlayerController.__PLY_Stat.__PUB_Move_Speed);

            //screenPos = Camera.main.WorldToScreenPoint(enemyIndicator.transform.position);


        }
        //적이 플레이어 시야 안 쪽에 있으면
        else
        {
            //enemyIndicator가 안 보이게 한다.
            if (enemyIndicator.GetComponent<Image>().enabled) enemyIndicator.GetComponent<Image>().enabled = false;
        }
    }

    private void OnGUI()
    {
        
        iHealthBar.fillAmount = (float)sEnemyController.__ENE_Stat.__PUB__Health_Point / 10; // status image udpate
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