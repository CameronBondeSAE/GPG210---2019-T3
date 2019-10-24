using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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
        
        [ShowInInspector]
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

        public float angleToVelocity = 0;
        public float angleToVelocityMultiplier = 0;
        
        public float angleForceToGround = 0;
        public float angleForceToGroundMultiplier = 0;

        private void ApplyAirForces()
        {
            Vector3 localVelocity = transform.InverseTransformDirection(masterRb.velocity);

            //localVelocity.x = 0;
            //localVelocity.z = 0;

            angleToVelocity = Vector3.Angle(Vector3.forward, localVelocity);
            angleToVelocityMultiplier = Mathf.Sin(angleToVelocity * (Mathf.PI / 180)) + 0.3f;

            Vector3 finalForce =
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
    }
}
