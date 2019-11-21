using System;
using System.Collections;
using System.Collections.Generic;
using Students.Luca.Scripts.Helper;
using UnityEngine;

public class TMPTestPlayerManager : PlayerManager
{
    public Possessable p1Poss;
    public Camera p1Cam;
    public Possessable p2Poss;
    public Camera p2Cam;

    public bool initialized = false;

    private void OnEnable()
    {
        if (!initialized)
        {
            Init();
            initialized = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!initialized)
        {
            Init();
            initialized = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Init()
    {
        playerInfos = new List<PlayerInfo>();
        
        LucaController lc = FindObjectOfType<LucaController>();
        
        var testPlayer1 = new PlayerInfo
        {
            realCamera = p1Cam,
            playerCharacterPossessable = p1Poss,
            virtualCameraLayer = LayerMask.NameToLayer("Player1"),
            controller = p1Poss.GetComponent<Controller>()
        };
        testPlayer1.realCamera.cullingMask =
            GameHelper.AddLayerToMask(testPlayer1.realCamera.cullingMask, testPlayer1.virtualCameraLayer);
        //lc.possessable;

        playerInfos.Add(testPlayer1);

        var testPlayer2 = new PlayerInfo
        {
            realCamera = p2Cam,
            playerCharacterPossessable = p2Poss,
            virtualCameraLayer = LayerMask.NameToLayer("Player2"),
            controller = p2Poss.GetComponent<Controller>()
        };
        testPlayer2.realCamera.cullingMask =
            GameHelper.AddLayerToMask(testPlayer2.realCamera.cullingMask, testPlayer2.virtualCameraLayer);

        playerInfos.Add(testPlayer2);
        
        Debug.Log("P1 Mask: "+testPlayer1.virtualCameraLayer+" "+testPlayer1.realCamera.name+" P2 Mask: "+testPlayer2.virtualCameraLayer+" "+testPlayer2.realCamera.name);
    }
}
