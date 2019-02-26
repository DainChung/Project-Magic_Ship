using UnityEngine;
using System.Collections;

public class ConstructedOBJ : MonoBehaviour {

    //설치 후 일정 시간 뒤에 자동 소멸 하기 위해 필요한 UnitCoolTimer
    private UnitCoolTimer timer;

    //마우스로 설치까지 완료되기 전 == false; -> timer 작동 안 함
    //마우스로 설치까지 완료된 후 == true; -> timer 작동 시작
    private bool isConstructed = false;

    //자동 소멸 시간을 채웠는지 아닌지 판별하기 위한 부울 변수
    private bool isTimeOut = false;

    //마우스 위치를 ScreenToViewportPoint함수로 변환했을 때의 위치
    private Vector3 mouseViewPortVector;
    //설치될 위치
    private Vector3 nowVector;
    //설치 가능 여부
    private bool canItConstruct = false;

    //어떤 스킬인지 알기 위한 변수
    private SkillBaseStat whichSkill;
    //플레이어에게 설치가 끝났다고 알려주기 위한 용도
    private PlayerController player;

	// Use this for initialization
	void Start () {
        timer = transform.GetComponent<UnitCoolTimer>();

        //일단 SphereCollider로 한다.
        transform.GetComponent<BoxCollider>().isTrigger = true;
	}
	
	// Update is called once per frame
	void Update () {

        //마우스로 설치하는 동안 작동
        if (!(isConstructed))
        {
            //좌클릭 중이 아닐 때 설치 가능
            if (!(Input.GetMouseButton(0)))
            {
                RaycastHit hit;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                //지형이나 적, 플레이어 위치에는 설치 불가
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Water")) && isMouseOnGameScreen(hit))
                {
                    canItConstruct = true;

                    nowVector.Set(hit.point.x, 1.5f, hit.point.z);
                }
                else
                {
                    canItConstruct = false;
                }

                //의도된 위치에 둘 수 있는 지 확인용
                //Debug.Log("nowVector: " + nowVector);

                transform.position = nowVector;
            }

            //설치가능한 상태에서 좌클릭하면 바로 설치
            if (canItConstruct && Input.GetMouseButton(0))
            {
                //설치가 완료되었다고 player에게 정보 전달
                player._Set_SPW_MOS_Skill_Activated = false;
                //설치를 완료하고 timer를 작동시킨다.
                isConstructed = true;
                
                transform.GetComponent<BoxCollider>().isTrigger = false;
            }
        }
        //마우스로 설치가 완료되고 나서 작동(수정 가능성 있음)
        else
        {
            if (isTimeOut)
            {

            }
        }
	}

    private bool isMouseOnGameScreen(RaycastHit _hit)
    {
        bool result;
        Vector3 hitScreenPoint = Camera.main.WorldToScreenPoint(_hit.point);

        if (hitScreenPoint.x <= Screen.width && hitScreenPoint.z <= Screen.height)
        {
            result = true;
        }
        else
        {
            result = false;
        }

        return result;
    }

    //스킬 사용 버튼을 누르자마자 필요한 정보 받아올 것
    public void __Init_ConstructedOBJ(SkillBaseStat _whichSkill, PlayerController _player)
    {
        whichSkill = _whichSkill;
        player = _player;
    }
}
