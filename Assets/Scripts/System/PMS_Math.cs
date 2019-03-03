using UnityEngine;
using System.Collections;

//자주 사용되는 수학적 값 보정을 위한 namespace
namespace PMS_Math {

    //UnitStat이나 SkilBaseStat 등에 의한 값 변화와는 관계없이 항상 일정한 값들을 저장해둔 클래스
    public class ConstValueCollections {

    }

    //Quaternion과 관련된 수학적 값 보정을 위한 클래스
    public class Rotation_Math {

        //특정 Object의 rotation 값을 받아서 dist가 반지름인 원 위의 특정 지점의 x, z값을 반환하는 함수
        public static Vector2 Rotation_AND_Position (Quaternion rotation, float dist, float angle_FOR_Correction){

            float angle = (rotation.eulerAngles.y + angle_FOR_Correction) * Mathf.Deg2Rad;

            Vector2 result = new Vector2(Mathf.Cos(angle) * dist, Mathf.Sin(angle) * dist);
            //Debug.Log("result: "+ result);

            return result;
        }

        //반대 방향을 찾아주는 함수.
        //-180 < angle <= 180의 범위일 때만 유효하다.
        public static float Get_Opposite_Direction_Angle(float angle)
        {
            if (angle >= 0)
                angle -= 180;
            else
                angle += 180;

            return angle;
        }

        //0 <= angle < 360 에서 -180 <= angle < 180으로 바꿔주는 함수
        public static float Angle360_TO_Angle180 (float angle)
        {
            if (angle > 180)
            {
                //curAngle도 -180 < curAngle <= 180으로 변경한다.
                angle -= 360;
            }

            return angle;
        }
    }
    
}
