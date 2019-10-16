using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        localVelocity = transform.InverseTransformDirection(master.rb.velocity);
        localVelocity.y = 0;
        
        Vector3 newEulerRot = transform.localRotation.eulerAngles;
        if (inputTurnLeft)
        {
            newEulerRot.y = Mathf.MoveTowardsAngle(newEulerRot.y, localBaseRotation.y-maxTurnAngle, rotationSpeed * Time.deltaTime);
        }
        if (inputTurnRight)
        {
            newEulerRot.y = Mathf.MoveTowardsAngle(newEulerRot.y, localBaseRotation.y+maxTurnAngle, rotationSpeed * Time.deltaTime);
        }

        if (!inputTurnLeft && !inputTurnRight && !Mathf.Approximately(newEulerRot.y, localBaseRotation.y))
        {
            newEulerRot.y = Mathf.MoveTowardsAngle(newEulerRot.y, localBaseRotation.y, rotationSpeed * Time.deltaTime);
        }
        
        transform.localRotation = Quaternion.Euler(newEulerRot);
        
        
        
        // Friction
        if (localVelocity.magnitude > 0.01f)
        {
            /*float angleToVelocity = Vector3.SignedAngle(localVelocity,transform.forward, Vector3.up);
            Vector3 counterVelocity = Quaternion.AngleAxis(angleToVelocity, Vector3.up) * -localVelocity;
            master.rb.AddForceAtPosition(counterVelocity, master.transform.position + transform.localPosition);

            if (doDebug)
            {
                Debug.DrawRay(transform.position,localVelocity, Color.red);
                Debug.DrawRay(transform.position,transform.forward * 2, Color.green);
                Debug.DrawRay(transform.position,counterVelocity*10, Color.blue);
            }*/
            
            master.rb.AddForceAtPosition(-localVelocity, transform.TransformPoint(transform.localPosition));
            
            
            /*float angleToVelocity = Vector3.Angle(localVelocity,new Vector3(0,0,1));
            Vector3 counterVelocity = Quaternion.AngleAxis(angleToVelocity, Vector3.up) * (localVelocity * -1);//Quaternion.Euler(0, angleToVelocity, 0) * localVelocity * -1;
            //Debug.Log(""+angleToVelocity+"  "+master.rb.velocity+" "+master.rb.velocity.magnitude + "   "+counterVelocity);
            ApplyForce(1,counterVelocity/*-localVelocity * (angleToVelocity/180)#1#);*/
            
            //ApplyForce(1,-localVelocity);
        }
        
    }

    public void ApplyForce(float strength)
    {
        //master.AddForce();
        ApplyForce(strength,new Vector3(0,0,1));
    }

    void ApplyForce(float strength, Vector3 localDirection)
    {
        Vector3 force = transform.TransformDirection(localDirection) * strength;
        force.y = 0; // HACK
        master.rb.AddForceAtPosition(force,master.transform.position + transform.localPosition);
    }
}
