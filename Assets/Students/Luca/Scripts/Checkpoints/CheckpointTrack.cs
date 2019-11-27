using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Students.Luca.Scripts.Checkpoints
{
    /// <summary>
    /// TODO summary
    /// if just add checkpoints to list, it will automatically set up the recursive target chain-sequence
    /// </summary>
    public class CheckpointTrack : MonoBehaviour
    {
        /*public enum SequenceOrder
        {
            ListIndex,
            CheckpointDefined
        }*/
        
        public List<Checkpoint> checkpoints; // first checkpoint = start checkpoint
        /*public SequenceOrder checkpointSequenceOrder; // The way you've set up the */

        public event Checkpoint.PossessableReachedCheckpointDel OnPossessableReachedCheckpoint;

        [ShowInInspector, ReadOnly]
        private List<Checkpoint> checkpointReferenceList;

        private void Start()
        {
            checkpointReferenceList = new List<Checkpoint>();
            if (checkpoints == null)
            {
                checkpoints = new List<Checkpoint>();
            }
            else
            { // Following execution order important
                if (checkpoints.Count == 0) return;

                if((checkpoints.Count > 1 && (checkpoints[0]?.nextCheckpoints?.Count ?? 0) == 0))
                {
                    for(var i = 0; i < checkpoints.Count; i++)
                    {
                        if ((checkpoints[i]?.nextCheckpoints.Count ?? -1) == 0 && i != checkpoints.Count-1)
                        {
                            checkpoints[i].nextCheckpoints.Add(checkpoints[i+1]);
                        }
                    }
                }
                
                
                GetAllCheckpointsRecursively(checkpoints[0],out checkpointReferenceList);
                
                checkpointReferenceList.ForEach(checkpoint =>
                    {
                        checkpoint.OnPossessableEnteredCheckpoint += HandlePossessableReachedCheckpoint;
                    });
            }
        }

        private void OnDestroy()
        {
            checkpointReferenceList?.ForEach(checkpoint =>
            {
                checkpoint.OnPossessableEnteredCheckpoint -= HandlePossessableReachedCheckpoint;
            });
        }

        private void HandlePossessableReachedCheckpoint(Checkpoint checkpoint, Possessable possessable)
        {
            OnPossessableReachedCheckpoint?.Invoke(checkpoint, possessable);
        }

        public void AddCheckpoint(Checkpoint checkpoint)
        {
            checkpointReferenceList?.Add(checkpoint);
        }

        public Checkpoint GetFirstCheckpoint()
        {
            return checkpoints.Count > 0?checkpoints[0]:null;
        }

        public void SetDefaultCheckpointLayer(int layer) => checkpointReferenceList?.ForEach(checkpoint => checkpoint.gameObject.layer = layer);

        private static void GetAllCheckpointsRecursively(Checkpoint checkpoint, out List<Checkpoint> checkpointList)
        {
            checkpointList = new List<Checkpoint> {checkpoint};
            if ((checkpoint?.nextCheckpoints?.Count ?? 0) <= 0)
                return;
            
            foreach (var chkpt in checkpoint.nextCheckpoints)
            {
                GetAllCheckpointsRecursively(chkpt, out checkpointList);
            }
            checkpointList.Add(checkpoint);
            checkpointList = checkpointList.Distinct().ToList();
        }
    }
}