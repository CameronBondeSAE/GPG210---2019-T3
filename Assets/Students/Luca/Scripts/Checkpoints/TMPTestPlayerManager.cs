using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMPTestPlayerManager : PlayerManager
{
    
    
    // Start is called before the first frame update
    void Start()
    {
        playerInfos = new List<PlayerInfo>();
        
        LucaController lc = FindObjectOfType<LucaController>();
        
        PlayerInfo testPlayer1 = new PlayerInfo();
        testPlayer1.virtualCameraLayer = 9;
        testPlayer1.realCamera = Camera.main;
        testPlayer1.playerCharacterPossessable = lc.possessable;

        playerInfos.Add(testPlayer1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
