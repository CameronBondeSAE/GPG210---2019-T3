using System.Collections;
using System.Collections.Generic;
using Students.Blaide;
using UnityEngine;

namespace Students.Blaide
{
    public class Wing : VehicleComponent
    {
        public float liftCoefficient;
        public float airResistance;
        public Vector3 lastPosition;
    
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public override void Execute()
        {
            //base.Execute();
            Vector3 localVelocity = transform.InverseTransformDirection(transform.position - lastPosition)/ Time.deltaTime;
               
            rB.AddForceAtPosition (transform.TransformDirection(new Vector3(0,-localVelocity.y * airResistance,0)),transform.position);
        
       
            rB.AddForceAtPosition(transform.TransformDirection(new Vector3(0,localVelocity.z * liftCoefficient ,-localVelocity.z * (liftCoefficient/5))),transform.position);
            lastPosition = transform.position;
        }
    }

}

