using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Students.Luca
{
    public class Projectile : MonoBehaviour
    {
        public Rigidbody rb;
        public Transform exhausPosition;
        public Transform centerOfMass;
        public ParticleSystem windCuttingEffect;

        [ShowInInspector]
        protected bool inactive = true;

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
                rb.centerOfMass = centerOfMass.localPosition;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (rb.velocity.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.LookRotation(rb.velocity);
            }
                
        }

        private void OnCollisionEnter(Collision other)
        {
            if (inactive)
                return;

            windCuttingEffect.Stop();
            Debug.Log("Boom Collision!");
        }

        public virtual void SetProjectileActive(bool status)
        {
            inactive = !status;
        }

        public bool GetInactive()
        {
            return inactive;
        }
    }
}