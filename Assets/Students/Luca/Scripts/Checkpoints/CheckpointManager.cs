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
        /*/**
         * CheckpointVisibilityMethod : enum Defines how the checkpoints are being displayed/hidden for each player.
         #1#
        private enum CheckpointVisibilityMethod
        {
            /// <summary> There is a single gameobject per checkpoint and visibility per player is done through changing the layer to the according player specfic layer. Note: This might be inperformant(?) since for each player camera a pre-render and post-render method will be called.</summary>
            SingleObjLayering,
            /// There is a checkpoint-gameobject for each player and checkpoint. May be more performant? (Uses more gameobjects. Total Amount: ([No of players]+1) * [No of checkpoints]
            PerPlayerObjLayering
        }*/
        
        
        private enum TargetSelectionMaster
        {
            Self, // Checkpoint Manager selects new targets
            External // External Script decides what the next target is
        }
        
        public enum TargetSelectionProcedure
        {
            ExistingSequence, // Uses the predefined sequence in given CheckpointTrack [Has predefined end]
            RandomShuffleExistingSequence, // Takes the predefined checkpoints in given CheckpointTrack & randomly shuffles them [Has predefined end] TODO not implemented
            RandomExistingContinuous, // Continuously takes a new random checkpoint from the given CheckpointTrack [Has no end, terminated externally]  TODO not implemented
            
            ExistingExternalControl, // Uses the checkpoints of a given CheckpointTrack. Setting new targets is done externally. [End defined externally] TODO not implemented
            CustomExternalControl // Creates a new CheckpointTrack. Creation of checkpoints & selection is completely handled externally. [All defined externally] TODO not implemented
            /*RandomShuffleExistingUnique, // ? RandomExistingShuffleSequence
            RandomExistingAllowRepeat, // ? RandomExistingContinuous
            RandomNewWithinAngle,
            RandomNewWithinBounds*/
        }

