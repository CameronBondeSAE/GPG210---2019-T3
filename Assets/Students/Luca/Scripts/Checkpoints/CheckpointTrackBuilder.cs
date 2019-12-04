using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Students.Luca.Scripts.Checkpoints
{
    public class CheckpointTrackBuilder : MonoBehaviour
    {
        public enum CheckpointPlacement
        {
            TerrainGround, // Must be grounded on a terrain
            //Ground, // must be grounded on any surface
            //SpecifiedHeighAboveGround // Must be a specific height above ground
            // ..... 
        }

        #region Variables
        public int maxItr = 100; // TODO bettername

        public LayerMask raycastLayers = ~0; // Remove players etc from here

        public const float RandomSpecifier = float.NaN; // Do not change value; Hacky;

        public CheckpointPlacement checkpointPlacement;
        
        [SerializeField, ShowInInspector]
        private GameObject checkpointPrefab;
        public GameObject P_CheckpointPrefab => checkpointPrefab;
        
        [SerializeField, ShowInInspector]
        private float preferredRadius = 10;
        public float P_PreferredRadius => preferredRadius;
        
        [SerializeField, ShowInInspector]
        private float minRadius = 0;
        public float P_MinRadius => minRadius;
        
        [SerializeField, ShowInInspector]
        private float maxRadiusDelta = 1;
        public float P_MaxRadiusDelta => maxRadiusDelta;
        
        [SerializeField, ShowInInspector]
        private float maxAngleDelta = 5;
        public float P_MaxAngleDelta => maxAngleDelta;

        [SerializeField, ShowInInspector]
        private float heightAboveGround = 1;
        public float P_HeightAboveGround => heightAboveGround;
        
        [SerializeField, ShowInInspector]
        private bool targetMustBeVisibleFromStart = false;
        public bool P_TargetMustBeVisibleFromStart => targetMustBeVisibleFromStart;
        
        [SerializeField, ShowInInspector]
        private float maxSlope = 45;
        public float P_MaxSlope => maxSlope;
        
        [SerializeField, ShowInInspector]
        private float stepHeight = 1;
        public float PStepHeight => stepHeight;
        
        [SerializeField, ShowInInspector]
        private int slopeTestingSamples = 5; //  The higher the preciser but the more calc intensive
        public int P_SlopeTestingSamples => slopeTestingSamples;
        
        [SerializeField, ShowInInspector]
        private float maxFallDistance = 2; //  -1 do deactivate => unlimited fall distance
        public float P_MaxFallDistance => maxFallDistance;
        
        [SerializeField, ShowInInspector]
        private float maxAngularStep = 5;
        public float P_MaxAngularStep => maxAngularStep;
        
        // Debug Settings
        public bool doDebug = false;
        public float rayDisplayTime = .1f;
        
        public Color slopeSamplePosColor = new Color(255, 187, 0, 255);

        #endregion

        /// <summary>
        /// Instantiates a new gameobject with a CheckpointTrackBuilder instance on it and returns it.
        /// </summary>
        /// <returns>The CheckpointTrackBuilder attached to the new Gameobject.</returns>
        public static CheckpointTrackBuilder CreateInstance()
        {
            var go = new GameObject();
            go.name = "CheckpointTrackBuilder";
            return go.AddComponent<CheckpointTrackBuilder>();
        }
        
        #region Functional Setters

        public CheckpointTrackBuilder PreferredRadius(float pPreferredRadius)
        {
            preferredRadius = Mathf.Abs(pPreferredRadius < minRadius ? minRadius : pPreferredRadius);
            return this;
        }
        
        public CheckpointTrackBuilder CheckpointPrefab(GameObject pCheckpointPrefab)
        {
            checkpointPrefab = pCheckpointPrefab;
            return this;
        }

        public CheckpointTrackBuilder MinRadius(float pMinRadius)
        {
            minRadius = Mathf.Abs(pMinRadius);
            return this;
        }

        public CheckpointTrackBuilder MaxSlope(float pMaxSlope)
        {
            maxSlope = Mathf.Clamp(Mathf.Abs(pMaxSlope), 0, 360);
            return this;
        }

        public CheckpointTrackBuilder MaxRadiusDelta(float pMaxRadiusDelta)
        {
            maxRadiusDelta = Mathf.Abs(pMaxRadiusDelta);
            return this;
        }

        public CheckpointTrackBuilder MaxAngleDelta(float pMaxAngleDelta)
        {
            maxAngleDelta = Mathf.Clamp(Mathf.Abs(pMaxAngleDelta), 0, 180);
            return this;
        }

        public CheckpointTrackBuilder HeightAboveGround(float pHeightAboveGround)
        {
            heightAboveGround = pHeightAboveGround;
            return this;
        }

        public CheckpointTrackBuilder TargetMustBeVisibleFromStart(bool state)
        {
            targetMustBeVisibleFromStart = state;
            return this;
        }

        public CheckpointTrackBuilder SlopeTestingSamples(int amount)
        {
            slopeTestingSamples = Mathf.Abs(amount);
            return this;
        }
        
        public CheckpointTrackBuilder StepHeight(float height)
        {
            stepHeight = Mathf.Abs(height);
            return this;
        }
        
        public CheckpointTrackBuilder MaxFallDistance(float height)
        {
            maxFallDistance = height;
            return this;
        }
        
        public CheckpointTrackBuilder MaxAngularStep(float maxStep)
        {
            maxAngularStep = maxStep;
            return this;
        }

        #endregion

        /// <summary>
        /// Searches a position within a give radius & angle of the given checkpoint restricted by given settings + instance settings and creates a new
        /// Checkpoint Gameobject if a valid position was found.
        /// </summary>
        /// <param name="startCheckpoint">The checkpoint from which you are looking for a new position.</param>
        /// <param name="coneMaxAngle">
        ///     The maximum angle from the start checkpoints forward direction to the left and right to check for a target.
        ///     <b>Maximum allowed value is 180.</b> (Given amount on each side left & right; <example>If the value is 180, it will check for a valid position 180° to the left and 180° to the right - so 360° in total.</example>)
        /// </param>
        /// <param name="signedStartAngle">
        ///    The angle toward the checkpoints forward direction from which to start sampling for a valid position.
        ///     For a <b>random</b> value enter CheckpointTrackBuilder.RandomSpecifier.
        /// </param>
        /// <param name="coneMinAngle">
        ///    The minimum angle from the start checkpoints forward direction to the left and right to check for a target.
        ///     <i>Helpful if you don't want a position in front of the checkpoint.</i>
        /// </param>
        /// <returns>A reference to the newly instantiated checkpoint or null if no valid position was found.</returns>
        public Checkpoint GenCheckpointWithinRadius(Checkpoint startCheckpoint, float coneMaxAngle = 180f,
            float signedStartAngle = 0, float coneMinAngle = 0)
        {
            if (startCheckpoint == null)
                return null;
            var startPoint = startCheckpoint.transform.position;
            var forward = startCheckpoint.transform.forward;
            var up = startCheckpoint.transform.up;
            return GenCheckpointWithinRadius(startPoint, forward, up,
                coneMaxAngle, signedStartAngle, coneMinAngle);
        }

        /// <summary>
        /// Searches a position within a give radius & angle of the given start position restricted by given settings + instance settings and creates a new
        /// Checkpoint Gameobject if a valid position was found.
        /// </summary>
        /// <param name="startPos">The start position from which to start searching for a new target.</param>
        /// <param name="forwardDir">The forward direction.</param>
        /// <param name="upDir">The up direction</param>
        /// <param name="coneMaxAngle">
        ///     The maximum angle from the start checkpoints forward direction to the left and right to check for a target. <break />
        ///     <b>Values allowed: 0-180.</b> <break />
        ///     (Given amount on each side left & right;<example>If the value is 180, it will check for a valid position 180° to the left and 180° to the right - so 360° in total.</example>)
        /// </param>
        /// <param name="signedStartAngle">
        ///    The angle toward the checkpoints forward direction from which to start sampling for a valid position. <break />
        ///     For a <b>random</b> value enter CheckpointTrackBuilder.RandomSpecifier. <break />
        ///     For an angle towards the left, negate the given value.
        /// </param>
        /// <param name="coneMinAngle">
        ///    The minimum angle from the start checkpoints forward direction to the left and right to check for a target. <break />
        ///     <b>Enter a value smaller or equal to 0 for no restrictions.</b> <break />
        ///     <i>Helpful if you don't want a position in front of the checkpoint.</i>
        /// </param>
        /// <returns>A reference to the newly instantiated checkpoint or null if no valid position was found.</returns>
        /// <returns></returns>
        public Checkpoint GenCheckpointWithinRadius(Vector3 startPos, Vector3 forwardDir, Vector3 upDir,
            float coneMaxAngle = 180f, float signedStartAngle = 0, float coneMinAngle = 0)
        {
            // ========= VALIDATE INPUT PARAMS
            
            forwardDir = forwardDir.Equals(default) ? Vector3.forward : forwardDir;
            
            coneMinAngle =
                Mathf.Clamp(coneMinAngle, 0,
                    coneMaxAngle);
            coneMaxAngle = Mathf.Clamp(coneMaxAngle, 0, 180);

            
            signedStartAngle = float.IsNaN(signedStartAngle)
                ? (Random.Range(0, 2) * 2 - 1) * Random.Range(coneMinAngle, coneMaxAngle)
                : Mathf.Clamp(Mathf.Abs(signedStartAngle), coneMinAngle, coneMaxAngle) * Mathf.Sign(signedStartAngle);

            // ========= TEMP VARIABLES
            var potentialValidPos = startPos;
            var validNewCheckpointRotation = Quaternion.identity;
            var foundValidPos = false;
            var itrCounter = 0;

            var currentAngleLeft = signedStartAngle; // Current angle to original starting dir on left
            var currentAngleRight = signedStartAngle; // Current angle to original starting dir on right
            var angularStepCounter = 0;

            var leftCheckingDone = false;
            var rightCheckingDone = false;
            
            // ========= START CHECKING FOR POSITIONS (One iteration per angle)
            while (!foundValidPos && itrCounter < maxItr && (!leftCheckingDone || !rightCheckingDone))
            {
                // ========= DEFINE THE ANGLE & DIRECTION TO FORWARD DIR
                float currentAngle;
                var angularDirection = angularStepCounter%2 == 0 ? -1 : 1;
                if (angularDirection == -1)
                {
                    var currentAngleLeftSample = Mathf.MoveTowards(currentAngleLeft, -coneMaxAngle, maxAngularStep);
                    if (currentAngleLeftSample < -coneMaxAngle)
                    {
                        leftCheckingDone = true;
                        angularStepCounter++;
                        continue;
                    }
                    
                    if (currentAngleLeftSample < coneMinAngle && currentAngleLeft > coneMinAngle)
                        currentAngleLeft = coneMinAngle;
                    else if (currentAngleLeftSample > -coneMinAngle && currentAngleLeftSample < coneMinAngle)
                        currentAngleLeft = -coneMinAngle;
                    else
                        currentAngleLeft = currentAngleLeftSample;

                    currentAngle = currentAngleLeft;
                }
                else
                {
                    var currentAngleRightSample = Mathf.MoveTowards(currentAngleRight, coneMaxAngle, maxAngularStep);
                    if (currentAngleRightSample > coneMaxAngle)
                    {
                        rightCheckingDone = true;
                        angularStepCounter++;
                        continue;
                    }
                    
                    if (currentAngleRightSample > -coneMinAngle && currentAngleRight < -coneMinAngle)
                        currentAngleRight = -coneMinAngle;
                    else if (currentAngleRightSample > -coneMinAngle && currentAngleRightSample < coneMinAngle)
                        currentAngleRight = coneMinAngle;
                    else
                        currentAngleRight = currentAngleRightSample;

                    currentAngle = currentAngleRight;
                }
                
                var currentSamplingDir = Quaternion.AngleAxis(currentAngle, upDir) * forwardDir;
                angularStepCounter++;
                
                // ========= CALCULATE THE TARGET POSITION TO CHECK AGAINST
                potentialValidPos = startPos + currentSamplingDir * preferredRadius;
                
                #if UNITY_EDITOR
                if(doDebug)
                    Debug.DrawLine(startPos, potentialValidPos, Color.red, rayDisplayTime);
                #endif
                // ========= START THE MAGIC
                if (checkpointPlacement == CheckpointPlacement.TerrainGround)
                {
                    // ========= FIND TERRAIN & EVALUATE ACTUAL TARGET POSITION ON THE GROUND
                    var terrain = GetClosestTerrain(potentialValidPos);
                    if (terrain == null) continue;
                    
                    var targetHeightAtPos = terrain.SampleHeight(potentialValidPos) + heightAboveGround;
                    potentialValidPos.y = targetHeightAtPos;

                    var dirToPotValidPos = (potentialValidPos-startPos).normalized;
                    var distToPotValidPos = Vector3.Distance(startPos, potentialValidPos);

                    if (targetMustBeVisibleFromStart && Physics.Raycast(startPos, dirToPotValidPos, out var hitInfo, distToPotValidPos, raycastLayers))
                    {
                        
                        
                        // Obstacle in the way, no valid pos
                        continue;
                    }
                    
                    #if UNITY_EDITOR
                    if (doDebug)
                        Debug.DrawLine(startPos, startPos + currentSamplingDir * preferredRadius, Color.blue, rayDisplayTime);
                    #endif
                    // ========= SAMPLE TESTING
                    var slopeSampleDist = distToPotValidPos / slopeTestingSamples;
                    var lastSamplePos = startPos;
                    var lastSampleTooSteep = false; // Used for fall height calculation
                    var currentFallDistance = 0f;
                    for (var i = 1; i <= slopeTestingSamples; i++)
                    {
                        // ========= FIND THE TERRAIN BELOW SAMPLE
                        var samplePos = startPos + slopeSampleDist* i*dirToPotValidPos;
                        terrain = GetClosestTerrain(samplePos);
                        
                        if (terrain == null)
                            goto EndOfMainLoop; // Invalid position, there no terrain was found at this pos
                        
                        var terrainLocalSamplePos = terrain.transform.InverseTransformPoint(samplePos);
                        samplePos = SetPointToTerrainHeight(samplePos, heightAboveGround, true, terrain);
                        var sampleSteepness = terrain.terrainData.GetSteepness(terrainLocalSamplePos.x/terrain.terrainData.size.x, terrainLocalSamplePos.z/terrain.terrainData.size.z);

                        var dirFromLastToCurrentSample = (samplePos - lastSamplePos).normalized;

                        #if UNITY_EDITOR
                        if (doDebug)
                            Debug.DrawRay(samplePos, Vector3.up * 3, slopeSamplePosColor, rayDisplayTime);
                        #endif
                        
                        var dirFromLastToCurrentSampleNoY = new Vector3(dirFromLastToCurrentSample.x,0,dirFromLastToCurrentSample.z);
                            
                        // ========= CALCULATE STEEPNESS AT SAMPLE POSITION
                        var axisLeftDir = Quaternion.AngleAxis(Mathf.Sign(dirFromLastToCurrentSample.y)*-90, Vector3.up) * dirFromLastToCurrentSampleNoY;
                        var steepnessDirectionalNormal = Quaternion.AngleAxis(sampleSteepness, axisLeftDir) * dirFromLastToCurrentSampleNoY; // Normal of the terrain in the direction of (LastSampl -> Current Sample)
                        steepnessDirectionalNormal.Normalize();
                        
                        #if UNITY_EDITOR
                        if(doDebug)
                            Debug.DrawRay(samplePos, steepnessDirectionalNormal * 5, Color.magenta, rayDisplayTime);
                        #endif
                        
                        // ========= EVALUATE VALID FALL DISTANCE/DROP
                        if ((sampleSteepness > maxSlope || lastSampleTooSteep) && steepnessDirectionalNormal.y < 0)
                        {
                            currentFallDistance += lastSamplePos.y - samplePos.y;

                            if (currentFallDistance > maxFallDistance)
                                goto SampleFailed;
                        }
                        else
                            currentFallDistance = 0;
                        
                        // ========= CHECK IF TOO STEEP AT CURRENT SAMPLE POS (+ Check against step height)
                        if (sampleSteepness > maxSlope)
                        {
                            // TODO Important: We assume the terrain lays "flat" in the world - no rotation;
                            lastSampleTooSteep = true;

                            var slopeHeightSampleDistance = Mathf.Abs(stepHeight / Mathf.Sin(sampleSteepness));
                            steepnessDirectionalNormal *= (slopeHeightSampleDistance / 2);

                            // ========= CHECK AGAINST STEP HEIGHT (See if the too steep point is passable)
                            var frontSubSamplePoint = samplePos + steepnessDirectionalNormal;
                            var backSubSamplePoint = samplePos - steepnessDirectionalNormal;

                            // Set the 2 sub-samples to according terrain heights
                            frontSubSamplePoint = SetPointToTerrainHeight(frontSubSamplePoint, heightAboveGround, true);
                            backSubSamplePoint = SetPointToTerrainHeight(backSubSamplePoint, heightAboveGround, true);


                            var subSampleSteepness = float.NaN;
                            if (frontSubSamplePoint != default && backSubSamplePoint != default)
                            {
                                var dirBackToFront = frontSubSamplePoint - backSubSamplePoint;
                                var dirBackToFrontZeroY = new Vector3(dirBackToFront.x, 0, dirBackToFront.z);
                                subSampleSteepness = Vector3.Angle(dirBackToFrontZeroY, dirBackToFront);
                                
                                #if UNITY_EDITOR
                                if (doDebug)
                                {
                                    Debug.DrawRay(frontSubSamplePoint, Vector3.up, Color.blue, rayDisplayTime);
                                    Debug.DrawRay(backSubSamplePoint, Vector3.up, Color.blue, rayDisplayTime);
                                    Debug.DrawRay(backSubSamplePoint, dirBackToFront, (float.IsNaN(subSampleSteepness) || subSampleSteepness > maxSlope)?Color.red:Color.green, rayDisplayTime);
                                    
                                }
                                #endif
                            }
                            
                            // Steepness at sample point AND slope height is too high; No valid path!
                            if ((float.IsNaN(subSampleSteepness) || subSampleSteepness > maxSlope) && (maxFallDistance < 0 || steepnessDirectionalNormal.y >= 0 ))
                            {
                                goto SampleFailed;
                            }
                        }
                        else
                            lastSampleTooSteep = false;
                        
                        #if UNITY_EDITOR
                        if (doDebug)
                            Debug.DrawLine(samplePos, lastSamplePos, Color.green,rayDisplayTime);
                        #endif

                        // ========= CHECK FOR ANY OBSTACLE BETWEEN LAST & CURRENT SAMPLE
                        var distFromLastToCurrentSample = Vector3.Distance(lastSamplePos, samplePos);
                        if (Physics.Raycast(lastSamplePos, dirFromLastToCurrentSample, out hitInfo, distFromLastToCurrentSample, raycastLayers))
                        {
                            #if UNITY_EDITOR
                            if(doDebug)
                                Debug.DrawLine(lastSamplePos, hitInfo.point, Color.cyan,rayDisplayTime);
                            #endif
                        
                            // Obstacle in the way, no valid pos
                            goto SampleFailed;
                        }
                        
                        
                        lastSamplePos = samplePos;
                        continue;
                        
                        SampleFailed:
                        #if UNITY_EDITOR
                        if (doDebug)
                            Debug.DrawLine(samplePos, lastSamplePos, Color.red,rayDisplayTime);
                        #endif
                        goto EndOfMainLoop;

                    }
                    
                    // ========= FOUND A VALID POSITION! (By reaching this point it means the path is valid)
                    foundValidPos = true;
                    var steepness = terrain.terrainData.GetSteepness(potentialValidPos.x, potentialValidPos.z);
                    var nextCheckpointForwardDirZeroY = dirToPotValidPos;
                    nextCheckpointForwardDirZeroY.y = 0;
                    var newCheckpointLeftDir = Quaternion.FromToRotation(new Vector3(forwardDir.x, 0,forwardDir.z), nextCheckpointForwardDirZeroY) * Vector3.Cross(forwardDir, upDir);//Quaternion.FromToRotation(forwardDir, (dirToStart*-1)) * Vector3.Cross(forwardDir, upDir);
                    
                    validNewCheckpointRotation = Quaternion.LookRotation(Quaternion.AngleAxis(steepness, newCheckpointLeftDir) * nextCheckpointForwardDirZeroY);
                }

                /*if (checkpointPlacement == CheckpointPlacement.Ground ||
                    (checkpointPlacement == CheckpointPlacement.TerrainGround && terrain == null))
                {
                    
                }*/
                EndOfMainLoop:
                itrCounter++;
            }
            
            return (foundValidPos ? CreateCheckpointAtPosition(potentialValidPos, validNewCheckpointRotation) : null);
        }

/*

    // TODO Maybe possible to make a function for coroutine invocation that can be used for all methods? maybe not.
        
        // TODO Following is an idea to also offer execution as a coroutine
        // If previousCheckpoints != null, it will add the new checkpoint to the nextCheckpointTargets list of each contained checkpoint
        // checkpointDoneCallback: UnityAction<Checkpoint> that gets invoked when the checkpoint is done creating delivering the new checkpoint object as parameter
        public void CR_GenCheckpointWithinRadius(Vector3 startPos, Vector3 startForwardDir, Vector3 upDir, float coneMaxAngle = 180f, float signedStartAngle = 0, float coneMinAngle = 0, List<Checkpoint> previousCheckpoints = null, UnityAction<Checkpoint> checkpointDoneCallback = null)
        {
            StartCoroutine(_GenCheckpointWithinRadius(checkpointDoneCallback, startPos, startForwardDir, upDir, coneMaxAngle,signedStartAngle,coneMinAngle, previousCheckpoints));

        }

        private IEnumerator _GenCheckpointWithinRadius(UnityAction<Checkpoint> callbackFunc, Vector3 startPos, Vector3 startForwardDir, Vector3 upDir,  float coneMaxAngle = 180f,
            float signedStartAngle = 0, float coneMinAngle = 0, List<Checkpoint> previousCheckpoints = null)
        {
            Checkpoint newCheckpoint = null;

            // TODO Functionality
            
            previousCheckpoints?.ForEach(checkpoint =>
            {
                checkpoint.nextCheckpoints.Add(newCheckpoint);
            });
            
            callbackFunc?.Invoke(newCheckpoint);
            
            yield return 0;
        }*/

        /// <summary>
        /// Gets the closest terrain at given position.
        /// </summary>
        /// <param name="pos">The sample position in world coordinates.</param>
        /// <param name="mustBeWithinBounds">Defines if the sample position must be within the bounds of the terrain.</param>
        /// <returns>Terrain</returns>
        private static Terrain GetClosestTerrain(Vector3 pos, bool mustBeWithinBounds = true)
        {
            var terrains = Terrain.activeTerrains;
            
            if (terrains.Length == 0)
                return null;
            
            Terrain closestTerrain = null;
            var closestTerrainDist = float.PositiveInfinity;
            
            terrains.ForEach(terrain =>
            {
                pos.y = terrain.terrainData.bounds.center.y; // HACK
                var dist = Vector3.Distance(terrain.GetPosition(), pos);
                
                if (!(dist < closestTerrainDist) || (mustBeWithinBounds &&
                                                     !terrain.terrainData.bounds.Contains(
                                                         terrain.transform.InverseTransformPoint(pos)))) return;
                closestTerrain = terrain;
                closestTerrainDist = dist;

            });
            return closestTerrain;
        }

        // IMPORTANT: You can specify a terrain to check on, if not set, it tries to find it itself.
        // mustHaveTerrainBelow = if set to true, it will return default(Vector3) if no terrain below point was found. If false, it will return the given point.
        /// <summary>
        /// Sets the given positions height to the height of the underlying terrain (if any).
        /// </summary>
        /// <param name="point">The position in world coordinates to move to the terrain.</param>
        /// <param name="heightAboveTerrain">Sets the positions height to the given value above the terrains height.</param>
        /// <param name="mustHaveTerrainBelow">Specifies if there must be a terrain below the point in order to return a valid position.</param>
        /// <param name="terrainToUse">If specified, the point will be tried to set to the given terrains height.</param>
        /// <returns>Returns the given position with a modified height. If mustHaveTerrainBelow is set to true, default(Vector3) will be returned otherwise the given point, unmodified.</returns>
        private static Vector3 SetPointToTerrainHeight(Vector3 point, float heightAboveTerrain = 0, bool mustHaveTerrainBelow = false, Terrain terrainToUse = null)
        {
            terrainToUse = !terrainToUse? GetClosestTerrain(point) : terrainToUse;
            if (terrainToUse == null)
                return mustHaveTerrainBelow ? default : point;

            point.y = terrainToUse.SampleHeight(point) + heightAboveTerrain;

            return point;
        }

        /// <summary>
        /// Creates a new Checkpoint Gameobject and returns it.
        /// </summary>
        /// <param name="worldPos">The position in world space to spawn the checkpoint in.</param>
        /// <param name="worldRot">The rotation in world space of the new checkpoint.</param>
        /// <returns>A newly instatiated checkpoint.</returns>
        private Checkpoint CreateCheckpointAtPosition(Vector3 worldPos, Quaternion worldRot)
        {
            if (checkpointPrefab == null)
                return null;

            var checkpointGo = GameObject.Instantiate(checkpointPrefab, worldPos, worldRot);
            var checkpoint = checkpointGo.GetComponent<Checkpoint>() ?? checkpointGo.AddComponent<Checkpoint>();
            
            return checkpoint;
        }
    }
}
