using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DylanThruster : DylanCar
{

    public float thrusterForceMultiplier;

    public float maxDistance = 3;
    
    public AnimationCurve springStrength;

    private void FixedUpdate()
    {
        RaycastHit hitInfo;
        Ray downRay = new Ray(transform.position, Vector3.down);

        

        if (Physics.Raycast(downRay, out hitInfo))
        {
            float hover = maxDistance - hitInfo.distance;
            if(hover > 0)
            {
                AddDownwardThrust(springStrength.Evaluate(hitInfo.distance) * thrusterForceMultiplier);
            }
        }

        foreach(GameObject thruster in thrusters)
        {
            //Vector3 localVelocity = thruster.transform.InverseTransformDirection(rb.velocity);
            //Debug.Log(localVelocity);
            Debug.DrawRay(thruster.transform.position,transform.TransformDirection(Vector3.down) , Color.red);
        }
    }
    
    public void AddDownwardThrust(float force)
    {
        foreach (GameObject thruster in thrusters)
        {
            rb.AddForceAtPosition(thruster.transform.up * force, thruster.transform.position);
        }
    }

}
