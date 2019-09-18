using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System;

//해당 스크립트 폐기 가능성 존재
//번거롭게 새로운 클래스를 만드는 것보다 DB에 직접 값들을 저장하고 읽어오는 것이 유용할 수 있음
namespace PMS_AISystem
{

    public class IntVector3 {
        public int vecX;
        public int vecY;
        public int vecZ;

        public void InitIntVector3(int x, int y, int z)
        {
            vecX = x;
            vecY = y;
            vecZ = z;
        }

        public void InitIntVector3(IntVector3 input)
        {
            vecX = input.vecX;
            vecY = input.vecY;
            vecZ = input.vecZ;
        }

        public IntVector3(int x, int y, int z)
        {
            vecX = x;
            vecY = y;
            vecZ = z;
        }

        public IntVector3(IntVector3 vec)
        {
            vecX = vec.vecX;
            vecY = vec.vecY;
            vecZ = vec.vecZ;
        }

        public string IntVector3ToString()
        {
            string result = "(" + vecX + ", " + vecY + ", " + vecZ + ")";

            return result;
        }
    }

    public class Behave
    {
        public IntVector3 Doing;
        public float Time;

        public Behave(IntVector3 doing, float time)
        {
            Doing = doing;
            Time = time;
        }
    }

    public class SituationCUR : IDisposable
    {

        public string _id;

        public float _posX;
        public float _posZ;

        public float _dist;
        public float _angleComp;

        public IntVector3 _doing = new IntVector3(-1,-1,-1);

        public float _time;

        public SituationCUR(string id, float x, float z, float dist, float angleComp, IntVector3 doing, float time)
        {
            _id = id;
            _posX = x;
            _posZ = z;
            _dist = dist;
            _angleComp = angleComp;
            _doing = doing;
            _time = time;
        }

        public SituationCUR(string id, float dist, float angleComp, Behave behave)
        {
            _id = id;
            _posX = 0f;
            _posZ = 0f;
            _dist = dist;
            _angleComp = angleComp;
            _doing = behave.Doing;
            _time = behave.Time;
        }

        public SituationCUR()
        {
            _id = "NULL";
            _posX = -1f;
            _posZ = -1f;
            _dist = 0f;
            _angleComp = 0f;
            _doing = new IntVector3(-1, -1, -1);
            _time = -1f;
        }

        public SituationCUR(SituationCUR cur)
        {
            _id = cur._id;
            _posX = cur._posX;
            _posZ = cur._posZ;
            _dist = cur._dist;
            _angleComp = cur._angleComp;
            _doing = new IntVector3(cur._doing);
            _time = cur._time;
        }

        public void Set(string id, float x, float z, float dist, float angleComp, IntVector3 doing, float time)
        {
            _id = id;
            _posX = x;
            _posZ = z;
            _dist = dist;
            _angleComp = angleComp;
            _doing = doing;
            _time = time;
        }

        public void Set(SituationCUR cur)
        {
            _id = cur._id;
            _posX = cur._posX;
            _posZ = cur._posZ;
            _dist = cur._dist;
            _angleComp = cur._angleComp;
            _doing = cur._doing;
            _time = cur._time;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {}
    }

    public class SituationAFT : IDisposable
    {

        public string _id;

        public float _posX;
        public float _posZ;

        public float _dist;
        public float _angleComp;

        public string _beforeID;
        public int _beforeDB;

        public int _hitCounter;

        public bool _closer;

        public SituationAFT(string id, float x, float z, float dist, float angleComp, string beforeID, int beforeDB, int hitCounter, bool closer)
        {
            _id = id;
            _posX = x;
            _posZ = z;
            _dist = dist;
            _angleComp = angleComp;
            _beforeID = beforeID;
            _beforeDB = beforeDB;
            _hitCounter = hitCounter;
            _closer = closer;
        }

