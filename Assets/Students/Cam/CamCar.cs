using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCar : MonoBehaviour
{
    public Vector3 force;
    public Vector3 offset;

    public AnimationCurve springStrengthCurve;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    float distance;
    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hitInfo;
        Physics.Raycast(transform.position, Vector3.down, out hitInfo, distance);
        
        // position is a WORLD position. So a local offset, requires you to add the current position
        GetComponent<Rigidbody>().AddForceAtPosition(force * distance, transform.position + offset);
    }
}
