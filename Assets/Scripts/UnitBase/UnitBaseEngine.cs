using UnityEngine;
using System.Collections;

using System.Reflection;

using SkillBaseCode;

using PMS_Math;

public class UnitBaseEngine : MonoBehaviour {

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

    //-----------------------------------------

    void Awake()
    {
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

    //==================================================================================================================================================

    //유닛들의 기본적인 움직임에 관한 클래스
    public class Unit__Base_Movement_Engine {

        private UnitBaseEngine _unit_Base_Engine;
        public UnitBaseEngine _SET_unit_Base_Engine
        {
            set { _unit_Base_Engine = value; }
        }

        public Vector3 movingVector;

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
                movingVector.Set(0, 0, (move_Speed + speed_BUF_Amount) * Time.deltaTime * dir);
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
        public void __GET_BUFF__About_Speed(int isBuff_OR_DeBuff, SkillBaseStat whichSkill)
        {
            try
            {
                speed_BUF_Amount += (whichSkill.__GET_Skill_Rate * isBuff_OR_DeBuff);

                //지속시간 계산
                //player인 경우
                if (_unit_Base_Engine.playerController != null)
                {
                    _unit_Base_Engine.playerController.StartCoroutine(
                        _unit_Base_Engine.playerController.__PLY_CoolTimer.Timer_Do_Once(whichSkill.__GET_Skill_ING_Time,
                        (input) => { _unit_Base_Engine._unit_Stat.__PUB_Stat_Locker[0] = input; },
                        _unit_Base_Engine._unit_Stat.__PUB_Stat_Locker[0])
                        );
                }
                //enemy인 경우
                else if (_unit_Base_Engine.enemyController != null)
                {
                    _unit_Base_Engine.enemyController.StartCoroutine(
                        _unit_Base_Engine.enemyController._GET__ENE_AI_Engine.enemyCoolTimer.Timer_Do_Once(whichSkill.__GET_Skill_ING_Time,
                            (input) => { _unit_Base_Engine._unit_Stat.__PUB_Stat_Locker[0] = input; },
                            _unit_Base_Engine._unit_Stat.__PUB_Stat_Locker[0])
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
            speed_BUF_Amount = 0;
        }
    }

    //==================================================================================================================================================
    //==================================================================================================================================================

    //유닛들의 기본적인 공격에 대한 클래스
    //이 외에도 스킬에 대한 내용도 넣어야 될 것으로 보임.
    public class Unit__Base_Combat_Engine {

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
            GameObject threw_Ammo = Resources.Load(prefabBulletPath + "SampleBullet") as GameObject;
            //나중에는 SkillBaseStat에 private string bulletName 변수를 만들고 그것을 읽어오도록 할 것
            //예시)  GameObject threw_Ammo = Resources.Load(prefabBulletPath + whichSkill.bulletName) as GameObject;

            //private string bulletName 변수는 Sample__SkillDataBase.csv에서 읽어오도록 만들것

            GameObject spawned_OBJ;
            spawned_OBJ = (GameObject)(MonoBehaviour.Instantiate(threw_Ammo, spawnPosition, spawnRotation));
            //투사체가 날아가는 속도를 특정 값으로 설정. 나중엔 DB에서 긁어올 것
            spawned_OBJ.GetComponent<AmmoBase>().__Init_Ammo(
                55.0f,
                attacker.tag,
                unit_Base_Engine._unit_Stat.__PUB_ATK__Val,
                unit_Base_Engine._unit_Stat.__PUB_Critical_Rate,
                unit_Base_Engine._unit_Stat.__PUB_Critical_P,
                whichSkill
                );
        }


        //투사체를 발사할 때 발사 위치와 발사 방향을 따로 지정할 필요가 없는 경우
        public void Default_ATK(ref Transform attacker, SkillBaseStat whichSkill)
        {
            GameObject threw_Ammo = Resources.Load(prefabBulletPath + "SampleBullet") as GameObject;

            GameObject spawned_OBJ;
            spawned_OBJ = (GameObject)(MonoBehaviour.Instantiate(threw_Ammo, attacker.position, attacker.rotation));
            //투사체가 날아가는 속도를 특정 값으로 설정. 나중엔 DB에서 긁어올 것
            spawned_OBJ.GetComponent<AmmoBase>().__Init_Ammo(
                55.0f,
                attacker.tag,
                unit_Base_Engine._unit_Stat.__PUB_ATK__Val,
                unit_Base_Engine._unit_Stat.__PUB_Critical_Rate,
                unit_Base_Engine._unit_Stat.__PUB_Critical_P,
                whichSkill
                );
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
            if (whichSkill.__Get_Skill_ID == "00000001")
            {
                funcName += "00000000";
                Debug.Log("This is HP DeBuff Skill");
            }
            else if (whichSkill.__Get_Skill_ID == "00000004")
            {
                funcName += "00000003";
                Debug.Log("This is MP Heal Skill");
            }
            //임시 조치. 코드 정리 중 ID 체계화 과정에서 수정 필수
            //산탄(00000005), 속사(00000006)
            else if (whichSkill.__Get_Skill_ID == "00000006")
            {
                funcName += "00000005";
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
    private void _Skill_00000000(Transform attacker, SkillBaseStat whichSkill, bool isUnitUsingThis)
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

                //차선책으로 클래스 간 상속 구조를 뜯어고치고 UnitBaseEngine을 각 Object에 직접 추가하여 StartCoroutine함수를 this.StartCoroutine(...)으로 개편하고
                //__PLY_Stat은 UnitBaseEngine에 변수를 따로 만들어서 함수 인자로 전달받지 말고 클래스 내에서 직접 가져다 쓸 것 => Enemy와 Player 간의 함수 통합 이슈 자체가 사라짐
                StartCoroutine(
                    _unit_Stat.__Get_HIT__About_Health_FREQ(whichSkill.__GET_Skill_ING_Time,
                    1.0f,
                    (int)(whichSkill.__GET_Skill_Rate),
                    isHit_OR_Heal)
                    );
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
    private void _Skill_00000002(Transform attacker, SkillBaseStat whichSkill, bool isUnitUsingThis)
    {
        int isBuFF_OR_DeBuff = 0;

        //SP(이동속도) 버프 스킬 OR 플레이어가 SP(이동속도) 디버프를 받았을 때
        if (whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.BUF || !(isUnitUsingThis))
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
        //유닛이 상대에게 SP(이동속도) 디버프를 걸 때 스킬
        else if (whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.DBF)
        {

        }
        //오류
        else
        {

        }
    }

    /** 마나에 관여하는 스킬
     * @param duringTime 회복 지속시간
     * @param freqTime 다음 회복까지의 시간
     * @param Amount 한 번 회복할 때의 회복양
     * @param isUnitUsingThis Mana increases(true), Mana decreases(false)
     */
    public void _Skill_00000004(float duringTime = 4.0f, float freqTime = 1.0f, int Amount = 2, bool isUnitUsingThis = true)
    {
        int IsHeal;

        ////도트 힐 OR 도트 딜
        //if (whichSkill.__GET_Skill_Code_T == _SKILL_CODE_Time.FREQ &&
        //    whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.BUF || !(isUnitUsingThis))
        {
                if (isUnitUsingThis) IsHeal = 1;
                else IsHeal = -1;

                // 마나가 주기적으로 상승/감소한다.
                StartCoroutine(_unit_Stat.HealManaRepeat(duringTime, freqTime, Amount, IsHeal)
                    );
        }
    }

    //산탄(5번, "00000005") 스킬과 속사(6번, "00000006") 스킬을 통합한 것
    private void _Skill_00000005(Transform attacker, SkillBaseStat whichSkill, bool isUnitUsingThis)
    {
        Vector2 valueVec2 = Rotation_Math.Rotation_AND_Position(attacker.rotation, 0.58f, 0.0f);

        float posX = attacker.position.x;
        float posZ = attacker.position.z;

        //나중엔 int indexMax = whichSkill.ammoAmount;
        int indexMax = 3;
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
    private void _Skill_00000007(Transform attacker, SkillBaseStat whichSkill, bool isUnitUsingThis)
    {
        //player인 경우
        if (playerController != null)
        {
            playerController._Set_SPW_MOS_Skill_Activated = true;
        }

        //Enemy의 경우 마우스를 이용한 투사체가 아닌 물체 소환을 수행할 이유가 없으므로 신경쓰지 않는다.
        //7번 스킬 지형소환처럼 마우스로 투사체가 아닌 물체를 소환하는 스킬
        if (whichSkill.__GET_Skill_Code_S == _SKILL_CODE_Sub.MOS)
        {
            //따로 만들어둔 SampleConstructedOBJ를 소환하도록 한다.
            GameObject constructedOBJ = Resources.Load(_unit_Combat_Engine.__GET_prefabBulletPath + "SampleConstructedOBJ") as GameObject;

            //소환할 오브젝트 소환하고 값 설정
            GameObject spawned_OBJ;
            spawned_OBJ = (GameObject)(MonoBehaviour.Instantiate(constructedOBJ, playerController.transform.position, playerController.transform.rotation));

            spawned_OBJ.GetComponent<ConstructedOBJ>().__Init_ConstructedOBJ(whichSkill, playerController);
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

    /** 8. 우박: 하늘에서 우박이 떨어진다.
     * @param Distance Distance between player and hailstone spawner
     * @param Height Height of spawn location 
     */
    public void SpawnHailstoneSpawner(float Distance = 10.0f, float Height = 10.0f)
    {
        // 스포너 프리팹 가져오기
        GameObject SpawnerPrefab = (GameObject) Resources.Load("Prefabs/SpawnArea/Hailstone Spawner");

        Vector3 SpawnPosition = transform.position + transform.forward * Distance + new Vector3(0, Height, 0);
        Instantiate(SpawnerPrefab, SpawnPosition, GetComponent<Transform>().rotation);
    }

    /** 기에 관여하는 스킬
     * @param duringTime 회복 지속시간
     * @param freqTime 다음 회복까지의 시간
     * @param Amount 한 번 회복할 때의 회복양
     * @param isUnitUsingThis Mana increases(true), Mana decreases(false)
     */
    public void _Skill_00000009(float duringTime = 4.0f, float freqTime = 1.0f, int Amount = 2, bool isUnitUsingThis = true)
    {
        int IsHeal;

        ////도트 힐 OR 도트 딜
        //if (whichSkill.__GET_Skill_Code_T == _SKILL_CODE_Time.FREQ &&
        //    whichSkill.__GET_Skill_Code_M == _SKILL_CODE_Main.BUF || !(isUnitUsingThis))
        {
            if (isUnitUsingThis) IsHeal = 1;
            else IsHeal = -1;

            // 마나가 주기적으로 상승/감소한다.
            StartCoroutine(_unit_Stat.HealPowerRepeat(duringTime, freqTime, Amount, IsHeal)
                );
        }
    }
}
