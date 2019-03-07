using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace PMS_AISystem
{
    //딥러닝을 적용하기 위한 현재 상황에 관한 정보들
    public class Situation
    {
        //적의 위치와 바라보는 방향
        private Vector3 enemyPosition;
        private Quaternion enemyRotation;

        //플레이어의 위치와 바라보는 방향
        private Vector3 playerPosition;
        private Quaternion playerRotation;

        //적과 플레이어의 위치, 방향 차
        private Vector3 enemy_TO_playerPosition;
        //rotation 대신 각도 값으로 넣을 수도 있음
        //private float enemy_TO_playerAngle;
        private Quaternion enemy_TO_playerRotation;

        //적의 현재 HP, MP, PP
        private int enemyCurHP;
        private int enemyCurMP;
        private int enemyCurPP;

        //플레이어의 현재 HP, MP, PP
        private int playerCurHP;
        private int playerCurMP;
        private int playerCurPP;

        //이전 행동에서 사용한 함수 이름
        private string beforeFuncName = "";
        //이번 행동에서 사용할 함수
        private string curFuncName = "";

        //얼마나 적합한 행동인 지를 나타내는 변수
        private int curPoint;
        //curPoint 계산 방식
        private int pointVer;

        //위 변수들에 대한 Getter
        public Vector3 _GET_enemyPosition { get { return enemyPosition; } }
        public Quaternion _GET_enemyRotation { get { return enemyRotation; } }

        public Vector3 _GET_playerPosition { get { return playerPosition; } }
        public Quaternion _GET_playerRotation { get { return playerRotation; } }

        public Vector3 _GET_enemy_TO_playerPosition { get { return enemy_TO_playerPosition; } }
        //public float _GET_enemy_TO_playerAngle { get{ return enemy_TO_playerAngle; } }
        public Quaternion _GET_enemy_TO_playerRotation { get { return enemy_TO_playerRotation; } }

        public int _GET_enemyCurHP { get { return enemyCurHP; } }
        public int _GET_enemyCurMP { get { return enemyCurMP; } }
        public int _GET_enemyCurPP { get { return enemyCurPP; } }

        public int _GET_playerCurHP { get { return playerCurHP; } }
        public int _GET_playerCurMP { get { return playerCurMP; } }
        public int _GET_playerCurPP { get { return playerCurPP; } }

        public string _GET_beforeFuncName { get { return beforeFuncName; } }
        public string _GET_curFuncName { get { return curFuncName; } }

        public int _GET_pointVer { get { return pointVer; } }
        public int _GET_curPoint { get { return curPoint; } }


        //값 설정하는 함수
        public void _SET_Situation(Transform enemy, Transform player, Vector3 enemyCur, Vector3 playerCur, string beforeFunc, string curFunc, int pointVersion)
        {
            enemyPosition = enemy.position;
            enemyRotation = enemy.rotation;

            playerPosition = player.position;
            playerRotation = player.rotation;

            enemy_TO_playerPosition = enemyPosition - playerPosition;
            //enemy_TO_playerAngle = enemyAngle - playerAngle;
            //아래 방식은 오류 뜸
            //enemy_TO_playerRotation = enemyRotation - playerRotation;

            enemyCurHP = (int)enemyCur.x;
            enemyCurMP = (int)enemyCur.y;
            enemyCurPP = (int)enemyCur.z;

            playerCurHP = (int)playerCur.x;
            playerCurMP = (int)playerCur.y;
            playerCurPP = (int)playerCur.z;

            beforeFuncName = beforeFunc;
            curFuncName = curFunc;

            pointVer = pointVersion;

            if (pointVer == 0)
            {
                curPoint = enemyCurHP - playerCurHP;
            }
            else if (pointVer == 1)
            {
                curPoint = enemyCurHP - playerCurHP - playerCurMP;
            }
            else
            {

            }
        }

        //임의의 두 상황이 비슷한 지 검사하는 함수
        public List<bool> IsSimilarSituation(Situation situation_A, Situation situation_B)
        {
            List<bool> result = new List<bool>();

            //각 상황에서 적과 플레이어 간의 위치 값 차이가 크게 나지 않는 경우
            //3.0f 보다 값이 커지거나 작아질 수 있음
            if (Vector3.Distance(situation_A._GET_enemy_TO_playerPosition, situation_B._GET_enemy_TO_playerPosition) <= 3.0f)
            {
                result.Add(true);
            }
            else
            {
                result.Add(false);
            }

            //각도 차이가 크게 나지 않는 경우
            
            //적과 플레이어의 HP, MP, PP 차이가 크게 나지 않는 경우

            //이전에 호출한 함수가 일치하는 경우

            //이번에 호출하는 함수가 일치하는 경우

            return result; 
        }

        //위 함수의 반환 값이 {true, true, true, false, false}와 2개 이하 일치하고 point_Ver가 같다면 situation_A와 situation_B의 point를 비교하여 낮은 쪽은 폐기하고 높은 쪽만 DB에 남긴다.
    }
}
