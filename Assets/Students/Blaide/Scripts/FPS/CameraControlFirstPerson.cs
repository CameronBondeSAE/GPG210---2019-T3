﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Students.Blaide
{
    public class CameraControlFirstPerson : MonoBehaviour
    {
        public GameObject cameraGameObj;
        public float inputFilter;
        public float rotationSpeed;
        public bool invertYAxis;
        private float mouseX, mouseY;
        private PlayerMovement playerMovement;
        private float deltaX, deltaY;
        private Vector3 camAngle;
        private Vector3 targetAngle;
        private Vector3 startAngle;

        private Transform standInRotor;

        public bool rotationEnabled = true;
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            playerMovement = GetComponent<PlayerMovement>();
            startAngle = cameraGameObj.transform.rotation.eulerAngles; 
        }
        
        void Update()
        {
            if (Application.isFocused && rotationEnabled)
            {
                RotateCamera();
            }
        }

        void RotateCamera()
        {
            camAngle = cameraGameObj.transform.rotation.eulerAngles; //Get the actual angle, 
            mouseX = playerMovement.rightStickBuffer.x;// This is actually the horizontal axis
            mouseY = -playerMovement.rightStickBuffer.y;// this is the vertical axis

                // taking the mouse movement on the x axis
                deltaX = mouseX * Time.deltaTime * rotationSpeed;
                // taking the mouse movement on the y axis.
                deltaY = mouseY * Time.deltaTime * rotationSpeed;

            camAngle = cameraGameObj.transform.rotation.eulerAngles;
            camAngle.x += deltaY;
            camAngle.y += deltaX;
            camAngle.x = camAngle.x % 360; // This just takes any numbers outside of 0 - 360 and converts them to a number between 0-360.
            // camera angle can not rotate to completely vertical  because weird stuff happens... because im doing euler quaternion conversions.
            if (camAngle.x > 70 && camAngle.x < 180)
            {
                camAngle.x = 70;
            }
            else if (camAngle.x < 278 && camAngle.x > 180)
            {
                camAngle.x = 278;
            }
            transform.rotation = Quaternion.Euler(0, camAngle.y, 0);
            cameraGameObj.transform.rotation = Quaternion.Euler(camAngle.x, camAngle.y, 0);
        }
        public void ResetAngle()
        {
            cameraGameObj.transform.rotation = Quaternion.Euler(transform.rotation.x,transform.rotation.y, 0);
        }

    }
}
