
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Analytics;

namespace Students.Luca
{
    public class BazookaExplosiveProjectile : Projectile
    {
        public ParticleSystem explosionEffect;
        public ParticleSystem exhaustEffect;
        public float explosionForce = 10;
        public float explosionRadius = 10;

        private bool exploded = false;

        public float minExplosionTreshold = 3; // Can't explode within this Treshold; -1 to deactivate
        public float maxExplosionTreshold = 10; // Will explode after this Treshold; -1 to deactivate
        private float timePassed = 0;

        void Start()
        {
            Init();
        }

        void Update()
        {
            if(rb.velocity.magnitude > 0.1f)
                transform.rotation = Quaternion.LookRotation(rb.velocity);

            timePassed += Time.deltaTime;
            
            if(rb.velocity.magnitude > 0.1f && !windCuttingEffect.isPlaying)
                windCuttingEffect.Play();
            else if(rb.velocity.magnitude <= 0.1f && windCuttingEffect.isPlaying)
                windCuttingEffect.Stop();

            if (maxExplosionTreshold >= 0 && timePassed >= maxExplosionTreshold && !exploded)
            {
                StartCoroutine(Explode());
            }
        }
        
        private void OnEnable()
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = false;
            exploded = false;
            
            exhaustEffect?.Play();
            timePassed = 0;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (exploded && (minExplosionTreshold >= 0 && timePassed < minExplosionTreshold))
                return;
            rb.isKinematic = true;
            exhaustEffect.Stop();
            StartCoroutine(Explode());
        }

        IEnumerator Explode()
        {
            exploded = true;
            explosionEffect.Play();
            
            Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider col in nearbyColliders)
            {
                Rigidbody rb = col.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius,3f);
            }

            while (!explosionEffect.isStopped)
            {
                yield return new WaitForSeconds(0.2f);
            }
            
            ObjectPool.ReturnObject(gameObject);
        }
    }
}