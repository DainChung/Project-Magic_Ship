using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using File_IO;

//Sample Scene에서만 임시로 사용할 것
//후에 프로토타입 버전 주둔지 Scene 제작 시 기본 알고리즘만 따와서 변경할 것
public class __Sample_Skill_Button_Manager : MonoBehaviour {

    //임시로 만든 Exception
    public class Exception_FOR___Sample_Skill_Button_Manager {
        [Serializable]
        public class FullSkillInventoryException : Exception {
            public FullSkillInventoryException() { }
        }

        //skill 인벤토리에 빈 자리가 있는지 확인한다.
        public static void Validate_FullSkillInventoryException(Transform[] skillInven)
        {
            int isFull = 0;

            for (int i = 0; i < 3; i++)
            {
                //Empty라고 쓰여진 칸이 없을 때마다
                if (skillInven[i].GetChild(0).GetComponent<Text>().text != "Empty")
                {
                    //isFull을 증가시킨다.
                    isFull++;
                }
            }

            //모든 칸에 스킬이 배정되어 있으면
            if (isFull == 3)
            {
                //FullSkillInventoryException을 던진다 => 스킬 추가 
                throw new FullSkillInventoryException();
            }
        }

        //====================================================================================

        [Serializable]
        public class EmptySkillInventoryException : Exception {
            public EmptySkillInventoryException() { }
        }

        //해당 스킬 인벤토리가 비어있는 칸인지 확인한다.
        public static void Validate_EmptySkillInventoryException(string isEmpty)
        {
            if (isEmpty == "Empty")
            {
                throw new EmptySkillInventoryException();
            }
        }

        //====================================================================================

        [Serializable]
        public class SkillOverlapped_IN_InventoryException : Exception {
            public SkillOverlapped_IN_InventoryException() { }
        }

        //장착하려고 하는 스킬이 이미 장착되어 있는지를 확인한다.
        public static void Validate_SkillOverlapped_IN_InventoryException(string isOverlapped, List<SkillBaseStat> listEquipped)
        {
            foreach (SkillBaseStat skillStat in listEquipped)
            {
                if (isOverlapped == skillStat.__GET_Skill_Name)
                {
                    throw new SkillOverlapped_IN_InventoryException();
                }
            }
        }
    }

    public Transform exitButton;
    public Transform[] sampleSkillInventory = new Transform[3];
    public Transform[] sampleButtons = new Transform[5];

    private List<SkillBaseStat> allSkills = new List<SkillBaseStat>();
    private List<SkillBaseStat> equippedSkills = new List<SkillBaseStat>();

    void Awake()
    {
        //Sample_SkillDB에 저장된 스킬 내용들을 불러오기
        allSkills = IO_CSV.__Get_All_SkillBaseStat();

        //Sample_Player_Info에 저장된 스킬 내용들 불러오기
        equippedSkills = Player_Info_Manager.Read_SkillBaseStat();
    }

    void Start()
    {
        //스킬 버튼에 스킬 이름들이 나오도록 한다. (장착 스킬 제외)
        for (int i = 0; i < 5; i++)
        {
            string logForSample = allSkills[i].__GET_Skill_Name + "를 장착합니다";

            sampleButtons[i].GetChild(0).GetComponent<Text>().text = allSkills[i].__GET_Skill_Name;
            sampleButtons[i].GetComponent<Button>().onClick.AddListener( () => Equip_Skill(logForSample) );
        }

        UpdateSkillInventory();
    }

    void Update()
    {
        
    }

    //스킬을 장착하는 함수
    //스킬 중복 장착하는 경우가 없도록 Exception을 추가해야 됨.
    void Equip_Skill(string log)
    {
        try
        {
            //Exception 여부를 검사한다. (스킬을 추가로 장착할 수 있는지 확인한다.)
            Exception_FOR___Sample_Skill_Button_Manager.Validate_FullSkillInventoryException(sampleSkillInventory);

            //스킬을 장착할 빈 자리가 있으면 스킬을 장착한다.
            Debug.Log(log);
        }
        //스킬을 장착할 빈 자리가 없는 경우
        catch (Exception_FOR___Sample_Skill_Button_Manager.FullSkillInventoryException)
        {
            //안내 메세지만 띄운다.
            Debug.Log("더 장착할 수 없습니다.");
        }
    }

    //스킬을 장착해제하는 함수
    //아직 파일에 쓰는 기능이 없음 20190122
    void UnEquip_Skill(string buttonText)
    {
        try
        {
            //Exception 여부를 검사한다. (장착 해제하려고 한 칸이 빈 칸인 지 확인한다.)
            Exception_FOR___Sample_Skill_Button_Manager.Validate_EmptySkillInventoryException(buttonText);

            //스킬을 제거한다.
            Debug.Log(buttonText + "스킬을 제거합니다");

            equippedSkills.Remove(Search_SkillBaseStat_BY_Name_IN_List(buttonText, equippedSkills));
            //정보를 업데이트한다.
            UpdateSkillInventory();
        }
        catch(Exception_FOR___Sample_Skill_Button_Manager.EmptySkillInventoryException)
        {
            Debug.Log("비어있습니다");
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

    //장착된 스킬을 업데이트해주는 함수
    private void UpdateSkillInventory()
    {
        //장착 스킬 버튼에 장착한 스킬들 이름이 나오도록 한다.
        //비어있으면 "Empty"로 대신 출력하도록 한다.
        for (int i = 0; i < 3; i++)
        {
            //Listener를 모두 제거한다. (이렇게 안 하면 Listener가 계속 중첩되어 정상작동이 되지 않음)
            sampleSkillInventory[i].GetComponent<Button>().onClick.RemoveAllListeners();

            try
            { 
                //버튼 Text를 장착한 스킬 이름으로 변경한다.
                sampleSkillInventory[i].GetChild(0).GetComponent<Text>().text = equippedSkills[i].__GET_Skill_Name;

                //Listener에 넣기 위한 string 설정
                string sampleSkillInven_Text = sampleSkillInventory[i].GetChild(0).GetComponent<Text>().text;
                //Listener를 새로 추가한다.
                sampleSkillInventory[i].GetComponent<Button>().onClick.AddListener(() => UnEquip_Skill(sampleSkillInven_Text));
            }
            //장착한 스킬이 3개 미만인 경우, "Empty"로 대신 출력하도록 한다.
            catch (System.ArgumentOutOfRangeException)
            {
                //빈 칸의 경우 Empty가 출력되도록 한다.
                sampleSkillInventory[i].GetChild(0).GetComponent<Text>().text = "Empty";
                //Empty Listener를 추가하여 Exception이 정상작동하도록 한다.
                sampleSkillInventory[i].GetComponent<Button>().onClick.AddListener(() => UnEquip_Skill("Empty"));
            }
        }
    }
}
