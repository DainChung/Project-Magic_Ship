using UnityEngine;
using System.Collections;

//스킬 분류를 편하게 하기 위한 것들
namespace SkillBaseCode {
    public enum _SKILL_CODE_Main { BUF, DBF, ATK, FIN };
    public enum _SKILL_CODE_Sub { NULL, HP, MP, PP, SP };
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
