﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
//1. 필요로 하는 클래스가 적용된 오브젝트에 함께 올려 놓아야 됨.
//2. 코루틴 인자가 System.Action<bool>이기 때문에 아래와 같은 방법으로 인자를 넣어주어야 함.
// Timer( my_Time, (input) => { my_Bool = input; } )
//3. yield retuen new WaitForSeconds(1f);를 반복문에 넣고 돌리는 방식으로 개량해야 됨.
//4. 함수 인자에 Action<float> time을 추가하고 SkillBaseStat으로 가서 public float time을 만들고 SkillBaseStat.time을 Action<float> time에 연결할 것.
public class UnitCoolTimer : MonoBehaviour {

    //1초보다 작은 단위로 셀 수 있긴 함
    //아직 미적용
    public IEnumerator Timer(float timeVal, Action<float> remainedTime)
    {
        float time = timeVal;

        while (time > 0)
        {
            time -= Time.deltaTime;

            if (time < 0) time = 0;
            remainedTime(time);

            //반복문 한 번 돌 때마다 한 번씩 쉰다(다음 프레임까지 대기)
            yield return null;
        }

        //해당 코루틴 자동 종료
        yield break;
    }

    public IEnumerator Timer(float timeVal, Action<bool> timeLocker, bool beforeVal, Action<float> real_remained_Time)
    {
        float remained_Time;

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

        
        for (remained_Time = timeVal; remained_Time >= 0; remained_Time--)
        {
            real_remained_Time(remained_Time);    

            yield return new WaitForSeconds(1.0f);
        }
        
        //시간 계산
        //yield return new WaitForSeconds(timeVal);

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

    public IEnumerator Timer_Do_Once(float timeVal, Action<int> timeLocker, int lockVal, int index)
    {
        //시간 계산
        yield return new WaitForSeconds(timeVal);

        //시간이 다 지나면
        //Timer가 시작될 때 바꿀 값이 1(true)이었다면
        if ((lockVal & index) == index)
        {
            //해당하는 값을 0으로 바꿔준다.
            timeLocker(lockVal & (~index));
        }
        //Timer가 시작될 때 바꿀 값이 0(false)이었다면
        else
        {
            //해당하는 값을 1로 바꿔준다.
            timeLocker(lockVal | index);
        }

        //코루틴 종료
        yield break;
    }

    public IEnumerator Timer(float timeVal, Action<int> timeLocker, int lockVal, int index, Action<float> outputRemainedTime)
    {
        float remained_Time;

        if ((lockVal & index) == index)
        {
            timeLocker(lockVal & (~index));
        }
        else
        {
            timeLocker(lockVal + index);
        }

        for (remained_Time = timeVal; remained_Time >= 0; remained_Time--)
        {
            outputRemainedTime(remained_Time);

            yield return new WaitForSeconds(1.0f);
        }

        if ((lockVal & index) == index)
        {
            timeLocker(lockVal & (~index));
        }
        else
        {
            timeLocker(lockVal + index);
        }

        //해당 코루틴 자동 종료
        yield break;
    }
}
