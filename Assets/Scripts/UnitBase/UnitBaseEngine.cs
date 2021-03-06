﻿using UnityEngine;
using System.Collections;

using System.Reflection;

using SkillBaseCode;

using PMS_Math;

public class UnitBaseEngine : MonoBehaviour
{

    //------------------------------------------
    //인스펙터 창에 보일 필요가 없으므로 숨긴다.
    //각 클래스를 사용하기 위한 선언
    [HideInInspector]
    public Unit__Base_Movement_Engine _unit_Move_Engine = new Unit__Base_Movement_Engine();
    [HideInInspector]
    public Unit__Base_Combat_Engine _unit_Combat_Engine = new Unit__Base_Combat_Engine();

    //스탯에 접촉하기 위한 선언
    [HideInInspector]
    public Unit__Base_Stat _unit_Stat;

    //PlayerController나 EnemyController가 부득이하게 필요한 경우 (Unit__Base_Engine에서 사용하는 경우)를 위한 임시 조치
    [HideInInspector]
    public PlayerController playerController;
    [HideInInspector]
    public EnemyController enemyController;

    private int _coolTimeCheck = 0x000000000000;
    public int coolTimeCheck
    {
        get { return _coolTimeCheck; }
        set { _coolTimeCheck = value; }
    }
    public enum COOL_TIME_CODE { FORWARD, BACK, RIGHT, LEFT, ATK, HP, MP, PP, SP, CR, EX };
    float dummy = 0.0f;

    //-----------------------------------------

    void Awake()
    {
        //기본 마나 회복 활성화
        SetCoolTimeCheck(COOL_TIME_CODE.MP);

        //플레이어면
        if (playerController != null)
        {
            //Unit__Base_Combat_Engine에 player라는 정보를 전달한다.
            _unit_Combat_Engine._playerController = playerController;
        }
        //Enemy면
        else
        {
            //Unit__Base_Combat_Engine에 enemy라는 정보를 전달한다.
            _unit_Combat_Engine._enemyController = enemyController;
        }
    }

    public int TimeCodeToINT(COOL_TIME_CODE code)
    {
        int timeIndex = 0x000000000001;

        for (int i = 1; i <= (int)(code); i++)
            timeIndex = timeIndex << 1;

        return timeIndex;
    }

    public bool CoolTimeCheck(COOL_TIME_CODE code)
    {
        int timeIndex = TimeCodeToINT(code);

        return ((coolTimeCheck & timeIndex) == timeIndex);
    }

    public void SetCoolTimeCheck(COOL_TIME_CODE code)
    {
        int timeIndex = TimeCodeToINT(code);

        if ((coolTimeCheck & timeIndex) == timeIndex)
            coolTimeCheck -= timeIndex;
        else
            coolTimeCheck += timeIndex;
    }

    public void ManageBUF()
    {
        //스피드 버프 OR 디버프 지속시간 종료 여부
        if (CoolTimeCheck(COOL_TIME_CODE.SP))
        {
            _unit_Move_Engine.Init_Speed_BUF_Amount();
        }
        //기본 마나 회복, 10초당 1번만 작동
        if (CoolTimeCheck(COOL_TIME_CODE.MP) && (_unit_Stat.__PUB__Mana_Point < _unit_Stat.__GET_Max_MP))
        {
            //마나 1 회복
            //첫 마나 회복 버그 방지용
            if (CoolTimeCheck(COOL_TIME_CODE.EX))
                _unit_Stat.__Get_HIT__About_Mana(1, -1);
            else
                SetCoolTimeCheck(COOL_TIME_CODE.EX);
            //잠금
            SetCoolTimeCheck(COOL_TIME_CODE.MP);
            //일단 10초마다 마나를 회복하기 위해 타이머 작동
            if (enemyController != null)
            {
                StartCoroutine(enemyController.enemyCoolTimer.Timer_Do_Once(10.0f,
                        (input) => { coolTimeCheck = input; },
                        coolTimeCheck,
                        TimeCodeToINT(COOL_TIME_CODE.MP)
                        ));
            }
            else if (playerController != null)
            {
                StartCoroutine(playerController.__PLY_CoolTimer.Timer_Do_Once(10.0f,
                        (input) => { coolTimeCheck = input; },
                        coolTimeCheck,
                        TimeCodeToINT(COOL_TIME_CODE.MP)
                        ));
            }
            else { }
        }
    }

