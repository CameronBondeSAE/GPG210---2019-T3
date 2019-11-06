using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DylanCar : Possessable
{
    public List<GameObject> drivingWheels;
    public List<GameObject> turningWheels;

    private DylanThruster dylanThruster;

    public Transform centreOfMass;

    private DylanController controller;

    public float turningSpeed;
    public float speed;
    public float maxSpeed;

    //public bool isPossessed;

    public float maxTurningAngle = 90;
    //public float maxLeftAngle = 90;

    /*
    public KeyCode forward;
    public KeyCode backward;
    public KeyCode left;
    public KeyCode right;
    */

    public Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        dylanThruster = FindObjectOfType<DylanThruster>();
        rb.centerOfMass = centreOfMass.localPosition;
        controller = FindObjectOfType<DylanController>();
    }

    private void FixedUpdate()
    {
        //speed = Input.GetAxis("Vertical") * maxSpeed;
        //turningSpeed = Input.GetAxis("Horizontal") * maxTurningAngle;

            if (speed >= maxSpeed)
            {
                speed = maxSpeed;
            }

            if (dylanThruster.onGround)
            {
            /*
                if (Input.GetKey(left))
                {
                    //turningSpeed -= 1;
                    TurnWheel(turningSpeed);
                }
                else if (Input.GetKey(right))
                {
                    //turningSpeed += 1;
                    TurnWheel(turningSpeed);
                }
                else if (Input.GetKey(forward))
                {
                    //speed += 1f;
                    dylanThruster.AddForwardThrust(speed);
                }
                else if (Input.GetKey(backward))
                {
                    //speed -= 1;
                    dylanThruster.AddBackwardThrust(speed);
                }
                else
                {
                    FixTireAngle();
                    speed = 0;
                }*/

                TurnWheel(turningSpeed);
                dylanThruster.AddForwardThrust(speed);


            }
        
        
    }

    public override void LeftStickAxis(Vector2 value)
    {
        turningSpeed = value.x * maxTurningAngle;
        speed = value.y * maxSpeed;
    }

    void TurnWheel(float turnSpeed)
    {
        foreach (GameObject wheel in turningWheels)
        {
            wheel.transform.localRotation = Quaternion.Euler(dylanThruster.defaultWheelRotation.x, turnSpeed, dylanThruster.defaultWheelRotation.z);
        }
        
    }

    //no longer needed since the change to axis controls,
    public void FixTireAngle()
    {
        foreach (GameObject wheel in turningWheels)
        {
            wheel.transform.localRotation = Quaternion.Euler(dylanThruster.defaultWheelRotation.x, 0, dylanThruster.defaultWheelRotation.z);
            turningSpeed = 0;
        }
    }

    /*//was a test to turn back wheels the opposite way to the front ones
    void TurnBackWheels(float turnSpeed)
    {
        foreach (GameObject wheel in backWheels)
        {
            wheel.transform.localRotation = Quaternion.Euler(dylanThruster.defaultWheelRotation.x, turnSpeed, dylanThruster.defaultWheelRotation.z);
        }
    }
    */

    
}
