using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody rb;
    public Transform exhausPosition;
    public Transform centerOfMass;
    public ParticleSystem windCuttingEffect;
    
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    protected void Init()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        if (centerOfMass != null)
        {
            rb.centerOfMass = centerOfMass.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        windCuttingEffect.Stop();
        Debug.Log("Boom Collision!");
    }
}
