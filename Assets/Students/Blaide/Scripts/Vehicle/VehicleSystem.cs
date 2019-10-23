using System;
using System.Collections;
using System.Collections.Generic;
using Students.Blaide;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Students.Blaide
{
    public class VehicleSystem : MonoBehaviour 
    {
        public float baseEngineTorque;
        public AnimationCurve springCurve;
        public float forceMultiplier;
        public float maxDistance;
        public float accelerator;
        public float steering;
        public AnimationCurve tyreFriction;
        public float breaking;
        public Rigidbody rB;
        public Transform centreOfMass;
        public List<VehicleComponent> vehicleComponents;
        public float velocity;
        // Start is called before the first frame update
        void Start()
        {
            rB = GetComponent<Rigidbody>();
            rB.centerOfMass.Set(centreOfMass.localPosition.x,centreOfMass.localPosition.y,centreOfMass.localPosition.z);
        }
        // Update is called once per frame
        private void Update()
        {
            accelerator = Input.GetAxis("Vertical");
            steering = Input.GetAxis("Horizontal") * 35;
            breaking = Input.GetAxis("Jump");
            velocity = rB.velocity.magnitude;
        }

        void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space) )
            {
                if (IsUpsideDown())
                {
                  FlipCar();  
                }

                Debug.Log("up.y:" + transform.up.y);
            }
            
            foreach (VehicleComponent vehicleComponent in vehicleComponents)
            {
                vehicleComponent.Execute();
            }
            
            foreach ( Wheel wheel in GetComponentsInChildren<Wheel>())
            {
                if (wheel.steeringWheel)
                {
                    wheel.transform.localRotation = Quaternion.Euler(steering * (wheel.invertSteering?-1:1) + 90 ,-90,270);
                }

                if (wheel.driveWheel && wheel.isGrounded)
                { 
                    rB.AddForceAtPosition(wheel.transform.forward* accelerator* (baseEngineTorque/DriveWheels()),wheel.transform.position);
                }
                
            }
            
        }
        
        public int DriveWheels()
        {
            int i = 0;
            foreach (Wheel wheel in GetComponentsInChildren<Wheel>())
            {
                if (wheel.driveWheel)
                {
                    i++;
                }
            }
            return i;
        }

        private bool IsUpsideDown()
        {
            return transform.forward.y < -0.5;
        }

        private void FlipCar()
        { 
            rB.velocity = new Vector3(0,0,0); 
            rB.angularVelocity = new Vector3(0,0,0); 
            transform.position += new Vector3(0, 5, 0);
            transform.rotation = Quaternion.Euler(-90,0,0);
        }

    }

}



