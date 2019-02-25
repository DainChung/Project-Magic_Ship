using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

    public float smooth;

    private GameObject playerObject;

    private float cameraSpeed;

    private float xDist;
    private float yDist;
    private float zDist;

	// Use this for initialization
	void Start () {
        //나중에 변경할 것
        playerObject = GameObject.Find("SamplePlayer");

        cameraSpeed = playerObject.GetComponent<PlayerController>().__PLY_Stat.__PUB_Move_Speed;

        xDist = transform.position.x - playerObject.transform.position.x;
        yDist = transform.position.y - playerObject.transform.position.y;
        zDist = transform.position.z - playerObject.transform.position.z;
    }
	
	// Update is called once per frame
	void Update () {
        //사용자가 지정한 플레이어 캐릭터와 카메라의 거리가 유지되도록 카메라의 목표 지점을 설정.
        //Lerp함수를 이용해 카메라가 캐릭터를 따라가는 것이 자연스럽게 느껴지도록 한다.
        Vector3 cameraPosition = new Vector3(playerObject.transform.position.x + xDist, playerObject.transform.position.y + yDist, playerObject.transform.position.z + zDist);
        transform.position = Vector3.Lerp(transform.position, cameraPosition, Time.deltaTime * cameraSpeed * smooth);

        //이 부분은 사용자가 카메라를 회전시키는 것을 감안해서 변경 사항이 필요할 것으로 보임.
        //플레이어 캐릭터와 실제 카메라의 위치 차를 계산, 실제 카메라의 방향을 바꾸어 카메라가 플레이어를 바라보도록 한다.
        //어쩌면 xDist, yDist, zDist 값이 바뀌는 것만으로도 수정할 것이 없을 지도 모른다.
        Quaternion destiCameraRotation = Quaternion.LookRotation(playerObject.transform.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, destiCameraRotation, Time.deltaTime * smooth);
    }
}
