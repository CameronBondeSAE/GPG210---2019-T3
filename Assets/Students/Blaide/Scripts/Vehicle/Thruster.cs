using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  Students.Blaide
{
    public class Thruster : VehicleComponent
    {
        public float thrustInput;

        public float thrustMultiplier;

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
            thrustInput = vehicleSystem.accelerator;
            rB.AddForceAtPosition(this.transform.up *thrustInput * thrustMultiplier, transform.position);
            base.Execute();
        }
    }


}
