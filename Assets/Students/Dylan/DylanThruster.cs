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

    private void Start()
    {
        mainBody = FindObjectOfType<DylanCar>();
        defaultWheelRotation = transform.localRotation;
    }

    private void FixedUpdate()
    {
        //currentSteeringAngles = new Vector3(0, 0, mainBody.turningSpeed);
        RaycastHit hitInfo;
        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hitInfo, springLength);

        Vector3 localVelocity = transform.InverseTransformDirection(mainBody.rb.velocity);
        onGround = (hitInfo.collider != null);

        if (onGround)
        {
            Vector3 direction = new Vector3((-localVelocity.x * lateralFriction) * springStrength.Evaluate(Mathf.Abs(localVelocity.x)), 0, 0);
            mainBody.rb.AddForceAtPosition((transform.up * thrusterForceMultiplier) * (springLength - hitInfo.distance), direction);
            wheelModel.transform.position = hitInfo.point + Vector3.up * 0.5f;
        }
        else
        {
            wheelModel.transform.position = transform.position + transform.up * -(springLength - 0.5f);
        }

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down));
        //steeringWheel.eulerAngles = currentSteeringAngles;
    }


    public void AddForwardThrust(float speed)
    {
        Vector3 localVelocity = transform.InverseTransformDirection(mainBody.rb.velocity);
        Vector3 direction = new Vector3((-localVelocity.x * lateralFriction) * springStrength.Evaluate(Mathf.Abs(localVelocity.x)), 0, 0);
        foreach (GameObject wheel in mainBody.drivingWheels)
        {
            mainBody.rb.AddForceAtPosition(transform.TransformDirection(Vector3.right) * speed, wheel.transform.position);
            //mainBody.rb.AddForceAtPosition(transform.TransformDirection(direction), transform.position);
        }
    }

    public void AddBackwardThrust(float speed)
    {
        Vector3 localVelocity = transform.InverseTransformDirection(mainBody.rb.velocity);
        Vector3 direction = new Vector3((-localVelocity.x * lateralFriction) * springStrength.Evaluate(Mathf.Abs(localVelocity.x)), 0, 0);
        foreach (GameObject wheel in mainBody.drivingWheels)
        {
            mainBody.rb.AddForceAtPosition(transform.TransformDirection(Vector3.right) * speed, wheel.transform.position);
            //mainBody.rb.AddForceAtPosition(transform.TransformDirection(direction), transform.position);
        }
    }

    /*    public void AddDownwardThrust(float force)
        {
            foreach (GameObject thruster in thrusters)
            {
                rb.AddForceAtPosition(thruster.transform.up * force, thruster.transform.position);
            }
        }*/

}