using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Students.Luca.Scripts.Checkpoints
{
    /// <summary>
    /// This class stores and manages a set of checkpoints.
    /// </summary>
    [ExecuteInEditMode]
    public class CheckpointTrack : MonoBehaviour
    {
        private enum CheckpointListContentType{ CheckpointSequence, StartLocations}

        public bool autoInitializeOnStart = false;
        [ShowInInspector, SerializeField]
        private CheckpointListContentType checkpointListContentType = CheckpointListContentType.StartLocations;
        public List<Checkpoint> checkpoints;

        public event Checkpoint.PossessableReachedCheckpointDel OnPossessableReachedCheckpoint;

        [ShowInInspector, ReadOnly]
        private List<Checkpoint> _checkpointReferenceList;

        [field: ShowInInspector]
        [field: ReadOnly]
        [field: NonSerialized]
        public bool Initialized { get; private set; } = false;

        private void Awake()
        {
            if (!autoInitializeOnStart) return;
            Init();
        }

        public void Init()
        {
            if(Initialized) return;
            
            _checkpointReferenceList = new List<Checkpoint>();
            if (checkpoints == null)
            {
                checkpoints = new List<Checkpoint>();
            }
            else if (checkpoints.Count > 0)
            { // Following execution order important
                

                if((checkpoints.Count > 1 && (checkpoints[0]?.nextCheckpoints?.Count ?? 0) == 0))
                {
                    for(var i = 0; i < checkpoints.Count; i++)
                    {
                        if (checkpointListContentType == CheckpointListContentType.CheckpointSequence && (checkpoints[i]?.nextCheckpoints.Count ?? -1) == 0 && i != checkpoints.Count-1)
                        {
                            checkpoints[i].nextCheckpoints.Add(checkpoints[i+1]);
                        }
                        else
                        {
                            GetAllCheckpointsRecursively(checkpoints[i],out var checkpointRefListPart);
                            _checkpointReferenceList.AddRange(checkpointRefListPart);
                        }
                    }
                }
                
                if (checkpointListContentType == CheckpointListContentType.CheckpointSequence)
                    GetAllCheckpointsRecursively(checkpoints[0],out _checkpointReferenceList);
                
                _checkpointReferenceList.ForEach(checkpoint =>
                {
                    if(checkpoint != null)
                        checkpoint.OnPossessableEnteredCheckpoint += HandlePossessableReachedCheckpoint;
                });
            }

            Initialized = true;
        }

        private void OnDestroy()
        {
            _checkpointReferenceList?.ForEach(checkpoint =>
            {
                if(checkpoint != null)
                    checkpoint.OnPossessableEnteredCheckpoint -= HandlePossessableReachedCheckpoint;
            });
        }

        private void HandlePossessableReachedCheckpoint(Checkpoint checkpoint, Possessable possessable)
        {
            OnPossessableReachedCheckpoint?.Invoke(checkpoint, possessable);
        }

        /// <summary>
        /// Adds a checkpoint to the track, registers to necessary events.
        /// </summary>
        /// <param name="checkpoint">The checkpoint to add to the track.</param>
        /// <param name="isStartCheckpoint">If set to true, the checkpoint will be declared as start checkpoint.</param>
        public void AddCheckpoint(Checkpoint checkpoint, bool isStartCheckpoint = false)
        {
            if(checkpoint == null || _checkpointReferenceList.Contains(checkpoint))
                return;
            
            _checkpointReferenceList?.Add(checkpoint);
            checkpoint.OnPossessableEnteredCheckpoint += HandlePossessableReachedCheckpoint;

            if ((checkpoint.nextCheckpoints?.Count ?? 0) > 0)
            {
                GetAllCheckpointsRecursively(checkpoint,out var checkpointRefListPart);
                checkpointRefListPart.Where(cp => cp != null).ForEach(cp =>
                {
                    cp.OnPossessableEnteredCheckpoint += HandlePossessableReachedCheckpoint;
                });
                _checkpointReferenceList?.AddRange(checkpointRefListPart);
            }
            
            if (!isStartCheckpoint) return;
            checkpoints.Insert(0, checkpoint);
        }

        /// <summary>
        /// Returns a reference to the latest added start checkpoint.
        /// </summary>
        /// <param name="random">If set to true, a random start checkpoint will be returned.</param>
        /// <returns></returns>
        public Checkpoint GetStartCheckpoint(bool random = false)
        {
            var index = 0;
            if (random && checkpointListContentType == CheckpointListContentType.StartLocations)
            {
                index = Random.Range(0, checkpoints.Count - 1);
            }
            return checkpoints.Count > index?checkpoints[index]:null;
        }

        /// <summary>
        /// Returns a list containing all start checkpoints.
        /// </summary>
        /// <returns>Returns a list containing all start checkpoints.</returns>
        public List<Checkpoint> GetStartCheckpoints()
        {
            if (checkpointListContentType == CheckpointListContentType.StartLocations)
                return checkpoints;
            return checkpoints.Count > 0?new List<Checkpoint>(){checkpoints[0]}: null;
        }

        /// <summary>
        /// Sets the layer of all checkpoints related to this track to the given layer.
        /// </summary>
        /// <param name="layer">The layer to set the checkpoints to.</param>
        public void SetDefaultCheckpointLayer(int layer) => _checkpointReferenceList?.Where(checkpoint => checkpoint != null).ForEach(checkpoint => checkpoint.gameObject.layer = layer);

        /// <summary>
        /// Gets a list of all future checkpoints connected to given checkpoint. RECURSIVE FUNCTION.
        /// </summary>
        /// <param name="checkpoint">The start node.</param>
        /// <param name="checkpointList">Reference to the list to which the result should be written to.</param>
        private static void GetAllCheckpointsRecursively(Checkpoint checkpoint, out List<Checkpoint> checkpointList)
        {
            checkpointList = new List<Checkpoint> {checkpoint};
            if ((checkpoint?.nextCheckpoints?.Count ?? 0) <= 0)
                return;
            
            foreach (var chkpt in checkpoint.nextCheckpoints)
            {
                GetAllCheckpointsRecursively(chkpt, out var cpl);
                checkpointList.AddRange(cpl);
            }
            checkpointList = checkpointList.Distinct().ToList();
        }

        /// <summary>
        /// Returns a list with all checkpoints related to this track.
        /// </summary>
        /// <returns>List</returns>
        public List<Checkpoint> GetAllCheckpoints()
        {
            return new List<Checkpoint>(_checkpointReferenceList);
        }

        #region Editor Stuff

        #if UNITY_EDITOR
        [Header("Editor Gizmos Settings")]
        [ShowInInspector, SerializeField]
        private bool displayGizmos = true;
        [ShowInInspector, SerializeField]
        private bool alwaysDisplayGizmos = false;
        [ShowInInspector, SerializeField]
        private Color checkpointStartNodeColor = Color.green;
        [ShowInInspector, SerializeField]
        private Color checkpointEndNodeColor = Color.red;
        [ShowInInspector, SerializeField]
        private Color checkpointNodeColor = Color.yellow;
        [ShowInInspector, SerializeField]
        private Color nodeConnectionColor = Color.yellow;
        [ShowInInspector, SerializeField]
        private Color nodeConnectionArrowColor = new Color32(255,52,0, 255);
        [ShowInInspector, SerializeField]
        private float nodeConnectionArrowSize = 10;
        [ShowInInspector, SerializeField]
        private float nodeSphereSize = 2;
        private void OnDrawGizmosSelected()
        {
            if (!displayGizmos || alwaysDisplayGizmos) return;
            DrawGizmos();
        }

        private void OnDrawGizmos()
        {
            if (!displayGizmos || !alwaysDisplayGizmos) return;
            DrawGizmos();
        }

        private void DrawGizmos()
        {
            if(!(_checkpointReferenceList?.Count > 0)) return;
            var firstCheckpoint = GetStartCheckpoint();
            Gizmos.color = checkpointStartNodeColor;
            if(checkpointListContentType == CheckpointListContentType.CheckpointSequence)
                Gizmos.DrawSphere(firstCheckpoint.transform.position, nodeSphereSize);
            else
            {
                checkpoints?.Where(checkpoint => checkpoint != null).ForEach(checkpoint =>
                {
                    Gizmos.DrawSphere(firstCheckpoint.transform.position, nodeSphereSize);
                });
            }
                
            foreach (var checkpoint in _checkpointReferenceList.Where(checkpoint => checkpoint != null))
            {
                var checkpointPosition = checkpoint.transform.position;
                if (!checkpoint.nextCheckpoints?.Where(cp => cp != null).Any() ?? true)
                {
                    Gizmos.color = checkpointEndNodeColor;
                    Gizmos.DrawSphere(checkpointPosition, nodeSphereSize);
                    continue;
                }
                
                if((checkpointListContentType == CheckpointListContentType.CheckpointSequence && checkpoint != firstCheckpoint) ||
                   (checkpointListContentType == CheckpointListContentType.StartLocations && !(checkpoints?.Contains(checkpoint) ?? false)))
                {
                    Gizmos.color = checkpointNodeColor;
                    Gizmos.DrawSphere(checkpointPosition, nodeSphereSize/2);
                }
                    
                foreach (var nextCheckpoint in checkpoint.nextCheckpoints.Where(nextCheckpoint => nextCheckpoint != null))
                {
                    var nextCheckpointPosition = nextCheckpoint.transform.position;
                    Gizmos.color = checkpointNodeColor;
                    Gizmos.DrawLine(checkpointPosition, nextCheckpointPosition);
                    Handles.color = nodeConnectionArrowColor;
                    var dir = nextCheckpointPosition - checkpointPosition;
                    var rot = Quaternion.LookRotation(dir);
                    Handles.ArrowHandleCap(0, checkpointPosition, rot, nodeConnectionArrowSize, EventType.Repaint);
                    Handles.ArrowHandleCap(0, nextCheckpointPosition - dir*.5f, rot,nodeConnectionArrowSize,EventType.Repaint);
                }
            }
        }
        #endif
        #endregion
    }
}