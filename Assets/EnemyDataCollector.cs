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
    public List<AIData> aiDatasGreedy = new List<AIData>();

    public List<AIData> data_FOR_Learn = new List<AIData>();

    public List<SituationCUR> goodBehaveList = new List<SituationCUR>();
    public List<SituationCUR> goodBehaveListLOW = new List<SituationCUR>();

    public int mod = 0;
    public int sampleFileString = 0;

    private int coroutineLocker = 0;

    public float sigmaValue = 0.0f;

    public double sampleAlpha = 0.0f;
    public double sampleGamma = 0.0f;
    public int sample_Q_Depth = 3;
    bool sampleLock = false;

    private SortANDSearch sortANDSearch = new SortANDSearch();

    List<AIData> dqn_CacheDataList = new List<AIData>();
    int cacheCount = 10000;

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

    public class SortANDSearch {

        List<AIData> aiDatasSORT = new List<AIData>();
        List<AIData> result = new List<AIData>();
        List<SituationCUR> sitCURSORT = new List<SituationCUR>();

        AIData tempAD = new AIData();
        SituationCUR tempsitCUR = new SituationCUR();

        public List<AIData> QuickSort_AIData(List<AIData> input)
        {
            aiDatasSORT = input;

            quicksort_AIData(0, aiDatasSORT.Count - 1);

            return aiDatasSORT;
        }

        public List<AIData> Search_AIData_List(float dist, float angle, List<AIData> input)
        {
            result.Clear();

            int index = Search_AIData(dist, angle, input);
            int minIndex = index - 500, maxIndex = index + 500;

            if (minIndex < 0) minIndex = 0;
            if (maxIndex > input.Count - 1) maxIndex = input.Count - 1;

            for (int i = minIndex; i <= maxIndex; i++)
            {
                if (input[i].sitCUR._dist >= dist && input[i].sitCUR._dist < dist + 2
                    && input[i].sitCUR._angleComp >= angle && input[i].sitCUR._angleComp < angle + 1)
                {
                    Debug.Log("Find it: " + i);
                    result.Add(new AIData(input[i]));
                }
            }

            return result;
        }

        void quicksort_AIData(int l, int r)
        {
            int p = l;
            int j = p;
            int i = l + 1;

            if (l < r)
            {
                for (; i <= r; i++)
                {
                    if (aiDatasSORT[i].sitCUR._angleComp < aiDatasSORT[p].sitCUR._angleComp)
                    {
                        j++;

                        //스왑
                        tempAD = aiDatasSORT[j];
                        aiDatasSORT[j] = aiDatasSORT[i];
                        aiDatasSORT[i] = tempAD;
                    }
                }
                //스왑
                tempAD = aiDatasSORT[l];
                aiDatasSORT[l] = aiDatasSORT[j];
                aiDatasSORT[j] = tempAD;

                p = j;

                quicksort_AIData(l, p - 1);
                quicksort_AIData(p + 1, r);
            }
        }

        public int Search_AIData(float dist, float angle, List<AIData> input)
        {
            int result = 0;
            aiDatasSORT = input;

            search_AIData(dist, angle, aiDatasSORT.Count, aiDatasSORT.Count, 1, ref result);

            return result;
        }

        void search_AIData(float dist, float angle, int i, int length, int j, ref int resultIndex)
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
                if (aiDatasSORT[index].sitCUR._angleComp > angle)
                {
                    j *= 2;
                    search_AIData(dist, angle, -index, length, j, ref resultIndex);
                }
                else if (aiDatasSORT[index].sitCUR._angleComp < angle)
                {
                    j *= 2;
                    search_AIData(dist, angle, index, length, j, ref resultIndex);
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
                    float comp = Mathf.Abs(aiDatasSORT[q].sitCUR._dist - dist);

                    if (comp < _dist)
                    {
                        _dist = comp;
                        resultIndex = q;
                    }
                }
            }
        }

        public List<SituationCUR> QuickSort_SitCUR(List<SituationCUR> input)
        {
            sitCURSORT = input;

            quicksort_SitCUR(0, sitCURSORT.Count - 1);

            return sitCURSORT;
        }

        void quicksort_SitCUR(int l, int r)
        {
            int p = l;
            int j = p;
            int i = l + 1;

            if (l < r)
            {
                for (; i <= r; i++)
                {
                    if (sitCURSORT[i]._angleComp < sitCURSORT[p]._angleComp)
                    {
                        j++;
                        //스와핑
                        tempsitCUR = sitCURSORT[j];
                        sitCURSORT[j] = sitCURSORT[i];
                        sitCURSORT[i] = tempsitCUR;
                    }
                }
                //스와핑
                tempsitCUR = sitCURSORT[l];
                sitCURSORT[l] = sitCURSORT[j];
                sitCURSORT[j] = tempsitCUR;
                p = j;

                quicksort_SitCUR(l, p - 1);
                quicksort_SitCUR(p + 1, r);
            }
        }

        public int Search_SitCUR(float dist, float angle, int i, int length, int j, List<SituationCUR> input)
        {
            int result = 0;
            sitCURSORT = input;

            search_SitCUR(dist, angle, i, length, j, ref result);

            return result;
        }

        void search_SitCUR(float dist, float angle, int i, int length, int j, ref int result)
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
                if (sitCURSORT[index]._angleComp > angle)
                {
                    j *= 2;
                    search_SitCUR(dist, angle, -index, length, j, ref result);
                }
                else if (sitCURSORT[index]._angleComp < angle)
                {
                    j *= 2;
                    search_SitCUR(dist, angle, index, length, j, ref result);
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
                    float comp = Mathf.Abs(sitCURSORT[q]._dist - dist);

                    if (comp < _dist)
                    {
                        _dist = comp;
                        result = q;
                    }
                }
            }
        }
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

    public SituationCUR SearchGoodSitCUR(float dist, float angle, bool isHPLOW, bool isGreedy)
    {
        SituationCUR result = new SituationCUR("NULL", -1f, -1, -1, -1, new IntVector3(-1, -1, -1), -1f);
        int index = -1;

        if (!isGreedy)
        {
            //Debug.Log("CUR: " + dist + ", " + angle);
            search(dist, angle, goodBehaveList.Count, goodBehaveList.Count, 1, ref index);
            //index = sortANDSearch.Search_SitCUR(dist, angle, goodBehaveList.Count, goodBehaveList.Count, 1, goodBehaveList);
            result = goodBehaveList[index];
            //Debug.Log("Result: " + result._dist + ", " + result._angleComp);
        }
        else
        {
            //index = sortANDSearch.Search_SitCUR(dist, angle, goodBehaveList.Count, goodBehaveList.Count, 1, goodBehaveListLOW);
            result = aiDatasGreedy[sortANDSearch.Search_AIData(dist, angle, aiDatasGreedy)].sitCUR;
        }

        //Debug.Log("CurD: "+ dist + ". CurA:"+ angle + ", DBD: "+ goodBehaveList[resultIndex]._dist + ", DBA: "+ goodBehaveList[resultIndex]._angleComp);

        return result;
    }

    double GetSimpleError(List<double> a, List<double> b)
    {
        double result = 0.0;

        for (int i = 0; i < a.Count; i++)
        {
            result += Mathf.Abs((float)(a[i] - b[i]));
        }

        return result / a.Count;
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
            //goodBehaveList = IO_SqlDB.ReadSitCUR_FROM_DB("behaveDataGreatATK");
            //goodBehaveList = sortANDSearch.QuickSort_SitCUR(goodBehaveList);

            aiDatasGreedy = IO_SqlDB.ReadAIData_FROM_DB("behaveDataScored");
        }
        //수렴 완벽에 가깝게 잘 됨
        //이제 전체 데이터 학습에 대해 어떻게 할 것인지 고찰이 필요함
        else if (mod == 1)
        {
            if (sampleFileString == 0)
            {
                //goodBehaveList = IO_SqlDB.ReadSitCUR_FROM_DB("behaveDataScored");

                List<FCNN> sample = new List<FCNN>();

                //TABLE 한개당 36 * 36까지는 수용가능
                sample.Add(new FCNN(6, 36, 36, 0.1));

                IO_SqlDB.WriteDB_FCNN("___________Sample", new List<string>(), sample, true);
            }
            else if (sampleFileString == 1)
            {
                //goodBehaveList = IO_SqlDB.ReadSitCUR_FROM_DB("Q_LearnedData");
                //aiDatasGreedy = IO_SqlDB.ReadAIData_FROM_DB("behaveDataScored");
                //sortANDSearch.QuickSort_AIData(aiDatasGreedy);

                //depth 6, input 5개, output 5개에서 가장 이상적인 LearningRate == 0.000002
                FCNN sampleNet = new FCNN(6, 5, 5, 0.000002);
                List<double> inputput = new List<double>();
                List<double> target = new List<double>();

                //각도는 * 0.01, 거리는 * 0.1해서 넣어야 됨
                inputput.Add(-0.26);
                inputput.Add(0.4);
                inputput.Add(0.75);
                inputput.Add(inputput[0] * inputput[0]);
                inputput.Add(inputput[1] * inputput[1]);
                sampleNet.SetInput(inputput);

                target.Add(3.516);
                target.Add(6.714);
                target.Add(121.419);
                target.Add(8.188);
                target.Add(25.423);

                int index = 0;

                //수렴 잘 함
                for (int i = 0; i < 10000; i++)
                {
                    sampleNet.ForwardProp();
                    if (i % 1000 == 0)
                        Debug.Log(i + ": " + sampleNet.output[0] + ", " + sampleNet.output[1] + ", " + sampleNet.output[2] + ", " + sampleNet.output[3] + ", " + sampleNet.output[4]
                             );
                    sampleNet.BackProp(target);

                    if (GetSimpleError(sampleNet.output, target) <= 10 && GetSimpleError(sampleNet.output, target) > 1.0)
                        sampleNet.learningRate -= 0.0000000001;
                    else if(GetSimpleError(sampleNet.output, target) <= 1.0 && GetSimpleError(sampleNet.output, target) > 0.015)
                        sampleNet.learningRate -= 0.000000000001;

                    if (GetSimpleError(sampleNet.output, target) > 10)
                        sampleNet.learningRate += 0.0000002;

                    if (GetSimpleError(sampleNet.output, target) <= 0.015)
                    {
                        index = i;
                        break;
                    }
                }
                sampleNet.ForwardProp();
                Debug.Log("Last: " + sampleNet.output[0] + ", " + sampleNet.output[1] + ", " + sampleNet.output[2] + ", " + sampleNet.output[3] + ", " + sampleNet.output[4]
                             );
                if(index != 0)
                    Debug.Log("Early End: "+index);
            }
            else
            {
                goodBehaveList = IO_SqlDB.ReadSitCUR_FROM_DB("behaveDataScored_2_0");
            }
            goodBehaveList = sortANDSearch.QuickSort_SitCUR(goodBehaveList);
            //quickSort(0, goodBehaveList.Count - 1);

            //goodBehaveListLOW = IO_SqlDB.ReadSitCUR_FROM_DB("behaveDataScoredLOW");
            //quickSortLOW(0, goodBehaveListLOW.Count - 1);
        }
        //학습 모드 - QLearning
        else if (mod == 5)
        {
            //aiDatasGreedy = IO_SqlDB.ReadAIData_FROM_DB("behaveDataScored");
            //aiDatasGreedy = sortANDSearch.QuickSort_AIData(aiDatasGreedy);

            //Debug.Log("Start");
            //for (int angleM = 18; angleM <= 18; angleM++)
            //{
            //    for (int angleS = 0; angleS < 10; angleS += 2)
            //    {
            //        for (int d = 0; d <= 50; d += 2)
            //        {
            //            for (float t = 0.5f; t < 1.5f; t += 0.25f)
            //            {
            //                GetQValue(angleM, angleS, d, t);
            //            }
            //        }
            //        aiDatas.Clear();
            //    }
            //}
            //Debug.Log("Fin");
            //if (sampleFileString != 1)
            //    StartCoroutine(GetQBaseDataStarter(-18, -15));
            //else
            //    StartCoroutine(Q_LearningStarter());
        }
        //DQN
        else if (mod == 6)
        {

        }
        //분류가 되지 않은 AIData들을 각도별로 분류하는 모드
        else if (mod == 7)
        {
            //StartCoroutine(Div_BY_Angle_HugeBehaveData());
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

    IEnumerator Q_LearningStarter()
    {
        //각 상황들에 대한 Greedy한 행동들을 모두 읽어오기
        aiDatasGreedy = IO_SqlDB.ReadAIData_FROM_DB("behaveDataScored");
        aiDatasGreedy = sortANDSearch.QuickSort_AIData(aiDatasGreedy);

        for (int angle = -18; angle < 18; angle++)
        {
            Debug.Log("A: "+ angle);
            StartCoroutine(QLearning(angle, false));
            aiDatas.RemoveRange(0, aiDatas.Count);

            yield return null;
        }

        yield break;
    }

    List<AIData> GetGreedyResultAIDatas(AIData input)
    {
        List<AIData> result = new List<AIData>();
        AIData cacheData = new AIData();

        if (dqn_CacheDataList.Count > 0)
            cacheData = new AIData(dqn_CacheDataList[dqn_CacheDataList.Count - 1]);

        //input을 input 결과에서 가장 Greedy한 AIData로 덮어씀
        input.Set(aiDatasGreedy[sortANDSearch.Search_AIData(input.sitAFT._dist, input.sitAFT._angleComp, aiDatasGreedy)]);

        //데이터 라벨링
        if ((int)(input.sitCUR._dist) % 2 != 0) input.sitCUR._dist = (int)(input.sitCUR._dist) - 1;
        else input.sitCUR._dist = (int)(input.sitCUR._dist);

        input.sitCUR._angleComp = (int)(input.sitCUR._angleComp);

        if (dqn_CacheDataList.Count >= cacheCount)
            dqn_CacheDataList.Clear();

        //dqn_CacheDataList에 저장된 데이터에서 먼저 검색
        if (dqn_CacheDataList.Count > 0)
            result = sortANDSearch.Search_AIData_List((int)(input.sitCUR._dist), (int)(input.sitCUR._angleComp), dqn_CacheDataList);

        //찾을 수 없다면
        if (result.Count == 0)
        {
            //input상황과 일치하는 모든 데이터를 Q_BaseData에서 읽기
            result = IO_SqlDB.ReadAIData_FROM_DB_Angle_AND_Dist("Q_BaseData", (int)(input.sitCUR._angleComp), 1, (int)(input.sitCUR._dist), 2);
        }

        //Greedy한 행동이 아닌 경우를 모두 제외하고 반환
        for (int i = 0; i < result.Count; i++)
        {
            if (result[i].sitCUR._doing != input.sitCUR._doing)
            {
                result.RemoveAt(i);
                i--;
            }
            else
            {
                if (dqn_CacheDataList.Count > 0)
                {
                    int index = sortANDSearch.Search_AIData(result[i].sitCUR._dist, result[i].sitCUR._angleComp, dqn_CacheDataList);

                    //중복 데이터면 캐시List에 저장하지 않는다.
                    if (result[i].sitAFT._dist == dqn_CacheDataList[index].sitAFT._dist
                        && result[i].sitAFT._angleComp == dqn_CacheDataList[index].sitAFT._angleComp
                        && result[i].sitCUR._id == dqn_CacheDataList[index].sitCUR._id)
                        continue;
                    //새로운 데이터면 저장한다.
                    else
                        dqn_CacheDataList.Add(result[i]);
                }
                else
                {
                    dqn_CacheDataList.Add(result[i]);
                }
            }
        }

        //추가된 사항이 있으면 dqn_CacheDataList 퀵 정렬 수행
        try
        {
            if (cacheData.sitCUR._id != dqn_CacheDataList[dqn_CacheDataList.Count - 1].sitCUR._id)
            {
                dqn_CacheDataList = sortANDSearch.QuickSort_AIData(dqn_CacheDataList);
            }
        }
        //dqn_CacheDataList가 비어있음
        catch (ArgumentOutOfRangeException)
        {
            //Nothing to Do
        }

        //자료가 없으면 평균값으로 수행한다.
        if (result.Count == 0)
            result.Add(input);

        return result;
    }

    //모든 행동에 대해 Q값을 구해서 다른 DB에 저장 겸 다시 라벨링
    //각도는 2도, 거리는 2, 시간은 0.5초로 다시 라벨링 해서 저장할 것. 단, 행동은 84개에서 36개로 축소(Rotation쪽을 단방향 회전, 무회전으로 축소)
    void GetQValue(int angleMain, int angleSUB, int dist, float time)
    {
        string fileName = "behaveDataAngle";
        string code = "";

        int angle = angleMain * 10 + angleSUB;
        string id = "(" + angle + ", " + dist + ", " + time + ")";
        AIData[,,] datas = new AIData[3,3,4];

        for (int m = 0; m < 3; m++)
        {
            for (int r = 0; r < 3; r++)
            {
                for (int a = 0; a < 4; a++)
                {
                    datas[m, r, a] = new AIData(new SituationCUR(id, 0f, 0f, dist, angle, new IntVector3(m, r, a), time), new SituationAFT(id, 0f, 0f, 0f, 0f, "", 0, -1, false));
                }
            }
        }

        //code 초기화
        int angleTemp = angleMain;

        if (angleTemp < 0)
        {
            angleTemp *= (-1);
            code += "M";
        }
        else
        {
            code += "P";
        }
        code += angleTemp.ToString();

        //읽기
        if (aiDatas.Count == 0)
        {
            fileName += code;

            aiDatas = IO_SqlDB.ReadAIData_FROM_DB_Angle(fileName, angle, 2);
        }

        for (int i = 0; i < aiDatas.Count; i++)
        {
            if (dist <= aiDatas[i].sitCUR._dist && (dist + 2 > aiDatas[i].sitCUR._dist || dist == 50)
                && angle <= aiDatas[i].sitCUR._angleComp && angle + 2 >= aiDatas[i].sitCUR._angleComp
                && time <= aiDatas[i].sitCUR._time && time + 0.25f > aiDatas[i].sitCUR._time
                && aiDatas[i].sitCUR._doing.vecY < 3)
            {
                int m = aiDatas[i].sitCUR._doing.vecX;
                int r = aiDatas[i].sitCUR._doing.vecY;
                int a = aiDatas[i].sitCUR._doing.vecZ;

                //Reward값 계산 후 저장, Q값 계산 후 저장, 행동결과 저장, 명중률, data 갯수 카운팅
                datas[m, r, a].sitAFT._posX += CalScore(false, aiDatas[i]);
                datas[m, r, a].sitAFT._posZ += (float)(Q_Func(3, 0, (double)(datas[m, r, a].sitAFT._posX), aiDatas[i]));
                datas[m, r, a].sitAFT._dist += aiDatas[i].sitAFT._dist;
                datas[m, r, a].sitAFT._angleComp += aiDatas[i].sitAFT._angleComp;
                datas[m, r, a].sitCUR._posX += aiDatas[i].sitAFT._hitCounter;
                datas[m, r, a].sitAFT._beforeDB++;

                aiDatas.RemoveAt(i);
                i--;
            }
            else
                continue;
        }

        //평균 내기
        for (int m = 0; m < 3; m++)
        {
            for (int r = 0; r < 3; r++)
            {
                for (int a = 0; a < 4; a++)
                {
                    if (datas[m, r, a].sitAFT._beforeDB != 0)
                    {
                        int count = datas[m, r, a].sitAFT._beforeDB;

                        datas[m, r, a].sitAFT._posX /= count;
                        datas[m, r, a].sitAFT._posZ /= count;
                        datas[m, r, a].sitAFT._dist /= count;
                        datas[m, r, a].sitAFT._angleComp /= count;
                        datas[m, r, a].sitCUR._posX /= count;

                        data_FOR_Learn.Add(datas[m, r, a]);
                    }
                }
            }
        }

        //라벨링 후 저장을 위한 과정
        fileName = "LabeledQDataAngle";
        fileName += code;

        StartCoroutine(Save_DataFORLearn(fileName));
    }

    IEnumerator Save_DataFORLearn(string fileName)
    {
        List<AIData> saveData = new List<AIData>();

        for (int i = 0; i < data_FOR_Learn.Count; i++)
        {
            if (data_FOR_Learn[i].sitCUR._doing.vecX != -1)
            {
                saveData.Add(data_FOR_Learn[i]);
            }
        }

        IO_SqlDB.WriteDB_AIDatas(fileName, saveData);
        data_FOR_Learn.Clear();
        sampleLock = true;

        yield break;
    }

    IEnumerator Save_DataFORLearn(string fileName, List<AIData> input)
    {
        List<AIData> saveData = new List<AIData>();

        for (int i = 0; i < input.Count; i++)
        {
            if (input[i].sitCUR._doing.vecX != -1)
            {
                saveData.Add(input[i]);
            }
        }

        Debug.Log(saveData.Count);
        IO_SqlDB.WriteDB_AIDatas(fileName, saveData);
        data_FOR_Learn.Clear();

        Debug.Log("input Save Done");
        //sampleLock = true;

        yield break;
    }

    private void ReadAIData_BY_Angle(string fileName, int angle, int angleNum, bool isLOW_HP_Behave)
    {
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
    }

    IEnumerator GetQBaseDataStarter(int startAngle, int endAngle)
    {
        //bool saveLocker = true;

        for (int angle = startAngle; angle < endAngle; angle++)
        {
            for (int angleSUB = 0; angleSUB <= 9; angleSUB++)
            {
                for (float dist = 0f; dist <= 50f; dist += 2.0f)
                {
                    yield return null;

                    StartCoroutine(GetQBaseData(angle, angleSUB, dist, false));
                }
                aiDatas.RemoveRange(0, aiDatas.Count);
            }
        }

        StartCoroutine(Save_DataFORLearn("Q_BaseData"));
        //초기화
        coroutineLocker = 0;
        aiDatas.Clear();
        data_FOR_Learn.Clear();
        
        if (endAngle != 18 && sampleLock)
        {
            startAngle += 3;
            endAngle += 3;
            yield return StartCoroutine(GetQBaseDataStarter(startAngle, endAngle));
            sampleLock = false;
        }

        yield break;
    }

    IEnumerator GetQBaseData(int angleNum, int angleSUB, float distNum, bool isLOW_HP_Behave)
    {
        int angle = angleNum * 10 + angleSUB;

        ReadAIData_BY_Angle("behaveDataAngle", angle, angleNum, isLOW_HP_Behave);

        SituationCUR[,,] dataCollections = new SituationCUR[3, 7, 4];
        List<SituationAFT> aftCollections = new List<SituationAFT>();

        //초기화
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    dataCollections[i, j, k] = new SituationCUR();
                    dataCollections[i, j, k]._posX = 1;
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

            if ((angle <= angleData && angleData < angle + 1f) && (distNum <= distData && distData < distNum + 2.0f))
            {
                //점수 결과를 _beforeDB에다 각각 저장
                aiDatas[i].sitAFT._beforeDB = CalScore(angleData, distData, distNum, angle, isLOW_HP_Behave, aiDatas[i]);

                //dataCollections 중 올바른 위치에 값 저장
                int mov = aiDatas[i].sitCUR._doing.vecX;
                int rot = aiDatas[i].sitCUR._doing.vecY;
                int atk = aiDatas[i].sitCUR._doing.vecZ;

                bool findIt = false;
                int labeledDist = (int)(aiDatas[i].sitAFT._dist + 0.01f);
                int labeledAngle = (int)(aiDatas[i].sitAFT._angleComp + 0.01f);

                if (labeledDist % 2 != 0)
                    labeledDist--;
                if (labeledAngle % 2 != 0)
                    labeledAngle--;

                string id = angle + ", " + distNum + ": " + aiDatas[i].sitCUR._doing.IntVector3ToString();
                int aftTotalCount = 0;

                //aftCollection의 길이가 1이상인 경우
                if (aftCollections.Count != 0)
                {
                    for (int aftC_count = aftCollections.Count - 1; aftC_count >= 0; aftC_count--)
                    {
                        //id가 일치할 때 
                        if (aftCollections[aftC_count]._id == id)
                        {
                            aftCollections[aftC_count]._posZ = aftCollections[aftC_count]._posZ + 1f;
                            aftTotalCount = (int)(aftCollections[aftC_count]._posZ + 0.5f);

                            //행동 결과가 유사할 때
                            if ((aftCollections[aftC_count]._angleComp - labeledAngle < 1)
                                && (aftCollections[aftC_count]._angleComp >= labeledAngle)
                                && (aftCollections[aftC_count]._dist - labeledDist < 2)
                                && (aftCollections[aftC_count]._dist >= labeledDist))
                            {
                                aftCollections[aftC_count]._beforeDB += aiDatas[i].sitAFT._beforeDB;
                                aftCollections[aftC_count]._hitCounter += aiDatas[i].sitAFT._hitCounter;
                                //횟수 세기
                                aftCollections[aftC_count]._posX = aftCollections[aftC_count]._posX + 1f;
                                findIt = true;
                            }
                        }
                    }

                    //유사한게 없으면 새로 만든다
                    if (!findIt)
                    {
                        if (aftTotalCount == 0)
                            aftTotalCount = 1;

                        aftCollections.Add(new SituationAFT(id, 1f, aftTotalCount,
                            labeledDist,
                            labeledAngle, "NULL",
                            aiDatas[i].sitAFT._beforeDB,
                            aiDatas[i].sitAFT._hitCounter, false));
                    }
                }
                //aftCollection의 길이가 0인 경우
                else
                {
                    aftCollections.Add(new SituationAFT(id, 1f, 1f,
                        labeledDist,
                        labeledAngle, "NULL",
                        aiDatas[i].sitAFT._beforeDB,
                        aiDatas[i].sitAFT._hitCounter, false));
                }

                //SitCUR 저장
                dataCollections[mov, rot, atk]._id = id;
                dataCollections[mov, rot, atk]._angleComp = angle;
                dataCollections[mov, rot, atk]._dist = distNum;
                dataCollections[mov, rot, atk]._doing = new IntVector3(mov, rot, atk);
                dataCollections[mov, rot, atk]._time += aiDatas[i].sitCUR._time;
                dataCollections[mov, rot, atk]._posX = 1 + dataCollections[mov, rot, atk]._posX;
            }
        }

        int count = 0;

        for (int aftcount = 0; aftcount < aftCollections.Count; aftcount++)
        {
            count = (int)(aftCollections[aftcount]._posX + 0.5f);

            if (count == 0)
                break;

            aftCollections[aftcount]._beforeDB /= count;
            aftCollections[aftcount]._hitCounter /= count;
        }

        //통계내고 data_FOR_Learn으로 모으기
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    if (dataCollections[i, j, k]._doing.vecX != -1)
                    {
                        dataCollections[i, j, k]._posX -= 1f;
                        count = (int)(dataCollections[i, j, k]._posX + 0.1f);

                        //dataCollections[i, j, k]._angleComp /= count;
                        //dataCollections[i, j, k]._dist /= count;
                        dataCollections[i, j, k]._time /= count;

                        if (dataCollections[i, j, k]._time < 0)
                            dataCollections[i, j, k]._time *= (-1);

                        for (int aft = 0; aft < aftCollections.Count; aft++)
                        {
                            if (dataCollections[i, j, k]._id == aftCollections[aft]._id
                                && (aftCollections[aft]._posX / aftCollections[aft]._posZ >= 0.1f))
                            {
                                data_FOR_Learn.Add(new AIData(dataCollections[i, j, k], aftCollections[aft]));

                                aftCollections.RemoveAt(aft);
                                aft--;
                            }
                        } 
                    }
                }
            }
        }

        yield break;
    }

    //Q Learning (3단계까지 검토)
    //alpha == 0.97 && gamma == 0.9 (gamma 값은 변경될 수도 있음)
    IEnumerator QLearning(int angleNum, bool isLOW_HP_Behave)
    {
        //Debug.Log(angle + ", " + distNum + " Start");
        //ReadAIData_BY_Angle("behaveDataAngle", angle, angleNum, isLOW_HP_Behave);
        if (aiDatas.Count == 0)
        {
            aiDatas = IO_SqlDB.ReadAIData_FROM_DB_Angle("Q_BaseData", angleNum * 10, 10);
            if (aiDatas.Count == 0)
            {
                coroutineLocker++;
                yield break;
            }
        }

        //통계내기
        for (int i = 0; i < aiDatas.Count; i++)
        {

                if (aiDatas[i].sitCUR._id != "NULL")
                {
                    data_FOR_Learn.Add(new AIData(aiDatas[i]));
                    //Q 값 추적 후 추가
                    data_FOR_Learn[data_FOR_Learn.Count - 1].sitAFT._posZ = (float)(Q_Func(sample_Q_Depth, 0, 0, new AIData(data_FOR_Learn[i].sitCUR, data_FOR_Learn[i].sitAFT)));
                }
        }

        StartCoroutine(Save_DataFORLearn("Q_LearnedData"));
        yield break;
    }

    private int CalScore(float angleData, float distData, float distNum, int angle, bool isLOW_HP_Behave, AIData data)
    {
        int result = 0;

        //체력 절반 이하, 절반 초과에 따라 다른 행동점수 가산점 OR 불이익 정책 사용
        //체력이 절반을 초과할 때
        if (!isLOW_HP_Behave)
        {
            result += data.sitAFT._hitCounter;

            if (data.sitCUR._doing.vecX == 0 && data.sitAFT._hitCounter > 0)
                result += 1;

            if (distNum >= 30f && data.sitCUR._doing.vecX != 0 && data.sitCUR._doing.vecY != 0
                && data.sitAFT._closer == true && data.sitAFT._dist + 10f < distNum)
            {
                result += 3;
            }

            if (distNum >= 30f && data.sitCUR._doing.vecX == 0 && data.sitCUR._doing.vecY == 0
                && data.sitAFT._closer == false && data.sitAFT._dist >= distNum)
            {
                result -= (int)(20 + data.sitAFT._dist);
            }

            if (data.sitAFT._hitCounter == 0 && data.sitCUR._doing.vecZ != 0)
            {
                result = 0;
            }
        }
        //체력이 절반 이상일 때
        else
        {
            if (data.sitCUR._doing.vecX != 0 && data.sitCUR._doing.vecY != 0)
                result--;

            if (data.sitAFT._dist > distData)
                result++;

            if (distNum >= 30f && data.sitCUR._doing.vecX == 0 && data.sitCUR._doing.vecY == 0
                && data.sitAFT._closer == true && data.sitAFT._dist <= distNum)
            {
                result -= 3;
            }

            if (distNum >= 30f && data.sitCUR._doing.vecX != 0 && data.sitCUR._doing.vecY != 0
                && data.sitAFT._closer == false && data.sitAFT._dist > distNum)
            {
                result += 3;
            }

            if (distNum <= 30f && data.sitCUR._doing.vecX == 0 && data.sitCUR._doing.vecY == 0
                && data.sitAFT._closer == true && data.sitAFT._dist <= distNum)
            {
                result -= 3;
            }

            if (distNum <= 30f && data.sitCUR._doing.vecX != 0 && data.sitCUR._doing.vecY != 0
                && data.sitAFT._closer == true && data.sitAFT._dist > distNum)
            {
                result += 3;
            }
        }

        //아무것도 안 했으면 감점
        if (data.sitCUR._doing.vecX == 0)
        {
            result -= 10;
            if (data.sitCUR._doing.vecY == 0 && data.sitCUR._doing.vecZ == 0)
                result -= 100;
        }


        return result;
    }

    private int CalScore(bool isLOW_HP_Behave, AIData data)
    {
        int result = 0;

        //체력 절반 이하, 절반 초과에 따라 다른 행동점수 가산점 OR 불이익 정책 사용
        //체력이 절반을 초과할 때
        if (!isLOW_HP_Behave)
        {
            result += data.sitAFT._hitCounter * 10;

            if (data.sitCUR._doing.vecX == 0 && data.sitAFT._hitCounter > 0)
                result += 1;

            if (data.sitCUR._dist >= 30f && data.sitCUR._doing.vecX != 0 && data.sitCUR._doing.vecY != 0
                && data.sitAFT._closer == true && data.sitAFT._dist + 10f < data.sitCUR._dist)
            {
                result += 3;
            }

            if (data.sitCUR._dist >= 30f && data.sitCUR._doing.vecX == 0 && data.sitCUR._doing.vecY == 0
                && data.sitAFT._closer == false && data.sitAFT._dist >= data.sitCUR._dist)
            {
                result -= (int)(20 + data.sitAFT._dist);
            }

            if (data.sitAFT._hitCounter == 0 && data.sitCUR._doing.vecZ != 0)
            {
                result = 0;
            }
        }
        //체력이 절반 이하일 때
        else
        {
            if (data.sitCUR._doing.vecX != 0 && data.sitCUR._doing.vecY != 0)
                result--;

            if (data.sitAFT._dist > data.sitCUR._dist)
                result++;

            if (data.sitCUR._dist >= 30f && data.sitCUR._doing.vecX == 0 && data.sitCUR._doing.vecY == 0
                && data.sitAFT._closer == true && data.sitAFT._dist <= data.sitCUR._dist)
            {
                result += 3;
            }

            if (data.sitCUR._dist >= 30f && data.sitCUR._doing.vecX != 0 && data.sitCUR._doing.vecY != 0
                && data.sitAFT._closer == false && data.sitAFT._dist > data.sitCUR._dist)
            {
                result -= 3;
            }

            if (data.sitCUR._dist <= 30f && data.sitCUR._doing.vecX == 0 && data.sitCUR._doing.vecY == 0
                && data.sitAFT._closer == false && data.sitAFT._dist <= data.sitCUR._dist)
            {
                result -= 3;
            }

            if (data.sitCUR._dist <= 30f && data.sitCUR._doing.vecX != 0 && data.sitCUR._doing.vecY != 0
                && data.sitAFT._closer == true && data.sitAFT._dist > data.sitCUR._dist)
            {
                result += 3;
            }
        }

        //아무것도 안 했으면 감점
        if (data.sitCUR._doing.vecX == 0)
        {
            result -= 10;
            if (data.sitCUR._doing.vecY == 0 && data.sitCUR._doing.vecZ == 0)
                result -= 100;
        }


        return result;
    }

    //alpha == 0.97 && gamma == 0.9 (gamma 값은 변경될 수도 있음)
    //result = (1 - alpha) * beforeQResult + alpha * (curReward + gamma * NextGreedyReward);
    //curReward는 직접 받아와야
    //NextGreedyReward는 
    //beforeQResult는?
    /*
     * depth: 탐색할 최대 깊이
     * index: 현재 깊이
     * beforeQ: 직전 Q값
     * Q_aiData: GreedyReward와 curReward 값을 추적하기 위한 AIData
    */
    private double Q_Func(int depth, int index, double beforeQ, AIData Q_aiData)
    {
        double result = 0;

        AIData nextGreedyData = aiDatasGreedy[sortANDSearch.Search_AIData(Q_aiData.sitAFT._dist, Q_aiData.sitAFT._angleComp, aiDatasGreedy)];
        double nextGreedyReward = CalScore(false, nextGreedyData);

        result = (1 - sampleAlpha) * beforeQ + sampleAlpha * (Q_aiData.sitAFT._posX + sampleGamma * nextGreedyReward);
        //Debug.Log(index + ": " + result + "<-" + beforeQ + "," + Q_aiData.sitAFT._beforeDB + ", " + nextGreedyReward);

        if (depth - index >= 1)
            result = Q_Func(depth, ++index, result, nextGreedyData);

        return result;
    }

    void Update()
    {
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
    } 

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
            IO_SqlDB.WriteDB_CUR("behaveDataReinforce", listSitCUR);
            yield return null;
            IO_SqlDB.WriteDB_AFT("behaveDataReinforce", listSitAFT);
            yield return null;
        }

        Debug.Log("HP HIGH Data " + listSitCUR.Count + " Save Complete");
        yield break;
    }
}
