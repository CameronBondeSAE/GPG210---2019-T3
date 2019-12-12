using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Students.Blaide
{
    public class ExplosiveVehicle : MonoBehaviour
    {
        [ShowInInspector]
        private Rigidbody rB;
        public GameObject explosionPrefab;
        public float impulseThreshold;
        public float explosionForce;
        public float explosionRadius;
        public float upwardsModifier;
        public float invincibilityTimer;
        public float currentTimerValue;
        public Fuel fuel;
        public bool explodeOnOutOfFuel;
        public Possessable possessable;

        // Start is called before the first frame update
        
        void Start()
        {
            rB = rB == null ? GetComponent<Rigidbody>() : rB;
            currentTimerValue = invincibilityTimer;
            fuel = fuel == null ? GetComponent<Fuel>() : fuel;
            if(explodeOnOutOfFuel) fuel.OnOutOfFuel += ExplodeFromCentre;
            possessable = possessable == null ? GetComponent<Possessable>() : possessable;
        } 
    
        // Update is called once per frame
        void Update()
        {
            if (currentTimerValue > 0)
            {
                currentTimerValue -= Time.deltaTime;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            Vector3 point = other.contacts[0].point;

            if (other.impulse.magnitude >= impulseThreshold && currentTimerValue <= 0)
            {
                
                
               Explode(point);

            }
  
        }
        
        private void SetUpChildObject(Rigidbody tRB, Vector3 centre)
        {

            tRB.isKinematic = false;
            tRB.useGravity = true;
            tRB.constraints = RigidbodyConstraints.None;
            tRB.AddExplosionForce(explosionForce * tRB.mass, centre, explosionRadius, upwardsModifier);
            foreach (MonoBehaviour M in  tRB.gameObject.GetComponents<MonoBehaviour>())
            {
                M.enabled = false;
            }
           
        }

        public void ExplodeFromCentre()
        {
            Explode(transform.position);
        }

        public void Explode( Vector3 point)
        {
            
            foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
            {
                if (t.gameObject.GetComponent<CinemachineVirtualCamera>() == null && t.gameObject != this.gameObject && t.gameObject.GetComponent<MeshFilter>() != null)
                {
                    if (t.gameObject.GetComponent<Collider>() != null)
                    {
                        t.GetComponent<Collider>().isTrigger = false;
                    }
                    else
                    {
                        MeshCollider col = t.gameObject.AddComponent<MeshCollider>();
                        col.isTrigger = false;
                        col.convex = true;
                    }

                    if (t.gameObject.GetComponent<Rigidbody>() != null)
                    {
                        SetUpChildObject(t.gameObject.GetComponent<Rigidbody>(), point);
                    }
                    else
                    {
                        SetUpChildObject(t.gameObject.AddComponent<Rigidbody>(), point);
                    }
                    
                }
            }

            if (possessable != null)
            { 
                possessable.Eject();
            }

            rB.AddExplosionForce(explosionForce * rB.mass, point, explosionRadius, upwardsModifier);
            rB.gameObject.GetComponent<VehicleSystem>().enabled = false;
        }
    }
    

}

