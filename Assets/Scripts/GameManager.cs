﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject mainImage;
    public GameObject text;
    public GameObject headSet;
    public GameObject MoneyText;
    public GameObject Pannel1;
    public GameObject Pannel2;
    public GameObject Fade;
    public Sprite gameOverSpr;
    public Sprite gameClearSpr;
    /*public GameObject Life;
    public Sprite lifeZero;
    public Sprite lifeOne;
    public Sprite lifeTwo;
    public Sprite lifeThree;*/

    //Image lifeImage;
    Text message;
    Text money;
    public GameObject Boss;
    AudioSource soundPlayer;
    Animator hsAnimator;
    public AudioClip piron;


    public bool bossIsGoal = true;
    public bool hsAlwaysActive = true;
    bool goal = false;

    public static int score = 0;

    string messages;
    string nowMessages;
    string oldMessages;

    public static int battleScore = 100;
    public static int defeats = 0;
    public static int retry = 0;
    // Start is called before the first frame update
    void Start()
    {
        mainImage.SetActive(false);
        Pannel1.SetActive(false);
        Pannel2.SetActive(false);
        //lifeImage = Life.GetComponent<Image>();
        message = text.GetComponent<Text>();
        money = MoneyText.GetComponent<Text>();
        soundPlayer = headSet.GetComponent<AudioSource>();
        hsAnimator = headSet.GetComponent<Animator>();

        if(hsAlwaysActive == false)
        {
            headSet.SetActive(false);
        }

        nowMessages = PlayerController.messages;
        oldMessages = PlayerController.messages;

        UpdateScore();

        if(CheckPointManager.progress != 0)
        {
            GameObject[] cps = GameObject.FindGameObjectsWithTag("Respawn");
            int i = 0;
            bool done = false;
            while(i < cps.Length && !done)
            {
                int progress = cps[i].GetComponent<CheckPointManager>().individualNum;
                if(progress == CheckPointManager.progress) done = true;
                if(!done) i++;
            }
            Vector3 pos = cps[i].transform.position;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = pos;
        }
    }

    // Update is called once per frame
    void Update()
    { 
        /*if(PlayerController.life >= 3)
        {
            lifeImage.sprite = lifeThree;
        }
        else if(PlayerController.life == 2)
        {
            lifeImage.sprite = lifeTwo;
        }
        else if(PlayerController.life == 1)
        {
            lifeImage.sprite = lifeOne;
        }
        else
        {
            lifeImage.sprite = lifeZero;
        }*/

        if(PlayerController.score != 0)
        {
            score += PlayerController.score;
            PlayerController.score = 0;
            UpdateScore();
        }

        nowMessages = PlayerController.messages;
        if(nowMessages != oldMessages)
        {
            StartCoroutine("StoryTeller");
            oldMessages = nowMessages;
        }

        if(PlayerController.gameState == "gameover")
        {
            Pannel1.SetActive(true);
            mainImage.GetComponent<Image>().sprite = gameOverSpr;
            mainImage.SetActive(true);
        }
        if(bossIsGoal && !goal)
        {
            if(Boss == null)
            {
                goal = true;
                StartCoroutine("BossDefeated");
            }
        }
        if(PlayerController.gameState == "gameclear" && !bossIsGoal)
        {
            Pannel2.SetActive(true);
            mainImage.GetComponent<Image>().sprite = gameClearSpr;
            mainImage.SetActive(true);
            CheckPointManager.progress = 0;
            goal = true;
        }
    }

    void UpdateScore()
    {
        money.text = "¥" + score.ToString();
    }

    IEnumerator StoryTeller()
    {
        string recieved = nowMessages;
        string[] messages = recieved.Split(' ');
        int length = messages.Length;
        int i;
        for(i = 0; i < length; i++)
        {
            hsAnimator.SetTrigger("Call");
            message.text = messages[i];
            soundPlayer.PlayOneShot(piron);
            yield return new WaitForSeconds(messages[i].Length / 5.0f);
            if(recieved != nowMessages)
            {
                yield break;
            }
        }
        message.text = "";
    }

    IEnumerator BossDefeated()
    {
        Image I = Fade.GetComponent<Image>();
        GameObject NextButton = Pannel2.transform.Find("NextButton").gameObject;
        GameObject musicPlayer = GameObject.FindGameObjectWithTag("Music");
        float t = 0.0f;
        float speed = 0.5f;
        PlayerController.gameState = "gameclear";
        Fade.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        while(t <= 1.0f)
        {
            I.color = Color.Lerp(Color.clear, Color.black, t);
            t += speed * Time.deltaTime;
            yield return null;
        }
        if(musicPlayer != null && !NextButton.GetComponent<ChangeScene>().continueMusic)
        {
            float i;
            float downspeed = 0.01f;
            AudioSource auds = musicPlayer.GetComponent<AudioSource>();
            for(i = 1.0f; i >= 0.0f; i -= downspeed)
            {
                auds.volume = i;
                yield return null;
            }
        }
        yield return new WaitForSeconds(1.0f);
        NextButton.GetComponent<ChangeScene>().Load();
    }
}
