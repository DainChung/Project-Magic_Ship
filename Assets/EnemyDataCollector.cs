using UnityEngine;
using System;
using System.Collections;

using System.Collections.Generic;

using PMS_AISystem;
using File_IO;

//한 스크립트에서 저장 총괄하기
public class EnemyDataCollector : MonoBehaviour {

    //20190606 임시
    [HideInInspector]
    public List<int> listScore_FOR_PPT = new List<int>();

    public List<SituationCUR> listSitCUR = new List<SituationCUR>();
    public List<SituationCUR> listSitCUR0 = new List<SituationCUR>();
    public List<SituationCUR> listSitCUR1 = new List<SituationCUR>();

    public List<SituationAFT> listSitAFT = new List<SituationAFT>();
    public List<SituationAFT> listSitAFT0 = new List<SituationAFT>();
    public List<SituationAFT> listSitAFT1 = new List<SituationAFT>();

    public List<AIData> aiDatas = new List<AIData>();

    public List<AIData> data_FOR_Learn = new List<AIData>();

    public List<SituationCUR> goodBehaveList = new List<SituationCUR>();
    public List<SituationCUR> goodBehaveListLOW = new List<SituationCUR>();

    public int mod = 0;
    public int sampleFileString = 0;

    private int coroutineLocker = 0;

    public float sigmaValue = 0.0f;

    void swap(int a, int b)
    {
        SituationCUR t = goodBehaveList[a];
        goodBehaveList[a] = goodBehaveList[b];
        goodBehaveList[b] = t;
    }

    void swapLOW(int a, int b)
    {
        SituationCUR t = goodBehaveListLOW[a];
        goodBehaveListLOW[a] = goodBehaveListLOW[b];
        goodBehaveListLOW[b] = t;
    }

    void quickSort(int l, int r)
    {
        int p = l;
        int j = p;
        int i = l + 1;

        if (l < r)
        {
            for (; i <= r; i++)
            {
                if (goodBehaveList[i]._angleComp < goodBehaveList[p]._angleComp)
                {
                    j++;
                    swap(j, i);
                }
            }
            swap(l, j);
            p = j;

            quickSort(l, p - 1);
            quickSort(p + 1, r);
        }
    }

    void quickSortLOW(int l, int r)
    {
        int p = l;
        int j = p;
        int i = l + 1;

        if (l < r)
        {
            for (; i <= r; i++)
            {
                if (goodBehaveListLOW[i]._angleComp < goodBehaveListLOW[p]._angleComp)
                {
                    j++;
                    swapLOW(j, i);
                }
            }
            swapLOW(l, j);
            p = j;

            quickSortLOW(l, p - 1);
            quickSortLOW(p + 1, r);
        }
    }

    void quickSort_AIData(int l, int r)
    {
        int p = l;
        int j = p;
        int i = l + 1;

        if (l < r)
        {
            for (; i <= r; i++)
            {
                if (aiDatas[i].sitCUR._angleComp < aiDatas[p].sitCUR._angleComp)
                {
                    j++;

                    AIData temp = aiDatas[j];
                    aiDatas[j] = aiDatas[i];
                    aiDatas[i] = temp;
                }
            }
            AIData temp2 = aiDatas[l];
            aiDatas[l] = aiDatas[j];
            aiDatas[j] = temp2;
            p = j;

            quickSort_AIData(l, p - 1);
            quickSort_AIData(p + 1, r);
        }
    }

    void quickSort_AIData_DIST(int l, int r)
    {
        int p = l;
        int j = p;
        int i = l + 1;

        if (l < r)
        {
            for (; i <= r; i++)
            {
                if (Mathf.Abs(data_FOR_Learn[i].sitCUR._dist - data_FOR_Learn[i].sitAFT._dist) < Mathf.Abs(data_FOR_Learn[p].sitCUR._dist - data_FOR_Learn[p].sitAFT._dist))
                {
                    j++;

                    AIData temp = data_FOR_Learn[j];
                    data_FOR_Learn[j] = data_FOR_Learn[i];
                    data_FOR_Learn[i] = temp;
                }
            }
            AIData temp2 = data_FOR_Learn[l];
            data_FOR_Learn[l] = data_FOR_Learn[j];
            data_FOR_Learn[j] = temp2;
            p = j;

            quickSort_AIData_DIST(l, p - 1);
            quickSort_AIData_DIST(p + 1, r);
        }
    }

    void search_AIData(float angle, int i, int length, int j, ref int resultIndex)
    {
        int index = i;

        if (j == 1)
            index = (int)(length / 2);
        else
        {
            if (index < 0)
            {
                index *= (-1);
                index -= (int)(length / (j * 2));
            }
            else
            {
                index += (int)(length / (j * 2));
            }
        }

        if (angle <= aiDatas[index].sitCUR._angleComp && aiDatas[index].sitCUR._angleComp < angle + 10)
        {
            if (aiDatas[index].sitCUR._angleComp > angle)
            {
                j *= 2;
                search_AIData(angle, -index, length, j, ref resultIndex);
            }
            else if (aiDatas[index].sitCUR._angleComp < angle)
            {
                j *= 2;
                search_AIData(angle, index, length, j, ref resultIndex);
            }
        }
        else
        {
            resultIndex = index;
        }
    }

