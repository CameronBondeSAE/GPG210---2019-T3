using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Students.Luca
{
    public class Opener : MonoBehaviour
    {
        public KeyCode toggleKey;

        public float rotationSpeed = 30;

        [ShowInInspector] private bool openDoor = false;

        public bool OpenDoor
        {
            get => openDoor;
            set { openDoor = value; }
        }

        public Vector3 openRot = Vector3.zero;
        public Vector3 closeRot = Vector3.zero;

        private Quaternion openRotation;
        private Quaternion closeRotation;

        // Start is called before the first frame update
        void Start()
        {
            openRotation = Quaternion.Euler(openRot);
            closeRotation = Quaternion.Euler(closeRot);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(toggleKey))
            {
                OpenDoor = !OpenDoor;
            }

            if (openDoor && transform.localRotation != openRotation)
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, openRotation,
                    rotationSpeed * Time.deltaTime);

            }
            else if (!openDoor && transform.localRotation != closeRotation)
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, closeRotation,
                    rotationSpeed * Time.deltaTime);
            }
        }
    }
}
