using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Students.Blaide
{
    public class Wheel : VehicleComponent
    {
        public GameObject wheelModel;
        public bool driveWheel;
        public bool steeringWheel;
        public bool invertSteering; 
        public Vector3 LastPosition;
        public float suspensionHeight;
        public AnimationCurve springCurve;
        public AnimationCurve frictionCurve;
        public float springMultiplier;
        public float breaking;
        public bool isGrounded;
        public float breakPercent;
        // Start is called before the first frame update
        void Start()
        {
        }
        // Update is called once per frame
        public override void Execute()
        {
            breaking = vehicleSystem.breaking;
        
            RaycastHit hit;
            Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit,suspensionHeight);
            isGrounded = (hit.collider != null);
            if (isGrounded)//(hit.collider != null && hit.collider.gameObject != carMainSystem.gameObject)
            { 
                //Suspension
                rB.AddForceAtPosition(transform.up * springMultiplier * springCurve.Evaluate(hit.distance), transform.position);
                wheelModel.transform.position = hit.point + Vector3.up *0.4f;
                //asymmetric friction
                // Vector3 localVelocity = wheel.transform.InverseTransformDirection(rB.velocity);
               
                Vector3 localVelocity = transform.InverseTransformDirection(transform.position - LastPosition)/ Time.deltaTime;
               
                rB.AddForceAtPosition (transform.TransformDirection(new Vector3(-localVelocity.x *frictionCurve.Evaluate(Mathf.Abs(localVelocity.x)),0,-localVelocity.z *frictionCurve.Evaluate(Mathf.Abs(localVelocity.z)) * breaking *(breakPercent/100)))  * 0.8f ,transform.position);
            }
            else
            {
                wheelModel.transform.position = transform.position + transform.up * -(suspensionHeight - 0.4f);

            }
            LastPosition = transform.position;

        }
    
    }

}

