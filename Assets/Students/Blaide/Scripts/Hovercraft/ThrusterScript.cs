using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterScript : MonoBehaviour
{
    public List<GameObject> thrusters;
    public float forceMultiplier;
    private Rigidbody rB;
    // Start is called before the first frame update
    void Start()
    {
        rB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyThrust(float force)
    {
        foreach (GameObject thruster in thrusters)
        {
            rB.AddForceAtPosition ( thruster.transform.forward * force * forceMultiplier * Time.deltaTime ,thruster.transform.position);
        }
    }

    public void RotateThrusters(float steeringAxis)
    {
        foreach (GameObject thruster in thrusters)
        {
            thruster.transform.Rotate(Vector3.up, steeringAxis *0.1f);
        }
    }
}
