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
        
        
        public float accelerator;
        public float steering;
        public float breaking;
        public float aileronSteering;
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
            aileronSteering = Input.GetAxis("Vertical") * 35;
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
                    wheel.transform.localRotation = Quaternion.Euler(steering * (wheel.invertSteering?-1:1) + wheel.defaultRotation.eulerAngles.x ,wheel.defaultRotation.eulerAngles.y,wheel.defaultRotation.eulerAngles.z);
                }

                if (wheel.driveWheel && wheel.isGrounded)
                { 
                    rB.AddForceAtPosition(wheel.transform.forward* accelerator* (baseEngineTorque/DriveWheels()),wheel.transform.position);
                }
                
            }

            foreach (Wing wing in GetComponentsInChildren<Wing>())
            {
                if (wing.Steering)
                {
                    wing.transform.localRotation = Quaternion.Euler(steering * (wing.invertSteering?-1:1) + wing.defaultRotation.eulerAngles.x ,wing.defaultRotation.eulerAngles.y,wing.defaultRotation.eulerAngles.z);
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



