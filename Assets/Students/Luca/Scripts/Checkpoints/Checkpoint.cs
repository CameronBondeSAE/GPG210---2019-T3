using System;
using System.Collections.Generic;
using UnityEngine;

namespace Students.Luca.Scripts.Checkpoints
{
    public class Checkpoint : MonoBehaviour
    {
        public delegate void PossessableReachedCheckpointDel(Checkpoint checkpoint, Possessable possessable);

        public Checkpoint lastCheckpoint;
        public List<Checkpoint> nextCheckpoints;

        /// <summary>
        /// When a possessable passes through this checkpoint, this event will be invoked.
        /// </summary>
        public event PossessableReachedCheckpointDel OnPossessableEnteredCheckpoint;

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

        private void OnDrawGizmosSelected()
        {
            GetComponentInParent<CheckpointTrack>()?.SendMessage("OnDrawGizmosSelected");
        }
    }
}
