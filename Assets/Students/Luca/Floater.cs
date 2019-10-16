using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    private Rigidbody rb;
    public float zeroForceHeight = 3;
    
    public float currentDistanceToGround = 0;

    public AnimationCurve forceHeightCurve;
    
    public Vector3 maxForce;
    public Vector3 offset;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position+offset, Vector3.down, out hit, zeroForceHeight))
        {
            currentDistanceToGround = hit.distance;
        }

        //Vector3 finalForce = 1/currentDistanceToGround * maxForce;

        Vector3 finalForce = forceHeightCurve.Evaluate(currentDistanceToGround / zeroForceHeight) * maxForce;
        
        rb.AddForceAtPosition(finalForce, transform.position + offset);
    }
}
