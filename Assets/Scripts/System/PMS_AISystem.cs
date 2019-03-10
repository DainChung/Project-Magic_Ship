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
        //0: HP, 1: MP, 2: PP
        private List<int> enemyCur = new List<int>();

        //플레이어의 현재 HP, MP, PP
        //0: HP, 1: MP, 2: PP
        private List<int> playerCur = new List<int>();

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

        public List<int> _GET_enemyCur { get { return enemyCur; } }

        public List<int> _GET_playerCur { get { return playerCur; } }

        public string _GET_beforeFuncName { get { return beforeFuncName; } }
        public string _GET_curFuncName { get { return curFuncName; } }

        public int _GET_pointVer { get { return pointVer; } }
        public int _GET_curPoint { get { return curPoint; } }


        //값 설정하는 함수
        public void _SET_Situation(Transform enemy, Transform player, Vector3 enemyCurrent, Vector3 playerCurrent, string beforeFunc, string curFunc, int pointVersion)
        {
            enemyPosition = enemy.position;
            enemyRotation = enemy.rotation;

            playerPosition = player.position;
            playerRotation = player.rotation;

            enemy_TO_playerPosition = enemyPosition - playerPosition;
            //enemy_TO_playerAngle = enemyAngle - playerAngle;
            //아래 방식은 오류 뜸
            //enemy_TO_playerRotation = enemyRotation - playerRotation;

            enemyCur.Add((int)enemyCurrent.x);
            enemyCur.Add((int)enemyCurrent.y);
            enemyCur.Add((int)enemyCurrent.z);

            playerCur.Add((int)playerCurrent.x);
            playerCur.Add((int)playerCurrent.y);
            playerCur.Add((int)playerCurrent.z);

            beforeFuncName = beforeFunc;
            curFuncName = curFunc;

            pointVer = pointVersion;

            //어떤 상태를 적합한 행동으로 규정할 것인지에 따라 curPoint 수식이 달라진다.
            if (pointVer == 0)
            {
                //enemy의 HP가 높고 player의 HP가 작을 수록 적합한 행동
                curPoint = enemyCur[0] - playerCur[0];
            }
            else if (pointVer == 1)
            {
                //enemy의 HP가 높고 player의 HP, MP가 작을 수록 적합한 행동
                curPoint = enemyCur[0] - playerCur[0] - playerCur[1];
            }
            else
            {

            }
        }

        //임의의 두 상황이 비슷한 지 검사하는 함수
        public List<bool> IsSimilarSituation(Situation situation_A, Situation situation_B)
        {
            List<bool> result = new List<bool>();
            //HP, MP, PP가 얼마나 비슷한지 검토하기 위한 변수
            int IsUnitCurSimilar = 0;

            //각 상황에서 적과 플레이어 간의 위치 값 차이가 크게 나지 않는 경우
            //3.0f 보다 값이 커지거나 작아질 수 있음
            if (Vector3.Distance(situation_A._GET_enemy_TO_playerPosition, situation_B._GET_enemy_TO_playerPosition) <= 3.0f) {     result.Add(true); }
            //차이가 큰 경우
            else {      result.Add(false); }

            //각도 차이가 크게 나지 않는 경우


            //적과 플레이어의 HP, MP, PP 차이가 크게 나지 않는 경우
            //반복문은 3번 작동한다.
            for(int index = 0; index < situation_A.enemyCur.Count; index++)
            {
                //임의의 두 상황에서 적의 HP, MP, PP 차이가 3이하인 경우 (기준 값은 얼마든지 변할 수 있음)
                //비교해야되는 두 변수가 ( 현재HP / 최대HP )처럼 비율로 변경될 가능성이 크다. player 쪽도 마찬가지
                if (situation_A.enemyCur[index] - situation_B.enemyCur[index] <= 3 && situation_A.enemyCur[index] - situation_B.enemyCur[index] >= -3)
                {
                    //얼마나 두 상황이 비슷한지 체크한다.
                    IsUnitCurSimilar++;
                }

                //임의의 두 상황에서 플레이어의 HP, MP, PP 차이가 3이하인 경우 (기준 값은 얼마든지 변할 수 있음)
                if (situation_A.playerCur[index] - situation_B.playerCur[index] <= 3 && situation_A.playerCur[index] - situation_B.playerCur[index] >= -3)
                {
                    //얼마나 두 상황이 비슷한지 체크한다.
                    IsUnitCurSimilar++;
                }
                //반복문이 끝날 때 IsUnitSimilar는 다음의 조건을 만족한다.
                //0 <= IsUnitSimilar <= 6
            }

            //4개 항목이 유사하면 유사한 상황으로 판정한다.
            //이 기준값 역시 변할 수 있음.
            if (IsUnitCurSimilar >= 4) {    result.Add(true); }
            else { result.Add(false); }


            //이전에 호출한 함수가 일치하는 경우
            if (situation_A._GET_beforeFuncName == situation_B._GET_beforeFuncName) { result.Add(true); }
            //일치하지 않는 경우
            else {  result.Add(false); }

            //이번에 호출하는 함수가 일치하는 경우
            if (situation_A._GET_curFuncName == situation_B.curFuncName) { result.Add(true); }
            //일치하지 않는 경우
            else { result.Add(false); }

            return result; 
        }

        //위 함수의 반환 값이 {true, true, true, false, false}와 3개 이상 일치하고 point_Ver가 같다면 situation_A와 situation_B의 point를 비교하여 낮은 쪽은 폐기하고 높은 쪽만 DB에 남긴다.
        // {true, true, true, true, false} 등 기준에 대한 논의는 추후에 진행할 것
    }
}