    //==================================================================================================================================================

    //유닛들의 기본적인 움직임에 관한 클래스
    public class Unit__Base_Movement_Engine
    {

        private UnitBaseEngine _unit_Base_Engine;
        public UnitBaseEngine _SET_unit_Base_Engine
        {
            set { _unit_Base_Engine = value; }
        }

        public Vector3 movingVector;
        public ParticleSystem movingEffect;

        protected float speed_BUF_Amount;

        //몇몇 변수 값 초기화
        void Awake()
        {
            movingVector.Set(0, 0, 0);
            speed_BUF_Amount = 0.0f;
        }

        //기본적인 이동을 위한 함수
        public void Move_OBJ(float move_Speed, ref Transform moved_OBJ, float dir)
        {
            try
            {
                //스피드 버프 값을 더하여 이동속도를 늘리거나 줄인다.
                movingVector.Set(0, 0, move_Speed * Time.deltaTime * dir);
                //moved_OBJ.GetComponent<Rigidbody>().AddForce(movingVector);
                moved_OBJ.Translate(movingVector);
                if (dir == 1)
                {
                    movingEffect.startSpeed = 16;
                    movingEffect.Play();
                }
                else
                {
                    movingEffect.startSpeed = -16;
                    movingEffect.Play();
                }

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
                rotated_OBJ.Rotate(Vector3.up, Time.deltaTime * rotate_Speed * dir);
            }
            //나중에 dir 값과 관련된 Exception 따로 넣어서 수정할 것
            catch (System.Exception)
            {
                Debug.Log("dir 값이 1 또는 -1이어야 합니다.");
            }
        }

        //스피드 버프 또는 디버프에 대한 함수 (이동속도 값을 직접적으로 제어)
        public void __GET_BUFF__About_Speed(int isBuff_OR_DeBuff, SkillBaseStat whichSkill)
        {
            try
            {
                //whichSkill의 _Skill_Rate 값을 곱한만큼으로 이동속도와 회전속도를 증감한다.
                //(적용될 이동속도) += (유닛의 원래 이동속도) * (스킬 계수)
                //(적용될 회전속도) += (유닛의 원래 회전속도) * (스킬 계수)

                _unit_Base_Engine._unit_Stat.__PUB_Move_Speed += _unit_Base_Engine._unit_Stat.__GET_FOriginalMoveSpeed * whichSkill.__GET_Skill_Rate * isBuff_OR_DeBuff;
                _unit_Base_Engine._unit_Stat.__PUB_Rotation_Speed += _unit_Base_Engine._unit_Stat.__GET_FOriginalRotateSpeed * whichSkill.__GET_Skill_Rate * isBuff_OR_DeBuff;

                //지속시간 계산
                //player인 경우
                if (_unit_Base_Engine.playerController != null)
                {
                    _unit_Base_Engine.StartCoroutine(
                        _unit_Base_Engine.playerController.__PLY_CoolTimer.Timer_Do_Once(
                            whichSkill.__GET_Skill_ING_Time,
                            (input) => { _unit_Base_Engine.coolTimeCheck = input; },
                            _unit_Base_Engine.coolTimeCheck,
                            _unit_Base_Engine.TimeCodeToINT(COOL_TIME_CODE.SP)
                            )
                        );
                }
                //enemy인 경우
                else if (_unit_Base_Engine.enemyController != null)
                {
                    _unit_Base_Engine.StartCoroutine(
                            _unit_Base_Engine.enemyController.GET_enemyAIEngine.enemyCoolTimer.Timer_Do_Once(
                                whichSkill.__GET_Skill_ING_Time,
                                (input) => { _unit_Base_Engine.coolTimeCheck = input; },
                                _unit_Base_Engine.coolTimeCheck,
                                _unit_Base_Engine.TimeCodeToINT(COOL_TIME_CODE.SP)
                                )
                            );
                }
                //오류
                else
                {

                }
            }
            //isBuff_OR_DeBuff 값이 1 또는 -1이 아니면 적용되는 Exception으로 대체 예정
            //또는 _unit_Base_Engine.playerController == null && _unit_Base_Engine.enemyController == null 인 경우
            catch (System.Exception)
            {
                //아무것도 안 한다.
                
            }
        }

