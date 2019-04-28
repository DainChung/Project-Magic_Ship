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

    public List<SituationAFT> listSitAFT = new List<SituationAFT>();
    public List<SituationAFT> listSitAFT0 = new List<SituationAFT>();
    public List<SituationAFT> listSitAFT1 = new List<SituationAFT>();

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
        IntVector3 result = new IntVector3(-1, -1, -1);
        int resultIndex = -1;

        search(dist, angle, goodBehaveList.Count, goodBehaveList.Count, 1, ref resultIndex);
        result = goodBehaveList[resultIndex]._doing;

        //Debug.Log("CurD: "+ dist + ". CurA:"+ angle + ", DBD: "+ goodBehaveList[resultIndex]._dist + ", DBA: "+ goodBehaveList[resultIndex]._angleComp);

        return result;
    }

    public SituationCUR SearchGoodSitCUR(float dist, float angle)
    {
        SituationCUR result = new SituationCUR("NULL", -1f, -1, -1, -1, new IntVector3(-1,-1,-1), -1f);
        int resultIndex = -1;

        search(dist, angle, goodBehaveList.Count, goodBehaveList.Count, 1, ref resultIndex);
        result = goodBehaveList[resultIndex];

        //Debug.Log("CurD: "+ dist + ". CurA:"+ angle + ", DBD: "+ goodBehaveList[resultIndex]._dist + ", DBA: "+ goodBehaveList[resultIndex]._angleComp);

        return result;
    }

    void Awake()
    {
        goodBehaveList = IO_SqlDB.ReadSitCUR_FROM_DB("behaveDataGoodATK");
        quickSort(0, goodBehaveList.Count - 1);
    }

    void Update()
    {
        //모두 저장
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(Saver());
        }

        //BigData 유닛이 검증한 우수 행동 별도 저장
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(SaverGreat());
        }
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
        IO_SqlDB.WriteDB_CUR("behaveData", listSitCUR);
        yield return null;
        IO_SqlDB.WriteDB_AFT("behaveData", listSitAFT);
        yield return null;

        InitAIDatas(listSitCUR0, listSitAFT0);
        InitAIDatas(listSitCUR1, listSitAFT1);

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

        Debug.Log("Data " + listSitCUR.Count + " Save Complete");
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
