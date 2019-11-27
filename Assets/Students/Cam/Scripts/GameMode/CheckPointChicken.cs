using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointChicken : GameModeBase
{
    public PlayerManager playerManager;
    public GameObject fuelUIPrefab;
    
    GameObject fuelUI;

    public override void Activate()
    {
        base.Activate();

        playerManager = FindObjectOfType<PlayerManager>();
        playerManager.OnNewPlayerJoinedGame += OnNewPlayerJoinedGame;
    }

    private void OnNewPlayerJoinedGame(PlayerInfo playerinfo)
    {
        playerinfo.playerVehicleInteraction.OnVehicleEntered += OnVehicleEntered;
        playerinfo.playerVehicleInteraction.OnVehicleExited += OnVehicleExited;
    }

    private void OnVehicleEntered(PlayerInfo info)
    {
        fuelUI = Instantiate(fuelUIPrefab); // TODO should be pooled really
        fuelUI.GetComponentInChildren<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
    }

    private void OnVehicleExited(PlayerInfo info)
    {
        Destroy(fuelUIPrefab); // TODO should be pooled really
    }

    public override void StartGame()
    {
        base.StartGame();
        
    }
}
