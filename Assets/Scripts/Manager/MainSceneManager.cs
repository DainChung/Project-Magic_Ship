using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

using File_IO;

using PMS_Exception;

public class MainSceneManager : MonoBehaviour {

    public GameObject windowUpgrade;
    public GameObject windowSkillEquip;

    //================================================================================================
    //업그레이드 창에 대한 버튼과 정보들 및 필요한 변수들
    public Button buttonSavePlayerInfo;

    public Button buttonUpgrade;
    public Button buttonDowngrade;
    public Button buttonBackTOOrigin;

    public Text textStatInfo;
    public Text textGoldInfo;
    public Text textSystemLog_Upgrade;

    //테스트용
    public Button buttonCHEATGetGold;
    //테스트용
    public Button buttonCHEATLoseGold;

    private string statName = "NULL";
    private int statNameIndex = 0;

    private Transform player;

    public Button[] buttonsStat = new Button[8];

    //업그레이드, 다운그레이드 결과를 임시로 저장하는 배열
    private float[] statBeforeSave = new float[8];
    private int goldBeforeSave = -1;

    //나중에 CSV파일에서 읽어오도록 변경할 것
    private string[] statNameArray = new string[8];
    private float[] statValue = new float[8];
    private float[] statMinValue = new float[8];
    private float[] statMaxValue = new float[8];
    private int[] statPrice = new int[8];
    //================================================================================================

    //================================================================================================
    //스킬 장착 창에 대한 버튼과 정보들 및 필요한 변수들
    public Button buttonSaveEquippedSkills;

    public Text textSystemLog_Skill;

    public Transform[] buttonsSkillInventory = new Transform[3];
    //일단 11개로 한정, 단 폭풍우 스킬은 나중에 보스 스테이지 클리어 전까지는 사용할 수 없도록 묶어둘 것
    public Transform[] buttonsValidSkills = new Transform[11];

    private List<SkillBaseStat> allSkills = new List<SkillBaseStat>();
    private List<SkillBaseStat> equippedSkills = new List<SkillBaseStat>();

    //================================================================================================

    void Awake()
    {
        //================================================================================================
        //업그레이드 관련 내용 초기화
        //나중에 CSV 파일에서 읽어올것-------------------------------------------------------
        //--------------------------------------------
        statNameArray[0] = "이동속도";
        statNameArray[1] = "회전속도";

        statNameArray[2] = "최대 체력";
        statNameArray[3] = "최대 마력";
        statNameArray[4] = "최대 궁극지 게이지";

        statNameArray[5] = "공격력";
        statNameArray[6] = "치명타 확률";
        statNameArray[7] = "치명타 계수";
        //--------------------------------------------
        statValue[0] = 5f;
        statValue[1] = 5f;

        statValue[2] = 1f;
        statValue[3] = 1f;
        statValue[4] = 1f;

        statValue[5] = 1f;
        statValue[6] = 1f;
        statValue[7] = 0.1f;
        //--------------------------------------------
        statMinValue[0] = 5f;
        statMinValue[1] = 20f;

        statMinValue[2] = 5f;
        statMinValue[3] = 6f;
        statMinValue[4] = 7f;

        statMinValue[5] = 1f;
        statMinValue[6] = 5f;
        statMinValue[7] = 1.5f;
        //--------------------------------------------
        statMaxValue[0] = 100f;
        statMaxValue[1] = 90f;

        statMaxValue[2] = 30f;
        statMaxValue[3] = 25f;
        statMaxValue[4] = 20f;

        statMaxValue[5] = 10f;
        statMaxValue[6] = 20f;
        statMaxValue[7] = 3f;
        //--------------------------------------------
        statPrice[0] = 1;
        statPrice[1] = 2;

        statPrice[2] = 3;
        statPrice[3] = 4;
        statPrice[4] = 5;

        statPrice[5] = 6;
        statPrice[6] = 7;
        statPrice[7] = 8;
        //--------------------------------------------
        //---------------------------------------------------------------------------------
        //수정한 적 없으면 아래와 같이 -1로 초기화 한다.
        for (int index = 0; index < statBeforeSave.Length; index++)
        {
            statBeforeSave[index] = -1f;
        }

        //================================================================================================

        //================================================================================================
        //스킬 장착 관련 내용 초기화
        //Sample_SkillDB에 저장된 스킬 내용들을 불러오기
        allSkills = IO_CSV.__Get_All_SkillBaseStat();

        //Sample_Player_Info에 저장된 스킬 내용들 불러오기
        equippedSkills = Player_Info_Manager.Read_Equipped_SkillBaseStat();
        //================================================================================================
    }

