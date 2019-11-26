using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardUI : MonoBehaviour
{
    
    public List<int> playerPosition = new List<int>();
    //private CheckpointReachedPlayerData checkPointData;

    public Text playerPos1Text;

    [SerializeField] private int playerPos1 = 3;
    [SerializeField] private int playerPos2 = 2;

    private void Start()
    {
        
        playerPosition.Add(playerPos1);
        playerPosition.Add(playerPos2);
    }

    private void Update()
    {
        SortLeaderBoard();
        for (int i = 0; i < playerPosition.Count; i++)
        {
            Debug.Log(playerPosition[i]);
        }

    }
    
    //TODO figure out why list isn't sorting properly
    private void SortLeaderBoard()
    {
        playerPosition.Sort(SortList);
    }
    private int SortList(int a, int b)
    {
        return a.CompareTo(b);
    }

}