/*

        public enum RandomCheckpointPlacementMethod
        {
            Ground,
            TerrainGround,
            SpecifiedHeighAboveGround,
            RandomWithinBonds
        }*/

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


        /// <summary> Stores references to all checkpoints. By default, the order/index of the list is used for the checkpoint sequence. </summary>
        [Header("Settings")]
        
        [ShowInInspector, SerializeField]
        private bool autoInitializeOnStart = true;
        [ShowInInspector, ReadOnly]
        private TargetSelectionMaster _targetSelectionMaster;

        public PlayerTargetMode playerTargetMode;
        private List<Checkpoint> _currentSharedTargets;

        public TargetSelectionProcedure targetSelectionProcedure;
        
        [SerializeField, ShowInInspector]
        private CheckpointTrack activeCheckpointTrack;

        public CheckpointTrack ActiveCheckpointTrack
        {
            get => activeCheckpointTrack;
            set
            {
                if (activeCheckpointTrack)
                    TerminateCheckpointTrack(activeCheckpointTrack);

                if (value)
                    InitCheckpointTrack(value);
                
                activeCheckpointTrack = value;
                
                // TODO any other setup? reset player data... ?
            }
        }
        private void InitCheckpointTrack(CheckpointTrack checkpointTrack)
        {
            if (checkpointTrack == null)
                return;
            
            checkpointTrack.SetDefaultCheckpointLayer(_defaultCheckpointLayerIndex);
            checkpointTrack.OnPossessableReachedCheckpoint += HandlePossessableReachedCheckpointEvent;
        }
        private void TerminateCheckpointTrack(CheckpointTrack checkpointTrack) => checkpointTrack.OnPossessableReachedCheckpoint -= HandlePossessableReachedCheckpointEvent;


        /// <summary> ONLY USED FOR <see cref="CheckpointVisibilityMethod.SingleObjLayering"/>. Default layer of the checkpoint objects. Make this layer hidden to cameras (Remove from culling). </summary>
        [Header("Single Object Layering Settings")]
        public string defaultCheckpointLayer = "checkpoints";
        private int _defaultCheckpointLayerIndex = 0;

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
            if (autoInitializeOnStart)
            {
                ActiveCheckpointTrack?.Init();
                var suc = Init();
                if(!(ActiveCheckpointTrack?.Initialized ?? true) && !suc)
                    Debug.LogWarning("Couldn't auto-initialize CheckpointManager on Start. (Possible reasons: Already initialized; ActiveCheckpointTrack null OR not initialized;)");
            }
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
            if(Initialized || (ActiveCheckpointTrack == null || !ActiveCheckpointTrack.Initialized))
                return false;
            
            switch (targetSelectionProcedure)
            {
                case TargetSelectionProcedure.ExistingExternalControl:
                case TargetSelectionProcedure.CustomExternalControl:
                    _targetSelectionMaster = TargetSelectionMaster.External;
                    break;
                case TargetSelectionProcedure.ExistingSequence:
                case TargetSelectionProcedure.RandomShuffleExistingSequence:
                case TargetSelectionProcedure.RandomExistingContinuous:
                default:
                    _targetSelectionMaster = TargetSelectionMaster.Self;
                    break;
            }
            
            _defaultCheckpointLayerIndex = LayerMask.NameToLayer(defaultCheckpointLayer);
            if (playerManager == null)
                playerManager = FindObjectOfType<PlayerManager>();

            // Make sure the Proeprty Setter gets invoked for registering to events.
            if (activeCheckpointTrack != null) InitCheckpointTrack(ActiveCheckpointTrack);

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
        
        public bool IsLastCheckpoint(Checkpoint checkpoint)
        {
            return (checkpoint?.nextCheckpoints?.Count ?? 0) == 0;
        }

        #endregion

        #region Player Related Methods
        
        public void ResetAllPlayerCheckpointStatus()
        {
            _currentPlayerCheckpointStatus = new Dictionary<PlayerInfo, CheckpointReachedPlayerData>();
        }

        public void ResetPlayerCheckpointStatus(PlayerInfo playerInfo)
        {
            if (_currentPlayerCheckpointStatus?.ContainsKey(playerInfo) ?? false)
            {
                _currentPlayerCheckpointStatus.Remove(playerInfo);
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
        
            NotifyPlayerReachedCheckpoint(data);
            if(IsLastCheckpoint(checkpoint))
                NotifyPlayerReachedLastCheckpoint(data);
        }
        
        public Checkpoint GetLastReachedCheckpoint(PlayerInfo playerInfo)
        {
            return (!_currentPlayerCheckpointStatus?.ContainsKey(playerInfo) ?? true)
                ? null
                : _currentPlayerCheckpointStatus[playerInfo].reachedCheckpoint;
        }
        
        public List<Checkpoint> GetCurrentPlayerCheckpointTargets(PlayerInfo playerInfo)
        {
            if (playerTargetMode == PlayerTargetMode.SharedTarget)
            {
                return _currentSharedTargets ?? new List<Checkpoint>() {activeCheckpointTrack?.GetStartCheckpoint()};
            }
            else
            {
                
                return GetCurrentPlayerCheckpointData(playerInfo)?.GetNextCheckpointTargets() ?? new List<Checkpoint>(){activeCheckpointTrack?.GetStartCheckpoint()};
                //return GetNextCheckpointTargets(GetLastReachedCheckpoint(playerInfo)) ?? new List<Checkpoint>(){activeCheckpointTrack?.GetFirstCheckpoint()};
            }
        }
        
        // Internally use this function to set a new target
        private bool SetNextCheckpointTargetsInternal(List<Checkpoint> checkpoints, PlayerInfo playerInfo = default)
        {
            if (_targetSelectionMaster == TargetSelectionMaster.External)
                return false;
            
            return _ForceSetCheckpointTargets(23849729, checkpoints, playerInfo);
        }

        // Only use this function externally. NOT for internal use,
        public bool SetNextCheckpointTargets(List<Checkpoint> checkpoints, PlayerInfo playerInfo = default)
        {
            if (_targetSelectionMaster == TargetSelectionMaster.Self)
                return false;
            
            return _ForceSetCheckpointTargets(23849729, checkpoints, playerInfo);
        }

        // Only use this function externally. NOT for internal use,
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
            _currentSharedTargets = checkpoints;
            return true;

        }
        
        public CheckpointReachedPlayerData GetCurrentPlayerCheckpointData(PlayerInfo playerInfo)
        {
            return (!_currentPlayerCheckpointStatus?.ContainsKey(playerInfo) ?? true)
                ? null
                : _currentPlayerCheckpointStatus[playerInfo];
        }

        #endregion

        #region Checkpoint Finding

        public Checkpoint FindClosesCheckpoint(Vector3 point)
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

        public Checkpoint FindRandomCheckpoint()
        {
            return FindRandomCheckpoints(1)?[0];
        }

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

        public List<Checkpoint> GetAllCheckpoints() => activeCheckpointTrack?.GetAllCheckpoints();

        
        

        #endregion
        
        #region EventHandling

        // Gets executed when a possessable passes a [Checkpoint]
        private void HandlePossessableReachedCheckpointEvent(Checkpoint checkpoint, Possessable possessable)
        {
            if (playerManager?.playerInfos == null) return;

            var playerInfo = playerManager.playerInfos.FirstOrDefault(playerInfoEntry => playerInfoEntry.controller?.possessable == possessable);

            
            if (playerInfo.Equals(default(PlayerInfo)))
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

            if (playerInfo.Equals(default(PlayerInfo)))
                return;
            
            var curCheckpointTargets = GetCurrentPlayerCheckpointTargets(playerInfo);
            curCheckpointTargets?.ForEach(checkpoint => checkpoint.gameObject.layer = playerInfo.virtualCameraLayer);
        }

        private void HandleEndCameraRenderingEvent(ScriptableRenderContext arg1, Camera cam)
        {
            var playerInfo = playerManager?.playerInfos?.FirstOrDefault(pi => pi.realCamera == cam) ?? default;
            
            if (playerInfo.Equals(default(PlayerInfo)))
                return;
            
            var curCheckpointTargets = GetCurrentPlayerCheckpointTargets(playerInfo);
            curCheckpointTargets?.Where(checkpoint => checkpoint != null).ForEach(checkpoint =>
                checkpoint.gameObject.layer = _defaultCheckpointLayerIndex);
        }

        #endregion
        
    }
}
