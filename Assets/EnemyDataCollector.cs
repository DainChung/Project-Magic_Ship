using UnityEngine;
using System.Collections;

using System.Collections.Generic;

using PMS_AISystem;
using File_IO;

//한 스크립트에서 저장 총괄하기
public class EnemyDataCollector : MonoBehaviour {

    public List<SituationCUR> listSitCUR = new List<SituationCUR>();
    public List<SituationCUR> listSitCUR0 = new List<SituationCUR>();
    public List<SituationCUR> listSitCUR1 = new List<SituationCUR>();
    public List<SituationCUR> listSitCUR2 = new List<SituationCUR>();
    public List<SituationCUR> listSitCUR3 = new List<SituationCUR>();
    public List<SituationCUR> listSitCUR4 = new List<SituationCUR>();
    public List<SituationCUR> listSitCUR5 = new List<SituationCUR>();
    public List<SituationCUR> listSitCUR6 = new List<SituationCUR>();
    public List<SituationCUR> listSitCUR7 = new List<SituationCUR>();
    public List<SituationCUR> listSitCUR8 = new List<SituationCUR>();
    public List<SituationCUR> listSitCUR9 = new List<SituationCUR>();
    public List<SituationCUR> listSitCUR10 = new List<SituationCUR>();

    public List<SituationAFT> listSitAFT = new List<SituationAFT>();
    public List<SituationAFT> listSitAFT0 = new List<SituationAFT>();
    public List<SituationAFT> listSitAFT1 = new List<SituationAFT>();
    public List<SituationAFT> listSitAFT2 = new List<SituationAFT>();
    public List<SituationAFT> listSitAFT3 = new List<SituationAFT>();
    public List<SituationAFT> listSitAFT4 = new List<SituationAFT>();
    public List<SituationAFT> listSitAFT5 = new List<SituationAFT>();
    public List<SituationAFT> listSitAFT6 = new List<SituationAFT>();
    public List<SituationAFT> listSitAFT7 = new List<SituationAFT>();
    public List<SituationAFT> listSitAFT8 = new List<SituationAFT>();
    public List<SituationAFT> listSitAFT9 = new List<SituationAFT>();
    public List<SituationAFT> listSitAFT10 = new List<SituationAFT>();

    public List<AIData> aiDatas = new List<AIData>();

    public List<SituationCUR> goodBehaveList = new List<SituationCUR>();

