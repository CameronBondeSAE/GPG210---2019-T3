using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Students.Blaide
{
    public class ExplosiveVehicle : MonoBehaviour
    {
        private Rigidbody rB;
        public GameObject explosionPrefab;
        public Preset smokeParticleSystemPreset;
        public Preset trailRendererPreset;
        public Material smokeMaterial;
        public float impulseThreshold;
        public float explosionForce;
        public float explosionRadius;
        public float upwardsModifier;
        public float invincibilityTimer;
        public float currentTimerValue;
        public float restartDelay;
        // Start is called before the first frame update
        void Start()
        {
            rB = GetComponent<Rigidbody>();
            currentTimerValue = invincibilityTimer;
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
                foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
                {
                    if (t.gameObject.GetComponent<Camera>() == null && t.gameObject != this.gameObject)
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

                        /*if (t.gameObject.GetComponent<ParticleSystem>() == null)
                        {
                            ParticleSystem parti = t.gameObject.AddComponent<ParticleSystem>();
                            smokeParticleSystemPreset.ApplyTo(parti);
                            ParticleSystemRenderer pRend = parti.GetComponent<ParticleSystemRenderer>();
                            pRend.material = smokeMaterial;
                            pRend.trailMaterial = smokeMaterial;

                        }*/
                        TrailRenderer tRend = t.gameObject.AddComponent<TrailRenderer>();
                        trailRendererPreset.ApplyTo(tRend);
                        tRend.material = smokeMaterial;
                    }
                }
                rB.gameObject.GetComponent<VehicleSystem>().enabled = false;
                
                Destroy(this.gameObject, restartDelay);

            }
  
        }

        private void OnDestroy()
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

