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
    /// TODO summary
    /// if just add checkpoints to list, it will automatically set up the recursive target chain-sequence
    /// </summary>
    [ExecuteInEditMode]
    public class CheckpointTrack : MonoBehaviour
    {
        /*public enum SequenceOrder
        {
            ListIndex,
            CheckpointDefined
        }*/
        private enum CheckpointListContentType{ CheckpointSequence, StartLocations}

        public bool autoInitializeOnStart = false;
        [ShowInInspector, SerializeField]
        private CheckpointListContentType checkpointListContentType;
        public List<Checkpoint> checkpoints; // first checkpoint = start checkpoint
        /*public SequenceOrder checkpointSequenceOrder; // The way you've set up the */

        public event Checkpoint.PossessableReachedCheckpointDel OnPossessableReachedCheckpoint;

        [ShowInInspector, ReadOnly]
        private List<Checkpoint> _checkpointReferenceList;

        [field: ShowInInspector]
        [field: ReadOnly]
        public bool Initialized { get; private set; } = false;

        private void Awake()
        {
            if (autoInitializeOnStart)
            {
                Init();
                if(!Initialized)
                    Debug.LogWarning("Couldn't auto-initialize CheckpointTrack on Awake.");
            }
        }

        public void Init()
        {
            if(Initialized)
                return;
            
            _checkpointReferenceList = new List<Checkpoint>();
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
                        if (checkpointListContentType == CheckpointListContentType.CheckpointSequence && (checkpoints[i]?.nextCheckpoints.Count ?? -1) == 0 && i != checkpoints.Count-1)
                        {
                            checkpoints[i].nextCheckpoints.Add(checkpoints[i+1]);
                        }
                        else
                        {
                            List<Checkpoint> checkpointRefListPart;
                            GetAllCheckpointsRecursively(checkpoints[i],out checkpointRefListPart);
                            _checkpointReferenceList.AddRange(checkpointRefListPart);
                        }
                    }
                }
                
                if (checkpointListContentType == CheckpointListContentType.CheckpointSequence)
                    GetAllCheckpointsRecursively(checkpoints[0],out _checkpointReferenceList);
                
                _checkpointReferenceList.ForEach(checkpoint =>
                {
                    checkpoint.OnPossessableEnteredCheckpoint += HandlePossessableReachedCheckpoint;
                });
            }

            Initialized = true;
        }

        private void OnDestroy()
        {
            _checkpointReferenceList?.ForEach(checkpoint =>
            {
                checkpoint.OnPossessableEnteredCheckpoint -= HandlePossessableReachedCheckpoint;
            });
        }

        private void HandlePossessableReachedCheckpoint(Checkpoint checkpoint, Possessable possessable)
        {
            OnPossessableReachedCheckpoint?.Invoke(checkpoint, possessable);
        }

        public void AddCheckpoint(Checkpoint checkpoint, bool isStartCheckpoint = false)
        {
            _checkpointReferenceList?.Add(checkpoint);
            if (!isStartCheckpoint) return;
            checkpoints.Add(checkpoint);
            checkpoints.Insert(0, checkpoint);
        }

        public Checkpoint GetStartCheckpoint(bool random = false)
        {
            var index = 0;
            if (random && checkpointListContentType == CheckpointListContentType.StartLocations)
            {
                index = Random.Range(0, checkpoints.Count - 1);
            }
            return checkpoints.Count > 0?checkpoints[index]:null;
        }

        public List<Checkpoint> GetStartCheckpoints()
        {
            if (checkpointListContentType == CheckpointListContentType.StartLocations)
                return checkpoints;
            return checkpoints.Count > 0?new List<Checkpoint>(){checkpoints[0]}: null;
        }

        public void SetDefaultCheckpointLayer(int layer) => _checkpointReferenceList?.ForEach(checkpoint => checkpoint.gameObject.layer = layer);

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
                if (checkpoint.nextCheckpoints?.Count == 0)
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