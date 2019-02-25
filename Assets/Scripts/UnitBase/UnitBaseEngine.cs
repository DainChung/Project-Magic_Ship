using UnityEngine;
using System.Collections;

using SkillBaseCode;

public class Unit__Base_Engine {

    //유닛들의 기본적인 움직임에 관한 클래스
    public class Unit__Base_Movement_Engine {

        public Vector3 movingVector;

        private float speed_BUF_Amount;

        //몇몇 변수 값 초기화
        void Awake()
        {
            movingVector.Set(0,0,0);
            speed_BUF_Amount = 0.0f;
        }

        //기본적인 이동을 위한 함수
        public void Move_OBJ(float move_Speed, ref Transform moved_OBJ, float dir)
        {
            try
            {
                //스피드 버프 값을 더하여 이동속도를 늘리거나 줄인다.
                movingVector.Set(0,0,(move_Speed + speed_BUF_Amount) * Time.deltaTime * dir);
                //moved_OBJ.GetComponent<Rigidbody>().AddForce(movingVector);
                moved_OBJ.Translate(movingVector);
            }
            //나중에 dir 값과 관련된 Exception 따로 넣어서 수정할 것
            //후진 시 이속 감소의 구체적인 값은 아직 미정
            catch (System.Exception)
            {
                Debug.Log("dir 값이 1 또는 -0.1이어야 합니다.");
            }
        }
        //기본적인 회전을 위한 함수
        public void Rotate_OBJ(float rotate_Speed, ref Transform rotated_OBJ, int dir)
        {
            try
            {
                rotated_OBJ.Rotate(Vector3.up, Time.deltaTime * (rotate_Speed + speed_BUF_Amount) * dir);
            }
            //나중에 dir 값과 관련된 Exception 따로 넣어서 수정할 것
            catch (System.Exception)
            {
                Debug.Log("dir 값이 1 또는 -1이어야 합니다.");
            }
        }

        //스피드 버프 또는 디버프에 대한 함수 (이동속도 값을 직접적으로 제어하지 않고 다른 변수로 제어)
        public void __GET_BUFF__About_Speed(int isBuff_OR_DeBuff, SkillBaseStat whichSkill, Unit__Base_Stat unitStat, PlayerController plyC)
        {
            try
            {
                speed_BUF_Amount += (whichSkill.__GET_Skill_Rate * isBuff_OR_DeBuff);

                //지속시간 계산
                plyC.StartCoroutine(plyC.__PLY_CoolTimer.Timer_Do_Once(whichSkill.__GET_Skill_ING_Time, (input) => { unitStat.__PUB_Stat_Locker[0] = input; }, unitStat.__PUB_Stat_Locker[0]) );
            }
            //isBuff_OR_DeBuff 값이 1 또는 -1이 아니면 적용되는 Exception으로 대체 예정
            catch (System.Exception)
            {
                //아무것도 안 한다.
            }
        }

        //스피드 관련 버프 OR 디버프가 끝나면 다시 초기화 한다.
        public void Init_Speed_BUF_Amount()
        {
            speed_BUF_Amount = 0;
        }
    }

    //유닛들의 기본적인 공격에 대한 클래스
    //이 외에도 스킬에 대한 내용도 넣어야 될 것으로 보임.
    public class Unit__Base_Combat_Engine {

        private Unit__Base_Movement_Engine unit_M_Engine;
        public Unit__Base_Movement_Engine __SET_unit_M_Engine
        {
            set { unit_M_Engine = value; }
        }

        //하나의 투사체를 일직선 상으로 발사하는 기본 공격 (디버프 유무를 나중에 추가할 것)
        public void Default_ATK(ref GameObject threw_Ammo, ref Transform attacker, int damage, float criRate, float criPoint, SkillBaseStat whichSkill)
        {
            GameObject spawned_OBJ;
            spawned_OBJ = (GameObject)(MonoBehaviour.Instantiate(threw_Ammo, attacker.position, attacker.rotation));
            //투사체가 날아가는 속도를 특정 값으로 설정. 나중엔 DB에서 긁어올 것
            spawned_OBJ.GetComponent<AmmoBase>().__Init_Ammo(55.0f, attacker.tag, damage, criRate, criPoint, whichSkill);
        }


