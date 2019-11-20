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
        public KeyCode pitchUp;
        public KeyCode pitchDown;
        public KeyCode yawLeft;
        public KeyCode yawRight;
        public KeyCode rollLeft;
        public KeyCode rollRight;
        public KeyCode forward;
        public KeyCode reverse;
        
        public float baseEngineTorque;
        public float accelerator;
        public float wheelSteering;
        public float Reverse;
        
        public float pitchSteering;
        public float yawSteering;
        public float rollSteering;
        
        public Rigidbody rB;
        public Transform centreOfMass;
        public List<VehicleComponent> vehicleComponents;
        
        public float velocity;
        
        public bool DebugKeyboardControls;
        
        // Start is called before the first frame update
        void Start()
        {
            rB = GetComponent<Rigidbody>();
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
            accelerator = value;
        }

        public override void LeftTrigger(float value)
        {
            
        }

        // Update is called once per frame
        private void Update()
        {
            rB.centerOfMass = centreOfMass.localPosition;
            velocity = rB.velocity.magnitude;

            if (DebugKeyboardControls)
            {
                accelerator = Input.GetAxis("Vertical");
                wheelSteering = Input.GetAxis("Horizontal") * 35;
                pitchSteering = InputToAxis(pitchUp, pitchDown, pitchSteering);
                yawSteering = InputToAxis(yawLeft, yawRight, yawSteering);
                rollSteering = InputToAxis(rollLeft, rollRight, rollSteering);
            }
        }
        float InputToAxis(KeyCode upKey,KeyCode downKey, float axis)
        {
            if (Input.GetKey(upKey) && axis <= 45)
            {
                axis += 1;
            }
            else if (Input.GetKey(downKey) && axis >= -45)
            {
                axis -= 1;
            }
            else
            {
                if (axis > 1)
                {
                    axis -= 1;
                }
                else if (axis < -1)
                {
                    axis += 1;
                }
                else
                {
                    axis = 0;
                }
            }

            return axis;
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



