using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    
    public List<int> playerPosition = new List<int>();
    //private CheckpointReachedPlayerData checkPointData;

    public Text playerScore;

    public List<Text> playerScores = new List<Text>();
    

    public void ScoreManagerInit()
    {
        //make player scores the same size as player positions 
        
    }
    private void Update()
    {
        

    }

    public void FinalScoreBoard()
    {
        SortScoreBoard();
        for (int i = 0; i < playerPosition.Count; i++)
        {
            playerScores[i].text = playerPosition[i].ToString();
        }
    }
    
    //TODO figure out why list isn't sorting properly
    private void SortScoreBoard()
    {
        playerPosition.Sort(SortList);
    }
    private int SortList(int a, int b)
    {
        return a.CompareTo(b);
    }

    private GameObject CreateText()
    {
        GameObject scoreText = new GameObject("ScoreText");
        scoreText.transform.SetParent(this.transform);
        scoreText .GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        return scoreText;
    }

}
