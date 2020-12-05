# Voxel Pirates (가칭 : Project-Magic_Ship)
DI &amp; CH

This is just for graduation

Do NOT use it for Commercial Purpose OR Graduation without our permission

## 역할분배

 - DainChung : [딥러닝 인공지능 구현 및 학습](https://github.com/DainChung/Project-Magic_Ship/blob/master/README.md#4-%EC%9D%B8%EA%B3%B5%EC%A7%80%EB%8A%A5-dainchung), 적 표시 UI, 유닛 시스템, 파일 IO, 캐릭터 모델링
 - rockatoo : 플레이어 상태바, 스킬 시스템 및 UI, 적 캐릭터 체력 바 UI

<br/><br/>

## 1. 개요

FCNN으로 학습한 2개의 인공지능 캐릭터를 상대하는 슈팅게임입니다.


 - 이름 : Voxel Pirates (가칭 : Project-Magic_Ship)
 - 장르 : 슈팅 게임
 - 기간 : 2019-01-20 ~ 2019-11-12 (발표)
 - 인원 : 2인 (프로그래머 2명)
 - 사용 프로그램 : Unity 5.4.5p5, MagicaVoxel 0.98.2(모델링)

<br/><br/>

## 2. 플레이 영상

[![Video Label](http://img.youtube.com/vi/PUOweO88Ll4/0.jpg)](https://youtu.be/PUOweO88Ll4)

[유튜브 링크](https://youtu.be/PUOweO88Ll4)

<br/><br/>

## 3. UI 구성

> 시작 화면

![Voxel_Pirates_0_시작](https://user-images.githubusercontent.com/11573611/101238784-d7df8600-3725-11eb-862d-725f5891acd4.png)

엔터키를 누르면 버튼이 나타납니다.

<br/><br/>

> 게임 화면

![Voxel Pirates 안내_SMALL](https://user-images.githubusercontent.com/11573611/100620273-920b7200-3361-11eb-8c00-f07a3b8fa76e.png)

 1) 플레이어 상태바 : 플레이어의 현재 체력, 마력을 표시합니다. 스킬별로 쿨타임을 확인할 수 있습니다.
 2) 현재 공격 방향 : 정면, 우측, 좌측 중 한 곳으로 포탄을 발사할 수 있습니다.
 3) 적 표시 UI : 적이 멀리 있으면 작게, 가까이 있으면 크게 표시됩니다.
 
> 결과화면

![Voxel_Pirates_1_결과](https://user-images.githubusercontent.com/11573611/101238786-d910b300-3725-11eb-8653-fc1fa47ef0c8.png)


 - 클리어 화면: 모든 적을 처치하면 점수와 시간이 출력됩니다.
 - 패배 화면: 플레이어 체력이 다 떨어지면 출력됩니다.

<br/><br/>

## 4. 인공지능 (DainChung)

![캡처](https://user-images.githubusercontent.com/11573611/100614192-1a394980-3359-11eb-8826-443dd2cea58d.PNG)

DQN을 사용하여 가장 Q값이 높은 행동을 찾아냅니다.
응답시간을 줄이기 위해 인공신경망이 판단한 결과를 FCNNed_QData.db파일에 저장했습니다.
게임 상의 인공지능 캐릭터는 DB파일을 사용하여 움직입니다.
<br/>
 - 학습목표 : 플레이어와 일정 거리를 유지하면서 플레이어에게 데미지를 줄수록 Q값이 높아집니다.
 - 인공신경망 입력값 : 벡터, 행동을 지속한 시간 등 인게임 데이터
 - 인공신경망 출력값 : 36가지 행동별 Q값
 <br/>
 
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
 <br/>
 
 > DQN 함수
 
 - DQN 함수 중 일부분 입니다.
 - 개발과 학습을 병행한 관계로 학습률(learningRate) 조정에 대한 이해가 부족해서 아래와 같이 구현하게 되었습니다.
 - net의 출력값과 실제 데이터 간의 평균제곱오차가 0.00001(enoughError) 이하가 되면 학습을 중지합니다.
 
 <pre>
 <code>        
            //무한 루프 방지
            for(int i= 0; i < 10000; i++)
            {
                //FCNN net의 출력값 계산
                net.ForwardProp();
                //net의 출력값과 실제 데이터 간의 평균제곱오차 계산
                error = GetError(net.output, target);

                //평균제곱오차에 따라 학습률 조정
                if (error > 10)
                    learningRate += pluslR;
                else if (error <= 10 && error > 1.0)
                    learningRate -= minuslR0;
                else if (error <= 1.0 && error > enoughERROR)
                    learningRate -= minuslR1;
                else { break; }
 
                //net의 학습률 업데이트 후 역전파
                net.learningRate = learningRate;
                net.BackProp(target);
            }
            net.ForwardProp();
 </code>
 </pre>

------------------------------------------------------------
## 출처
- UI 디자인, 폰트, UI 효과음, 배경화면 오브젝트 : [Kenny](https://www.kenney.nl/)
- 플레이어 상태바 UI : [OpenGameArt ui-pieces](https://opengameart.org/content/ui-pieces)
- 적 캐릭터 체력 바 UI : [OpenGameArt sleek-bars](https://opengameart.org/content/sleek-bars)
- 그 외 출처는 [Others Source.txt](https://github.com/DainChung/Project-Magic_Ship/blob/master/Others%20Source.txt)에 기록되어 있습니다.