    // Use this for initialization
    void Start () {
        //모든 window를 숨긴다.
        windowSkillEquip.SetActive(false);
        windowUpgrade.SetActive(false);

        //================================================================================================
        //업그레이드 관련 내용 초기화

        //플레이어 정보를 수정하고 저장하기 위한 초기화
        player = GameObject.FindGameObjectWithTag("Player").transform;

        UpgradeWindowCleaner();

        //saveButton을 누르면 Sample__PlayerEquippedInfo.csv에 장착한 스킬 정보를 저장하도록 한다.
        buttonSavePlayerInfo.onClick.AddListener(() => SavePlayerInfo());

        //초기화 과정에서 무언가가 잘못됨. 일단 노가다로 해결
        buttonsStat[0].onClick.AddListener(() => ClickOnStatButton(0));
        buttonsStat[1].onClick.AddListener(() => ClickOnStatButton(1));

        buttonsStat[2].onClick.AddListener(() => ClickOnStatButton(2));
        buttonsStat[3].onClick.AddListener(() => ClickOnStatButton(3));
        buttonsStat[4].onClick.AddListener(() => ClickOnStatButton(4));

        buttonsStat[5].onClick.AddListener(() => ClickOnStatButton(5));
        buttonsStat[6].onClick.AddListener(() => ClickOnStatButton(6));
        buttonsStat[7].onClick.AddListener(() => ClickOnStatButton(7));

        buttonUpgrade.onClick.AddListener(() => Upgrade_OR_Downgrade_Stat(1f));
        buttonDowngrade.onClick.AddListener(() => Upgrade_OR_Downgrade_Stat(-1f));
        buttonBackTOOrigin.onClick.AddListener(() => Back_TO_Origin_Stat());

        //Debug용
        buttonCHEATGetGold.onClick.AddListener(() => ___CHEAT___GetGold());
        buttonCHEATLoseGold.onClick.AddListener(() => ___CHEAT___LoseGold());
        //================================================================================================
        //스킬 장착 관련 내용 초기화

        buttonSaveEquippedSkills.onClick.AddListener(() => Save_EquippedSkills());

        //스킬 버튼에 스킬 이름들이 나오도록 한다. (장착 스킬 제외)
        for (int i = 0; i < buttonsValidSkills.Length; i++)
        {
            string logForSample = allSkills[i].__GET_Skill_Name;

            buttonsValidSkills[i].GetChild(0).GetComponent<Text>().text = allSkills[i].__GET_Skill_Name;
            buttonsValidSkills[i].GetComponent<Button>().onClick.AddListener(() => Equip_Skill(logForSample));
        }

        SkillEquipWindowCleaner();
        SkillEquipWindowLogCleaner();
        //================================================================================================
    }

    //================================================================================================
    //Upgrade 창에서 사용하지만 보안이 필요없는 것
    public void UpgradeWindowCleaner()
    {
        statName = "NULL";
        statNameIndex = 0;

        //소지 골드 표시
        goldBeforeSave = player.GetComponent<Player_Info_Manager>().__GET_playerInfo._GET_Player_Gold;
        textGoldInfo.text = "소지 골드: " + goldBeforeSave;

        textStatInfo.text = "NULL";
        textSystemLog_Upgrade.text = "선박 업그레이드 가능";

        //아무 스탯도 선택하지 않았을 때 업그레이드를 못하도록 막는다.
        buttonUpgrade.enabled = false;
        buttonDowngrade.enabled = false;
        buttonBackTOOrigin.enabled = false;
    }

    //----------------------------------------------------------------------------------------------------------------------------------
    //Upgrade 창에서 사용하기 위한 함수들 중 보안이 필요한 것들
    private void SavePlayerInfo()
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

        List<string> newPlayerInfo = new List<string>();

        newPlayerInfo.Add(player.GetComponent<Player_Info_Manager>().__GET_playerInfo._GET_Player_ID);
        newPlayerInfo.Add(player.GetComponent<Player_Info_Manager>().__GET_playerInfo._GET_Player_Name);

        newPlayerInfo.Add(player.GetComponent<Player_Info_Manager>().__GET_playerInfo._GET_Player_Level.ToString());
        newPlayerInfo.Add(player.GetComponent<Player_Info_Manager>().__GET_playerInfo._GET_Player_Exp.ToString());

        if (goldBeforeSave != -1) newPlayerInfo.Add(goldBeforeSave.ToString());
        else newPlayerInfo.Add(player.GetComponent<Player_Info_Manager>().__GET_playerInfo._GET_Player_Gold.ToString());

        newPlayerInfo.Add(player.GetComponent<Player_Info_Manager>().__GET_playerInfo._GET_Player_Cash.ToString());

        newPlayerInfo.Add(player.GetComponent<Player_Info_Manager>().__GET_playerInfo._GET_Player_SkinID);


        player.GetComponent<Player_Info_Manager>().__SET_Player_Info(newPlayerInfo);


