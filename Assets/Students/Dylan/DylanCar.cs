using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DylanCar : MonoBehaviour
{
    public List<GameObject> thrusters;
    public List<GameObject> turningThrusters;

    private DylanTurningThruster turningThruster;
    private DylanThruster dylanThruster;

    public float turningSpeed;
    public float speed;

    public Rigidbody rb;

    private Vector3 originalRot;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = 1;
        turningThruster = GetComponent<DylanTurningThruster>();
        dylanThruster = GetComponent<DylanThruster>();
        originalRot = new Vector3(0,0,0);
    }

    private void FixedUpdate()
    {
        
        //turning left and right
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            turningThruster.MoveLeft(turningSpeed);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            turningThruster.MoveRight(turningSpeed);
        }
        //moving forward and back
        if (Input.GetKey(KeyCode.UpArrow))
        {
            turningThruster.AddForwardThrust(speed);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            turningThruster.AddBackwardThrust(speed);
        }
        
    }

    



}
