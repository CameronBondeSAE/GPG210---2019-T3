using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace Students.Luca.Scripts
{
    public class Vehicle : Possessable
    {
        public float floorAngularDrag = 0.05f;
        public float flyAngularDrag = 10;
        
        public Rigidbody rb;
        public Transform centerOfMass;

        public float maxGroundedDistance = 4;
        public float currentDistanceToGround = 0;

        /*public List<IIncDecreasable> forwardEngines = null;
        public List<IIncDecreasable> turnLeftParts = null;
        public List<IIncDecreasable> turnRightParts = null;*/
        public List<IRotatable> turnableParts = null;

        public List<InputReceiver> inputReceivers;
        
        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();

            if (centerOfMass != null)
            {
                rb.centerOfMass = centerOfMass.localPosition;
            }
        }

        // Update is called once per frame
        void Update()
        {
            rb.angularDrag = IsGrounded() ? floorAngularDrag : flyAngularDrag;
            
            // TEMP HACK
            //LeftStickAxis(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        }

        public override void LeftStickAxis(Vector2 value)
        {
            if (inputReceivers != null)
            {
                foreach (var inputReceiver in inputReceivers)
                {
                    inputReceiver.LeftStickAxis(value);
                }
            }
            
            /*
            if(!Mathf.Approximately(value.y,0))
                InputMoveForward(value.y);
            if(!Mathf.Approximately(value.x,0))
                InputTurn(value.x);*/
        }
        
        /*private void InputMoveForward(float inputAxis)
        {
            if (forwardEngines != null)
            {
                foreach (var engine in forwardEngines)
                {
                    if (inputAxis > 0)
                        engine.IncreaseValue();
                    else if(inputAxis < 0)
                        engine.DecreaseValue();
                }
            }
        }
        
        private void InputTurn(float inputAxis)
        {

            if (turnLeftParts != null)
            {
                foreach (var part in turnLeftParts)
                {
                    if (inputAxis < 0)
                        part.IncreaseValue();
                    else if(inputAxis > 0)
                        part.DecreaseValue();
                }
            }
            if (turnRightParts != null)
            {
                foreach (var part in turnRightParts)
                {
                    if (inputAxis > 0)
                        part.IncreaseValue();
                    else if(inputAxis < 0)
                        part.DecreaseValue();
                }
            }
            
            if (turnableParts != null)
            {
                foreach (var part in turnableParts)
                {
                    if (inputAxis < 0)
                        part.TurnLeft();
                    else if(inputAxis > 0)
                        part.TurnRight();
                }
            }
        }*/

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