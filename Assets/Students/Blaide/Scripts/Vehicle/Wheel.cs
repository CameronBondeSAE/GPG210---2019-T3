using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Students.Blaide
{
    public class Wheel : VehicleComponent
    {
        public GameObject wheelModel;
        public float wheelModelHeightOffset = 0.50f;
        public bool driveWheel;
        public bool steeringWheel;
        public bool invertSteering;
        
        public float suspensionHeight;
        public float springDampening;
        public AnimationCurve springCurve;
        public AnimationCurve frictionCurve;
        public float springStiffness;
        public float springHeightLast;
        public Vector3 suspensionAngle;
        public float reverse;
        public float steering;
        public float accelerator;
        public float wheelAcceleration;
        public float braking;
        public bool isGrounded;
        public float breakPercent;
        public Quaternion defaultRotation;
        // Start is called before the first frame update
        void Start()
        {
            defaultRotation = transform.localRotation;
        }
        // Update is called once per frame
        public override void Execute()
        {
            reverse = vehicleSystem.Reverse;
            steering = vehicleSystem.wheelSteering;
            accelerator = vehicleSystem.accelerator;
            //if (reverse >= 0.01f && accelerator >= 0.01f )
            
                if(reverse > accelerator)
                {
                    braking = accelerator;
                    wheelAcceleration = -reverse;
                }
                else 
                {
                    braking = reverse;
                    wheelAcceleration = accelerator;
                }
            

            if (steeringWheel)
            {
                transform.localRotation = defaultRotation* Quaternion.AngleAxis(steering,Vector3.up);
            }
            
            RaycastHit hit;
            Physics.Raycast(transform.position,  transform.TransformDirection(Quaternion.Euler(suspensionAngle)* Vector3.down), out hit,suspensionHeight);
            isGrounded = (hit.collider != null);
            if (isGrounded)//(hit.collider != null && hit.collider.gameObject != carMainSystem.gameObject)
            { 
                //Suspension
                Vector3 springVelocity = new Vector3(0,hit.distance - springHeightLast,0);
                
                rB.AddForceAtPosition(-transform.TransformDirection(Quaternion.Euler(suspensionAngle)* Vector3.down) * springCurve.Evaluate(hit.distance/suspensionHeight) * springStiffness, transform.position);

                //dampening
                rB.AddForceAtPosition(transform.up *springVelocity.y * springDampening,transform.position);
                springHeightLast = hit.distance;

                wheelModel.transform.position = hit.point + Vector3.up * wheelModelHeightOffset;
                //asymmetric friction
                // Vector3 localVelocity = wheel.transform.InverseTransformDirection(rB.velocity);
               
                // localVelocity = transform.InverseTransformDirection(transform.position - lastPosition)/ Time.deltaTime;
                // rB.AddForceAtPosition (transform.TransformDirection(new Vector3(-localVelocity.x *frictionCurve.Evaluate(Mathf.Abs(localVelocity.x)),0,-localVelocity.z *frictionCurve.Evaluate(Mathf.Abs(localVelocity.z)) * breaking *(breakPercent/100)))  * rB.mass ,transform.position);

                localVelocity = transform.InverseTransformDirection(rB.GetPointVelocity(wheelModel.transform.position));
                rB.AddForceAtPosition ( transform.TransformDirection(new Vector3(-localVelocity.x *frictionCurve.Evaluate(Mathf.Abs(localVelocity.x )*0.01f),0,-localVelocity.z *frictionCurve.Evaluate(Mathf.Abs(localVelocity.z)*0.01f) * braking *(breakPercent/100)))  * rB.mass ,wheelModel.transform.position);

                
               
                

                
                if (driveWheel && isGrounded)
                {
                    //rB.AddForceAtPosition(transform.forward* accelerator* (vehicleSystem.baseEngineTorque/vehicleSystem.DriveWheels()) * (rB.mass / 3),wheelModel.transform.position);
                    
                    rB.AddForceAtPosition(Vector3.ProjectOnPlane(transform.forward ,hit.normal) * wheelAcceleration* (vehicleSystem.baseEngineTorque/vehicleSystem.DriveWheels()) * (rB.mass / 3),transform.position);
                }
                
            }
            else
            {
                wheelModel.transform.position = transform.position + transform.up * -(suspensionHeight - wheelModelHeightOffset);

            }

        }

    }

}

