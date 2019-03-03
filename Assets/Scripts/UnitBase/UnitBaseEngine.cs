using UnityEngine;
using System.Collections;

using System.Reflection;

using SkillBaseCode;

using PMS_Math;

public class Unit__Base_Engine {

    //==================================================================================================================================================

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

    //==================================================================================================================================================
    //==================================================================================================================================================

    //스킬들의 기능을 담고 있는 클래스
    public class Unit__Skill_Engine
    {
        private Unit__Base_Combat_Engine unit_Combat_Engine;
        public Unit__Base_Combat_Engine __SET_unit_Combat_Engine
        {
            set { unit_Combat_Engine = value; }
        }

        private Unit__Base_Movement_Engine unit_Move_Engine;
        public Unit__Base_Movement_Engine __SET_unit_Move_Engine
        {
            set { unit_Move_Engine = value; }
        }

        //Unit__Combat_Engine에 있는 스킬 함수들 이식, 스킬 함수 호출 방법을 Reflection으로 통일
        //HP쪽 스킬들처럼 한 함수가 두 가지 이상의 일을 하는 경우에는 일단 "00000000" || "00000001" 인 경우 모두 "00000000"으로 해석하도록 임시 조치를 취할 것

        //HP에 관여하는 스킬들
        //플레이어 전용
        private void _Skill_00000000(Transform attacker, SkillBaseStat whichSkill, PlayerController plyC, bool isUnitUsingThis)
        {
            int isHit_OR_Heal = 0;

            //도트 힐 OR 도트 딜
            if (whichSkill.__GET_Skill_Code_T == _SKILL_CODE_Time.FREQ)
            {
                //도트 힐 스킬을 사용할 때 OR 도트 딜 디버프를 받았을 때
                if (whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.BUF || !(isUnitUsingThis))
                {
                    //플레이어가 사용한 HP 도트 힐 스킬이면
                    if (isUnitUsingThis)
                    {
                        //__Get_HIT__About_Health_FREQ가 HP 도트 힐을 수행하도록 한다.
                        isHit_OR_Heal = -1;
                    }
                    //플레이어가 HP 도트 딜 스킬에 피격된 거면
                    else if (!(isUnitUsingThis))
                    {
                        //__Get_HIT__About_Health_FREQ가 HP 도트 딜을 수행하도록 한다.
                        isHit_OR_Heal = 1;
                    }
                    //오류
                    else
                    {
                    }
                    

                    //그놈의 StartCoroutine 때문에 PlayerController를 받아와서 이렇게 이상한 형태로 작업함. 후에 다른 방법 알아낸다면 수정 필요
                    plyC.StartCoroutine(plyC.__PLY_Stat.__Get_HIT__About_Health_FREQ(whichSkill.__GET_Skill_ING_Time, 1.0f, (int)(whichSkill.__GET_Skill_Rate), isHit_OR_Heal));
                }
                //상대에게 도트 딜을 넣을 스킬
                else if (whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.DBF)
                {
                    //디버프가 담긴 하나의 투사체를 발사한다.
                    //투사체의 외형 바꾸기는 일단 넘어갈 것.
                    unit_Combat_Engine.Default_ATK(ref attacker, plyC.__PLY_Stat, whichSkill);
                }
                //오류
                else
                {

                }
            }
        }
        //Enemy 전용
        private void _Skill_ENE_00000000(Transform attacker, SkillBaseStat whichSkill, EnemyController eneC, bool isUnitUsingThis)
        {
            int isHit_OR_Heal = 0;

            //도트 힐 OR 도트 딜
            if (whichSkill.__GET_Skill_Code_T == _SKILL_CODE_Time.FREQ)
            {
                //도트 힐 스킬을 사용할 때 OR 도트 딜 디버프를 받았을 때
                if (whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.BUF || !(isUnitUsingThis))
                {
                    //플레이어가 사용한 HP 도트 힐 스킬이면
                    if (isUnitUsingThis)
                    {
                        //__Get_HIT__About_Health_FREQ가 HP 도트 힐을 수행하도록 한다.
                        isHit_OR_Heal = -1;
                    }
                    //플레이어가 HP 도트 딜 스킬에 피격된 거면
                    else if (!(isUnitUsingThis))
                    {
                        //__Get_HIT__About_Health_FREQ가 HP 도트 딜을 수행하도록 한다.
                        isHit_OR_Heal = 1;
                    }
                    //오류
                    else
                    {
                    }


                    //그놈의 StartCoroutine 때문에 PlayerController를 받아와서 이렇게 이상한 형태로 작업함. 후에 다른 방법 알아낸다면 수정 필요
                    eneC.StartCoroutine(eneC.__ENE_Stat.__Get_HIT__About_Health_FREQ(whichSkill.__GET_Skill_ING_Time, 1.0f, (int)(whichSkill.__GET_Skill_Rate), isHit_OR_Heal));
                }
                //상대에게 도트 딜을 넣을 스킬
                else if (whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.DBF)
                {
                    //디버프가 담긴 하나의 투사체를 발사한다.
                    //투사체의 외형 바꾸기는 일단 넘어갈 것.
                    unit_Combat_Engine.Default_ATK(ref attacker, eneC.__ENE_Stat, whichSkill);
                }
                //오류
                else
                {

                }
            }
        }

