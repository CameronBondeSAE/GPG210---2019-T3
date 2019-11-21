using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Rendering;

namespace Students.Luca.Scripts.Checkpoints
{
    public class CheckpointManager : MonoBehaviour
    {
        /**
         * CheckpointVisibilityMethod : enum Defines how the checkpoints are being displayed/hidden for each player.
         * SingleObjLayering: There is a single gameobject per checkpoint & visibility per player is done through changing the layer
         *     to the according player specfic layer.
         *     Note: This might be inperformant(?) since for each player camera a pre-render and post-render method will be called.
         * PerPlayerObjLayering: There is a checkpoint-gameobject for each player & checkpoint.
         *     May be more performant? (Uses more gameobjects. Total Amount: ([No of players]+1) * [No of checkpoints]
         */
        private enum CheckpointVisibilityMethod
        {
            SingleObjLayering,
            PerPlayerObjLayering
        }
        
        [Header("Components")]
        public PlayerManager playerManager;

        [Header("Settings")]
        public List<Checkpoint> checkpoints = null;
        [ShowInInspector, SerializeField, Tooltip("SingleObjLayering: Less gameobjects, 2 possibily expensive method-calls per camera/frame; PerPlayerObjLayering: More gameobjects, possibly more efficient?")]
        private CheckpointVisibilityMethod checkpointVisibilityMethod = CheckpointVisibilityMethod.SingleObjLayering;
        [Header("Single Object Layering Settings")]
        public string defaultCheckpointLayer = "checkpoints";

        //[Header("Per Player Object Layering Settings")]
        private Dictionary<PlayerInfo, List<Checkpoint>> playerCheckpoints;
        
        //private Dictionary<PlayerInfo, int> currentPlayerCheckpointStatus;
        private Dictionary<PlayerInfo, CheckpointReachedPlayerData> currentPlayerCheckpointStatus;

        #region Events & EventHandling

        public delegate void PlayerReachedCheckpointDel(PlayerInfo playerInfo, int checkPointIndex);

        public event PlayerReachedCheckpointDel OnPlayerReachedCheckpoint;
        public event PlayerReachedCheckpointDel OnPlayerReachedLastCheckpoint;

        private void NotifyPlayerReachedCheckpoint(PlayerInfo playerInfo, int checkPointIndex)
        {
            OnPlayerReachedCheckpoint?.Invoke(playerInfo, checkPointIndex);
        }

        private void NotifyPlayerReachedLastCheckpoint(PlayerInfo playerInfo)
        {
            OnPlayerReachedLastCheckpoint?.Invoke(playerInfo, checkpoints?.Count ?? 0);

            CheckpointReachedPlayerData playerData = GetCurrentPlayerCheckpointData(playerInfo);
            Debug.Log("Player "+playerInfo.realCamera.name+" reached the last checkpoint! #Checkpoints reached: "+playerData?.GetReachedCheckpointsCount()+" - Total Time: "+playerData?.GetTotalTimeSinceFirstCheckpoint());
        }

        private void HandlePossessableReachedCheckpointEvent(Checkpoint checkpoint, Possessable possessable)
        {
            if (playerManager?.playerInfos == null) return;

            var playerInfo = playerManager.playerInfos.FirstOrDefault(playerInfoEntry => playerInfoEntry.playerCharacterPossessable == possessable);
            if (playerInfo.Equals(default(PlayerInfo)))
                return;
            //Debug.Log("HandlePossessableReachedCheckpointEvent: "+checkpoint.gameObject.layer+" "+possessable.name);

            var checkpointIndex = -1;
            switch (checkpointVisibilityMethod)
            {
                case CheckpointVisibilityMethod.SingleObjLayering:
                    checkpointIndex = checkpoints?.IndexOf(checkpoint) ?? -1;
                    break;
                case CheckpointVisibilityMethod.PerPlayerObjLayering:
                    checkpointIndex = playerCheckpoints?[playerInfo]?.IndexOf(checkpoint) ?? -1;
                    break;
            }
            
            UpdatePlayerCheckpointStatus(playerInfo, checkpointIndex);

        }
    
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
    
        #endregion
    
    
        // Start is called before the first frame update
        private void Start()
        {
            if (playerManager == null)
                playerManager = FindObjectOfType<TMPTestPlayerManager>(); // TODO TESTING; CHANGE TO PlayerManager
            
            ResetPlayerCheckpointStatus();
            ResetCheckpoints();

            if (checkpointVisibilityMethod == CheckpointVisibilityMethod.SingleObjLayering)
            {
                //Camera.onPreCull += HandleCameraPreCullEvent;
                //Camera.onPostRender += HandleCameraPostRenderEvent;
                RenderPipelineManager.beginCameraRendering += HandleBeginCameraRenderingEvent;
                RenderPipelineManager.endCameraRendering += HandleEndCameraRenderingEvent;
            }
        }

        public void ResetPlayerCheckpointStatus()
        {
            currentPlayerCheckpointStatus = new Dictionary<PlayerInfo, CheckpointReachedPlayerData>();
        }

