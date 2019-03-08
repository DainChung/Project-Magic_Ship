using UnityEngine;
using System.Collections;

public class BulletManager : MonoBehaviour {
    public void ModifySpeedForSeconds(Collider Target, float Modifier, float Duration)
    {
        StartCoroutine(ModifySpeedForSeconds_Corutine(Target, Modifier, Duration));
    }

    /** 역풍탄 효과 수행 함수
     * @param Target 이동속도 보정할 대상의 Collider
     * @param Modifier 이동속도에 곱해지는 인수
     * @param Duration 이동속도 변화 지속시간
     *
     * @description 효과를 적용할
     */    
    public IEnumerator ModifySpeedForSeconds_Corutine(Collider Target, float Modifier, float Duration)
    {
        Target.GetComponent<UnitBaseEngine>()._unit_Stat.__PUB_Move_Speed *= Modifier;
        yield return new WaitForSeconds(Duration);
        Target.GetComponent<UnitBaseEngine>()._unit_Stat.__PUB_Move_Speed /= Modifier;
        yield break;
    }
}