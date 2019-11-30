using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using Students.Luca.Scripts.Checkpoints;
using UnityEditor;
using UnityEngine;

namespace Students.Luca.Scripts.GameMode
{
    public class Test_CheckpointGameMode : GameModeBase
    {
        public CheckpointTrackBuilder cpTrackBuilder;
        public CheckpointManager cpManager;

        public GameObject checkpointPrefab;
        
        public Checkpoint testStartCheckpoint;
        // Start is called before the first frame update
        void Start()
        {
            if(cpTrackBuilder == null)
                cpTrackBuilder = CheckpointTrackBuilder.CreateInstance();
            if (cpManager == null)
                cpManager = FindObjectOfType<CheckpointManager>();

            cpManager.OnPlayerReachedCheckpoint += HandlePlayerReachedCheckpointEvent;
            cpManager.OnPlayerReachedLastCheckpoint += HandlePlayerReachedLastCheckpointEvent;
            
            testStartCheckpoint.transform.SetParent(cpManager.ActiveCheckpointTrack.transform);
            cpManager.ActiveCheckpointTrack.AddCheckpoint(testStartCheckpoint, true);
            
            // Set first targets / start positions
            var pm = FindObjectOfType<TMPTestPlayerManager>(); // HACK
            pm.OnNewPlayerJoinedGame += HandlePlayerJoinedGame;
            Debug.Log(pm+" - "+(pm?.playerInfos?.Count ?? 0));
            pm?.playerInfos?.ForEach(pi =>
            {
                cpManager.SetNextCheckpointTarget(testStartCheckpoint, pi);
            });
            
            
            cpTrackBuilder.doDebug = true;
            cpTrackBuilder.maxItr = 100;
            float heightAboveGround = 8;//checkpointPrefab.GetComponent<MeshFilter>()?.sharedMesh.bounds.extents.y ?? 1;
            cpTrackBuilder.CheckpointPrefab(checkpointPrefab).SlopeTestingSamples(40).StepHeight(2).MaxFallDistance(30).MaxSlope(30).HeightAboveGround(heightAboveGround).PreferredRadius(70);
            // Testing
            //MeshFilter mf = PrefabUtility.GetAddedComponents(checkpointPrefab)?.OfType<MeshFilter>().FirstOrDefault();
            
            //var newCheckpoint = cpTrackBuilder.HeightAboveGround(heightAboveGround).PreferredRadius(80).GenCheckpointWithinRadius(testStartCheckpoint, 180, CheckpointTrackBuilder.RandomSpecifier, 0);

            StartCoroutine(FindAndConnectNewCheckpoint(testStartCheckpoint, numberOfCheckpointsToGenerateInAdvance));

        }

        private void HandlePlayerJoinedGame(PlayerInfo playerInfo)
        {
            cpManager.SetNextCheckpointTarget(testStartCheckpoint, playerInfo);
        }

        private void HandlePlayerReachedLastCheckpointEvent(CheckpointReachedPlayerData playercheckpointdata)
        {
            // TODO
        }

        private void HandlePlayerReachedCheckpointEvent(CheckpointReachedPlayerData playercheckpointdata)
        {
            cpManager.SetNextCheckpointTargets(playercheckpointdata.reachedCheckpoint.nextCheckpoints,
                playercheckpointdata.playerInfo);

            playercheckpointdata.reachedCheckpoint.nextCheckpoints?.ForEach(checkpoint =>
            {
                StartCoroutine(FindAndConnectNewCheckpoint(checkpoint,numberOfCheckpointsToGenerateInAdvance));
            });

        }

        public int numberOfCheckpointsToGenerateInAdvance = 2;
        
        public int test_checkpointCount = 20;
        [ShowInInspector, ReadOnly]
        private int test_checkpointCounter = 0;
        public float debugTimeInBetween = 1;
        IEnumerator FindAndConnectNewCheckpoint(Checkpoint startCheckpoint, int depth = 0)
        {
            var chkpoint = cpTrackBuilder.GenCheckpointWithinRadius(startCheckpoint, 120, CheckpointTrackBuilder.RandomSpecifier, 15);

            if (chkpoint == null)
                yield break;
            
            chkpoint.transform.SetParent(cpTrackBuilder.transform);
            startCheckpoint.nextCheckpoints.Add(chkpoint);
            cpManager.ActiveCheckpointTrack.AddCheckpoint(chkpoint);
            

            if (depth > 1)
            {
                if(debugTimeInBetween > 0)
                    yield return new WaitForSeconds(debugTimeInBetween);
                else 
                    yield return new WaitForEndOfFrame();
                
                StartCoroutine(FindAndConnectNewCheckpoint(chkpoint,depth-1));
            }
            else
            {
                cpManager.ActiveCheckpointTrack.SetDefaultCheckpointLayer(LayerMask.NameToLayer(cpManager.defaultCheckpointLayer)); // HACKY
            }
            
            /*test_checkpointCounter++;
            if (test_checkpointCounter < test_checkpointCount)
            {
                if(debugTimeInBetween > 0)
                    yield return new WaitForSeconds(debugTimeInBetween);
                else 
                    yield return new WaitForEndOfFrame();
                
                StartCoroutine(FindAndConnectNewCheckpoint(chkpoint));
            }*/

            yield return 0;
        }

        // Update is called once per frame
        public override void Activate()
        {
            base.Activate();
            
            
        }
    }
}