        //SP(이동속도)에 관여하는 스킬들
        private void _Skill_00000002(Transform attacker, SkillBaseStat whichSkill, PlayerController plyC, bool isUnitUsingThis)
        {
            int isBuFF_OR_DeBuff = 0;

            //SP(이동속도) 버프 스킬 OR 플레이어가 SP(이동속도) 디버프를 받았을 때
            if (whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.BUF || !(isUnitUsingThis))
            {
                //플레이어가 사용한 거면
                if (isUnitUsingThis)
                {
                    //__GET_BUFF__About_Speed가 SP(이동속도) 버프를 수행한다.
                    isBuFF_OR_DeBuff = 1;
                }
                //플레이어가 SP(이동속도) 디버프 스킬에 피격된 거면
                else if (!(isUnitUsingThis))
                {
                    //__GET_BUFF__About_Speed가 SP(이동속도) 디버프를 수행한다.
                    isBuFF_OR_DeBuff = -1;
                }
                //오류
                else
                {
                }

                //이동속도 버프 OR 디버프를 수행한다.
                unit_Move_Engine.__GET_BUFF__About_Speed(isBuFF_OR_DeBuff, whichSkill, plyC.__PLY_Stat, plyC);
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

        //후에 5번 스킬과 6번 스킬을 통합할 것
        //산탄 스킬
        private void _Skill_00000005(Transform attacker, SkillBaseStat whichSkill, PlayerController plyC, bool isPlayerUsingThis)
        {
            Vector2 valueVec2 = Rotation_Math.Rotation_AND_Position(attacker.rotation, 0.58f, 0.0f);

            float posX = attacker.position.x;
            float posY = attacker.position.y;
            float posZ = attacker.position.z;

            //일단 지정된 attacker(캐릭터의 전면부)에서 3개의 기본 탄환을 서로 각 사선에 평행하도록 발사할 것.
            //탄환을 몇 개 발사할 것인지는 나중에 SkillBaseStat에서 읽어올 것
            //일단 3개니까 이렇게 작성할 것

            //본 함수의 0.58f와 1.16f, 이 두 수치에 대해서는 나중에 namespace ConstValueCollection에서 const float spawnDist 변수를 만들어서 관리할 것

            //앞에서 발사하는 거면
            if (attacker.name == "Front")
            {
                unit_Combat_Engine.Default_ATK(ref attacker, attacker.position, attacker.rotation, plyC.__PLY_Stat, whichSkill);
                unit_Combat_Engine.Default_ATK(ref attacker, new Vector3(posX + valueVec2.x, posY, posZ - valueVec2.y), attacker.rotation, plyC.__PLY_Stat, whichSkill);
                unit_Combat_Engine.Default_ATK(ref attacker, new Vector3(posX - valueVec2.x, posY, posZ + valueVec2.y), attacker.rotation, plyC.__PLY_Stat, whichSkill);
                //Default_ATK(ref attacker, new Vector3(newX + 0.58f, newY, newZ), attacker.rotation, unitStat.__PUB_ATK__Val, unitStat.__PUB_Critical_Rate, unitStat.__PUB_Critical_P, whichSkill);
                //Default_ATK(ref attacker, new Vector3(newX - 0.58f, newY, newZ), attacker.rotation, unitStat.__PUB_ATK__Val, unitStat.__PUB_Critical_Rate, unitStat.__PUB_Critical_P, whichSkill);
            }
            //좌측 또는 우측에서 발사하는 거면
            else
            {
                unit_Combat_Engine.Default_ATK(ref attacker, attacker.position, attacker.rotation, plyC.__PLY_Stat, whichSkill);
                unit_Combat_Engine.Default_ATK(ref attacker, new Vector3(posX - valueVec2.x, posY, posZ + valueVec2.y), attacker.rotation, plyC.__PLY_Stat, whichSkill);
                unit_Combat_Engine.Default_ATK(ref attacker, new Vector3(posX + valueVec2.x, posY, posZ - valueVec2.y), attacker.rotation, plyC.__PLY_Stat, whichSkill);
            }
        }

        //속사 스킬
        private void _Skill_00000006(Transform attacker, SkillBaseStat whichSkill, PlayerController plyC, bool isPlayerUsingThis)
        {
            float newX = attacker.position.x;
            float newY = attacker.position.y;
            float newZ = attacker.position.z;

            //산탄 스킬과 코드가 매우 유사하므로 최적화에 대한 고찰이 필요

            //좌측 또는 우측에서 발사하는 거면
            if (attacker.name != "Front")
            {
                unit_Combat_Engine.Default_ATK(ref attacker, attacker.position, attacker.rotation, plyC.__PLY_Stat, whichSkill);
                unit_Combat_Engine.Default_ATK(ref attacker, new Vector3(newX + 1.16f, newY, newZ), attacker.rotation, plyC.__PLY_Stat, whichSkill);
                unit_Combat_Engine.Default_ATK(ref attacker, new Vector3(newX - 1.16f, newY, newZ), attacker.rotation, plyC.__PLY_Stat, whichSkill);
            }
            //앞에서 발사하는 거면
            else
            {
                unit_Combat_Engine.Default_ATK(ref attacker, attacker.position, attacker.rotation, plyC.__PLY_Stat, whichSkill);
                unit_Combat_Engine.Default_ATK(ref attacker, new Vector3(newX, newY, newZ + 1.16f), attacker.rotation, plyC.__PLY_Stat, whichSkill);
                unit_Combat_Engine.Default_ATK(ref attacker, new Vector3(newX, newY, newZ - 1.16f), attacker.rotation, plyC.__PLY_Stat, whichSkill);
            }
        }

        //지형 소환
        private void _Skill_00000007(Transform attacker, SkillBaseStat whichSkill, PlayerController plyC, bool isPlayerUsingThis)
        {
            plyC._Set_SPW_MOS_Skill_Activated = true;

            //7번 스킬 지형소환처럼 마우스로 투사체가 아닌 물체를 소환하는 스킬
            if (whichSkill.__GET_Skill_Code_S == _SKILL_CODE_Sub.MOS)
            {
                //따로 만들어둔 SampleConstructedOBJ를 소환하도록 한다.
                GameObject constructedOBJ = Resources.Load(unit_Combat_Engine.__GET_prefabBulletPath + "SampleConstructedOBJ") as GameObject;

                //소환할 오브젝트 소환하고 값 설정
                GameObject spawned_OBJ;
                spawned_OBJ = (GameObject)(MonoBehaviour.Instantiate(constructedOBJ, plyC.transform.position, plyC.transform.rotation));

                spawned_OBJ.GetComponent<ConstructedOBJ>().__Init_ConstructedOBJ(whichSkill, plyC);
            }

            //8번 스킬 우박처럼 마우스로 투사체가 아닌 것을 소환하는 스킬
            else if (whichSkill.__GET_Skill_Code_S == _SKILL_CODE_Sub.NULL)
            {

            }
            //잘못된 스킬
            else
            {

            }
        }

        //제너릭 메소드(T) 이용하여 Using_Skill(...) 계열과 Using_Skill_ENE(...)계열 통합할 것.
        //그 외 제너릭 메소드를 이용해서 통합할 수 있는 것드 통합할 것

        //위 내용 완료 후 커밋

        //커밋 후 ID 체계화 작업 및 함수 이름에 적용

        //위 내용 완료 후 커밋
    }

    //==================================================================================================================================================
    //==================================================================================================================================================

    //유닛들의 기본적인 공격에 대한 클래스
    //이 외에도 스킬에 대한 내용도 넣어야 될 것으로 보임.
    public class Unit__Base_Combat_Engine {

        private Unit__Base_Movement_Engine unit_M_Engine;
        public Unit__Base_Movement_Engine __SET_unit_M_Engine
        {
            set { unit_M_Engine = value; }
        }

        private Unit__Skill_Engine unit_Skill_Engine;
        public Unit__Skill_Engine __SET_unit_Skill_Engine
        {
            set { unit_Skill_Engine = value; }
        }

        //===============================================================================================================================================

        //소환할 각종 투사체 및 오브젝트들이 있는 경로
        //Resources.Load 함수에 대한 설명은 하단의 Default_ATK 함수 주석 부분 참조
        private string prefabBulletPath = "Prefabs/Bullet/";
        public string __GET_prefabBulletPath
        {
            get { return prefabBulletPath; }
        }


        //하나의 투사체를 일직선 상으로 발사하는 기본 공격 (디버프 유무를 나중에 추가할 것)
        //투사체를 발사할 때 발사 위치와 발사 방향을 따로 지정해줘야 되는 경우
        public void Default_ATK(ref Transform attacker, Vector3 spawnPosition, Quaternion spawnRotation, Unit__Base_Stat unitStat, SkillBaseStat whichSkill)
        {
            //"Assets/Resources/Prefabs/Bullets" 경로에서 직접 Prefab을 뽑아쓰는 쪽으로 변경

            //Resources.Load를 쓸 때는 "Assets/Resources" 경로 내부의 파일에만 접근할 수 있으며
            //Resources.Load("Assets/Resources/<임의폴더0>/<임의폴더1>/<읽으려는거>") 이렇게 쓰면 안 되고
            //Resources.Load("<임의폴더0>/<임의폴더1>/<읽으려는거>") as <자료형> 이런 식으로 써야 된다.

            //읽으려는게 "Assets/Resources" 안에 있지만 그 하위 폴더에 없는 거면
            //Resources.Load("<읽으려는거>") as <자료형> 이렇게 쓰면 된다.
            GameObject threw_Ammo = Resources.Load(prefabBulletPath + "SampleBullet") as GameObject;
            //나중에는 SkillBaseStat에 private string bulletName 변수를 만들고 그것을 읽어오도록 할 것
            //예시)  GameObject threw_Ammo = Resources.Load(prefabBulletPath + whichSkill.bulletName) as GameObject;

            //private string bulletName 변수는 Sample__SkillDataBase.csv에서 읽어오도록 만들것

            GameObject spawned_OBJ;
            spawned_OBJ = (GameObject)(MonoBehaviour.Instantiate(threw_Ammo, spawnPosition, spawnRotation));
            //투사체가 날아가는 속도를 특정 값으로 설정. 나중엔 DB에서 긁어올 것
            spawned_OBJ.GetComponent<AmmoBase>().__Init_Ammo(55.0f, attacker.tag, unitStat.__PUB_ATK__Val, unitStat.__PUB_Critical_Rate, unitStat.__PUB_Critical_P, whichSkill);
        }


        //투사체를 발사할 때 발사 위치와 발사 방향을 따로 지정할 필요가 없는 경우
        public void Default_ATK(ref Transform attacker, Unit__Base_Stat unitStat, SkillBaseStat whichSkill)
        {
            GameObject threw_Ammo = Resources.Load(prefabBulletPath + "SampleBullet") as GameObject;
            //나중에는 SkillBaseStat에 private string bulletName 변수를 만들고 그것을 읽어오도록 할 것
            //예시)  GameObject threw_Ammo = Resources.Load(prefabBulletPath + whichSkill.bulletName) as GameObject;

            //private string bulletName 변수는 Sample__SkillDataBase.csv에서 읽어오도록 만들것

            GameObject spawned_OBJ;
            spawned_OBJ = (GameObject)(MonoBehaviour.Instantiate(threw_Ammo, attacker.position, attacker.rotation));
            //투사체가 날아가는 속도를 특정 값으로 설정. 나중엔 DB에서 긁어올 것
            spawned_OBJ.GetComponent<AmmoBase>().__Init_Ammo(55.0f, attacker.tag, unitStat.__PUB_ATK__Val, unitStat.__PUB_Critical_Rate, unitStat.__PUB_Critical_P, whichSkill);
        }



        ////Ammo부분은 역시 SkillBaseStat으로 나중에 옮기는 편이 좋을 것으로 보임.
        ////Player 전용
        //public void Using_Skill(ref Transform attacker, SkillBaseStat whichSkill, PlayerController plyC, bool isPlayerUsingThis)
        //{
        //    string funcName = "_Skill_";

        //    //임시 조치. 코드 정리 도중 ID 체계화 과정에서 수정 필수
        //    if (whichSkill.__Get_Skill_ID == "00000001")
        //    {
        //        funcName += "00000000";
        //        Debug.Log("This is HP DeBuff Skill");
        //    }
        //    else
        //    {
        //        funcName += whichSkill.__Get_Skill_ID;
        //    }

        //    //funcName = "_Skill_" + whichSkill.__Get_Skill_ID;


        //    System.Type type = unit_Skill_Engine.GetType();

        //    MethodInfo method = type.GetMethod(funcName);
        //    //protected나 private 함수에 접근할 수 있도록 하는 조치
        //    BindingFlags eFlags = BindingFlags.Instance | BindingFlags.NonPublic;

        //    //접근을 위한 조치를 반영한다.
        //    method = typeof(Unit__Skill_Engine).GetMethod(funcName, eFlags);
        //    //funcName과 동일한 이름을 가진 함수를 unit_Skill_Engine에서 호출한다.
        //    method.Invoke(unit_Skill_Engine, new object[] { attacker, whichSkill, plyC, isPlayerUsingThis });
        //}

        //Using_Skill 통합본 작업 중
        //Using_Skill 플레이어 전용 함수 내용 이식
        public void Using_Skill<T>(ref Transform attacker, SkillBaseStat whichSkill, T controller, bool isUnitUsingThis)
        {

            string funcName = "_Skill_";

            //----------------------------------------------------------------
            //실험을 위한 임시 코딩
            //매우 위험한 방법

            //이렇게 하면 Using_Skill은 통합할 수 있지만 각 스킬 함수들은 통합할 수 없음.
            PlayerController nogada_P = new PlayerController();
            EnemyController nogada_E = new EnemyController();

            if (controller.GetType() == nogada_P.GetType())
            {
                Debug.Log("Nogada_P");
            }
            else if (controller.GetType() == nogada_E.GetType())
            {
                Debug.Log("Enemy");
                funcName += "ENE_";
            }
            //----------------------------------------------------------------

            //임시 조치. 코드 정리 도중 ID 체계화 과정에서 수정 필수
            if (whichSkill.__Get_Skill_ID == "00000001")
            {
                funcName += "00000000";
                Debug.Log("This is HP DeBuff Skill");
            }
            else
            {
                funcName += whichSkill.__Get_Skill_ID;
            }

            //funcName = "_Skill_" + whichSkill.__Get_Skill_ID;

            //Reflection 기법에 대해선 아래 주소들 참고
            //0. Reflection 기본 사용법 : http://www.vcskicks.com/call-function.php
            //1. private나 protected 함수에 대한 Reflection 사용법 : https://stackoverflow.com/questions/8413524/how-to-get-an-overloaded-private-protected-method-using-reflection
            System.Type type = unit_Skill_Engine.GetType();

            MethodInfo method = type.GetMethod(funcName);
            //protected나 private 함수에 접근할 수 있도록 하는 조치
            BindingFlags eFlags = BindingFlags.Instance | BindingFlags.NonPublic;

            //접근을 위한 조치를 반영한다.
            method = typeof(Unit__Skill_Engine).GetMethod(funcName, eFlags);
            //funcName과 동일한 이름을 가진 함수를 unit_Skill_Engine에서 호출한다.
            method.Invoke(unit_Skill_Engine, new object[] { attacker, whichSkill, controller, isUnitUsingThis });
        }

        //=================================================================================================================================================================================
        //Enemy 전용 스킬 함수
        public void Using_Skill_ENE(ref Transform attacker, SkillBaseStat whichSkill, Unit__Base_Stat unitStat, EnemyController eneC, bool isEnemyUsingThis)
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
                    _Skill_HP_ENE(whichSkill, unitStat, eneC, isEnemyUsingThis);
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
        private void _Skill_HP_ENE(SkillBaseStat whichSkill, Unit__Base_Stat unitStat, EnemyController eneC, bool isEnemyUsingThis)
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
