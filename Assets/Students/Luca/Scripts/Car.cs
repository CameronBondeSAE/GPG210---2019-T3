using System.Collections;
using System.Collections.Generic;
using Students.Luca;
using UnityEngine;
using UnityEngine.UIElements;

namespace Students.Luca
{
    public class Car : MonoBehaviour
    {
        public Rigidbody rb;
        
        [Header("Car Settings")]
        public List<Wheel> steeringWheels;
        public List<Wheel> driveWheels;

        public float motorStrength;
        
        [Header("Float Settings")]
        public float currentDistanceToGround = 0;
        public float zeroForceHeight = 3;
        public AnimationCurve forceHeightCurve;
        public Vector3 maxForce;
    
        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
        
            if(steeringWheels == null)
                steeringWheels = new List<Wheel>();
            if(driveWheels == null)
                driveWheels = new List<Wheel>();
        }

        // Update is called once per frame
        void Update()
        {
            //HandleFloating();
            HandleInput();
        }

        private void HandleFloating()
        {
            RaycastHit hit;
            Debug.DrawRay(transform.position, -currentDistanceToGround * transform.up, Color.blue);
            if (Physics.Raycast(transform.position, -transform.up, out hit, zeroForceHeight))
            {
                currentDistanceToGround = hit.distance;
            }
            else
            {
                currentDistanceToGround = zeroForceHeight;
            }
            float curveValue = Mathf.Clamp(currentDistanceToGround, 0, zeroForceHeight) / zeroForceHeight;
            Vector3 finalForce = transform.TransformDirection(forceHeightCurve.Evaluate(curveValue) * maxForce);
        
            rb.AddForceAtPosition(finalForce, transform.position);
        }

        private void HandleInput()
        {
            // Forward
            if (Input.GetKey(KeyCode.W))
            {
                float wheelForce = motorStrength / driveWheels.Count;
                foreach (var wheel in driveWheels)
                {
                    wheel.ApplyForce(wheelForce);
                    /*
                    // HACK TEST
                    Vector3 dir = wheel.transform.forward;
                    dir.y = 0;
                    rb.AddForceAtPosition(wheelForce * dir, wheel.transform.position);*/
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
            }

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
    }

}