    void swap(int a, int b)
    {
        SituationCUR t = goodBehaveList[a];
        goodBehaveList[a] = goodBehaveList[b];
        goodBehaveList[b] = t;
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
                index += (int)(length / (j*2));
            }
        }

        if ((int)(length/j) > 16)
        {
            if (goodBehaveList[index]._angleComp > angle)
            {
                j*=2;
                search(dist, angle, -index, length, j, ref result);
            }
            else if (goodBehaveList[index]._angleComp < angle)
            {
                j*=2;
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

            if (max > length - 1)   max = length - 1;
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

    public IntVector3 SearchGoodDoing(float dist, float angle)
    {
        IntVector3 result = new IntVector3(-1,-1,-1);
        int resultIndex = -1;

        search(dist, angle, goodBehaveList.Count, goodBehaveList.Count, 1, ref resultIndex);
        result = goodBehaveList[resultIndex]._doing;

        Debug.Log("CurD: "+ dist + ". CurA:"+ angle + ", DBD: "+ goodBehaveList[resultIndex]._dist + ", DBA: "+ goodBehaveList[resultIndex]._angleComp);

        return result;
    }

    void Awake()
    {
        //goodBehaveList = IO_SqlDB.ReadSitCUR_FROM_DB("behaveDataGoodATK");
        //quickSort(0, goodBehaveList.Count - 1);

        //for (int i = 0; i < goodBehaveList.Count; i++)
        //{
        //    Debug.Log(goodBehaveList[i]._angleComp);
        //}


        //aiDatas = IO_SqlDB.ReadAIData_FROM_DB("behaveData0");
        //aiDatas = IO_SqlDB.ReadAIData_FROM_DB("behaveData1");
        //aiDatas = IO_SqlDB.ReadAIData_FROM_DB("behaveData2");
        //aiDatas = IO_SqlDB.ReadAIData_FROM_DB("behaveData3");
        //aiDatas = IO_SqlDB.ReadAIData_FROM_DB("behaveData4");

        //for (int i = 0; i < aiDatas.Count; i++)
        //{
        //    if (aiDatas[i].sitAFT._hitCounter == 0)
        //    {
        //        aiDatas.RemoveAt(i);
        //        i--;
        //    }
        //}

        //if (aiDatas.Count != 0)
        //{
        //    for (int i = 0; i < aiDatas.Count; i++)
        //    {
        //        listSitCUR.Add(aiDatas[i].sitCUR);
        //        listSitAFT.Add(aiDatas[i].sitAFT);
        //    }

        //    IO_SqlDB.WriteDB_CUR("behaveDataGoodATK", listSitCUR);
        //    IO_SqlDB.WriteDB_AFT("behaveDataGoodATK", listSitAFT);
        //}
    }

    void Update()
    {
        //모두 저장
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(Saver());
        }
    }

    IEnumerator Saver()
    {
        IO_SqlDB.WriteDB_CUR("behaveData", listSitCUR0, 0);
        IO_SqlDB.WriteDB_AFT("behaveData", listSitAFT0, 0);

        yield return null;

        IO_SqlDB.WriteDB_CUR("behaveData", listSitCUR1, 1);
        IO_SqlDB.WriteDB_AFT("behaveData", listSitAFT1, 1);

        yield return null;

        IO_SqlDB.WriteDB_CUR("behaveData", listSitCUR2, 2);
        IO_SqlDB.WriteDB_AFT("behaveData", listSitAFT2, 2);

        yield return null;

        IO_SqlDB.WriteDB_CUR("behaveData", listSitCUR3, 3);
        IO_SqlDB.WriteDB_AFT("behaveData", listSitAFT3, 3);

        IO_SqlDB.WriteDB_CUR("behaveData", listSitCUR4, 4);
        IO_SqlDB.WriteDB_AFT("behaveData", listSitAFT4, 4);

        yield return null;

        IO_SqlDB.WriteDB_CUR("behaveData", listSitCUR5, 5);
        IO_SqlDB.WriteDB_AFT("behaveData", listSitAFT5, 5);

        IO_SqlDB.WriteDB_CUR("behaveData", listSitCUR6, 6);
        IO_SqlDB.WriteDB_AFT("behaveData", listSitAFT6, 6);

        yield return null;

        IO_SqlDB.WriteDB_CUR("behaveData", listSitCUR7, 7);
        IO_SqlDB.WriteDB_AFT("behaveData", listSitAFT7, 7);

        IO_SqlDB.WriteDB_CUR("behaveData", listSitCUR8, 8);
        IO_SqlDB.WriteDB_AFT("behaveData", listSitAFT8, 8);

        yield return null;

        IO_SqlDB.WriteDB_CUR("behaveData", listSitCUR9, 9);
        IO_SqlDB.WriteDB_AFT("behaveData", listSitAFT9, 9);

        IO_SqlDB.WriteDB_CUR("behaveData", listSitCUR10, 10);
        IO_SqlDB.WriteDB_AFT("behaveData", listSitAFT10, 10);

        yield return null;

        for (int i = 0; i < listSitCUR.Count; i++)
        {
            Debug.Log("CURIndex: "+i + "/" + (listSitCUR.Count - 1));
            IO_SqlDB.WriteDB_CUR("behaveData", listSitCUR[i]);

            Debug.Log("AFTIndex: " + i + "/" + (listSitAFT.Count - 1));
            IO_SqlDB.WriteDB_AFT("behaveData", listSitAFT[i]);

            yield return null;
        }

        InitAIDatas(listSitCUR0, listSitAFT0);
        InitAIDatas(listSitCUR1, listSitAFT1);
        InitAIDatas(listSitCUR2, listSitAFT2);
        InitAIDatas(listSitCUR3, listSitAFT3);
        InitAIDatas(listSitCUR4, listSitAFT4);

        yield return null;

        List<SituationCUR> cur = new List<SituationCUR>();
        List<SituationAFT> aft = new List<SituationAFT>();

        for (int i = 0; i < aiDatas.Count; i++)
        {
            cur.Add(aiDatas[i].sitCUR);
            aft.Add(aiDatas[i].sitAFT);
        }

        yield return null;

        IO_SqlDB.WriteDB_CUR("behaveDataGoodATK", cur);
        IO_SqlDB.WriteDB_AFT("behaveDataGoodATK", aft);

        Debug.Log("Data " + (listSitCUR0.Count+ listSitCUR1.Count+ listSitCUR2.Count+ listSitCUR3.Count+ listSitCUR4.Count+ listSitCUR5.Count+ listSitCUR6.Count+listSitCUR7.Count+ listSitCUR8.Count+ listSitCUR9.Count + listSitCUR10.Count+ listSitCUR.Count) + " Save Complete");
        yield break;
    }

    void InitAIDatas(List<SituationCUR> cur, List<SituationAFT> aft)
    {
        for (int i = 0; i < cur.Count; i++)
        {
            if (aft[i]._hitCounter != 0)
            {
                aiDatas.Add(new AIData(cur[i], aft[i]));
            }
        }
    }
}
