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
            Possessable possessable = other.GetComponent<Possessable>();

            if (possessable == null)
                possessable = other.GetComponentInParent<Possessable>();
            
            if(possessable != null)
                OnPossessableEnteredCheckpoint?.Invoke(this, possessable);
        }
        
        void OnDrawGizmosSelected()
        {
            transform.parent.SendMessage("OnDrawGizmosSelected");
        }
    }
}
