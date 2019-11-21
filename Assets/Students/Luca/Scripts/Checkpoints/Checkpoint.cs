using System;
using UnityEngine;

namespace Students.Luca.Scripts.Checkpoints
{
    public class Checkpoint : MonoBehaviour
    {
        public int was = 3;
    
        public delegate void PossessableReachedCheckpointDel(Checkpoint checkpoint, Possessable possessable);

        public event PossessableReachedCheckpointDel OnPlayerEnteredCheckpoint;
    
    
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnTriggerEnter(Collider other)
        {
            Possessable possessable = other.GetComponent<Possessable>();
        
            if(possessable != null)
                OnPlayerEnteredCheckpoint?.Invoke(this, possessable);
        }

        private void OnPreCull()
        {
            
        }
    }
}
