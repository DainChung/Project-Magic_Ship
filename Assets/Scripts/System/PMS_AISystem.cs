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

    //public class NeuronNetwork
    //{
    //    public Matrix inputLayer;
    //    public List<Matrix> layers = new List<Matrix>();
    //    public List<Matrix> layersOutput = new List<Matrix>();

    //    public int outputLayerIndex = 0;
    //    List<double> grad = new List<double>();

    //    int depth;

    //    //numOFInput은 2 이상의 양수가 권장됨, depth는 3이상이 권장됨 (depth == 3이면 for문 작동 안 함)
    //    //input 중 하나는 bia로 사용해야 됨, inputLayer를 제외한 각 Layer에서 bia를 고려해서 지정된 값을 넣어줘야 됨(ex) numOFInput == 2이면 -2)
    //    public NeuronNetwork(int depth, int numOFInput, int numOFOutput)
    //    {
    //        if (depth >= 3)
    //        {
    //            this.depth = depth;

    //            //하나는 bia
    //            inputLayer = new Matrix(1, numOFInput, false);

    //            layers.Add(new Matrix(numOFInput, depth * numOFInput, true));
    //            for (int i = 1; i < depth - 2; i++)
    //                layers.Add(new Matrix(depth * numOFInput, depth * numOFInput, true));
    //            layers.Add(new Matrix(depth * numOFInput, depth * (numOFInput - 1), true));

    //            layers.Add(new Matrix(depth * (numOFInput - 1), numOFOutput, true));
    //            outputLayerIndex = layers.Count - 1;

    //            for (int j = 0; j < numOFOutput; j++)
    //                grad.Add(0.0);

    //            for (int k = 0; k < layers.Count; k++)
    //                layersOutput.Add(new Matrix(1, layers[k].col, false));
    //        }
    //        else
    //        {
    //            Debug.Log("Depth는 3 이상의 양수만 허용");
    //        }
    //    }

    //    public void ForwardProp(List<double> inputVal)
    //    {
    //        Matrix result = inputLayer;

    //        if(inputLayer.values.Count == inputVal.Count)
    //            inputLayer.values = inputVal;

    //        try {
    //            for (int i = 0; i < this.depth; i++)
    //            {
    //                //행렬 곱셈
    //                result = result.Mul(layers[i]);
    //                //ELU(행렬 곱 결과)
    //                result.values = ELU(result.values);
    //                layersOutput[i] = result;
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            Debug.Log("ERROR");
    //        }
    //    }

    //    List<double> ELU(List<double> vals)
    //    {
    //        for (int i = 0; i < vals.Count; i++)
    //            vals[i] = vals[i] > 0.0 ? vals[i] : 0.01 * (Mathf.Exp((float)(vals[i])) - 1);

    //        return vals;
    //    }

    //    List<double> GradELU(List<double> vals)
    //    {
    //        for (int i = 0; i < vals.Count; i++)
    //            vals[i] = vals[i] > 0.0 ? 1 : 0.01 * Mathf.Exp((float)(vals[i]));

    //        return vals;
    //    }

    //    //weight 값이 대부분 1로 고정되는 알 수 없는 현상 발생
    //    public void ADAM(List<double> target)
    //    {
    //        ADAM(target, layers.Count - 1);
    //        //Debug.Log("ADAM");
    //        //layers[layers.Count - 1].DebugMat();

    //        for (int i = layers.Count - 2; i > -1; i--)
    //            ADAM(layersOutput[i].values, i); 

    //    }

    //    void ADAM(List<double> target, int layerNum)
    //    {
    //        List<double> gradELU = GradELU(layers[layers.Count - 1].values);

    //        try
    //        {
    //            for (int i = 0; i < grad.Count; i++)
    //            {
    //                grad[i] = (layers[layerNum].values[i] - target[i]) * gradELU[i];

    //                layers[layerNum].m[i] = 0.9 * layers[layerNum].m[i] + 0.1 * grad[i];
    //                layers[layerNum].v[i] = 0.999 * layers[layerNum].v[i] + 0.001 * grad[i] * grad[i];
    //            }


    //            int j = 0;
    //            Debug.Log("B: " + layerNum);
    //            //output 건들면 안 됨
    //            for (j = 0; j < layers[layerNum].values.Count - 1; j++)
    //            {
    //                layers[layerNum].values[j] -= 0.1 * layers[layerNum].m[j] / (Mathf.Sqrt((float)(layers[layerNum].v[j])) + 0.00000001);
    //            }
    //            Debug.Log("C: " + layerNum);
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.Log(e);
    //        }
    //    }
    //}

    public class NeuralNetwork
    {
        //layer의 weight값들
        public List<Matrix> layers = new List<Matrix>();
        //각 layer에서의 grad
        List<List<double>> grad = new List<List<double>>();
        //NeuralNetwork의 최종 output
        public List<double> output = new List<double>();
        //hiddenLayer들의 output값들
        List<Matrix> layerOut = new List<Matrix>();

        int depth = 0;
        public double learningRate = 0.0;

        //depth >= 3으로 가정
        //일단 FCNN
        public NeuralNetwork(int layerDepth, int inputNum, int outputNum, double learningRate)
        {
            this.depth = layerDepth;
            this.learningRate = learningRate;

            //bia 포함
            inputNum++;

            //inputLayer
            layers.Add(new Matrix(1, inputNum - 1, false));
            //bia
            layers[0].values[0][inputNum - 2] = 1.0;

            //hiddenLayers
            for (int i = 1; i < layerDepth - 1; i++)
            {
                layers.Add(new Matrix(inputNum, inputNum - 1, true));
                layerOut.Add(new Matrix(1, inputNum - 1, false));

                for (int j = 0; j < inputNum - 1; j++)
                    layers[i].values[inputNum - 1][j] = 1.0;
                
                grad.Add(new List<double>());
                for (int k = 0; k < inputNum; k++)
                {
                    grad[grad.Count - 1].Add(0.0);
                }
            }

            //outputLayer
            layers.Add(new Matrix(inputNum, outputNum, true));
            grad.Add(new List<double>());
            for (int j = 0; j < outputNum; j++)
            {
                layers[layerDepth - 1].values[inputNum - 1][j] = 1.0;
                grad[grad.Count - 1].Add(0.0);
                output.Add(0.0);
            }
        }

        public void ForwardProp()
        {
            Matrix result = layers[0];

            for (int i = 1; i < layers.Count; i++)
            {
                result = RELU(result.Multiply(layers[i]));

                if(i <= layerOut.Count)
                    layerOut[i - 1] = result;
                //bia
                result.values[result.values.Count - 1].Add(1.0);
            }

            for (int c = 0; c < result.col; c++)
                output[c] = result.values[0][c];
        }

        Matrix RELU(Matrix inputMat)
        {
            Matrix result = inputMat;

            for (int r = 0; r < result.row; r++)
            {
                for (int c = 0; c < result.col; c++)
                {
                    result.values[r][c] = result.values[r][c] > 0.0 ? result.values[r][c] : 0;
                }
            }

            return result;
        }

        double SlopeRELU(double inputVal)
        {
            return inputVal > 0.0 ? 1 : 0;
        }

        public void BackProp(List<double> target)
        {
            //Debug.Log("BackProp 0");
            //Output Layer의 grad 구하기
            for (int i = 0; i < target.Count; i++)
            {
                grad[depth - 2][i] = (target[i] - output[i]) * SlopeRELU(output[i]);
            }

            //Debug.Log("BackProp 1");
            //HiddenLayer의 grad 구하기
            for (int j = depth - 3; j >= 0; j--)
            {
                grad[j] = layers[j + 1].Multiply(grad[j + 1]);

                for (int k = 0; k < layers[j].col - 1; k++)
                {
                    grad[j][k] = grad[j][k] * SlopeRELU(layerOut[j].values[0][k]);
                }
            }

            //Debug.Log("BackProp 2");
            //weight값 적용
            for (int l = depth - 1; l >= 1; l--)
                GradDes(l);
        }

        void GradDes(int index)
        {
            List<double> outVal = new List<double>();

            if (index == depth - 1)
                outVal = output;
            else
                outVal = layerOut[index - 1].values[0];

            //Debug.Log("GradDes 0");
            for (int r = 0; r < layers[index].row; r++)
            {
                for (int c = 0; c < layers[index].col; c++)
                {
                    layers[index].values[r][c] += learningRate * grad[index - 1][c] * outVal[c];
                }
            }
        }

        public void SetInput(List<double> inputValues)
        {
            if (layers[0].values[0].Count == inputValues.Count)
                layers[0].values[0] = inputValues;
            else
                Debug.Log("Error");
        }
    }

    public class Matrix
    {
        public List<List<double>> values = new List<List<double>>();
        public int row, col;

        public Matrix(int row, int col, bool isRandomVal)
        {
            this.row = row;
            this.col = col;

            for (int r = 0; r < row; r++)
            {
                values.Add(new List<double>());
                for (int c = 0; c < col; c++)
                {
                    if (!isRandomVal)
                    {
                        values[r].Add(0.0);
                    }
                    else
                    {
                        values[r].Add((double)((r + 1) * (c+1)));
                        values[r][c] /= 10;
                        //values[r].Add((double)(UnityEngine.Random.Range(0.0f, 1.0f)));
                    }
                }
            }
        }

        public Matrix()
        {}

        double mul(Matrix other, int col)
        {
            double result = 0.0;

            for (int c = 0; c < this.col; c++)
                result += this.values[0][c] * other.values[c][col];  
                         
            return result;
        }

        double mul(List<double> other, int row)
        {
            double result = 0.0;

            for (int c = 0; c < this.col; c++)
                result += this.values[row][c] * other[c];

            return result;
        }

        public Matrix Multiply(Matrix other)
        {
            Matrix result = new Matrix(this.row, other.col, false);

            for (int c = 0; c < result.col; c++)
                    result.values[0][c] += mul(other, c);

            return result;
        }

        public List<double> Multiply(List<double> other)
        {
            List<double> result = new List<double>();

            for (int r = 0; r < this.row; r++)
            {
                result.Add(0.0);
                result[r] = mul(other, r);
            }

            return result;
        }

        public void DebugMat()
        {
            for (int r = 0; r < this.row; r++)
                for (int c = 0; c < this.col; c++)
                    Debug.Log(r+", "+c+" : "+this.values[r][c]);
        }
    }

    public class Neuron
    {
        public List<double> inputs = new List<double>();
        public List<double> weights = new List<double>();
        public double output = 0.0;
        public double bia = 1.0;

        double m = 0.0, v = 0.0;

        public Neuron(int inputNUM)
        {
            int i = 0;

            while (i < inputNUM)
            {
                this.inputs.Add(0.0);
                this.weights.Add((double)(UnityEngine.Random.Range(0.0f, 1.0f)));
                    
                i++;
            }
        }

        public void FeedForward()
        {
            for (int i = 0; i < this.inputs.Count; i++)
                output += (inputs[i] * weights[i] - bia);
            
            output = output > 0.0 ? output : 0.01 * (Mathf.Exp((float)(output)) - 1);
        }

        //public void ADAM(double target, List<Neuron> allNeurons)
        //{
        //    double grad = (output - target) * Grad_ELU();

        //    m = 0.9 * m + 0.1 * grad;
        //    v = 0.999 * v + 0.001 * grad * grad;

        //    weights[0] -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * dist * 0.001;
        //    //angle의 경우 최대 3자리 수인 관계로 절대값을 줄여줘야 됨(음수일 경우도 고려 필요)
        //    //angle 값을 축소해서 곱해주면 제대로 수렴, 그냥 곱해주면 무조건 -0.01로 수렴하려고 함, 음수 또는 양수 인 경우 모두 적합함
        //    weights[1] -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * angle * 0.002;
        //    weights[2] -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * time * 0.0005;

        //    bia -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001);
        //}

        //public void ADAM(double target)
        //{
        //    double grad = (output - target) * Grad_ELU();

        //    m = 0.9 * m + 0.1 * grad;
        //    v = 0.999 * v + 0.001 * grad * grad;

        //    //hiddenLayer의 output값들을 먼저 변경시키고 weight에 대한 값 조정을 할 것
        //    //단, inputLayer의 input값들은 변경하지 않도록 해야함
        //    //doM -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * doM;
        //    //doR -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * doR;
        //    //doA -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * doA;

        //    //dist = 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * dist * 0.001;
        //    //angle -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * angle * 0.0002;
        //    //time -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * time * 0.0005;

        //    //weights[0] -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * doM * 0.001;
        //    //weights[1] -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * doR * 0.001;
        //    //weights[2] -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * doA * 0.001;

        //    weights[0] -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * dist * 0.001;
        //    //angle의 경우 최대 3자리 수인 관계로 절대값을 줄여줘야 됨(음수일 경우도 고려 필요)
        //    //angle 값을 축소해서 곱해주면 제대로 수렴, 그냥 곱해주면 무조건 -0.01로 수렴하려고 함, 음수 또는 양수 인 경우 모두 적합함
        //    weights[1] -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * angle * 0.002;
        //    weights[2] -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001) * time * 0.0005;

        //    bia -= 0.1 * m / (Mathf.Sqrt((float)(v)) + 0.00000001);
        //}

        //public double Get_ELU()
        //{
        //    double result = dist * weights[0] + angle * weights[1] + time * weights[2] - 3 * bia;

        //    return result > 0.0 ? result : 0.01 * (Mathf.Exp((float)(result)) - 1);
        //}

        //double Grad_ELU()
        //{
        //    return output > 0.0 ? 1.0 : 0.01 * Mathf.Exp((float)(output));
        //}
    }
}
