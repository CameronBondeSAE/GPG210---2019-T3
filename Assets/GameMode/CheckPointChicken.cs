using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Students.Luca.Scripts.Checkpoints;
using UnityEngine;

public class CheckPointChicken : GameModeBase
{
    public Dictionary<PlayerInfo, ModePlayerInfo> modePlayerInfoLookup = new Dictionary<PlayerInfo, ModePlayerInfo>();
    #region Variables
    [TitleGroup("Components")]
    public PlayerManager playerManager;
    public CheckpointManager checkpointManager;
    public CheckpointTrackBuilder cpTrackBuilder;

    #region Objective Variables
    [TitleGroup("Objectives")]
    [ToggleGroup("useObjTimeLimit", ToggleGroupTitle = "Time Limit"), SerializeField, ShowInInspector]
    private bool useObjTimeLimit;
    
    
    [ToggleGroup("useObjTimeLimit"), SerializeField, HideLabel, ShowInInspector, ValueDropdown("TimeLimitOptions", IsUniqueList = true), OnValueChanged("UpdateObjTimeLimit")]
    private float objTimeLimitSelection; // In Seconds
    [ToggleGroup("useObjTimeLimit"), SerializeField, ShowInInspector, MinValue(0), SuffixLabel("Seconds"), EnableIf("UpdateObjTimeLimit")]
    private float objTimeLimit; // In Seconds
    [ToggleGroup("useObjTimeLimit"), SerializeField, ShowInInspector, MinValue(1), Tooltip("The frequency how often to check if the timer reached 0.")]
    private float objCheckerInterval = 1;

    private float _objCheckerIntervalCooldown = 0;

    [DisplayAsString, ShowInInspector, DisableInEditorMode, HideInEditorMode,ToggleGroup("useObjTimeLimit")]
    public float TimeLeft => (objTimeLimit - Time.time + GameStartTime);// Odin Inspector Use Only

    
    // Odin Editor Use Only
    private bool UpdateObjTimeLimit()
    {
        var isCustom = objTimeLimitSelection < 0;
        if(!isCustom && objTimeLimit != objTimeLimitSelection)
            objTimeLimit = objTimeLimitSelection;

        return isCustom;
    }
    
    // Odin Editor Use Only
    private IEnumerable TimeLimitOptions = new ValueDropdownList<float>()
    {
        { "3 Minutes", 180 },
        { "5 Minutes", 300 },
        { "10 Minutes", 600 },
        { "30 Minutes", 1800 },
        { "45 Minutes", 2700 },
        { "1 Hour", 3600 },
        { "Custom", -1 }
    };
    
    [ToggleGroup("useObjScoreLimit", ToggleGroupTitle = "Score Limit"), SerializeField, ShowInInspector]
    private bool useObjScoreLimit;

    [ToggleGroup("useObjScoreLimit"), SerializeField, ShowInInspector, MinValue(0)]
    private int objScoreLimit;
    
    [ToggleGroup("useObjCheckpointLimit", ToggleGroupTitle = "Checkpoint Limit"), SerializeField, ShowInInspector]
    private bool useObjCheckpointLimit;

    [ToggleGroup("useObjCheckpointLimit"), SerializeField, ShowInInspector, MinValue(0)]
    private int objCheckpointLimit;

    #endregion
    
    [TitleGroup("User Interface")]
    [PreviewField]
    public GameObject fuelUIPrefab;
    [PreviewField]
    public GameObject scoreUIPrefab;
    
    
    [TitleGroup("Checkpoint System Settings")]
    [PreviewField]
    public GameObject checkpointPrefab;
    public Checkpoint startCheckpoint;
    [MinValue(0)]
    public int numberOfCheckpointsToGenerateInAdvance = 2;
    [MinValue(0.01)] // Hacky name
    public float timeInBetweenTrackCalculations = .1f;

    [Range(0,180)]
    public float cpBuilderMinConeAngle = 0;
    [Range(0,180)]
    public float cpBuilderMaxConeAngle = 90;

    
    FuelUI fuelUI;
    ScoreUI scoreUI;
    #endregion
    
