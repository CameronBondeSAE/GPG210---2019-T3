using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverCraftThrustControl : MonoBehaviour
{
    public ThrusterScript tScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            tScript.ApplyThrust(1);
        }
        
        tScript.RotateThrusters(-Input.GetAxis("Mouse X"));
    }
}
