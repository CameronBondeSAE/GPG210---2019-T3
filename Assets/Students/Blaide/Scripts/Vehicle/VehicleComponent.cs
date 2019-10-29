using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Students.Blaide
{
    public class VehicleComponent : MonoBehaviour
    {
        public VehicleSystem vehicleSystem;
        public Vector3 lastPosition;
        public Rigidbody rB;
        // Start is called before the first frame update
        void Start()
        {
            rB = vehicleSystem.rB;
            lastPosition = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public virtual void Execute()
        {
        
        }
    } 

}

