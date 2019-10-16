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

    private Vector3 localVelocity = Vector3.zero;
    // Update is called once per frame
    void Update()
    {
        localVelocity = transform.InverseTransformDirection(master.rb.velocity);
        
        Vector3 newEulerRot = transform.rotation.eulerAngles;
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
        if (master.rb.velocity.magnitude > 0.1f)
        {
            float angleToVelocity = Vector3.Angle(localVelocity,new Vector3(0,0,1));
            Vector3 counterVelocity = Quaternion.AngleAxis(angleToVelocity, Vector3.up) * localVelocity * -1;
            master.rb.AddForceAtPosition(counterVelocity, master.transform.position + transform.localPosition);
            
            Debug.DrawRay(transform.position,localVelocity, Color.red);
            Debug.DrawRay(transform.position,transform.forward * 10, Color.green);
            Debug.DrawRay(transform.position,counterVelocity, Color.blue);
            
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
        master.rb.AddForceAtPosition(transform.TransformDirection(localDirection) * strength,master.transform.position + transform.localPosition);
    }
}
