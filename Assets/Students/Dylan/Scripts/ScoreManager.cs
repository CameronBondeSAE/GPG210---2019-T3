using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    
    private List<int> playerPosition = new List<int>();
    //private CheckpointReachedPlayerData checkPointData;

    private  Text playerScore;

    private List<Text> playerScores = new List<Text>();

    private float scoreToTween = 1f;
    private float tweenDuration = 2f;
    
    public Text scoreText;

    private void Awake()
    {
        ScoreManagerInit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TweenScore(1);
        }

        scoreText.text = scoreToTween.ToString("F0");
    }

    public void ScoreManagerInit()
    {
        //make player scores the same size as player positions 

        scoreText.gameObject.SetActive(false);
    }
    
    public void TweenScore(float scoreToIncrease)
    {
        
        scoreText.transform.localScale = new Vector3(1, 1, 1);
        scoreText.gameObject.SetActive(true);
        DOTween.To(Getter, Setter,scoreToIncrease,tweenDuration).OnComplete(MakeTextTransparent);
    }

    private void Setter(float value)
    {
        scoreToTween = value;

        scoreText.transform.localScale = new Vector3(value, value, 1);
    }

    private float Getter()
    {
        return scoreToTween;
    }

    private void MakeTextTransparent()
    {
        scoreText.gameObject.SetActive(false);
    }

    public void FinalScoreBoard()
    {
        SortScoreBoard();
        for (int i = 0; i < playerPosition.Count; i++)
        {
            playerScores[i].text = playerPosition[i].ToString();
        }
    }
    
    
    private void SortScoreBoard()
    {
        playerPosition.Sort(SortList);
    }
    private int SortList(int a, int b)
    {
        return a.CompareTo(b);
    }
    /*
    private GameObject CreateText()
    {
        GameObject scoreText = new GameObject("ScoreText");
        scoreText.transform.SetParent(this.transform);
        scoreText .GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        return scoreText;
    }*/

}
