using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{

    public Text targetScore;

    public GameObject startUIPrefab;
    
    public void Init()
    {
        //gets target score from gamemanager
        //targetScore = 
        
        
    }

    private void StartGame()
    {
        
    }

    private void SpawnUI()
    {
        startUIPrefab.gameObject.SetActive(true);
    }
    
    
}
