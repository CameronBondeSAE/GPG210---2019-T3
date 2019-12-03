using Sirenix.OdinInspector;
using UnityEngine;

namespace Students.Luca.Scripts
{
    public class NoFuelThruster : InputReceiver/*, IIncDecreasable*/
    {
        protected AudioSource audioSource;

        [ShowInInspector, ReadOnly]
        private bool inputIncreaseForce = false;
        [ShowInInspector, ReadOnly]
        private bool inputDecreaseForce = false;
        public bool autoResetForce = false;

        public Rigidbody masterRb;
        public ParticleSystem exhaustBoostParticleSystem;
        public ParticleSystem exhaustDefaultParticleSystem;
        public Transform exhaustDirection;

        [ShowInInspector] bool turnedOn = false;

        [ShowInInspector]
        public bool TurnedOn
        {
            get => turnedOn;
            set
            {
                turnedOn = TurnOn(value);
            }
        }

        [ShowInInspector] float inputAddForce = 0; // 0-1

        public float InputAddForce
        {
            get => inputAddForce;
            set => inputAddForce = Mathf.Clamp(value, 0, 1);
        }

        public float maxForce = 100;

        [ShowInInspector]
        public float CurrentForce => (TurnedOn ? inputAddForce * maxForce : 0);

        public float fuelCostPerFs = 0.1f; // Cost per force/second

        public float CurrentFuelUsagePerSecond => CurrentForce * fuelCostPerFs;

        public float acceleration = 0.2f;

        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (masterRb == null)
            {
                masterRb = GetComponentInParent<Rigidbody>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (TurnedOn)
            {
                if (inputIncreaseForce)
                {
                    InputAddForce = Mathf.MoveTowards(InputAddForce, 1, acceleration);
                }
                else if ((inputDecreaseForce || autoResetForce) && InputAddForce > 0)
                {
                    InputAddForce = Mathf.MoveTowards(InputAddForce, 0, acceleration);
                }

                if (CurrentForce > 0)
                {
                    ApplyForce();
                    if (!exhaustBoostParticleSystem?.isPlaying ?? false)
                        exhaustBoostParticleSystem?.Play();
                }
            }

            if ((exhaustBoostParticleSystem?.isPlaying ?? false) && CurrentForce <= 0)
            {
                exhaustBoostParticleSystem?.Stop();
            }

            if (audioSource != null && audioSource.isPlaying && TurnedOn)
            {
                audioSource.volume = Mathf.Abs(CurrentForce)/maxForce;
                audioSource.pitch = Mathf.Clamp(1+Mathf.Abs(CurrentForce)/maxForce,1f, 2f);
            }
            
            inputIncreaseForce = false; // Hacky
            inputDecreaseForce = false; // Hacky
        }

        private void ApplyForce()
        {
            Vector3 finalForce = (-exhaustDirection?.forward ?? Vector3.up) * CurrentForce;
            Debug.DrawRay(transform.position, finalForce, Color.red);
            masterRb.AddForceAtPosition(finalForce, transform.position);
        }

        private bool TurnOn(bool turnOn)
        {
            if (turnOn == TurnedOn)
                return turnOn;


            if (turnOn)
            {
                exhaustDefaultParticleSystem?.Play();

                if(audioSource.clip != null)
                    audioSource.Play();
                
                return true;
            }
            else
            {
                exhaustDefaultParticleSystem?.Stop();
                exhaustBoostParticleSystem?.Stop();
                
                if(audioSource.isPlaying)
                    audioSource.Stop();
                
                return false;
            }
        }

        public override void LeftStickAxis(Vector2 value)
        {
            value = CalculateLSAValue(value);

            if (value.y > 0)
            {
                inputIncreaseForce = true;
                inputDecreaseForce = false;
            }
            else if (value.y < 0)
            {
                inputDecreaseForce = true;
                inputIncreaseForce = false;
            }
            else
            {
                inputIncreaseForce = false;
                inputDecreaseForce = false;
            }
        }

        public override void RightStickAxis(Vector2 value)
        {
            
        }

        public override void LeftTrigger(float value)
        {
            
        }

        public override void RightTrigger(float value)
        {
            
        }

        public override void Stop()
        {
            inputIncreaseForce = false;
            inputDecreaseForce = false;
        }

        public override float GetCurrentForceSecondValue()
        {
            return CurrentFuelUsagePerSecond;
        }
    }
}