        private void ResetCheckpoints()
        {
            if (checkpointVisibilityMethod == CheckpointVisibilityMethod.PerPlayerObjLayering)
            {
                // Delete existing player Checkpoints
                playerCheckpoints?.Values.ForEach(playerCheckpointList =>
                {
                    playerCheckpointList?.ForEach(playerCheckpoint =>
                    {
                        playerCheckpoint.OnPlayerEnteredCheckpoint -= HandlePossessableReachedCheckpointEvent;
                        Destroy(playerCheckpoint.gameObject);
                    });
                });
                
                playerCheckpoints = new Dictionary<PlayerInfo, List<Checkpoint>>();
            }
                
            
            if (checkpoints == null || checkpoints.Count <= 0) return;
            
            foreach (var checkpoint in checkpoints.Where(checkpoint => checkpoint != null))
            {
                switch (checkpointVisibilityMethod)
                {
                    default:
                    case CheckpointVisibilityMethod.SingleObjLayering:
                        checkpoint.OnPlayerEnteredCheckpoint += HandlePossessableReachedCheckpointEvent;
                        checkpoint.gameObject.layer = LayerMask.NameToLayer(defaultCheckpointLayer);
                        break;
                    case CheckpointVisibilityMethod.PerPlayerObjLayering:
                    {
                        playerManager?.playerInfos?.ForEach(playerInfo =>
                        {
                            var playerCheckpointObj = Instantiate(checkpoint.gameObject);
                            playerCheckpointObj.layer = playerInfo.virtualCameraLayer;
                            playerCheckpointObj.SetActive(false);
                            var playerCheckpoint = playerCheckpointObj.GetComponent<Checkpoint>();
                            playerCheckpoint.OnPlayerEnteredCheckpoint += HandlePossessableReachedCheckpointEvent;
                                
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
                        });
                        
                        checkpoint.gameObject.SetActive(false);
                        break;
                    }
                }
            }

            if (checkpointVisibilityMethod == CheckpointVisibilityMethod.PerPlayerObjLayering)
                playerManager?.playerInfos?.ForEach(UpdateCheckpointVisibility);
        }

        private void OnDestroy()
        {
            if (checkpoints != null && checkpoints.Count > 0)
            {
                foreach (var checkpoint in checkpoints)
                {
                    checkpoint.OnPlayerEnteredCheckpoint -= HandlePossessableReachedCheckpointEvent;
                }
            }

            if (checkpointVisibilityMethod == CheckpointVisibilityMethod.SingleObjLayering)
            {
                //Camera.onPreCull -= HandleCameraPreCullEvent;
                //Camera.onPostRender -= HandleCameraPostRenderEvent;
                RenderPipelineManager.beginCameraRendering -= HandleBeginCameraRenderingEvent;
                RenderPipelineManager.endCameraRendering -= HandleEndCameraRenderingEvent;
            }
            
        }

        private void UpdatePlayerCheckpointStatus(PlayerInfo playerInfo, int checkPoint)
        {
            //Debug.Log("Upt Player Chkp Status: "+playerInfo.realCamera.name+" "+checkPoint);
            if (currentPlayerCheckpointStatus == null)
                currentPlayerCheckpointStatus = new Dictionary<PlayerInfo, CheckpointReachedPlayerData>();

            if (currentPlayerCheckpointStatus.ContainsKey(playerInfo))
            {
                currentPlayerCheckpointStatus[playerInfo] = new CheckpointReachedPlayerData(checkPoint, Time.time, currentPlayerCheckpointStatus[playerInfo]);
            }
            else
            {
                currentPlayerCheckpointStatus.Add(playerInfo, new CheckpointReachedPlayerData(checkPoint, Time.time, null));
            }
        
            NotifyPlayerReachedCheckpoint(playerInfo, checkPoint);
            if(IsLastCheckpoint(checkPoint))
                NotifyPlayerReachedLastCheckpoint(playerInfo);
        
            
            if (checkpointVisibilityMethod == CheckpointVisibilityMethod.PerPlayerObjLayering)
                UpdateCheckpointVisibility(playerInfo);
        }

        // reachedCheckpoint = the checkpoint the player just reached = hiding it!
        private void UpdateCheckpointVisibility(PlayerInfo playerInfo)
        {
            if(!(playerCheckpoints?.ContainsKey(playerInfo) ?? false) || (playerCheckpoints[playerInfo]?.Count ?? 0) == 0)
                return;
            
            var lastReachedCheckpointIndex = GetLastReachedCheckpoint(playerInfo);//currentPlayerCheckpointStatus[playerInfo];
            var nextCheckpointIndex = GetCurrentPlayerCheckpointTarget(playerInfo);
            
            /*var reachedCheckpoint = (lastReachedCheckpointIndex >= 0 && lastReachedCheckpointIndex < checkpoints.Count) ? checkpoints[lastReachedCheckpointIndex] : null;
            var nextCheckpoint = (nextCheckpointIndex >= 0 && nextCheckpointIndex < checkpoints.Count) ? checkpoints[nextCheckpointIndex] : null;*/

            var reachedCheckpoint = (lastReachedCheckpointIndex >= 0 && lastReachedCheckpointIndex < playerCheckpoints[playerInfo].Count) ? playerCheckpoints[playerInfo][lastReachedCheckpointIndex] : null;
            var nextCheckpoint = (nextCheckpointIndex >= 0 && nextCheckpointIndex < playerCheckpoints[playerInfo].Count) ? playerCheckpoints[playerInfo][nextCheckpointIndex] : null;
            
            reachedCheckpoint?.gameObject.SetActive(false);
            nextCheckpoint?.gameObject.SetActive(true);
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
                
        }*/



        /*private void HandleCameraPostRenderEvent(Camera cam)
        {
            Debug.Log("Post Render Event "+cam.name);
            /*PlayerInfo playerInfo = playerManager.playerInfos.First(pi => pi.realCamera == cam);
            
            var curCheckpointTargetIndex = GetCurrentPlayerCheckpointTarget(playerInfo);
            var curCheckpointTarget = (curCheckpointTargetIndex >= 0 && curCheckpointTargetIndex < checkpoints.Count) ? checkpoints[curCheckpointTargetIndex] : null;

            if (curCheckpointTarget != null)
                curCheckpointTarget.gameObject.layer = LayerMask.NameToLayer(defaultCheckpointLayer);#1#
        }*/

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
    }
}
