using System.Collections.Generic;
using Students.Luca.Scripts.AI;
using UnityEngine;

namespace Students.Luca.Scripts.Vehicles
{
    public class Boat : Possessable
    {
        public List<BoatMotor> motors;
        public BuoyantBody buoyantBody;
        public Fuel fuel;
        public FuelDrainer fuelDrainer;

        private Vector3 _localFrontTipOfBoat = Vector3.zero;
        private Collider col;
        private MeshFilter _meshFilter;
        
        // Fast Hacky way to invert input for rotations
        public float rotationInputMultiplierX = 1;
        public float rotationInputMultiplierY = 1;
        
        private void Start()
        {
            if (buoyantBody == null)
                buoyantBody = GetComponent<BuoyantBody>();
            
            if (fuel == null)
                fuel = GetComponent<Fuel>();

            if (fuelDrainer == null)
                fuelDrainer = GetComponent<FuelDrainer>();
            
            if (motors == null)
                motors = new List<BoatMotor>();


            col = GetComponent<Collider>();

            _meshFilter = GetComponent<MeshFilter>();
            if (_meshFilter != null)
                _localFrontTipOfBoat = transform.localPosition + _meshFilter.mesh.bounds.extents;
        }

        public override void Activate(Controller c)
        {
            base.Activate(c);

            // TODO Temporary Hack
            var bd = GetComponent<AIBoatDriver>();
            if(bd != null)
                bd.enabled = false;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            
            // TODO Temporary Hack
            var bd = GetComponent<AIBoatDriver>();
            if(bd != null)
                bd.enabled = true;
        }

        public override void RightTrigger(float value)
        {
            if (motors == null || motors.Count <= 0) return;
            foreach (var motor in motors)
            {
                motor.CurrentDesiredSpeed = value;
            }

            // Fuel Hack; FuelDrainer should be on each motor.
            if (fuel == null || fuelDrainer == null) return;
            fuel.DrainFuel(value * fuelDrainer.fuelDrainPerFs);
        }
        
        public override void LeftTrigger(float value)
        {
            if (motors == null || motors.Count <= 0) return;
            foreach (var motor in motors)
            {
                motor.CurrentDesiredSpeed = -value;
            }
            
            // Fuel Hack; FuelDrainer should be on each motor.
            if (fuel == null || fuelDrainer == null) return;
            fuel.DrainFuel(value * fuelDrainer.fuelDrainPerFs);
        }


        public override void LeftStickAxis(Vector2 value)
        {
            if (motors != null && motors.Count > 0)
            {
                foreach (var motor in motors)
                {
                    var newDesiredRotation = motor.CurrentDesiredRotation;
                    newDesiredRotation.y = value.x * rotationInputMultiplierX;
                    motor.CurrentDesiredRotation = newDesiredRotation;
                }
            }
        }
        
        public override void RightStickAxis(Vector2 value)
        {
            if (motors == null || motors.Count <= 0) return;
            foreach (var motor in motors)
            {
                var newDesiredRotation = motor.CurrentDesiredRotation;
                newDesiredRotation.x = value.y * rotationInputMultiplierY;
                motor.CurrentDesiredRotation = newDesiredRotation;
            }
        }

        public bool IsInWater()
        {
            return buoyantBody?.IsInWater() ?? false;
        }

        public bool IsUnderWater()
        { 
            return buoyantBody?.IsUnderWater() ?? false;
        }

        public Vector3 GetFrontTipOfBoat()
        {
            return transform.TransformPoint(_localFrontTipOfBoat);
        }

        public float GetBoatLength()
        {
            return col?.bounds.size.z ?? (_meshFilter?.mesh.bounds.size.z ?? 0);
        }
    }
}
