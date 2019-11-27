using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

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
        
        
        
        public enum TargetSelectionProcedure
        {
            Sequence,
            /*RandomShuffleExistingUnique,
            RandomExistingAllowRepeat,
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
        
        
        
        
        
        /// <summary> The CheckpointManager gets player info from the playerManager and listens for joining/leaving players. </summary>
        [Header("Components")]
        public PlayerManager playerManager;


        /// <summary> Stores references to all checkpoints. By default, the order/index of the list is used for the checkpoint sequence. </summary>
        [Header("Settings")]
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
            checkpointTrack.SetDefaultCheckpointLayer(defaultCheckpointLayerIndex);
            checkpointTrack.OnPossessableReachedCheckpoint += HandlePossessableReachedCheckpointEvent;
        }
        private void TerminateCheckpointTrack(CheckpointTrack checkpointTrack) => checkpointTrack.OnPossessableReachedCheckpoint -= HandlePossessableReachedCheckpointEvent;

        public PlayerTargetMode playerTargetMode;
        private List<Checkpoint> currentSharedTargets;

        public TargetSelectionProcedure targetSelectionProcedure;

        /// <summary> ONLY USED FOR <see cref="CheckpointVisibilityMethod.SingleObjLayering"/>. Default layer of the checkpoint objects. Make this layer hidden to cameras (Remove from culling). </summary>
        [Header("Single Object Layering Settings")]
        public string defaultCheckpointLayer = "checkpoints";

        private int defaultCheckpointLayerIndex = 0;

        [ShowInInspector, Sirenix.OdinInspector.ReadOnly]
        private Dictionary<PlayerInfo, CheckpointReachedPlayerData> currentPlayerCheckpointStatus;

        
        
        #region Events & EventHandling

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
            defaultCheckpointLayerIndex = LayerMask.NameToLayer(defaultCheckpointLayer);
            if (playerManager == null)
                playerManager = FindObjectOfType<PlayerManager>();

            // Make sure the Proeprty Setter gets invoked for registering to events.
            if (activeCheckpointTrack != null) InitCheckpointTrack(ActiveCheckpointTrack);

            if (playerTargetMode == PlayerTargetMode.SharedTarget)
                currentSharedTargets = new List<Checkpoint>(){activeCheckpointTrack?.GetFirstCheckpoint()};
            
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


        public void ResetAllPlayerCheckpointStatus()
        {
            currentPlayerCheckpointStatus = new Dictionary<PlayerInfo, CheckpointReachedPlayerData>();
        }

        public void ResetPlayerCheckpointStatus(PlayerInfo playerInfo)
        {
            if (currentPlayerCheckpointStatus?.ContainsKey(playerInfo) ?? false)
            {
                currentPlayerCheckpointStatus.Remove(playerInfo);
            }
        }
        
        // Gets executed when a possessable passes a [Checkpoint]
        private void HandlePossessableReachedCheckpointEvent(Checkpoint checkpoint, Possessable possessable)
        {
            if (playerManager?.playerInfos == null) return;

            var playerInfo = playerManager.playerInfos.FirstOrDefault(playerInfoEntry => playerInfoEntry.controller?.possessable == possessable);
            
            if (playerInfo.Equals(default(PlayerInfo)) || (playerTargetMode == PlayerTargetMode.SharedTarget && !currentSharedTargets.Contains(checkpoint)))
                return;
            
            UpdatePlayerCheckpointStatus(playerInfo, checkpoint);
            
            if(playerTargetMode == PlayerTargetMode.SharedTarget)
                currentSharedTargets = checkpoint.nextCheckpoints;
        }

        // Updates the last reached checkpoint of a player
        private void UpdatePlayerCheckpointStatus(PlayerInfo playerInfo, Checkpoint checkpoint)
        {
            if (currentPlayerCheckpointStatus == null)
                currentPlayerCheckpointStatus = new Dictionary<PlayerInfo, CheckpointReachedPlayerData>();

            CheckpointReachedPlayerData data;
            if (currentPlayerCheckpointStatus.ContainsKey(playerInfo))
            {
                data = new CheckpointReachedPlayerData(playerInfo, checkpoint, Time.time, currentPlayerCheckpointStatus[playerInfo]);
                currentPlayerCheckpointStatus[playerInfo] = data;
            }
            else
            {
                data = new CheckpointReachedPlayerData(playerInfo, checkpoint, Time.time, null);
                currentPlayerCheckpointStatus.Add(playerInfo, data);
            }
        
            NotifyPlayerReachedCheckpoint(data);
            if(IsLastCheckpoint(checkpoint))
                NotifyPlayerReachedLastCheckpoint(data);
        }
        
        private void HandleNewPlayerJoinedEvent(PlayerInfo playerinfo){}
        
        private void HandlePlayerLeftGameEvent(PlayerInfo playerinfo)
        {
            
            ResetPlayerCheckpointStatus(playerinfo);
        }
        
        public Checkpoint GetLastReachedCheckpoint(PlayerInfo playerInfo)
        {
            return (!currentPlayerCheckpointStatus?.ContainsKey(playerInfo) ?? true)
                ? null
                : currentPlayerCheckpointStatus[playerInfo].checkpoint;
        }
        
        public List<Checkpoint> GetCurrentPlayerCheckpointTargets(PlayerInfo playerInfo)
        {
            if (playerTargetMode == PlayerTargetMode.SharedTarget)
            {
                return currentSharedTargets ?? new List<Checkpoint>() {activeCheckpointTrack?.GetFirstCheckpoint()};
            }
            else
            {
                return GetLastReachedCheckpoint(playerInfo)?.nextCheckpoints ?? new List<Checkpoint>(){activeCheckpointTrack?.GetFirstCheckpoint()};
            }
            
            
        }

        public bool IsLastCheckpoint(Checkpoint checkpoint)
        {
            return (checkpoint?.nextCheckpoints?.Count ?? 0) == 0;
        }

        public CheckpointReachedPlayerData GetCurrentPlayerCheckpointData(PlayerInfo playerInfo)
        {
            return (!currentPlayerCheckpointStatus?.ContainsKey(playerInfo) ?? true)
                ? null
                : currentPlayerCheckpointStatus[playerInfo];
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
                checkpoint.gameObject.layer = defaultCheckpointLayerIndex);
        }
    }
}