    void search(float dist, float angle, int i, int length, int j, ref int result)
    {
        int index = i;

        if (j == 1)
            index = (int)(length / 2);
        else
        {
            if (index < 0)
            {
                index *= (-1);
                index -= (int)(length / (j * 2));
            }
            else
            {
                index += (int)(length / (j * 2));
            }
        }

        if ((int)(length / j) > 16)
        {
            if (goodBehaveList[index]._angleComp > angle)
            {
                j *= 2;
                search(dist, angle, -index, length, j, ref result);
            }
            else if (goodBehaveList[index]._angleComp < angle)
            {
                j *= 2;
                search(dist, angle, index, length, j, ref result);
            }
            else
            {
                result = index;
            }
        }
        else
        {
            float _dist = 999f;
            int min = index - 15;
            int max = index + 15;

            if (max > length - 1) max = length - 1;
            if (min < 0) min = 0;

            for (int q = min; q < max; q++)
            {
                float comp = Mathf.Abs(goodBehaveList[q]._dist - dist);

                if (comp < _dist)
                {
                    _dist = comp;
                    result = q;
                }
            }
        }
    }

    void searchLOW(float dist, float angle, int i, int length, int j, ref int result)
    {
        int index = i;

        if (j == 1)
            index = (int)(length / 2);
        else
        {
            if (index < 0)
            {
                index *= (-1);
                index -= (int)(length / (j * 2));
            }
            else
            {
                index += (int)(length / (j * 2));
            }
        }

        if ((int)(length / j) > 16)
        {
            if (goodBehaveListLOW[index]._angleComp > angle)
            {
                j *= 2;
                searchLOW(dist, angle, -index, length, j, ref result);
            }
            else if (goodBehaveListLOW[index]._angleComp < angle)
            {
                j *= 2;
                searchLOW(dist, angle, index, length, j, ref result);
            }
            else
            {
                result = index;
            }
        }
        else
        {
            float _dist = 999f;
            int min = index - 15;
            int max = index + 15;

            if (max > length - 1) max = length - 1;
            if (min < 0) min = 0;

            for (int q = min; q < max; q++)
            {
                float comp = Mathf.Abs(goodBehaveListLOW[q]._dist - dist);

                if (comp < _dist)
                {
                    _dist = comp;
                    result = q;
                }
            }
        }
    }

    void search(float angle, int i, int length, int j, ref int resultIndex, string id)
    {
        int index = i;

        if (j == 1)
            index = (int)(length / 2);
        else
        {
            if (index < 0)
            {
                index *= (-1);
                index -= (int)(length / (j * 2));
            }
            else
            {
                index += (int)(length / (j * 2));
            }
        }

        if (goodBehaveList[index]._angleComp > angle)
        {
            j *= 2;
            search(angle, -index, length, j, ref resultIndex, id);
        }
        else if (goodBehaveList[index]._angleComp < angle)
        {
            j *= 2;
            search(angle, index, length, j, ref resultIndex, id);
        }
        else
        {
            if (goodBehaveList[index]._angleComp == angle && goodBehaveList[index]._id != id)
                resultIndex = index;
            else
                resultIndex = -1;
        }
    }

    public IntVector3 SearchGoodDoing(float dist, float angle)
    {
        IntVector3 result = new IntVector3(-1, -1, -1);
        int resultIndex = -1;

        search(dist, angle, goodBehaveList.Count, goodBehaveList.Count, 1, ref resultIndex);
        result = goodBehaveList[resultIndex]._doing;

        //Debug.Log("CurD: "+ dist + ". CurA:"+ angle + ", DBD: "+ goodBehaveList[resultIndex]._dist + ", DBA: "+ goodBehaveList[resultIndex]._angleComp);

        return result;
    }

    public class TrackSameData {
        public string id;
        public int count;

        public TrackSameData(string ID, int COUNT)
        {
            id = ID;
            count = COUNT;
        }
    }

    List<TrackSameData> SearchSameData()
    {
        List<TrackSameData> temp = new List<TrackSameData>();
        bool there = false;

        for (int i = 0; i < goodBehaveList.Count - 1; i++)
        {
            if (goodBehaveList[i + 1]._id == goodBehaveList[i]._id)
            {
                goodBehaveList.RemoveAt(i);
                
                if (temp.Count != 0)
                {
                    for (int j = 0; j < temp.Count; j++)
                    {
                        if (temp[j].id == goodBehaveList[i]._id)
                        {
                            temp[j].count++;
                            there = true;
                            break;
                        }
                    }

                    if (!there)
                    {
                        temp.Add(new TrackSameData(goodBehaveList[i]._id, 1));
                    }
                    else
                    {
                        there = false;
                    }
                }
                else
                {
                    temp.Add(new TrackSameData(goodBehaveList[i]._id, 1));
                }

                i--;
            }
        }


        return temp;
    }

    public SituationCUR SearchGoodSitCUR(float dist, float angle, bool isHPLOW)
    {
        SituationCUR result = new SituationCUR("NULL", -1f, -1, -1, -1, new IntVector3(-1, -1, -1), -1f);
        int resultIndex = -1;

        if (!isHPLOW)
        {
            search(dist, angle, goodBehaveList.Count, goodBehaveList.Count, 1, ref resultIndex);
            result = goodBehaveList[resultIndex];
        }
        else
        {
            searchLOW(dist, angle, goodBehaveListLOW.Count, goodBehaveListLOW.Count, 1, ref resultIndex);
            result = goodBehaveListLOW[resultIndex];
        }

        //Debug.Log("CurD: "+ dist + ". CurA:"+ angle + ", DBD: "+ goodBehaveList[resultIndex]._dist + ", DBA: "+ goodBehaveList[resultIndex]._angleComp);

        return result;
    }