        //스피드 관련 버프 OR 디버프가 끝나면 다시 초기화 한다.
        public void Init_Speed_BUF_Amount()
        {
            _unit_Base_Engine._unit_Stat.__PUB_Move_Speed = _unit_Base_Engine._unit_Stat.__GET_FOriginalMoveSpeed;
            _unit_Base_Engine._unit_Stat.__PUB_Rotation_Speed = _unit_Base_Engine._unit_Stat.__GET_FOriginalRotateSpeed;

            _unit_Base_Engine.SetCoolTimeCheck(COOL_TIME_CODE.SP);
        }
    }

    //==================================================================================================================================================
    //==================================================================================================================================================

    //유닛들의 기본적인 공격에 대한 클래스
    //이 외에도 스킬에 대한 내용도 넣어야 될 것으로 보임.
    public class Unit__Base_Combat_Engine
    {

        private UnitBaseEngine unit_Base_Engine;
        public UnitBaseEngine __SET_unit_Base_Engine
        {
            set { unit_Base_Engine = value; }
        }

        public PlayerController _playerController;
        public EnemyController _enemyController;

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
        public void Default_ATK(ref Transform attacker, Vector3 spawnPosition, Quaternion spawnRotation, SkillBaseStat whichSkill)
        {
            //"Assets/Resources/Prefabs/Bullets" 경로에서 직접 Prefab을 뽑아쓰는 쪽으로 변경

            //Resources.Load를 쓸 때는 "Assets/Resources" 경로 내부의 파일에만 접근할 수 있으며
            //Resources.Load("Assets/Resources/<임의폴더0>/<임의폴더1>/<읽으려는거>") 이렇게 쓰면 안 되고
            //Resources.Load("<임의폴더0>/<임의폴더1>/<읽으려는거>") as <자료형> 이런 식으로 써야 된다.

            //읽으려는게 "Assets/Resources" 안에 있지만 그 하위 폴더에 없는 거면
            //Resources.Load("<읽으려는거>") as <자료형> 이렇게 쓰면 된다.
            //GameObject threw_Ammo = ;
            //나중에는 SkillBaseStat에 private string bulletName 변수를 만들고 그것을 읽어오도록 할 것
            //예시)  GameObject threw_Ammo = Resources.Load(prefabBulletPath + whichSkill.bulletName) as GameObject;

            //private string bulletName 변수는 Sample__SkillDataBase.csv에서 읽어오도록 만들것

            GameObject spawned_OBJ, effect;
            spawned_OBJ = (GameObject)(MonoBehaviour.Instantiate(Resources.Load(prefabBulletPath + "SampleBullet") as GameObject, spawnPosition, spawnRotation));
            //effect
            effect = (GameObject)(MonoBehaviour.Instantiate(Resources.Load("Prefabs/Effect/Explosion") as GameObject, spawnPosition, spawnRotation));
            effect.GetComponent<Explosion>().source = unit_Base_Engine.transform;
            MonoBehaviour.Instantiate(Resources.Load("Prefabs/Effect/Explo") as GameObject, spawnPosition, spawnRotation);
            //투사체가 날아가는 속도를 특정 값으로 설정. 나중엔 DB에서 긁어올 것
            if (unit_Base_Engine.enemyController != null)
            {
                spawned_OBJ.GetComponent<AmmoBase>().__Init_Ammo(
                    unit_Base_Engine.enemyController,
                    55.0f,
                    attacker.tag,
                    unit_Base_Engine._unit_Stat.__PUB_ATK__Val,
                    unit_Base_Engine._unit_Stat.__PUB_Critical_Rate,
                    unit_Base_Engine._unit_Stat.__PUB_Critical_P,
                    whichSkill
                    );
            }
            else
            {
                spawned_OBJ.GetComponent<AmmoBase>().__Init_Ammo(
                    55.0f,
                    attacker.tag,
                    unit_Base_Engine._unit_Stat.__PUB_ATK__Val,
                    unit_Base_Engine._unit_Stat.__PUB_Critical_Rate,
                    unit_Base_Engine._unit_Stat.__PUB_Critical_P,
                    whichSkill
                    );
            }
        }


