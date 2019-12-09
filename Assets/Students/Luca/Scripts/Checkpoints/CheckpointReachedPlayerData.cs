using System.Collections.Generic;
using System.Linq;

namespace Students.Luca.Scripts.Checkpoints
{
    public class CheckpointReachedPlayerData
    {
        public readonly PlayerInfo playerInfo;
        public readonly CheckpointReachedPlayerData previousCheckpointData;
        public readonly Checkpoint reachedCheckpoint;
        public readonly float reachTime;
        private readonly List<Checkpoint> nextCheckpointTargets;
        public readonly bool _lockNextCheckpointTargets;

        public CheckpointReachedPlayerData(PlayerInfo pPayerInfo, Checkpoint pReachedCheckpoint, float pReachTime, CheckpointReachedPlayerData pPreviousCheckpointData = null, List<Checkpoint> pNextCheckpointTargets = null, bool pLockNextCheckpointTargets = false)
        {
            playerInfo = pPayerInfo;
            reachedCheckpoint = pReachedCheckpoint;
            reachTime = pReachTime;
            previousCheckpointData = pPreviousCheckpointData;
            nextCheckpointTargets = pNextCheckpointTargets;
            _lockNextCheckpointTargets = pLockNextCheckpointTargets;
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

        // Checks whether the player has given checkpoint as future target
        public bool PlayerHasCheckpointAsTarget(Checkpoint checkpoint)
        {
            var hasTarget = false;

            if ((nextCheckpointTargets?.Count ?? 0) > 0)
            {
                hasTarget = nextCheckpointTargets.Aggregate(hasTarget, (current, nextCheckpoint) => current || PlayerHasCheckpointAsTarget(checkpoint));
            }
            
            return hasTarget;
        }

        // Checks whether the player ever reached given checkpoint
        public bool PlayerHasReachedCheckpoint(Checkpoint checkpoint)
        {
            return reachedCheckpoint == checkpoint || (previousCheckpointData?.PlayerHasReachedCheckpoint(checkpoint) ?? false);
        }

        public bool AddNextCheckpointTarget(Checkpoint checkpoint)
        {
            if (_lockNextCheckpointTargets || nextCheckpointTargets == null)
                return false;
            
            nextCheckpointTargets.Add(checkpoint);
            return true;
        }

        public bool AddNextCheckpointTargets(List<Checkpoint> checkpoints)
        {
            if (_lockNextCheckpointTargets || nextCheckpointTargets == null)
                return false;
            
            nextCheckpointTargets.AddRange(checkpoints);
            return true;
        }

        public bool ResetNextCheckpointTargets()
        {
            if (_lockNextCheckpointTargets || nextCheckpointTargets == null)
                return false;
            
            nextCheckpointTargets.Clear();
            return true;
        }

        public bool RemoveNextCheckpointTarget(Checkpoint checkpoint)
        {
            if (_lockNextCheckpointTargets || nextCheckpointTargets == null)
                return false;
            
            return nextCheckpointTargets.Remove(checkpoint);
        }

        public List<Checkpoint> GetNextCheckpointTargets()
        {
            return new List<Checkpoint>(nextCheckpointTargets);
        }
    }
}