using System.Collections;
using System.Collections.Generic;
using Students.Luca;
using Students.Luca.Scripts.Archive;
using UnityEngine;
using UnityEngine.UIElements;

namespace Students.Luca
{
    public class Car : MonoBehaviour
    {
        public float floorAngularDrag = 0.05f;
        public float flyAngularDrag = 10;
        
        public Rigidbody rb;
        public Transform centerOfMass;
        
        [Header("Car Settings")]
        public List<Luca.Scripts.Archive.Wheel> steeringWheels;
        public List<Luca.Scripts.Archive.Wheel> driveWheels;

        public float motorStrength;
        
        public float acceleration = 0;
        public float steeringWheel = 0;

        public float maxGroundedDistance = 4;
        public float currentDistanceToGround = 0;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
        
            if(steeringWheels == null)
                steeringWheels = new List<Luca.Scripts.Archive.Wheel>();
            if(driveWheels == null)
                driveWheels = new List<Luca.Scripts.Archive.Wheel>();

            if (centerOfMass != null)
            {
                rb.centerOfMass = centerOfMass.localPosition;
            }
        }

        // Update is called once per frame
        void Update()
        {
            rb.angularDrag = IsGrounded() ? floorAngularDrag : flyAngularDrag;
            
            HandleInput();

            InformWheels();

            //DEBUG
            if (rb.velocity.magnitude > 0.1)
            {
                Debug.DrawRay(transform.position, rb.velocity, Color.red);
            }
        }

        private void InformWheels()
        {
            if (!ApproximatelyT(acceleration,0,0.05f))
            {
                float wheelForce = (motorStrength / driveWheels.Count)*acceleration;
                foreach (var wheel in driveWheels)
                {
                    wheel.ApplyForce(wheelForce, Vector3.forward);
                }
            }
        }

        private void HandleInput()
        {
            acceleration = Input.GetAxis("Vertical");
            steeringWheel = Input.GetAxis("Horizontal");
            /*

            // Forward
            if (Input.GetKey(KeyCode.W))
            {
                float wheelForce = motorStrength / driveWheels.Count;
                foreach (var wheel in driveWheels)
                {
                    wheel.ApplyForce(wheelForce);
                }
            }
            
            // Backward
            if (Input.GetKey(KeyCode.S))
            {
                float wheelForce = motorStrength / driveWheels.Count;
                foreach (var wheel in driveWheels)
                {
                    wheel.ApplyForce(-wheelForce);
                }
            }*/

            // Turn Left
            if (Input.GetKeyDown(KeyCode.A))
            {
                foreach (var wheel in steeringWheels)
                {
                    wheel.inputTurnLeft = true;
                }
            }

            if (Input.GetKeyUp(KeyCode.A))
            {
                foreach (var wheel in steeringWheels)
                {
                    wheel.inputTurnLeft = false;
                }
            }
        
            // Turn Right
            if (Input.GetKeyDown(KeyCode.D))
            {
                foreach (var wheel in steeringWheels)
                {
                    wheel.inputTurnRight = true;
                }
            }

            if (Input.GetKeyUp(KeyCode.D))
            {
                foreach (var wheel in steeringWheels)
                {
                    wheel.inputTurnRight = false;
                }
            }
        }

        public static bool ApproximatelyT(float a, float b, float threshold)
        {
            return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
        }

        protected virtual bool IsGrounded()
        {
            RaycastHit hit;
            Debug.DrawRay(transform.position, currentDistanceToGround*/*-transform.up*/-Vector3.up, Color.blue);
            if (Physics.Raycast(transform.position, /*-transform.up*/-Vector3.up, out hit, 100)) // TODO do raycast from bottom/exhaust
            {
                currentDistanceToGround = hit.distance;
            }
            else
            {
                currentDistanceToGround = float.PositiveInfinity; //zeroForceHeight;
            }
            
            return currentDistanceToGround <= maxGroundedDistance;
        }
    }

}