        public SituationAFT()
        {
            _id = "NULL";
            _posX = -1f;
            _posZ = -1f;
            _dist = -1f;
            _angleComp = -1f;
            _beforeID = "NULL";
            _beforeDB = -1;
            _hitCounter = -1;
            _closer = false;
        }

        public SituationAFT(SituationAFT aft)
        {
            _id = aft._id;
            _posX = aft._posX;
            _posZ = aft._posZ;
            _dist = aft._dist;
            _angleComp = aft._angleComp;
            _beforeID = aft._beforeID;
            _beforeDB = aft._beforeDB;
            _hitCounter = aft._hitCounter;
            _closer = aft._closer;
        }

        public void Set(string id, float x, float z, float dist, float angleComp, string beforeID, int beforeDB, int hitCounter, bool closer)
        {
            _id = id;
            _posX = x;
            _posZ = z;
            _dist = dist;
            _angleComp = angleComp;
            _beforeID = beforeID;
            _beforeDB = beforeDB;
            _hitCounter = hitCounter;
            _closer = closer;
        }

        public void Set(SituationAFT aft)
        {
            _id = aft._id;
            _posX = aft._posX;
            _posZ = aft._posZ;
            _dist = aft._dist;
            _angleComp = aft._angleComp;
            _beforeID = aft._beforeID;
            _beforeDB = aft._beforeDB;
            _hitCounter = aft._hitCounter;
            _closer = aft._closer;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        { }
    }

    public class AIData : IDisposable
    {
        bool disposed = false;

        public SituationCUR sitCUR;
        public SituationAFT sitAFT;

        public AIData(SituationCUR cur, SituationAFT aft)
        {
            sitCUR = cur;
            sitAFT = aft;
        }

        public AIData(AIData data)
        {
            sitCUR = new SituationCUR(data.sitCUR);
            sitAFT = new SituationAFT(data.sitAFT);
        }

        public AIData()
        {
            sitCUR = new SituationCUR("NULL", -1f, -1f, -1f, -1f, new IntVector3(-1,-1,-1), -1f);
            sitAFT = new SituationAFT("NULL", -1f, -1f, -1f, -1f, "NULL", -1, -1 , false);
        }

        public void Set(AIData data)
        {
            sitCUR.Set(data.sitCUR);
            sitAFT.Set(data.sitAFT);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                sitCUR.Dispose();
                sitAFT.Dispose();
            }
        }
    }

    public class AIDataList
    {
        public List<AIData> datas;

        public AIDataList()
        {
            datas = new List<AIData>();
        }
    }

    public class DeepAIData
    {
        public SituationCUR sitCUR;
        public List<SituationAFT> sitAFTList = new List<SituationAFT>();

        public DeepAIData(SituationCUR cur, List<SituationAFT> aft)
        {
            sitCUR = cur;
            sitAFTList = aft;
        }

        public DeepAIData(SituationCUR cur, SituationAFT aft)
        {
            sitCUR = cur;
            sitAFTList.Add(aft);
        }

        public DeepAIData(SituationAFT aft)
        {
            sitAFTList.Add(new SituationAFT(aft));
        }

        public DeepAIData(SituationAFT aft, int index)
        {
            sitAFTList[index] = aft;
        }

        public DeepAIData()
        {
            sitAFTList.Add(new SituationAFT());
        }

        public DeepAIData(AIData data)
        {
            sitCUR = new SituationCUR(data.sitCUR);
            sitAFTList.Add(new SituationAFT(data.sitAFT));
        }
    }

    public struct NeuronAIData
    {
        public IntVector3 doing;
        public float dist, angle, time;
        public int bia;
    }
 
    public class Neuron
    {
        //reward의 weight는 2.4
        //hitCounter의 weight는 5로 가정한다.
        //input은 reward와 hitcounter
        //bia는 aft.dist에 따라 결정된다.
        //alpha값은 0.01f

        public double[] weights = new double[6];

        public double doM, doR, doA, dist, angle, time, bia;

        public double output_PUB;
        bool isNegative = false;
        double output;

        double m = 0.0, v = 0.0;

