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
        public void Default_ATK(ref GameObject threw_Ammo, ref Transform attacker, int damage)
        {
            GameObject spawned_OBJ;
            spawned_OBJ = (GameObject)(MonoBehaviour.Instantiate(threw_Ammo, attacker.position, attacker.rotation));
            //투사체가 날아가는 속도를 특정 값으로 설정. 나중엔 DB에서 긁어올 것
            spawned_OBJ.GetComponent<AmmoBase>().__Init_Ammo(55.0f, attacker.tag, damage);
        }


        //Ammo부분은 역시 SkillBaseStat으로 나중에 옮기는 편이 좋을 것으로 보임.
        //atttacker부분은 조금 애매함.
        //Player 전용
        public void Using_Skill(ref GameObject threw_Ammo, ref Transform attacker, SkillBaseStat whichSkill, Unit__Base_Stat unitStat, PlayerController plyC)
        {
            //버프 타입의 스킬
            if (whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.BUF)
            {
                //체력 버프 스킬 (일단 FREQ 버전으로 한정할 것)
                if (whichSkill.__GET_Skill_Code_S == _SKILL_CODE_Sub.HP)
                    _Skill_ADD_Health_Freq(whichSkill, unitStat, plyC);
                else if (whichSkill.__GET_Skill_Code_S == _SKILL_CODE_Sub.SP)
                    unit_M_Engine.__GET_BUFF__About_Speed(1, whichSkill, unitStat, plyC);
                else { }
            }
            //공격 타입의 스킬 (일단 체력 깎는 디버프 공격 스킬로 한정할 것)
            else if (whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.ATK)
            {

            }
            //오류(지정된 타입의 스킬이 아님)
            else { }
        }

        //Enemy 전용
        //아직 미구현
        public void Using_Skill()
        {

        }

        //체력, 마나, 필살기 게이지 등등을 회복하는 스킬들
        //체력을 몇 초당 얼만큼 회복하는 형태의 스킬
        //플레이어 전용
        private void _Skill_ADD_Health_Freq(SkillBaseStat whichSkill, Unit__Base_Stat unitStat, PlayerController plyC)
        {
            //힐 형태로 넣는다.(isHit_OR_Heal == -1)
            //그놈의 StartCoroutine 때문에 PlayerController를 받아와서 이렇게 이상한 형태로 작업함. 후에 다른 방법 알아내서 수정 필요
            plyC.StartCoroutine(  unitStat.__Get_HIT__About_Health_FREQ(whichSkill.__GET_Skill_ING_Time, 1.0f, (int)(whichSkill.__GET_Skill_Rate), -1)  );
        }

        //Enemy 전용
        private void _Skill_ADD_Health_Freq()
        {
        }

        //공격 스킬
        //상대방 체력을 몇 초당 얼만큼 깎아내는 디버프 투사체 발사 스킬(1개만 발사)
        private void _Skill_ATK_Health_DEBUFF_Freq()
        {

        }
    }
}
