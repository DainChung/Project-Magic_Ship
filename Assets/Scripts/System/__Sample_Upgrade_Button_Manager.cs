using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

public class __Sample_Upgrade_Button_Manager : MonoBehaviour {

    //임시 배열, 나중에 이런 과정을 거치지 않고도 기능을 수행할 수 있도록 할 것.
    private string[] statNameArray = new string[8];
    private float[] statValue = new float[8];

    //임시로 업그레이드, 다운그레이드 결과를 저장하는 배열
    private float[] statBeforeSave = new float[8];

    public Transform exitButton;
    public Transform saveButton;

    public Transform upgradeButton;
    public Transform downgradeButton;
    public Transform back_TO_OriginButtons;
    public Transform statInfo;
    private Text statInfoText;

    public Transform[] statButtons = new Transform[8];

    public Transform getGoldCheat;
    public Transform loseGoldCheat;
    public Transform goldInfo;
    private Text goldInfoText;

    //현재 수정 중인 스탯
    private string statName = "NULL";
    private int statNameIndex = 0;

    private Transform player;

    void Awake()
    {
        statNameArray[0] = "이동속도";
        statNameArray[1] = "회전속도";

        statNameArray[2] = "최대 체력";
        statNameArray[3] = "최대 마력";
        statNameArray[4] = "최대 궁극지 게이지";

        statNameArray[5] = "공격력";
        statNameArray[6] = "치명타 확률";
        statNameArray[7] = "치명타 계수";

        statValue[0] = 5f;
        statValue[1] = 5f;

        statValue[2] = 1f;
        statValue[3] = 1f;
        statValue[4] = 1f;

        statValue[5] = 1f;
        statValue[6] = 1f;
        statValue[7] = 0.1f;

        //수정한 적 없으면 아래와 같이 -1로 초기화 한다.
        for (int index = 0; index < statBeforeSave.Length; index++)
        {
            statBeforeSave[index] = -1f;
        }
    }

	// Use this for initialization
	void Start () {
        //숨길 버튼 숨기기
        __Sample_Show_OR_Hide_Buttons(false);

        //플레이어 정보를 수정하고 저장하기 위한 초기화
        player = GameObject.FindGameObjectWithTag("Player").transform;

        //소지 골드 표시
        goldInfoText = goldInfo.GetChild(0).GetComponent<Text>();
        goldInfoText.text = "소지 골드: " + player.GetComponent<Player_Info_Manager>().__GET_playerInfo._GET_Player_Gold;

        statInfoText = statInfo.GetChild(0).GetComponent<Text>();
        statInfoText.text = "NULL";

        //exitButton을 누르면 ProtoType.unity로 scene을 넘기도록 한다.
        exitButton.GetComponent<Button>().onClick.AddListener(() => Go_TO_ProtoTypeScene());

        //saveButton을 누르면 Sample__PlayerEquippedInfo.csv에 장착한 스킬 정보를 저장하도록 한다.
        saveButton.GetComponent<Button>().onClick.AddListener(() => Save_PlayerInfo());

        //초기화 과정에서 무언가가 잘못됨. 일단 노가다로 해결
        statButtons[0].GetComponent<Button>().onClick.AddListener(() => ClickOnStatButton(0));
        statButtons[1].GetComponent<Button>().onClick.AddListener(() => ClickOnStatButton(1));

        statButtons[2].GetComponent<Button>().onClick.AddListener(() => ClickOnStatButton(2));
        statButtons[3].GetComponent<Button>().onClick.AddListener(() => ClickOnStatButton(3));
        statButtons[4].GetComponent<Button>().onClick.AddListener(() => ClickOnStatButton(4));

        statButtons[5].GetComponent<Button>().onClick.AddListener(() => ClickOnStatButton(5));
        statButtons[6].GetComponent<Button>().onClick.AddListener(() => ClickOnStatButton(6));
        statButtons[7].GetComponent<Button>().onClick.AddListener(() => ClickOnStatButton(7));

        upgradeButton.GetComponent<Button>().onClick.AddListener(() => Upgrade_OR_Downgrade_Stat(1f));
        downgradeButton.GetComponent<Button>().onClick.AddListener(() => Upgrade_OR_Downgrade_Stat(-1f));
        back_TO_OriginButtons.GetComponent<Button>().onClick.AddListener(() => Back_TO_Origin_Stat());
    }

