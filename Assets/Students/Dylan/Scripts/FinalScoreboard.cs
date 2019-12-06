using System.Collections;
using System.Collections.Generic;
using Cam;
using UnityEngine;
using UnityEngine.UI;

public class FinalScoreboard : MonoBehaviour
{
    private PlayerManager playerManager;
    private GameModeManager gameModeManager;
    
    private List<int> playerPosition = new List<int>();
    
    public GameObject finalScoreboardPrefab;
    
    public List<Text> playerScores = new List<Text>();
    
    
    private void Awake()
    {
        //finalScoreboardPrefab.gameObject.SetActive(false);
        playerManager = FindObjectOfType<PlayerManager>();
        gameModeManager = FindObjectOfType<GameModeManager>();
        //sets up final score board to be called on game end
        gameModeManager.OnGameHasEnded += GameFinished;
    }
    
    private void GameFinished()
    {
        AddToScoreBoard();
        SpawnFinalScoreBoard();
        
    }

    private void SpawnFinalScoreBoard()
    {
        finalScoreboardPrefab.gameObject.SetActive(true);
        SortScoreBoard();
    }
    
    public void FinalScoreBoard()
    {
        for (int i = 0; i < playerPosition.Count; i++)
        {
            playerScores[i].text = "Player" + i + playerPosition[i].ToString("fo");
        }
        
    }

    private void AddToScoreBoard()
    {
        playerPosition.Clear();
        //adds all current players to player postions list to be sorted
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
        return a.CompareTo(b);
    }
}
