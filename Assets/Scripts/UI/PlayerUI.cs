using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerUI : MonoBehaviour {
    private PlayerController sPlayerController; // 플레이어의 정보

    public Image iBar_Health;   // 체력 게이지
    public Image iBar_Mana;     // 마나 게이지
    public Image iBar_Power;    // 필살기 게이지

    public Image iSkill1;       // 첫번째 스킬
    public Image iSkill2;       // 두번째 스킬
    public Image iSkill3;       // 세번째 스킬

    private void Start()
    {
        // 플레이어 정보 가져오기
        try
        {
            sPlayerController = GameObject.Find("SamplePlayer").GetComponent<PlayerController>();
        }
        catch (System.Exception e)
        {
            Debug.Log("[PlayerUI] " + e.Message);
        }
    }

    private void OnGUI()
    {
        // 스탯 업데이트 (체력, 마나, 파워)
        iBar_Health.fillAmount = (float)sPlayerController.__PLY_Stat.__PUB__Health_Point / 10f;
        iBar_Mana.fillAmount = (float)sPlayerController.__PLY_Stat.__PUB__Mana_Point / 10f;
        iBar_Power.fillAmount = (float)sPlayerController.__PLY_Stat.__PUB__Power_Point / 10f;

        // 스킬 상태 업데이트
        iSkill1.fillAmount = 1 - sPlayerController.__PLY_Selected_Skills[0].time / sPlayerController.__PLY_Selected_Skills[0].__GET_Skill_Cool_Time;
        iSkill2.fillAmount = 1 - sPlayerController.__PLY_Selected_Skills[1].time / sPlayerController.__PLY_Selected_Skills[1].__GET_Skill_Cool_Time;
        iSkill3.fillAmount = 1 - sPlayerController.__PLY_Selected_Skills[2].time / sPlayerController.__PLY_Selected_Skills[2].__GET_Skill_Cool_Time;
    }
}
