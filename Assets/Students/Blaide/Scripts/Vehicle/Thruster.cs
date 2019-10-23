using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  Students.Blaide
{
    public class Thruster : VehicleComponent
    {
        public float thrustInput;

        private float accelerator;
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
            rB.AddForceAtPosition(this.transform.forward *thrustInput, transform.position);
            base.Execute();
        }
    }


}
