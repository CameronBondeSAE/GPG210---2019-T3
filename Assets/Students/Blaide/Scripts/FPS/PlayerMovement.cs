using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

namespace Students.Blaide
{
    public class PlayerMovement : Possessable
    {
        // Start is called before the first frame update
        private CharacterController charCtrlr;
        private Rigidbody rB;

        public float inputFilter;
        public float speed;
        public float friction;
        public float maxSpeed;

        private Vector3 velocity;
        private Vector3 moveDir;
        public Vector2 leftStickBuffer;
        public Vector2 rightStickBuffer;
        private bool JumpPressed;

        public float jumpForce;

        void Start()
        {
            charCtrlr = GetComponent<CharacterController>();
            rB = GetComponent<Rigidbody>();
        }

        public override void LeftStickAxis(Vector2 value)
        {
            leftStickBuffer.y = value.y;
            leftStickBuffer.x = value.x;
        }

        public override void RightStickAxis(Vector2 value)
        {
            rightStickBuffer.y = value.y;
            rightStickBuffer.x = value.x;
        }

        public override void OnActionButton1()
        {
            JumpPressed = true;
        }
        
        void FixedUpdate()
        {
            moveDir.z = leftStickBuffer.y;
            moveDir.x = leftStickBuffer.x;
            moveDir = transform.rotation * moveDir;
            velocity += moveDir * (charCtrlr.isGrounded ? 1 : 0.5f) * speed * Time.deltaTime;

            if (charCtrlr.isGrounded)
            {
                velocity.x -= velocity.normalized.x * Time.deltaTime * 1;
                velocity.z -= velocity.normalized.z * Time.deltaTime * 1;
            }

            if ( JumpPressed && charCtrlr.isGrounded)
            {
                velocity.y = jumpForce;
            }
            else if (!charCtrlr.isGrounded)
            {
                velocity.y += -0.5f * Time.deltaTime;
            }
            JumpPressed = false;
            charCtrlr.Move(velocity);
        }
    }
}
