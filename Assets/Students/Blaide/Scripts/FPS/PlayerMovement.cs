using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

namespace Students.Blaide
{
    public class PlayerMovement : Possessable
    {
        // Start is called before the first frame update
        private Rigidbody rB;

        public Transform feetPos;
        public LayerMask JumpOffAble;
        public float speed;
        public float maxVelocity;
        
        private Vector3 velocity;
        private Vector3 moveDir = Vector3.zero;
        public Vector2 leftStickBuffer;
        public Vector2 rightStickBuffer;
        private bool JumpPressed;

        public float jumpForce;

        void Start()
        {
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
            moveDir.y = 0;
            moveDir = transform.rotation * moveDir;
            moveDir *= (IsGrounded() ? 1 : 0.2f) * speed * Time.deltaTime;
            if (IsGrounded())
            {
                if ( JumpPressed )
               {
                   rB.AddForce(Vector3.up *jumpForce);
               }
            }
            JumpPressed = false;
            
            rB.AddForce(moveDir);
            
           // Debug.Log("moveDir.z = " + moveDir.z + ". moveDir.x = " + moveDir.x + ". moveDir.y = "+ moveDir.y + ". JumpForce = " + jumpForce + ".");
        }

        public bool IsGrounded()
        {
            RaycastHit hit;
            float playerClearance = 1;
            Vector3 rayDirection = feetPos.InverseTransformDirection(Vector3.down);
            Vector3 rayOrigin = feetPos.position;
            bool didHit = (Physics.Raycast(rayOrigin, rayDirection, out hit, 0.3f, JumpOffAble));
            return didHit;
        }

        public RaycastHit Ground()
        {
            RaycastHit hit;
            float playerClearance = 1;
            Vector3 rayDirection = feetPos.InverseTransformDirection(Vector3.down);
            Vector3 rayOrigin = feetPos.position;
            Physics.Raycast(rayOrigin, rayDirection, out hit, 0.3f, JumpOffAble);
            return hit;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(feetPos.position,feetPos.position + feetPos.InverseTransformDirection(Vector3.down)*0.3f);
        }
    } 
}

