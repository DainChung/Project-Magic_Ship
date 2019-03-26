using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonScript : MonoBehaviour {

	public void GoToStage(string name)
    {
        SceneManager.LoadScene(name);
    }
}
