using UnityEngine;
using UnityEngine.Serialization;

namespace Students.Luca
{
    public class Wheel : MonoBehaviour
    {
        public static float groundFriction = 0.1f;

        public Car master;
    
        //private Rigidbody rb;
        private Vector3 localBaseRotation;

        public bool inputTurnLeft = false;
        public bool inputTurnRight = false;

        public float maxTurnAngle = 20; //Degrees
        public float rotationSpeed = 30;

        public float turnForceMultiplier = 0.5f;

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
        // Update is called once per frame
        void Update()
        {
            HandleFloating();
            
            localVelocity = transform.InverseTransformDirection(master.rb.velocity);
            //localVelocity.y = 0;

            bool rotChanged = false; // HACK TODO
            Vector3 newEulerRot = transform.localRotation.eulerAngles;
            if (inputTurnLeft)
            {
                newEulerRot.y = Mathf.MoveTowardsAngle(newEulerRot.y, localBaseRotation.y-maxTurnAngle, rotationSpeed * Time.deltaTime);
                rotChanged = true;
            }
            if (inputTurnRight)
            {
                newEulerRot.y = Mathf.MoveTowardsAngle(newEulerRot.y, localBaseRotation.y+maxTurnAngle, rotationSpeed * Time.deltaTime);
                rotChanged = true;
            }

            if (!inputTurnLeft && !inputTurnRight && !Mathf.Approximately(newEulerRot.y, localBaseRotation.y))
            {
                newEulerRot.y = Mathf.MoveTowardsAngle(newEulerRot.y, localBaseRotation.y, rotationSpeed * Time.deltaTime);
                rotChanged = true;
            }

            if (rotChanged)
            {
                transform.localRotation = Quaternion.Euler(newEulerRot);
            }


            if ((!Car.ApproximatelyT(master.rb.velocity.x, 0, 0.01f) ||
                !Car.ApproximatelyT(master.rb.velocity.z, 0, 0.01f)) && IsGrounded())
            {
                //localVelocity.y = 0;
                
                master.rb.AddForceAtPosition(master.transform.TransformDirection(-localVelocity)*turnForceMultiplier, transform.position);
                
                if (doDebug)
                {
                    Vector3 linepos = transform.position;
                    //linepos.y += 2;
                    Debug.DrawRay(linepos,master.rb.velocity, Color.red);
                    Debug.DrawRay(linepos,transform.TransformDirection(Vector3.forward), Color.green);
                    
                    //Debug.DrawRay(linepos,master.transform.TransformDirection(localVelocity), Color.yellow);
                    Debug.DrawRay(linepos,master.transform.TransformDirection(-localVelocity)*turnForceMultiplier, Color.blue);
                }
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

        public void ApplyForce(float strength)
        {
            //master.AddForce();
            ApplyForce(strength,Vector3.forward);
        }

        void ApplyForce(float strength, Vector3 localDirection)
        {
            if (!IsGrounded())
                return;
            
            Vector3 force = transform.TransformDirection(localDirection) * strength;
            force.y = 0; // HACK
            master.rb.AddForceAtPosition(force,transform.position);
        }
        
        private void HandleFloating()
        {
            RaycastHit hit;
            Debug.DrawRay(transform.position, currentDistanceToGround*-3 * transform.up, Color.blue);
            if (Physics.Raycast(transform.position, transform.up*-1, out hit, 100)) // TODO do raycast from bottom/exhaust
            {
                currentDistanceToGround = hit.distance;
            }
            else
            {
                currentDistanceToGround = float.PositiveInfinity; //zeroForceHeight;
            }
            float curveValue = Mathf.Clamp(currentDistanceToGround, 0, zeroForceHeight) / zeroForceHeight;
            Vector3 finalForce = transform.TransformDirection(forceHeightCurve.Evaluate(curveValue) * maxForce);
        
            master.rb.AddForceAtPosition(finalForce, transform.position);
        }

        bool IsGrounded()
        {
            return currentDistanceToGround <= distanceToGround;
        }
    }
}
