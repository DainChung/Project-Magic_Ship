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

    public class Matrix
    {
        public List<double> values = new List<double>();
        public int row, col;

        public Matrix(int r, int c, bool doRandomInit)
        {
            row = r;
            col = c;

            for (int i = 0; i < row * col; i++)
            {
                if (doRandomInit) { values.Add((double)(UnityEngine.Random.Range(0.0f, 1.0f) * 0.01)); }
                else { values.Add(0.0); }
            }
        }

        public void SetValue(double val)
        {
            for (int i = 0; i < values.Count; i++)
                values[i] = val;
        }

        // [0,0]부터 세는 것을 전제로 함
        public void Set(int r, int c, double val)
        {
            values[this.col * r + c] = val;
        }

        public void Set(List<double> val)
        {
            values = val;
        }

        double mul(int _r, int _c, Matrix other)
        {
            double result = 0.0;

            
            for (int i = 0; i < this.col; i++)
                result += this.values[this.col * _r + i] * other.values[other.col * i + _c];

            return result;
        }

        public Matrix Mul(Matrix other)
        {
            Matrix result = new Matrix(this.row, other.col, false);

            for (int r = 0; r < result.row; r++)
                for (int c = 0; c < result.col; c++)
                    result.values[result.col * r + c] += mul(r, c, other);

            return result;
        }

        public void DebugMat()
        {  
            for (int i = 0; i < this.row; i++)
                for(int  j = 0; j < this.col; j++)
                    Debug.Log("("+i+ ", " + j+")=>" + (this.col * i + j) + " : " + values[this.col * i + j]);
        }
    }

    public class NeuronNetwork
    {
        public Matrix inputLayer;
        public List<Matrix> layers = new List<Matrix>();
        public List<Matrix> layersOutput = new List<Matrix>();

        public int outputLayerIndex = 0;
        List<double> grad = new List<double>();

        int depth;

        //numOFInput은 2 이상의 양수가 권장됨, depth는 3이상이 권장됨 (depth == 3이면 for문 작동 안 함)
        //input 중 하나는 bia로 사용해야 됨, inputLayer를 제외한 각 Layer에서 bia를 고려해서 지정된 값을 넣어줘야 됨(ex) numOFInput == 2이면 -2)
        public NeuronNetwork(int depth, int numOFInput, int numOFOutput)
        {
            if (depth >= 3)
            {
                this.depth = depth;

                //하나는 bia
                inputLayer = new Matrix(1, numOFInput, false);

                layers.Add(new Matrix(numOFInput, depth * numOFInput, true));
                for (int i = 1; i < depth - 2; i++)
                    layers.Add(new Matrix(depth * numOFInput, depth * numOFInput, true));
                layers.Add(new Matrix(depth * numOFInput, depth * (numOFInput - 1), true));

                layers.Add(new Matrix(depth * (numOFInput - 1), numOFOutput, true));
                outputLayerIndex = layers.Count - 1;

                for (int j = 0; j < numOFOutput; j++)
                    grad.Add(0.0);

                for (int k = 0; k < layers.Count; k++)
                    layersOutput.Add(new Matrix(1, layers[k].col, false));
            }
            else
            {
                Debug.Log("Depth는 3 이상의 양수만 허용");
            }
        }

        public void ForwardProp(List<double> inputVal)
        {
            Matrix result = inputLayer;

            if(inputLayer.values.Count == inputVal.Count)
                inputLayer.values = inputVal;

            try {
                for (int i = 0; i < this.depth; i++)
                {
                    //행렬 곱셈
                    result = result.Mul(layers[i]);
                    //ELU(행렬 곱 결과)
                    result.values = ELU(result.values);
                    layersOutput[i] = result;
                }
            }
            catch (Exception)
            {
                Debug.Log("ERROR");
            }
        }

        List<double> ELU(List<double> vals)
        {
            for (int i = 0; i < vals.Count; i++)
                vals[i] = vals[i] > 0.0 ? vals[i] : 0.01 * (Mathf.Exp((float)(vals[i])) - 1);

            return vals;
        }

        List<double> GradELU(List<double> vals)
        {
            for (int i = 0; i < vals.Count; i++)
                vals[i] = vals[i] > 0.0 ? 1 : 0.01 * Mathf.Exp((float)(vals[i]));

            return vals;
        }

        public void ADAM(List<double> target)
        {
            ADAM(target, layers.Count - 1);
            //layers[layers.Count - 1].DebugMat();

            for (int i = layers.Count - 2; i >= 0; i--)
                ADAM(layersOutput[i].values, i); 

        }

        void ADAM(List<double> target, int layerNum)
        {
            List<double> gradELU = GradELU(layers[layers.Count - 1].values);
            List<double> m = new List<double>();
            List<double> v = new List<double>();

            try
            {
                for (int i = 0; i < grad.Count; i++)
                {
                    m.Add(0.0);
                    v.Add(0.0);

                    grad[i] = (layers[layerNum].values[i] - target[i]) * gradELU[i];

                    m[i] = 0.9 * m[i] + 0.1 * grad[i];
                    v[i] = 0.999 * v[i] + 0.001 * grad[i] * grad[i];

                    //Debug.Log("ADAM: "+grad[i] + ", " + m[i] + ", " + v[i]);
                }


                int j = 0;

                for (j = 0; j < layers[layerNum].values.Count - 1; j++)
                {
                    layers[layerNum].values[j] -= 0.1 * m[j] / (Mathf.Sqrt((float)(v[j])) + 0.00000001) * layersOutput[layerNum].values[j] * 0.0001;
                    //Debug.Log(layerNum+": "+layers[layerNum].values[j] +", " + layersOutput[layerNum].values[j]);

                    if (layerNum >= 1)
                        layersOutput[layerNum - 1].values[j] -= 0.1 * m[j] / (Mathf.Sqrt((float)(v[j])) + 0.00000001) * layersOutput[layerNum].values[j] * 0.001;
                }

                //layers[layerNum].values[j] -= 0.1 * m[j] / (Mathf.Sqrt((float)(v[j])) + 0.00000001) * layersOutput[layerNum].values[j];
            }
            catch (Exception e)
            {
                //Debug.Log(e);
            }
        }
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
            double grad = (output - target) * Grad_ELU();

            m = 0.9 * m + 0.1 * grad;
            v = 0.999 * v + 0.001 * grad * grad;

            //hiddenLayer의 output값들을 먼저 변경시키고 weight에 대한 값 조정을 할 것
            //단, inputLayer의 input값들은 변경하지 않도록 해야함
            //doM -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * doM;
            //doR -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * doR;
            //doA -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * doA;

            //dist = 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * dist * 0.001;
            //angle -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * angle * 0.0002;
            //time -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * time * 0.0005;

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
            double result = doM * weights[0] + doR * weights[1] + doA * weights[2] + dist * weights[3] + angle * weights[4] + time * weights[5] - 6 * bia;

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
