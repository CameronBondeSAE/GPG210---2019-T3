using System;
using System.Collections.Generic;
using UnityEngine;

namespace Students.Luca.Scripts
{
    
    public class WheelMotor : InputReceiver/*, IIncDecreasable*/
    {
        public bool inputAccelerate = false;
        public bool inputInverseAccelerate = false;
        
        public List<Wheel> wheels;
        public float motorStrength=100;

        public bool autoResetAcceleration = false;
        
        public float acceleration = 0;

        private void Start()
        {
            if(wheels == null)
                wheels = new List<Wheel>();
        }

        private void Update()
        {/*
            if (inputAccelerate)
            {
                acceleration = Mathf.Clamp(acceleration+Time.deltaTime, -1, 1);
            }else if (inputInverseAccelerate)
            {
                acceleration = Mathf.Clamp(acceleration-Time.deltaTime, -1, 1);
            }else if (autoResetAcceleration)
            {
                acceleration = Mathf.MoveTowards(acceleration, 0, Time.deltaTime);
            }
            
            if (!Car.ApproximatelyT(acceleration,0,0.05f))
            {
                float wheelForce = (motorStrength / wheels.Count)*acceleration;
                foreach (var wheel in wheels)
                {
                    wheel.ApplyForce(wheelForce, Vector3.forward);
                }
            }

            inputAccelerate = false;
            inputInverseAccelerate = false;*/
        }

        /*public void IncreaseValue()
        {
            inputAccelerate = true;
        }

        public void DecreaseValue()
        {
            inputInverseAccelerate = true;
        }*/

        public override void LeftStickAxis(Vector2 value)
        {
            if (!useLSA)
                return;

            value = CalculateLSAValue(value);
            
            if (wheels != null)
            {
                foreach (var wheel in wheels)
                {
                    wheel.LeftStickAxis(value);
                }
            }

            if (value.y > 0)
            {
                acceleration = Mathf.Clamp(acceleration+Time.deltaTime, -1, 1);
            }else if (value.y < 0)
            {
                acceleration = Mathf.Clamp(acceleration-Time.deltaTime, -1, 1);
            }else if (autoResetAcceleration && !Mathf.Approximately(acceleration,0))
            {
                acceleration = Mathf.MoveTowards(acceleration, 0, Time.deltaTime);
            }
            
            // TODO this is allwazs 4x4.
            if (!Car.ApproximatelyT(acceleration,0,0.05f) && wheels != null)
            {
                float wheelForce = (motorStrength / wheels.Count)*acceleration;
                foreach (var wheel in wheels)
                {
                    wheel.ApplyForce(wheelForce, Vector3.forward);
                }
            }
        }

        public override void RightStickAxis(Vector2 value)
        {
            
            if (wheels != null)
            {
                foreach (var wheel in wheels)
                {
                    wheel.RightStickAxis(value);
                }
            }
        }
    }
}