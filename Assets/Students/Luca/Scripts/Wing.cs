using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Students.Luca.Scripts
{
    public class Wing : MonoBehaviour
    {
        public Rigidbody masterRb;

        public KeyCode forwardRotateKey;
        public KeyCode backwardRotateKey;
        public float maxXRotAngle = 30;
        public float rotationSpeed = 30;

        public float airForce = 10;

        public float wingSizeMultiplier = 1;

        private Quaternion defaultRotation;
        private Quaternion maxForwardRotation; // Up
        private Quaternion maxBackwardRotation; // Down

        // Start is called before the first frame update
        void Start()
        {
            defaultRotation = transform.localRotation;
            //Hack
            Vector3 maxRotationEuler = transform.localRotation.eulerAngles;
            maxRotationEuler.x += maxXRotAngle;
            maxForwardRotation = Quaternion.Euler(maxRotationEuler);

            maxRotationEuler = transform.localRotation.eulerAngles;
            maxRotationEuler.x -= maxXRotAngle;
            maxBackwardRotation = Quaternion.Euler(maxRotationEuler);


            if (masterRb == null)
            {
                masterRb = GetComponentInParent<Rigidbody>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(forwardRotateKey))
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, maxForwardRotation,
                    rotationSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(backwardRotateKey))
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, maxBackwardRotation,
                    rotationSpeed * Time.deltaTime);
            }
            else if (!Mathf.Approximately(transform.localRotation.eulerAngles.x, defaultRotation.eulerAngles.x))
            {
                transform.localRotation =
                    Quaternion.RotateTowards(transform.localRotation, defaultRotation, rotationSpeed * Time.deltaTime);
            }

            ApplyAirForces();

        }

        public float angleToVelocity = 0;
        public float angleForceToGround = 0;
        public float angleToVelocityMultiplier = 0;

        private void ApplyAirForces()
        {
            Vector3 localVelocity = transform.InverseTransformDirection(masterRb.velocity);

            //localVelocity.x = 0;
            //localVelocity.z = 0;

            angleToVelocity = Vector3.Angle(Vector3.forward, localVelocity);
            angleToVelocityMultiplier = Mathf.Sin(angleToVelocity * (Mathf.PI / 180)) + 0.3f;
            Debug.DrawRay(transform.position,
                masterRb.transform.TransformDirection(localVelocity * angleToVelocityMultiplier * airForce *
                                                      wingSizeMultiplier), Color.green);
            Debug.DrawRay(transform.position,
                masterRb.transform.TransformDirection(-localVelocity * angleToVelocityMultiplier * airForce *
                                                      wingSizeMultiplier), Color.white);

            Vector3 finalForce =
                masterRb.transform.TransformDirection(-localVelocity * angleToVelocityMultiplier * airForce *
                                                      wingSizeMultiplier);

            // Slow down force depending on the angle towards the ground
            angleForceToGround = Vector3.Angle(Vector3.forward, finalForce);
            finalForce *= Mathf.Cos(angleForceToGround);

            masterRb.AddForceAtPosition(finalForce, transform.position);
        }
    }
}
