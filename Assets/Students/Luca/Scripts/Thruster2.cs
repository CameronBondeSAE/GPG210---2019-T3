using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class Thruster2 : MonoBehaviour
{
    public KeyCode igniteKey; // HACK
    public KeyCode addForceKey; // HACK
    
    public Rigidbody masterRb;
    public ParticleSystem exhaustBoostParticleSystem;
    public ParticleSystem exhaustDefaultParticleSystem;
    public Transform exhaustDirection;

    [ShowInInspector]
    bool turnedOn = false;
    public bool TurnedOn
    {
        get => turnedOn;
        set
        {
            turnedOn = TurnOn(value);;
        }
    }

    [ShowInInspector]
    float inputAddForce = 0; // 0-1
    public float InputAddForce
    {
        get => inputAddForce;
        set => inputAddForce = Mathf.Clamp((value%1)/1,0,1);
    }

    public float maxForce = 100;

    public float CurrentForce => (TurnedOn ? inputAddForce * maxForce : 0);

    public float maxFuel = 100;
    
    [ShowInInspector, SerializeField]
    float currentFuel = 100;
    public float CurrentFuel
    {
        get => currentFuel;
        set => currentFuel = (value < 0?0:(value >= maxFuel?maxFuel:value));
    }

    public float fuelCostPerFs = 0.1f; // Cost per force/second

    // Start is called before the first frame update
    void Start()
    {
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

        if (Input.GetKey(addForceKey))
        {
            InputAddForce += Time.deltaTime/2;
        }else if (InputAddForce > 0)
        {
            InputAddForce -= Time.deltaTime;
        }
        
        if (TurnedOn)
        {
            if (currentFuel <= 0)
                TurnedOn = false;
            
            DrainFuel();

            if (CurrentForce > 0)
            {
                ApplyForce();
                if(!exhaustBoostParticleSystem?.isPlaying ?? false)
                    exhaustBoostParticleSystem?.Play();
            }
        }
        
        if ((exhaustBoostParticleSystem?.isPlaying ?? false) && CurrentForce <= 0)
        {
            exhaustBoostParticleSystem?.Stop();
        }
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
        if(turnOn == TurnedOn)
            return turnOn;
        
        
        if (turnOn)
        {
            if (currentFuel <= 0)
                return false;
            
            exhaustDefaultParticleSystem?.Play();
            
            return true;
        }
        else
        {
            exhaustDefaultParticleSystem?.Stop();
            exhaustBoostParticleSystem?.Stop();
            return false;
        }
    }
}
