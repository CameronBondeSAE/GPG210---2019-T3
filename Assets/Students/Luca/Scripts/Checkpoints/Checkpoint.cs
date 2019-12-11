using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace Students.Luca.Scripts.Checkpoints
{
    public class Checkpoint : MonoBehaviour
    {
        public delegate void PossessableReachedCheckpointDel(Checkpoint checkpoint, Possessable possessable);

        public List<Checkpoint> nextCheckpoints;

        /// <summary>
        /// When a possessable passes through this checkpoint, this event will be invoked.
        /// </summary>
        public event PossessableReachedCheckpointDel OnPossessableEnteredCheckpoint;
        
        // Checkpoint Pool; Super Hacky; Shouldn't be in here
        private static Stack<Checkpoint> checkpointPool = new Stack<Checkpoint>();
        
        public static GameObject checkpointPrefab;
        public static bool doPooling = true;
        public static int maxObjects = 30;

        private void Start()
        {
            if (nextCheckpoints == null)
            {
                nextCheckpoints = new List<Checkpoint>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var possessable = other.GetComponent<Possessable>();

            if (possessable == null)
                possessable = other.GetComponentInParent<Possessable>();
            
            if(possessable != null)
                OnPossessableEnteredCheckpoint?.Invoke(this, possessable);
        }

        public List<Checkpoint> GetEndPoints()
        {
            var endPoints = new List<Checkpoint>();

            var nxtCheckpointsNoNulls = nextCheckpoints?.Where(checkpoint => checkpoint != null).ToList();
            if ((nxtCheckpointsNoNulls?.Count ?? 0) > 0)
            {
                nxtCheckpointsNoNulls.ForEach(checkpoint =>
                {
                    endPoints.AddRange(checkpoint.GetEndPoints());
                });
            }
            else
            {
                endPoints.Add(this);
            }
            
            return endPoints;
        }

        private void OnDrawGizmosSelected()
        {
            GetComponentInParent<CheckpointTrack>()?.SendMessage("OnDrawGizmosSelected");
        }

        // Deletes checkpoints or returns it to hacky pool
        public void Release()
        {
            ReturnCheckpointToPool(this);
        }

        public static void ReturnCheckpointToPool(Checkpoint checkpoint)
        {
            if (doPooling && checkpointPool.Count < maxObjects)
            {
                checkpoint.gameObject.SetActive(false);
                checkpoint.Reset();
                if(!checkpointPool.Contains(checkpoint))
                    checkpointPool.Push(checkpoint);
                    
                return;
            }
            Destroy(checkpoint.gameObject);
        }

        public static Checkpoint GetCheckpointObjFromPool()
        {
            Checkpoint newCheckpoint = null;

            while (checkpointPool.Count > 0 && newCheckpoint == null)
            {
                newCheckpoint = checkpointPool.Pop();
            }
                
            if(newCheckpoint == null)
            {
                newCheckpoint = Instantiate(checkpointPrefab).GetComponent<Checkpoint>();
            }
            newCheckpoint.gameObject.SetActive(true);
            return newCheckpoint;
        }

        private void Reset()
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            nextCheckpoints = new List<Checkpoint>();
            OnPossessableEnteredCheckpoint = null;
        }
    }
}
