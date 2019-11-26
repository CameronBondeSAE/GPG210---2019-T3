using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using DG.Tweening;
using Sirenix.OdinInspector;
using Students.Luca.Scripts;
using Students.Luca.Scripts.Helper;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIBoatDriver : AIDriverBase
{
    public new Boat Possessable
    {
        get => (Boat)base.Possessable;
        set => base.Possessable = value;
    }
    
    [Header("Debug Data")]
    public bool doDebug = false;
    
    [ShowInInspector]
    private Vector3 currentMainTargetLocation = Vector3.negativeInfinity;
    [ShowInInspector]
    private Vector3 currentTargetLocation = Vector3.negativeInfinity;

    private Vector3 dirToCurrentTargetLoc;
    private Vector3 dirToMainTargetLoc;
    
    [ShowInInspector] private float distanceToCurrentTarget;
    [ShowInInspector] private float distanceToMainTarget;
    
    [ShowInInspector] private float angleToMainTarget;
    [ShowInInspector] private float angleToCurrentTarget;
    
    [ShowInInspector] private Vector2 currentLeftStickInput = Vector2.zero;


    
    [Header("Target Settings")]
    public float defaultTurnAngleTreshold = 45; // Degrees
    public float defaultTravelDistance = 5;
    public float distanceToTargetTreshold = 1f;

    public int maxTargetSampleItr = 30;
    public float targetSampleRadialStep = 5; // Degrees
    public float targetSampleDistStep = .1f;
    public float minSampleDistanceToBoat = 2f; // Minimum distance the boat needs to be away from a new target
    public float minSampleDistanceToObstacles = 2f;

    public LayerMask obstacleCollisionLayers;
    
    [Header("Move To Target Settings")]
    public float angleToTargetTreshold = 5; // Min angle to target required in order to turn
    public float maxTurnAtAngle = 30; // Degrees; If the angle to the target is >= this amount, it will do a hard  turn
    public float hackySpeedLogFrequency = 1; // # Per second
    private float hackySpeedLogCooldown = 0;
    
    public float obstacleAvoidanceDistance = 5;
    public float ActualObstacleAvoidanceDistance => obstacleAvoidanceDistance + Possessable.GetBoatLength();
    public float moveBackwardsMaxObstacleDistance = 1f;
    public float ActualMoveBackwardsMaxObstacleDistance =>
        moveBackwardsMaxObstacleDistance + Possessable.GetBoatLength();


    [ShowInInspector]
    private float distanceToObstacleInFront = 0;
    
    [ShowInInspector]
    private bool moveBackward = false;
    
    [Header("Other Settings")]
    public float collisionCheckFrequency = 1f; // Collision Raycasts per second
    [ShowInInspector]
    private float collisionCheckCooldown = 0;

    public float findNewTargetAfterCollisionCooldown = 2f;
    private float currentFindNewTargetAfterCollisionCooldown = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        if (Possessable == null)
            Possessable = GetComponent<Boat>();

        if (Possessable != null)
        {
            currentMainTargetLocation = Possessable.transform.position;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if(Possessable == null)
            return;

        //distanceToTarget = Vector3.Distance(possessable.transform.position, currentTargetLocation);
        
        dirToCurrentTargetLoc = currentTargetLocation - Possessable.transform.position;
        dirToMainTargetLoc = currentMainTargetLocation - Possessable.transform.position;
        distanceToCurrentTarget = dirToCurrentTargetLoc.magnitude;//Vector3.Distance(new Vector3(Possessable.transform.position.x,0,Possessable.transform.position.z), new Vector3(currentTargetLocation.x,0,currentTargetLocation.z));
        distanceToMainTarget = dirToMainTargetLoc.magnitude;//Vector3.Distance(new Vector3(Possessable.transform.position.x,0,Possessable.transform.position.z), new Vector3(currentMainTargetLocation.x,0,currentMainTargetLocation.z));
        
        angleToMainTarget = Vector3.Angle(dirToMainTargetLoc, Possessable.transform.forward);
        angleToCurrentTarget = Vector3.Angle(dirToCurrentTargetLoc, Possessable.transform.forward);
        
        
        
        if (currentMainTargetLocation == Vector3.negativeInfinity || distanceToMainTarget <= distanceToTargetTreshold)
        {
            Vector3 newRotDeg = Possessable.transform.rotation.eulerAngles;
            newRotDeg.y += Random.Range(-defaultTurnAngleTreshold,defaultTurnAngleTreshold);
            Vector3 initialRaycastForwardDir = Quaternion.Euler(newRotDeg) * Vector3.forward;//Possessable.transform.forward;
            Debug.DrawRay(Possessable.transform.position, initialRaycastForwardDir * 10, Color.green, 5f);
            currentMainTargetLocation = GetNewTargetLocation(defaultTravelDistance,initialRaycastForwardDir,TSSDirSetting.PingPong, TSSDistReductionSetting.PerSample);
            currentTargetLocation = currentMainTargetLocation;
        }else if (currentMainTargetLocation != currentTargetLocation &&
                  (distanceToCurrentTarget <= distanceToTargetTreshold || 
                   (!DoRcCollisionCheck(dirToMainTargetLoc, distanceToMainTarget)/* && 
                        angleToMainTarget < 45*/
                        /*(dirToMainTargetLoc * Mathf.Cos(angleToMainTarget*Mathf.Deg2Rad)).magnitude <= (dirToCurrentTargetLoc * Mathf.Cos(angleToCurrentTarget*Mathf.Deg2Rad)).magnitude*/)))
        {
            currentTargetLocation = currentMainTargetLocation;
        }

        
        
        // Check for (potential) collision in front
        if (collisionCheckCooldown <= 0)
        {
            
            RaycastHit forwardRayHit = GetRcCollision(Possessable.transform.forward, ActualObstacleAvoidanceDistance);
            if (!forwardRayHit.Equals(default(RaycastHit)))
            {
                distanceToObstacleInFront = forwardRayHit.distance;

                if (distanceToObstacleInFront < distanceToCurrentTarget || angleToCurrentTarget > 90)
                {
                    // Check if the path to the current target would be free & the angle towards the current target is bigger than X; if yes, move backwards
                    if (/*!DoRcCollisionCheck(dirToCurrentTargetLoc, distanceToTarget) && angleToCurrentTarget > 45 &&*/ distanceToObstacleInFront <= ActualMoveBackwardsMaxObstacleDistance)
                    {
                        moveBackward = true;
                    }
                    else if( currentFindNewTargetAfterCollisionCooldown <= 0)
                    {
                        moveBackward = false;
                        TSSDirSetting tssDirSetting = TSSDirSetting.PingPong;
                        if (angleToMainTarget < 30)
                        {
                            switch (GameHelper.AngleDir(Possessable.transform.forward, dirToMainTargetLoc, Vector3.up))
                            {
                                case -1:
                                    tssDirSetting = TSSDirSetting.Left;
                                    break;
                                case 1:
                                    tssDirSetting = TSSDirSetting.Right;
                                    break;
                                default:
                                case 0:
                                    tssDirSetting = TSSDirSetting.PingPong;
                                    break;
                            }
                        }
                        currentTargetLocation = GetNewTargetLocation(-1, Possessable.transform.forward,tssDirSetting, TSSDistReductionSetting.PerSample);
                        currentFindNewTargetAfterCollisionCooldown = findNewTargetAfterCollisionCooldown;
                    }
                        
                }
                
                
                
                if(doDebug)
                    Debug.DrawRay(Possessable.transform.position, Possessable.transform.forward*forwardRayHit.distance, Color.yellow, 1f);
            }
            else
            {
                distanceToObstacleInFront = -1;
                moveBackward = false;
            }
            
            collisionCheckCooldown = collisionCheckFrequency;
        }
        else
        {
            collisionCheckCooldown -= Time.deltaTime;
        }
        
        if (angleToCurrentTarget > 90 && DoRcCollisionCheck(dirToCurrentTargetLoc, distanceToCurrentTarget))
        {
            moveBackward = true;
        }
        else if(distanceToObstacleInFront < 0 ||distanceToObstacleInFront > ActualMoveBackwardsMaxObstacleDistance)
        {
            moveBackward = false;
        }

        if (currentFindNewTargetAfterCollisionCooldown > 0)
            currentFindNewTargetAfterCollisionCooldown -= Time.deltaTime;
        
        
        // Check if in water
        if (!Possessable.IsInWater() || Possessable.IsUnderWater())
        {
            currentLeftStickInput.y = 0;
        }else if (Possessable.transform.position != currentTargetLocation && distanceToCurrentTarget > distanceToTargetTreshold) // move to target
        {
            MoveToTarget();
        }
            
        
        // Apply Input to possessable
        Possessable.LeftStickAxis(currentLeftStickInput);

        // Super hacky Speed vs Input Log
        if (hackySpeedLogCooldown <= 0)
        {
            LogInputVsSpeed(); // SUPER HACKY
            hackySpeedLogCooldown = hackySpeedLogFrequency;
        }
        else
            hackySpeedLogCooldown -= Time.deltaTime;

        
        if (doDebug)
        {
            Debug.DrawRay(Possessable.transform.position, dirToCurrentTargetLoc, Color.magenta);
            Debug.DrawRay(currentTargetLocation, (currentMainTargetLocation-currentTargetLocation), Color.black);
        }
    }

    // Target Search Scan Directional Setting
    private enum TSSDirSetting
    {
        PingPong,
        Left,
        Right
    }

    // Target Search Scan Distance Reduction Setting
    private enum TSSDistReductionSetting
    {
        AfterEachRound,
        PerSample
    }
    
    // preferredDistance = -1: Try to get nearest position where there is no collision; if no collision it defaults to preferred distance.
    private Vector3 GetNewTargetLocation(float preferredDistance, Vector3 raycastStartDir, TSSDirSetting tssDirSetting = TSSDirSetting.PingPong, TSSDistReductionSetting tssDistReductionSetting = TSSDistReductionSetting.AfterEachRound)
    {
        Vector3 newTargetLocation = transform.position;

        int itrCounter = 0;

        Vector3 raycastDir = raycastStartDir;
        
        int dirMultiplier = tssDirSetting == TSSDirSetting.Left? -1 : 1;
        float secondlastDistanceToObstacle = -1;
        float lastDistanceToObstacle = -1;
        float rxMaxDistance = preferredDistance > 0 ? preferredDistance : defaultTravelDistance;

        int iterationsPerRound = Mathf.FloorToInt(360 / targetSampleRadialStep);
        while (itrCounter < maxTargetSampleItr)
        {
            if (tssDistReductionSetting == TSSDistReductionSetting.AfterEachRound && itrCounter > iterationsPerRound && itrCounter%iterationsPerRound == 1/* && Mathf.Floor(itrCounter / 2f) * Mathf.Abs(targetSampleRadialStep) > 180*/)
            {
                rxMaxDistance -= targetSampleDistStep;
                if (rxMaxDistance < minSampleDistanceToBoat) 
                {
                    break; // Too close, no target found.
                }
            }else if (tssDistReductionSetting == TSSDistReductionSetting.PerSample)
            {
                rxMaxDistance = preferredDistance > 0 ? preferredDistance : defaultTravelDistance;
            }
            
            if (doDebug)
            {
                Debug.DrawRay(Possessable.transform.position, raycastDir.normalized * rxMaxDistance, Color.red, .1f);
            }

            do
            {
                RaycastHit hit = GetRcCollision(raycastDir, rxMaxDistance);
                float nextLastDistToObstacle = Vector3.Distance(Possessable.transform.position, hit.point); // HACKY TMP
                if (hit.Equals(default(RaycastHit)))
                {// Found location
                    
                    float actualDistance = (preferredDistance > 0 ? rxMaxDistance : (tssDirSetting == TSSDirSetting.PingPong ? (secondlastDistanceToObstacle > 0 ? secondlastDistanceToObstacle : rxMaxDistance) : (lastDistanceToObstacle > 0 ? lastDistanceToObstacle : rxMaxDistance)));
                    
                    Vector3 potentialNewTargetLocation; // (Second last cuz the raycast check is pingponging)
                    
                    
                    // TODO check if wall too near, if yes, reduce distance.
                    Collider[] obstacleHits = new Collider[1];
                    float obstacleHitCount = 0;
                    //Debug.Log("==============  Found pos, check for obstacles ==========");
                    do
                    {
                        if (obstacleHitCount > 0)
                        {
                            actualDistance -= targetSampleDistStep;
                            nextLastDistToObstacle = actualDistance;
                            if (actualDistance < minSampleDistanceToBoat) 
                            {
                                //Debug.Log("==============  Couldnt find Pos, continue search ==========");
                                if (tssDistReductionSetting == TSSDistReductionSetting.PerSample)
                                    rxMaxDistance = actualDistance;
                                
                                goto UglyGoToPos; // No valid position found, continue searching
                            }
                        }
                        
                        potentialNewTargetLocation = Possessable.transform.position + raycastDir.normalized * actualDistance;
                        potentialNewTargetLocation.y = Possessable.transform.position.y; // Hacky

                        obstacleHitCount = Physics.OverlapBoxNonAlloc(potentialNewTargetLocation,
                            new Vector3(minSampleDistanceToObstacles, minSampleDistanceToObstacles,
                                minSampleDistanceToObstacles), obstacleHits, Quaternion.identity, obstacleCollisionLayers);

                    } while (obstacleHitCount > 0);
                    /*Debug.Log("==============  Found FINAL pos, done checking for obstacles ==========");
                        
                    if(actualDistance > defaultTravelDistance)
                        Debug.Log("HYARGH: "+actualDistance+" rxd: "+rxMaxDistance+" 2ndlast: "+secondlastDistanceToObstacle+" last: "+lastDistanceToObstacle);*/
                    
                    newTargetLocation = potentialNewTargetLocation;
                    goto UglyExitGoToPos; 
                }
                UglyGoToPos:
                secondlastDistanceToObstacle = lastDistanceToObstacle;
                lastDistanceToObstacle = nextLastDistToObstacle;

                if (tssDistReductionSetting == TSSDistReductionSetting.PerSample)
                {
                    float newDistance = hit.distance - minSampleDistanceToObstacles;
                    rxMaxDistance = newDistance < rxMaxDistance ? newDistance : rxMaxDistance;
                    //rxMaxDistance -= targetSampleDistStep;
                }
                
            } while (rxMaxDistance >= minSampleDistanceToBoat && tssDistReductionSetting == TSSDistReductionSetting.PerSample);
            
            float yRot = (tssDirSetting == TSSDirSetting.PingPong) ? ((targetSampleRadialStep * Mathf.Floor(itrCounter / 2f))%180) : ((targetSampleRadialStep * itrCounter)%360);
            raycastDir = Quaternion.Euler(0,  yRot * dirMultiplier, 0) *
                         raycastStartDir;
            
            if(tssDirSetting == TSSDirSetting.PingPong)
                dirMultiplier *= -1;
            
            itrCounter++;
        }
        UglyExitGoToPos:
        
        return newTargetLocation;
    }

    private bool DoRcCollisionCheck(Vector3 dir, float maxDistance = -1)
    {
        return !(GetRcCollision(dir, maxDistance).Equals(default(RaycastHit)));
    }

    private RaycastHit GetRcCollision(Vector3 dir, float maxDistance = -1)
    {
        return GetRcCollision(Possessable.transform.position, dir, maxDistance);
    }
    
    private RaycastHit GetRcCollision(Vector3 startPos, Vector3 dir, float maxDistance = -1)
    {
        Physics.Raycast(startPos, dir.normalized, out var hit,
            (maxDistance<=0?defaultTravelDistance:maxDistance),obstacleCollisionLayers);
            
        return hit;
    }

    private void MoveToTarget()
    {
        //var dirToTarget = currentTargetLocation - possessable.transform.position;
        
        
        // Handle Turning
        if (angleToCurrentTarget > angleToTargetTreshold)
        {
            var targetSide = GameHelper.AngleDir(Possessable.transform.forward, dirToCurrentTargetLoc, Vector3.up); // CHeck if target is on the left or right
            
            var lTriggerInput = Mathf.Clamp(angleToCurrentTarget, 0, maxTurnAtAngle) / maxTurnAtAngle; // The bigger the angle to target, the "harder" the turn
            switch (targetSide)
            {
                // Target is on Left
                case 1:
                    currentLeftStickInput.x = lTriggerInput * (moveBackward?1:-1);
                    break;
                // Target is on Right
                case -1:
                    currentLeftStickInput.x = lTriggerInput * (moveBackward?-1:1);
                    break;
            }
        }
        else
        {
            currentLeftStickInput.x = 0;
        }
        
        // Handle Speed
        float velocityNeededToReachTargetInXSec = distanceToCurrentTarget / 2f;

        double roundedDesiredVel = Math.Round(velocityNeededToReachTargetInXSec, 1) * (moveBackward?-1:1);
        float forwardInput = 0f;

        /*if (moveBackward)
        {
            forwardInput = -Mathf.Clamp(1/((distanceToObstacleInFront*1.5f) / (ActualObstacleAvoidanceDistance)),0,1);
        }else */if (!Mathf.Approximately((float)roundedDesiredVel, 0) && hackyInputVsVelocityLog.ContainsKey(roundedDesiredVel) && hackyInputVsVelocityLog[roundedDesiredVel] != null && hackyInputVsVelocityLog[roundedDesiredVel].Count > 5 && !(moveBackward && distanceToObstacleInFront > 0)) // TODO Make var
        {
            forwardInput = (float)hackyInputVsVelocityLog[roundedDesiredVel].Average();
            /*if(doDebug)
                Debug.Log("Super Smart Speed. Desired Vel: "+roundedDesiredVel+" Avg Input: "+forwardInput);*/
        }
        else
        {
            // TODO Consider using motor strength for calculation?
            if(moveBackward && distanceToObstacleInFront > 0)
                forwardInput = -Mathf.Clamp(1/((distanceToObstacleInFront*2.5f) / (ActualObstacleAvoidanceDistance)),0,1);
            else if(moveBackward)
                forwardInput = -Mathf.Clamp((distanceToObstacleInFront*2.5f) / (ActualObstacleAvoidanceDistance),0,1);
            else
                forwardInput = Mathf.Clamp((distanceToCurrentTarget*2.5f) / (defaultTravelDistance),0,1); //Random.Range(0.5f, 1f); // Hacky
        }

        currentLeftStickInput.y = forwardInput;
    }
    
    /// <summary>
    /// TODO I'd also have to consider the current turning angle for the current speed etc.. .
    /// </summary>
    [ShowInInspector]
    private readonly Dictionary<double, List<double>> hackyInputVsVelocityLog = new Dictionary<double, List<double>>(); // Velocity [ List [ input value ]]


    private void LogInputVsSpeed()
    {
        // HACCKKEEYY
        if (currentLeftStickInput.y >= 0.1f)
        {
            double roundedForwardInput = Math.Round(currentLeftStickInput.y, 2);
            Vector3 currentForwardVelocity =
                Possessable.transform.InverseTransformDirection(Possessable.GetComponent<Rigidbody>().velocity);
            double roundedVelocity = Math.Round(currentForwardVelocity.z, 1);
            
            if (hackyInputVsVelocityLog.ContainsKey(roundedVelocity))
            {
                hackyInputVsVelocityLog[roundedVelocity].Add(roundedForwardInput);
            }
            else
            {
                hackyInputVsVelocityLog.Add(roundedVelocity, new List<double>(){roundedForwardInput});
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!doDebug)
            return;

        if (currentTargetLocation != Vector3.negativeInfinity && currentTargetLocation != currentMainTargetLocation)
        {
            Gizmos.color = new Color32(255,0,162, 200);
            Gizmos.DrawCube(currentTargetLocation, Vector3.one * 2);
        }
        
        if (currentMainTargetLocation != Vector3.negativeInfinity)
        {
            Gizmos.color = new Color32(255,187,0, 200);
            Gizmos.DrawCube(currentMainTargetLocation, Vector3.one * 4);
        }
        
    }
}
