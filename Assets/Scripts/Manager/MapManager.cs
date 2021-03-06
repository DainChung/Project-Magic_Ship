﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapManager : MonoBehaviour {

    public GameObject seaObject_DEEP;

    public int mapWidth;
    public int mapHeight;

    private int mapMax_X = 80;
    private int mapMax_Z = 80;

    //Scene 상에 남은 적의 수
    private int remainedEnemys = 0;
    private int remainedEnemySpawners = 0;

    private PlayerUI playerUI;

    private const float dist = 1.6f;

    public Image tutorial;
    public GameObject button_tutorialExit;
    public SpriteRenderer arrow;

    public Image dead;
    public Result result;

    public GameObject button_retry;
    public GameObject button_menu;

    double startTime, clearTime;

    void Awake()
    {
        mapMax_X = mapWidth;
        mapMax_Z = mapHeight;
    }

	// Use this for initialization
	void Start()
    {
        try
        {
            transform.GetComponent<ButtonScript>().Pause();
            GameObject.Find("SamplePlayer").GetComponent<PlayerController>().playerMustBeFreeze = true;
            arrow.enabled = false;
        }
        catch (System.Exception)
        {
            GameObject.Find("SamplePlayer").GetComponent<PlayerController>().playerMustBeFreeze = false;
        }

        Vector3 pos = new Vector3(0, 0, 0);

        playerUI = GameObject.Find("Main Camera").GetComponent<PlayerUI>();

        try
        {
            dead.enabled = false;

            result.ShowIt(false);

            button_retry.GetComponent<Button>().enabled = false;
            button_retry.GetComponent<Image>().enabled = false;
            button_retry.transform.GetChild(0).GetComponent<Text>().enabled = false;
            button_menu.GetComponent<Button>().enabled = false;
            button_menu.GetComponent<Image>().enabled = false;
            button_menu.transform.GetChild(0).GetComponent<Text>().enabled = false;
        }
        catch (System.Exception)
        { }
        //일정 거리마다 파도효과용 바다 블럭 배치
        for (int x = 0; x < mapMax_X; x++)
        {
            for (int z = 0; z < mapMax_Z; z++)
            {
                pos.Set(x * dist, Random.Range(-0.1f, 0.7f), z * dist);
                Instantiate(seaObject_DEEP, pos, seaObject_DEEP.transform.rotation);
            }
        }
    }

    public void ExitTutorial()
    {
        transform.GetComponent<ButtonScript>().Resume();
        GameObject.Find("SamplePlayer").GetComponent<PlayerController>().playerMustBeFreeze = false;

        tutorial.enabled = false;
        button_tutorialExit.GetComponent<Button>().enabled = false;
        button_tutorialExit.GetComponent<Image>().enabled = false;
        button_tutorialExit.GetComponent<AudioSource>().Play();
        arrow.enabled = true;

        startTime = System.DateTime.Now.Subtract(System.DateTime.MinValue).TotalSeconds;
    }

    public void __SET_remainedEnemys()
    {
        playerUI.SearchEnemys();

        remainedEnemySpawners = GameObject.FindGameObjectsWithTag("Spawner").Length;

        //남아있는 적의 수 업데이트
        remainedEnemys = playerUI.enemysNum;
        //파괴된 Enemys가 일정시간 동안 메모리 상에 남아있기 때문에 1만큼 빼줘야 한다.
        remainedEnemys--;
        if (playerUI.enemysNum != (remainedEnemys + 1)) { Debug.Log("잘못된 입력"); }

        //남아있는 Spawner와 적이 없으면 스테이지 클리어로 인정된다.
        if ((remainedEnemys <= 0) && (remainedEnemySpawners <= 0))
        {
            remainedEnemys = 0; remainedEnemySpawners = 0;  //Debug.Log("스테이지 클리어");
            clearTime = System.DateTime.Now.Subtract(System.DateTime.MinValue).TotalSeconds - startTime;

            try
            {
                GameObject.Find("SamplePlayer").GetComponent<PlayerController>().playerMustBeFreeze = true;

                clearTime = (int)(clearTime * 100)/100;

                result.score = (int)(3000 + GameObject.Find("SamplePlayer").GetComponent<PlayerController>().__PLY_Stat.__PUB__Health_Point - clearTime * 3);
                result.texts[2].text = clearTime.ToString();
                result.ShowIt(true);

                button_retry.GetComponent<Button>().enabled = true;
                button_retry.GetComponent<Image>().enabled = true;
                button_retry.transform.GetChild(0).GetComponent<Text>().enabled = true;
                button_menu.GetComponent<Button>().enabled = true;
                button_menu.GetComponent<Image>().enabled = true;
                button_menu.transform.GetChild(0).GetComponent<Text>().enabled = true;
            }
            catch (System.Exception)
            { }
        }
    }

    public void PlayerDead()
    {
        try
        {
            dead.enabled = true;
            GameObject.Find("SamplePlayer").GetComponent<PlayerController>().playerMustBeFreeze = true;

            button_retry.GetComponent<Button>().enabled = true;
            button_retry.GetComponent<Image>().enabled = true;
            button_retry.transform.GetChild(0).GetComponent<Text>().enabled = true;
            button_menu.GetComponent<Button>().enabled = true;
            button_menu.GetComponent<Image>().enabled = true;
            button_menu.transform.GetChild(0).GetComponent<Text>().enabled = true;
            //Debug.Log("실패");
        }
        catch (System.Exception)
        { }
    }
}
