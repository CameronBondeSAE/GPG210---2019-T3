﻿using System.Collections;
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
            rB = vehicleSystem.rB;
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

