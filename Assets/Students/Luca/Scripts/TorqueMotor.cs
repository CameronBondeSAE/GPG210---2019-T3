using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


namespace Students.Luca
{
    public class TorqueMotor : MonoBehaviour
    {
        public KeyCode increaseKey;
        public KeyCode decreaseKey;
        
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
            if (Input.GetKey(increaseKey))
            {
                InputMultiplier += Time.deltaTime/2;
            }else if (Input.GetKey(decreaseKey))
            {
                InputMultiplier -= Time.deltaTime/2;
            }
        }
    }
}
