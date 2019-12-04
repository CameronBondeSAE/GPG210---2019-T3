using System;
using System.Collections;
using System.Collections.Generic;
using Students.Luca.Scripts.Checkpoints;
using UnityEngine;

namespace Students.Blaide
{
    public class OutOfBounds : MonoBehaviour
    {
        public Collider boundaryBox;
        public Transform respawnPoint;
        public CheckpointManager checkpointManager;

        public float respawnHeightOffSet = 5f;
        // Start is called before the first frame update
        void Start()
        {
            checkpointManager = FindObjectOfType<CheckpointManager>();
        }
        // Update is called once per frame
        void Update()
        {
        }
        
        private void OnTriggerExit(Collider other)
        {
            Debug.Log(other.gameObject.name + "Left the map");

            if (other.transform.root.GetComponent<Possessable>() != null && other.transform.root.GetComponent<Possessable>().CurrentController != null)
            {
                if (checkpointManager.GetLastReachedCheckpoint(other.transform.root.GetComponent<Possessable>().CurrentController.playerInfo) != null)
                {
                    other.transform.root.transform.position = checkpointManager
                        .GetLastReachedCheckpoint(other.transform.root.GetComponent<Possessable>().CurrentController
                            .playerInfo).transform.position;
                }
                else
                {
                    other.transform.root.transform.position = respawnPoint.position + Vector3.up * respawnHeightOffSet;
                }
            }
        }
    }
}