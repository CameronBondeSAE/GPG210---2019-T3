using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DylanCar : Possessable
{
    public List<GameObject> drivingWheels;
    public List<GameObject> turningWheels;

    private DylanThruster dylanThruster;

    public Transform centreOfMass;

    public float turningSpeed;
    public float speed;
    public float maxSpeed;
    public float breaking;
    public float breakingPower;
    public float boost;
    public float boostPower;

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
    }

    private void FixedUpdate()
    {
        //speed = Input.GetAxis("Vertical") * maxSpeed;
        //turningSpeed = Input.GetAxis("Horizontal") * maxTurningAngle;
        //breaking = Input.GetAxis("Jump") * breakingPower;
        //boost = Input.GetAxis("Jump") * boostPower;

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

        dylanThruster.TurnWheel(turningSpeed);
        dylanThruster.AddForwardThrust(speed);
        dylanThruster.Break(breaking);


        }
        dylanThruster.Boost(boost);
        if(Input.GetKeyDown(KeyCode.R))
        {
            dylanThruster.FlipCar();
        }
        
        
    }

    public override void LeftStickAxis(Vector2 value)
    {
        turningSpeed = value.x * maxTurningAngle;
        //speed = value.y * maxSpeed;
        base.LeftStickAxis(value);
    }

    public override void RightTrigger(float value)
    {
        speed = value * maxSpeed;
    }

    public override void LeftTrigger(float value)
    {
        breaking = value * breakingPower;
    }

    public override void OnActionButton1()
    {
        dylanThruster.FlipCar();
    }
}
