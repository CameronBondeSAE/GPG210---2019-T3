using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{

    public Text targetScore;

    public GameObject startUIPrefab;

    private GameModeBase gameModeBase;

    private void Awake()
    {
        //bad  
        gameModeBase = FindObjectOfType<GameModeBase>();
    }

    public void Init()
    {
        CheckHighScore();
    }

    private void CheckHighScore()
    {
        //gets target score 
        if (PlayerPrefs.HasKey("Highscore"))
        {
            targetScore.text = "Target Score: " + PlayerPrefs.GetInt("Highscore").ToString("F0");

        }
        else
        {
            //get from gamemode if there is no highscore
            //targetScore.text = "Target Score: " + 
        }
    }
    
    //set to a button on start score if you want to enable highscore reset
    public void ResetHighscore()
    {
        PlayerPrefs.DeleteAll();
        CheckHighScore();
    }

    private void SpawnUI()
    {
        startUIPrefab.gameObject.SetActive(true);
    }
    
    
    
    
}
