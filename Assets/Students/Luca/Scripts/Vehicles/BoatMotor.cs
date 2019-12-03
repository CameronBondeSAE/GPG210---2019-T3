using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Students.Luca.Scripts
{
    public class BoatMotor : MonoBehaviour
    {
        
        public Rigidbody masterRb = null;
        public Transform exhaustPosition;
        public float motorStrength = 400;
        public BuoyantBody buoyantBody = null;

        public ParticleSystem exhaustEmition;
        

        private float currentSpeed = 0; // -1 ... 1
        public float CurrentSpeed
        {
            get => currentSpeed;
            set => currentSpeed = Mathf.Clamp(value,-1,1);
        }
        
        private float currentDesiredSpeed = 0; // -1 ... 1
        [ShowInInspector]
        public float CurrentDesiredSpeed
        {
            get => currentDesiredSpeed;
            set => currentDesiredSpeed = Mathf.Clamp(value,-1,1);
        }
        
        
        public Vector3 autoResetAngle = Vector3.zero;
        public Vector3 maxTurnAngles = Vector3.zero; //Degrees
        public float rotationSpeed = 30;
        private Vector3 localBaseRotation;
        
        [ShowInInspector]
        private Vector3 currentDesiredRotation = Vector3.zero;
        public Vector3 CurrentDesiredRotation
        {
            get => currentDesiredRotation;
            set => currentDesiredRotation = new Vector3(Mathf.Clamp(value.x,-1,1),Mathf.Clamp(value.y,-1,1),Mathf.Clamp(value.z,-1,1));
        }

        public Water currentWater = null;
        
        [ShowInInspector]
        private bool isInWater = false;
        
        // Start is called before the first frame update
        void Start()
        {
            if (buoyantBody == null)
            {
                buoyantBody = GetComponent<BuoyantBody>();
            }
            if (masterRb == null)
            {
                masterRb = GetComponentInParent<Rigidbody>();
            }
            localBaseRotation = transform.localRotation.eulerAngles;
        }

        // Update is called once per frame
        void Update()
        {
            // HACK
            if (buoyantBody)
                currentWater = buoyantBody.CurrentWater;
            
            if (currentWater)
            {
                var exhaustPos = exhaustPosition.position;
                isInWater = exhaustPos.y <= currentWater.GetSurfaceLevelAtPos(exhaustPos.x, exhaustPos.z);
            }


            HandleSpeed();
            HandleRotation();
        }

        void HandleSpeed()
        {
            // Handle Speed
            if (!Mathf.Approximately(CurrentSpeed, CurrentDesiredSpeed))
            {
                CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, CurrentDesiredSpeed, Time.deltaTime);
            }
            
            if (isInWater && !Mathf.Approximately(CurrentSpeed, 0))
            {
                Vector3 force = motorStrength * CurrentSpeed * exhaustPosition.forward;
                masterRb?.AddForceAtPosition(force,exhaustPosition.position);
                
                if(!exhaustEmition.isPlaying)
                    exhaustEmition?.Play();
                
                //Debug.DrawRay(exhaustPosition.position,force,Color.green);
            }
            else
            {
                exhaustEmition?.Stop();
            }
        }

        void HandleRotation()
        {
            // Handle Rotation
            Vector3 newRotation = transform.localRotation.eulerAngles;

            // Handle X Rotation (Hacky)
            float desiredAngleXDeg = localBaseRotation.x + Mathf.Sign(CurrentDesiredRotation.x) * maxTurnAngles.x;
            if (!Mathf.Approximately(CurrentDesiredRotation.x, 0))
            {
                newRotation.x = Mathf.MoveTowardsAngle(transform.localRotation.eulerAngles.x,desiredAngleXDeg, Time.deltaTime*rotationSpeed);
            }
            else if(autoResetAngle.x > 0 && !Mathf.Approximately(transform.localRotation.eulerAngles.x,localBaseRotation.x))
            {
                newRotation.x = Mathf.MoveTowardsAngle(transform.localRotation.eulerAngles.x, localBaseRotation.x,
                    Time.deltaTime * rotationSpeed);
            }
            /*float desiredAngleXDeg = 0;
            if (autoResetAngle.x > 0)
            {
                desiredAngleXDeg = localBaseRotation.x + Mathf.Sign(CurrentDesiredRotation.x) * maxTurnAngles.x;
                if (!Mathf.Approximately(CurrentDesiredRotation.x, 0))
                {
                    newRotation.x = Mathf.MoveTowardsAngle(transform.localRotation.eulerAngles.x,desiredAngleXDeg, Time.deltaTime*rotationSpeed);
                }
                else if(!Mathf.Approximately(transform.localRotation.eulerAngles.x,localBaseRotation.x))
                {
                    newRotation.x = Mathf.MoveTowards(transform.localRotation.eulerAngles.x, localBaseRotation.x,
                        Time.deltaTime * rotationSpeed);
                }
            }
            else
            {
                desiredAngleXDeg = localBaseRotation.x + CurrentDesiredRotation.x * maxTurnAngles.x;
                if (!Mathf.Approximately(transform.localRotation.eulerAngles.x, desiredAngleXDeg))
                {
                    newRotation.x = Mathf.MoveTowardsAngle(transform.localRotation.eulerAngles.x,desiredAngleXDeg, Time.deltaTime*rotationSpeed);
                }
            }*/
            
            
            // Handly Y Rotation (AutoReset on/off not supported)
            float desiredAngleYDeg = localBaseRotation.y + CurrentDesiredRotation.y * maxTurnAngles.y;
            if (!Mathf.Approximately(transform.localRotation.eulerAngles.y, desiredAngleYDeg))
            {
                newRotation.y = Mathf.MoveTowardsAngle(transform.localRotation.eulerAngles.y,desiredAngleYDeg, Time.deltaTime*rotationSpeed);
            }
            
            // Handly Z Rotation (AutoReset on/off not supported)
            float desiredAngleZDeg = localBaseRotation.z + CurrentDesiredRotation.z * maxTurnAngles.z;
            if (!Mathf.Approximately(transform.localRotation.eulerAngles.z, desiredAngleZDeg))
            {
                newRotation.z = Mathf.MoveTowardsAngle(transform.localRotation.eulerAngles.z,desiredAngleZDeg, Time.deltaTime*rotationSpeed);
            }

            transform.localRotation = Quaternion.Euler(newRotation);

            
            /*if(autoResetAngle.x > 0)
                currentDesiredRotation.x = Mathf.MoveTowards(currentDesiredRotation.x, 0, Time.deltaTime);*/
            if(autoResetAngle.y > 0)
                currentDesiredRotation.y = Mathf.MoveTowards(currentDesiredRotation.y, 0, Time.deltaTime);
            if(autoResetAngle.z > 0)
                currentDesiredRotation.z = Mathf.MoveTowards(currentDesiredRotation.z, 0, Time.deltaTime);
        }
        
    }
}
