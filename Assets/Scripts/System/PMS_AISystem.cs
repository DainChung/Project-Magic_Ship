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

    public class FCNN
    {
        //layer의 weight값들
        public List<Matrix> layers = new List<Matrix>();
        //각 layer에서의 grad
        List<List<double>> grad = new List<List<double>>();
        //NeuralNetwork의 최종 output
        public List<double> output = new List<double>();
        //hiddenLayer들의 output값들
        List<Matrix> layerOut = new List<Matrix>();
        public List<double> outputInfo = new List<double>();

        public int depth = 0;
        public double learningRate = 0.0;
        public int inputNum, outputNum;

        public FCNN()
        { }

        public FCNN(FCNN fcnn)
        {
            this.depth = fcnn.depth;
            this.learningRate = fcnn.learningRate;
            this.inputNum = fcnn.inputNum;
            this.outputNum = fcnn.outputNum;

            this.layers = fcnn.layers;
            this.outputInfo = fcnn.outputInfo;
            this.output = fcnn.output;
        }

        //depth >= 3으로 가정
        //일단 FCNN
        public FCNN(int layerDepth, int inputNum, int outputNum, double learningRate)
        {
            this.depth = layerDepth;
            this.learningRate = learningRate;
            this.inputNum = inputNum;
            this.outputNum = outputNum;

            //bia 포함
            inputNum++;

            //inputLayer
            layers.Add(new Matrix(1, inputNum - 1, false));
            //bia
            layers[0].values[0][inputNum - 2] = 1.0;

            //hiddenLayers=======================================================
            //첫번째 hiddenLayer
            layers.Add(new Matrix(inputNum, outputNum, true));
            layerOut.Add(new Matrix(1, outputNum, false));

            for (int j = 0; j < layers[layers.Count - 1].col; j++)
                layers[layers.Count - 1].values[inputNum - 1][j] = 1.0;

            grad.Add(new List<double>());
            for (int k = 0; k < layers[layers.Count - 1].col; k++)
            {
                grad[grad.Count - 1].Add(0.0);
            }

            //2 + n 번째 hiddenLayer
            for (int i = 2; i <= layerDepth - 2; i++)
            {
                layers.Add(new Matrix(outputNum, outputNum, true));
                layerOut.Add(new Matrix(1, outputNum, false));
                for (int j = 0; j < layers[i].col; j++)
                    layers[i].values[outputNum - 1][j] = 1.0;
                grad.Add(new List<double>());
                for (int k = 0; k < layers[i].col; k++)
                {
                    grad[grad.Count - 1].Add(0.0);
                }
            }
            //Debug.Log(grad[grad.Count - 1].Count);
            //=================================================================

            //outputLayer
            layers.Add(new Matrix(outputNum, outputNum, true));
            grad.Add(new List<double>());

            for (int j = 0; j < outputNum; j++)
            {
                layers[layerDepth - 1].values[outputNum - 1][j] = 1.0;
                grad[grad.Count - 1].Add(0.0);
                output.Add(0.0);
                outputInfo.Add(0);
            }
        }

        public void ForwardProp()
        {
            Matrix result = layers[0];

            for (int i = 1; i < layers.Count; i++)
            {
                result = RELU(result.Multiply(layers[i]));

                if (i <= layerOut.Count)
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
            //grad
            for (int j = depth - 3; j >= 0; j--)
            {
                //Debug.Log(j + ": " + grad[j].Count);
                //Debug.Log((j + 1) + ": " + layers[j + 1].values.Count + ", " + layers[j + 1].values[0].Count);
                //Debug.Log((j+1) + ": " + grad[j+1].Count);
                //FCNN 으로 수행
                grad[j] = layers[j + 1].Multiply(grad[j + 1]);

                for (int k = 0; k < grad[j].Count; k++)
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

        //input 36개(bia 제외), output 36개일 때만 작동
        public void SetInput(float angle, float dist, float time)
        {
            if (inputNum == 36 && outputNum == 36)
            {
                layers[0].values[0] = SetInputValue(angle, dist, time);
            }
            else
                Debug.Log(angle + ", "+dist + ", "+time+" Error");
        }

        //input 36개, output 36개일 때만 정상 작동
        List<double> SetInputValue(float angle, float dist, float time)
        {
            List<double> result = new List<double>();

            double newAngle = (angle + 180.0) * 0.01;
            double newDist = dist * 0.1;
            double newTime = time;

            result.Add(newAngle);
            result.Add(newDist);
            result.Add(newTime);

            result.Add(newAngle * newAngle);
            result.Add(newDist * newDist);
            result.Add(newTime * newTime);

            result.Add(newAngle * newDist);
            result.Add(newDist * newTime);
            result.Add(newTime * newAngle);

            for (int i = 9; i < 36; i++)
                result.Add(0.0);

            return result;
        }

        public List<double> RefineTarget(List<double> target)
        {
            List<double> result = new List<double>();

            for (int i = 0; i < target.Count; i++)
            {
                result.Add(target[i]);
                if (result[i] < 0)
                {
                    result[i] *= (-1);
                    outputInfo[i] = -1;
                }
                else if (result[i] > 40)
                {
                    outputInfo[i] = result[i] - 40;
                    result[i] = 40;
                }
                else if (target[i] == 0)
                {
                    outputInfo[i] = -2;
                }
            }

            return result;
        }

        public int GetMaxQIndex()
        {
            int maxQindex = -1;
            double maxQ = -999.0f;

            for (int i = 0; i < outputNum; i++)
            {
                if (outputInfo[i] == -1)
                    output[i] *= -1;
                else if (outputInfo[i] > 0)
                    output[i] += outputInfo[i];

                if (maxQ < output[i])
                {
                    maxQ = output[i];
                    maxQindex = i;
                }
            }

            return maxQindex;
        }

        public double GetMaxQ()
        {
            double maxQ = -999.0f;

            for (int i = 0; i < outputNum; i++)
            {
                if (outputInfo[i] == -1)
                    output[i] *= -1;
                else if (outputInfo[i] > 0)
                    output[i] += outputInfo[i];

                if (maxQ < output[i])
                    maxQ = output[i];
            }

            return maxQ;
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

            double val = 0.0;

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
                        val = (double)((r + 1) * (c + 1));

                        while (val >= 1)
                        {
                            val *= 0.1;
                        }

                        values[r].Add(val);
                        values[r][c] /= 10;
                        //values[r].Add((double)(UnityEngine.Random.Range(0.0f, 1.0f)));
                    }
                }
            }
        }

        public Matrix()
        {}

        public void CopyMatrix(Matrix newMat)
        {
            if (this.row != newMat.row || this.col != newMat.col)
            {
                Debug.Log("Matrix Copy ERROR");
            }
            else
            {
                for (int r = 0; r < this.row; r++)
                {
                    for (int c = 0; c < this.col; c++)
                    {
                        this.values[r][c] = newMat.values[r][c];
                    }
                }
            }
        }

        public void CopyMatrix(List<List<double>> newMat)
        {
            try
            {
                for (int r = 0; r < this.row; r++)
                {
                    for (int c = 0; c < this.col; c++)
                    {
                        this.values[r][c] = newMat[r][c];
                    }
                }
            }
            catch (Exception)
            {
                Debug.Log("Matrix Copy Error, List");
            }
        }

        double mul(Matrix other, int col)
        {
            double result = 0.0;

            for (int c = 0; c < this.col; c++)
                result += this.values[0][c] * other.values[c][col];  
                         
            return result;
        }

        double mul(List<double> other, int col)
        {
            double result = 0.0;

            for (int r = 0; r < this.row; r++)
                result += this.values[r][col] * other[col];

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

            for (int c = 0; c < this.col; c++)
            {
                result.Add(0.0);
                result[c] = mul(other, c);
            }

            return result;
        }

        public void DebugMat()
        {
            for (int r = 0; r < this.row; r++)
                for (int c = 0; c < this.col; c++)
                    Debug.Log(r + ", " + c + " : " + this.values[r][c]);
        }

        public void DebugMat(int row)
        {
            for (int c = 0; c < this.col; c++)
                Debug.Log(row + ", " + c + " : " + this.values[row][c]);
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
