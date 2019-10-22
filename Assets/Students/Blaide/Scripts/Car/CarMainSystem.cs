using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarMainSystem : MonoBehaviour
{
    public InputAction asda;
    public float baseEngineTorque;
    public AnimationCurve springCurve;
    public List<WheelScript> wheels;
    public float forceMultiplier;
    public float maxDistance;
    public float accelerator;
    public float steering;
    public AnimationCurve tyreFriction;
    public float revs;
    public float maxRevs;
    public float revDrop;
    public float accelerationMultiplier;
    public AnimationCurve accelerationCurve;
    public float breaking;
    private Rigidbody rB;
    public Transform centreOfMass;


    public float velocity;
    // Start is called before the first frame update
    void Start()
    {
        rB = GetComponent<Rigidbody>();
        rB.centerOfMass.Set(centreOfMass.localPosition.x,centreOfMass.localPosition.y,centreOfMass.localPosition.z);
    }

    // Update is called once per frame
    private void Update()
    {
        accelerator = Input.GetAxis("Vertical");
        steering = Input.GetAxis("Horizontal") * 45;
        breaking = Input.GetAxis("Jump");
        velocity = rB.velocity.magnitude;
    }

    void FixedUpdate()
    {

        if (accelerator != 0f && revs < maxRevs)
        {
            revs += accelerator * accelerationMultiplier;
        }
        else if (revs > 0)
        {
            revs -= revDrop;
        }
        else
        {
            revs = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space) )
        {
            if (IsUpsideDown())
            {
              FlipCar();  
            }

            Debug.Log("up.y:" + transform.up.y);
        }


        foreach ( WheelScript wheel in wheels)
        {
            if (wheel.steeringWheel)
            {
                wheel.transform.localRotation = Quaternion.Euler(steering * (wheel.invertSteering?-1:1) + 90 ,-90,270); 
            }

            
            RaycastHit hit;
            Physics.Raycast(wheel.transform.position, wheel.transform.TransformDirection(Vector3.down), out hit,maxDistance);
            if (hit.collider != null && hit.collider.gameObject != this)
            { 
                //Suspension
                rB.AddForceAtPosition(wheel.transform.up * forceMultiplier * springCurve.Evaluate(hit.distance), wheel.transform.position);
                wheel.wheelModel.transform.position = hit.point + Vector3.up *0.4f;


                if (wheel.driveWheel)
                {
                  rB.AddForceAtPosition(wheel.transform.forward* accelerationCurve.Evaluate(revs) * (baseEngineTorque/DriveWheels()),wheel.transform.position);  
                }
                
                
                //asymmetric friction
               // Vector3 localVelocity = wheel.transform.InverseTransformDirection(rB.velocity);
               
               Vector3 localVelocity = wheel.transform.InverseTransformDirection(wheel.transform.position - wheel.LastPosition)/ Time.deltaTime;
               
                rB.AddForceAtPosition (wheel.transform.TransformDirection(new Vector3(-localVelocity.x *tyreFriction.Evaluate(Mathf.Abs(localVelocity.x)),0,-localVelocity.z *tyreFriction.Evaluate(Mathf.Abs(localVelocity.z)) * breaking))  * 0.8f ,wheel.transform.position);
            }
            else
            {
                wheel.wheelModel.transform.position = wheel.transform.position +  wheel.transform.up * -(maxDistance - 0.4f);

            }


            wheel.LastPosition = wheel.transform.position;
        }
    }
    
    public int DriveWheels()
    {
        int i = 0;
        foreach (WheelScript wheel in wheels)
        {
            if (wheel.driveWheel)
            {
                i++;
            }
        }

        return i;
    }

    private bool IsUpsideDown()
    {
        return transform.forward.y < -0.5;
    }

    private void FlipCar()
    {
        rB.velocity = new Vector3(0,0,0);
        rB.angularVelocity = new Vector3(0,0,0);
        transform.position += new Vector3(0, 5, 0);
        transform.rotation = Quaternion.Euler(-90,0,0);
    }

}
