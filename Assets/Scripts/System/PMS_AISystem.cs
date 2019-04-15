using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace PMS_AISystem
{
    //딥러닝을 적용하기 위한 현재 상황에 관한 정보들
    public class Situation {
        //적과 플레이어의 위치, 방향 차
        private Vector3 enemy_TO_playerPosition;
        //rotation 대신 각도 값으로 진행한다.
        private float enemy_TO_playerAngle;

        //---------------------------------------------------
        ////보류
        ////적의 현재 HP, MP, PP
        ////0: HP, 1: MP, 2: PP
        //private List<int> enemyCur = new List<int>();

        ////플레이어의 현재 HP, MP, PP
        ////0: HP, 1: MP, 2: PP
        //private List<int> playerCur = new List<int>();
        //---------------------------------------------------

        private string dataID = "";

        //이전 행동의 ID
        private string beforeDataID = "";

        //이번 행동에서 사용할 함수 인덱스들
        //curFuncName.x : 이동에 관한 함수 인덱스
        //curFuncName.y : 회전에 관한 함수 인덱스
        //curFuncName.z : 공격 OR 스킬에 관한 함수 인덱스
        private Vector3 curFuncName = new Vector3();

        //플레이어가 피격받은 행동인지 알려주는 변수 (적합한 행동이었는 지)
        bool isPlayerGetHit;

        //행동 시작 시점인지 알려주는 변수
        bool isDoSomethingStart;

        //위 변수들에 대한 Getter
        public Vector3 _GET_enemy_TO_playerPosition { get { return enemy_TO_playerPosition; } }
        public float _GET_enemy_TO_playerAngle { get{ return enemy_TO_playerAngle; } }

        //public List<int> _GET_enemyCur { get { return enemyCur; } }

        //public List<int> _GET_playerCur { get { return playerCur; } }

        public string _GET_beforeDataID { get { return beforeDataID; } }
        public Vector3 _GET_curFuncName { get { return curFuncName; } }

        //값 설정하는 함수
        public void _SET_Situation(string _dataID, Transform _enemy, Vector3 _playerPos, string _beforeID, Vector3 _curFunc, bool _isPlayerGetHit, bool _isDoSomethingStart)//, int pointVersion, Vector3 enemyCurrent, Vector3 playerCurrent)
        {
            dataID = _dataID;

            enemy_TO_playerPosition = _enemy.position - _playerPos;

            enemy_TO_playerAngle = _enemy.GetComponent<EnemyController>()._GET__ENE_AI_Engine.destiAngle;

            beforeDataID = _beforeID;
            curFuncName = _curFunc;

            isPlayerGetHit = _isPlayerGetHit;
            isDoSomethingStart = _isDoSomethingStart;
        }

        //임의의 두 상황이 비슷한 지 검사하는 함수
        public List<bool> IsSimilarSituation(Situation situation_A, Situation situation_B)
        {
            List<bool> result = new List<bool>();

            //각 상황에서 적과 플레이어 간의 위치 값 차이가 크게 나지 않는 경우
            //3.0f 보다 값이 커지거나 작아질 수 있음
            if (Vector3.Distance(situation_A._GET_enemy_TO_playerPosition, situation_B._GET_enemy_TO_playerPosition) <= 3.0f) { result.Add(true); }
            //차이가 큰 경우
            else { result.Add(false); }

            //각도 차이가 크게 나지 않는 경우
            if (Mathf.Abs(situation_A._GET_enemy_TO_playerAngle - situation_B._GET_enemy_TO_playerAngle) <= 3.0f) { result.Add(true); }
            else { result.Add(false); }

            //이전에 호출한 함수가 일치하는 경우
            if (situation_A.beforeDataID == situation_B.beforeDataID) { result.Add(true); }
            //일치하지 않는 경우
            else { result.Add(false); }

            //이번에 호출하는 함수가 일치하는 경우
            if (situation_A._GET_curFuncName == situation_B.curFuncName) { result.Add(true); }
            //일치하지 않는 경우
            else { result.Add(false); }

            return result; 
        }

        //위 함수의 반환 값을 이용하여 {true, true, , }처럼 앞의 두 개가 true이면 임의의 두 상황이 유사했다고 판단한다.
        //위 함수를 이용하여 임의의 두 상황이 유사하지만 {true, true, , false}처럼 맨 뒤의 값이 false이면 두 상황은 유사했지만 다른 행동을 한 것이다.
    }
}
