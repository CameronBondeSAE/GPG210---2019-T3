using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Bazooka : MonoBehaviour
{
    public Transform rotationPoint;
    
    public ObjectPool objectPool; // Hack
    
    public string projectilePoolItemName;
    public float fireStrength = 800;
    public float shootCooldown = 2;

    public Transform projectileExitVector;

    public Rigidbody rb;
    
    private float currentShootCooldown = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        if (rb == null)
        {
            rb = GetComponentInParent<Rigidbody>();
        }

        if (objectPool == null)
        {
            objectPool = GameObject.FindObjectOfType<ObjectPool>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // TODO Integrate in Input Manager
        if (Input.GetMouseButtonDown(0) && currentShootCooldown <= 0)
        {
            ShootProjectile();
            currentShootCooldown = shootCooldown;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f && transform.localRotation.x < -0.75f) // HACK VALUES
        {
            transform.RotateAround(rotationPoint.position, transform.right, 10);
        }else if (Input.GetAxis("Mouse ScrollWheel") < 0f && transform.localRotation.x >= -0.85f) // HACK VALUES
        {
            transform.RotateAround(rotationPoint.position, transform.right, -10);
        }

        if (currentShootCooldown > 0)
        {
            currentShootCooldown -= Time.deltaTime;
        }
    }

    private void ShootProjectile()
    {
        GameObject projectileObj = objectPool.SpawnObject(projectilePoolItemName);
        projectileObj.transform.rotation = Quaternion.LookRotation(projectileExitVector.forward);//projectileExitVector.rotation;
        
        projectileObj.transform.position = projectileExitVector.position;

        Projectile projectile = projectileObj.GetComponent<Projectile>();
        Vector3 forcePosition = projectile.exhausPosition?.position ?? projectileObj.transform.position;
        projectile.rb.AddForceAtPosition(projectileObj.transform.forward * fireStrength,forcePosition,ForceMode.Impulse);
        
    }
}
