using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonScript : MonoBehaviour {

    public void GoToGameStage(string name)
    {
        int index = -1;

        if (name == "Stage 1") index = 0;
        else if (name == "Stage 2") index = 1;
        else if (name == "Stage 3") index = 2;
        else index = -2;

        int isLocked = 0;

        if (index >= 0)
            isLocked = GameObject.FindWithTag("Player").GetComponent<Player_Info_Manager>().__GET_playerInfo._GET_stageIsLocked[index];
        else
            isLocked = 3;

        //스테이지 잠김
        if (isLocked == 0) Debug.Log("잠긴 스테이지");
        //스테이지 열림
        else if (isLocked == 1)
        {
            SceneManager.LoadScene(name);
            Debug.Log("플레이 가능한 스테이지");
        }
        //이미 클리어한 스테이지
        else if (isLocked == 2)
        {
            SceneManager.LoadScene(name);
            Debug.Log("이미 클리어한 스테이지");
        }
        else
        {
            //SceneManager.LoadScene(name);
        }
    }

	public void GoToStage(string name)
    {
        SceneManager.LoadScene(name);
    }

    // Resume Game
    public void Resume()
    {
        Time.timeScale = 1.0f;
    }

    // Pause Game
    public void Pause()
    {
        Time.timeScale = 0.0f;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
