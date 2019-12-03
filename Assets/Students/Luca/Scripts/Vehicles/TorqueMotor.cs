using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


namespace Students.Luca.Scripts
{
    public class TorqueMotor : InputReceiver
    {
        public Vector3 rotationAxisMaxTorque;

        public Rigidbody rb;
        
        [ShowInInspector]
        private float inputMultiplier = 0;

        public float InputMultiplier
        {
            get => inputMultiplier;
            set => inputMultiplier = Mathf.Clamp(value,-1,1);
        }

        // Start is called before the first frame update
        void Start()
        {
            if (rb == null)
                rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            rb.maxAngularVelocity = rotationAxisMaxTorque.magnitude; // Hack
            
            if (!Mathf.Approximately(InputMultiplier, 0))
            {
                rb.AddRelativeTorque(InputMultiplier * rotationAxisMaxTorque);
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }


        public override void LeftStickAxis(Vector2 value)
        {
            if (!useLSA)
                return;

            value = CalculateLSAValue(value);

            if (!Mathf.Approximately(LSA_X_ValueMultiplier,0))
            {
                //InputMultiplier += value.x * (Time.deltaTime/3); // Hacky
                InputMultiplier = Mathf.MoveTowards(InputMultiplier, Mathf.Sign(value.x),(Time.deltaTime/3)); // Hacky
            }
            if (!Mathf.Approximately(LSA_Y_ValueMultiplier,0))
            {
                //InputMultiplier += value.y * (Time.deltaTime/3); // Hacky
                InputMultiplier = Mathf.MoveTowards(InputMultiplier, Mathf.Sign(value.y),(Time.deltaTime/3)); // Hacky
            }
        }

        public override void RightStickAxis(Vector2 value)
        {
            if (!useRSA)
                return;

            value = CalculateRSAValue(value);

            if (Mathf.Abs(RSA_X_ValueMultiplier) > 0)
            {
                //InputMultiplier += value.x * (Time.deltaTime/3); // Hacky
                InputMultiplier = Mathf.MoveTowards(InputMultiplier, Mathf.Sign(value.x),(Time.deltaTime/3)); // Hacky
            }
            if (Mathf.Abs(RSA_Y_ValueMultiplier) > 0)
            {
                //InputMultiplier += value.y * (Time.deltaTime/3); // Hacky
                InputMultiplier = Mathf.MoveTowards(InputMultiplier, Mathf.Sign(value.y),(Time.deltaTime/3)); // Hacky
            }
        }

        public override void LeftTrigger(float value)
        {
            
        }

        public override void RightTrigger(float value)
        {
            
        }

        public override void Stop()
        {
            InputMultiplier = 0;
        }

        public override float GetCurrentForceSecondValue()
        {
            return InputMultiplier * rotationAxisMaxTorque.magnitude;
        }
    }
}