        //투사체를 발사할 때 발사 위치와 발사 방향을 따로 지정할 필요가 없는 경우
        public void Default_ATK(ref Transform attacker, SkillBaseStat whichSkill)
        {
            //GameObject threw_Ammo =;

            GameObject spawned_OBJ, effect;
            spawned_OBJ = (GameObject)(MonoBehaviour.Instantiate(Resources.Load(prefabBulletPath + "SampleBullet") as GameObject, attacker.position, attacker.rotation));
            //effect
            effect = (GameObject)(MonoBehaviour.Instantiate(Resources.Load("Prefabs/Effect/Explosion") as GameObject, attacker.position, attacker.rotation));
            effect.GetComponent<Explosion>().source = unit_Base_Engine.transform;
            MonoBehaviour.Instantiate(Resources.Load("Prefabs/Effect/Explo") as GameObject, attacker.position, attacker.rotation);
            //투사체가 날아가는 속도를 특정 값으로 설정. 나중엔 DB에서 긁어올 것
            if (unit_Base_Engine.enemyController != null)
            {
                spawned_OBJ.GetComponent<AmmoBase>().__Init_Ammo(
                    unit_Base_Engine.enemyController,
                    55.0f,
                    attacker.tag,
                    unit_Base_Engine._unit_Stat.__PUB_ATK__Val,
                    unit_Base_Engine._unit_Stat.__PUB_Critical_Rate,
                    unit_Base_Engine._unit_Stat.__PUB_Critical_P,
                    whichSkill
                    );
            }
            else
            {
                spawned_OBJ.GetComponent<AmmoBase>().__Init_Ammo(
                    55.0f,
                    attacker.tag,
                    unit_Base_Engine._unit_Stat.__PUB_ATK__Val,
                    unit_Base_Engine._unit_Stat.__PUB_Critical_Rate,
                    unit_Base_Engine._unit_Stat.__PUB_Critical_P,
                    whichSkill
                    );
            }
        }

        /** 탄환 발사
         * @param attacker 탄환을 발사하는 주체
         * @param BulletName 발사할 탄환의 이름 ("Resources/Prefabs/Bullet/" 경로상의 프리팹)
         */
        public void Default_ATK(ref Transform attacker, string BulletName)
        {
            // 탄환 생성
            GameObject OBullet = Resources.Load(prefabBulletPath + BulletName) as GameObject;
            GameObject spawned_OBJ = (GameObject)(MonoBehaviour.Instantiate(OBullet, attacker.position, attacker.rotation));

            // 탄환 발사
            spawned_OBJ.GetComponent<AmmoBase>().__Init_Ammo(
                55.0f,
                attacker.tag,
                unit_Base_Engine._unit_Stat.__PUB_ATK__Val,
                unit_Base_Engine._unit_Stat.__PUB_Critical_Rate,
                unit_Base_Engine._unit_Stat.__PUB_Critical_P
                );
        }

