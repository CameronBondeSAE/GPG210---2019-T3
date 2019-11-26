using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelComponent : MonoBehaviour
{
    public float currentFuel;
    public float maxFuel = 100f;

    public float fuelDrainRate = 1;

    //not each vehicle just references this bool and if its true then take no input
    public bool outOfFuel;

    public void Update()
    {
        if (currentFuel <= 0)
        {
            outOfFuel = true;
        }
        else
        {
            outOfFuel = false;
        }
    }

    //each vehicle references this function and calls it everytime there is input from player
    public void DrainFuel(float fuel)
    {
        currentFuel -= fuelDrainRate;
    }

    public bool OutOfFuel()
    {
        return currentFuel <= 0;
    }
}
