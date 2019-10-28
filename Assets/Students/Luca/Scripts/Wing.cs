using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Students.Luca
{
    public class Wing : MonoBehaviour
    {
        public GameObject autoResetIndicatorDebug;
        public bool doDebug = false;
        
        public Rigidbody masterRb;

        public KeyCode toggleAutoResetKey; // Toggles bool "autoResetAngle"
        public KeyCode forwardRotateKey;
        public KeyCode backwardRotateKey;
        public Vector3 maxRotationAngles = new Vector3(0,0,0);
        /*public float maxXRotAngle = 30; // TODO DELETE*/
        public float rotationSpeed = 30;

        public bool useAngularVelocity = false;
        
        [ShowInInspector, SerializeField]
        private bool autoResetAngle = true; // If true, it will reset to its default position when no input is there

        public bool AutoResetAngle
        {
            get => autoResetAngle;
            set
            {
                autoResetAngle = value;
                
                // Following is Debug Code. Delete.
                if (autoResetIndicatorDebug == null || !doDebug)
                    return;
                
                if (value && !autoResetIndicatorDebug.activeSelf)
                {
                    autoResetIndicatorDebug.SetActive(true);
                }else if (!value && autoResetIndicatorDebug.activeSelf)
                {
                    autoResetIndicatorDebug.SetActive(false);
                }
            }
        }

        public float airForce = 10;

        public float wingSizeMultiplier = 1;

        private Quaternion defaultRotation;
        private Quaternion maxForwardRotation; // Up
        private Quaternion maxBackwardRotation; // Down


        public float angleToVelocity = 0;
        public float angleToVelocityMultiplier = 0;
        
        public float angleForceToGround = 0;
        public float angleForceToGroundMultiplier = 0;

        // TEST VALUES FOR ANGULAR VELOCITY/ROTOR BLADES
        public float airDensity = 1.1644f; // https://en.wikipedia.org/wiki/Density_of_air#Dry_air 30°C 1.1644
        public float rotorArea = 3.5f; // Could be calculated by its actual mesh size; in m^2 
        public float rotorRadius = 7; // Fictional length of the rotor; will be used to calculate lift forces at specific points
        public float numberOfAttackPoints = 5; // Defines how many "Attack"-points (/Positions which generate upforce) there should be (Since we don't want to have one for every pixel like in reality)
        public Rigidbody rotorRb;
        
        [ShowInInspector]
        private float actualRotorRadius = 1; // Length of the rotor mesh
        
        
        // Start is called before the first frame update
        void Start()
        {
            defaultRotation = transform.localRotation;
            //Hack
            Vector3 maxRotationEuler = transform.localRotation.eulerAngles;
            maxRotationEuler += maxRotationAngles;
            maxForwardRotation = Quaternion.Euler(maxRotationEuler);

            maxRotationEuler = transform.localRotation.eulerAngles;
            maxRotationEuler -= maxRotationAngles;
            maxBackwardRotation = Quaternion.Euler(maxRotationEuler);


            if (masterRb == null)
            {
                masterRb = GetComponentInParent<Rigidbody>();
            }
            
            // Angular WIng style stuff
            if (useAngularVelocity)
            {
                Collider rotorCollider = GetComponent<Collider>();
                if (rotorCollider != null)
                {
                    actualRotorRadius = Mathf.Abs(transform.InverseTransformDirection(rotorCollider.bounds.size).x);
                }
                else
                {
                    
                    actualRotorRadius = Mathf.Abs(transform.InverseTransformDirection(GetComponent<Mesh>()?.bounds.size ?? Vector3.one).x);
                }
                
            }
            
            // Debug stuff
            if (autoResetIndicatorDebug == null)
                return;
            if (autoResetIndicatorDebug.activeSelf && (!autoResetAngle || !doDebug))
            {
                autoResetIndicatorDebug.SetActive(false);
            }else if (doDebug && AutoResetAngle && !autoResetIndicatorDebug.activeSelf)
            {
                autoResetIndicatorDebug.SetActive(true);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(toggleAutoResetKey))
            {
                AutoResetAngle = !AutoResetAngle;
            }
            
            if (Input.GetKey(forwardRotateKey))
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, maxForwardRotation,
                    rotationSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(backwardRotateKey))
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, maxBackwardRotation,
                    rotationSpeed * Time.deltaTime);
            }
            else if (AutoResetAngle && transform.localRotation != defaultRotation)
            {
                transform.localRotation =
                    Quaternion.RotateTowards(transform.localRotation, defaultRotation, rotationSpeed * Time.deltaTime);
            }/*
            else if (AutoResetAngle && !Mathf.Approximately(transform.localRotation.eulerAngles.x, defaultRotation.eulerAngles.x))
            {
                transform.localRotation =
                    Quaternion.RotateTowards(transform.localRotation, defaultRotation, rotationSpeed * Time.deltaTime);
            }*/

            ApplyAirForces();

        }
        
        private void ApplyAirForces()
        {
            Vector3 finalForce = Vector3.zero;
            if (!useAngularVelocity)
            {
                Vector3 localVelocity = transform.InverseTransformDirection(masterRb.velocity);

                //localVelocity.x = 0;
                //localVelocity.z = 0;

                angleToVelocity = Vector3.Angle(Vector3.forward, localVelocity);
                angleToVelocityMultiplier = Mathf.Sin(angleToVelocity * (Mathf.PI / 180)) + 0.3f;

                finalForce =
                    masterRb.transform.TransformDirection(-localVelocity * angleToVelocityMultiplier * airForce * wingSizeMultiplier);

                if (doDebug)
                {
                    //Debug.DrawRay(transform.position, -finalForce, Color.green);
                    Debug.DrawRay(transform.position,
                        finalForce, Color.green);
                }
            

                //TODO Needed?  Slow down force depending on the angle towards the ground
                angleForceToGround = Vector3.Angle(new Vector3(finalForce.x,0,finalForce.z), finalForce);
                angleForceToGroundMultiplier = Mathf.Cos(angleForceToGround);

                //finalForce *= angleForceToGroundMultiplier;
                masterRb.AddForceAtPosition(finalForce, transform.position);
            }
            else
            {
                // L = CI * (r * V^2)/2 * A -> Lift = coefficient * (density * velocity^2)/2 * wing area
                // ==> Simplified: .5 * CI * r * V^2 * A

                Vector3 angularVelocityY = rotorRb?.angularVelocity ?? Vector3.zero; // TODO Wrong naming
                angularVelocityY.x = float.IsNaN(angularVelocityY.x) ? 0 : (angularVelocityY.x>0.5f?angularVelocityY.x:(angularVelocityY.x<-.5f?angularVelocityY.x:0));
                angularVelocityY.z = float.IsNaN(angularVelocityY.z) ? 0 : (angularVelocityY.z>0.5f?angularVelocityY.z:(angularVelocityY.z<-.5f?angularVelocityY.z:0));
                angularVelocityY.y = float.IsNaN(angularVelocityY.y) ? 0 : (angularVelocityY.y>0.5f?angularVelocityY.y:(angularVelocityY.y<-.5f?angularVelocityY.y:0));

                float localBladeAngle = transform.localRotation.eulerAngles.x % 360;
                localBladeAngle = Mathf.Abs(localBladeAngle > 180 ? (localBladeAngle - 360) : -localBladeAngle); // Limit Angle to 180°
                float angleOfAttack = /*Vector3.Angle(transform.forward, new Vector3(transform.forward.x,0,transform.forward.z))*/localBladeAngle*Mathf.Deg2Rad;
                
                // Coefficient CI: 2 * PI * Angle (Angle of Attack in Radians!)
                float CI = 2 * Mathf.PI * angleOfAttack;

                float xVirtualPositionSteps = rotorRadius / (numberOfAttackPoints+1); // +1 Because we don't count the rotor position as an attack point (Since the up lift is 0 there anyways)
                float xActualPositionSteps = actualRotorRadius / (numberOfAttackPoints+1);
                for (int i = 1; i <= numberOfAttackPoints; i++) // Info: Purposely starting at i=1 since we don't have to calculate the 0-point/Rotor position
                {
                    // Velocity at Tip
                    float velocitySquared = Mathf.Pow(i*xVirtualPositionSteps * angularVelocityY.magnitude,2); // Distance from Rotor (Not accurate in this model) times the angular velocity of the rotor
                    
                    // Lift Force
                    Vector3 forceDir = Vector3.up/*transform.up*/;
                    forceDir.Scale(transform.InverseTransformDirection(angularVelocityY.Sign()));
                    
                    Vector3 liftForce = .5f * CI * airDensity * rotorArea * velocitySquared * transform.TransformDirection(forceDir);
                    Vector3 forcePos = Vector3.right * ((actualRotorRadius/2) - i*xActualPositionSteps); // Requires pivot to be in the center; Right mus point to rotor Hacky
                    masterRb.AddForceAtPosition(liftForce, transform.TransformPoint(forcePos));
                    
                    if (doDebug)
                    {
                        //Debug.Log(liftForce+" at pos "+transform.TransformPoint(forcePos) + " CI: "+CI+" Force Dir: "+forceDir+" "+velocitySquared + " " + localBladeAngle);
                        Debug.DrawRay(transform.TransformPoint(forcePos),
                            liftForce, Color.green);
                    }
                }
                /*

                
                // Velocity at Tip
                Vector3 vTip = rotorRadius * anglularVelocityY;
                Vector3 vTipSquared = vTip;
                vTipSquared.Scale(vTip);
                
                // Lift Force (At tip of Rotor)
                
                finalForce = .5f * CI * rotorRadius * vTipSquared * rotorArea;
                //Debug.Log(finalForce+ "  "+transform.localRotation.eulerAngles.x+ " vTip^2: "+vTipSquared+" CL: "+CI+" RotRad: "+rotorRadius+" angVel: "+anglularVelocityY);
                Vector3 tipPos = Vector3.right * -(rotorRadius/2); // If Pivot in center....Hacky anyways
                masterRb.AddForceAtPosition(finalForce, transform.TransformPoint(tipPos));
                
                if (doDebug)
                {
                    Debug.DrawRay(transform.TransformPoint(tipPos),
                        finalForce, Color.green);
                }*/
            }
            
        }
    }
}
