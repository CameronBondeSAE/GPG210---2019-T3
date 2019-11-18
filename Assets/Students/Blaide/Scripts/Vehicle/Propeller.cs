using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Students.Blaide
{
    public class Propeller : VehicleComponent
    {
        public float thrustInput;

        public float thrustMultiplier;
        public float propSpeed;

        // Start is called before the first frame update
        void Start()
        {
            AssignToVehicle();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void Execute()
        {
            thrustInput = vehicleSystem.accelerator;
            rB.AddForceAtPosition(vehicleSystem.transform.up * thrustInput * thrustMultiplier, transform.position);
            transform.Rotate(new Vector3(0,0,1) * thrustInput * propSpeed,Space.Self);
            //transform.Rotate()
            base.Execute();
        }
    }
}
