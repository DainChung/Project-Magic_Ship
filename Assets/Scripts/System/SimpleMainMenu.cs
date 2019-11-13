using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SimpleMainMenu : MonoBehaviour {

    public Text pressEnter;

    public GameObject[] buttonMain;

	// Use this for initialization
	void Start () {
        pressEnter.enabled = true;

        for (int i = 0; i < buttonMain.Length; i++)
        {
            buttonMain[i].GetComponent<Image>().enabled = false;
            buttonMain[i].GetComponent<Button>().enabled = false;
            buttonMain[i].GetComponentInChildren<Text>().enabled = false;
        }

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Return))
        {
            pressEnter.enabled = false;
            transform.GetComponent<AudioSource>().Play();

            for (int i = 0; i < buttonMain.Length; i++)
            {
                buttonMain[i].GetComponent<Image>().enabled = true;
                buttonMain[i].GetComponent<Button>().enabled = true;
                buttonMain[i].GetComponentInChildren<Text>().enabled = true;
            }
        }
	}
}