    void Go_TO_ProtoTypeScene()
    {
        Debug.Log("게임화면으로 넘어갑니다.");

        //Scene이름을 넣으면 작동됨.
        //단, File > Build Setting에 추가된 Scene만 Load할 수 있음

        //LoadSceneMode.Single은 다른 모든 Scene을 종료하고 "로딩하려는거"만 띄우는 것
        //LoadSceneMode.Additive는 지금까지 켜진 Scene을 종료하지 않고 "로딩하려는거"를 띄우는 것
        SceneManager.LoadScene("ProtoType", LoadSceneMode.Single);
    }

    //장착한 스탯 내용을 파일에 저장하는 함수
    void Save_PlayerInfo()
    {
        //수정한 값 반영
        if (statBeforeSave[0] != -1f) player.GetComponent<PlayerController>().__PLY_Stat.__PUB_Move_Speed = statBeforeSave[0];
        if (statBeforeSave[1] != -1f) player.GetComponent<PlayerController>().__PLY_Stat.__PUB_Rotation_Speed = statBeforeSave[1];

        if (statBeforeSave[2] != -1f) player.GetComponent<PlayerController>().__PLY_Stat.__SET_Max_HP = (int)(statBeforeSave[2]);
        if (statBeforeSave[3] != -1f) player.GetComponent<PlayerController>().__PLY_Stat.__SET_Max_MP = (int)(statBeforeSave[3]);
        if (statBeforeSave[4] != -1f) player.GetComponent<PlayerController>().__PLY_Stat.__SET_Max_PP = (int)(statBeforeSave[4]);

        if (statBeforeSave[5] != -1f) player.GetComponent<PlayerController>().__PLY_Stat.__PUB_ATK__Val = (int)(statBeforeSave[5]);
        if (statBeforeSave[6] != -1f) player.GetComponent<PlayerController>().__PLY_Stat.__PUB_Critical_Rate = statBeforeSave[6];
        if (statBeforeSave[7] != -1f) player.GetComponent<PlayerController>().__PLY_Stat.__PUB_Critical_P = statBeforeSave[7];

        Debug.Log("저장되었습니다.");
        //저장 기능 수행
        player.GetComponent<Player_Info_Manager>().Write_Player_Info();
    }

    //임의의 스탯 버튼을 클릭했을 때 업그레이드, 다운그레이드, 되돌리기 버튼과 스탯 정보를 보여주는 함수
    void ClickOnStatButton(int statIndex_)
    {
        statNameIndex = statIndex_;

        StatIndex_TO_StatName(statNameIndex);

        Debug.Log(statName + "을 수정할 수 있습니다.");
        __Sample_Show_OR_Hide_Buttons(true);

        //값 읽어오기
        statBeforeSave[0] = player.GetComponent<PlayerController>().__PLY_Stat.__PUB_Move_Speed;
        statBeforeSave[1] = player.GetComponent<PlayerController>().__PLY_Stat.__PUB_Rotation_Speed;

        statBeforeSave[2] = player.GetComponent<PlayerController>().__PLY_Stat.__GET_Max_HP;
        statBeforeSave[3] = player.GetComponent<PlayerController>().__PLY_Stat.__GET_Max_MP;
        statBeforeSave[4] = player.GetComponent<PlayerController>().__PLY_Stat.__GET_Max_PP;

        statBeforeSave[5] = player.GetComponent<PlayerController>().__PLY_Stat.__PUB_ATK__Val;
        statBeforeSave[6] = player.GetComponent<PlayerController>().__PLY_Stat.__PUB_Critical_Rate;
        statBeforeSave[7] = player.GetComponent<PlayerController>().__PLY_Stat.__PUB_Critical_P;

        //Text 출력
        statInfoText.text = statName + " 값: " + statBeforeSave[statNameIndex].ToString();
    }