        //Ammo부분은 역시 SkillBaseStat으로 나중에 옮기는 편이 좋을 것으로 보임.
        //atttacker부분은 조금 애매함.
        //Player 전용
        public void Using_Skill(ref GameObject threw_Ammo, ref Transform attacker, SkillBaseStat whichSkill, Unit__Base_Stat unitStat, PlayerController plyC, bool isPlayerUsingThis)
        {
            //필살기인 경우
            if (whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.FIN)
            {

            }
            //일반 스킬인 경우
            else
            {
                //HP에 관여하는 스킬들
                if (whichSkill.__GET_Skill_Code_S == _SKILL_CODE_Sub.HP)
                {
                    _Skill_HP(ref threw_Ammo, ref attacker, whichSkill, unitStat, plyC, isPlayerUsingThis);
                }
                //MP에 관여하는 스킬들 (회복하는 경우에 한정)
                else if (whichSkill.__GET_Skill_Code_S == _SKILL_CODE_Sub.MP)
                {

                }
                //특별한 기능 없음
                else if (whichSkill.__GET_Skill_Code_S == _SKILL_CODE_Sub.NULL)
                {

                }
                //PP에 관여하는 스킬들 (회복하는 경우에 한정)
                else if (whichSkill.__GET_Skill_Code_S == _SKILL_CODE_Sub.PP)
                {

                }
                //이동속도에 관여하는 스킬들
                else if (whichSkill.__GET_Skill_Code_S == _SKILL_CODE_Sub.SP)
                {
                    _SKill_SP(ref threw_Ammo, whichSkill, unitStat, plyC, isPlayerUsingThis);
                }
                //잘못된 스킬
                else
                {

                }
            }
        }

        //HP에 관여하는 스킬들
        //플레이어 전용
        private void _Skill_HP(ref GameObject threw_Ammo, ref Transform attacker, SkillBaseStat whichSkill, Unit__Base_Stat unitStat, PlayerController plyC, bool isPlayerUsingThis)
        {
            int isHit_OR_Heal = 0;

            //도트 힐 OR 도트 딜
            if (whichSkill.__GET_Skill_Code_T == _SKILL_CODE_Time.FREQ)
            {
                //도트 힐 스킬을 사용할 때 OR 도트 딜 디버프를 받았을 때
                if (whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.BUF || !(isPlayerUsingThis))
                {
                    //플레이어가 사용한 HP 도트 힐 스킬이면
                    if (isPlayerUsingThis)
                    {
                        //__Get_HIT__About_Health_FREQ가 HP 도트 힐을 수행하도록 한다.
                        isHit_OR_Heal = -1;
                    }
                    //플레이어가 HP 도트 딜 스킬에 피격된 거면
                    else if (!(isPlayerUsingThis))
                    {
                        //__Get_HIT__About_Health_FREQ가 HP 도트 딜을 수행하도록 한다.
                        isHit_OR_Heal = 1;
                    }
                    //오류
                    else
                    {
                    }


                    //그놈의 StartCoroutine 때문에 PlayerController를 받아와서 이렇게 이상한 형태로 작업함. 후에 다른 방법 알아낸다면 수정 필요
                    plyC.StartCoroutine(unitStat.__Get_HIT__About_Health_FREQ(whichSkill.__GET_Skill_ING_Time, 1.0f, (int)(whichSkill.__GET_Skill_Rate), isHit_OR_Heal));
                }
                //상대에게 도트 딜을 넣을 스킬
                else if (whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.DBF)
                {
                    //디버프가 담긴 하나의 투사체를 발사한다.
                    //투사체의 외형 바꾸기는 일단 넘어갈 것.
                    Default_ATK(ref threw_Ammo, ref attacker, unitStat.__PUB_ATK__Val, unitStat.__PUB_Critical_Rate, unitStat.__PUB_Critical_P, whichSkill);
                }
                //오류
                else
                {

                }
            }
            //일반 힐
            else if (whichSkill.__GET_Skill_Code_T == _SKILL_CODE_Time.NULL)
            {

            }
            //오류
            else
            {

            }
        }

