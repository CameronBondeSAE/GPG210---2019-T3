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

    public bool initplayers = false;
    
    public bool player1Join = false;
    public bool player1JoinCheckValue = false;
    
    public bool player2Join = false;
    public bool player2JoinCheckValue = false;

    private PlayerInfo p1Info;
    private PlayerInfo p2Info;
    
    public bool Player1Join
    {
        get => player1Join;
        set
        {
            player1Join = value;
            player1JoinCheckValue = value;
            if(value)
                NotifyNewPlayerJoinedGame(p1Info);
            else
                NotifyPlayerLeftGame(p1Info);
        }
    }
    public bool Player2Join
    {
        get => player2Join;
        set
        {
            player2Join = value;
            player2JoinCheckValue = value;
            if(value)
                NotifyNewPlayerJoinedGame(p2Info);
            else
                NotifyPlayerLeftGame(p2Info);
        }
    }

    private void OnEnable()
    {
        /*if (!initialized)
        {
            Init();
            initialized = true;
        }*/
    }

    // Start is called before the first frame update
    void Start()
    {
        /*if (!initialized)
        {
            Init();
            initialized = true;
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            Player1Join = !Player1Join;
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            Player2Join = !Player2Join;
        }
        
        if (initplayers && !initialized)
        {
            Init();
            initialized = true;
        }
    }

    private void OnValidate()
    {
        /*if (player1Join != player1JoinCheckValue)
            Player1Join = player1Join;
        if (player2Join != player2JoinCheckValue)
            Player2Join = player2Join;*/
    }

    private void Init()
    {
        playerInfos = new List<PlayerInfo>();
        
        LucaController lc = FindObjectOfType<LucaController>();
        
        p1Info = new PlayerInfo
        {
            realCamera = p1Cam,
            playerCharacterPossessable = p1Poss,
            virtualCameraLayer = LayerMask.NameToLayer("Player1"),
            controller = p1Poss.GetComponent<Controller>()
        };
        p1Info.realCamera.cullingMask =
            GameHelper.AddLayerToMask(p1Info.realCamera.cullingMask, p1Info.virtualCameraLayer);
        //lc.possessable;

        playerInfos.Add(p1Info);

        p2Info = new PlayerInfo
        {
            realCamera = p2Cam,
            playerCharacterPossessable = p2Poss,
            virtualCameraLayer = LayerMask.NameToLayer("Player2"),
            controller = p2Poss.GetComponent<Controller>()
        };
        p2Info.realCamera.cullingMask =
            GameHelper.AddLayerToMask(p2Info.realCamera.cullingMask, p2Info.virtualCameraLayer);

        playerInfos.Add(p2Info);
        
        Debug.Log("P1 Mask: "+p1Info.virtualCameraLayer+" "+p1Info.realCamera.name+" P2 Mask: "+p2Info.virtualCameraLayer+" "+p2Info.realCamera.name);
    }
}
