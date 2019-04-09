using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonScript : MonoBehaviour {

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
}
