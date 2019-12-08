using System.Collections;
using System.Collections.Generic;
using Cam;
using UnityEngine;
using UnityEngine.UI;

public class FinalScoreboard : MonoBehaviour
{
    private PlayerManager playerManager;
    private GameModeManager gameModeManager;
    private GameModeBase gameModeBase;
    
    private List<int> playerPosition = new List<int>();
    
    public GameObject finalScoreboardPrefab;
    
    public List<Text> playerScores = new List<Text>();
    
    
    private void Awake()
    {
        //finalScoreboardPrefab.gameObject.SetActive(false);
        //bad
        playerManager = FindObjectOfType<PlayerManager>();
        gameModeManager = FindObjectOfType<GameModeManager>();
        gameModeBase = FindObjectOfType<GameModeBase>();
        //sets up final score board to be called on game end
        gameModeManager.OnGameHasEnded += GameFinished;
        //TurnOffText();
    }

    //this was a test function to see if i could use a for loop to turn the gameobjects off
    //now instead it will be used in reverse to turn them on depending on the player amount
    //in the FinalScoreboard function
    private void TurnOffText()
    {
        for (var i = 0; i < playerScores.Count; i++)
        {
            playerScores[i].gameObject.SetActive(false);
        }
    }
    
    private void GameFinished()
    {
        AddToScoreBoard();
        SpawnFinalScoreBoard();
        
    }

    private void SpawnFinalScoreBoard()
    {
        SortScoreBoard(); 
        FinalScoreBoard();
        
        //checks the score against the highscore saved in the game mode base
        /*
        if (PlayerPrefs.SetInt("Highscore", playerPosition[0]) > gamemodebase.highScore)
        {
            PlayerPrefs.SetInt("Highscore", playerPosition[0]);
            PlayerPrefs.Save();

        }*/
        finalScoreboardPrefab.gameObject.SetActive(true);
    }
    
    private void FinalScoreBoard()
    {
        //as player scores are put into the text fields it turns on the corresponding game objects 
        for (int i = 0; i < playerPosition.Count; i++)
        {
            playerScores[i].text = "Player" + i + playerPosition[i].ToString("F0");
            playerScores[i].gameObject.SetActive(true);
        }
        
    }

    private void AddToScoreBoard()
    {
        //ensures the list is clear before adding to it
        playerPosition.Clear();
        //adds all current players to player positions list to be sorted
        for (int i = 0; i < playerManager.playerInfos.Count; i++)
        {
            playerPosition[i] = playerManager.playerInfos[i].score;
            
        }
        
    }
    
    private void SortScoreBoard()
    {
        playerPosition.Sort(SortList);
    }
    private int SortList(int a, int b)
    {
        //should sort the list but highest number
        return a.CompareTo(b);
    }
}