    void Awake()
    {
        //데이터 정리용
        //모든 행동 DB는 정리가 되지 않음 => 학습 시킬 때 우수행동 DB에 없는 건 모두 안 좋은 행동으로 취급해야 됨. 
        //학습 단계시 유의할 것
        //aiDatas = IO_SqlDB.ReadAIData_FROM_DB_FOR_KILL_JUNK("behaveDataGreatATK");

        //for (int i = 0; i < aiDatas.Count; i++)
        //{
        //    listSitCUR.Add(aiDatas[i].sitCUR);
        //    listSitAFT.Add(aiDatas[i].sitAFT);
        //}

        //IO_SqlDB.WriteDB_CUR("behaveDataGoodATK_TEMP", listSitCUR);
        //IO_SqlDB.WriteDB_AFT("behaveDataGoodATK_TEMP", listSitAFT);

        if (mod == 0)
        {
            goodBehaveList = IO_SqlDB.ReadSitCUR_FROM_DB("behaveDataGreatATK");
            quickSort(0, goodBehaveList.Count - 1);
        }
        else if (mod == 1)
        {
            if (sampleFileString == 2)
            {
                //List<AIData> tempAIData = IO_SqlDB.ReadAIData_FROM_DB("behaveDataScored");
                goodBehaveList = IO_SqlDB.ReadSitCUR_FROM_DB("behaveDataScored");

                //for (int i = 0; i < goodBehaveList.Count; i++)
                //{
                //    goodBehaveList[i]._posX = tempAIData[i].sitAFT._beforeDB;
                //    goodBehaveList[i]._posZ = tempAIData[i].sitAFT._hitCounter;
                //}
            }
            else if (sampleFileString == 1)
            {
                goodBehaveList = IO_SqlDB.ReadSitCUR_FROM_DB("behaveDataScored_2_3");
            }
            else
            {
                goodBehaveList = IO_SqlDB.ReadSitCUR_FROM_DB("behaveDataScored_2_0");
            }
            quickSort(0, goodBehaveList.Count - 1);

            //goodBehaveListLOW = IO_SqlDB.ReadSitCUR_FROM_DB("behaveDataScoredLOW");
            //quickSortLOW(0, goodBehaveListLOW.Count - 1);
        }
        //else if (mod == 2)
        //{

        //    aiDatas = IO_SqlDB.ReadAIData_FROM_DB("behaveDataGoodATK");

        //    for (int i = 0; i < aiDatas.Count; i++)
        //    {
        //        if (aiDatas[i].sitCUR._doing.vecZ == 2 && aiDatas[i].sitAFT._angleComp < 0 && aiDatas[i].sitCUR._angleComp < 0)
        //        {
        //            Debug.Log("2MM");
        //        }
        //        else if (aiDatas[i].sitCUR._doing.vecZ == 3 && aiDatas[i].sitAFT._angleComp > 0 && aiDatas[i].sitCUR._angleComp > 0)
        //        {
        //            Debug.Log("3PP");
        //        }
        //        else if (aiDatas[i].sitCUR._doing.vecZ != 0 && (aiDatas[i].sitCUR._angleComp > 170f || aiDatas[i].sitCUR._angleComp < -170f) && (aiDatas[i].sitAFT._angleComp > 170f || aiDatas[i].sitAFT._angleComp < -170f))
        //        {
        //            Debug.Log("Angle");
        //        }
        //        else
        //        { }
        //    }
        //}
        //else if (mod == 3)
        //{
        //    GameObject[] enemys = GameObject.FindGameObjectsWithTag("SampleEnemy");

        //    if (enemys.Length > 0)
        //    {
        //        for (int i = 0; i < enemys.Length; i++)
        //        {
        //            Destroy(enemys[i]);
        //        }
        //    }

        //    goodBehaveList = IO_SqlDB.ReadSitCUR_FROM_DB("behaveDataGreatATK");
        //    aiDatas = IO_SqlDB.ReadAIData_FROM_DB("behaveDataGreatATK");

        //    quickSort(0, goodBehaveList.Count - 1);

        //    for (int i = 0; i < aiDatas.Count; i++)
        //    {
        //        if (aiDatas[i].sitAFT._id == "NULL")
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            listSitAFT.Add(aiDatas[i].sitAFT);
        //        }
        //    }

        //    List<TrackSameData> sameDatas = SearchSameData();

        //    if (sameDatas.Count == 0)
        //    {
        //        Debug.Log("DB is Clean");
        //    }
        //    else
        //    {
        //        Debug.Log("Cleaning DB Start");

        //        for (int i = 0; i < sameDatas.Count; i++)
        //        {
        //            listSitAFT0.Add(new SituationAFT(sameDatas[i].id, -1f, -1f, -1f, 400f, "NULL", 0, (sameDatas[i].count + 1), false));
        //        }

        //        //중복되는 AFT 내용 발견시 병합
        //        for (int i = 0; i < listSitAFT.Count; i++)
        //        {
        //            for (int j = 0; j < listSitAFT0.Count; j++)
        //            {
        //                if (listSitAFT[i]._id == listSitAFT0[j]._id)
        //                {
        //                    listSitAFT[i]._hitCounter += (listSitAFT0[j]._hitCounter);
        //                    listSitAFT0.RemoveAt(j);
        //                    j--;
        //                }
        //            }
        //        }

        //        //미중복 사례와 중복 사례 통합
        //        for (int i = 0; i < listSitAFT0.Count; i++)
        //        {
        //            listSitAFT.Add(listSitAFT0[i]);
        //        }


        //        IO_SqlDB.WriteDB_CUR("behaveDataGreatATK_TEMP", goodBehaveList);
        //        IO_SqlDB.WriteDB_AFT_FOR_GreatTEMP("behaveDataGreatATK_TEMP", listSitAFT);

        //        Debug.Log("Cleaning DB Complete");
        //    }
        //}
        //else if (mod == 4)
        //{
        //    goodBehaveList = IO_SqlDB.ReadSitCUR_FROM_DB("behaveDataGreatATK");
        //    aiDatas = IO_SqlDB.ReadAIData_FROM_DB("behaveDataGreatATK");

        //    quickSort(0, goodBehaveList.Count - 1);

        //    for (int i = 0; i < aiDatas.Count; i++)
        //    {
        //        if (aiDatas[i].sitAFT._id == "NULL")
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            listSitAFT.Add(aiDatas[i].sitAFT);
        //        }
        //    }

        //    aiDatas.RemoveRange(0, aiDatas.Count);

        //    listSitAFT0 = IO_SqlDB.ReadSitAFT_FROM_DB("behaveDataGoodATK");

        //    Debug.Log("Reading Done");

        //    for (int i = 0; i < listSitAFT0.Count; i++)
        //    {
        //        for (int j = 0; j < goodBehaveList.Count; j++)
        //        {
        //            if (goodBehaveList[j]._id == listSitAFT0[i]._id)
        //            {
        //                listSitAFT1.Add(listSitAFT0[i]);
        //                break;
        //            }
        //        }
        //    }

        //    Debug.Log("Searching Done");

        //    StartCoroutine(SaveAFT());
        //}
        //학습 모드 - 행동 학습 준비 - 각도별 분류
        else if (mod == 5)
        {
            
        }
        //학습 모드 - 강화학습
        //체력이 낮은 경우에 대해서도 학습할 것
        //
        else if (mod == 6)
        {
            StartCoroutine(AllDataLearning());
        }
        //분류가 되지 않은 AIData들을 각도별로 분류하는 모드
        else if (mod == 7)
        {
            //aiDatas = IO_SqlDB.ReadAIData_FORM_DB("______Sample", -180);

            //for (int i = 0; i < aiDatas.Count; i++)
            //{
            //    Debug.Log(i + ": " + aiDatas[i].sitCUR._dist + ", " + aiDatas[i].sitCUR._angleComp + ", " + aiDatas[i].sitAFT._dist);
            //}

            //aiDatas.RemoveRange(0, aiDatas.Count);

            //aiDatas = IO_SqlDB.ReadAIData_FORM_DB("______Sample", -179);

            //for (int i = 0; i < aiDatas.Count; i++)
            //{
            //    Debug.Log(i + ": " + aiDatas[i].sitCUR._dist + ", " + aiDatas[i].sitCUR._angleComp + ", " + aiDatas[i].sitAFT._dist);
            //}
            //aiDatas.RemoveRange(0, aiDatas.Count);

            StartCoroutine(Div_BY_Angle_HugeBehaveData());
        }
        else
        { }
    }

