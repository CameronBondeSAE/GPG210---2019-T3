using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DylanThruster : MonoBehaviour
{

    public float thrusterForceMultiplier;

    public GameObject wheelModel;
    public Quaternion defaultWheelRotation;

    public bool onGround;

    public DylanCar mainBody;

    public Quaternion steeringWheel;
    public Vector3 currentSteeringAngles;

    public float lateralFriction = 10;

    public AnimationCurve springStrength;
    public float springLength = 1.5f;
    private float disToGround;

    private void Start()
    {
        //mainBody = FindObjectOfType<DylanCar>();
        defaultWheelRotation = transform.localRotation;
    }

    public void SetWheelPosition()
    {
        #region Wheel Position

        disToGround = springLength;
        //currentSteeringAngles = new Vector3(0, 0, mainBody.turningSpeed);
        RaycastHit hitInfo;
        //sets how far from the ground the weeks should be
        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hitInfo, springLength);

        Vector3 localVelocity = transform.InverseTransformDirection(mainBody.rb.velocity);
        //onGround = (hitInfo.collider != null);
        //sets the bool for when to apply the downward force that keeps the car up
        onGround = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down),disToGround);
        if (onGround)
        {
            //Vector3 direction = new Vector3((-localVelocity.x * lateralFriction) * springStrength.Evaluate(Mathf.Abs(localVelocity.x)), 0, 0);
            mainBody.rb.AddForceAtPosition((transform.up * thrusterForceMultiplier) * (springLength - hitInfo.distance), transform.position);
            wheelModel.transform.position = hitInfo.point + Vector3.up * 0.5f;
        }
        else
        {
            wheelModel.transform.position = transform.position + transform.up * -(springLength - 0.5f);
        }

        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down));
        //steeringWheel.eulerAngles = currentSteeringAngles;

        #endregion
    }

    //pushes car forward
    public void AddForwardThrust(float speed)
    {
        //Vector3 localVelocity = transform.InverseTransformDirection(mainBody.rb.velocity);
        //Vector3 direction = new Vector3((-localVelocity.x * lateralFriction) * springStrength.Evaluate(Mathf.Abs(localVelocity.x)), 0, 0);
        foreach (GameObject wheel in mainBody.drivingWheels)
        {
            //mainBody.rb.AddForceAtPosition(transform.TransformDirection(mainBody.transform.InverseTransformDirection(Vector3.right)) * speed * Time.deltaTime, wheel.transform.position);
            mainBody.rb.AddForceAtPosition(wheel.transform.localRotation *mainBody.transform.right * speed * Time.deltaTime,wheel.transform.position);
          
        }
    }

    //slows car down
    public void Break(float breakPower)
    {
        Vector3 localVelocity = transform.InverseTransformDirection(mainBody.rb.velocity);
        //Vector3 direction = new Vector3((-localVelocity.x * lateralFriction) * springStrength.Evaluate(Mathf.Abs(localVelocity.x)), 0, 0);
        foreach (GameObject wheel in mainBody.drivingWheels)
        {
            mainBody.rb.AddForceAtPosition(transform.TransformDirection(-Vector3.right) * breakPower * Time.deltaTime, wheel.transform.position);
            //mainBody.rb.AddForceAtPosition(transform.TransformDirection(direction), transform.position);
        }
    }

    //no longer needed due to the change to axis control rather than keycode control
    public void Boost(float boostPower)
    {
        foreach (GameObject wheel in mainBody.drivingWheels)
        {
            mainBody.rb.AddForceAtPosition(transform.TransformDirection(Vector3.right) * boostPower * Time.deltaTime, wheel.transform.position);
            //mainBody.rb.AddForceAtPosition(transform.TransformDirection(direction), transform.position);
        }
    }

    //turn car wheels
    public void TurnWheel(float turnSpeed)
    {
        foreach (GameObject wheel in mainBody.turningWheels)
        {
            wheel.transform.localRotation = Quaternion.Euler(defaultWheelRotation.x, turnSpeed, defaultWheelRotation.z);
        }

    }
    
    //flip car
    public void FlipCar()
    {
        mainBody.rb.velocity = new Vector3(0, 0, 0);
        mainBody.transform.position += new Vector3(0, 3, 0);
        mainBody.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    /*    public void AddDownwardThrust(float force)
        {
            foreach (GameObject thruster in thrusters)
            {
                rb.AddForceAtPosition(thruster.transform.up * force, thruster.transform.position);
            }
        }*/

}