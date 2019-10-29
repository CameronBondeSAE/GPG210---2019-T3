using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

namespace Students.Blaide
{
    public class ExplosiveVehicle : MonoBehaviour
    {
        public Rigidbody rB;
        public GameObject explosionPrefab;
        public float impulseThreshold;
        public float explosionForce;
        public float explosionRadius;
        public float upwardsModifier;
        // Start is called before the first frame update
        void Start()
        {
            rB = GetComponent<Rigidbody>();
        }
    
        // Update is called once per frame
        void Update()
        {
            
        }

        private void OnCollisionEnter(Collision other)
        {
            Vector3 point = other.contacts[0].point;
            
            
            if (other.impulse.magnitude >= impulseThreshold)
            {
                foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
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
                       SetUpChildObject(t.gameObject.GetComponent<Rigidbody>(),point);
                    }
                    else
                    {
                        SetUpChildObject(t.gameObject.AddComponent<Rigidbody>(),point);
                    }
                    
                }
            }
            
        }
    private void SetUpChildObject(Rigidbody tRB, Vector3 centre)
        {

            tRB.isKinematic = false;
            tRB.useGravity = true;
            tRB.constraints = RigidbodyConstraints.None;
            tRB.AddExplosionForce(explosionForce, centre, explosionRadius, upwardsModifier);
        }
    }

}

