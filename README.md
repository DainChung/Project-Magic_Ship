# Voxel Pirates (가칭 : Project-Magic_Ship)
DI &amp; CH

This is just for graduation

Do NOT use it for Commercial Purpose OR Graduation without our permission

## 역할분배

 - DainChung : [딥러닝 인공지능 구현 및 학습](https://github.com/DainChung/Project-Magic_Ship/blob/master/README.md#4-%EC%9D%B8%EA%B3%B5%EC%A7%80%EB%8A%A5-dainchung), 적 표시 UI, 유닛 시스템, 파일 IO, 캐릭터 모델링
 - rockatoo : 플레이어 상태바, 스킬 시스템 및 UI, 적 캐릭터 체력 바 UI

## 1. 개요

FCNN으로 학습한 2개의 인공지능 캐릭터를 상대하는 슈팅게임입니다.


 - 이름 : Voxel Pirates (가칭 : Project-Magic_Ship)
 - 장르 : 슈팅 게임
 - 기간 : 2019-01-20 ~ 2019-11-12 (발표)
 - 인원 : 2인 (프로그래머 2명)
 - 사용 프로그램 : Unity 5.4.5p5, MagicaVoxel 0.98.2(모델링)

## 2. 플레이 영상

[![Video Label](http://img.youtube.com/vi/PUOweO88Ll4/0.jpg)](https://youtu.be/PUOweO88Ll4)

[유튜브 링크](https://youtu.be/PUOweO88Ll4)

## 3. UI 구성

> 시작 화면



> 게임 화면

![그림2](https://user-images.githubusercontent.com/11573611/100609151-e0644500-3350-11eb-9e3e-1de25f5501b9.png)

 1) 플레이어 상태바 : 플레이어의 현재 체력, 마력을 표시합니다.
 2) 현재 공격 방향 : 정면, 우측, 좌측 중 한 곳으로 포탄을 발사할 수 있습니다.
 3) 적 표시 UI : 적이 멀리 있으면 작게, 가까이 있으면 크게 표시됩니다.
 
> 클리어



> 패배



------------------------------------------------------------
## 4. 인공지능 (DainChung)

![캡처](https://user-images.githubusercontent.com/11573611/100614192-1a394980-3359-11eb-8826-443dd2cea58d.PNG)

DQN을 사용하여 가장 Q값이 높은 행동을 찾아냅니다.
응답시간을 줄이기 위해 인공신경망이 판단한 결과를 FCNNed_QData.db파일에 저장했습니다.
게임 상의 인공지능 캐릭터는 DB파일을 사용하여 움직입니다.

 - 학습목표 : 플레이어와 일정 거리를 유지하면서 플레이어에게 데미지를 줄수록 Q값이 높아집니다.
 - 인공신경망 입력값 : 벡터, 행동을 지속한 시간 등 인게임 데이터
 - 인공신경망 출력값 : 36가지 행동별 Q값
 
 > FCNN의 생성자
 <pre>
 <code>
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
                grad[grad.Count - 1].Add(0.0);

            //두번째 이후의 hiddenLayer
            for (int i = 2; i <= layerDepth - 2; i++)
            {
                layers.Add(new Matrix(outputNum, outputNum, true));
                layerOut.Add(new Matrix(1, outputNum, false));
                for (int j = 0; j < layers[i].col; j++)
                    layers[i].values[outputNum - 1][j] = 1.0;
                    
                grad.Add(new List<double>());
                for (int k = 0; k < layers[i].col; k++)
                    grad[grad.Count - 1].Add(0.0);
            }
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
 </code>
 </pre>
 
 > DQN
 
 DQN 함수 중 일부분입니다.

 <pre>
 <code>
       FCNN net = new FCNN(6, 36, 36, learningRate);
       
       (중략)
       
       //무한 루프 방지
       for(int i= 0; i < 10000; i++)
       {
            net.ForwardProp();
            error = GetError(net.output, target);

            //평균제곱오차 값에 따라 학습률을 변동시켜 학습속도를 높여줍니다.
            if (error > 10)
                 learningRate += pluslR;
            else if (error <= 10 && error > 1.0)
                 learningRate -= minuslR0;
            else if (error <= 1.0 && error > enoughERROR)
                 learningRate -= minuslR1;
            else { break; }

            net.learningRate = learningRate;
            net.BackProp(target);
       }
       net.ForwardProp();
 </code>
 </pre>
 
  - target : 학습에 사용된 데이터
  - pluslR : error 값이 아주 클 때, 학습률 증가량
  - minuslR0 : error 값이 클 때, 학습률 감소량
  - minuslR1 : error 값이 아주 작을 때, 학습률 감소량
  
------------------------------------------------------------
## 출처
- UI 디자인, 폰트, UI 효과음, 배경화면 오브젝트 : [Kenny](https://www.kenney.nl/)
- 플레이어 상태바 UI : [OpenGameArt ui-pieces](https://opengameart.org/content/ui-pieces)
- 적 캐릭터 체력 바 UI : [OpenGameArt sleek-bars](https://opengameart.org/content/sleek-bars)
- 그 외 출처는 [Others Source.txt](https://github.com/DainChung/Project-Magic_Ship/blob/master/Others%20Source.txt)에 기록되어 있습니다.