        //스킬 사용시 모두 이 함수를 통함.
        public void Using_Skill(ref Transform attacker, SkillBaseStat whichSkill, bool isUnitUsingThis)
        {
            string funcName = "_Skill_";

            //----------------------------------------------------------------
            //아래 코드를 이용하여 player와 enemy 구분을 확인함

            //Debug.Log(controller.ToString());

            //Debug.Log(controller.GetType().ToString());

            //스킬 통합이 완료되기 전까지 
            //if (_playerController != null)
            //{
            //    Debug.Log("Nogada_P");
            //}
            //else if (_enemyController != null)
            //{
            //    Debug.Log("Enemy");
            //}
            ////----------------------------------------------------------------

            //임시 조치. 코드 정리 도중 ID 체계화 과정에서 수정 필수
            //체력 회복(00000000), 디버프(00000001)
            if (whichSkill.__Get_Skill_ID == "NORMAL_HP_01")
            {
                funcName += "NORMAL_HP_00";
                //Debug.Log("This is HP DeBuff Skill");
            }
            //이동 속도 버프, 디버프 묶기
            //이속 버프(00000002), 디버프(00000003)
            else if (whichSkill.__Get_Skill_ID == "NORMAL_SP_01")
            {
                funcName += "NORMAL_SP_00";
                //Debug.Log("This is SP DeBuff Skill");
            }
            //임시 조치. 코드 정리 중 ID 체계화 과정에서 수정 필수
            //산탄(00000005), 속사(00000006)
            else if (whichSkill.__Get_Skill_ID == "NORMAL_ATK_01")
            {
                funcName += "NORMAL_ATK_00";
            }
            else
            {
                funcName += whichSkill.__Get_Skill_ID;
            }

            //funcName = "_Skill_" + whichSkill.__Get_Skill_ID;

            //Reflection 기법에 대해선 아래 주소들 참고
            //0. Reflection 기본 사용법 : http://www.vcskicks.com/call-function.php
            //1. private나 protected 함수에 대한 Reflection 사용법 : https://stackoverflow.com/questions/8413524/how-to-get-an-overloaded-private-protected-method-using-reflection
            System.Type type = unit_Base_Engine.GetType();

            MethodInfo method = type.GetMethod(funcName);
            //protected나 private 함수에 접근할 수 있도록 하는 조치
            BindingFlags eFlags = BindingFlags.Instance | BindingFlags.NonPublic;

            //접근을 위한 조치를 반영한다.
            method = typeof(UnitBaseEngine).GetMethod(funcName, eFlags);
            //funcName과 동일한 이름을 가진 함수를 unit_Skill_Engine에서 호출한다.
            method.Invoke(unit_Base_Engine, new object[] { attacker, whichSkill, isUnitUsingThis });
        }
    }

    //=================================================================================================================================================================================
    //이하 스킬 함수들
    //=================================================================================================================================================================================
    //Unit__Combat_Engine에 있는 스킬 함수들 이식, 스킬 함수 호출 방법을 Reflection으로 통일
    //HP쪽 스킬들처럼 한 함수가 두 가지 이상의 일을 하는 경우에는 일단 "00000000" || "00000001" 인 경우 모두 "00000000"으로 해석하도록 임시 조치를 취할 것

