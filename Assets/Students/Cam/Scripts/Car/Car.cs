using System;
using UnityEngine;
using System.Collections;

namespace Cam
{
	public class Car : Possessable
	{
		public Wheel[] steeringWheels;
		public Wheel[] drivingWheels;

		public float accelerationForce;

		//	public Wheel turningWheelLeft;
		//	public Wheel turningWheelRight;
		//	public Wheel drivingWheelBackLeft;
		//	public Wheel drivingWheelBackRight;
		public float driving;
		public float steering;

		// Update is called once per frame
		void Update()
		{
			foreach (Wheel steeringWheel in steeringWheels)
			{
				steeringWheel.transform.localRotation = Quaternion.Euler(0, steering, 0);
			}


			foreach (Wheel drivingWheel in drivingWheels)
			{
				drivingWheel.LongditudinalForce(driving * accelerationForce);
			}


			//		print(steering);
			//
			//		JointLimits jl = new JointLimits();
			//		jl.min = steering;
			//		jl.max = steering+1;
			//		turningWheelLeft.GetComponent<HingeJoint>().limits = jl;
			//		turningWheelRight.GetComponent<HingeJoint>().limits = jl;

			//		turningWheelLeft.transform.localEulerAngles = new Vector3(0, 0, steering);
			//		turningWheelLeft.transform.RotateAround(Vector3.zero, Vector3.up, steering);
			//		turningWheelRight.transform.Rotate(new Vector3(0, steering, 0), Space.World);

		}

		public override void LeftStickAxis(Vector2 value)
		{
            base.LeftStickAxis(value);
            
			steering = value.x * 30;
		}

        public override void RightStickAxis(Vector2 value)
        {
            base.RightStickAxis(value);
        }

        public override void LeftTrigger(float value)
        {
            base.LeftTrigger(value);
        }

        public override void RightTrigger(float value)
        {
            base.RightTrigger(value);
            
            driving = value;
        }

        public override void Activate(Controller c)
        {
            base.Activate(c);
        }
    }

}