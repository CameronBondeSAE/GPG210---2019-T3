using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    
    private readonly List<int> playerPosition = new List<int>();
    //private CheckpointReachedPlayerData checkPointData;

    //private Text playerScore;

    private List<Text> playerScores = new List<Text>();

    private int scoreTextSize = 2;
    private int tweenDuration = 3;
    
    private int testInt;
    
    public Text scoreText;

    private void Awake()
    {
        //Init();
        scoreText.gameObject.SetActive(false);
    }

    private void Update()
    {
        //was a test to ensure that the function would work
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //TweenScore(scoreTextSize);
            
            //when calling the function you need to pass in the score you wish to increase
            
        }

        //scoreText.text = "Score: " + scoreToTween.ToString("F0");
    }

    //called by gamemode
    //TODO still needs to find how many players are in and create the player position list accordingly
    public void Init(PlayerInfo playerInfo, GameModeBase gameModeBase)
    {
        //make player scores the same size as player positions 
        Canvas canvas = GetComponentInChildren<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = playerInfo.realCamera;
        canvas.planeDistance = 2f;
        gameModeBase.OnScoreChanged += UpdateUi;
        scoreText.text = "Score: " + playerInfo.score.ToString("F0");
        scoreText.gameObject.SetActive(false);
    }

    private void UpdateUi(PlayerInfo arg1, GameModeBase arg2)
    {
        TweenScore(arg1.score);
    }

    //when calling the function you need to pass in the score you wish to increase
    //its done like this so that when a player passes the checkpoint you could simply just
    //call this function from there or just have an event go off and have this go off as a response
    private void TweenScore(int playerScore)
    {
        scoreText.transform.localScale = new Vector3(1, 1, 1);
        scoreText.gameObject.SetActive(true);
        //the score increase is just a private variable at the top that equals 1
        //change it to increase how much score each checkpoint will give
        DOTween.To(Getter, Setter,scoreTextSize,tweenDuration).OnComplete(ResetText);
        scoreText.text = "Score: " + playerScore.ToString("F0");
    }

    private void Setter(int value)
    {
        scoreTextSize = value;

        scoreText.transform.localScale = new Vector3(value, value, 1);
        scoreText.fontSize *= value;
    }

    private int Getter()
    {
        return scoreTextSize;
    }

    private void ResetText()
    {
        scoreTextSize = 1;
        scoreText.fontSize = 17;
        //scoreText.gameObject.SetActive(false);
    }

    
    //this would be called as game finishes and places each into corresponding text fields
    /*
    public void FinalScoreBoard()
    {
        SortScoreBoard();
        for (int i = 0; i < playerPosition.Count; i++)
        {
            playerScores[i].text = playerPosition[i].ToString();
        }
    }
    */
    
    private void SortScoreBoard()
    {
        playerPosition.Sort(SortList);
    }
    private int SortList(int a, int b)
    {
        return a.CompareTo(b);
    }
    
    /*
    //this was a test to try and create text and de-spawn it but the do-tween works much better
    private GameObject CreateText()
    {
        GameObject scoreText = new GameObject("ScoreText");
        scoreText.transform.SetParent(this.transform);
        scoreText .GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        return scoreText;
    }*/

}
