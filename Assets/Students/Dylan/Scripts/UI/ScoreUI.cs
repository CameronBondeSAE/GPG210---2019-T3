using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    private int scoreTextSize = 2;
    private int tweenDuration = 3;
    
    public Text scoreText;

    //called by gamemode
    //TODO still needs to find how many players are in and create the player position list accordingly
    public void Init(PlayerInfo playerInfo, GameModeBase gameModeBase)
    {
        //sets position of each canvas 
        Canvas canvas = GetComponentInChildren<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = playerInfo.realCamera;
        canvas.planeDistance = 2f;
        //sets up the score ui to update on event call
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
        
        //scoreText.gameObject.SetActive(true);
        //the score increase is just a private variable at the top that equals 1
        //change it to increase how much score each checkpoint will give
        DOTween.To(Getter, Setter,scoreTextSize,tweenDuration).OnComplete(ResetText);
        scoreText.text = "Score: " + playerScore.ToString("F0");
    }

    //setter for do tween
    private void Setter(int value)
    {
        scoreTextSize = value;

        scoreText.transform.localScale = new Vector3(value, value, 1);
        //scoreText.fontSize += value;
    }

    //getter for dotween
    private int Getter()
    {
        return scoreTextSize;
    }

    private void ResetText()
    {
        scoreTextSize = 1;
        scoreText.fontSize = 30;
        scoreText.transform.localScale = new Vector3(1, 1, 1);
        //scoreText.gameObject.SetActive(false);
    }

    
    //this would be called as game finishes and places each into corresponding text fields
    
    
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
