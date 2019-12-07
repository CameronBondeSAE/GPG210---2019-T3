using System;
using System.Collections;
using System.Collections.Generic;
using Students.Luca.Scripts.Checkpoints;
using UnityEngine;

namespace Students.Blaide
{
    public class OutOfBounds : MonoBehaviour
    {
        /// <summary>
        /// This just puts whatever leaves the boundary back into the boundary or, if its not important it  should delete it.
        /// </summary>
        public Transform respawnPoint;
        public CheckpointManager checkpointManager;
        public float respawnHeightOffSet = 5f;
        // Start is called before the first frame update
        void Start()
        {
            checkpointManager = FindObjectOfType<CheckpointManager>();
        }

        private void OnTriggerExit(Collider other)
        {
            GameObject root = other.transform.root.gameObject;
            Debug.Log(other.gameObject.name + "Left the map");

            if (other.transform.root.GetComponent<Possessable>() != null &&
                other.transform.root.GetComponent<Possessable>().CurrentController != null)
            {
                if (checkpointManager.GetLastReachedCheckpoint(other.transform.root.GetComponent<Possessable>()
                        .CurrentController.playerInfo) != null)
                {
                    root.transform.position = checkpointManager
                        .GetLastReachedCheckpoint(other.transform.root.GetComponent<Possessable>().CurrentController
                            .playerInfo).transform.position;
                }
                else
                {
                    root.transform.position = respawnPoint.position + Vector3.up * respawnHeightOffSet;
                }
            }
            else
            {
                Debug.Log(root.name + "Wasn't part of a vehicle that was being used, so it was destroyed.");
                Destroy(root);
            }
        }
    }
}