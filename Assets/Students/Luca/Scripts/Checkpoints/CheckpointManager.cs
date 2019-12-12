using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Students.Luca.Scripts.Checkpoints
{
/// <summary>
/// This class allows you to set up a sequence of checkpoints and manages visibility and status of each player.
/// </summary>
/// <remarks> 
/// <b>Use in Code:</b>
/// There are 2 events:
/// <list type="bullet">
///    <item>
///        <term><see cref="OnPlayerReachedCheckpoint"/></term>
///         <description>
///            The event <see cref="OnPlayerReachedCheckpoint"/> gets invoked when a player passes through a checkpoint. It delivers a <see cref="CheckpointReachedPlayerData"/> object containing data about the player and checkpoints reached.
///         </description>
///    </item>
///    <item>
///        <term><see cref="OnPlayerReachedLastCheckpoint"/></term>
///         <description>
///            The event <see cref="OnPlayerReachedLastCheckpoint"/> gets invoked when a player reached the last checkpoint. It delivers a <see cref="CheckpointReachedPlayerData"/> object containing data about the player and checkpoints reached.
///         </description>
///    </item>
/// </list>
/// 
/// </remarks>
/// <remarks> 
/// <b>Set Up in Editor:</b>
/// <list type="number">
///    <item>
///        <term><see cref="Students.Luca.Scripts.Checkpoints.Checkpoint"/></term>
///         <description>
///            Create several checkpoint <see cref="UnityEngine.GameObject"/> with <see cref="Students.Luca.Scripts.Checkpoints.Checkpoint"/> Component (+Trigger <see cref="UnityEngine.Collider"/>) attached.
///         </description>
///    </item>
///    <item>
///        <term><see cref="Students.Luca.Scripts.Checkpoints.Checkpoint"/></term>
///         <description>
///            Add each checkpoint to the <see cref="checkpoints"/> list in the inspector. The order/index in the list defines the sequence (top-down)
///         </description>
///    </item>
///    <item>
///        <term><see cref="Students.Luca.Scripts.Checkpoints.Checkpoint"/></term>
///         <description>
///            Set the <see cref="playerManager"/> OR make sure a <see cref="PlayerManager"/> Component exists somewhere in the current scene.
///         </description>
///    </item>
///    <item>
///        <term><see cref="Students.Luca.Scripts.Checkpoints.Checkpoint"/></term>
///         <description>
///            Set <see cref="checkpointVisibilityMethod"/>: Defines how the checkpoints are being displayed/hidden for each player. They have the same outcome. <break />
///             --- <see cref="CheckpointVisibilityMethod.SingleObjLayering"/>: There is a single gameobject per checkpoint and visibility per player is done through changing the layer to the according player specfic layer.
///             --- <see cref="CheckpointVisibilityMethod.PerPlayerObjLayering"/>: There is a checkpoint-gameobject for each player and checkpoint. (Uses more gameobjects. Total Amount: ([No of players]+1) * [No of checkpoints]
///         </description>
///    </item>
///    <item>
///        <term><see cref="Students.Luca.Scripts.Checkpoints.Checkpoint"/></term>
///         <description>
///            Create new layer that is not being culled, define the name of that layer in defaultCheckpointLayer
///         </description>
///    </item>
/// </list>
/// 
/// </remarks>
public class CheckpointManager : MonoBehaviour
    {
        #region Enums
        private enum TargetSelectionMaster
        {
            Self, // Checkpoint Manager selects new targets
            External // External Script decides what the next target is
        }
        
        public enum TargetSelectionProcedure
        {
            InternalExistingSequence, // Uses the predefined sequence in given CheckpointTrack [Has predefined end]
            ExternallyControlled, // Creates a new CheckpointTrack. Creation of checkpoints & selection is completely handled externally. [All defined externally]
            NOIMPL_RandomShuffleExistingSequence, // Takes the predefined checkpoints in given CheckpointTrack & randomly shuffles them [Has predefined end] TODO not implemented
            NOIMPL_RandomExistingContinuous, // Continuously takes a new random checkpoint from the given CheckpointTrack [Has no end, terminated externally]  TODO not implemented
        }



        public enum PlayerTargetMode
        {
            PersonalTarget, // Each player has his own targets
            SharedTarget // All players chase one target
        }
        #endregion

        #region Variables
        /// <summary> The CheckpointManager gets player info from the playerManager and listens for joining/leaving players. </summary>
        [Header("Components")]
        public PlayerManager playerManager;
        
        
        [SerializeField, ShowInInspector]
        private CheckpointTrack activeCheckpointTrack;


        /// <summary> Stores references to all checkpoints. By default, the order/index of the list is used for the checkpoint sequence. </summary>
        [Header("Settings")]
        
        [ShowInInspector, ReadOnly]
        private TargetSelectionMaster _targetSelectionMaster;

        public PlayerTargetMode playerTargetMode;

        public TargetSelectionProcedure targetSelectionProcedure;
        
        
        /// <summary> Default layer of the checkpoint objects. Make this layer hidden to cameras (Remove from culling). </summary>
        public string defaultCheckpointLayer = "checkpoints";
        private int _defaultCheckpointLayerIndex = 0;
        
        [Header("Visibility & Highlighting")]
        public bool hidePastTargets = true;
        public bool hideFutureTargets = true;
        public int visibleFutureTargetsDepth = -1; // -1 = show all, This is not the amount of targets shown, its the "tree-depth" (Since you can have multiple next targets)
        public int highlightNearFutureTargetsDepth = 0; // The amount ("tree-depth", not actual count of checkpoints) of future targets to highlight with a specified special material.
        
        public Material defaultCheckpointMaterial;
        public Material activeCheckpointMaterial;
        public Material inactivePastCheckpointMaterial;
        public Material inactiveFutureCheckpointMaterial;
        public Material highlightedNearFutureCheckpointMaterial;
        
        
        private Dictionary<PlayerInfo, List<Checkpoint>> _pastPlayerTargets = new Dictionary<PlayerInfo, List<Checkpoint>>();
        private List<Checkpoint> _currentSharedTargets;
        
        public CheckpointTrack ActiveCheckpointTrack
        {
            get => activeCheckpointTrack;
            set
            {
                if (activeCheckpointTrack)
                    UnsetCheckpointTrack(activeCheckpointTrack);

                if (value)
                    InitCheckpointTrack(value);
                
                activeCheckpointTrack = value;
                
                // TODO Possibly need any other setup? reset player data... ?
            }
        }
        public void InitCheckpointTrack(CheckpointTrack checkpointTrack)
        {
            if (checkpointTrack == null)
                return;
            
            checkpointTrack.SetDefaultCheckpointLayer(_defaultCheckpointLayerIndex);
            checkpointTrack.OnPossessableReachedCheckpoint += HandlePossessableReachedCheckpointEvent;
        }
        public void UnsetCheckpointTrack(CheckpointTrack checkpointTrack) => checkpointTrack.OnPossessableReachedCheckpoint -= HandlePossessableReachedCheckpointEvent;



        [ShowInInspector, ReadOnly]
        private Dictionary<PlayerInfo, CheckpointReachedPlayerData> _currentPlayerCheckpointStatus;

        [field: ShowInInspector]
        [field: ReadOnly]
        public bool Initialized { get; private set; } = false;

        
        #endregion
        
        
        #region Events Declarations / Invocation

        public delegate void PlayerReachedCheckpointDel(CheckpointReachedPlayerData playerCheckpointData);

        public event PlayerReachedCheckpointDel OnPlayerReachedCheckpoint;
        public event PlayerReachedCheckpointDel OnPlayerReachedLastCheckpoint;
        

        private void NotifyPlayerReachedCheckpoint(CheckpointReachedPlayerData playerCheckpointData)
        {
            OnPlayerReachedCheckpoint?.Invoke(playerCheckpointData);
        }

        private void NotifyPlayerReachedLastCheckpoint(CheckpointReachedPlayerData playerCheckpointData)
        {
            OnPlayerReachedLastCheckpoint?.Invoke(playerCheckpointData);

            CheckpointReachedPlayerData playerData = GetCurrentPlayerCheckpointData(playerCheckpointData.playerInfo);
            Debug.Log("Player "+playerCheckpointData.playerInfo.realCamera.name+" reached the last checkpoint! #Checkpoints reached: "+playerCheckpointData?.GetReachedCheckpointsCount()+" - Total Time: "+playerCheckpointData?.GetTotalTimeSinceFirstCheckpoint());
        }
    
        #endregion
    
    
        // Start is called before the first frame update
        private void Start()
        {
            if (ActiveCheckpointTrack.autoInitializeOnStart)
            {
                ActiveCheckpointTrack?.Init();
                InitCheckpointTrack(ActiveCheckpointTrack);
            }
            
            var suc = Init();
        }

        private void OnDestroy()
        {
            if (activeCheckpointTrack != null) InitCheckpointTrack(ActiveCheckpointTrack);
            
            if (playerManager != null)
            {
                //playerManager.OnNewPlayerJoinedGame -= HandleNewPlayerJoinedEvent;
                playerManager.OnPlayerLeftGame -= HandlePlayerLeftGameEvent;
            }
            
            //Camera.onPreCull -= HandleCameraPreCullEvent;
            //Camera.onPostRender -= HandleCameraPostRenderEvent;
            RenderPipelineManager.beginCameraRendering -= HandleBeginCameraRenderingEvent;
            RenderPipelineManager.endCameraRendering -= HandleEndCameraRenderingEvent;
        }

        public bool Init()
        {
            if(Initialized)
                return false;
            
            switch (targetSelectionProcedure)
            {
                case TargetSelectionProcedure.ExternallyControlled:
                    _targetSelectionMaster = TargetSelectionMaster.External;
                    break;
                case TargetSelectionProcedure.InternalExistingSequence:
                case TargetSelectionProcedure.NOIMPL_RandomShuffleExistingSequence:
                case TargetSelectionProcedure.NOIMPL_RandomExistingContinuous:
                default:
                    _targetSelectionMaster = TargetSelectionMaster.Self;
                    break;
            }
            
            _defaultCheckpointLayerIndex = LayerMask.NameToLayer(defaultCheckpointLayer);
            if (playerManager == null)
                playerManager = FindObjectOfType<PlayerManager>();

            if (playerTargetMode == PlayerTargetMode.SharedTarget)
                _currentSharedTargets = new List<Checkpoint>(){ActiveCheckpointTrack?.GetStartCheckpoint()};
            
            //Camera.onPreCull += HandleCameraPreCullEvent;
            //Camera.onPostRender += HandleCameraPostRenderEvent;
            RenderPipelineManager.beginCameraRendering += HandleBeginCameraRenderingEvent;
            RenderPipelineManager.endCameraRendering += HandleEndCameraRenderingEvent;
            
            ResetAllPlayerCheckpointStatus();
            //ResetCheckpoints();

            if (playerManager != null)
            {
                //playerManager.OnNewPlayerJoinedGame += HandleNewPlayerJoinedEvent;
                playerManager.OnPlayerLeftGame += HandlePlayerLeftGameEvent;
            }

            Initialized = true;
            return true;
        }
        
        #region Checkpoint Specific Methods

        private List<Checkpoint> GetNextCheckpointTargets(Checkpoint checkpoint)
        {
            return checkpoint?.nextCheckpoints;
        }
        
        /// <summary>
        /// Checks if given checkpoint is an end node.
        /// </summary>
        /// <returns>True if given checkpoint is an end node.</returns>
        public bool IsLastCheckpoint(Checkpoint checkpoint)
        {
            return (checkpoint?.nextCheckpoints?.Count ?? 0) == 0;
        }

        #endregion

        #region Player Related Methods
        
        /// <summary>
        /// Deletes the checkpoint status data of all players.
        /// </summary>
        public void ResetAllPlayerCheckpointStatus()
        {
            _currentPlayerCheckpointStatus = new Dictionary<PlayerInfo, CheckpointReachedPlayerData>();
            _pastPlayerTargets = new Dictionary<PlayerInfo, List<Checkpoint>>();
        }

        /// <summary>
        /// Resets the checkpoint status data of given player.
        /// </summary>
        public void ResetPlayerCheckpointStatus(PlayerInfo playerInfo)
        {
            if (_currentPlayerCheckpointStatus?.ContainsKey(playerInfo) ?? false)
            {
                _currentPlayerCheckpointStatus.Remove(playerInfo);
            }

            if (_pastPlayerTargets?.ContainsKey(playerInfo) ?? false)
            {
                _pastPlayerTargets.Remove(playerInfo);
            }
        }

        // Updates the last reached checkpoint of a player
        private void UpdatePlayerCheckpointStatus(PlayerInfo playerInfo, Checkpoint checkpoint)
        {
            if (_currentPlayerCheckpointStatus == null)
                _currentPlayerCheckpointStatus = new Dictionary<PlayerInfo, CheckpointReachedPlayerData>();

            var nextCheckpointTargets = _targetSelectionMaster == TargetSelectionMaster.Self?GetNextCheckpointTargets(checkpoint):null;
            var lockCheckpointTargets = _targetSelectionMaster == TargetSelectionMaster.Self;
            CheckpointReachedPlayerData data;
            if (_currentPlayerCheckpointStatus.ContainsKey(playerInfo))
            {
                data = new CheckpointReachedPlayerData(playerInfo, checkpoint, Time.time, _currentPlayerCheckpointStatus[playerInfo], nextCheckpointTargets, lockCheckpointTargets);
                _currentPlayerCheckpointStatus[playerInfo] = data;
            }
            else
            {
                data = new CheckpointReachedPlayerData(playerInfo, checkpoint, Time.time, null, nextCheckpointTargets, lockCheckpointTargets);
                _currentPlayerCheckpointStatus.Add(playerInfo, data);
            }

            if (_pastPlayerTargets.ContainsKey(playerInfo) && checkpoint != null)
            {
                _pastPlayerTargets[playerInfo].Add(checkpoint);
            }
            else if(checkpoint != null)
            {
                _pastPlayerTargets.Add(playerInfo, new List<Checkpoint>(){checkpoint});
            }
        
            NotifyPlayerReachedCheckpoint(data);
            if(IsLastCheckpoint(checkpoint))
                NotifyPlayerReachedLastCheckpoint(data);
        }
        
        /// <summary>
        /// Returns the last reached checkpoint of given player.
        /// </summary>
        public Checkpoint GetLastReachedCheckpoint(PlayerInfo playerInfo)
        {
            return (!_currentPlayerCheckpointStatus?.ContainsKey(playerInfo) ?? true)
                ? null
                : _currentPlayerCheckpointStatus[playerInfo].reachedCheckpoint;
        }
        
        /// <summary>
        /// Returns a list of the current targets of the given player.
        /// </summary>
        public List<Checkpoint> GetCurrentPlayerCheckpointTargets(PlayerInfo playerInfo)
        {
            if (playerTargetMode == PlayerTargetMode.SharedTarget)
            {
                return _currentSharedTargets ?? new List<Checkpoint>() {activeCheckpointTrack?.GetStartCheckpoint()};
            }
            return GetCurrentPlayerCheckpointData(playerInfo)?.GetNextCheckpointTargets() ?? new List<Checkpoint>(){activeCheckpointTrack?.GetStartCheckpoint()};
        }
        
        // Internally use this function to set a new target
        private bool SetNextCheckpointTargetsInternal(List<Checkpoint> checkpoints, PlayerInfo playerInfo = default)
        {
            return _targetSelectionMaster != TargetSelectionMaster.External &&
                   _ForceSetCheckpointTargets(23849729, checkpoints, playerInfo);
        }

         /// <summary>
         /// Sets the next checkpoint targets of the given player. If in shared target mode, it will set the targets
         /// for all players. This method only has an effect if external track modification is allowed (See TargetSelectionMaster).
         /// [ ONLY USE THIS METHOD EXTERNALLY ]
         /// </summary>
         /// <param name="checkpoints">List of the next targets.</param>
         /// <param name="playerInfo"></param>
         /// <returns>True if the targets were successfully set.</returns>
        public bool SetNextCheckpointTargets(List<Checkpoint> checkpoints, PlayerInfo playerInfo = default)
        {
            if (_targetSelectionMaster == TargetSelectionMaster.Self)
                return false;
            return _ForceSetCheckpointTargets(23849729, checkpoints, playerInfo);
        }

        /// <summary>
        /// Sets the next checkpoint target of the given player. If in shared target mode, it will set the target
        /// for all players. This method only has an effect if external track modification is allowed (See TargetSelectionMaster).
        /// [ ONLY USE THIS METHOD EXTERNALLY ]
        /// </summary>
        /// <param name="checkpoint">The next target.</param>
        /// <param name="playerInfo"></param>
        /// <returns>True if the targets were successfully set.</returns>
        public bool SetNextCheckpointTarget(Checkpoint checkpoint, PlayerInfo playerInfo = default)
        {
            return SetNextCheckpointTargets(new List<Checkpoint>(){checkpoint}, playerInfo);
        }
        
        
        private bool _ForceSetCheckpointTargets(int hardcodedSuperTopSecretSecurityUsageCode ,List<Checkpoint> checkpoints, PlayerInfo playerInfo = default)
        {
            if (hardcodedSuperTopSecretSecurityUsageCode != 23849729) // hacky; Target is to make sure that nobody uses this method except for SetNextCheckpointTargets and SetNextCheckpointTargetsInternal
                return false;
            
            var crpd = GetCurrentPlayerCheckpointData(playerInfo);
            
            if (playerTargetMode == PlayerTargetMode.PersonalTarget)
                return crpd?.AddNextCheckpointTargets(checkpoints) ?? false;

            _currentPlayerCheckpointStatus?.Values.ForEach(playerCpdt =>
            {
                playerCpdt?.ResetNextCheckpointTargets();
                playerCpdt?.AddNextCheckpointTargets(checkpoints);
            });
            
            _currentSharedTargets = checkpoints;
            return true;

        }

        public void DeleteCheckpoint(Checkpoint checkpoint, bool deleteRecursively = true, bool safeDelete = true)
        {
            if(checkpoint == null)
                return;

            var canDelete = true;
            if (safeDelete)
            {
                if((_currentPlayerCheckpointStatus?.Count ?? 0) > 0 )
                {
                    _currentPlayerCheckpointStatus.Values.ForEach(currentPlayerCheckpointData =>
                        {
                            canDelete = canDelete &&
                                        !(currentPlayerCheckpointData?.PlayerHasCheckpointAsTarget(checkpoint) ?? false) &&
                                        !(currentPlayerCheckpointData?.PlayerHasReachedCheckpoint(checkpoint) ?? false);
                        });
                }
            }
            
            if(canDelete)
                ActiveCheckpointTrack?.DeleteCheckpoint(checkpoint,deleteRecursively);
        }
        
        /// <summary>
        /// Returns an object containing the current checkpoint status data of given player.
        /// </summary>
        public CheckpointReachedPlayerData GetCurrentPlayerCheckpointData(PlayerInfo playerInfo)
        {
            return (!_currentPlayerCheckpointStatus?.ContainsKey(playerInfo) ?? true)
                ? null
                : _currentPlayerCheckpointStatus[playerInfo];
        }

        #endregion

        #region Checkpoint Finding

        /// <summary>
        /// Searches for the nearest checkpoint from the given point.
        /// </summary>
        public Checkpoint FindClosestCheckpoint(Vector3 point)
        {
            Checkpoint closestCheckpoint = null;
            var closestObjDistance = float.PositiveInfinity;

            activeCheckpointTrack?.GetAllCheckpoints().Where(checkpoint => checkpoint != null).ForEach(checkpoint =>
            {
                var dist = Vector3.Distance(point, checkpoint.transform.position);
                if (closestObjDistance <= dist) return;
                closestCheckpoint = checkpoint;
                closestObjDistance = dist;
            });
            
            return closestCheckpoint;
        }

        /// <summary>
        /// Searches for checkpoints within a given radius & angle and returns a list will found checkpoints.
        /// </summary>
        /// <param name="point">Center point in world space of the search.</param>
        /// <param name="radius">The maximum distance to search away from the given point.</param>
        /// <param name="coneAngle">Limits the search area. Given value spreads to left and right => total search angle will be double. Valid values: 0-180</param>
        /// <param name="forward">Forward direction. Needed if coneAngle != 180.</param>
        /// <returns></returns>
        public List<Checkpoint> FindCheckpointsWithinRadius(Vector3 point, float radius, float coneAngle = 180f, Vector3 forward = default)
        {
            var checkpoints = new List<Checkpoint>();

            if (forward == default) // !Mathf.Approximately(Math.Abs(coneAngle % 180f),0) && 
                forward = Vector3.forward;
            var hasAngleLimitation = true;
            if (Mathf.Approximately(coneAngle%180, 0))
            {
                hasAngleLimitation = false;
                coneAngle = 180;
            }
            else
            {
                coneAngle = coneAngle % 180;
            }
            
            activeCheckpointTrack?.GetAllCheckpoints().Where(checkpoint => checkpoint != null).ForEach(checkpoint =>
            {
                var checkpointPos = checkpoint.transform.position;
                var dist = Vector3.Distance(point, checkpointPos);
                var angle = hasAngleLimitation ? Vector3.Angle(forward, checkpointPos) : 0;

                if (dist <= radius && angle < coneAngle)
                    checkpoints.Add(checkpoint);
            });

            return checkpoints;
        }

        /// <summary>
        /// Returns a random checkpoint on the track.
        /// </summary>
        public Checkpoint FindRandomCheckpoint()
        {
            return FindRandomCheckpoints(1)?[0];
        }

        /// <summary>
        /// Returns a list of random checkpoints.
        /// </summary>
        /// <param name="amount">The amount of random checkpoints to return.</param>
        /// <returns></returns>
        public List<Checkpoint> FindRandomCheckpoints(int amount = 1)
        {
            var randomCheckpoints = new List<Checkpoint>();
            var checkpoints = activeCheckpointTrack?.GetAllCheckpoints().Where(checkpoint => checkpoint != null).ToList();
            var checkpointsMaxIndex = checkpoints?.Count-1 ?? -1;
            
            if (checkpointsMaxIndex == -1)
                return randomCheckpoints;
            
            amount = Mathf.Clamp(amount, 0, checkpointsMaxIndex);

            while (randomCheckpoints.Count < amount && checkpointsMaxIndex >= 0)
            {
                var i = Random.Range(0, checkpointsMaxIndex);
                Checkpoint checkpoint = checkpoints[i];
                if (randomCheckpoints.Contains(checkpoint)) continue;
                randomCheckpoints.Add(checkpoint);
                checkpoints.Remove(checkpoint); // TODO: Not sure if indexes get updated when removing an item? I suppose
                checkpointsMaxIndex--;
            }
            

            return randomCheckpoints;
        }

        /// <returns>Returns a list of all checkpoints.</returns>
        public List<Checkpoint> GetAllCheckpoints() => activeCheckpointTrack?.GetAllCheckpoints();

        
        

        #endregion
        
        #region EventHandling

        // Gets executed when a possessable passes a [Checkpoint]
        private void HandlePossessableReachedCheckpointEvent(Checkpoint checkpoint, Possessable possessable)
        {
            if (playerManager?.playerInfos == null) return;

            var playerInfo = playerManager.playerInfos.FirstOrDefault(playerInfoEntry => playerInfoEntry.controller?.possessable == possessable);

            
            if (playerInfo == null)
                return;
            
            var currentPlayerTargets = GetCurrentPlayerCheckpointTargets(playerInfo);
            if (!currentPlayerTargets.Contains(checkpoint))
                return;

            bool x = SetNextCheckpointTargetsInternal(GetNextCheckpointTargets(checkpoint));
            /*if(playerTargetMode == PlayerTargetMode.SharedTarget && targetSelectionMaster == TargetSelectionMaster.Self)
                currentSharedTargets = GetNextCheckpointTargets(checkpoint);*/
            
            UpdatePlayerCheckpointStatus(playerInfo, checkpoint);
        }
        
        private void HandleNewPlayerJoinedEvent(PlayerInfo playerinfo){}
        
        private void HandlePlayerLeftGameEvent(PlayerInfo playerinfo)
        {
            
            ResetPlayerCheckpointStatus(playerinfo);
        }
        
        private void HandleBeginCameraRenderingEvent(ScriptableRenderContext arg1, Camera cam)
        {
            var playerInfo = playerManager?.playerInfos?.FirstOrDefault(pi => pi.realCamera == cam) ?? default;

            if (playerInfo == null)
                return;

            // Handle display of past targets
            if (!hidePastTargets && (_pastPlayerTargets?.ContainsKey(playerInfo) ?? false))
            {
                _pastPlayerTargets[playerInfo].Where(checkpoint => checkpoint != null).ForEach(checkpoint =>
                {
                    if (inactivePastCheckpointMaterial != null)
                    {
                        // Hacky & In-Performant
                        var r = checkpoint.GetComponentInChildren<Renderer>();
                        
                        if (r != null)
                        {
                            r.material = inactivePastCheckpointMaterial;
                            var emission = checkpoint.fx.emission;
                            emission.enabled = false;
                        }
                    }
                        
                    checkpoint.gameObject.layer = playerInfo.virtualCameraLayer;
                });
            }
            else
            {
                if ((_pastPlayerTargets?.ContainsKey(playerInfo) ?? false))
                {
                    _pastPlayerTargets[playerInfo].Where(checkpoint => checkpoint != null).ForEach(checkpoint =>
                        {
                            
                            var r = checkpoint.GetComponentInChildren<Renderer>();
                        
                            if (r != null)
                            {
                                r.material = inactivePastCheckpointMaterial;
                                var emission = checkpoint.fx.emission;
                                emission.enabled = false;
                            }
                            checkpoint.enabled = false;
                        });
                }
            }

            // Handle display of current & future checkpoints
            var curCheckpointTargets = GetCurrentPlayerCheckpointTargets(playerInfo);
            curCheckpointTargets?.Where(checkpoint => checkpoint != null).ForEach(checkpoint =>
            {
                if (activeCheckpointMaterial != null)
                {
                    // Hacky & In-Performant
                    var r = checkpoint.GetComponentInChildren<Renderer>();
                    if (r != null)
                    {
                        r.material = activeCheckpointMaterial;
                        
                        var emission = checkpoint.fx.emission;
                        emission.enabled = true;
                    }
                }
                checkpoint.gameObject.layer = playerInfo.virtualCameraLayer;
                HandleFutureTargetsHighlighting(checkpoint.nextCheckpoints, playerInfo, false);
            });
        }

        private void HandleEndCameraRenderingEvent(ScriptableRenderContext arg1, Camera cam)
        {
            var playerInfo = playerManager?.playerInfos?.FirstOrDefault(pi => pi.realCamera == cam) ?? default;
            
            if (playerInfo == null)
                return;
            
            // Handle hiding of past targets
            if (!hidePastTargets && (_pastPlayerTargets?.ContainsKey(playerInfo) ?? false))
            {
                _pastPlayerTargets[playerInfo].Where(checkpoint => checkpoint != null).ForEach(checkpoint =>
                {
                    checkpoint.gameObject.layer = _defaultCheckpointLayerIndex;
                });
            }
            
            // Handle hiding of current & future checkpoints
            var curCheckpointTargets = GetCurrentPlayerCheckpointTargets(playerInfo);
            curCheckpointTargets?.Where(checkpoint => checkpoint != null).ForEach(checkpoint =>
                {
                    checkpoint.gameObject.layer = _defaultCheckpointLayerIndex;
                    HandleFutureTargetsHighlighting(checkpoint?.nextCheckpoints, playerInfo, true);
                });
        }

        private void HandleFutureTargetsHighlighting(List<Checkpoint> futureTargets, PlayerInfo playerInfo, bool hide, int currentFutureTargetDepth = 0)
        {
            if(hideFutureTargets ||
               visibleFutureTargetsDepth == 0 ||
               (inactiveFutureCheckpointMaterial == null && highlightedNearFutureCheckpointMaterial == null) ||
               playerInfo.Equals(default))
                return;
            futureTargets?.ForEach(checkpoint =>
            {
                if(checkpoint == null)
                    return;
                
                var r = checkpoint.GetComponentInChildren<Renderer>();
                if (r != null && !hide)
                {
                    if ((highlightNearFutureTargetsDepth == -1 ||
                         currentFutureTargetDepth < highlightNearFutureTargetsDepth) &&
                        highlightedNearFutureCheckpointMaterial != null)
                    {
                        r.material = highlightedNearFutureCheckpointMaterial;
                        var emission = checkpoint.fx.emission;
                        emission.enabled = false;
                    }
                    else if (currentFutureTargetDepth < visibleFutureTargetsDepth &&
                             inactiveFutureCheckpointMaterial != null)
                    {
                        r.material = inactiveFutureCheckpointMaterial;
                        var emission = checkpoint.fx.emission;
                        emission.enabled = false;
                    }
                }
                checkpoint.gameObject.layer = hide ? _defaultCheckpointLayerIndex : playerInfo.virtualCameraLayer;
                
                
                if((checkpoint.nextCheckpoints?.Count??0) > 0 && (visibleFutureTargetsDepth == -1 || currentFutureTargetDepth+1 < visibleFutureTargetsDepth))
                    HandleFutureTargetsHighlighting(checkpoint.nextCheckpoints, playerInfo, hide, currentFutureTargetDepth+1);
            });
            
        }

        #endregion

        private void OnValidate()
        {
            switch (targetSelectionProcedure)
            {
                case TargetSelectionProcedure.ExternallyControlled:
                    _targetSelectionMaster = TargetSelectionMaster.External;
                    break;
                case TargetSelectionProcedure.InternalExistingSequence:
                case TargetSelectionProcedure.NOIMPL_RandomShuffleExistingSequence:
                case TargetSelectionProcedure.NOIMPL_RandomExistingContinuous:
                default:
                    _targetSelectionMaster = TargetSelectionMaster.Self;
                    break;
            }
        }
    }
}
