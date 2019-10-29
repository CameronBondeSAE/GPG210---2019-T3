using System.Net.Configuration;
using UnityEngine;
using UnityEngine.Serialization;

namespace Students.Luca
{
    public class Wheel : MonoBehaviour
    {
        public Car master;
    
        private Vector3 localBaseRotation;

        public bool inputTurnLeft = false;
        public bool inputTurnRight = false;

        public float maxTurnAngle = 20; //Degrees
        public float rotationSpeed = 30;

        public float turnForceMultiplier = 1;

        public AnimationCurve tireToVelocityMultiplierCurve;
        public AnimationCurve velocityFrictionMultiplierCurve;

        [Header("Float Settings")]
        public float distanceToGround = 1f;
        public float zeroForceHeight = 3;
        public AnimationCurve forceHeightCurve;
        public Vector3 maxForce;
        public float currentDistanceToGround = 0;
    
        // Start is called before the first frame update
        void Start()
        {
            master = GetComponentInParent<Car>();
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
            
            HandleFloating();
            
            localVelocity = transform.InverseTransformDirection(master.rb.velocity);
            HandleRotation();


            // APPLY FORCES
            if (!Car.ApproximatelyT(/*master.rb.velocity*/localVelocity.magnitude, 0, 0.01f) && isGrounded)
            {
                
                // Friction & co
                float angleToVelocity = Vector3.Angle(transform.forward,master.rb.velocity);

                float tToVAngleMultCurveIndex = ((angleToVelocity>90?180-angleToVelocity:angleToVelocity) % 90) / 90;
                float tireToVelocityAngleMultiplier = tireToVelocityMultiplierCurve.Evaluate(tToVAngleMultCurveIndex);
                    
                float frictionMultiplier = velocityFrictionMultiplierCurve.Evaluate(localVelocity.magnitude);

                localVelocity.Scale(new Vector3(1, 1, 0));
                Vector3 finalForce = tireToVelocityAngleMultiplier * frictionMultiplier * (master.rb.mass/2) *
                                     master.transform.TransformDirection(-localVelocity); // (master.rb.mass / 2) HACK
                
                
                master.rb.AddForceAtPosition(finalForce, transform.position);
                /*

                // Apply "Break" Force if wheels are in a 90° angle // HACKY
                if (Car.ApproximatelyT(newEulerRotation.y, 90, 0.1f) || Car.ApproximatelyT(newEulerRotation.y, 270, 0.1f))
                {
                    master.rb.AddForceAtPosition(-master.rb.velocity, transform.position, ForceMode.VelocityChange);
                    Debug.DrawRay(transform.position,-master.rb.velocity, Color.magenta);
                }
                else if (!Car.ApproximatelyT(newEulerRotation.y, 0, 0.05f))// Apply turn force (Only if wheels aren't looking forward)
                {
                    
                    
                    /*float angleToVelocity = Vector3.Angle(transform.forward,master.rb.velocity);
                    // tireToVelocityAngleMultiplier = (angleToVelocity % maxTurnAngle)/maxTurnAngle; // Hacky
                    float tireToVelocityAngleMultiplier = tireToVelocityMultiplierCurve.Evaluate((angleToVelocity % maxTurnAngle)/maxTurnAngle); // Hacky, needed?
                    
                    master.rb.AddForceAtPosition(tireToVelocityAngleMultiplier*turnForceMultiplier*master.transform.TransformDirection(-localVelocity), transform.position);
                    Debug.DrawRay(transform.position,tireToVelocityAngleMultiplier*turnForceMultiplier*master.transform.TransformDirection(-localVelocity), Color.magenta);#1#
                }*/
                
                
                if (doDebug)
                {
                    //Debug.Log(angleToVelocity + " " + tireToVelocityAngleMultiplier + " " + frictionMultiplier + " " + finalForce.magnitude);
                    //Debug.DrawRay(transform.position,master.rb.velocity, Color.red);
                    Debug.DrawRay(transform.position,finalForce, Color.yellow);
                    Debug.DrawRay(transform.position, -localVelocity, Color.cyan);
                }

                RandomUpdateHackFunction();
            }

        

            /*

            if (!Car.ApproximatelyT(master.rb.velocity.x,0,0.01f) || !Car.ApproximatelyT(master.rb.velocity.z,0,0.01f))
            {
                //float angleToVelocity = Vector3.SignedAngle(transform.forward,master.rb.velocity, Vector3.up);
                Vector3 localVelocityZeroY = new Vector3(localVelocity.x,0,localVelocity.z);
                float angleToVelocity = Vector3.SignedAngle(Vector3.forward,localVelocityZeroY, Vector3.up);

                // Make sure forward & backward are 0° & left/right 90°
                float absAngleToVelocity = Mathf.Abs(angleToVelocity);
                if ((absAngleToVelocity > 90 && absAngleToVelocity < 180) || (absAngleToVelocity > 270 && absAngleToVelocity < 360))
                {
                    angleToVelocity = (90 - (absAngleToVelocity % 90)) * Mathf.Sign(angleToVelocity);
                }
                
                //angleToVelocity = angleToVelocity > 0 ? angleToVelocity + 90 : angleToVelocity - 90;
                Vector3 counterVelocity = Quaternion.AngleAxis(angleToVelocity, Vector3.up) * -master.rb.velocity;//Quaternion.AngleAxis(angleToVelocity, Vector3.up) * -master.rb.velocity;//master.rb.velocity/Mathf.Cos(Mathf.Abs(angleToVelocity)*Mathf.Deg2Rad);
                //counterVelocity *= -1;
                //counterVelocity *= Mathf.Sin((Mathf.Abs(angleToVelocity) % 90)); // 90° = 1, 0° = 0
                
                ////counterVelocity = Vector3.ClampMagnitude(counterVelocity, master.rb.velocity.magnitude); // Counter force can't be bigger than Forward force! 
                
                if(!Car.ApproximatelyT(angleToVelocity,0,0.01f))
                    master.rb.AddForceAtPosition(counterVelocity, transform.position);
                
                if (doDebug)
                {
                    angleToVelocity = angleToVelocity % 90; //TMP JUST FOR LOGGING
                    
                    Debug.Log(angleToVelocity + " "+ GetHashCode());

                    if (angleToVelocity > 60 || angleToVelocity < -60)
                    {
                        Debug.Log(Vector3.forward + " " + localVelocityZeroY + " "+master.rb.velocity);
                    }
                    
                    
                    Vector3 linepos = transform.position;
                    linepos.y += 2;ity, Color.green);
                    
                    Debug.DrawRay(linepos,transform.TransformDirection(master.rb.velocity), Color.red);
                    Debug.DrawRay(linepos,transform.TransformDirection(Vector3.forward), Color.green);
                    
                    Debug.DrawRay(linepos,-localVelocity, Color.yellow);
                }
            }*/
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

            if (!inputTurnLeft && !inputTurnRight && !Mathf.Approximately(newEulerRotation.y, localBaseRotation.y))
            {
                newEulerRotation.y = Mathf.MoveTowardsAngle(newEulerRotation.y, localBaseRotation.y, rotationSpeed * Time.deltaTime);
                rotChanged = true;
            }

            if (rotChanged)
            {
                transform.localRotation = Quaternion.Euler(newEulerRotation);
            }
        }

        public void ApplyForce(float strength, Vector3 localDirection)
        {
            if (!isGrounded)
                return;
            
            Vector3 force = transform.TransformDirection(localDirection) * strength;
            Debug.DrawRay(transform.position,force, Color.black);
            
            master.rb.AddForceAtPosition(force,transform.position);
        }
        
        protected void HandleFloating()
        {
            float curveValue = Mathf.Clamp(currentDistanceToGround, 0, zeroForceHeight) / zeroForceHeight;
            float distanceToCenterOfMassDividor = (master.centerOfMass == null) ? 1 : Vector3.Distance(transform.position, master.centerOfMass.position);
            
            Vector3 finalForce = transform.TransformDirection(forceHeightCurve.Evaluate(curveValue) * master.rb.mass * maxForce) / distanceToCenterOfMassDividor;

            if (doDebug)
            {
                Debug.DrawRay(transform.position,finalForce,Color.white);
            }
            
            master.rb.AddForceAtPosition(finalForce, transform.position);
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
    }
}
