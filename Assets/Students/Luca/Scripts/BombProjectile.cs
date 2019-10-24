using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Students.Luca
{
    public class BombProjectile : Projectile
    {
        public ParticleSystem explosionEffect;
        public float explosionForce = 20;
        public float explosionRadius = 20;

        private bool exploded = false;

        public float minExplosionTreshold = 5; // Can't explode within this Treshold; -1 to deactivate
        public float maxExplosionTreshold = -1; // Will explode after this Treshold; -1 to deactivate
        private float timePassed = 0;

        void Start()
        {
            Init();
        }

        void Update()
        {
            /*if(rb.velocity.magnitude > 0.1f)
                transform.rotation = Quaternion.LookRotation(rb.velocity);*/

            timePassed += Time.deltaTime;
            
            if(rb.velocity.magnitude > 4f && !windCuttingEffect.isPlaying)
                windCuttingEffect.Play();
            else if(rb.velocity.magnitude <= 4f && windCuttingEffect.isPlaying)
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
            
            timePassed = 0;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (inactive || ( exploded && (minExplosionTreshold >= 0 && timePassed < minExplosionTreshold)))
                return;
            
            rb.isKinematic = true;
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