        //SP(이동속도)에 관여하는 스킬들
        private void _SKill_SP(ref GameObject threw_Ammo, SkillBaseStat whichSkill, Unit__Base_Stat unitStat, PlayerController plyC, bool isPlayerUsingThis)
        {
            int isBuFF_OR_DeBuff = 0;

            //SP(이동속도) 버프 스킬 OR 플레이어가 SP(이동속도) 디버프를 받았을 때
            if (whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.BUF || !(isPlayerUsingThis))
            {
                //플레이어가 사용한 거면
                if (isPlayerUsingThis)
                {
                    //__GET_BUFF__About_Speed가 SP(이동속도) 버프를 수행한다.
                    isBuFF_OR_DeBuff = 1;
                }
                //플레이어가 SP(이동속도) 디버프 스킬에 피격된 거면
                else if (!(isPlayerUsingThis))
                {
                    //__GET_BUFF__About_Speed가 SP(이동속도) 디버프를 수행한다.
                    isBuFF_OR_DeBuff = -1;
                }
                //오류
                else
                {
                }

                //이동속도 버프 OR 디버프를 수행한다.
                unit_M_Engine.__GET_BUFF__About_Speed(isBuFF_OR_DeBuff, whichSkill, unitStat, plyC);
            }
            //상대에게 SP(이동속도) 디버프를 걸 때 스킬
            else if (whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.DBF)
            {

            }
            //오류
            else
            {

            }
        }

        //=================================================================================================================================================================================
        //Enemy 전용 스킬 함수
        public void Using_Skill_ENE(ref GameObject threw_Ammo, ref Transform attacker, SkillBaseStat whichSkill, Unit__Base_Stat unitStat, EnemyController eneC, bool isEnemyUsingThis)
        {
            //필살기인 경우
            if (whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.FIN)
            {

            }
            //일반 스킬인 경우
            else
            {
                //HP에 관여하는 스킬들
                if (whichSkill.__GET_Skill_Code_S == _SKILL_CODE_Sub.HP)
                {
                    _Skill_HP_ENE(ref threw_Ammo, whichSkill, unitStat, eneC, isEnemyUsingThis);
                }
                //MP에 관여하는 스킬들 (회복하는 경우에 한정)
                else if (whichSkill.__GET_Skill_Code_S == _SKILL_CODE_Sub.MP)
                {

                }
                //특별한 기능 없음
                else if (whichSkill.__GET_Skill_Code_S == _SKILL_CODE_Sub.NULL)
                {

                }
                //PP에 관여하는 스킬들 (회복하는 경우에 한정)
                else if (whichSkill.__GET_Skill_Code_S == _SKILL_CODE_Sub.PP)
                {

                }
                //이동속도에 관여하는 스킬들
                else if (whichSkill.__GET_Skill_Code_S == _SKILL_CODE_Sub.SP)
                {
                    //_SKill_SP_ENE(ref threw_Ammo, whichSkill, unitStat, eneC, isEnemyUsingThis);
                }
                //잘못된 스킬
                else
                {

                }
            }
        }

        //HP에 관여하는 스킬들
        //Enemy 전용
        private void _Skill_HP_ENE(ref GameObject threw_Ammo, SkillBaseStat whichSkill, Unit__Base_Stat unitStat, EnemyController eneC, bool isEnemyUsingThis)
        {
            int isHit_OR_Heal = 0;

            //도트 힐 OR 도트 딜
            if (whichSkill.__GET_Skill_Code_T == _SKILL_CODE_Time.FREQ)
            {
                //도트 힐 스킬을 사용할 때 OR 도트 딜 디버프를 받았을 때
                if (whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.BUF || !(isEnemyUsingThis))
                {
                    //Enemy가 사용한 HP 도트 힐 스킬이면
                    if (isEnemyUsingThis)
                    {
                        //__Get_HIT__About_Health_FREQ가 HP 도트 힐을 수행하도록 한다.
                        isHit_OR_Heal = -1;
                    }
                    //Enemy가 HP 도트 딜 스킬에 피격된 거면
                    else if (!(isEnemyUsingThis))
                    {
                        //__Get_HIT__About_Health_FREQ가 HP 도트 딜을 수행하도록 한다.
                        isHit_OR_Heal = 1;
                    }
                    //오류
                    else
                    {
                    }


                    //그놈의 StartCoroutine 때문에 EnemyController를 받아와서 이렇게 이상한 형태로 작업함. 후에 다른 방법 알아낸다면 수정 필요
                    eneC.StartCoroutine(unitStat.__Get_HIT__About_Health_FREQ(whichSkill.__GET_Skill_ING_Time, 1.0f, (int)(whichSkill.__GET_Skill_Rate), isHit_OR_Heal));
                }
                //상대에게 도트 딜을 넣을 스킬
                else if (whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.DBF)
                {

                }
                //오류
                else
                {

                }
            }
            //일반 힐
            else if (whichSkill.__GET_Skill_Code_T == _SKILL_CODE_Time.NULL)
            {

            }
            //오류
            else
            {

            }
        }
    }
}
