using System.Collections;
using System.Collections.Generic;
using Students.Luca.Scripts.Archive;
using UnityEngine;

namespace Students.Luca.Scripts.Archive
{
    public class Vehicle : MonoBehaviour
    {
        public Rigidbody rb;

        public Thruster wheelFrontRight;
        public Thruster wheelFrontLeft;
        public Thruster wheelBackRight;
        public Thruster wheelBackLeft;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();

            wheelFrontRight.master = this;
            wheelFrontLeft.master = this;
            wheelBackRight.master = this;
            wheelBackLeft.master = this;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                wheelFrontLeft.inputTiltRight = true;
                wheelBackLeft.inputTiltRight = true;
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                wheelFrontLeft.inputTiltRight = false;
                wheelBackLeft.inputTiltRight = false;
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                wheelFrontRight.inputTiltLeft = true;
                wheelBackRight.inputTiltLeft = true;
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                wheelFrontRight.inputTiltLeft = false;
                wheelBackRight.inputTiltLeft = false;
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                wheelFrontRight.inputTiltForward = true;
                wheelFrontLeft.inputTiltForward = true;
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                wheelFrontRight.inputTiltForward = false;
                wheelFrontLeft.inputTiltForward = false;
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                wheelBackRight.inputTiltBackward = true;
                wheelBackLeft.inputTiltBackward = true;
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                wheelBackRight.inputTiltBackward = false;
                wheelBackLeft.inputTiltBackward = false;
            }

            /*if (Input.GetKey(KeyCode.W))
            {
                float angleFR = wheelFrontRight.transform.localRotation.eulerAngles.x + 30*Time.deltaTime;
                if(angleFR < 15)
                    wheelFrontRight.transform.Rotate( transform.TransformDirection(Vector3.right), 30*Time.deltaTime);
                
                float angleFL = wheelFrontLeft.transform.localRotation.eulerAngles.x + 30*Time.deltaTime;
                if(angleFL < 15)
                    wheelFrontLeft.transform.Rotate(transform.TransformDirection(Vector3.right), 30*Time.deltaTime);
                
                float angleBR = wheelBackRight.transform.localRotation.eulerAngles.x + 30*Time.deltaTime;
                if(angleBR < 15)
                    wheelBackRight.transform.Rotate( transform.TransformDirection(Vector3.right), 30*Time.deltaTime);
                
                float angleBL = wheelBackLeft.transform.localRotation.eulerAngles.x + 30*Time.deltaTime;
                if(angleBL < 15)
                    wheelBackLeft.transform.Rotate(transform.TransformDirection(Vector3.right), 30*Time.deltaTime);
            }
            else
            {
                float angleFR = wheelFrontRight.transform.localRotation.eulerAngles.x - 90*Time.deltaTime;
                if(angleFR > 0)
                    wheelFrontRight.transform.Rotate(transform.TransformDirection(Vector3.right), -90*Time.deltaTime);
                float angleFL = wheelFrontLeft.transform.localRotation.eulerAngles.x - 90*Time.deltaTime;
                if(angleFL > 0)
                    wheelFrontLeft.transform.Rotate(transform.TransformDirection(Vector3.right), -90*Time.deltaTime);
                float angleBR = wheelBackRight.transform.localRotation.eulerAngles.x - 90*Time.deltaTime;
                if(angleBR > 0)
                    wheelBackRight.transform.Rotate(transform.TransformDirection(Vector3.right), -90*Time.deltaTime);
                float angleBL = wheelBackLeft.transform.localRotation.eulerAngles.x - 90*Time.deltaTime;
                if(angleBL > 0)
                    wheelBackLeft.transform.Rotate(transform.TransformDirection(Vector3.right), -90*Time.deltaTime);
            }*/
        }

        public void AddForce(Vector3 offset, Vector3 force)
        {
            Debug.DrawRay(transform.position + offset, -force, Color.red);
            rb.AddForceAtPosition(force, transform.position + offset);
        }

    }
}
