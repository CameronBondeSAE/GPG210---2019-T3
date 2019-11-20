using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Students.Luca.Scripts.Checkpoints
{
    public class CheckpointManager : MonoBehaviour
    {
        public PlayerManager playerManager;
    
        public List<Checkpoint> checkpoints = null;
        private Dictionary<PlayerInfo, int> currentPlayerCheckpointStatus = new Dictionary<PlayerInfo, int>();


        #region Events & EventHandling

        public delegate void PlayerReachedCheckpointDel(PlayerInfo playerInfo, int checkPointIndex);

        public event PlayerReachedCheckpointDel OnPlayerReachedCheckpoint;
        public event PlayerReachedCheckpointDel OnPlayerReachedLastCheckpoint;

        protected void NotifyPlayerReachedCheckpoint(PlayerInfo playerInfo, int checkPointIndex)
        {
            OnPlayerReachedCheckpoint?.Invoke(playerInfo, checkPointIndex);
        }
        protected void NotifyPlayerReachedLastCheckpoint(PlayerInfo playerInfo)
        {
            OnPlayerReachedLastCheckpoint?.Invoke(playerInfo, checkpoints?.Count ?? 0);
        }

        private void HandlePossessableReachedCheckpointEvent(Checkpoint checkpoint, Possessable possessable)
        {
            if (playerManager?.playerInfos == null) return;
        
            PlayerInfo? playerInfo = null;
            var checkpointIndex = checkpoints.IndexOf(checkpoint);
            
            foreach (var playerInfoEntry in playerManager.playerInfos.Where(playerInfoEntry => playerInfoEntry.playerCharacterPossessable == possessable))
            {
                playerInfo = playerInfoEntry;
                break;
            }

            if (playerInfo.HasValue && checkpointIndex >= 0)
            {
                UpdatePlayerCheckpointStatus(playerInfo.Value, checkpointIndex);
                //Debug.Log("Reached a checkpoint! "+ possessable + " - "+checkpoint + "  --  "+checkpointIndex +"  --- "+GetCurrentPlayerCheckpointTarget(playerInfo.Value));
            }

        }
    
    
        #endregion
    
    
        // Start is called before the first frame update
        void Start()
        {
            if (!playerManager)
                playerManager = FindObjectOfType<TMPTestPlayerManager>(); // TODO TESTING; CHANGE TO PlayerManager

            if (checkpoints != null && checkpoints.Count > 0)
            {
                foreach (var checkpoint in checkpoints)
                {
                    if(checkpoint != null)
                        checkpoint.OnPlayerEnteredCheckpoint += HandlePossessableReachedCheckpointEvent;
                }
            }
        
            // TODO TEMP TEST CODE
            UpdatePlayerCheckpointStatus(playerManager.playerInfos[0], 0);
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
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void UpdatePlayerCheckpointStatus(PlayerInfo player, int checkPoint)
        {
            if (currentPlayerCheckpointStatus == null)
                currentPlayerCheckpointStatus = new Dictionary<PlayerInfo, int>();

            if (currentPlayerCheckpointStatus.ContainsKey(player))
            {
                currentPlayerCheckpointStatus[player] = checkPoint;
            }
            else
            {
                currentPlayerCheckpointStatus.Add(player, checkPoint);
            }
        
            NotifyPlayerReachedCheckpoint(player, checkPoint);
            if(IsLastCheckpoint(checkPoint))
                NotifyPlayerReachedLastCheckpoint(player);
        
            UpdateCheckpointVisibility(player);
        }

        // reachedCheckpoint = the checkpoint the player just reached = hiding it!
        private void UpdateCheckpointVisibility(PlayerInfo playerInfo)
        {
            var lastReachedCheckpointIndex = currentPlayerCheckpointStatus[playerInfo];
            var nextCheckpointIndex = GetCurrentPlayerCheckpointTarget(playerInfo);
            // TODO hide reachedCheckpoint, show next checkpoint

            var reachedCheckpoint = (lastReachedCheckpointIndex >= 0 && lastReachedCheckpointIndex < checkpoints.Count) ? checkpoints[lastReachedCheckpointIndex] : null;
            var nextCheckpoint = (nextCheckpointIndex >= 0 && nextCheckpointIndex < checkpoints.Count) ? checkpoints[nextCheckpointIndex] : null;

            //Debug.Log(reachedCheckpoint +" "+lastReachedCheckpointIndex+"   ----   "+nextCheckpoint +" "+nextCheckpointIndex);
            
            // TODO Enable/Disable only for given player
            reachedCheckpoint?.gameObject.SetActive(false);
            nextCheckpoint?.gameObject.SetActive(true);
        }

        public int GetCurrentPlayerCheckpointTarget(PlayerInfo playerInfo)
        {
            return (!currentPlayerCheckpointStatus?.ContainsKey(playerInfo) ?? true)
                ? (checkpoints == null || checkpoints.Count == 0 ? -1 : 0)
                : (IsLastCheckpoint(currentPlayerCheckpointStatus[playerInfo]) ? -1 : currentPlayerCheckpointStatus[playerInfo]+1);
        }

        public bool IsLastCheckpoint(int checkpointIndex)
        {
            return checkpointIndex == (checkpoints?.Count ?? -1);
        }
    }
}
