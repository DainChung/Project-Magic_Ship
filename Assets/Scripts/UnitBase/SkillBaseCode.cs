using UnityEngine;
using System.Collections;

//스킬 분류를 편하게 하기 위한 것들
namespace SkillBaseCode {

    //FIN은 필살기 스킬 분류
    //SPW는 7번, 8번 스킬(각각 지형소환, 우박 스킬)처럼 투사체 외에 무언가를 소환하는 스킬 분류
    public enum _SKILL_CODE_Main { BUF, DBF, ATK, FIN, SPW };

    //MOS는 SPW(투사체 외에 무언가를 소환하는) 스킬 중 마우스를 이용하여 설치하는 스킬 분류
    public enum _SKILL_CODE_Sub { NULL, HP, MP, PP, SP, MOS };
    public enum _SKILL_CODE_Time { NULL, FREQ };

    public class SkillCode {
        public _SKILL_CODE_Main _Skill_Code_M;
        public _SKILL_CODE_Sub _Skill_Code_S;
        public _SKILL_CODE_Time _Skill_Code_T;
    }
}

/*
public class SkillBaseCode {
	public enum _SKILL_CODE_Main { BUF, DBF, ATK, FIN };
    public enum _SKILL_CODE_Sub { NULL, HP, MP, PP, SP };
    public enum _SKILL_CODE_Time { NULL, FREQ };
}
*/
