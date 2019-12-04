using System;
using System.Collections;
using System.Collections.Generic;
using Students.Luca.Scripts.Checkpoints;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheckPointChicken : GameModeBase
{
    public PlayerManager playerManager;
    public GameObject fuelUIPrefab;
    public GameObject scoreUIPrefab;

    public Dictionary<PlayerInfo, ModePlayerInfo> modePlayerInfoLookup = new Dictionary<PlayerInfo, ModePlayerInfo>();

    FuelUI fuelUI;
    ScoreUI scoreUI;

    [Header("Checkpoint System Settings")]
    // Checkpoint related variables
    public CheckpointTrackBuilder cpTrackBuilder;
    public CheckpointManager cpManager;
    public GameObject checkpointPrefab;
    public Checkpoint testStartCheckpoint;
    public int numberOfCheckpointsToGenerateInAdvance = 2; // Hacky name
    public float timeInBetweenTrackCalculations = 1;

    public float cpBuilderMinConeAngle = 0;
    public float cpBuilderMaxConeAngle = 90;

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
        float heightAboveGround = testStartCheckpoint.GetComponent<MeshFilter>()?.sharedMesh.bounds.extents.y * testStartCheckpoint.transform.lossyScale.y ?? 8;
        cpTrackBuilder.CheckpointPrefab(checkpointPrefab).HeightAboveGround(heightAboveGround);
        
        // ======= Spawn some more checkpoints
        StartCoroutine(FindAndConnectNewCheckpoint(testStartCheckpoint, numberOfCheckpointsToGenerateInAdvance));
    }

    private void OnNewPlayerJoinedGame(PlayerInfo info)
    {
        scoreUI = Instantiate(scoreUIPrefab).GetComponent<ScoreUI>();
        scoreUI.GetComponent<ScoreUI>().Init(info, this);

        ModePlayerInfo modePlayerInfo = new ModePlayerInfo {scoreUI = scoreUI};
        modePlayerInfoLookup.Add(info, modePlayerInfo);
        
        info.playerVehicleInteraction.OnVehicleEntered += OnVehicleEntered;
        info.playerVehicleInteraction.OnVehicleExited += OnVehicleExited;
        cpManager.SetNextCheckpointTarget(testStartCheckpoint, info);

    }

    private void OnVehicleEntered(PlayerInfo info)
    {
        Fuel fuel = info.controller.possessable.GetComponent<Fuel>();

        if (fuel)
        {
            fuelUI = Instantiate(fuelUIPrefab).GetComponent<FuelUI>(); // TODO should be pooled really
            fuelUI.Init(info);

            modePlayerInfoLookup[info].fuelUI = fuelUI;
        }

    }

    private void OnVehicleExited(PlayerInfo info)
    {
        Destroy(modePlayerInfoLookup[info].fuelUI.gameObject); // TODO should be pooled really
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

        playercheckpointdata.playerInfo.score++;
        InvokeScoreChanged(playercheckpointdata.playerInfo, this);
        
        
        
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
        
        var chkpoint = cpTrackBuilder.GenCheckpointWithinRadius(startCheckpoint, cpBuilderMaxConeAngle, CheckpointTrackBuilder.RandomSpecifier, cpBuilderMinConeAngle);
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

public class ModePlayerInfo
{
    public FuelUI fuelUI;
    public ScoreUI scoreUI;
}