        public Neuron(double wDM, double wDR, double wDA, double wD, double wA, double wT)
        {
            weights[0] = wDM;
            weights[1] = wDR;
            weights[2] = wDA;

            weights[3] = wD;
            weights[4] = wA;
            weights[5] = wT;
        }

        public Neuron(List<Neuron> neurons)
        {
            for (int i = 0; i < neurons.Count; i++)
            {
                weights[0] += neurons[i].weights[0];
                weights[1] += neurons[i].weights[1];
                weights[2] += neurons[i].weights[2];

                weights[3] += neurons[i].weights[3];
                weights[4] += neurons[i].weights[4];
                weights[5] += neurons[i].weights[5];
            }

            weights[0] /= neurons.Count;
            weights[1] /= neurons.Count;
            weights[2] /= neurons.Count;
            weights[3] /= neurons.Count;
            weights[4] /= neurons.Count;
            weights[5] /= neurons.Count;
        }

        public void NeuronLOG(int num)
        {
            if (num == 0)
                Debug.Log("Dist: " + output_PUB);
            else if (num == 1)
                Debug.Log("Angle: " + output_PUB);
            else if (num == 2)
                Debug.Log("Reward: " + output_PUB);

            Debug.Log("wDM: " + weights[0] + ", wDR: " + weights[1] + ", wDA: " + weights[2] + ", wD: " + weights[3] + ", wA: " + weights[4] + ", wT: " + weights[5]);
        }

        public double FF(IntVector3 _doing, double _dist, double _angle, double _time, double _bia)
        {
            doM = (double)(_doing.vecX);
            doR = (double)(_doing.vecY);
            doA = (double)(_doing.vecZ);

            dist = _dist;
            angle = _angle;
            time = _time;
            bia = _bia;

            output = Get_ELU();
            output_PUB = output;
            if (isNegative)
                output_PUB *= (-1);

            return output;
        }

        public void ADAM(double target)
        {
            if (target < 0)
            {
                target *= (-1);
                isNegative = true;
            }

            double grad = (output - target) * Grad_ELU();

            m = 0.9 * m + 0.1 * grad;
            v = 0.999 * v + 0.001 * grad * grad;

            weights[0] -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * doM;
            weights[1] -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * doR;
            weights[2] -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * doA;

            weights[3] -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * dist * 0.001;
            //angle의 경우 최대 3자리 수인 관계로 절대값을 줄여줘야 됨(음수일 경우도 고려 필요)
            //angle 값을 축소해서 곱해주면 제대로 수렴, 그냥 곱해주면 무조건 -0.01로 수렴하려고 함, 음수 또는 양수 인 경우 모두 적합함
            weights[4] -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * angle * 0.0002;
            weights[5] -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * time * 0.0005;

            bia -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001);
        }

        double _ADAM(double m, double v, double val)
        {
            return 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * val;
        }

        public double Get_ELU()
        {
            double result = doM * weights[0] + doR * weights[1] + doA * weights[2] +
                dist * weights[3] + angle * weights[4] + time * weights[5] - 6 * bia;

            return result > 0.0 ? result : 0.01 * (Mathf.Exp((float)(result)) - 1);
        }

        double Grad_ELU()
        {
            return output > 0.0 ? 1.0 : 0.01 * Mathf.Exp((float)(output));
        }

        public static float ELU(float reward, int hitCounter, int dist)
        {
            float result = reward * 2.4f + 5 * hitCounter - 0.02f * Mathf.Sqrt(dist);

            if (result <= 0)
                result = 0.01f * (Mathf.Exp(result) - 1);

            return result;
        }

        public static float Slope_OF_ELU(float reward, int hitCounter, int dist)
        {
            float result = reward * 2.4f + 5 * hitCounter - 0.02f * Mathf.Sqrt(dist);

            if (result <= 0)
                result = 0.01f * Mathf.Exp(result);
            else
                result = 1;

            return result;
        }
    }
}
