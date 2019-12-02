using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Students.Blaide;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Students.Blaide
{
    public class VehicleSystem : Possessable
    {
        public float baseEngineTorque;
        public float accelerator;
        public float leftTriggerValue;
        public float rightTriggerValue;
        public float wheelSteering;
        public float reverse;
        
        public float pitchSteering;
        public float yawSteering;
        public float rollSteering;
        
        public Rigidbody rB;
        public Transform centreOfMass;
        public List<VehicleComponent> vehicleComponents;
        public float fuelDrainRate;
        public Fuel fuel;


        // Start is called before the first frame update
        void Start()
        {
            rB = GetComponent<Rigidbody>();
            fuel = GetComponent<Fuel>();
        }

        public override void Activate(Controller c)
        {
            base.Activate(c);
        }

        public override void Deactivate()
        {
            base.Deactivate();
            rightTriggerValue = 0;
            leftTriggerValue = 0;
            pitchSteering = 0;
            rollSteering = 0;
        }
        public override void LeftStickAxis(Vector2 value)
        {
            wheelSteering = value.x * 35;
        }

        public override void RightStickAxis(Vector2 value)
        {
            pitchSteering = value.y * 45;
            rollSteering = value.x * 45;
        }

        public override void RightTrigger(float value)
        {
            rightTriggerValue = value;
        }

        public override void LeftTrigger(float value)
        {
            leftTriggerValue = value;
        }

        // Update is called once per frame
        private void Update()
        {
            accelerator = rightTriggerValue;// - leftTriggerValue;
            reverse = leftTriggerValue;
            rB.centerOfMass = centreOfMass.localPosition;
            //accelerator = 0;
            
        }
        void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space) )
            {
                if (IsUpsideDown())
                {
                  FlipCar();  
                }
            }

            if (!fuel.OutOfFuel)
            {
               fuel.DrainFuel(accelerator*fuelDrainRate); 
            }



            foreach (VehicleComponent vehicleComponent in GetComponentsInChildren<VehicleComponent>())
            {
                vehicleComponent.Execute();
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



