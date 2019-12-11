using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Students.Luca.Scripts.Checkpoints;
using UnityEngine;
using Random = UnityEngine.Random;

public class CheckPointChicken : GameModeBase
{
    public Dictionary<PlayerInfo, ModePlayerInfo> modePlayerInfoLookup = new Dictionary<PlayerInfo, ModePlayerInfo>();

    #region Variables

    [TitleGroup("Components")] public PlayerManager playerManager;
    public CheckpointManager checkpointManager;
    public CheckpointTrackBuilder cpTrackBuilder;

    #region Objective Variables

    [TitleGroup("Objectives")]
    [ToggleGroup("Objectives/useObjTimeLimit", ToggleGroupTitle = "Time Limit"), SerializeField, ShowInInspector]
    private bool useObjTimeLimit;

    [ButtonGroup("Objectives/useObjTimeLimit/TimeLimitButtons", -1), Button(ButtonSizes.Small), LabelText("5 min")]
    private void SetTimeLimit5M() => objTimeLimit = 300;

    [ButtonGroup("Objectives/useObjTimeLimit/TimeLimitButtons"), LabelText("10 min")]
    private void SetTimeLimit10M() => objTimeLimit = 600;

    [ButtonGroup("Objectives/useObjTimeLimit/TimeLimitButtons"), LabelText("30 min")]
    private void SetTimeLimit30M() => objTimeLimit = 1800;

    [ButtonGroup("Objectives/useObjTimeLimit/TimeLimitButtons"), LabelText("45 min")]
    private void SetTimeLimit45M() => objTimeLimit = 2700;

    [ButtonGroup("Objectives/useObjTimeLimit/TimeLimitButtons"), LabelText("1 hour")]
    private void SetTimeLimit1H() => objTimeLimit = 3600;

    [ToggleGroup("Objectives/useObjTimeLimit"), SerializeField, ShowInInspector, MinValue(0), SuffixLabel("Seconds"),
     LabelText("Time Limit")]
    private float objTimeLimit; // In Seconds

    [ToggleGroup("Objectives/useObjTimeLimit"), SerializeField, ShowInInspector, MinValue(1),
     Tooltip("The frequency how often to check if the timer reached 0."), LabelText("Time Check Interval")]
    private float objCheckerInterval = 1;

    private float _objCheckerIntervalCooldown = 0;

    [DisplayAsString, ShowInInspector, DisableInEditorMode, HideInEditorMode, ToggleGroup("Objectives/useObjTimeLimit")]
    public float TimeLeft => (objTimeLimit - Time.time + GameStartTime); // Odin Inspector Use Only

    [ToggleGroup("Objectives/useObjScoreLimit", ToggleGroupTitle = "Score Limit"), SerializeField, ShowInInspector]
    private bool useObjScoreLimit;

    [ToggleGroup("Objectives/useObjScoreLimit"), SerializeField, ShowInInspector, MinValue(0), LabelText("Score Cap")]
    private int objScoreLimit;

    [ToggleGroup("Objectives/useObjCheckpointLimit", ToggleGroupTitle = "Checkpoint Limit"), SerializeField,
     ShowInInspector]
    private bool useObjCheckpointLimit;

    [ToggleGroup("Objectives/useObjCheckpointLimit"), SerializeField, ShowInInspector, MinValue(0),
     LabelText("# Checkpoint Target")]
    private int objCheckpointLimit;

    #endregion

    [TitleGroup("User Interface")] [PreviewField]
    public GameObject fuelUIPrefab;

    [PreviewField] public GameObject scoreUIPrefab;

    private bool CheckCheckpointPerformanceSettings()
    {
        var width = useTreeWidthRandomRange
            ? Mathf.Max(upcomingTreeWidthRndRange.x, upcomingTreeWidthRndRange.y)
            : upcomingTreeWidth;
        return Mathf.Pow(upcomingTreeDepth, width) > 10;
    }
    
    [DetailedInfoBox(
         "Performance Warning: Your settings will create a lot of checkpoints.", "" +
                                                                                 "At start: aprox. (([Tree Width]^([Tree Depth]+1)-1) / ([Tree Width]-1).\n" +
                                                                                 "Recurring: ~[Tree Width]^[Tree Depth] Each time a player reaches a checkpoint." +
                                                                                 "\n This can lead to a low performance.",
         InfoMessageType.Warning, "CheckCheckpointPerformanceSettings")]
    [TitleGroup("Checkpoint System Settings")] [TitleGroup("Checkpoint System Settings"), PreviewField]
    public GameObject checkpointPrefab;