    void StatIndex_TO_StatName(int statIndex)
    {
        statName = statNameArray[statIndex];
    }

    //isUpgrade_OR_Downgrade == 1 || isUpgrade_OR_Downgrade == -1 이 두 가지의 경우에만 작동한다.
    //일단 노가다로 처리한다.
    void Upgrade_OR_Downgrade_Stat(float isUpgrade_OR_Downgrade)
    {
        if (isUpgrade_OR_Downgrade == 1 || isUpgrade_OR_Downgrade == -1)
        {
            //업그레이드면 증가하고, 다운그레이드면 감소한다.
            float value_About_Stat = statValue[statNameIndex] * isUpgrade_OR_Downgrade;

            if (statNameIndex == 0)
                statBeforeSave[0] += value_About_Stat;
            else if (statNameIndex == 1)
                statBeforeSave[1] += value_About_Stat;

            else if (statNameIndex == 2)
                statBeforeSave[2] += value_About_Stat;
            else if (statNameIndex == 3)
                statBeforeSave[3] += value_About_Stat;
            else if (statNameIndex == 4)
                statBeforeSave[4] += value_About_Stat;

            else if (statNameIndex == 5)
                statBeforeSave[5] += value_About_Stat;
            else if (statNameIndex == 6)
                statBeforeSave[6] += value_About_Stat;
            else if (statNameIndex == 7)
                statBeforeSave[7] += value_About_Stat;
            else { }
        }

        //Text 업데이트
        statInfoText.text = statName + " 값: " + statBeforeSave[statNameIndex].ToString();
    }

    void Back_TO_Origin_Stat()
    {
        if (statNameIndex == 0)
            statBeforeSave[0] = player.GetComponent<PlayerController>().__PLY_Stat.__PUB_Move_Speed;
        else if (statNameIndex == 1)
            statBeforeSave[1] = player.GetComponent<PlayerController>().__PLY_Stat.__PUB_Rotation_Speed;

        else if (statNameIndex == 2)
            statBeforeSave[2] = player.GetComponent<PlayerController>().__PLY_Stat.__GET_Max_HP;
        else if (statNameIndex == 3)
            statBeforeSave[3] = player.GetComponent<PlayerController>().__PLY_Stat.__GET_Max_MP;
        else if (statNameIndex == 4)
            statBeforeSave[4] = player.GetComponent<PlayerController>().__PLY_Stat.__GET_Max_PP;

        else if (statNameIndex == 5)
            statBeforeSave[5] = player.GetComponent<PlayerController>().__PLY_Stat.__PUB_ATK__Val;
        else if (statNameIndex == 6)
            statBeforeSave[6] = player.GetComponent<PlayerController>().__PLY_Stat.__PUB_Critical_Rate;
        else if (statNameIndex == 7)
            statBeforeSave[7] = player.GetComponent<PlayerController>().__PLY_Stat.__PUB_Critical_P;
        else { }

        //Text 업데이트
        statInfoText.text = statName + " 값: " + statBeforeSave[statNameIndex].ToString();
    }


    //임시로 사용하는 함수 => 특정 버튼들을 화면에 표시하거나 숨긴다.
    private void __Sample_Show_OR_Hide_Buttons(bool youWannaShowThat)
    {
        upgradeButton.GetComponent<Image>().enabled = youWannaShowThat;
        upgradeButton.GetChild(0).GetComponent<Text>().enabled = youWannaShowThat;

        downgradeButton.GetComponent<Image>().enabled = youWannaShowThat;
        downgradeButton.GetChild(0).GetComponent<Text>().enabled = youWannaShowThat;

        back_TO_OriginButtons.GetComponent<Image>().enabled = youWannaShowThat;
        back_TO_OriginButtons.GetChild(0).GetComponent<Text>().enabled = youWannaShowThat;

        statInfo.GetComponent<Image>().enabled = youWannaShowThat;
        statInfo.GetChild(0).GetComponent<Text>().enabled = youWannaShowThat;
    }
}