    //HP에 관여하는 스킬들
    private void _Skill_NORMAL_HP_00(Transform attacker, SkillBaseStat whichSkill, bool isUnitUsingThis)
    {
        int isHit_OR_Heal = 0;

        //도트 힐 OR 도트 딜
        if (whichSkill.__GET_Skill_Code_T == _SKILL_CODE_Time.FREQ)
        {
            //도트 힐 스킬을 사용할 때 OR 도트 딜 디버프를 받았을 때
            if (whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.BUF || !(isUnitUsingThis))
            {
                //이미 체력 버프나 디버프를 받는 중이 아니면
                if (    !(CoolTimeCheck(COOL_TIME_CODE.HP))  )
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

                    //StartCoroutine 때문에 PlayerController를 받아와서 이렇게 이상한 형태로 작업함. 후에 다른 방법 알아낸다면 수정 필요

                    //차선책으로 클래스 간 상속 구조를 뜯어고치고 UnitBaseEngine을 각 Object에 직접 추가하여 StartCoroutine함수를 this.StartCoroutine(...)으로 개편하고
                    //__PLY_Stat은 UnitBaseEngine에 변수를 따로 만들어서 함수 인자로 전달받지 말고 클래스 내에서 직접 가져다 쓸 것 => Enemy와 Player 간의 함수 통합 이슈 자체가 사라짐
                    StartCoroutine(
                        _unit_Stat.__Get_HIT__About_Health_FREQ(
                            whichSkill.__GET_Skill_ING_Time,
                            1.0f,
                            (int)(whichSkill.__GET_Skill_Rate),
                            isHit_OR_Heal
                            )
                        );
                }
                //이미 체력 버프나 디버프를 받고 있는 중이면
                else
                {
                    Debug.Log("체력 버프, 디버프 중첩 방지");
                }

            }
            //상대에게 도트 딜을 넣을 스킬
            else if (whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.DBF)
            {
                //디버프가 담긴 하나의 투사체를 발사한다.
                //투사체의 외형 바꾸기는 일단 넘어갈 것.

                _unit_Combat_Engine.Default_ATK(ref attacker, whichSkill);
            }
            //오류
            else
            {

            }
        }
    }

    //SP(이동속도)에 관여하는 스킬들
    private void _Skill_NORMAL_SP_00(Transform attacker, SkillBaseStat whichSkill, bool isUnitUsingThis)
    {
        int isBuFF_OR_DeBuff = 0;

        //SP(이동속도) 버프 스킬 OR 플레이어가 SP(이동속도) 디버프를 받았을 때
        if (whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.BUF || !(isUnitUsingThis))
        {

            //이미 이동속도 버프나 디버프를 받는 중이 아니면
            if (!(CoolTimeCheck(COOL_TIME_CODE.SP)))
            {
                //유닛이 사용한 거면
                if (isUnitUsingThis)
                {
                    //__GET_BUFF__About_Speed가 SP(이동속도) 버프를 수행한다.
                    isBuFF_OR_DeBuff = 1;
                }
                //유닛이 SP(이동속도) 디버프 스킬에 피격된 거면
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
                _unit_Move_Engine.__GET_BUFF__About_Speed(isBuFF_OR_DeBuff, whichSkill);
            }
            //이미 이동속도 버프나 디버프를 받고 있는 중이면
            else
            {
                //Debug.Log("이동속도 버프, 디버프 중첩 방지");
            }

        }
        //유닛이 상대에게 SP(이동속도) 디버프를 걸 때 스킬
        else if (whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.DBF)
        {
            _unit_Combat_Engine.Default_ATK(ref attacker, whichSkill);
        }
        //오류
        else
        {

        }
    }

    /** 마나에 관여하는 스킬
     * @param duringTime 회복 또는 감소 지속시간
     * @param freqTime 다음 회복 또는 감소까지의 시간
     * @param Amount 한 번 회복하거나 감소하는 마나량
     * @param isUnitUsingThis 마나 회복(true), 마나 감소(false)
     */
    private void _Skill_NORMAL_MP_00(Transform attacker, SkillBaseStat whichSkill, bool isUnitUsingThis)
    {
        int isHIT_OR_Heal;

        if (isUnitUsingThis) isHIT_OR_Heal = -1;
        else isHIT_OR_Heal = 1;

        // 마나가 주기적으로 상승/감소한다.
        // 후에 SkillBaseStat에 _Skill_FREQ_Time 변수를 새로 만들것
        StartCoroutine(
            _unit_Stat.__Get_HIT__About_Mana_FREQ(
                whichSkill.__GET_Skill_ING_Time,
                1.0f,
                (int)(whichSkill.__GET_Skill_Rate),
                isHIT_OR_Heal
                )
            );
    }

    //산탄(5번, "00000005") 스킬과 속사(6번, "00000006") 스킬을 통합한 것
    private void _Skill_NORMAL_ATK_00(Transform attacker, SkillBaseStat whichSkill, bool isUnitUsingThis)
    {
        Vector2 valueVec2 = Rotation_Math.Rotation_AND_Position(attacker.rotation, 0.58f, 0.0f);

        float posX = attacker.position.x;
        float posZ = attacker.position.z;

        //나중엔 int indexMax = whichSkill.ammoAmount;
        int indexMax = (int)(whichSkill.__GET_Skill_Rate);
        int posHelper = -1;

        //산탄 스킬이면 1, 속사스킬이면 -1
        int isShotGun_OR_FastGun = 1;

        //보정된 x, z 좌표값
        float newX;
        float newZ;

        //일단 지정된 attacker(캐릭터의 전면부)에서 3개의 기본 탄환을 서로 각 사선에 평행하도록 발사할 것.
        //탄환을 몇 개 발사할 것인지는 나중에 SkillBaseStat에서 읽어올 것
        //일단 3개니까 이렇게 작성할 것

        //본 함수의 0.58f와 1.16f, 이 두 수치에 대해서는 나중에 namespace ConstValueCollection에서 const float spawnDist 변수를 만들어서 관리할 것

        //indexMax 값 변형 -> 홀수여야만 정상작동
        indexMax = (indexMax - 1) / 2;

        //속사 스킬이면
        if (whichSkill.__Get_Skill_ID == "00000006")
        {
            //속사 스킬의 경우, 앞에서 발사하는 경우 좌표 값 보정 예시 (탄환 개수가 최대 3개일때, 보정해야 되는 지점)
            //_unit_Combat_Engine.Default_ATK(ref attacker, new Vector3(posX + valueVec2.y, attacker.position.y, posZ + valueVec2.x), attacker.rotation, whichSkill);
            //_unit_Combat_Engine.Default_ATK(ref attacker, new Vector3(posX - valueVec2.y, attacker.position.y, posZ - valueVec2.x), attacker.rotation, whichSkill);

            //값을 바꿔치기 해서 위의 보정 예시가 아래 반복문에서 적용되도록 한다.
            valueVec2.Set(valueVec2.y, valueVec2.x);

            //z 좌표 보정 수식의 부호를 바꿔준다.
            isShotGun_OR_FastGun = -1;
        }
        //산탄 스킬의 경우, 앞에서 발사하는 경우 좌표 값 보정 예시 (탄환 개수가 최대 3개일 때, 보정해야 되는 지점)
        //_unit_Combat_Engine.Default_ATK(ref attacker, new Vector3(posX + valueVec2.x, attacker.position.y, posZ - valueVec2.y), attacker.rotation, whichSkill);
        //_unit_Combat_Engine.Default_ATK(ref attacker, new Vector3(posX - valueVec2.x, attacker.position.y, posZ + valueVec2.y), attacker.rotation, whichSkill);

        //음수부터 반복문을 돌린다
        for (int index = -indexMax; index < indexMax + 1; index++)
        {

            //x, z 좌표값 보정
            newX = posX - (valueVec2.x * posHelper * index);
            //속사 (6번, "00000006")스킬인 경우
            //newZ = posZ - (valueVec.y * posHelper * index)가 된다 (isShotGun_OR_FastGun == -1)
            newZ = posZ + (valueVec2.y * posHelper * index) * isShotGun_OR_FastGun;

            _unit_Combat_Engine.Default_ATK(ref attacker, new Vector3(newX, attacker.position.y, newZ), attacker.rotation, whichSkill);


            posHelper *= (-1);
        }
    }

    //지형 소환
    //Enemy의 경우 마우스를 이용한 투사체가 아닌 물체 소환을 수행할 이유가 없으므로 신경쓰지 않는다.
    //7번 스킬 지형소환처럼 마우스로 투사체가 아닌 물체를 소환하는 스킬
    private void _Skill_NORMAL_SPW_00(Transform attacker, SkillBaseStat whichSkill, bool isUnitUsingThis)
    {
        //player인 경우
        if (playerController != null)
        {
            playerController._Set_SPW_MOS_Skill_Activated = true;
        }

        //따로 만들어둔 SampleConstructedOBJ를 소환하도록 한다.
        GameObject constructedOBJ = Resources.Load(_unit_Combat_Engine.__GET_prefabBulletPath + "SampleConstructedOBJ") as GameObject;

        //소환할 오브젝트 소환하고 값 설정
        GameObject spawned_OBJ;
        spawned_OBJ = (GameObject)(MonoBehaviour.Instantiate(constructedOBJ, playerController.transform.position, playerController.transform.rotation));

        spawned_OBJ.GetComponent<ConstructedOBJ>().__Init_ConstructedOBJ(whichSkill, playerController);

        //SpawnHailstoneSpawner 내용을 여기로 이식할 것
        //또는 별도 함수로 독립 시키고 이 함수를 간략화할 것
    }

    /** 8. 우박: 하늘에서 우박이 떨어진다.
     * @param Distance Distance 플레이어와 HailStoneSpawner 사이의 거리
     * @param Height HailStoneSpawner의 높이 
     */
    private void _Skill_NORMAL_SPW_01(Transform attacker, SkillBaseStat whichSkill, bool isUnitUsingThis)
    {
        //distance랑 height 값도 나중엔 whichSkill에서 긁어오거나 PMS_Math에서 긁어올 것
        float distance = 10.0f;
        float height = 10.0f;

        bool isPlayerUsing;

        //플레이어면 플레이어가 사용했다는 정보를 전달
        if (playerController != null) isPlayerUsing = true;
        //플레이어가 아니면 Enemy가 사용했다는 정보를 전달
        else isPlayerUsing = false;

     // 스포너 프리팹 가져오기
     GameObject spawnerPrefab = (GameObject)Resources.Load("Prefabs/SpawnArea/Hailstone Spawner");

        Vector3 spawnPosition = transform.position + transform.forward * distance + new Vector3(0, height, 0);

        GameObject spawned_Area;
        spawned_Area = (GameObject)(Instantiate(spawnerPrefab, spawnPosition, transform.rotation));

        spawned_Area.GetComponent<HailStoneSpawner>().__Init_HailStoneSpawner(whichSkill, _unit_Stat.__PUB_ATK__Val, isPlayerUsing);
    }

    /** 기에 관여하는 스킬
     * @param duringTime 회복 또는 감소 지속시간
     * @param freqTime 다음 회복 또는 감소까지의 시간
     * @param Amount 한 번 회복하거나 감소하는 기의 양
     * @param isUnitUsingThis 기 회복(true), 기 감소(false)
     */
    private void _Skill_NORMAL_PP_00(Transform attacker, SkillBaseStat whichSkill, bool isUnitUsingThis)
    {
        int isHIT_OR_Heal;

        if (isUnitUsingThis) isHIT_OR_Heal = -1;
        else isHIT_OR_Heal = 1;

        //기가 주기적으로 상승/감소한다.
        // 후에 SkillBaseStat에 _Skill_FREQ_Time 변수를 새로 만들것
        StartCoroutine(
                _unit_Stat.__Get_HIT__About_Power_FREQ(
                    whichSkill.__GET_Skill_ING_Time,
                    1.0f,
                    (int)(whichSkill.__GET_Skill_Rate),
                    isHIT_OR_Heal)
                );

    }

    //궁극기 - 폭풍우 (광범위 이속 디버프)
    private void _Skill_FIN000(Transform attacker, SkillBaseStat whichSkill, bool isUnitUsingThis)
    {
        bool isPlayerUsingThis;

        //플레이어면
        if (playerController != null) { isPlayerUsingThis = true; }
        //플레이어가 아니면
        else { isPlayerUsingThis = false; }

        //프리팹을 소환하고 필요한 정보들을 프리팹에 보낸다.
        GameObject ultimateSkillPrefab = (GameObject)(Resources.Load("Prefabs/SpawnArea/UltimateSkill_Storm"));

        GameObject spawnedUltimateSkill;
        spawnedUltimateSkill = Instantiate(ultimateSkillPrefab, transform.position, new Quaternion()) as GameObject;
        spawnedUltimateSkill.GetComponent<UltimateSkillBase>().Initialize_U_Skill(whichSkill, isPlayerUsingThis);
    }

    //public void UltimateSkill()
    //{
    //    GameObject UltimateSkillPrefab = (GameObject)Resources.Load("Prefabs/UltimateSkill");
    //    Instantiate(UltimateSkillPrefab, transform.position, new Quaternion());
    //}
}
