﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class UI : MonoBehaviour
{
    public float fuel;
    public float maxFuel;
    //public Text fuelText;
    private FuelComponent fuelComponent;

    private void Start()
    {
        fuelComponent = FindObjectOfType<FuelComponent>();
        //the below is getting a reference to the max fuel from the fuel component script
        maxFuel = fuelComponent.maxFuel;
        //fuelText.text = "Fuel: " + fuel.ToString("F2");
    }

    private void Update()
    {
        fuel = fuelComponent.currentFuel;
        //fuelText.text = "Fuel: " + fuel.ToString("F2");

        if (fuel >= 0)
        {
            //fuelText.text = "Fuel: Out Of Fuel";
        }
        if(fuel >= maxFuel)
        {
            fuel = maxFuel;
        }

    }

}