        textSystemLog_Upgrade.text = "저장되었습니다.";
        //저장 기능 수행
        player.GetComponent<Player_Info_Manager>().Write_Player_Info();
    }


    //임의의 스탯 버튼을 클릭했을 때 스탯 정보를 보여주는 함수
    private void ClickOnStatButton(int statIndex_)
    {
        buttonUpgrade.enabled = true;
        buttonDowngrade.enabled = true;
        buttonBackTOOrigin.enabled = true;

        statNameIndex = statIndex_;

        StatIndex_TO_StatName(statNameIndex);

        textSystemLog_Upgrade.text = statName + "을 수정할 수 있습니다.";

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
        textStatInfo.text = statName + " 값: " + statBeforeSave[statNameIndex].ToString();
    }

    private void StatIndex_TO_StatName(int statIndex)
    {
        statName = statNameArray[statIndex];
    }

    //isUpgrade_OR_Downgrade == 1 || isUpgrade_OR_Downgrade == -1 이 두 가지의 경우에만 작동한다.
    private void Upgrade_OR_Downgrade_Stat(float isUpgrade_OR_Downgrade)
    {


        if (isUpgrade_OR_Downgrade == 1 || isUpgrade_OR_Downgrade == -1)
        {
            if (goldBeforeSave >= statPrice[statNameIndex])
            {
                //돈 계산을 먼저 한다. 업그레이드하면 돈을 소모해야 한다.
                goldBeforeSave -= (int)(statPrice[statNameIndex] * isUpgrade_OR_Downgrade);

                //업그레이드면 증가하고, 다운그레이드면 감소한다.
                float value_About_Stat = statValue[statNameIndex] * isUpgrade_OR_Downgrade;

                statBeforeSave[statNameIndex] += value_About_Stat;

                //최소치 미만일 때
                if (statBeforeSave[statNameIndex] < statMinValue[statNameIndex])
                {
                    //골드 값 다시 회복
                    goldBeforeSave += (int)(statPrice[statNameIndex] * isUpgrade_OR_Downgrade);

                    textSystemLog_Upgrade.text = statName + "의 값을 더 낮출 수 없습니다.";
                    statBeforeSave[statNameIndex] = statMinValue[statNameIndex];
                }
                //최대치 초과일 때
                else if (statBeforeSave[statNameIndex] > statMaxValue[statNameIndex])
                {
                    //골드 값 다시 회복
                    goldBeforeSave += (int)(statPrice[statNameIndex] * isUpgrade_OR_Downgrade);

                    textSystemLog_Upgrade.text = statName + "의 값을 더 높일 수 없습니다.";
                    statBeforeSave[statNameIndex] = statMaxValue[statNameIndex];
                }
            }
            else
            {
                textSystemLog_Upgrade.text = "돈이 부족합니다.";
            }
        }

        //Text 업데이트
        textStatInfo.text = statName + " 값: " + statBeforeSave[statNameIndex].ToString();
        textGoldInfo.text = "소지 골드: " + goldBeforeSave;
    }

    private void Back_TO_Origin_Stat()
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

        goldBeforeSave = player.GetComponent<Player_Info_Manager>().__GET_playerInfo._GET_Player_Gold;

        //Text 업데이트
        textStatInfo.text = statName + " 값: " + statBeforeSave[statNameIndex].ToString();
        textGoldInfo.text = "소지 골드: " + player.GetComponent<Player_Info_Manager>().__GET_playerInfo._GET_Player_Gold;
    }

    //Debug용 함수
    void ___CHEAT___GetGold()
    {
        player.GetComponent<Player_Info_Manager>().__GET_playerInfo.__Buy_OR_Sell__About_Money("Gold", 100);
        goldBeforeSave += 100;
        textSystemLog_Upgrade.text = "GET GOLD CHEAT";
        textGoldInfo.text = "소지 골드: " + player.GetComponent<Player_Info_Manager>().__GET_playerInfo._GET_Player_Gold;
    }
    //Debug용 함수
    void ___CHEAT___LoseGold()
    {
        player.GetComponent<Player_Info_Manager>().__GET_playerInfo.__Buy_OR_Sell__About_Money("Gold", -100);
        goldBeforeSave -= 100;
        textSystemLog_Upgrade.text = "LOSE GOLD CHEAT";
        textGoldInfo.text = "소지 골드: " + player.GetComponent<Player_Info_Manager>().__GET_playerInfo._GET_Player_Gold;
    }
    //================================================================================================

    //================================================================================================
    //SkillEquip 창에서 사용하지만 보안이 필요없는 것
    //장착된 스킬을 업데이트해주는 함수
    public void SkillEquipWindowCleaner()
    {
        //장착 스킬 버튼에 장착한 스킬들 이름이 나오도록 한다.
        //비어있으면 "Empty"로 대신 출력하도록 한다.
        for (int i = 0; i < 3; i++)
        {
            //Listener를 모두 제거한다. (이렇게 안 하면 Listener가 계속 중첩되어 정상작동이 되지 않음)
            buttonsSkillInventory[i].GetComponent<Button>().onClick.RemoveAllListeners();

            try
            {
                //버튼 Text를 장착한 스킬 이름으로 변경한다.
                buttonsSkillInventory[i].GetChild(0).GetComponent<Text>().text = equippedSkills[i].__GET_Skill_Name;

                //Listener에 넣기 위한 string 설정
                string sampleSkillInven_Text = buttonsSkillInventory[i].GetChild(0).GetComponent<Text>().text;
                //Listener를 새로 추가한다.
                buttonsSkillInventory[i].GetComponent<Button>().onClick.AddListener(() => UnEquip_Skill(sampleSkillInven_Text));
            }
            //장착한 스킬이 3개 미만인 경우, "Empty"로 대신 출력하도록 한다.
            catch (System.ArgumentOutOfRangeException)
            {
                //빈 칸의 경우 Empty가 출력되도록 한다.
                buttonsSkillInventory[i].GetChild(0).GetComponent<Text>().text = "Empty";
                //Empty Listener를 추가하여 Exception이 정상작동하도록 한다.
                buttonsSkillInventory[i].GetComponent<Button>().onClick.AddListener(() => UnEquip_Skill("Empty"));
            }
        }
    }

    public void SkillEquipWindowLogCleaner()
    {
        equippedSkills = Player_Info_Manager.Read_Equipped_SkillBaseStat();
        SkillEquipWindowCleaner();
        textSystemLog_Skill.text = "스킬을 장착하거나 뺄 수 있습니다.";
    }

    //----------------------------------------------------------------------------------------------------------------------------------
    //SkillEquip 창에서 사용하기 위한 함수들 중 보안이 필요한 것

    //장착한 스킬 내용을 파일에 저장하는 함수
    private void Save_EquippedSkills()
    {
        textSystemLog_Skill.text = "저장되었습니다.";
        Player_Info_Manager.Write_Equipped_SkillBaseStat(equippedSkills);
    }

    //스킬을 장착하는 함수
    private void Equip_Skill(string skillName)
    {
        try
        {
            //Exception 여부를 검사한다. (스킬을 추가로 장착할 수 있는지 확인한다.)
            MainSceneManagerSkillException.Validate_FullSkillInventoryException(buttonsSkillInventory);
            MainSceneManagerSkillException.Validate_SkillOverlapped_IN_InventoryException(skillName, equippedSkills);

            //스킬을 장착할 빈 자리가 있으면 스킬을 장착한다.
            equippedSkills.Add(Search_SkillBaseStat_BY_Name_IN_List(skillName, allSkills));

            textSystemLog_Skill.text = skillName + "을(를) 장착합니다.";

            //정보를 업데이트한다.
            SkillEquipWindowCleaner();
        }
        //스킬을 장착할 빈 자리가 없는 경우
        catch (MainSceneManagerSkillException.FullSkillInventoryException)
        {
            //안내 메세지만 띄운다.
            textSystemLog_Skill.text = "더 장착할 수 없습니다.";
        }
        //장착하려고 하는 스킬이 이미 장착된 경우
        catch (MainSceneManagerSkillException.SkillOverlapped_IN_InventoryException)
        {
            //안내 메시지만 띄운다.
            textSystemLog_Skill.text = skillName + "을(를) 이미 장착했습니다.";
        }

    }

    //스킬을 장착해제하는 함수
    private void UnEquip_Skill(string buttonText)
    {
        try
        {
            //Exception 여부를 검사한다. (장착 해제하려고 한 칸이 빈 칸인 지 확인한다.)
            MainSceneManagerSkillException.Validate_EmptySkillInventoryException(buttonText);

            //스킬을 제거한다.
            textSystemLog_Skill.text = buttonText + "스킬을 제거합니다";

            equippedSkills.Remove(Search_SkillBaseStat_BY_Name_IN_List(buttonText, equippedSkills));
            //정보를 업데이트한다.
            SkillEquipWindowCleaner();
        }
        catch (MainSceneManagerSkillException.EmptySkillInventoryException)
        {
            textSystemLog_Skill.text = "비어있습니다";
        }
    }

    //스킬 이름으로 특정 SkillBaseStat을 찾는 함수
    private SkillBaseStat Search_SkillBaseStat_BY_Name_IN_List(string skillName, List<SkillBaseStat> searchedList)
    {
        SkillBaseStat resultSBSt = new SkillBaseStat();

        foreach (SkillBaseStat sBSt in searchedList)
        {
            if (skillName == sBSt.__GET_Skill_Name)
            {
                resultSBSt = sBSt;
                break;
            }
        }

        return resultSBSt;
    }
    //================================================================================================
}


