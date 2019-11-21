using System;
using UnityEngine;

namespace Students.Luca.Scripts.Checkpoints
{
    public class Checkpoint : MonoBehaviour
    {
        public delegate void PossessableReachedCheckpointDel(Checkpoint checkpoint, Possessable possessable);

        public event PossessableReachedCheckpointDel OnPlayerEnteredCheckpoint;

        private void OnTriggerEnter(Collider other)
        {
            Possessable possessable = other.GetComponent<Possessable>();

            if (possessable == null)
                possessable = other.GetComponentInParent<Possessable>();
            
            if(possessable != null)
                OnPlayerEnteredCheckpoint?.Invoke(this, possessable);
        }
    }
}
