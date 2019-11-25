using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Students.Luca.Scripts
{
    public class Boat : Possessable
    {
        public List<BoatMotor> motors;
        public BuoyantBody buoyantBody;

        private Vector3 localFrontTipOfBoat = Vector3.zero;
        private Collider collider;
        private MeshFilter meshFilter;
        
        // Start is called before the first frame update
        void Start()
        {
            if (buoyantBody == null)
                buoyantBody = GetComponent<BuoyantBody>();
            
            if (motors == null)
            {
                motors = new List<BoatMotor>();
            }

            collider = GetComponent<Collider>();

            meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                localFrontTipOfBoat = transform.localPosition + meshFilter.mesh.bounds.extents;
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
            return transform.TransformPoint(localFrontTipOfBoat);
        }

        public float GetBoatLength()
        {
            return collider?.bounds.size.z ?? (meshFilter?.mesh.bounds.size.z ?? 0);
        }
    }
}
