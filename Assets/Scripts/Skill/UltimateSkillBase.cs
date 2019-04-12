using UnityEngine;
using System.Collections;

using System.Collections.Generic;

using File_IO;

//지금은 장판까는 궁극기 전용 스크립트
//나중에 다른 방식의 궁극기를 만들게 되면 해당 스크립트를 수정할 것
public class UltimateSkillBase : MonoBehaviour {

    //이 스킬의 정보
    private SkillBaseStat thisSkillStat;

    //이 스킬의 부가 효과에 관한 정보
    private List<SkillBaseStat> subSkillStats = new List<SkillBaseStat>();

    bool isPlayerUsingThis;

    public void Initialize_U_Skill(SkillBaseStat whichSkill, bool isPlayerUsingThis_Input)
    {
        thisSkillStat = whichSkill;

        //나중엔 UltimateSkill DB를 따로 만들거나 기존의 SKill DB에 부가 효과에 대한 정보를 입력하고 거기서 읽어올 것.
        //아래 조치는 임시 조치

        //이속 디버프 추가
        subSkillStats.Add(IO_CSV.__Get_Searched_SkillBaseStat("NORMAL_SP_01"));
        //차이 확인을 위해 임의로 값을 변경한다. (상대 이속 & 회전속도를 70% 감소시키는 디버프)
        subSkillStats[0].__SET_Skill_Rate = 0.7f;

        isPlayerUsingThis = isPlayerUsingThis_Input;

        Destroy(gameObject, whichSkill.__GET_Skill_ING_Time);
    }

    void OnTriggerStay(Collider other)
    {
        //플레이어가 사용하고 있고 Enemy가 맞았다면
        if (isPlayerUsingThis && other.transform.tag == "SampleEnemy")
        {
            //부가 효과 디버프를 모두 걸어준다.   (부가 효과가 디버프만 있지 않다면 변경할 것)
            foreach (SkillBaseStat debuff in subSkillStats)
            {
                other.GetComponent<EnemyController>()._Enemy__GET_DeBuff(debuff);
            }
        }
        //Enemy가 사용하고 있고 player가 맞았다면
        else if (!isPlayerUsingThis && other.transform.tag == "Player")
        {
            //부가 효과 디버프를 모두 걸어준다.   (부가 효과가 디버프만 있지 않다면 변경할 것)
            foreach (SkillBaseStat debuff in subSkillStats)
            {
                other.GetComponent<PlayerController>()._Player_GET_DeBuff(debuff);
            }
        }

    }

    void OnTriggerExit(Collider other)
    {
        //Enemy가 사용하고 있고 player가 탈출했다면
        if (!isPlayerUsingThis && other.transform.tag == "Player")
        {
            //player가 탈출했음을 알려준다.
            other.GetComponent<PlayerController>()._Player_GetOut_FROM_UltimateSkill();
        }
    }
}
