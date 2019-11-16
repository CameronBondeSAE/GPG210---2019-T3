using Sirenix.OdinInspector;
using UnityEngine;

namespace Students.Luca.Scripts
{
    public class Thruster : InputReceiver/*, IIncDecreasable*/
    {
        protected AudioSource audioSource;

        public bool inputIncreaseForce = false;
        public bool inputDecreaseForce = false;
        public bool autoResetForce = false;
        
        public KeyCode igniteKey; // HACK

        public Rigidbody masterRb;
        public ParticleSystem exhaustBoostParticleSystem;
        public ParticleSystem exhaustDefaultParticleSystem;
        public Transform exhaustDirection;

        [ShowInInspector] bool turnedOn = false;

        public bool TurnedOn
        {
            get => turnedOn;
            set
            {
                turnedOn = TurnOn(value);
                ;
            }
        }

        [ShowInInspector] float inputAddForce = 0; // 0-1

        public float InputAddForce
        {
            get => inputAddForce;
            set => inputAddForce = Mathf.Clamp(value, 0, 1);
        }

        public float maxForce = 100;

        public float CurrentForce => (TurnedOn ? inputAddForce * maxForce : 0);

        public float maxFuel = 100;

        [ShowInInspector, SerializeField] float currentFuel = 100;

        public float CurrentFuel
        {
            get => currentFuel;
            set => currentFuel = (value < 0 ? 0 : (value >= maxFuel ? maxFuel : value));
        }

        public float fuelCostPerFs = 0.1f; // Cost per force/second

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
            //Input Hack
            if (Input.GetKeyDown(igniteKey))
            {
                TurnedOn = !TurnedOn;
            }

            

            if (TurnedOn)
            {
                if (inputIncreaseForce)
                {
                    InputAddForce += Time.deltaTime / 2;
                }
                else if ((inputDecreaseForce || autoResetForce) && InputAddForce > 0)
                {
                    InputAddForce -= Time.deltaTime;
                }
                
                if (currentFuel <= 0)
                    TurnedOn = false;

                DrainFuel();

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

        void DrainFuel()
        {
            if (currentFuel <= 0)
                return;
            float fuelDrain = Mathf.Abs(CurrentForce) * fuelCostPerFs * Time.deltaTime;
            currentFuel -= fuelDrain;
        }

        private bool TurnOn(bool turnOn)
        {
            if (turnOn == TurnedOn)
                return turnOn;


            if (turnOn)
            {
                if (currentFuel <= 0)
                    return false;

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

        public void IncreaseValue()
        {
            inputIncreaseForce = true;
        }

        public void DecreaseValue()
        {
            inputDecreaseForce = true;
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
            throw new System.NotImplementedException();
        }
    }
}