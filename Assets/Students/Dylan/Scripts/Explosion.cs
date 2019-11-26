using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float radius = 3f;
    public float force = 700f;
    
    private void OnCollisionEnter(Collision other)
    {
        Explode();
    }

    void Explode() 
    {
    
        //download particles if you want to spawn particles on explosion hit
    //Instantiate(particleeffect, transform.position, transform.rotation);
    
    Collider[] collidersToMove = Physics.OverlapSphere(transform.position, radius) ;
    
        foreach(Collider nearbyObject in collidersToMove) 
        {
        Rigidbody rb = nearbyObject. GetComponent<Rigidbody>() ;
            if(rb != null)
            {
                rb.AddExplosionForce(force,transform.position, radius);
            } 
        } 
    
    } 

}
