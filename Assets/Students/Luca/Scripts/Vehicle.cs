using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<IRotatable> turnableParts;

        public List<InputReceiver> inputReceivers;

        public Fuel fuel;
        private bool outOfFuel = false;
        
        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();

            if (centerOfMass != null)
            {
                rb.centerOfMass = centerOfMass.localPosition;
            }

            if (fuel == null)
                fuel = GetComponent<Fuel>();

            if (fuel != null)
            {
                fuel.OnOutOfFuel += HandleOutOfFuelEvent;
                outOfFuel = fuel.OutOfFuel;
            }
        }

        // Update is called once per frame
        void Update()
        {
            rb.angularDrag = IsGrounded() ? floorAngularDrag : flyAngularDrag; // hacky
        }

        private void OnDestroy()
        {
            if (fuel != null)
                fuel.OnOutOfFuel -= HandleOutOfFuelEvent;
        }

        private void HandleOutOfFuelEvent()
        {
            outOfFuel = true;
        }

        public override void LeftStickAxis(Vector2 value)
        {
            if (inputReceivers == null) return;
            
            if(outOfFuel) value = Vector2.zero;
            
            foreach (var inputReceiver in inputReceivers)
            {
                inputReceiver.LeftStickAxis(value);
                var fd = inputReceiver.GetComponent<FuelDrainer>();
                if (fd != null)
                {
                    fuel.DrainFuel(fd.fuelDrainPerFs * inputReceiver.GetCurrentForceSecondValue());
                }
            }
        }

        public override void RightStickAxis(Vector2 value)
        {
            if (inputReceivers == null) return;
            
            if(outOfFuel) value = Vector2.zero;
            
            foreach (var inputReceiver in inputReceivers)
            {
                inputReceiver.RightStickAxis(value);
                var fd = inputReceiver.GetComponent<FuelDrainer>();
                if (fd != null)
                {
                    fuel.DrainFuel(fd.fuelDrainPerFs * inputReceiver.GetCurrentForceSecondValue());
                }
            }
        }

        public override void LeftTrigger(float value)
        {
            if (inputReceivers == null) return;
            
            if(outOfFuel) value = 0;
            
            foreach (var inputReceiver in inputReceivers)
            {
                inputReceiver.LeftTrigger(value);
                var fd = inputReceiver.GetComponent<FuelDrainer>();
                if (fd != null)
                {
                    fuel.DrainFuel(fd.fuelDrainPerFs * inputReceiver.GetCurrentForceSecondValue());
                }
            }
        }

        public override void RightTrigger(float value)
        {
            if (inputReceivers == null) return;
            
            if(outOfFuel) value = 0;
            
            foreach (var inputReceiver in inputReceivers)
            {
                inputReceiver.RightTrigger(value);
                var fd = inputReceiver.GetComponent<FuelDrainer>();
                if (fd != null)
                {
                    fuel.DrainFuel(fd.fuelDrainPerFs * inputReceiver.GetCurrentForceSecondValue());
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
        
        
        
        public override void Activate(Controller c)
        {
            base.Activate(c);
            if (inputReceivers == null) return;
            foreach (var t in inputReceivers.Select(inputReceiver => inputReceiver.GetComponent<NoFuelThruster>()).Where(t => t != null))
            {
                t.TurnedOn = true;
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
            if (inputReceivers == null) return;
            foreach (var t in inputReceivers.Select(inputReceiver => inputReceiver.GetComponent<NoFuelThruster>()).Where(t => t != null))
            {
                t.TurnedOn = false;
            }
        }
    }
}