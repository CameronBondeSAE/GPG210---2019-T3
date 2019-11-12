using UnityEngine;

namespace Students.Luca.Scripts
{
    public class Wheel : InputReceiver, IRotatable
    {
        public Rigidbody master;
    
        private Vector3 localBaseRotation;

        public bool autoResetAngle = true;
        public bool inputTurnLeft = false;
        public bool inputTurnRight = false;

        public float maxTurnAngle = 20; //Degrees
        public float rotationSpeed = 30;

        public float turnForceMultiplier = 1;

        public AnimationCurve tireToVelocityMultiplierCurve;
        public AnimationCurve velocityFrictionMultiplierCurve;

        [Header("Float Settings")]
        public float distanceToGround = 1f;
        public float currentDistanceToGround = 0;
    
        // Start is called before the first frame update
        void Start()
        {
            master = GetComponentInParent<Rigidbody>();
            //rb = GetComponentInParent<Rigidbody>(); // Hacky
            localBaseRotation = transform.localRotation.eulerAngles;
        }

        public bool doDebug = false;
        private Vector3 localVelocity = Vector3.zero;

        private Vector3 newEulerRotation;
        private bool isGrounded = false;
        // Update is called once per frame
        void Update()
        {
            isGrounded = IsGrounded();
            
            localVelocity = transform.InverseTransformDirection(master.velocity);
            HandleRotation();


            // APPLY FORCES
            if (!Car.ApproximatelyT(/*master.rb.velocity*/localVelocity.magnitude, 0, 0.01f) && isGrounded)
            {
                
                // Friction & co
                float angleToVelocity = Vector3.Angle(transform.forward,master.velocity);

                float tToVAngleMultCurveIndex = ((angleToVelocity>90?180-angleToVelocity:angleToVelocity) % 90) / 90;
                float tireToVelocityAngleMultiplier = tireToVelocityMultiplierCurve.Evaluate(tToVAngleMultCurveIndex);
                    
                float frictionMultiplier = velocityFrictionMultiplierCurve.Evaluate(localVelocity.magnitude);

                localVelocity.Scale(new Vector3(1, 1, 0));
                Vector3 finalForce = tireToVelocityAngleMultiplier * frictionMultiplier * (master.mass/2) *
                                     master.transform.TransformDirection(-localVelocity); // (master.rb.mass / 2) HACK
                
                
                master.AddForceAtPosition(finalForce, transform.position);
                
                
                if (doDebug)
                {
                    Debug.DrawRay(transform.position,finalForce, Color.yellow);
                    Debug.DrawRay(transform.position, -localVelocity, Color.cyan);
                }

                RandomUpdateHackFunction();
            }
        }
        
        public virtual void RandomUpdateHackFunction()
        {
            
        }

        protected void HandleRotation()
        {
            bool rotChanged = false;
            newEulerRotation = transform.localRotation.eulerAngles;
            if (inputTurnLeft)
            {
                newEulerRotation.y = Mathf.MoveTowardsAngle(newEulerRotation.y, localBaseRotation.y-maxTurnAngle, rotationSpeed * Time.deltaTime);
                rotChanged = true;
            }
            if (inputTurnRight)
            {
                newEulerRotation.y = Mathf.MoveTowardsAngle(newEulerRotation.y, localBaseRotation.y+maxTurnAngle, rotationSpeed * Time.deltaTime);
                rotChanged = true;
            }

            if (autoResetAngle && !inputTurnLeft && !inputTurnRight && !Mathf.Approximately(newEulerRotation.y, localBaseRotation.y))
            {
                newEulerRotation.y = Mathf.MoveTowardsAngle(newEulerRotation.y, localBaseRotation.y, rotationSpeed * Time.deltaTime);
                rotChanged = true;
            }

            if (rotChanged)
            {
                transform.localRotation = Quaternion.Euler(newEulerRotation);
            }
            
            
            inputTurnLeft = false; // Hacky
            inputTurnRight = false; // Hacky
        }

        public void ApplyForce(float strength, Vector3 localDirection)
        {
            if (!isGrounded)
                return;
            
            Vector3 force = transform.TransformDirection(localDirection) * strength * LSA_Y_ValueMultiplier; // HACK
            Debug.DrawRay(transform.position,force, Color.black);
            
            master.AddForceAtPosition(force,transform.position);
        }

        protected virtual bool IsGrounded()
        {
            RaycastHit hit;
            Debug.DrawRay(transform.position, currentDistanceToGround*-transform.up/*-Vector3.up*/, Color.blue);
            if (Physics.Raycast(transform.position, -transform.up/*-Vector3.up*/, out hit, 100)) // TODO do raycast from bottom/exhaust
            {
                currentDistanceToGround = hit.distance;
            }
            else
            {
                currentDistanceToGround = float.PositiveInfinity; //zeroForceHeight;
            }
            
            return currentDistanceToGround <= distanceToGround;
        }

        public void TurnLeft()
        {
            inputTurnLeft = true;
            /*Vector3 newRot = transform.localRotation.eulerAngles;
            newRot.y = Mathf.MoveTowardsAngle(newRot.y,
                localBaseRotation.y - maxTurnAngle, rotationSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(newRot);*/
        }

        public void TurnRight()
        {
            inputTurnRight = true;
            /*Vector3 newRot = transform.localRotation.eulerAngles;
            newRot.y = Mathf.MoveTowardsAngle(newRot.y,
                localBaseRotation.y + maxTurnAngle, rotationSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(newRot);*/
        }

        public void ToggleAutoReset()
        {
            autoResetAngle = !autoResetAngle;
        }

        public override void LeftStickAxis(Vector2 value)
        {
            if (!useLSA)
                return;
            
            
            value = CalculateLSAValue(value); // Hacky
            
            // TODO use input value to define xurrent target angle.
            if (value.x < 0)
            {
                inputTurnLeft = true;
                inputTurnRight = false;
            }
            else if(value.x > 0)
            {
                inputTurnLeft = false;
                inputTurnRight = true;
            }
            else
            {
                inputTurnLeft = false;
                inputTurnRight = false;
            }
            
        }

        public override void RightStickAxis(Vector2 value)
        {
            throw new System.NotImplementedException();//TODO
        }
    }
}