    IEnumerator Div_BY_Angle_HugeBehaveData()
    {
        int startRowID, endRowID;

        //20만개씩 끊어서 수행한다.
        for (int i = 20; i < 40; i++)
        {
            startRowID = i * 200000 + 1;
            endRowID = (i + 1) * 200000;

            Debug.Log(startRowID + " ~ " + endRowID);

            aiDatas = IO_SqlDB.ReadAIData_FROM_DB("behaveData", startRowID, endRowID);
            yield return null;
            Debug.Log("Read Done");
            SaveDatas_Div_BY_Angle(true);
            aiDatas.RemoveRange(0, aiDatas.Count);

            yield return null;
        }

        Debug.Log("All Working Done");
        yield break;
    }

    void SaveDatas_Div_BY_Angle(bool isDB_behaveData)
    {
        //각도별 분류 (10도를 단위로 나눔, 각 리스트 당 평균 5500개로 추정됨)
        //양수
        List<AIData> pAngle170 = new List<AIData>();
        List<AIData> pAngle160 = new List<AIData>();
        List<AIData> pAngle150 = new List<AIData>();
        List<AIData> pAngle140 = new List<AIData>();
        List<AIData> pAngle130 = new List<AIData>();
        List<AIData> pAngle120 = new List<AIData>();
        List<AIData> pAngle110 = new List<AIData>();
        List<AIData> pAngle100 = new List<AIData>();

        List<AIData> pAngle90 = new List<AIData>();
        List<AIData> pAngle80 = new List<AIData>();
        List<AIData> pAngle70 = new List<AIData>();
        List<AIData> pAngle60 = new List<AIData>();
        List<AIData> pAngle50 = new List<AIData>();
        List<AIData> pAngle40 = new List<AIData>();
        List<AIData> pAngle30 = new List<AIData>();
        List<AIData> pAngle20 = new List<AIData>();
        List<AIData> pAngle10 = new List<AIData>();
        List<AIData> pAngle0 = new List<AIData>();

        //음수
        List<AIData> mAngle10 = new List<AIData>();
        List<AIData> mAngle20 = new List<AIData>();
        List<AIData> mAngle30 = new List<AIData>();
        List<AIData> mAngle40 = new List<AIData>();
        List<AIData> mAngle50 = new List<AIData>();
        List<AIData> mAngle60 = new List<AIData>();
        List<AIData> mAngle70 = new List<AIData>();
        List<AIData> mAngle80 = new List<AIData>();
        List<AIData> mAngle90 = new List<AIData>();

        List<AIData> mAngle100 = new List<AIData>();
        List<AIData> mAngle110 = new List<AIData>();
        List<AIData> mAngle120 = new List<AIData>();
        List<AIData> mAngle130 = new List<AIData>();
        List<AIData> mAngle140 = new List<AIData>();
        List<AIData> mAngle150 = new List<AIData>();
        List<AIData> mAngle160 = new List<AIData>();
        List<AIData> mAngle170 = new List<AIData>();
        List<AIData> mAngle180 = new List<AIData>();

        //aiDatas[i].sitCUR._angleComp 오름차순 정렬 (-180부터 180까지)
        quickSort_AIData(0, aiDatas.Count - 1);

        float angle = -180f;

        for (int i = 0; i < aiDatas.Count; i++)
        {
            if(isDB_behaveData)
                aiDatas[i].sitAFT._beforeDB = aiDatas[i].sitAFT._hitCounter;

            if (aiDatas[i].sitCUR._id == "NULL")
                continue;

            if (angle <= aiDatas[i].sitCUR._angleComp && aiDatas[i].sitCUR._angleComp < angle + 10)
            {
                if (angle == -180f) { mAngle180.Add(aiDatas[i]); }
                else if (angle == -170f) { mAngle170.Add(aiDatas[i]); }
                else if (angle == -160f) { mAngle160.Add(aiDatas[i]); }
                else if (angle == -150f) { mAngle150.Add(aiDatas[i]); }
                else if (angle == -140f) { mAngle140.Add(aiDatas[i]); }
                else if (angle == -130f) { mAngle130.Add(aiDatas[i]); }
                else if (angle == -120f) { mAngle120.Add(aiDatas[i]); }
                else if (angle == -110f) { mAngle110.Add(aiDatas[i]); }
                else if (angle == -100f) { mAngle100.Add(aiDatas[i]); }
                else if (angle == -90f) { mAngle90.Add(aiDatas[i]); }
                else if (angle == -80f) { mAngle80.Add(aiDatas[i]); }
                else if (angle == -70f) { mAngle70.Add(aiDatas[i]); }
                else if (angle == -60f) { mAngle60.Add(aiDatas[i]); }
                else if (angle == -50f) { mAngle50.Add(aiDatas[i]); }
                else if (angle == -40f) { mAngle40.Add(aiDatas[i]); }
                else if (angle == -30f) { mAngle30.Add(aiDatas[i]); }
                else if (angle == -20f) { mAngle20.Add(aiDatas[i]); }
                else if (angle == -10f) { mAngle10.Add(aiDatas[i]); }
                else if (angle == 0f) { pAngle0.Add(aiDatas[i]); }
                else if (angle == 10f) { pAngle10.Add(aiDatas[i]); }
                else if (angle == 20f) { pAngle20.Add(aiDatas[i]); }
                else if (angle == 30f) { pAngle30.Add(aiDatas[i]); }
                else if (angle == 40f) { pAngle40.Add(aiDatas[i]); }
                else if (angle == 50f) { pAngle50.Add(aiDatas[i]); }
                else if (angle == 60f) { pAngle60.Add(aiDatas[i]); }
                else if (angle == 70f) { pAngle70.Add(aiDatas[i]); }
                else if (angle == 80f) { pAngle80.Add(aiDatas[i]); }
                else if (angle == 90f) { pAngle90.Add(aiDatas[i]); }
                else if (angle == 100f) { pAngle100.Add(aiDatas[i]); }
                else if (angle == 110f) { pAngle110.Add(aiDatas[i]); }
                else if (angle == 120f) { pAngle120.Add(aiDatas[i]); }
                else if (angle == 130f) { pAngle130.Add(aiDatas[i]); }
                else if (angle == 140f) { pAngle140.Add(aiDatas[i]); }
                else if (angle == 150f) { pAngle150.Add(aiDatas[i]); }
                else if (angle == 160f) { pAngle160.Add(aiDatas[i]); }
                else if (angle == 170f) { pAngle170.Add(aiDatas[i]); }
                else if (angle >= 180f)
                {
                    break;
                }
            }
            else
            {
                angle += 10f;
                i--;
            }
        }

        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleM18", mAngle180);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleM17", mAngle170);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleM16", mAngle160);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleM15", mAngle150);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleM14", mAngle140);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleM13", mAngle130);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleM12", mAngle120);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleM11", mAngle110);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleM10", mAngle100);

        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleM9", mAngle90);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleM8", mAngle80);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleM7", mAngle70);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleM6", mAngle60);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleM5", mAngle50);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleM4", mAngle40);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleM3", mAngle30);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleM2", mAngle20);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleM1", mAngle10);

        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleP0", pAngle0);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleP1", pAngle10);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleP2", pAngle20);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleP3", pAngle30);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleP4", pAngle40);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleP5", pAngle50);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleP6", pAngle60);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleP7", pAngle70);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleP8", pAngle80);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleP9", pAngle90);

        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleP10", pAngle100);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleP11", pAngle110);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleP12", pAngle120);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleP13", pAngle130);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleP14", pAngle140);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleP15", pAngle150);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleP16", pAngle160);
        IO_SqlDB.WriteDB_AIDatas("behaveDataAngleP17", pAngle170);

        Debug.Log("Writing Done");
    }

    IEnumerator AllDataLearning()
    {
        bool saveLocker = true;

        for (int angle = -18; angle < 18; angle++)
        {
            for (int angleSUB = 0; angleSUB <= 9; angleSUB++)
            {
                for (float dist = 0f; dist <= 50f; dist += 2.0f)
                {
                    yield return null;
                    AIData data = new AIData();

                    StartCoroutine(LearnHighScoreBehave(angle, angleSUB, dist, false, (input) => { data = input; }));

                    data_FOR_Learn.Add(data);
                }
                aiDatas.RemoveRange(0, aiDatas.Count);
            }
        }

        while (saveLocker)
        {
            if (coroutineLocker >= 102)
            {
                StartCoroutine(Save_DataFORLearn());
                saveLocker = false;
            }

            yield return null;
        }
        yield break;
    }

    IEnumerator Save_DataFORLearn()
    {
        List<AIData> saveData = new List<AIData>();

        for (int i = 0; i < data_FOR_Learn.Count; i++)
        {
            if (data_FOR_Learn[i].sitCUR._doing.vecX != -1)
            {
                saveData.Add(data_FOR_Learn[i]);
            }
        }

        Debug.Log(saveData.Count);
        //IO_SqlDB.WriteDB_AIDatas("behaveDataScored", data_FOR_Learn);
        IO_SqlDB.WriteDB_AIDatas("behaveDataScored", saveData);

        Debug.Log("Save Done");

        yield break;
    }

    //AIData 
    IEnumerator LearnHighScoreBehave(int angleNum, int angleSUB, float distNum, bool isLOW_HP_Behave, Action<AIData> result)
    {
        string fileName = "behaveDataAngle";
        int angle = angleNum * 10 + angleSUB;

        if (isLOW_HP_Behave)
            fileName = fileName + "LOW";

        if (angleNum < 0)
        {
            int newAngleNum = angleNum * (-1);

            fileName = fileName + "M" + newAngleNum.ToString();
        }
        else
        {
            fileName = fileName + "P" + angleNum.ToString();
        }

        if (aiDatas.Count == 0)
        {
            Debug.Log("New Read " + angle);
            aiDatas = IO_SqlDB.ReadAIData_FROM_DB(fileName, angle);
            quickSort_AIData(0, aiDatas.Count - 1);
        }

        SituationCUR curInit = new SituationCUR("NULL", 0f, 0f, 0f, 0f, new IntVector3(-1,-1,-1), 0f);
        SituationAFT aftInit = new SituationAFT("NULL", 0f, 0f, 0f, 0f, "NULL", 0, 0, false);

        AIData[,,] dataCollections = new AIData[3,7,4];

        //초기화
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    dataCollections[i, j, k] = new AIData(curInit, aftInit);
                }
            }
        }

        //거리, 각도에 따른 행동별 상태값 총합
        for (int i = 0; i < aiDatas.Count; i++)
        {
            float angleData = aiDatas[i].sitCUR._angleComp;
            float distData = aiDatas[i].sitCUR._dist;

            if (angleData >= angle + 1)
                continue;

            //불량 데이터 무시
            if (aiDatas[i].sitCUR._doing.vecX == -1)
                continue;

            if (distData >= 50.5f)
                distData = 50f;

            if ((angle <= angleData && angleData < angle + 1f) && (distNum <= distData && distData < distNum + 0.5f))
            {
                //체력 절반 이하, 절반 초과에 따라 다른 행동점수 가산점 OR 불이익 정책 사용
                //체력이 절반을 초과할 때
                if (!isLOW_HP_Behave)
                {
                    aiDatas[i].sitAFT._beforeDB += aiDatas[i].sitAFT._hitCounter;

                    //if (aiDatas[i].sitCUR._doing.vecX != 0 && aiDatas[i].sitCUR._doing.vecY != 0)
                    //    aiDatas[i].sitAFT._beforeDB += aiDatas[i].sitAFT._hitCounter; //(aiDatas[i].sitAFT._hitCounter * 11);

                    if (distNum >= 30f && aiDatas[i].sitCUR._doing.vecX != 0 && aiDatas[i].sitCUR._doing.vecY != 0
                        && aiDatas[i].sitAFT._closer == true && aiDatas[i].sitAFT._dist + 10f < distNum)
                    {
                        aiDatas[i].sitAFT._beforeDB += 3;// 10;
                    }

                    if (distNum >= 30f && aiDatas[i].sitCUR._doing.vecX == 0 && aiDatas[i].sitCUR._doing.vecY == 0
                        && aiDatas[i].sitAFT._closer == false && aiDatas[i].sitAFT._dist >= distNum)
                    {
                        aiDatas[i].sitAFT._beforeDB -= (int)(20 + aiDatas[i].sitAFT._dist);//3;
                    }

                    //if (distNum < 30f && aiDatas[i].sitAFT._hitCounter > 0)
                    //{
                    //    aiDatas[i].sitAFT._beforeDB += 40;
                    //}

                    if (aiDatas[i].sitAFT._hitCounter == 0 && aiDatas[i].sitCUR._doing.vecZ != 0)
                    {
                        //aiDatas[i].sitAFT._beforeDB -= 5;
                        aiDatas[i].sitCUR._doing.vecZ = 0;
                    }
                }
                //체력이 절반 이하일 때
                else
                {
                    if (aiDatas[i].sitCUR._doing.vecX != 0 && aiDatas[i].sitCUR._doing.vecY != 0)
                        aiDatas[i].sitAFT._beforeDB--;

                    if (aiDatas[i].sitAFT._dist > distData)
                        aiDatas[i].sitAFT._beforeDB++;

                    if (distNum >= 30f && aiDatas[i].sitCUR._doing.vecX == 0 && aiDatas[i].sitCUR._doing.vecY == 0
                        && aiDatas[i].sitAFT._closer == true && aiDatas[i].sitAFT._dist <= distNum)
                    {
                        aiDatas[i].sitAFT._beforeDB -= 3;
                    }

                    if (distNum >= 30f && aiDatas[i].sitCUR._doing.vecX != 0 && aiDatas[i].sitCUR._doing.vecY != 0
                        && aiDatas[i].sitAFT._closer == false && aiDatas[i].sitAFT._dist > distNum)
                    {
                        aiDatas[i].sitAFT._beforeDB += 3;
                    }

                    if (distNum <= 30f && aiDatas[i].sitCUR._doing.vecX == 0 && aiDatas[i].sitCUR._doing.vecY == 0
                        && aiDatas[i].sitAFT._closer == true && aiDatas[i].sitAFT._dist <= distNum)
                    {
                        aiDatas[i].sitAFT._beforeDB -= 3;
                    }

                    if (distNum <= 30f && aiDatas[i].sitCUR._doing.vecX != 0 && aiDatas[i].sitCUR._doing.vecY != 0
                        && aiDatas[i].sitAFT._closer == true && aiDatas[i].sitAFT._dist > distNum)
                    {
                        aiDatas[i].sitAFT._beforeDB += 3;
                    }
                }

                //아무것도 안 했으면 감점
                if (aiDatas[i].sitCUR._doing.vecX == 0)
                {
                    aiDatas[i].sitAFT._beforeDB -= 10;

                    if (aiDatas[i].sitCUR._doing.vecY == 0 && aiDatas[i].sitCUR._doing.vecZ == 0)
                        aiDatas[i].sitAFT._beforeDB -= 10;
                }

                //dataCollections 중 올바른 위치에 값 저장
                int mov = aiDatas[i].sitCUR._doing.vecX;
                int rot = aiDatas[i].sitCUR._doing.vecY;
                int atk = aiDatas[i].sitCUR._doing.vecZ;

                SituationCUR curBowl = new SituationCUR("NULL", 0f, 0f, 0f, 0f, new IntVector3(-1, -1, -1), 0f);
                SituationAFT aftBowl = new SituationAFT("NULL", 0f, 0f, 0f, 0f, "NULL", 0, 0, false);

                curBowl._id = angle + ", " + distNum;
                curBowl._angleComp = angleData + dataCollections[mov, rot, atk].sitCUR._angleComp;
                curBowl._dist = distData + dataCollections[mov, rot, atk].sitCUR._dist;
                curBowl._doing = new IntVector3(mov, rot, atk);
                curBowl._time = aiDatas[i].sitCUR._time + dataCollections[mov, rot, atk].sitCUR._time;
                curBowl._posX = 1 + dataCollections[mov, rot, atk].sitCUR._posX;

                aftBowl._id = angle + ", " + distNum;
                aftBowl._angleComp = aiDatas[i].sitAFT._angleComp + dataCollections[mov, rot, atk].sitAFT._angleComp;
                aftBowl._dist = aiDatas[i].sitAFT._dist + dataCollections[mov, rot, atk].sitAFT._dist;
                aftBowl._beforeDB = aiDatas[i].sitAFT._beforeDB + dataCollections[mov, rot, atk].sitAFT._beforeDB;
                aftBowl._hitCounter = aiDatas[i].sitAFT._hitCounter + dataCollections[mov, rot, atk].sitAFT._hitCounter;

                if (aiDatas[i].sitAFT._closer)
                    aftBowl._posX = 1 + dataCollections[mov, rot, atk].sitAFT._posX;
                else
                    aftBowl._posZ = 1 + dataCollections[mov, rot, atk].sitAFT._posZ;

                dataCollections[mov, rot, atk].sitCUR = curBowl;
                dataCollections[mov, rot, atk].sitAFT = aftBowl;
            }
        }

        //통계내기
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    if (dataCollections[i, j, k].sitCUR._id != "NULL")
                    {
                        int count = (int)(dataCollections[i, j, k].sitCUR._posX);

                        //Debug.Log("("+i+", "+j+", "+k+"): "+ (double)(dataCollections[i, j, k].sitAFT._beforeDB * 10 / (int)(dataCollections[i, j, k].sitCUR._posX)));

                        dataCollections[i, j, k].sitCUR._angleComp /= count;
                        dataCollections[i, j, k].sitCUR._dist /= count;
                        dataCollections[i, j, k].sitCUR._time /= count;

                        dataCollections[i, j, k].sitAFT._angleComp /= count;

                        if (dataCollections[i, j, k].sitAFT._posX - dataCollections[i, j, k].sitAFT._posZ > 0)
                            dataCollections[i, j, k].sitAFT._closer = true;
                        else
                            dataCollections[i, j, k].sitAFT._closer = false;

                        //Debug.Log(dataCollections[i, j, k].sitCUR._doing.IntVector3ToString() + ": " + count);
                    }
                }
            }
        }
        //통계 결과 가장 높은 dataCollections[i, j, k].sitCUR._posZ (행동점수를 임시로 저장한 변수) 값을 추적
        double maxScore = -999999;
        IntVector3 maxScoreIndex = new IntVector3(-1,-1,-1);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    if (dataCollections[i, j, k].sitCUR._id != "NULL" && maxScore < (double)(dataCollections[i, j, k].sitAFT._beforeDB * 10 / (int)(dataCollections[i, j, k].sitCUR._posX)))
                    {
                        maxScore = (double)(dataCollections[i, j, k].sitAFT._beforeDB * 10 / (int)(dataCollections[i, j, k].sitCUR._posX));
                        maxScoreIndex = dataCollections[i, j, k].sitCUR._doing;
                    }
                }
            }
        }

        if (maxScoreIndex == new IntVector3(0, 0, 0))
        {
            maxScoreIndex.vecX = -1;
        }

        if (maxScoreIndex.vecX != -1)
        {
            dataCollections[maxScoreIndex.vecX, maxScoreIndex.vecY, maxScoreIndex.vecZ].sitAFT._beforeDB /= (int)(dataCollections[maxScoreIndex.vecX, maxScoreIndex.vecY, maxScoreIndex.vecZ].sitCUR._posX);

            dataCollections[maxScoreIndex.vecX, maxScoreIndex.vecY, maxScoreIndex.vecZ].sitCUR._posX = 0f;
            dataCollections[maxScoreIndex.vecX, maxScoreIndex.vecY, maxScoreIndex.vecZ].sitCUR._posZ = 0f;
            dataCollections[maxScoreIndex.vecX, maxScoreIndex.vecY, maxScoreIndex.vecZ].sitAFT._posX = 0f;
            dataCollections[maxScoreIndex.vecX, maxScoreIndex.vecY, maxScoreIndex.vecZ].sitAFT._posZ = 0f;

            result(dataCollections[maxScoreIndex.vecX, maxScoreIndex.vecY, maxScoreIndex.vecZ]);
        }
        //else
        //{
        //    result.sitCUR._doing = new IntVector3(-1, -1, -1);
        //}

        coroutineLocker++;
        yield break;
    }

    void Update()
    {
        //if (listSitCUR.Count != 0)
        //{
        //    Debug.Log("0: " + listSitCUR[0]._doing.vecX + ", " + listSitCUR[0]._doing.vecY + ", " + listSitCUR[0]._doing.vecZ);
        //}
        //모두 저장
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(Saver());
        }

        //20190606 임시
        if (Input.GetKeyDown(KeyCode.L))
        {
            IO_CSV.Writer_CSV("/ScoreCAL.csv", listScore_FOR_PPT);
        }

        ////BigData 유닛이 검증한 우수 행동 별도 저장
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    StartCoroutine(SaverGreat());
        //}
    }

    //IEnumerator NULLDataSampleSaver()
    //{
    //    IO_SqlDB.WriteDB_CUR("behaveData_Part0", listSitCUR);
    //    IO_SqlDB.WriteDB_AFT("behaveData_Part0", listSitAFT);

    //    Debug.Log("NullDataSaveDone");
    //    yield break;
    //}


    //IEnumerator SampleIE()
    //{
    //    SituationCUR cur = new SituationCUR("N", -1f, -1f, -1f, -1f, new IntVector3(-1, -1, -1), -1f);
    //    SituationAFT aft = new SituationAFT("N", -1f, -1f, -1f, -1f, "NN", -1, -1, false);

    //    for (int i = 0; i < 20; i++)
    //    {
    //        for (int j = 0; j < 10000; j++)
    //        {
    //            listSitCUR.Add(cur);
    //            listSitAFT.Add(aft);
    //        }

    //        Debug.Log("*");
    //        yield return null;
    //    }

    //    Debug.Log("Dummy Done");
    //    StartCoroutine("NULLDataSampleSaver");

    //    yield break;
    //}

    

    IEnumerator SaveAFT()
    {
        IO_SqlDB.WriteDB_AFT("behaveDataGreatATK_AFT", listSitAFT1);
        Debug.Log("Save Done");

        yield break;
    }

    IEnumerator SaverGreat()
    {
        for (int i = 0; i < listSitCUR.Count - 1; i++)
        {
            for (int j = i + 1; j < listSitCUR.Count; j++)
            {
                if (listSitCUR[i]._id == listSitCUR[j]._id)
                {
                    listSitCUR.RemoveAt(j);
                    j--;
                }
            }

            yield return null;
        }

        IO_SqlDB.WriteDB_CUR("behaveDataGreatATK", listSitCUR);


        Debug.Log(listSitCUR.Count + " GreatBehaves Save Complete");
        yield break;
    }

    IEnumerator Saver()
    {
        for (int i = 0; i < listSitCUR.Count; i++)
        {
            if ((listSitCUR[i]._doing.vecX == -1) || (listSitCUR[i]._doing.vecY == -1) || (listSitCUR[i]._doing.vecZ == -1))
            {
                listSitCUR.RemoveAt(i);
                i--;
            }
        }

        if (listSitCUR.Count != listSitAFT.Count)
        {
            Debug.Log("ERROR");
            listSitAFT.RemoveRange(0, listSitAFT.Count - listSitCUR.Count);
            Debug.Log("Kill ERROR Data Done");
        }

        if (listSitCUR.Count != 0)
        {

            //for (int i = 0; i < listSitCUR.Count; i++)
            //{
            //    aiDatas.Add(new AIData(listSitCUR[i], listSitAFT[i]));
            //}

            //yield return null;
            //SaveDatas_Div_BY_Angle(true);

            IO_SqlDB.WriteDB_CUR("behaveDataReinforce", listSitCUR);
            yield return null;
            IO_SqlDB.WriteDB_AFT("behaveDataReinforce", listSitAFT);
            yield return null;
        }

        Debug.Log("HP HIGH Data " + listSitCUR.Count + " Save Complete");
        yield break;
    }

    //void InitAIDatas(List<SituationCUR> cur, List<SituationAFT> aft)
    //{
    //    for (int i = 0; i < cur.Count; i++)
    //    {
    //        if (cur[i]._doing.vecZ == 2 && cur[i]._angleComp < 0 && aft[i]._angleComp < 0)
    //        {
    //            aft[i]._hitCounter = 0;
    //            continue;
    //        }
    //        else if (cur[i]._doing.vecZ == 3 && cur[i]._angleComp > 0 && aft[i]._angleComp > 0)
    //        {
    //            aft[i]._hitCounter = 0;
    //            continue;
    //        }
    //        else if (cur[i]._doing.vecZ != 0 && (cur[i]._angleComp > 170f || cur[i]._angleComp < -170f) && (aft[i]._angleComp > 170f || aft[i]._angleComp < -170f))
    //        {
    //            Debug.Log("3");
    //            aft[i]._hitCounter = 0;
    //            continue;
    //        }
    //        else
    //        {
    //            if (aft[i]._hitCounter != 0 && cur[i]._doing.vecZ != 0)
    //            {
    //                aiDatas.Add(new AIData(cur[i], aft[i]));
    //            }
    //        }
    //    }
    //}
}