    [TitleGroup("Checkpoint System Settings")]
    public Checkpoint startCheckpoint;

    
    [TitleGroup("Checkpoint System Settings"), MinValue(0),
     Tooltip(
         "Defines how deep down the track-tree checkpoints should be spawned in advanced. Usually a value of 1-3 does the job.")]
    public int upcomingTreeDepth = 2;

    [HorizontalGroup("Checkpoint System Settings/ChkpTreeWidth", MinWidth = 0.8f)]
    [VerticalGroup("Checkpoint System Settings/ChkpTreeWidth/Values"), Range(0, 10), Tooltip(
         "Defines how many checkpoints will be spawned per tree depth. (=> Number of targets the player can choose from.)"),
     HideIf("useTreeWidthRandomRange"), DisableIf("useTreeWidthRandomRange"), LabelText("Tree Width")]
    public int upcomingTreeWidth = 2;

    [VerticalGroup("Checkpoint System Settings/ChkpTreeWidth/Values"),MinMaxSlider(0, 10, ShowFields = false),
     Tooltip("Defines how many checkpoints will be spawned per tree depth. (=> Number of targets the player can choose from.)"),
     ShowIf("useTreeWidthRandomRange"), EnableIf("useTreeWidthRandomRange"), OnValueChanged("RoundTreeWidthRange"), LabelText("Tree Width (Rnd)")]
    public Vector2 upcomingTreeWidthRndRange = new Vector2(1, 3);

    public Vector2 TreeWidth => (useTreeWidthRandomRange
        ? upcomingTreeWidthRndRange
        : new Vector2(upcomingTreeWidth, upcomingTreeWidth));
    
    private void RoundTreeWidthRange(){
        upcomingTreeWidthRndRange.x = Mathf.RoundToInt(upcomingTreeWidthRndRange.x);
        upcomingTreeWidthRndRange.y = Mathf.RoundToInt(upcomingTreeWidthRndRange.y);
    }
    
    [HorizontalGroup("Checkpoint System Settings/ChkpTreeWidth"), HideLabel, Tooltip("Use Random Range")]
    public bool useTreeWidthRandomRange = false;
    
    [TitleGroup("Checkpoint System Settings"), MinValue(0.01)] // Hacky name
    public float timeInBetweenTrackCalculations = .1f;

    [TitleGroup("Checkpoint System Settings"), Range(0,180)]
    public float cpBuilderMinConeAngle = 0;
    [TitleGroup("Checkpoint System Settings"), Range(0,180)]
    public float cpBuilderMaxConeAngle = 90;

    
    FuelUI fuelUI;
    ScoreUI scoreUI;
    #endregion
    
    public event Action<CheckpointReachedPlayerData> OnTargetCheckpointChanged;
    
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

        Checkpoint.checkpointPrefab = checkpointPrefab;
        Checkpoint.doPooling = true;
        var maxPossibleTreeWidth = useTreeWidthRandomRange ? Mathf.Max(upcomingTreeWidthRndRange.x, upcomingTreeWidthRndRange.y) : (upcomingTreeWidth <= 0 ? 1 : upcomingTreeWidth);
        Checkpoint.maxObjects = (int)((Mathf.Pow(maxPossibleTreeWidth, (upcomingTreeDepth+1f))-1) / (maxPossibleTreeWidth-1))*2;
        
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
        StartCoroutine(FindAndConnectNewCheckpoint(startCheckpoint, upcomingTreeDepth,TreeWidth,0,1));
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

        // Add points
        playercheckpointdata.playerInfo.score++;
        InvokeScoreChanged(playercheckpointdata.playerInfo, this);
        
        // Validate Objectives
        if(CheckObjectives())
            GameModeActive = false;
        
