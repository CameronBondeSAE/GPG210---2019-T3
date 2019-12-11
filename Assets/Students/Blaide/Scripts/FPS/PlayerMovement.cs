using System;
using System.Collections;
using System.Collections.Generic;
using Students.Luca.Scripts;
using UnityEngine;
using UnityEngine.InputSystem.HID;

namespace Students.Blaide
{
    public class PlayerMovement : Possessable
    {
        /// <summary>
        /// This is playerMovement, It inherits from possessable as the player 'possesses' the playerCharacter.
        /// </summary>
        private Rigidbody rB;
        public Transform feetPos;
        public LayerMask ground;
        public float speed;
        public float maxVelocity;
        public Water water;
        public bool grounded;
        public bool inWater;
        public float submergence;
        private Vector3 velocity;
        private Vector3 moveDir = Vector3.zero;
        public Vector2 leftStickBuffer;
        public Vector2 rightStickBuffer;
        public CameraControlFirstPerson ccfps;
        private bool jumpPressed;
        public float waterDrag;
        private float defaultDrag;

        public float jumpForce;
        public float bouyancy;

        void Start()
        {
            rB = GetComponent<Rigidbody>();
            ccfps = GetComponent<CameraControlFirstPerson>();
            defaultDrag = rB.drag;
            water = FindObjectOfType<Water>();
        }

        public override void Activate(Controller c)
        {
            base.Activate(c);
            ccfps.ResetAngle();
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
            jumpPressed = true;
        }
        
        void FixedUpdate()
        {
            moveDir.z = leftStickBuffer.y;
            moveDir.x = leftStickBuffer.x;
            moveDir.y = 0;
            moveDir = transform.rotation * moveDir;
            
            RaycastHit hit;
            Vector3 rayDirection = feetPos.InverseTransformDirection(Vector3.down);
            Vector3 rayOrigin = feetPos.position;
            grounded = (Physics.Raycast(rayOrigin, rayDirection, out hit, 0.3f, ground,QueryTriggerInteraction.Collide));
            
            moveDir *= (grounded ? 1 : 0.2f) * speed * Time.deltaTime;
            
            if (grounded)
            {
                if (hit.collider.GetComponent<Water>()!= null)
                {
                    water = hit.collider.GetComponent<Water>();
                }
            }

            inWater = false;
            if (water != null)
            {
                submergence = water.GetSurfaceLevelAtPos(feetPos.position.x, feetPos.position.y) -feetPos.position.y;
                if ((water.GetComponent<Collider>().bounds.Contains(feetPos.position))&& submergence >= 0.01f)
                {
                    inWater=true;
                }
            }
            
            if (grounded && !inWater)
            { 
                if ( jumpPressed ) 
                {
                   rB.AddForce(Vector3.up *jumpForce); 
                }
                moveDir = Vector3.ProjectOnPlane(moveDir, hit.normal);
            }
            else if(inWater)
            {
 
                    rB.AddForce(Vector3.up *bouyancy * submergence *0.6f);

                rB.drag = waterDrag;
                
            }
            if (!inWater)
            {
                jumpPressed = false;
                rB.drag = defaultDrag;
            }
            rB.AddForce(moveDir);
            if (rB.velocity.magnitude > maxVelocity)
            {
                rB.AddForce(rB.velocity.normalized*(maxVelocity- rB.velocity.magnitude));
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(feetPos.position,feetPos.position + feetPos.InverseTransformDirection(Vector3.down)*0.3f);
        }

    } 
}

