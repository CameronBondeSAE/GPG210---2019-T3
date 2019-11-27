/*
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
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
public class CheckpointManagerBackup : MonoBehaviour
    {
        /**
         * CheckpointVisibilityMethod : enum Defines how the checkpoints are being displayed/hidden for each player.
         #1#
        private enum CheckpointVisibilityMethod
        {
            /// <summary> There is a single gameobject per checkpoint and visibility per player is done through changing the layer to the according player specfic layer. Note: This might be inperformant(?) since for each player camera a pre-render and post-render method will be called.</summary>
            SingleObjLayering,
            /// There is a checkpoint-gameobject for each player and checkpoint. May be more performant? (Uses more gameobjects. Total Amount: ([No of players]+1) * [No of checkpoints]
            PerPlayerObjLayering
        }
        
        
        
        
        
        /// <summary> The CheckpointManager gets player info from the playerManager and listens for joining/leaving players. </summary>
        [Header("Components")]
        public PlayerManager playerManager;

        
        /// <summary> Stores references to all checkpoints. By default, the order/index of the list is used for the checkpoint sequence. </summary>
        [Header("Settings")]
        public List<Checkpoint> checkpoints = null;
        [ShowInInspector, SerializeField, Tooltip("SingleObjLayering: Less gameobjects, 2 possibily expensive method-calls per camera/frame; PerPlayerObjLayering: More gameobjects, possibly more efficient?")]
        private CheckpointVisibilityMethod checkpointVisibilityMethod = CheckpointVisibilityMethod.SingleObjLayering;
        
        /// <summary> ONLY USED FOR <see cref="CheckpointVisibilityMethod.SingleObjLayering"/>. Default layer of the checkpoint objects. Make this layer hidden to cameras (Remove from culling). </summary>
        [Header("Single Object Layering Settings")]
        public string defaultCheckpointLayer = "checkpoints";

        //[Header("Per Player Object Layering Settings")]
        private Dictionary<PlayerInfo, List<Checkpoint>> playerCheckpoints;
        
        //private Dictionary<PlayerInfo, int> currentPlayerCheckpointStatus;
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
            if (playerManager == null)
                playerManager = FindObjectOfType<PlayerManager>();
            
            
            switch (checkpointVisibilityMethod)
            {
                case CheckpointVisibilityMethod.SingleObjLayering:
                    //Camera.onPreCull += HandleCameraPreCullEvent;
                    //Camera.onPostRender += HandleCameraPostRenderEvent;
                    RenderPipelineManager.beginCameraRendering += HandleBeginCameraRenderingEvent;
                    RenderPipelineManager.endCameraRendering += HandleEndCameraRenderingEvent;
                    break;
                case CheckpointVisibilityMethod.PerPlayerObjLayering:
                    playerCheckpoints = new Dictionary<PlayerInfo, List<Checkpoint>>();
                    break;
            }
            
            ResetAllPlayerCheckpointStatus();
            ResetCheckpoints();

            if (playerManager != null)
            {
                playerManager.OnNewPlayerJoinedGame += HandleNewPlayerJoinedEvent;
                playerManager.OnPlayerLeftGame += HandlePlayerLeftGameEvent;
            }
                

        }

        private void OnDestroy()
        {
            checkpoints?.ForEach(checkpoint =>
            {
                checkpoint.OnPossessableEnteredCheckpoint -= HandlePossessableReachedCheckpointEvent;
            });
            
            
            if (playerManager != null)
            {
                playerManager.OnNewPlayerJoinedGame -= HandleNewPlayerJoinedEvent;
                playerManager.OnPlayerLeftGame -= HandlePlayerLeftGameEvent;
            }

            if (checkpointVisibilityMethod == CheckpointVisibilityMethod.SingleObjLayering)
            {
                //Camera.onPreCull -= HandleCameraPreCullEvent;
                //Camera.onPostRender -= HandleCameraPostRenderEvent;
                RenderPipelineManager.beginCameraRendering -= HandleBeginCameraRenderingEvent;
                RenderPipelineManager.endCameraRendering -= HandleEndCameraRenderingEvent;
            }
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

        private void ResetCheckpoints()
        {
            if (checkpoints == null || checkpoints.Count <= 0) return;
            
            switch (checkpointVisibilityMethod)
            {
                case CheckpointVisibilityMethod.PerPlayerObjLayering:
                    // Delete existing player Checkpoints
                    playerCheckpoints?.Keys.ForEach(RemovePlayerCheckpoints);

                    // Set correct layer & hide "prefab" checkpoints
                    checkpoints?.ForEach(checkpoint =>
                    {
                        checkpoint.gameObject.layer = LayerMask.NameToLayer(defaultCheckpointLayer);
                        checkpoint.gameObject.SetActive(false);
                    });
                    
                    /*

                    playerCheckpoints?.Values.ForEach(playerCheckpointList =>
                    {
                        playerCheckpointList?.ForEach(playerCheckpoint =>
                        {
                            playerCheckpoint.OnPlayerEnteredCheckpoint -= HandlePossessableReachedCheckpointEvent;
                            Destroy(playerCheckpoint.gameObject);
                        });
                    });
                    playerCheckpoints = new Dictionary<PlayerInfo, List<Checkpoint>>();#1#
                

                    playerManager?.playerInfos?.Where(playerInfo => !playerInfo.Equals(default(PlayerInfo))).ForEach(playerInfo =>
                    {
                        InitNewPlayer(playerInfo);
                        UpdateCheckpointVisibility(playerInfo);
                    });
                    break;
                case CheckpointVisibilityMethod.SingleObjLayering:
                    checkpoints.Where(checkpoint => checkpoint != null).ForEach(checkpoint =>
                    {
                        checkpoint.OnPossessableEnteredCheckpoint += HandlePossessableReachedCheckpointEvent;
                        checkpoint.gameObject.layer = LayerMask.NameToLayer(defaultCheckpointLayer);
                    });
                    break;
            }
        }
        
        // Gets executed when a possessable passes a [Checkpoint]
        private void HandlePossessableReachedCheckpointEvent(Checkpoint checkpoint, Possessable possessable)
        {
            if (playerManager?.playerInfos == null) return;

            var playerInfo = playerManager.playerInfos.FirstOrDefault(playerInfoEntry => playerInfoEntry.controller?.possessable == possessable);
            
            if (playerInfo.Equals(default(PlayerInfo)))
                return;

            var checkpointIndex = -1;
            switch (checkpointVisibilityMethod)
            {
                case CheckpointVisibilityMethod.SingleObjLayering:
                    checkpointIndex = checkpoints?.IndexOf(checkpoint) ?? -1;
                    break;
                case CheckpointVisibilityMethod.PerPlayerObjLayering:
                    checkpointIndex = playerCheckpoints?.ContainsKey(playerInfo) ?? false ? playerCheckpoints?[playerInfo]?.IndexOf(checkpoint) ?? -1 : -1;
                    break;
            }
            
            UpdatePlayerCheckpointStatus(playerInfo, checkpointIndex);
        }
        
        private void HandlePlayerLeftGameEvent(PlayerInfo playerinfo)
        {
            
            Profiler.BeginSample("Player Left Game", gameObject);
            ResetPlayerCheckpointStatus(playerinfo);
            if (checkpointVisibilityMethod == CheckpointVisibilityMethod.PerPlayerObjLayering)
            {
                RemovePlayerCheckpoints(playerinfo);
            }
            Profiler.EndSample();
        }

        // Updates the last reached checkpoint of a player
        private void UpdatePlayerCheckpointStatus(PlayerInfo playerInfo, int checkPoint)
        {
            if (currentPlayerCheckpointStatus == null)
                currentPlayerCheckpointStatus = new Dictionary<PlayerInfo, CheckpointReachedPlayerData>();

            if (currentPlayerCheckpointStatus.ContainsKey(playerInfo))
            {
                currentPlayerCheckpointStatus[playerInfo] = new CheckpointReachedPlayerData(playerInfo, checkPoint, Time.time, currentPlayerCheckpointStatus[playerInfo]);
            }
            else
            {
                currentPlayerCheckpointStatus.Add(playerInfo, new CheckpointReachedPlayerData(playerInfo, checkPoint, Time.time, null));
            }
        
            NotifyPlayerReachedCheckpoint(currentPlayerCheckpointStatus[playerInfo]);
            if(IsLastCheckpoint(checkPoint))
                NotifyPlayerReachedLastCheckpoint(currentPlayerCheckpointStatus[playerInfo]);
        
            
            if (checkpointVisibilityMethod == CheckpointVisibilityMethod.PerPlayerObjLayering)
                UpdateCheckpointVisibility(playerInfo);
        }
        
        public int GetLastReachedCheckpoint(PlayerInfo playerInfo)
        {
            return (!currentPlayerCheckpointStatus?.ContainsKey(playerInfo) ?? true)
                ? -1
                : currentPlayerCheckpointStatus[playerInfo].checkpointIndex;
        }
        public int GetCurrentPlayerCheckpointTarget(PlayerInfo playerInfo)
        {
            return (!currentPlayerCheckpointStatus?.ContainsKey(playerInfo) ?? true)
                ? (checkpoints == null || checkpoints.Count == 0 ? -1 : 0)
                : (IsLastCheckpoint(currentPlayerCheckpointStatus[playerInfo].checkpointIndex) ? -1 : currentPlayerCheckpointStatus[playerInfo].checkpointIndex+1);
        }

        public bool IsLastCheckpoint(int checkpointIndex)
        {
            return checkpointIndex == (checkpoints?.Count -1 ?? -1);
        }

        public CheckpointReachedPlayerData GetCurrentPlayerCheckpointData(PlayerInfo playerInfo)
        {
            return (!currentPlayerCheckpointStatus?.ContainsKey(playerInfo) ?? true)
                ? null
                : currentPlayerCheckpointStatus[playerInfo];
        }

        #region Checkpoint Visibility Method: SingleObjLayering Specific Functions

        private void HandleBeginCameraRenderingEvent(ScriptableRenderContext arg1, Camera cam)
        {
            var playerInfo = playerManager.playerInfos.FirstOrDefault(pi => pi.realCamera == cam);

            if (playerInfo.Equals(default(PlayerInfo)))
                return;
            
            var curCheckpointTargetIndex = GetCurrentPlayerCheckpointTarget(playerInfo);
            var curCheckpointTarget = (curCheckpointTargetIndex >= 0 && curCheckpointTargetIndex < checkpoints.Count) ? checkpoints[curCheckpointTargetIndex] : null;

            if (curCheckpointTarget != null)
            {
                curCheckpointTarget.gameObject.layer = playerInfo.virtualCameraLayer;
            }
        }

        private void HandleEndCameraRenderingEvent(ScriptableRenderContext arg1, Camera cam)
        {
            var playerInfo = playerManager.playerInfos.FirstOrDefault(pi => pi.realCamera == cam);
            
            if (playerInfo.Equals(default(PlayerInfo)))
                return;
            
            var curCheckpointTargetIndex = GetCurrentPlayerCheckpointTarget(playerInfo);
            var curCheckpointTarget = (curCheckpointTargetIndex >= 0 && curCheckpointTargetIndex < checkpoints.Count) ? checkpoints[curCheckpointTargetIndex] : null;

            if (curCheckpointTarget != null)
                curCheckpointTarget.gameObject.layer = LayerMask.NameToLayer(defaultCheckpointLayer);
        }
        
        /*private void HandleCameraPreCullEvent(Camera cam)
        {
            Debug.Log("Pre Cull Event "+cam.name);
            var playerInfo = playerManager.playerInfos.FirstOrDefault(pi => pi.realCamera == cam);

            if (playerInfo.Equals(default(PlayerInfo)))
            {
                Debug.Log("PreCull: Didn't find playerInfo of cam "+cam.name+" players: "+playerManager.playerInfos.Count+" CamTag: "+cam.gameObject.tag+" layer: "+cam.gameObject.layer);
                return;
            }
            
            var curCheckpointTargetIndex = GetCurrentPlayerCheckpointTarget(playerInfo);
            var curCheckpointTarget = (curCheckpointTargetIndex >= 0 && curCheckpointTargetIndex < checkpoints.Count) ? checkpoints[curCheckpointTargetIndex] : null;

            if (curCheckpointTarget != null)
            {
                Debug.Log("PreCull: "+ cam.name +"       ------- YESSSSS "+playerInfo.virtualCameraLayer);
                curCheckpointTarget.gameObject.layer = playerInfo.virtualCameraLayer;
            }
            else
            {
                Debug.Log("PreCull: "+ cam.name+" - "+curCheckpointTargetIndex+" - "+playerInfo);
            }
                
        }#1#



        /*private void HandleCameraPostRenderEvent(Camera cam)
        {
            Debug.Log("Post Render Event "+cam.name);
            /*PlayerInfo playerInfo = playerManager.playerInfos.First(pi => pi.realCamera == cam);
            
            var curCheckpointTargetIndex = GetCurrentPlayerCheckpointTarget(playerInfo);
            var curCheckpointTarget = (curCheckpointTargetIndex >= 0 && curCheckpointTargetIndex < checkpoints.Count) ? checkpoints[curCheckpointTargetIndex] : null;

            if (curCheckpointTarget != null)
                curCheckpointTarget.gameObject.layer = LayerMask.NameToLayer(defaultCheckpointLayer);#2#
        }#1#

        #endregion
        
        #region Checkpoint Visibility Method: PerPlayerObjLayering Specific Functions


        private void RemovePlayerCheckpoints(PlayerInfo playerInfo)
        {
            if (!(playerCheckpoints?.ContainsKey(playerInfo) ?? false) || checkpointVisibilityMethod != CheckpointVisibilityMethod.PerPlayerObjLayering) return;
            
            playerCheckpoints[playerInfo]?.ForEach(playerCheckpoint =>
            {
                playerCheckpoint.OnPossessableEnteredCheckpoint -= HandlePossessableReachedCheckpointEvent;
                Destroy(playerCheckpoint.gameObject);
            });
            playerCheckpoints.Remove(playerInfo);
        }
        
        private void HandleNewPlayerJoinedEvent(PlayerInfo playerinfo)
        {
            if (checkpointVisibilityMethod != CheckpointVisibilityMethod.PerPlayerObjLayering)
                return;
                
            Debug.Log("Player joined:" + playerinfo.playerInput.playerIndex);
            Profiler.BeginSample("NewPLayer Instantiations", gameObject);
            InitNewPlayer(playerinfo);
            UpdateCheckpointVisibility(playerinfo); 
            Profiler.EndSample();
        }
        
        // Initializes a new player
        private void InitNewPlayer(PlayerInfo playerInfo)
        {
            if (checkpointVisibilityMethod != CheckpointVisibilityMethod.PerPlayerObjLayering)
                return;
            
            checkpoints?.ForEach(checkpoint =>
            {
                InitPlayerCheckpoint(playerInfo, checkpoint);
            });
        }
        
        // Spawns & Initializes a copy of a given checkpoint for a given player
        private void InitPlayerCheckpoint(PlayerInfo playerInfo, Checkpoint checkpoint)
        {
            if (checkpointVisibilityMethod != CheckpointVisibilityMethod.PerPlayerObjLayering)
                return;
            
            if(playerCheckpoints == null)
                playerCheckpoints = new Dictionary<PlayerInfo, List<Checkpoint>>();
            
            var playerCheckpointObj = Instantiate(checkpoint.gameObject);
            playerCheckpointObj.layer = playerInfo.virtualCameraLayer;
            playerCheckpointObj.SetActive(false);
            var playerCheckpoint = playerCheckpointObj.GetComponent<Checkpoint>();
            playerCheckpoint.OnPossessableEnteredCheckpoint += HandlePossessableReachedCheckpointEvent;
                                
            if (playerCheckpoint == null)
                playerCheckpoint = playerCheckpointObj.AddComponent<Checkpoint>();

            if (playerCheckpoints.ContainsKey(playerInfo))
            {
                playerCheckpoints[playerInfo].Add(playerCheckpoint);
            }
            else
            {
                playerCheckpoints.Add(playerInfo, new List<Checkpoint>(){playerCheckpoint});
            }
        }
        
        // Toggle visibility of the last reached checkpoint and the next target.
        private void UpdateCheckpointVisibility(PlayerInfo playerInfo)
        {
            if(!(playerCheckpoints?.ContainsKey(playerInfo) ?? false) || (playerCheckpoints[playerInfo]?.Count ?? 0) == 0)
                return;
            
            var lastReachedCheckpointIndex = GetLastReachedCheckpoint(playerInfo);//currentPlayerCheckpointStatus[playerInfo];
            var nextCheckpointIndex = GetCurrentPlayerCheckpointTarget(playerInfo);
            
            /*var reachedCheckpoint = (lastReachedCheckpointIndex >= 0 && lastReachedCheckpointIndex < checkpoints.Count) ? checkpoints[lastReachedCheckpointIndex] : null;
            var nextCheckpoint = (nextCheckpointIndex >= 0 && nextCheckpointIndex < checkpoints.Count) ? checkpoints[nextCheckpointIndex] : null;#1#

            var reachedCheckpoint = (lastReachedCheckpointIndex >= 0 && lastReachedCheckpointIndex < playerCheckpoints[playerInfo].Count) ? playerCheckpoints[playerInfo][lastReachedCheckpointIndex] : null;
            var nextCheckpoint = (nextCheckpointIndex >= 0 && nextCheckpointIndex < playerCheckpoints[playerInfo].Count) ? playerCheckpoints[playerInfo][nextCheckpointIndex] : null;
            
            reachedCheckpoint?.gameObject.SetActive(false);
            nextCheckpoint?.gameObject.SetActive(true);
        }

        #endregion
    }
}
*/
