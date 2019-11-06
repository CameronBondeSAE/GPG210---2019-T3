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
        public bool aileronSteering;
        public Quaternion defaultRotation;
        public bool invertSteering;
        public enum SteeringControl
        {
            pitch,yaw,roll
        }
        public SteeringControl steeringControl;
        // Start is called before the first frame update
        void Start()
        {
            defaultRotation = this.transform.localRotation;
        }
        // Update is called once per frame
        void Update()
        {
            if (aileronSteering)
            {
                if (steeringControl == SteeringControl.pitch)
                {
                    transform.localRotation = Quaternion.Euler(vehicleSystem.pitchSteering * (invertSteering ? -1 : 1) + defaultRotation.eulerAngles.x, defaultRotation.eulerAngles.y, defaultRotation.eulerAngles.z);
                }
                else if (steeringControl == SteeringControl.yaw)
                {
                    transform.localRotation = Quaternion.Euler(vehicleSystem.yawSteering * (invertSteering ? -1 : 1) + defaultRotation.eulerAngles.x, defaultRotation.eulerAngles.y, defaultRotation.eulerAngles.z);
                }
                else if (steeringControl == SteeringControl.roll)
                {
                    transform.localRotation = Quaternion.Euler(vehicleSystem.rollSteering * (invertSteering ? -1 : 1) + defaultRotation.eulerAngles.x, defaultRotation.eulerAngles.y, defaultRotation.eulerAngles.z);
                }
            }
        }
        public override void Execute()
        {
            //base.Execute();
            localVelocity = transform.InverseTransformDirection(rB.GetPointVelocity(transform.position));
            rB.AddForceAtPosition (transform.TransformDirection(new Vector3(0,-localVelocity.y * airResistance,0)),transform.position);
            rB.AddForceAtPosition(transform.TransformDirection(new Vector3(0,localVelocity.z * liftCoefficient ,-localVelocity.z * (liftCoefficient/5))),transform.position);
        }
    }

}

