﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public GameObject battleRank;
    public GameObject battleRankImage;
    public GameObject[] results;
    public GameObject button;
    public GameObject Fade;

    public Sprite A;
    public Sprite B;
    public Sprite C;
    public Sprite D;

    public AudioClip drum;
    public AudioClip pecha;
    AudioSource soundPlayer;
    // Start is called before the first frame update
    void Start()
    {
        soundPlayer = GetComponent<AudioSource>();

        int i = 0;
        for(i = 0; i < results.Length; i++)
        {
            results[i].SetActive(false);
        }
        battleRank.SetActive(false);
        battleRankImage.SetActive(false);
        button.SetActive(false);

        StartCoroutine("ShowResult");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ShowResult()
    {
        Image I = Fade.GetComponent<Image>();
        float t = 0.0f;
        float speed = 0.5f;

        Fade.SetActive(true);
        while(t <= 1.0f)
        {
            I.color = Color.Lerp(Color.black, Color.clear, t);
            t += speed * Time.deltaTime;
            yield return null;
        }

        int i;
        float interval1 = 0.5f;
        float interval2 = 1.0f;

        soundPlayer.PlayOneShot(drum);

        yield return new WaitForSeconds(interval2);
        for(i = 0; i < results.Length; i++)
        {
            results[i].SetActive(true);
            yield return new WaitForSeconds(interval1);
        }

        yield return new WaitForSeconds(interval1);
        battleRank.SetActive(true);

        yield return new WaitForSeconds(interval2);
        int bs = GameManager.battleScore;
        if(bs >= 90) battleRankImage.GetComponent<Image>().sprite = A;
        else if(bs >= 70) battleRankImage.GetComponent<Image>().sprite = B;
        else if(bs >= 50) battleRankImage.GetComponent<Image>().sprite = C;
        else battleRankImage.GetComponent<Image>().sprite = D;
        battleRankImage.SetActive(true);
        soundPlayer.PlayOneShot(pecha);

        yield return new WaitForSeconds(interval2);
        button.SetActive(true);
    }
}
