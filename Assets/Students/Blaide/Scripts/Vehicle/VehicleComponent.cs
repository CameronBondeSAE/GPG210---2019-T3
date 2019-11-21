using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Students.Blaide
{
    public class VehicleComponent : MonoBehaviour
    {
        public Vector3 localVelocity;
        public VehicleSystem vehicleSystem;
        public Rigidbody rB;
        // Start is called before the first frame update
        void Start()
        {
            AssignToVehicle();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public virtual void Execute()
        {
        
        }
        public virtual void AssignToVehicle()
        {
            if (vehicleSystem == null)
            {
                if (GetComponentInParent<VehicleSystem>() != null)
                {
                    vehicleSystem = GetComponentInParent<VehicleSystem>();
                }
                else if (transform.parent.GetComponentInParent<VehicleSystem>() != null)
                {
                    vehicleSystem = transform.parent.GetComponentInParent<VehicleSystem>();
                }
                else if (transform.parent.transform.parent.GetComponentInParent<VehicleSystem>() != null)
                {
                    vehicleSystem = transform.parent.transform.parent.GetComponentInParent<VehicleSystem>();
                }
            }
            rB = vehicleSystem.rB;
        }
    }
}

