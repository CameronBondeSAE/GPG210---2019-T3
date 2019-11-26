namespace Students.Luca.Scripts.Checkpoints
{
    public class CheckpointReachedPlayerData
    {
        public readonly PlayerInfo playerInfo;
        public readonly CheckpointReachedPlayerData previousCheckpointData;
        public readonly int checkpointIndex;
        public readonly float reachTime;

        public CheckpointReachedPlayerData(PlayerInfo pPayerInfo, int pCheckpointIndex, float pReachTime, CheckpointReachedPlayerData pPreviousCheckpointData = null)
        {
            playerInfo = pPayerInfo;
            checkpointIndex = pCheckpointIndex;
            reachTime = pReachTime;
            previousCheckpointData = pPreviousCheckpointData;
        }

        public float GetStartTime()
        {
            return previousCheckpointData?.GetStartTime() ?? reachTime;
        }

        // Returns 0 if there aren't any previous checkpoints
        public float GetTimeDeltaToPreviousCheckpoint()
        {
            return reachTime - (previousCheckpointData?.reachTime ?? reachTime);
        }

        public float GetTotalTimeSinceFirstCheckpoint()
        {
            return GetTimeDeltaToPreviousCheckpoint() + (previousCheckpointData?.GetTotalTimeSinceFirstCheckpoint() ?? 0);
        }

        public int GetReachedCheckpointsCount()
        {
            return 1 + (previousCheckpointData?.GetReachedCheckpointsCount() ?? 0);
        }
    }
}