using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace PMS_Exception {
    //스킬 장착과 관련된 Exception
    public class MainSceneManagerSkillException {
        [Serializable]
        public class FullSkillInventoryException : Exception
        {
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
        public class EmptySkillInventoryException : Exception
        {
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
        public class SkillOverlapped_IN_InventoryException : Exception
        {
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
}
