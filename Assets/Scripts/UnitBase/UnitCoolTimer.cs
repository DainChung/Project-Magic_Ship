using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
//1. 필요로 하는 클래스가 적용된 오브젝트에 함께 올려 놓아야 됨.
//2. 코루틴 인자가 System.Action<bool>이기 때문에 아래와 같은 방법으로 인자를 넣어주어야 함.
// Timer( my_Time, (input) => { my_Bool = input; } )
//3. 
public class UnitCoolTimer : MonoBehaviour {

    public IEnumerator Timer(float timeVal, Action<bool> timeLocker, bool beforeVal)
    {
        //이 코루틴이 시작되기 전의 bool 값에 따라 결과를 다르게 하도록 출력한다.
        if (beforeVal)
        {
            //locker 값을 false로 설정
            timeLocker(false);
        }
        else
        {
            //locker 값을 true로 설정
            timeLocker(true);
        }

        //시간 계산
        yield return new WaitForSeconds(timeVal);

        //시간이 다 지나면
        if (beforeVal)
        {
            //locker 값을 false로 설정
            timeLocker(true);
        }
        else
        {
            //locker 값을 true로 설정
            timeLocker(false);
        }

        //해당 코루틴 자동 종료
        yield break;
    }

    public IEnumerator Timer_Do_Once(float timeVal, Action<bool> timeLocker, bool beforeVal)
    {
        //시간 계산
        yield return new WaitForSeconds(timeVal);

        //시간이 다 지나면
        if (beforeVal)
        {
            //locker 값을 false로 설정
            timeLocker(false);
        }
        else
        {
            //locker 값을 true로 설정
            timeLocker(true);
        }

        //해당 코루틴 자동 종료
        yield break;
    }
}
