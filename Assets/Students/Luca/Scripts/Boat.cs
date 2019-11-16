using System.Collections.Generic;
using UnityEngine;

namespace Students.Luca.Scripts
{
    public class Boat : Possessable
    {
        public List<BoatMotor> motors;
        
        // Start is called before the first frame update
        void Start()
        {
            
            if (motors == null)
            {
                motors = new List<BoatMotor>();
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        
        public override void LeftStickAxis(Vector2 value)
        {
            if (motors != null && motors.Count > 0)
            {
                foreach (var motor in motors)
                {
                    motor.CurrentDesiredSpeed = value.y;
                    
                    Vector3 newDesiredRotation = motor.CurrentDesiredRotation;
                    newDesiredRotation.y = value.x;
                    motor.CurrentDesiredRotation = newDesiredRotation;
                }
            }
        }
        
        public override void RightStickAxis(Vector2 value)
        {
            if (motors != null && motors.Count > 0)
            {
                foreach (var motor in motors)
                {
                    Vector3 newDesiredRotation = motor.CurrentDesiredRotation;
                    newDesiredRotation.x = value.y;
                    motor.CurrentDesiredRotation = newDesiredRotation;
                }
            }
        }
    }
}
