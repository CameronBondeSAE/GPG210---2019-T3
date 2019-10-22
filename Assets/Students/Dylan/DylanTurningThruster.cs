using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DylanTurningThruster : DylanCar
{
    
    public void MoveRight(float force)
    {
        foreach (GameObject thruster in turningThrusters)
        {
            rb.AddForceAtPosition(thruster.transform.forward * force, thruster.transform.position);
        }
    }

    public void MoveLeft(float force)
    {
        foreach (GameObject thruster in turningThrusters)
        {
            rb.AddForceAtPosition(-thruster.transform.forward * force, thruster.transform.position);
        }
        
    }

    public void AddForwardThrust(float force)
    {
        foreach (GameObject thruster in turningThrusters)
        {
            rb.AddForceAtPosition(thruster.transform.TransformDirection(Vector3.right),thruster.transform.position);
        }
    }
    public void AddBackwardThrust(float force)
    {
        foreach (GameObject thruster in turningThrusters)
        {
            rb.AddForceAtPosition(-thruster.transform.TransformDirection(Vector3.right), thruster.transform.position);
        }
    }
}