    public override void Activate()
    {
        base.Activate();
      

        playerManager = FindObjectOfType<PlayerManager>();
        playerManager.OnNewPlayerJoinedGame += OnNewPlayerJoinedGame;
        
        // ======= Checkpoint System Setup
        if(cpTrackBuilder == null)
            cpTrackBuilder = CheckpointTrackBuilder.CreateInstance();
        if (checkpointManager == null)
            checkpointManager = FindObjectOfType<CheckpointManager>();

        checkpointManager.OnPlayerReachedCheckpoint += HandlePlayerReachedCheckpointEvent;
        checkpointManager.OnPlayerReachedLastCheckpoint += HandlePlayerReachedLastCheckpointEvent;
        
        // ======= Add sample Start Checkpoint Node
        startCheckpoint?.transform.SetParent(checkpointManager.ActiveCheckpointTrack.transform);
        checkpointManager.ActiveCheckpointTrack.AddCheckpoint(startCheckpoint, true);
        
        // ======= Set Initial Checkpoint Target of already joined players
        playerManager?.playerInfos?.ForEach(pi =>
        {
            checkpointManager.SetNextCheckpointTarget(startCheckpoint, pi);
        });
            
        // ======= Setting up the Checkpoint Track Builder
        cpTrackBuilder.doDebug = true;
        cpTrackBuilder.maxItr = 100;
        float heightAboveGround = startCheckpoint.GetComponent<MeshFilter>()?.sharedMesh.bounds.extents.y * startCheckpoint.transform.lossyScale.y ?? 8;
        cpTrackBuilder.CheckpointPrefab(checkpointPrefab).HeightAboveGround(heightAboveGround);
        
        // ======= Spawn some more checkpoints
        StartCoroutine(FindAndConnectNewCheckpoint(startCheckpoint, numberOfCheckpointsToGenerateInAdvance));
    }
    
    
    protected override void _HandleEndOfGameMode() => NotifyGameModeHasEnded(this);

    private void OnNewPlayerJoinedGame(PlayerInfo info)
    {
        scoreUI = Instantiate(scoreUIPrefab).GetComponent<ScoreUI>();
        scoreUI.GetComponent<ScoreUI>().Init(info, this);

        ModePlayerInfo modePlayerInfo = new ModePlayerInfo {scoreUI = scoreUI};
        modePlayerInfoLookup.Add(info, modePlayerInfo);
        
        info.playerVehicleInteraction.OnVehicleEntered += OnVehicleEntered;
        info.playerVehicleInteraction.OnVehicleExited += OnVehicleExited;
        checkpointManager.SetNextCheckpointTarget(startCheckpoint, info);

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

    private void Update()
    {
        if(!useObjTimeLimit)
            return;
        if (_objCheckerIntervalCooldown > 0)
        {
            _objCheckerIntervalCooldown -= Time.deltaTime;
        }
        else
        {
            if(CheckObjectives())
                GameModeActive = false;
            _objCheckerIntervalCooldown = objCheckerInterval;
        }
    }


    // Check whether a player met any of the activated objectives
    public override bool CheckObjectives()
    {
        if (useObjTimeLimit && TimeLeft <= 0)
            return true;
        
        foreach (var player in modePlayerInfoLookup.Keys)
        {
            if (useObjScoreLimit && player.score >= objScoreLimit)
                return true;

            if (useObjCheckpointLimit && (checkpointManager?.GetCurrentPlayerCheckpointData(player)?.GetReachedCheckpointsCount() ?? 0) >=
                objCheckpointLimit)
                return true;
        }


        return false;
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
        if(checkpointManager == null)
            return;

        playercheckpointdata.playerInfo.score++;
        InvokeScoreChanged(playercheckpointdata.playerInfo, this);
        
        // Validate Objectives
        if(CheckObjectives())
            GameModeActive = false;
        
        
        // Set new targets
        checkpointManager.SetNextCheckpointTargets(playercheckpointdata.reachedCheckpoint.nextCheckpoints,
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
        checkpointManager.ActiveCheckpointTrack.AddCheckpoint(chkpoint);
            

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
            checkpointManager.ActiveCheckpointTrack.SetDefaultCheckpointLayer(LayerMask.NameToLayer(checkpointManager.defaultCheckpointLayer)); // HACKY
        }

        yield return 0;
    }

    
}

public class ModePlayerInfo
{
    public FuelUI fuelUI;
    public ScoreUI scoreUI;
}
