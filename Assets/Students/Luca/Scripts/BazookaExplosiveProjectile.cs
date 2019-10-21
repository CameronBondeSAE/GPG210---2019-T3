
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


        void Start()
        {
            Init();
        }
        
        private void OnEnable()
        {
            rb.velocity = Vector3.zero;
            exploded = false;
            windCuttingEffect?.Play();
            exhaustEffect?.Play();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (exploded)
                return;

            windCuttingEffect.Stop();
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