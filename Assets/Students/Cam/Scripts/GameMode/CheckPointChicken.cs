﻿using System.Collections;
using System.Collections.Generic;
using Students.Luca.Scripts.Checkpoints;
using UnityEngine;

public class CheckPointChicken : GameModeBase
{
    public PlayerManager playerManager;
    public GameObject fuelUIPrefab;
    
    GameObject fuelUI;
    
    [Header("Checkpoint System Settings")]
    // Checkpoint related variables
    public CheckpointTrackBuilder cpTrackBuilder;
    public CheckpointManager cpManager;
    public GameObject checkpointPrefab;
    public Checkpoint testStartCheckpoint;
    public int numberOfCheckpointsToGenerateInAdvance = 2; // Hacky name
    public float timeInBetweenTrackCalculations = 1;

    public override void Activate()
    {
        base.Activate();

        playerManager = FindObjectOfType<PlayerManager>();
        playerManager.OnNewPlayerJoinedGame += OnNewPlayerJoinedGame;
        
        // ======= Checkpoint System Setup
        if(cpTrackBuilder == null)
            cpTrackBuilder = CheckpointTrackBuilder.CreateInstance();
        if (cpManager == null)
            cpManager = FindObjectOfType<CheckpointManager>();

        cpManager.OnPlayerReachedCheckpoint += HandlePlayerReachedCheckpointEvent;
        cpManager.OnPlayerReachedLastCheckpoint += HandlePlayerReachedLastCheckpointEvent;
        
        // ======= Add sample Start Checkpoint Node
        testStartCheckpoint.transform.SetParent(cpManager.ActiveCheckpointTrack.transform);
        cpManager.ActiveCheckpointTrack.AddCheckpoint(testStartCheckpoint, true);
        
        // ======= Set Initial Checkpoint Target of already joined players
        playerManager?.playerInfos?.ForEach(pi =>
        {
            cpManager.SetNextCheckpointTarget(testStartCheckpoint, pi);
        });
            
        // ======= Setting up the Checkpoint Track Builder
        cpTrackBuilder.doDebug = true;
        cpTrackBuilder.maxItr = 100;
        float heightAboveGround = testStartCheckpoint.GetComponent<MeshFilter>()?.sharedMesh.bounds.extents.y ?? 8;
        cpTrackBuilder.CheckpointPrefab(checkpointPrefab).SlopeTestingSamples(60).StepHeight(3).MaxFallDistance(30).MaxSlope(60).HeightAboveGround(heightAboveGround);
        
        // ======= Spawn some more checkpoints
        StartCoroutine(FindAndConnectNewCheckpoint(testStartCheckpoint, numberOfCheckpointsToGenerateInAdvance));
    }

    private void OnNewPlayerJoinedGame(PlayerInfo playerinfo)
    {
        playerinfo.playerVehicleInteraction.OnVehicleEntered += OnVehicleEntered;
        playerinfo.playerVehicleInteraction.OnVehicleExited += OnVehicleExited;
        cpManager.SetNextCheckpointTarget(testStartCheckpoint, playerinfo);
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

    private void HandlePlayerReachedLastCheckpointEvent(CheckpointReachedPlayerData playercheckpointdata)
    {
        // TODO
    }

    /// <summary>
    /// Called when a player reached a checkpoint. Set new targets here (In case target setting is handled outside of the Checkpoint Manager).
    /// </summary>
    /// <param name="playercheckpointdata"></param>
    private void HandlePlayerReachedCheckpointEvent(CheckpointReachedPlayerData playercheckpointdata)
    {
        if(cpManager == null)
            return;
        
        // Set new targets
        cpManager.SetNextCheckpointTargets(playercheckpointdata.reachedCheckpoint.nextCheckpoints,
            playercheckpointdata.playerInfo);

        // Create new checkpoints
        playercheckpointdata.reachedCheckpoint.nextCheckpoints?.ForEach(checkpoint =>
        {
            StartCoroutine(FindAndConnectNewCheckpoint(checkpoint,numberOfCheckpointsToGenerateInAdvance));
        });
    }
    
    /// <summary>
    /// Hacky.
    /// Finds & creates new checkpoint locations.
    /// </summary>
    /// <param name="startCheckpoint">The start checkpoint.</param>
    /// <param name="depth">Defines depth of checkpoint tree to create. Recursively recalls this method and assigns the checkpoint to the earlier created one.</param>
    /// <returns></returns>
    IEnumerator FindAndConnectNewCheckpoint(Checkpoint startCheckpoint, int depth = 0)
    {
        if(cpTrackBuilder == null)
            yield break;
        
        var chkpoint = cpTrackBuilder.GenCheckpointWithinRadius(startCheckpoint, 90, CheckpointTrackBuilder.RandomSpecifier, 0);

        if (chkpoint == null)
            yield break;
            
        chkpoint.transform.SetParent(cpTrackBuilder.transform);
        startCheckpoint.nextCheckpoints.Add(chkpoint);
        cpManager.ActiveCheckpointTrack.AddCheckpoint(chkpoint);
            

        if (depth > 1)
        {
            if(timeInBetweenTrackCalculations > 0)
                yield return new WaitForSeconds(timeInBetweenTrackCalculations);
            else 
                yield return new WaitForEndOfFrame();
                
            StartCoroutine(FindAndConnectNewCheckpoint(chkpoint,depth-1));
        }
        else
        {
            cpManager.ActiveCheckpointTrack.SetDefaultCheckpointLayer(LayerMask.NameToLayer(cpManager.defaultCheckpointLayer)); // HACKY
        }

        yield return 0;
    }
}