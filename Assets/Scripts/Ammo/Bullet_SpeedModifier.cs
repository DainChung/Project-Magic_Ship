using UnityEngine;
using System.Collections;

public class Bullet_SpeedModifier : AmmoBase {
    private BulletManager SBulletManager;

    /** 충돌 시에 변화할 이동속도의 곱셈 인자 */
    public float FMovementSpeedModifier;

    /** 충돌 시에 이동속도가 변화가 유지되는 지속시간 */
    public float FMovementSpeedModifyDuration;

    protected Bullet_SpeedModifier()
    {
        FMovementSpeedModifier = 0.1f;
    }

    protected void Start()
    {
        base.Start();

        SBulletManager = GameObject.Find("GameManager").GetComponent<BulletManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 적 or 플레이어에 충돌시, 일정 시간동안 이동속도 감소
        if(__Who_Shot == "Player" && other.transform.tag == "SampleEnemy" || __Who_Shot == "SampleEnemy" && other.transform.tag == "Player")
            SBulletManager.ModifySpeedForSeconds(other, FMovementSpeedModifier, FMovementSpeedModifyDuration);

        /** 부모 함수 호출
         * @execute 벽에 충돌시, 탄환 오브젝트 삭제
         * @execute 적 or 플레이어에 충돌시, 피격 연산 후 오브젝트 삭제
         */
        base.OnTriggerEnter(other);
    }
}