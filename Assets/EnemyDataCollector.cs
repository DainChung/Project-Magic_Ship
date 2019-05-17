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

    public int mod = 0;

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
                index += (int)(length / (j * 2));
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
            goodBehaveList = IO_SqlDB.ReadSitCUR_FROM_DB("behaveDataGoodATK");
            quickSort(0, goodBehaveList.Count - 1);
        }
        else if (mod == 1)
        {
            goodBehaveList = IO_SqlDB.ReadSitCUR_FROM_DB("behaveDataGreatATK");
            quickSort(0, goodBehaveList.Count - 1);
        }
        else if (mod == 2)
        {

            aiDatas = IO_SqlDB.ReadAIData_FROM_DB("behaveDataGoodATK");

            for (int i = 0; i < aiDatas.Count; i++)
            {
                if (aiDatas[i].sitCUR._doing.vecZ == 2 && aiDatas[i].sitAFT._angleComp < 0 && aiDatas[i].sitCUR._angleComp < 0)
                {
                    Debug.Log("2MM");
                }
                else if (aiDatas[i].sitCUR._doing.vecZ == 3 && aiDatas[i].sitAFT._angleComp > 0 && aiDatas[i].sitCUR._angleComp > 0)
                {
                    Debug.Log("3PP");
                }
                else if (aiDatas[i].sitCUR._doing.vecZ != 0 && (aiDatas[i].sitCUR._angleComp > 170f || aiDatas[i].sitCUR._angleComp < -170f) && (aiDatas[i].sitAFT._angleComp > 170f || aiDatas[i].sitAFT._angleComp < -170f))
                {
                    Debug.Log("Angle");
                }
                else
                { }
            }
        }
        else if (mod == 3)
        {
            GameObject[] enemys = GameObject.FindGameObjectsWithTag("SampleEnemy");

            for (int i = 0; i < enemys.Length; i++)
            {
                Destroy(enemys[i]);
            }

            goodBehaveList = IO_SqlDB.ReadSitCUR_FROM_DB("behaveDataGreatATK");
            aiDatas = IO_SqlDB.ReadAIData_FROM_DB("behaveDataGreatATK");

            quickSort(0, goodBehaveList.Count - 1);

            for (int i = 0; i < aiDatas.Count; i++)
            {
                if (aiDatas[i].sitAFT._id == "NULL")
                {
                    continue;
                }
                else
                {
                    listSitAFT.Add(aiDatas[i].sitAFT);
                }
            }

            List<TrackSameData> sameDatas = SearchSameData();

            if (sameDatas.Count == 0)
            {
                Debug.Log("DB is Clean");
            }
            else
            {
                Debug.Log("Cleaning DB Start");

                for (int i = 0; i < sameDatas.Count; i++)
                {
                    listSitAFT0.Add(new SituationAFT(sameDatas[i].id, -1f, -1f, -1f, 400f, "NULL", 0, (sameDatas[i].count + 1), false));
                }

                //중복되는 AFT 내용 발견시 병합
                for (int i = 0; i < listSitAFT.Count; i++)
                {
                    for (int j = 0; j < listSitAFT0.Count; j++)
                    {
                        if (listSitAFT[i]._id == listSitAFT0[j]._id)
                        {
                            listSitAFT[i]._hitCounter += (listSitAFT0[j]._hitCounter);
                            listSitAFT0.RemoveAt(j);
                            j--;
                        }
                    }
                }

                //미중복 사례와 중복 사례 통합
                for (int i = 0; i < listSitAFT0.Count; i++)
                {
                    listSitAFT.Add(listSitAFT0[i]);
                }


                IO_SqlDB.WriteDB_CUR("behaveDataGreatATK_TEMP", goodBehaveList);
                IO_SqlDB.WriteDB_AFT_FOR_GreatTEMP("behaveDataGreatATK_TEMP", listSitAFT);

                Debug.Log("Cleaning DB Complete");
            }
        }
        else
        { }
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
        //InitAIDatas(listSitCUR1, listSitAFT1);

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
        Debug.Log("Good Data: " + listSitCUR0.Count + " Save Complete");
        yield break;
    }

    void InitAIDatas(List<SituationCUR> cur, List<SituationAFT> aft)
    {
        for (int i = 0; i < cur.Count; i++)
        {
            if (cur[i]._doing.vecZ == 2 && cur[i]._angleComp < 0 && aft[i]._angleComp < 0)
            {
                aft[i]._hitCounter = 0;
                continue;
            }
            else if (cur[i]._doing.vecZ == 3 && cur[i]._angleComp > 0 && aft[i]._angleComp > 0)
            {
                aft[i]._hitCounter = 0;
                continue;
            }
            else if (cur[i]._doing.vecZ != 0 && (cur[i]._angleComp > 170f || cur[i]._angleComp < -170f) && (aft[i]._angleComp > 170f || aft[i]._angleComp < -170f))
            {
                Debug.Log("3");
                aft[i]._hitCounter = 0;
                continue;
            }
            else
            {
                if (aft[i]._hitCounter != 0 && cur[i]._doing.vecZ != 0)
                {
                    aiDatas.Add(new AIData(cur[i], aft[i]));
                }
            }
        }
    }
}