        // Set new targets
        checkpointManager.SetNextCheckpointTargets(playercheckpointdata.reachedCheckpoint.nextCheckpoints,
            playercheckpointdata.playerInfo);

        
        // Create new checkpoints
        int c = 0;
        playercheckpointdata?.reachedCheckpoint?.GetEndPoints()?.ForEach(endPoint =>
        {
            StartCoroutine(FindAndConnectNewCheckpoint(endPoint,1, TreeWidth, timeInBetweenTrackCalculations*c++));
        });
        
        
        // Delete unused old Checkpoints.
        if ((playercheckpointdata?.previousCheckpointData?.reachedCheckpoint?.nextCheckpoints?.Count ?? 0) > 0)
        {
            playercheckpointdata.previousCheckpointData.reachedCheckpoint.nextCheckpoints.ForEach(prevNxtCheckpoint =>
            {
                if(prevNxtCheckpoint != playercheckpointdata?.reachedCheckpoint)
                    checkpointManager.DeleteCheckpoint(prevNxtCheckpoint, true, true);
            });
        }
        OnTargetCheckpointChanged?.Invoke(playercheckpointdata);
        
        
        
        /*

        // Create new checkpoints
        playercheckpointdata.reachedCheckpoint.nextCheckpoints?.ForEach(checkpoint =>
        {
            StartCoroutine(FindAndConnectNewCheckpoint(checkpoint,numberOfCheckpointsToGenerateInAdvance));
        });*/
    }

    /// <summary>
    /// Hacky.
    /// Finds & creates new checkpoint locations.
    /// </summary>
    /// <param name="originCheckpoint">The start checkpoint.</param>
    /// <param name="depth">Defines depth of checkpoint tree to create. Recursively recalls this method and assigns the checkpoint to the earlier created one.</param>
    /// <param name="treeWidthRange">Range of tree width per depth-level. If x & y values are different, it takes a random number from that range. Vector2.zero / default is equivalent to a width of 1.</param>
    /// <param name="waitBeforeGeneration">If larger than 0, the execution of the method will be delayed by given amount.</param>
    /// <param name="ignoreTimeInBetweenCalculationsDepth">If > 0, timeInBetweenTrackCalculations timeouts will be ignored. The magnitude of the parameter defines until what depth it should be ignored.
    /// This can be useful if you want to spawn a certain amount of checkpoints immediately - for example when starting the level.</param>
    /// <returns></returns>
    IEnumerator FindAndConnectNewCheckpoint(Checkpoint originCheckpoint, int depth = 0, Vector2 treeWidthRange = default, float waitBeforeGeneration = 0, int ignoreTimeInBetweenCalculationsDepth = 0)
    {
        if(cpTrackBuilder == null)
            yield break;

        if (waitBeforeGeneration > 0)
        {
            yield return new WaitForSeconds(waitBeforeGeneration);
        }

        var treeWidthRangeX = (int) treeWidthRange.x;
        var treeWidthRangeY = (int) treeWidthRange.y;

        var treeWidth = (treeWidthRangeX + treeWidthRangeY <= 0)
            ? 1
            : ((treeWidthRangeX == treeWidthRangeY)
                ? treeWidthRangeX
                : Random.Range(Mathf.Min(treeWidthRangeX, treeWidthRangeY),
                    Mathf.Max(treeWidthRangeX, treeWidthRangeY)));

        for (var i = 0; i < treeWidth; i++)
        {
            var chkpoint = cpTrackBuilder.GenCheckpointWithinRadius(originCheckpoint, cpBuilderMaxConeAngle, CheckpointTrackBuilder.RandomSpecifier, cpBuilderMinConeAngle);
            if (chkpoint == null)
                yield break;
            
            chkpoint.transform.SetParent(cpTrackBuilder.transform);
            originCheckpoint.nextCheckpoints.Add(chkpoint);
            checkpointManager.ActiveCheckpointTrack.AddCheckpoint(chkpoint);
            

            if (depth > 1)
            {
                StartCoroutine(FindAndConnectNewCheckpoint(chkpoint,depth-1, treeWidthRange, timeInBetweenTrackCalculations, ignoreTimeInBetweenCalculationsDepth-1));
            }

            if(timeInBetweenTrackCalculations > 0 && ignoreTimeInBetweenCalculationsDepth <= 0)
                yield return new WaitForSeconds(timeInBetweenTrackCalculations);
        }
        
        if(depth <= 1)
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
